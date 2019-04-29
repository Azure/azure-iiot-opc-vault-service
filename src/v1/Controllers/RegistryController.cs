// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IIoT.OpcUa.Api.Registry;
using Microsoft.Azure.IIoT.OpcUa.Api.Registry.Models;
using Microsoft.Azure.IIoT.OpcUa.Services.Vault.CosmosDB.Models;
using Microsoft.Azure.IIoT.OpcUa.Services.Vault.v1.Auth;
using Microsoft.Azure.IIoT.OpcUa.Services.Vault.v1.Filters;
using Microsoft.Azure.IIoT.OpcUa.Services.Vault.v1.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Vault.v1.Controllers
{
    /// <summary>
    /// Application services.
    /// </summary>
    [ApiController]
    [Route(VersionInfo.PATH + "/reg"), TypeFilter(typeof(ExceptionsFilterAttribute))]
    [Produces("application/json")]
    [Authorize(Policy = Policies.CanRead)]
    public sealed class RegistryController : Controller
    {
        private readonly IApplicationsDatabase _applicationDatabase;
        private readonly IRegistryServiceApi _registryServiceApi;
        private readonly ILogger _logger;

        /// <inheritdoc/>
        public RegistryController(
            IApplicationsDatabase applicationDatabase,
            IRegistryServiceApi registryServiceApi,
            ILogger logger)
        {
            _applicationDatabase = applicationDatabase;
            _registryServiceApi = registryServiceApi;
            _logger = logger;
        }

        /// <summary>
        /// List applications which differ in the actual registry.
        /// </summary>
        /// <remarks>
        /// List all new and differing applications between the OPC UA registry
        /// and the security service database.
        /// </remarks>
        /// <returns>The differing application records</returns>
        [HttpGet("diff")]
        public async Task<RegistryApplicationStatusResponseApiModel> RegistryApplicationStatusDiffAsync(bool? allRecords)
        {
            var modelResult = new List<RegistryApplicationStatusApiModel>();
            var query = new ApplicationRegistrationQueryApiModel()
            {
                //ApplicationUri = applicationUri
            };
            foreach (var record in await _registryServiceApi.QueryAllApplicationsAsync(query))
            {
                var status = await GetApplicationStatusAsync(record);
                if ((allRecords != null && (bool)allRecords) ||
                    status.Status != RegistryApplicationStatusType.Ok)
                {
                    modelResult.Add(status);
                }
            }
            return new RegistryApplicationStatusResponseApiModel(modelResult, null);
        }

        /// <summary>
        /// Update applications which differ from the actual registry.
        /// </summary>
        /// <remarks>
        /// Update all new and differing applications between the OPC UA registry
        /// and the security service database.
        /// </remarks>
        /// <returns>The differing application records</returns>
        [HttpPost("update")]
        public async Task<RegistryApplicationStatusResponseApiModel> UpdateApplicationStatusDiffAsync(
            string registryId,
            bool? allRecords)
        {
            var modelResult = new List<RegistryApplicationStatusApiModel>();
            if (registryId == null)
            {
                var query = new ApplicationRegistrationQueryApiModel()
                {
                    //ApplicationUri = applicationUri
                };
                foreach (var record in await _registryServiceApi.QueryAllApplicationsAsync(query))
                {
                    var status = await GetApplicationStatusAsync(record);
                    if ((allRecords != null && (bool)allRecords) ||
                        status.Status == RegistryApplicationStatusType.New)
                    {
                        var newApplication = NewApplicationFromRegistry(record);
                        var registeredApplication = await _applicationDatabase.RegisterApplicationAsync(newApplication);
                        status.Application = new ApplicationRecordApiModel(registeredApplication);
                        modelResult.Add(status);
                    }
                }
            }
            else
            {
                var registryApplication = await _registryServiceApi.GetApplicationAsync(registryId);
                var status = await GetApplicationStatusAsync(registryApplication.Application);
                var newApplication = NewApplicationFromRegistry(registryApplication.Application);
                var registeredApplication = await _applicationDatabase.RegisterApplicationAsync(newApplication);
                status.Application = new ApplicationRecordApiModel(registeredApplication);
                modelResult.Add(status);
            }
            return new RegistryApplicationStatusResponseApiModel(modelResult, null);
        }

        /// <summary>
        /// Return status of an applications.
        /// </summary>
        /// <remarks>
        /// Returns the status of an application in the registry.
        /// </remarks>
        /// <returns>The application status records</returns>
        [HttpGet("{registryId}/status")]
        public async Task<RegistryApplicationStatusApiModel> RegistryStatusAsync(
            string registryId
            )
        {
            RegistryApplicationStatusApiModel modelResult = new RegistryApplicationStatusApiModel()
            {
                Status = RegistryApplicationStatusType.Unknown
            };
            ApplicationRegistrationApiModel record = await _registryServiceApi.GetApplicationAsync(registryId);
            if (record != null)
            {
                return await GetApplicationStatusAsync(record.Application);
            }
            return modelResult;
        }


        /// <summary>
        /// Get the registry service status.
        /// </summary>
        [HttpGet]
        public Task<StatusResponseApiModel> GetStatusAsync()
        {
            return _registryServiceApi.GetServiceStatusAsync();
        }


        private RegistryApplicationStatusType TestApplicationStatus(ApplicationInfoApiModel registry, Application application)
        {
            if (String.Equals(registry.ApplicationUri, application.ApplicationUri))
            {
                if ((int)registry.ApplicationType != (int)application.ApplicationType ||
                    !String.Equals(registry.ApplicationName, application.ApplicationName) ||
                    !String.Equals(registry.ProductUri, application.ProductUri) //||
                                                                                //!String.Equals(registry.ApplicationId, application.RegistryId)
                    )
                {
                    return RegistryApplicationStatusType.Update;
                }

                // TODO: discoveryUrls, Capabilities

                return RegistryApplicationStatusType.Ok;
            }
            return RegistryApplicationStatusType.Unknown;
        }

        private async Task<RegistryApplicationStatusApiModel> GetApplicationStatusAsync(ApplicationInfoApiModel record)
        {
            RegistryApplicationStatusApiModel modelResult = new RegistryApplicationStatusApiModel()
            {
                Status = RegistryApplicationStatusType.Unknown
            };
            if (record != null)
            {
                modelResult.Registry = record;
                modelResult.Status = RegistryApplicationStatusType.New;
                try
                {
                    var applications = await _applicationDatabase.ListApplicationAsync(record.ApplicationUri);
                    foreach (var application in applications)
                    {
                        var status = TestApplicationStatus(record, application);
                        if (status == RegistryApplicationStatusType.Ok ||
                            status == RegistryApplicationStatusType.Update)
                        {
                            // TODO: check if there are more results?
                            modelResult.Application = new ApplicationRecordApiModel(application);
                            modelResult.Status = status;
                            break;
                        }
                    }
                }
                catch
                {
                    // not found, new
                }
            }
            return modelResult;
        }

        private Application NewApplicationFromRegistry(ApplicationInfoApiModel record)
        {
            var applicationNames = new ApplicationName[]
                            {
                                new ApplicationName()
                                {
                                    Text = record.ApplicationName,
                                    Locale = record.Locale
                                }
                            };
            var newApplication = new Application()
            {
                ApplicationName = record.ApplicationName,
                ApplicationNames = applicationNames,
                ApplicationType = (Types.ApplicationType)record.ApplicationType,
                ApplicationUri = record.ApplicationUri,
                DiscoveryUrls = record.DiscoveryUrls.ToArray(),
                AuthorityId = User.Identity.Name,
                ProductUri = record.ProductUri,
                RegistryId = record.ApplicationId,
                ApplicationState = Types.ApplicationState.New,
                CreateTime = DateTime.UtcNow
            };
            if (record.ApplicationType != Api.Registry.Models.ApplicationType.Client)
            {
                if (record.Capabilities != null)
                {
                    newApplication.ServerCapabilities = String.Join(",", record.Capabilities);
                }
                else
                {
                    newApplication.ServerCapabilities = "NA";
                }
            }
            return newApplication;
        }
    }
}
