// Copyright (c) Microsoft. All rights reserved.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IoTSolutions.OpcGdsVault.Services;
using Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.v1.Filters;
using Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.v1.Models;

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.v1.Controllers
{
    [Route(ServiceInfo.PATH + "/groups"), TypeFilter(typeof(ExceptionsFilterAttribute))]
    [Produces("application/json")]

    public sealed class CertificateGroupController : Controller
    {
        private readonly ICertificateGroup certificateGroups;

        public CertificateGroupController(ICertificateGroup certificateGroups)
        {
            this.certificateGroups = certificateGroups;
        }

        /// <returns>List of certificate groups</returns>
        [HttpGet]
        public async Task<CertificateGroupListApiModel> GetAsync()
        {
            return new CertificateGroupListApiModel(await this.certificateGroups.GetCertificateGroupIds());
        }

        /// <summary>Get group configuration</summary>
        [HttpGet("{id}")]
        public async Task<CertificateGroupConfigurationApiModel> GetAsync(string id)
        {
            return new CertificateGroupConfigurationApiModel(
                id,
                await this.certificateGroups.GetCertificateGroupConfiguration(id));
        }

        /// <summary>Get group configuration</summary>
        [HttpGet("config")]
        public async Task<CertificateGroupConfigurationCollectionApiModel> GetConfigAsync()
        {
            return new CertificateGroupConfigurationCollectionApiModel(
                await this.certificateGroups.GetCertificateGroupConfigurationCollection());
        }

        /// <summary>Get group configuration</summary>
        [HttpGet("iothub")]
        public async Task<KeyVaultSecretApiModel> GetIotHubAsync(string id)
        {
            return new KeyVaultSecretApiModel(await this.certificateGroups.GetIotHubSecretAsync().ConfigureAwait(false));
        }

        /// <summary>Get CA Certificate chain</summary>
        [HttpGet("cacert/{id}")]
        public async Task<X509Certificate2CollectionApiModel> GetCACertificateChainAsync(string id)
        {
            return new X509Certificate2CollectionApiModel(
                await this.certificateGroups.GetCACertificateChainAsync(id));
        }

        /// <summary>Get CA CRL chain</summary>
        [HttpGet("cacrl/{id}")]
        public async Task<X509CrlCollectionApiModel> GetCACrlChainAsync(string id)
        {
            return new X509CrlCollectionApiModel(
                await this.certificateGroups.GetCACrlChainAsync(id));
        }

        /// <summary>Create new CA Certificate</summary>
        [HttpGet("create/{id}")]
        public async Task<X509Certificate2ApiModel> GetCACertificateAsync(string id)
        {
            return new X509Certificate2ApiModel(
                await this.certificateGroups.CreateCACertificateAsync(id));
        }

        /// <summary>Create new CA Certificate</summary>
        [HttpPost("create/{id}")]
        public async Task<X509Certificate2ApiModel> PostCreateAsync(string id)
        {
            return new X509Certificate2ApiModel(
                await this.certificateGroups.CreateCACertificateAsync(id));
        }

        /// <summary>Revoke Certificate</summary>
        [HttpPost("revoke/{id}")]
        public async Task<X509CrlApiModel> PostRevokeAsync(string id, [FromBody] X509Certificate2ApiModel cert)
        {
            return new X509CrlApiModel(
                await this.certificateGroups.RevokeCertificateAsync(
                    id,
                    cert.ToServiceModel()));
        }

        /// <summary>Certificate Signing Request</summary>
        [HttpPost("sign/{id}")]
        public async Task<X509Certificate2ApiModel> PostSignAsync(string id, [FromBody] SigningRequestApiModel sr)
        {
            return new X509Certificate2ApiModel(
                await this.certificateGroups.SigningRequestAsync(
                    id,
                    sr.ApplicationURI,
                    sr.ToServiceModel()));
        }

        /// <summary>Create New Key Pair</summary>
        [HttpPost("newkey/{id}")]
        public async Task<CertificateKeyPairApiModel> PostNewKeyAsync(string id, [FromBody] NewKeyPairRequestApiModel nkpr)
        {
            return new CertificateKeyPairApiModel(
                await this.certificateGroups.NewKeyPairRequestAsync(
                    id,
                    nkpr.ApplicationURI,
                    nkpr.SubjectName,
                    nkpr.DomainNames,
                    nkpr.PrivateKeyFormat,
                    nkpr.PrivateKeyPassword));
        }

    }
}
