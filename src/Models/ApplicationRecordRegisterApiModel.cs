// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Azure.IIoT.OpcUa.Api.Vault.Models;
using Opc.Ua.Gds.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.Azure.IIoT.WebApps.OpcUa.Vault.Models {
    /// <summary>
    /// helper for model validation in registration form
    /// </summary>
    public class ApplicationRecordRegisterApiModelAttribute : ValidationAttribute, IClientModelValidator {
        private readonly ServerCapabilities _serverCaps = new ServerCapabilities();
        private const int ApplicationTypeClient = 1;

        public void AddValidation(ClientModelValidationContext context) => throw new NotImplementedException();

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            var application = (ApplicationRecordRegisterApiModel)validationContext.ObjectInstance;
            var errorList = new List<string>();

            if (string.IsNullOrWhiteSpace(application.ApplicationUri)) { errorList.Add(nameof(application.ApplicationUri)); }
            if (string.IsNullOrWhiteSpace(application.ProductUri)) { errorList.Add(nameof(application.ProductUri)); }
            if (string.IsNullOrWhiteSpace(application.ApplicationName)) { errorList.Add(nameof(application.ApplicationName)); }
            if (application.ApplicationType != ApplicationType.Client) {
                if (string.IsNullOrWhiteSpace(application.ServerCapabilities)) { errorList.Add(nameof(application.ServerCapabilities)); }
                if (application.DiscoveryUrls != null) {
                    for (var i = 0; i < application.DiscoveryUrls.Count; i++) {
                        if (string.IsNullOrWhiteSpace(application.DiscoveryUrls[i])) { errorList.Add($"DiscoveryUrls[{i}]"); }
                    }
                }
                else {
                    errorList.Add($"DiscoveryUrls[0]");
                }
            }
            if (errorList.Count > 0) { return new ValidationResult("Required Field.", errorList); }

            /* entries will be ignored on register
            if (application.ApplicationType == ApplicationTypeClient)
            {
                if (!String.IsNullOrWhiteSpace(application.ServerCapabilities)) { errorList.Add(nameof(application.ServerCapabilities)); }
                for (int i = 0; i < application.DiscoveryUrls.Count; i++)
                {
                    if (!String.IsNullOrWhiteSpace(application.DiscoveryUrls[i])) { errorList.Add($"DiscoveryUrls[{i}]"); }
                }
                if (errorList.Count > 0) { return new ValidationResult("Invalid entry for client.", errorList); }
            }
            */

            if (!Uri.IsWellFormedUriString(application.ApplicationUri, UriKind.Absolute)) { errorList.Add("ApplicationUri"); }
            if (!Uri.IsWellFormedUriString(application.ProductUri, UriKind.Absolute)) { errorList.Add("ProductUri"); }
            if (application.ApplicationType != ApplicationType.Client) {
                for (var i = 0; i < application.DiscoveryUrls.Count; i++) {
                    if (!Uri.IsWellFormedUriString(application.DiscoveryUrls[i], UriKind.Absolute)) { errorList.Add($"DiscoveryUrls[{i}]"); continue; }
                    var uri = new Uri(application.DiscoveryUrls[i], UriKind.Absolute);
                    if (string.IsNullOrEmpty(uri.Host)) { errorList.Add($"DiscoveryUrls[{i}]"); continue; }
                    if (uri.HostNameType == UriHostNameType.Unknown) { errorList.Add($"DiscoveryUrls[{i}]"); continue; }
                }
            }
            if (errorList.Count > 0) { return new ValidationResult("Not a well formed Uri.", errorList); }

            if (application.ApplicationType != ApplicationType.Client &&
                !string.IsNullOrEmpty(application.ServerCapabilities)) {
                var serverCapModelArray = application.ServerCapabilities.Split(',');
                foreach (var cap in serverCapModelArray) {
                    var serverCap = _serverCaps.Find(cap);
                    if (serverCap == null) {
                        errorList.Add(nameof(application.ServerCapabilities));
                        return new ValidationResult(cap + " is not a valid ServerCapability.", errorList);
                    }
                }
            }

            return ValidationResult.Success;
        }
    }


    [ApplicationRecordRegisterApiModel]
    public class ApplicationRecordRegisterApiModel : ApplicationRecordApiModel {
        public ApplicationRecordRegisterApiModel() { }

        public ApplicationRecordRegisterApiModel(ApplicationRecordApiModel apiModel) :
            base(ApplicationState.New, apiModel.ApplicationType, apiModel.ApplicationId, apiModel.Id) {
            ApplicationUri = apiModel.ApplicationUri;
            ApplicationName = apiModel.ApplicationName;
            ApplicationNames = apiModel.ApplicationNames;
            ProductUri = apiModel.ProductUri;
            DiscoveryUrls = apiModel.DiscoveryUrls;
            ServerCapabilities = apiModel.ServerCapabilities;
            GatewayServerUri = apiModel.GatewayServerUri;
            DiscoveryProfileUri = apiModel.DiscoveryProfileUri;
        }
    }

}
