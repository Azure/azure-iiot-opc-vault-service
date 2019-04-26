// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.IIoT.Auth.Clients;
using Microsoft.Azure.IIoT.Http;
using Microsoft.Azure.IIoT.Http.Auth;
using Microsoft.Azure.IIoT.Http.Default;
using Microsoft.Azure.IIoT.Services.Auth.Clients;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Vault.v1.Auth
{

    /// <summary> 
    /// The http client used to access the IIoT registry.
    /// </summary>
    public class IIoTHttpClient : IHttpClient
    {
        private readonly HttpClient _httpClient;
        public IIoTHttpClient(
            IHttpContextAccessor ctx, 
            IClientConfig clientConfig,
            ILogger logger)
        {
            _httpClient = new HttpClient(new HttpClientFactory(new HttpHandlerFactory(new List<IHttpHandler> {
                new HttpBearerAuthentication(new IIoTTokenProvider(ctx, clientConfig), logger)
            }, logger), logger), logger);
        }

        public Task<IHttpResponse> DeleteAsync(IHttpRequest request)
        {
            return _httpClient.DeleteAsync(request);
        }

        public Task<IHttpResponse> GetAsync(IHttpRequest request)
        {
            return _httpClient.GetAsync(request);
        }

        public Task<IHttpResponse> HeadAsync(IHttpRequest request)
        {
            return _httpClient.HeadAsync(request);
        }

        public IHttpRequest NewRequest(Uri uri, string resourceId = null)
        {
            return _httpClient.NewRequest(uri, resourceId);
        }

        public Task<IHttpResponse> OptionsAsync(IHttpRequest request)
        {
            return _httpClient.OptionsAsync(request);
        }

        public Task<IHttpResponse> PatchAsync(IHttpRequest request)
        {
            return _httpClient.PatchAsync(request);
        }

        public Task<IHttpResponse> PostAsync(IHttpRequest request)
        {
            return _httpClient.PostAsync(request);
        }

        public Task<IHttpResponse> PutAsync(IHttpRequest request)
        {
            return _httpClient.PutAsync(request);
        }
    }
}
