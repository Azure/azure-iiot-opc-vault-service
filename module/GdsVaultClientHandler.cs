﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Opc.Ua.Gds.Server.GdsVault
{
    public class GdsVaultClientHandler
    {
        private IOpcGdsVault _gdsServiceClient;
        public GdsVaultClientHandler(IOpcGdsVault gdsServiceClient)
        {
            _gdsServiceClient = gdsServiceClient;
        }

        public async Task<X509Certificate2Collection> GetCACertificateChainAsync(string id)
        {
            var result = new X509Certificate2Collection();
            var chainApiModel = await _gdsServiceClient.GetCACertificateChainAsync(id).ConfigureAwait(false);
            foreach (var certApiModel in chainApiModel.Chain)
            {
                var cert = new X509Certificate2(Convert.FromBase64String(certApiModel.Certificate));
                result.Add(cert);
            }
            return result;
        }

        public async Task<IList<Opc.Ua.X509CRL>> GetCACrlChainAsync(string id)
        {
            var result = new List<Opc.Ua.X509CRL>();
            var chainApiModel = await _gdsServiceClient.GetCACrlChainAsync(id).ConfigureAwait(false);
            foreach (var certApiModel in chainApiModel.Chain)
            {
                var crl = new Opc.Ua.X509CRL(Convert.FromBase64String(certApiModel.Crl));
                result.Add(crl);
            }
            return result;
        }

        public async Task<CertificateGroupConfigurationCollection> GetCertificateConfigurationGroupsAsync(string baseStorePath)
        {
            var groups = await _gdsServiceClient.GetCertificateGroupConfigurationCollectionAsync().ConfigureAwait(false);
            var groupCollection = new CertificateGroupConfigurationCollection();
            foreach (var group in groups.Groups)
            {
                var newGroup = new CertificateGroupConfiguration()
                {
                    Id = group.Name,
                    CertificateType = group.CertificateType,
                    SubjectName = group.SubjectName,
                    BaseStorePath = baseStorePath + Path.DirectorySeparatorChar + group.Name,
                    DefaultCertificateHashSize = (ushort)group.DefaultCertificateHashSize,
                    DefaultCertificateKeySize = (ushort)group.DefaultCertificateKeySize,
                    DefaultCertificateLifetime = (ushort)group.DefaultCertificateLifetime,
                    CACertificateHashSize = (ushort)group.CACertificateHashSize,
                    CACertificateKeySize = (ushort)group.CACertificateKeySize,
                    CACertificateLifetime = (ushort)group.CACertificateLifetime
                };
                groupCollection.Add(newGroup);
            }
            return groupCollection;
        }

        public async Task<X509Certificate2> SigningRequestAsync(
            string id,
            ApplicationRecordDataType application,
            byte[] certificateRequest)
        {
            var sr = new SigningRequestApiModel()
            {
                ApplicationURI = application.ApplicationUri,
                Csr = Convert.ToBase64String(certificateRequest)
            };
            var certModel = await _gdsServiceClient.SigningRequestAsync(id, sr).ConfigureAwait(false);
            return new X509Certificate2(Convert.FromBase64String(certModel.Certificate));
        }

        public async Task<Opc.Ua.X509CRL> RevokeCertificateAsync(
            string id,
            X509Certificate2 certificate)
        {
            var certModel = new X509Certificate2ApiModel()
            {
                Certificate = Convert.ToBase64String(certificate.RawData),
                Subject = certificate.Subject,
                Thumbprint = certificate.Thumbprint
            };
            var crlModel = await _gdsServiceClient.RevokeCertificateAsync(id, certModel).ConfigureAwait(false);
            return new Opc.Ua.X509CRL(Convert.FromBase64String(crlModel.Crl));
        }

        public async Task<X509Certificate2KeyPair> NewKeyPairRequestAsync(
            string id,
            ApplicationRecordDataType application,
            string subjectName,
            string[] domainNames,
            string privateKeyFormat,
            string privateKeyPassword)
        {
            var certModel = new NewKeyPairRequestApiModel()
            {
                ApplicationURI = application.ApplicationUri,
                SubjectName = subjectName,
                DomainNames = domainNames,
                PrivateKeyFormat = privateKeyFormat,
                PrivateKeyPassword = privateKeyPassword
            };
            var nkpModel = await _gdsServiceClient.NewKeyPairRequestAsync(id, certModel).ConfigureAwait(false);
            return new X509Certificate2KeyPair(
                new X509Certificate2(Convert.FromBase64String(nkpModel.Certificate)),
                nkpModel.PrivateKeyFormat,
                Convert.FromBase64String(nkpModel.PrivateKey));
        }
    }
}
