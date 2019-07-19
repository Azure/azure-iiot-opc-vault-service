// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//


namespace Microsoft.Azure.IIoT.WebApps.OpcUa.Vault.Utils {
    using System;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;

    public static class Utils {
        public static string CertFileName(string signedCertificate) {
            try {
                var signedCertByteArray = Convert.FromBase64String(signedCertificate);
                var cert = new X509Certificate2(signedCertByteArray);
                var dn = Opc.Ua.Utils.ParseDistinguishedName(cert.Subject);
                var prefix = dn.Where(x => x.StartsWith("CN=", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Substring(3);
                return prefix + " [" + cert.Thumbprint + "]";
            }
            catch {
                return "Certificate";
            }
        }
    }
}
