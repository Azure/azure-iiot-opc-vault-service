// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Mono.Options;
using Opc.Ua.Configuration;
using Opc.Ua.Gds.Server.Database.CosmosDB;
using Opc.Ua.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Opc.Ua.Gds.Server
{
    public class ApplicationMessageDlg : IApplicationMessageDlg
    {
        private string message = string.Empty;
        private bool ask = false;

        public override void Message(string text, bool ask)
        {
            this.message = text;
            this.ask = ask;
        }

        public override async Task<bool> ShowAsync()
        {
            if (ask)
            {
                message += " (y/n, default y): ";
                Console.Write(message);
            }
            else
            {
                Console.WriteLine(message);
            }
            if (ask)
            {
                try
                {
                    ConsoleKeyInfo result = Console.ReadKey();
                    Console.WriteLine();
                    return await Task.FromResult((result.KeyChar == 'y') || (result.KeyChar == 'Y') || (result.KeyChar == '\r'));
                }
                catch
                {
                    // intentionally fall through
                }
            }
            return await Task.FromResult(true);
        }
    }

    public enum ExitCode : int
    {
        Ok = 0,
        ErrorServerNotStarted = 0x80,
        ErrorServerRunning = 0x81,
        ErrorServerException = 0x82,
        ErrorInvalidCommandLine = 0x100
    };

    public class Program
    {

        public static string Name = "Azure Industrial IoT Edge OPC UA Global Discovery Server";

        public static int Main(string[] args)
        {
            Console.WriteLine("Azure Industrial IoT Edge OPC UA Global Discovery Server");

            // command line options
            bool showHelp = false;
            string gdsVault = null;
            string appID = null;
            string cosmosDB = null;
            string cosmosDBKey = null;

            Mono.Options.OptionSet options = new Mono.Options.OptionSet {
                { "g|gdsvault=", "GDS Vault Url", g => gdsVault = g },
                { "a|appid=", "Active Directory Application Id", a => appID = a },
                { "c|cosmosdb=", "Cosmos DB Url", c => cosmosDB = c },
                { "k|key=", "Cosmos DB Key", k => cosmosDBKey = k },
                { "h|help", "show this message and exit", h => showHelp = h != null },
            };

            try
            {
                IList<string> extraArgs = options.Parse(args);
                foreach (string extraArg in extraArgs)
                {
                    Console.WriteLine("Error: Unknown option: {0}", extraArg);
                    showHelp = true;
                }
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                showHelp = true;
            }

            if (showHelp)
            {
                Console.WriteLine("Usage: dotnet Microsoft.Azure.IIoT.OpcUa.Services.Gds.Edge.dll [OPTIONS]");
                Console.WriteLine();

                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return (int)ExitCode.ErrorInvalidCommandLine;
            }

            EdgeGlobalDiscoveryServer server = new EdgeGlobalDiscoveryServer();
            server.Run(gdsVault, appID, cosmosDB, cosmosDBKey);

            return (int)EdgeGlobalDiscoveryServer.ExitCode;
        }
    }

    public class EdgeGlobalDiscoveryServer
    {
        GlobalDiscoverySampleServer server;
        Task status;
        DateTime lastEventTime;
        static ExitCode exitCode;

        public EdgeGlobalDiscoveryServer()
        {
        }

        public void Run(string gdsVault, string appID, string cosmosDB, string cosmosDBKey)
        {

            try
            {
                exitCode = ExitCode.ErrorServerNotStarted;
                ConsoleGlobalDiscoveryServer(gdsVault, appID, cosmosDB, cosmosDBKey).Wait();
                Console.WriteLine("Server started. Press Ctrl-C to exit...");
                exitCode = ExitCode.ErrorServerRunning;
            }
            catch (Exception ex)
            {
                Utils.Trace("ServiceResultException:" + ex.Message);
                Console.WriteLine("Exception: {0}", ex.Message);
                exitCode = ExitCode.ErrorServerException;
                return;
            }

            ManualResetEvent quitEvent = new ManualResetEvent(false);
            try
            {
                Console.CancelKeyPress += (sender, eArgs) =>
                {
                    quitEvent.Set();
                    eArgs.Cancel = true;
                };
            }
            catch
            {
            }

            // wait for timeout or Ctrl-C
            quitEvent.WaitOne();

            if (server != null)
            {
                Console.WriteLine("Server stopped. Waiting for exit...");

                using (GlobalDiscoverySampleServer _server = server)
                {
                    // Stop status thread
                    server = null;
                    status.Wait();
                    // Stop server and dispose
                    _server.Stop();
                }
            }

            exitCode = ExitCode.Ok;
        }

        public static ExitCode ExitCode { get => exitCode; }

        private static void CertificateValidator_CertificateValidation(CertificateValidator validator, CertificateValidationEventArgs e)
        {
            if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
            {
                // GDS accepts any client certificate
                e.Accept = true;
                Console.WriteLine("Accepted Certificate: {0}", e.Certificate.Subject);
            }
        }

        private async Task ConsoleGlobalDiscoveryServer(
            string gdsVaultServiceUrl, 
            string appId, 
            string dbServiceUrl, 
            string dbServiceKey)
        {
            ApplicationInstance.MessageDlg = new ApplicationMessageDlg();
            ApplicationInstance application = new ApplicationInstance
            {
                ApplicationName = Program.Name,
                ApplicationType = ApplicationType.Server,
                ConfigSectionName = "Microsoft.Azure.IIoT.OpcUa.Services.Gds.Edge"
            };

            // load the application configuration.
            ApplicationConfiguration config = await application.LoadApplicationConfiguration(false);

            // check the application certificate.
            bool haveAppCertificate = await application.CheckApplicationInstanceCertificate(false, 0);
            if (!haveAppCertificate)
            {
                throw new Exception("Application instance certificate invalid!");
            }

            if (!config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
            {
                config.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);
            }

            // get the DatabaseStorePath configuration parameter.
            GlobalDiscoveryServerConfiguration gdsConfiguration = config.ParseExtension<GlobalDiscoveryServerConfiguration>();

            // extract appId and vault name from database storage path
            string[] keyVaultConfig = gdsConfiguration.DatabaseStorePath?.Split(',');
            if (keyVaultConfig != null)
            {
                if (String.IsNullOrEmpty(gdsVaultServiceUrl))
                {
                    // try configuration using XML config
                    gdsVaultServiceUrl = keyVaultConfig[0];
                }

                if (String.IsNullOrEmpty(appId))
                {
                    if (keyVaultConfig.Length > 1 && !String.IsNullOrEmpty(keyVaultConfig[1]))
                    {
                        appId = keyVaultConfig[1];
                    }
                }

                if (String.IsNullOrEmpty(dbServiceUrl))
                {
                    // initialize database Uri
                    if (keyVaultConfig.Length >= 3 && !String.IsNullOrEmpty(keyVaultConfig[2]))
                    {
                        dbServiceUrl = keyVaultConfig[2];
                    }
                }

                if (String.IsNullOrEmpty(dbServiceKey))
                {
                    // initialize database Uri
                    if (keyVaultConfig.Length >= 4 && !String.IsNullOrEmpty(keyVaultConfig[3]))
                    {
                        dbServiceKey = keyVaultConfig[3];
                    }
                }
            }

            // The vault handler with authentication
            var gdsVaultHandler = new GdsServiceClientHandler(new Uri(gdsVaultServiceUrl));
#if TODO
            if (String.IsNullOrEmpty(appId))
            {
                // authenticate key vault with MSI (web app) or developer user account
                gdsVaultHandler.SetTokenProvider();
            }
            else
            {
                // authenticate key vault with app cert
                gdsVaultHandler.SetAssertionCertificate(appId, await config.SecurityConfiguration.ApplicationCertificate.LoadPrivateKey(string.Empty));
            }
#endif
            // read configurations from GDS Vault
            gdsConfiguration.CertificateGroups = await gdsVaultHandler.GetCertificateConfigurationGroupsAsync(gdsConfiguration.BaseCertificateGroupStorePath);
            UpdateGDSConfigurationDocument(config.Extensions, gdsConfiguration);

            var certGroup = new GdsServiceCertificateGroup(gdsVaultHandler);
            if (!String.IsNullOrEmpty(dbServiceUrl))
            {
                // TODO: use resource token not access key!
                var requestDB = new CosmosDBApplicationsDatabase(dbServiceUrl, dbServiceKey);
                requestDB.Initialize();
                var appDB = new GdsServiceApplicationsDatabase(gdsVaultHandler.GdsServiceClient);
                server = new GlobalDiscoverySampleServer(appDB, requestDB, certGroup);
            }
            else
            {

            }

            // start the server.
            await application.Start(server);

            // print endpoint info
            var endpoints = application.Server.GetEndpoints().Select(e => e.EndpointUrl).Distinct();
            foreach (var endpoint in endpoints)
            {
                Console.WriteLine(endpoint);
            }

            // start the status thread
            status = Task.Run(new Action(StatusThread));

            // print notification on session events
            server.CurrentInstance.SessionManager.SessionActivated += EventStatus;
            server.CurrentInstance.SessionManager.SessionClosing += EventStatus;
            server.CurrentInstance.SessionManager.SessionCreated += EventStatus;

        }

        /// <summary>
        /// Updates the config extension with the new configuration information.
        /// </summary>
        private static void UpdateGDSConfigurationDocument(XmlElementCollection extensions, GlobalDiscoveryServerConfiguration gdsConfiguration)
        {
            XmlDocument gdsDoc = new XmlDocument();
            var qualifiedName = EncodeableFactory.GetXmlName(typeof(GlobalDiscoveryServerConfiguration));
            XmlSerializer gdsSerializer = new XmlSerializer(typeof(GlobalDiscoveryServerConfiguration), qualifiedName.Namespace);
            using (XmlWriter writer = gdsDoc.CreateNavigator().AppendChild())
            {
                gdsSerializer.Serialize(writer, gdsConfiguration);
            }

            foreach (var extension in extensions)
            {
                if (extension.Name == qualifiedName.Name)
                {
                    extension.InnerXml = gdsDoc.DocumentElement.InnerXml;
                }
            }
        }


        private void EventStatus(Session session, SessionEventReason reason)
        {
            lastEventTime = DateTime.UtcNow;
            PrintSessionStatus(session, reason.ToString());
        }

        void PrintSessionStatus(Session session, string reason, bool lastContact = false)
        {
            lock (session.DiagnosticsLock)
            {
                string item = String.Format("{0,9}:{1,20}:", reason, session.SessionDiagnostics.SessionName);
                if (lastContact)
                {
                    item += String.Format("Last Event:{0:HH:mm:ss}", session.SessionDiagnostics.ClientLastContactTime.ToLocalTime());
                }
                else
                {
                    if (session.Identity != null)
                    {
                        item += String.Format(":{0,20}", session.Identity.DisplayName);
                    }
                    item += String.Format(":{0}", session.Id);
                }
                Console.WriteLine(item);
            }
        }

        private async void StatusThread()
        {
            while (server != null)
            {
                if (DateTime.UtcNow - lastEventTime > TimeSpan.FromMilliseconds(6000))
                {
                    IList<Session> sessions = server.CurrentInstance.SessionManager.GetSessions();
                    for (int ii = 0; ii < sessions.Count; ii++)
                    {
                        Session session = sessions[ii];
                        PrintSessionStatus(session, "-Status-", true);
                    }
                    lastEventTime = DateTime.UtcNow;
                }
                await Task.Delay(1000);
            }
        }
    }
}
