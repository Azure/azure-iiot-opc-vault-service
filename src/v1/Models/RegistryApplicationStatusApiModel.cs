// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Azure.IIoT.OpcUa.Api.Registry.Models;
using Newtonsoft.Json;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Vault.v1.Models
{

    /// <summary>
    /// The application database status when compared to the registry.
    /// </summary>
    public enum RegistryApplicationStatusType : int
    {
        /// <summary>
        /// The Application Id is not known in the registry.
        /// </summary>
        [EnumMember(Value = "unknown")]
        Unknown = 0,
        /// <summary>
        /// The application and registry state are up to date and ok.
        /// </summary>
        [EnumMember(Value = "ok")]
        Ok = 1,
        /// <summary>
        /// The registry contains a new application.
        /// </summary>
        [EnumMember(Value = "new")]
        New = 2,
        /// <summary>
        /// The registry contains updates compared to the application database.
        /// </summary>
        [EnumMember(Value = "update")]
        Update = 3
    }


    public sealed class RegistryApplicationStatusApiModel
    {
        /// <summary>
        /// The state of the applications in the registry and the security database.
        /// </summary>
        [JsonProperty(PropertyName = "status", Order = 10)]
        public RegistryApplicationStatusType Status;
        /// <summary>
        /// The current application information in the registry database.
        /// </summary>
        [JsonProperty(PropertyName = "registry", Order = 20)]
        public ApplicationInfoApiModel Registry;
        /// <summary>
        /// The application information in the security database.
        /// </summary>
        [JsonProperty(PropertyName = "application", Order = 30)]
        public ApplicationRecordApiModel Application;
    }
}
