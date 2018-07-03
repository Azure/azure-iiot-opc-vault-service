﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Opc.Ua;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.Test
{

    public class X509TestUtils
    {
        public static void VerifyApplicationCertIntegrity(
            X509Certificate2 newCert, 
            byte[] privateKey, 
            string privateKeyPassword, 
            string privateKeyFormat,
            X509Certificate2Collection issuerCertificates)
        {
            Assert.NotNull(newCert);
            if (privateKey != null)
            {
                X509Certificate2 newPrivateKeyCert = null;
                if (privateKeyFormat == "PFX")
                {
                    newPrivateKeyCert = CertificateFactory.CreateCertificateFromPKCS12(privateKey, privateKeyPassword);
                }
                else if (privateKeyFormat == "PEM")
                {
                    newPrivateKeyCert = CertificateFactory.CreateCertificateWithPEMPrivateKey(newCert, privateKey, privateKeyPassword);
                }
                else
                {
                    Assert.True(false, "Invalid private key format");
                }
                Assert.NotNull(newPrivateKeyCert);
                // verify the public cert matches the private key
                Assert.True(CertificateFactory.VerifyRSAKeyPair(newCert, newPrivateKeyCert, true));
                Assert.True(CertificateFactory.VerifyRSAKeyPair(newPrivateKeyCert, newPrivateKeyCert, true));
            }

            CertificateIdentifierCollection issuerCertIdCollection = new CertificateIdentifierCollection();
            foreach (var issuerCert in issuerCertificates)
            {
                issuerCertIdCollection.Add(new CertificateIdentifier(issuerCert));
            }

            // verify cert with issuer chain
            CertificateValidator certValidator = new CertificateValidator();
            CertificateTrustList issuerStore = new CertificateTrustList();
            CertificateTrustList trustedStore = new CertificateTrustList();
            trustedStore.TrustedCertificates = issuerCertIdCollection;
            certValidator.Update(trustedStore, issuerStore, null);
            Assert.Throws<Opc.Ua.ServiceResultException>(() =>
            {
                certValidator.Validate(newCert);
            });
            issuerStore.TrustedCertificates = issuerCertIdCollection;
            certValidator.Update(issuerStore, trustedStore, null);
            certValidator.Validate(newCert);
        }

        public static void VerifySignedApplicationCert(ApplicationTestData testApp, X509Certificate2 signedCert, X509Certificate2Collection issuerCerts)
        {
            X509Certificate2 issuerCert = issuerCerts[0];

            Assert.NotNull(signedCert);
            Assert.False(signedCert.HasPrivateKey);
            Assert.True(Opc.Ua.Utils.CompareDistinguishedName(testApp.Subject, signedCert.Subject));
            Assert.False(Opc.Ua.Utils.CompareDistinguishedName(signedCert.Issuer, signedCert.Subject));
            Assert.True(Opc.Ua.Utils.CompareDistinguishedName(signedCert.Issuer, issuerCert.Subject));

            // test basic constraints
            var constraints = FindBasicConstraintsExtension(signedCert);
            Assert.NotNull(constraints);
            Assert.True(constraints.Critical);
            Assert.False(constraints.CertificateAuthority);
            Assert.False(constraints.HasPathLengthConstraint);

            // key usage
            var keyUsage = FindKeyUsageExtension(signedCert);
            Assert.NotNull(keyUsage);
            Assert.True(keyUsage.Critical);
            Assert.True((keyUsage.KeyUsages & X509KeyUsageFlags.CrlSign) == 0);
            Assert.True((keyUsage.KeyUsages & X509KeyUsageFlags.DataEncipherment) == X509KeyUsageFlags.DataEncipherment);
            Assert.True((keyUsage.KeyUsages & X509KeyUsageFlags.DecipherOnly) == 0);
            Assert.True((keyUsage.KeyUsages & X509KeyUsageFlags.DigitalSignature) == X509KeyUsageFlags.DigitalSignature);
            Assert.True((keyUsage.KeyUsages & X509KeyUsageFlags.EncipherOnly) == 0);
            Assert.True((keyUsage.KeyUsages & X509KeyUsageFlags.KeyAgreement) == 0);
            Assert.True((keyUsage.KeyUsages & X509KeyUsageFlags.KeyCertSign) == 0);
            Assert.True((keyUsage.KeyUsages & X509KeyUsageFlags.KeyEncipherment) == X509KeyUsageFlags.KeyEncipherment);
            Assert.True((keyUsage.KeyUsages & X509KeyUsageFlags.NonRepudiation) == X509KeyUsageFlags.NonRepudiation);

            // enhanced key usage
            var enhancedKeyUsage = FindEnhancedKeyUsageExtension(signedCert);
            Assert.NotNull(enhancedKeyUsage);
            Assert.True(enhancedKeyUsage.Critical);

            // test for authority key
            X509AuthorityKeyIdentifierExtension authority = FindAuthorityKeyIdentifier(signedCert);
            Assert.NotNull(authority);
            Assert.NotNull(authority.SerialNumber);
            Assert.NotNull(authority.KeyId);
            Assert.NotNull(authority.AuthorityNames);

            // verify authority key in signed cert
            X509SubjectKeyIdentifierExtension subjectKeyId = FindSubjectKeyIdentifierExtension(issuerCert);
            Assert.Equal(subjectKeyId.SubjectKeyIdentifier, authority.KeyId);
            Assert.Equal(issuerCert.SerialNumber, authority.SerialNumber);

            X509SubjectAltNameExtension subjectAlternateName = FindSubjectAltName(signedCert);
            Assert.NotNull(subjectAlternateName);
            Assert.False(subjectAlternateName.Critical);
            var domainNames = Opc.Ua.Utils.GetDomainsFromCertficate(signedCert);
            foreach (var domainName in testApp.DomainNames)
            {
                Assert.True(domainNames.Contains(domainName, StringComparer.OrdinalIgnoreCase));
            }
            Assert.True(subjectAlternateName.Uris.Count == 1);
            var applicationUri = Opc.Ua.Utils.GetApplicationUriFromCertificate(signedCert);
            Assert.True(testApp.ApplicationRecord.ApplicationUri == applicationUri);
        }

        internal static X509BasicConstraintsExtension FindBasicConstraintsExtension(X509Certificate2 certificate)
        {
            for (int ii = 0; ii < certificate.Extensions.Count; ii++)
            {
                X509BasicConstraintsExtension extension = certificate.Extensions[ii] as X509BasicConstraintsExtension;
                if (extension != null)
                {
                    return extension;
                }
            }
            return null;
        }

        internal static X509KeyUsageExtension FindKeyUsageExtension(X509Certificate2 certificate)
        {
            for (int ii = 0; ii < certificate.Extensions.Count; ii++)
            {
                X509KeyUsageExtension extension = certificate.Extensions[ii] as X509KeyUsageExtension;
                if (extension != null)
                {
                    return extension;
                }
            }
            return null;
        }
        internal static X509EnhancedKeyUsageExtension FindEnhancedKeyUsageExtension(X509Certificate2 certificate)
        {
            for (int ii = 0; ii < certificate.Extensions.Count; ii++)
            {
                X509EnhancedKeyUsageExtension extension = certificate.Extensions[ii] as X509EnhancedKeyUsageExtension;
                if (extension != null)
                {
                    return extension;
                }
            }
            return null;
        }

        internal static X509AuthorityKeyIdentifierExtension FindAuthorityKeyIdentifier(X509Certificate2 certificate)
        {
            for (int ii = 0; ii < certificate.Extensions.Count; ii++)
            {
                X509Extension extension = certificate.Extensions[ii];

                switch (extension.Oid.Value)
                {
                    case X509AuthorityKeyIdentifierExtension.AuthorityKeyIdentifierOid:
                    case X509AuthorityKeyIdentifierExtension.AuthorityKeyIdentifier2Oid:
                        {
                            return new X509AuthorityKeyIdentifierExtension(extension, extension.Critical);
                        }
                }
            }

            return null;
        }

        internal static X509SubjectAltNameExtension FindSubjectAltName(X509Certificate2 certificate)
        {
            foreach (var extension in certificate.Extensions)
            {
                if (extension.Oid.Value == X509SubjectAltNameExtension.SubjectAltNameOid ||
                    extension.Oid.Value == X509SubjectAltNameExtension.SubjectAltName2Oid)
                {
                    return new X509SubjectAltNameExtension(extension, extension.Critical);
                }
            }
            return null;
        }

        internal static X509SubjectKeyIdentifierExtension FindSubjectKeyIdentifierExtension(X509Certificate2 certificate)
        {
            for (int ii = 0; ii < certificate.Extensions.Count; ii++)
            {
                X509SubjectKeyIdentifierExtension extension = certificate.Extensions[ii] as X509SubjectKeyIdentifierExtension;
                if (extension != null)
                {
                    return extension;
                }
            }
            return null;
        }


    }

}