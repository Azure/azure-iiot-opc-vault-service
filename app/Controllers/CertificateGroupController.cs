// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.App.Filters;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.App.TokenStorage;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.App.Utils;
using Microsoft.Rest;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.App.Controllers
{
    [Authorize]
    [ExceptionsFilterAttribute]

    public class CertificateGroupController : Controller
    {
        // see RFC 2585
        const string ContentTypeCert = "application/pkix-cert";
        const string ContentTypeCrl = "application/pkix-crl";
        // see CertificateContentType.Pfx
        const string ContentTypePfx = "application/x-pkcs12";
        // see CertificateContentType.Pem
        const string ContentTypePem = "application/x-pem-file";

        private IOpcGdsVault gdsVault;
        private readonly GdsVaultOptions gdsVaultOptions;
        private readonly AzureADOptions azureADOptions;
        private readonly ITokenCacheService tokenCacheService;

        public CertificateGroupController(
            GdsVaultOptions gdsVaultOptions,
            AzureADOptions azureADOptions,
            ITokenCacheService tokenCacheService)
        {
            this.gdsVaultOptions = gdsVaultOptions;
            this.azureADOptions = azureADOptions;
            this.tokenCacheService = tokenCacheService;
        }

        [ActionName("Index")]
        public async Task<ActionResult> IndexAsync()
        {
            AuthorizeGdsVaultClient();
            var requests = await gdsVault.GetCertificateGroupConfigurationCollectionAsync();
            return View(requests.Groups);
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            AuthorizeGdsVaultClient();
            var request = await gdsVault.GetCertificateGroupConfigurationAsync(id);
            return View(request);
        }

        [ActionName("DownloadIssuer")]
        public async Task<ActionResult> DownloadIssuerAsync(string requestId)
        {
            AuthorizeGdsVaultClient();
            var request = await gdsVault.ReadCertificateRequestAsync(requestId);
            if (request != null)
            {
                var issuer = await gdsVault.GetCACertificateChainAsync(request.CertificateGroupId);
                var byteArray = Convert.FromBase64String(issuer.Chain[0].Certificate);
                return new FileContentResult(byteArray, ContentTypeCert)
                {
                    FileDownloadName = CertFileName(issuer.Chain[0].Certificate) + ".der"
                };
            }
            return new NotFoundResult();
        }

        [ActionName("DownloadIssuerCrl")]
        public async Task<ActionResult> DownloadIssuerCrlAsync(string requestId)
        {
            AuthorizeGdsVaultClient();
            var request = await gdsVault.ReadCertificateRequestAsync(requestId);
            if (request != null)
            {
                var issuer = await gdsVault.GetCACertificateChainAsync(request.CertificateGroupId);
                var crl = await gdsVault.GetCACrlChainAsync(request.CertificateGroupId);
                var byteArray = Convert.FromBase64String(crl.Chain[0].Crl);
                return new FileContentResult(byteArray, ContentTypeCrl)
                {
                    FileDownloadName = CertFileName(issuer.Chain[0].Certificate) + ".crl"
                };
            }
            return new NotFoundResult();
        }

        private string CertFileName(string signedCertificate)
        {
            try
            {
                var signedCertByteArray = Convert.FromBase64String(signedCertificate);
                X509Certificate2 cert = new X509Certificate2(signedCertByteArray);
                return cert.Subject + "[" + cert.Thumbprint + "]";
            }
            catch
            {
                return "Certificate";
            }
        }

        private void AuthorizeGdsVaultClient()
        {
            if (gdsVault == null)
            {
                ServiceClientCredentials serviceClientCredentials =
                    new GdsVaultLoginCredentials(gdsVaultOptions, azureADOptions, tokenCacheService, User);
                gdsVault = new OpcGdsVault(new Uri(gdsVaultOptions.BaseAddress), serviceClientCredentials);
            }
        }

    }
}
