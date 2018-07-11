// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.v1.Filters;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.v1.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.v1.Controllers
{
    [Route(VersionInfo.PATH + "/request"), TypeFilter(typeof(ExceptionsFilterAttribute))]
    [Produces("application/json")]
    public sealed class CertificateRequestController : Controller
    {
        private readonly ICertificateRequest _certificateRequest;

        public CertificateRequestController(ICertificateRequest certificateRequest)
        {
            _certificateRequest = certificateRequest;
        }

        /// <summary>
        /// Create a new signing request.
        /// </summary>
        [HttpPost("sign")]
        [SwaggerOperation(operationId: "CreateSigningRequest")]
        public async Task<string> CreateSigningRequestAsync([FromBody] CreateSigningRequestApiModel signingRequest)
        {
            if (signingRequest == null)
            {
                throw new ArgumentNullException(nameof(signingRequest));
            }
            return await _certificateRequest.CreateSigningRequestAsync(
                signingRequest.ApplicationId,
                signingRequest.CertificateGroupId,
                signingRequest.CertificateTypeId,
                signingRequest.ToServiceModel(),
                signingRequest.AuthorityId);
        }

        /// <summary>
        /// Create a new key pair request.
        /// </summary>
        [HttpPost("keypair")]
        [SwaggerOperation(operationId: "CreateNewKeyPairRequest")]
        public async Task<string> CreateNewKeyPairRequestAsync([FromBody] CreateNewKeyPairRequestApiModel newKeyPairRequest)
        {
            if (newKeyPairRequest == null)
            {
                throw new ArgumentNullException(nameof(newKeyPairRequest));
            }
            return await _certificateRequest.CreateNewKeyPairAsync(
                newKeyPairRequest.ApplicationId,
                newKeyPairRequest.CertificateGroupId,
                newKeyPairRequest.CertificateTypeId,
                newKeyPairRequest.SubjectName,
                newKeyPairRequest.DomainNames,
                newKeyPairRequest.PrivateKeyFormat,
                newKeyPairRequest.PrivateKeyPassword,
                newKeyPairRequest.AuthorityId);
        }

        /// <summary>
        /// Approve request.
        /// </summary>
        [HttpPost("{requestId}/approve/{rejected}")]
        [SwaggerOperation(operationId: "ApproveCertificateRequest")]
        public async Task ApproveCertificateRequestAsync(string requestId, bool rejected)
        {
            await _certificateRequest.ApproveAsync(requestId, rejected);
        }

        /// <summary>
        /// Accept request.
        /// </summary>
        [HttpPost("{requestId}/accept")]
        [SwaggerOperation(operationId: "AcceptCertificateRequest")]
        public async Task AcceptCertificateRequestAsync(string requestId)
        {
            await _certificateRequest.AcceptAsync(requestId);
        }

        /// <summary>Read certificate request</summary>
        [HttpGet("{requestId}")]
        [SwaggerOperation(operationId: "ReadCertificateRequest")]
        public async Task<ReadRequestApiModel> ReadCertificateRequestAsync(string requestId)
        {
            var result = await _certificateRequest.ReadAsync(requestId);
            return new ReadRequestApiModel(
                result.State,
                result.ApplicationId,
                requestId,
                result.CertificateGroupId,
                result.CertificateTypeId,
                result.CertificateRequest,
                result.SubjectName,
                result.DomainNames,
                result.PrivateKeyFormat,
                result.PrivateKeyPassword);
        }

        /// <summary>Complete certificate request</summary>
        [HttpPost("{requestId}/{applicationId}/complete")]
        [SwaggerOperation(operationId: "CompleteCertificateRequest")]
        public async Task<CompleteRequestApiModel> CompleteCertificateRequestAsync(string requestId, string applicationId)
        {
            var result = await _certificateRequest.CompleteAsync(
                applicationId,
                requestId
                );
            return new CompleteRequestApiModel(
                result.State,
                applicationId,
                requestId,
                result.CertificateGroupId,
                result.CertificateTypeId,
                result.SignedCertificate,
                result.PrivateKey
                );
        }

    }
}
