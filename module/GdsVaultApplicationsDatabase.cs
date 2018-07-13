// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api.Models;
using Opc.Ua.Gds.Server.GdsVault;
using System;
using System.Collections.Generic;

namespace Opc.Ua.Gds.Server.Database.GdsVault
{
    public class GdsVaultApplicationsDatabase : ApplicationsDatabaseBase
    {
        private IOpcGdsVault _gdsVaultServiceClient { get; }

        public GdsVaultApplicationsDatabase(IOpcGdsVault GdsVaultServiceClient)
        {
            this._gdsVaultServiceClient = GdsVaultServiceClient;
        }

        #region IApplicationsDatabase 
        public override void Initialize()
        {
        }

        public override NodeId RegisterApplication(
                ApplicationRecordDataType application
                )
        {
            bool isUpdate = true;
            string applicationId = null;
            NodeId appNodeId = base.RegisterApplication(application);
            if (NodeId.IsNull(appNodeId))
            {
                isUpdate = false;
            }
            else
            {
                applicationId = GdsVaultClientHelper.GetServiceIdFromNodeId(appNodeId, NamespaceIndex);
            }

            string capabilities = base.ServerCapabilities(application);

            ApplicationRecordApiModel applicationModel = new ApplicationRecordApiModel
            {
                ApplicationId = applicationId,
                ApplicationUri = application.ApplicationUri,
                ApplicationName = application.ApplicationNames[0].Text,
                ApplicationType = (int)application.ApplicationType,
                ProductUri = application.ProductUri,
                ServerCapabilities = capabilities
            };

            if (application.DiscoveryUrls != null)
            {
                applicationModel.DiscoveryUrls = application.DiscoveryUrls.ToArray();
            }

            if (application.ApplicationNames != null && application.ApplicationNames.Count > 0)
            {
                var applicationNames = new List<ApplicationNameApiModel>();
                foreach (var applicationName in application.ApplicationNames)
                {
                    applicationNames.Add(new ApplicationNameApiModel()
                    {
                        Locale = applicationName.Locale,
                        Text = applicationName.Text
                    });
                }
                applicationModel.ApplicationNames = applicationNames.ToArray();
            }

            if (isUpdate)
            {
                string nodeId = _gdsVaultServiceClient.UpdateApplication(applicationId, applicationModel);
            }
            else
            {
                applicationId = _gdsVaultServiceClient.RegisterApplication(applicationModel);
            }

            return GdsVaultClientHelper.GetNodeIdFromServiceId(applicationId, NamespaceIndex);
        }


        public override void UnregisterApplication(NodeId applicationId)
        {
            string id = GdsVaultClientHelper.GetServiceIdFromNodeId(applicationId, NamespaceIndex);

            try
            {
                _gdsVaultServiceClient.UnregisterApplication(id);
            }
            catch
            {
                throw new ArgumentException("A record with the specified application id does not exist.", nameof(applicationId));
            }
        }

        public override ApplicationRecordDataType GetApplication(
            NodeId applicationId
            )
        {
            string id = GdsVaultClientHelper.GetServiceIdFromNodeId(applicationId, NamespaceIndex);
            ApplicationRecordApiModel result;

            try
            {
                result = _gdsVaultServiceClient.GetApplication(id);
            }
            catch
            {
                return null;
            }

            var names = new List<LocalizedText>();
            foreach (var applicationName in result.ApplicationNames)
            {
                names.Add(new LocalizedText(applicationName.Locale, applicationName.Text));
            }

            StringCollection discoveryUrls = null;

            var endpoints = result.DiscoveryUrls;
            if (endpoints != null)
            {
                discoveryUrls = new StringCollection();

                foreach (var endpoint in endpoints)
                {
                    discoveryUrls.Add(endpoint);
                }
            }

            var capabilities = new StringCollection();
            if (!String.IsNullOrWhiteSpace(result.ServerCapabilities))
            {
                capabilities.AddRange(result.ServerCapabilities.Split(','));
            }

            NodeId appNodeId = GdsVaultClientHelper.GetNodeIdFromServiceId(result.ApplicationId, NamespaceIndex);
            return new ApplicationRecordDataType()
            {
                ApplicationId = appNodeId,
                ApplicationUri = result.ApplicationUri,
                ApplicationType = (ApplicationType)result.ApplicationType,
                ApplicationNames = new LocalizedTextCollection(names),
                ProductUri = result.ProductUri,
                DiscoveryUrls = discoveryUrls,
                ServerCapabilities = capabilities
            };
        }

        public override ApplicationRecordDataType[] FindApplications(
            string applicationUri
            )
        {
            IList<ApplicationRecordApiModel> results;

            results = _gdsVaultServiceClient.FindApplication(applicationUri);

            List<ApplicationRecordDataType> records = new List<ApplicationRecordDataType>();

            foreach (var result in results)
            {
                LocalizedText[] names = null;

                if (result.ApplicationName != null)
                {
                    names = new LocalizedText[] { result.ApplicationName };
                }

                StringCollection discoveryUrls = null;

                var endpoints = result.DiscoveryUrls;
                if (endpoints != null)
                {
                    discoveryUrls = new StringCollection();

                    foreach (var endpoint in endpoints)
                    {
                        discoveryUrls.Add(endpoint);
                    }
                }

                string[] capabilities = null;

                if (result.ServerCapabilities != null)
                {
                    capabilities = result.ServerCapabilities.Split(',');
                }

                NodeId appNodeId = GdsVaultClientHelper.GetNodeIdFromServiceId(result.ApplicationId, NamespaceIndex);

                records.Add(new ApplicationRecordDataType()
                {
                    ApplicationId = appNodeId,
                    ApplicationUri = result.ApplicationUri,
                    ApplicationType = (ApplicationType)result.ApplicationType,
                    ApplicationNames = new LocalizedTextCollection(names),
                    ProductUri = result.ProductUri,
                    DiscoveryUrls = discoveryUrls,
                    ServerCapabilities = capabilities
                });
            }

            return records.ToArray();
        }

