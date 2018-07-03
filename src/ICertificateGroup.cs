// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.IoTSolutions.GdsVault.Services.Models;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Microsoft.Azure.IoTSolutions.GdsVault.Services
{
    public interface ICertificateGroup
    {
        Task<string[]> GetCertificateGroupIds();
        Task<Opc.Ua.Gds.Server.CertificateGroupConfiguration> GetCertificateGroupConfiguration(string id);
        Task<Opc.Ua.Gds.Server.CertificateGroupConfigurationCollection> GetCertificateGroupConfigurationCollection();
        Task<X509Certificate2Collection> GetCACertificateChainAsync(string id);
        Task<IList<Opc.Ua.X509CRL>> GetCACrlChainAsync(string id);
        Task<KeyVaultTrustList> GetTrustListAsync(string id);

        Task<X509Certificate2> SigningRequestAsync(
            string id,
            string applicationUri,
            byte[] certificateRequest
            );
        Task<Opc.Ua.X509CRL> RevokeCertificateAsync(
            string id,
            X509Certificate2 certificate
            );
        Task<X509Certificate2> CreateCACertificateAsync(
            string id
            );
        Task<Opc.Ua.Gds.Server.X509Certificate2KeyPair> NewKeyPairRequestAsync(
            string id,
            string applicationUri,
            string subjectName,
            string[] domainNames,
            string privateKeyFormat,
            string privateKeyPassword
            );
#if IOTHUB
        Task<string> GetIotHubSecretAsync();
#endif
    }
}
