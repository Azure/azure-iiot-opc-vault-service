// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.v1.Models
{
    public sealed class QueryRequestsResponseApiModel
    {
        [JsonProperty(PropertyName = "Requests", Order = 10)]
        public ReadRequestApiModel[] Requests { get; set; }

        /// <summary>
        /// Continuation token to use
        /// </summary>
        [JsonProperty(PropertyName = "ContinuationToken", Order = 20)]
        public string ContinuationToken { get; set; }

        public QueryRequestsResponseApiModel(ReadRequestResultModel[] requests)
        {
            List<ReadRequestApiModel> requestList = new List<ReadRequestApiModel>();
            foreach (ReadRequestResultModel request in requests)
            {
                requestList.Add(new ReadRequestApiModel(
                request.State,
                request.ApplicationId,
                request.RequestId,
                request.CertificateGroupId,
                request.CertificateTypeId,
                request.SigningRequest,
                request.SubjectName,
                request.DomainNames,
                request.PrivateKeyFormat,
                request.PrivateKeyPassword));
            }
            Requests = requestList.ToArray();
        }

    }
}
