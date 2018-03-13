// Copyright (c) Microsoft. All rights reserved.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IoTSolutions.GdsVault.Services;
using Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Filters;
using Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Models;

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Controllers
{
    [Route(Version.Path + "/groups"), TypeFilter(typeof(ExceptionsFilterAttribute))]
    public sealed class CertificateGroupsController : Controller
    {
        private readonly ICertificateGroups certificateGroups;

        public CertificateGroupsController(ICertificateGroups certificateGroups)
        {
            this.certificateGroups = certificateGroups;
        }

        /// <returns>List of certificate groups</returns>
        [HttpGet]
        public async Task<CertificateGroupListApiModel> GetAsync()
        {
            return new CertificateGroupListApiModel(await this.certificateGroups.GetCertificateGroupIds());
        }

        /// <summary>Get one device</summary>
        /// <param name="id">Device Id</param>
        [HttpGet("{id}")]
        public async Task<CertificateGroupConfigurationApiModel> GetAsync(string id)
        {
            return new CertificateGroupConfigurationApiModel(id, await this.certificateGroups.GetCertificateGroupConfiguration(id));
        }

        /// <summary>Create new CA Certificate</summary>
        [HttpGet("create/{id}")]
        public async Task<X509Certificate2ApiModel> GetCACertificateAsync(string id)
        {
            return new X509Certificate2ApiModel(await this.certificateGroups.CreateCACertificateAsync(id));
        }

        /// <summary>Create new CA Certificate</summary>
        [HttpPost("create/{id}")]
        public async Task<X509Certificate2ApiModel> PostCreateAsync(string id)
        {
            return new X509Certificate2ApiModel(await this.certificateGroups.CreateCACertificateAsync(id));
        }

        /// <summary>Revoke Certificate</summary>
        [HttpPost("revoke/{id}")]
        public async Task<X509CRLApiModel> PostRevokeAsync(string id, [FromBody] X509Certificate2ApiModel cert)
        {
            return new X509CRLApiModel(await this.certificateGroups.RevokeCertificateAsync(id, cert.ToServiceModel()));
        }

        /// <summary>Revoke Certificate</summary>
        [HttpPost("sign/{id}")]
        public async Task<X509Certificate2ApiModel> PostSignAsync(string id, [FromBody] SigningRequestApiModel sr)
        {
            return new X509Certificate2ApiModel(await this.certificateGroups.SigningRequestAsync(id, sr.ApplicationURI, sr.ToServiceModel()));
        }

        /// <summary>Revoke Certificate</summary>
        [HttpPost("newkey/{id}")]
        public async Task<PfxCertificateApiModel> PostNewKeyAsync(string id, [FromBody] NewKeyPairRequestApiModel nkpr)
        {
            return new PfxCertificateApiModel(await this.certificateGroups.NewKeyPairRequestAsync(id, nkpr.ApplicationURI, nkpr.SubjectName, nkpr.DomainNames));
        }

    }
}