        public override ServerOnNetwork[] QueryServers(
            uint startingRecordId,
            uint maxRecordsToReturn,
            string applicationName,
            string applicationUri,
            string productUri,
            string[] serverCapabilities,
            out DateTime lastCounterResetTime)
        {
            lastCounterResetTime = DateTime.MinValue;
            List<ServerOnNetwork> records = new List<ServerOnNetwork>();
            const uint defaultRecordsPerQuery = 10;
#if TODO
            lastCounterResetTime = queryCounterResetTime;

            bool matchQuery = false;
            bool complexQuery =
                !String.IsNullOrEmpty(applicationName) ||
                !String.IsNullOrEmpty(applicationUri) ||
                !String.IsNullOrEmpty(productUri) ||
                (serverCapabilities != null && serverCapabilities.Length > 0);

            if (complexQuery)
            {
                // TODO: implement query with server side match...
                matchQuery =
                    IsMatchPattern(applicationName) ||
                    IsMatchPattern(applicationUri) ||
                    IsMatchPattern(productUri);
            }

            bool lastQuery = false;
            do
            {
                uint queryRecords = complexQuery ? defaultRecordsPerQuery : maxRecordsToReturn;
                // TODO: implement query with server side match...
                string query = CreateServerQuery(startingRecordId, queryRecords);
                var applications = Applications.GetAsync(query).Result;
                lastQuery = queryRecords == 0 || applications.Count() < queryRecords || applications.Count() == 0;

                foreach (var result in applications)
                {
                    startingRecordId = result.ID + 1;

                    if (!String.IsNullOrEmpty(applicationName))
                    {
                        if (!Match(result.ApplicationName, applicationName))
                        {
                            continue;
                        }
                    }

                    if (!String.IsNullOrEmpty(applicationUri))
                    {
                        if (!Match(result.ApplicationUri, applicationUri))
                        {
                            continue;
                        }
                    }

                    if (!String.IsNullOrEmpty(productUri))
                    {
                        if (!Match(result.ProductUri, productUri))
                        {
                            continue;
                        }
                    }

                    string[] capabilities = null;
                    if (!String.IsNullOrEmpty(result.ServerCapabilities))
                    {
                        capabilities = result.ServerCapabilities.Split(',');
                    }

                    if (serverCapabilities != null && serverCapabilities.Length > 0)
                    {
                        bool match = true;
                        for (int ii = 0; ii < serverCapabilities.Length; ii++)
                        {
                            if (capabilities == null || !capabilities.Contains(serverCapabilities[ii]))
                            {
                                match = false;
                                break;
                            }
                        }

                        if (!match)
                        {
                            continue;
                        }
                    }

                    if (result.DiscoveryUrls != null)
                    {
                        foreach (var discoveryUrl in result.DiscoveryUrls)
                        {
                            records.Add(new ServerOnNetwork()
                            {
                                RecordId = result.ID,
                                ServerName = result.ApplicationName,
                                DiscoveryUrl = discoveryUrl,
                                ServerCapabilities = capabilities
                            });
                        }
                    }

                    if (--maxRecordsToReturn == 0)
                    {
                        break;
                    }
                }
            } while (maxRecordsToReturn > 0 && !lastQuery);
            return records.ToArray();
#endif
            throw new ServiceResultException("Not Implemented");
        }
#if TODO_REMOVE // ???
        public override bool SetApplicationCertificate(
            NodeId applicationId,
            byte[] certificate,
            bool isHttpsCertificate
            )
        {
            Guid id = GetNodeIdGuid(applicationId);

            var result = Applications.GetAsync(id).Result;
            if (result == null)
            {
                return false;
            }

            if (isHttpsCertificate)
            {
                result.HttpsCertificate = certificate;
            }
            else
            {
                result.Certificate = certificate;
            }

            Applications.UpdateAsync(result.ApplicationId, result).Wait();

            return true;
        }

        public override bool SetApplicationTrustLists(
            NodeId applicationId,
            NodeId trustListId,
            NodeId httpsTrustListId
            )
        {
            Guid id = GetNodeIdGuid(applicationId);

            var result = Applications.GetAsync(id).Result;
            if (result == null)
            {
                return false;
            }

            result.TrustListId = null;
            result.HttpsTrustListId = null;

            if (trustListId != null)
            {
                string storePath = trustListId.ToString();

                var result2 = CertificateStores.GetAsync(x => x.Path == storePath).Result.SingleOrDefault();
                if (result2 != null)
                {
                    result.TrustListId = result2.TrustListId;
                }
            }

            if (httpsTrustListId != null)
            {
                string storePath = httpsTrustListId.ToString();
                var result2 = CertificateStores.GetAsync(x => x.Path == storePath).Result.SingleOrDefault();
                if (result2 != null)
                {
                    result.HttpsTrustListId = result2.TrustListId;
                }
            }

            Applications.UpdateAsync(result.ApplicationId, result).Wait();

            return true;
        }
#endif
        #endregion
    }
}
