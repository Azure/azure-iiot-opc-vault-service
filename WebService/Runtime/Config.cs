// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Akka.Configuration;

namespace Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService.Runtime
{
    public interface IConfig
    {
        /// <summary>Web service listening port</summary>
        int Port { get; }

        /// <summary>Service layer configuration</summary>
        Services.Runtime.IConfig ServicesConfig { get; }
    }

    /// <summary>Web service configuration</summary>
    public class Config : IConfig
    {
        private const string Namespace = "com.microsoft.azure.iotsolutions.";
        private const string Application = "project-name-here.";

        public Config()
        {
            var config = ConfigurationFactory.ParseString(GetHoconConfiguration());

            this.Port = config.GetInt(Namespace + Application + "webservice-port");

            this.ServicesConfig = new Services.Runtime.Config
            {
                HubConnString = config.GetString(Namespace + Application + "iothub.connstring")
            };
        }

        /// <summary>Web service listening port</summary>
        public int Port { get; }

        /// <summary>Service layer configuration</summary>
        public Services.Runtime.IConfig ServicesConfig { get; }

        /// <summary>
        /// Read the `application.conf` HOCON file, enabling substitutions
        /// of ${NAME} placeholders with environment variables values.
        /// TODO: remove workaround when [1] is fixed.
        ///       [1] https://github.com/akkadotnet/HOCON/issues/40
        /// </summary>
        /// <returns>Configuration text content</returns>
        private static string GetHoconConfiguration()
        {
            // Pattern of env. vars in the configuration, e.g. ${VAR_NAME}
            var pattern = @"\${(?'key'[a-zA-Z_][a-zA-Z0-9_]*)}";
            var hocon = File.ReadAllText("application.conf") + "\n";

            // Extract the name of all the substitutions required
            var keys = (from Match m
                             in Regex.Matches(hocon, pattern)
                             select m.Groups[1].Value).ToArray();

            // Foreach substitution inject the env. var if available
            foreach (DictionaryEntry x in Environment.GetEnvironmentVariables())
            {
                if (keys.Contains(x.Key))
                    hocon += x.Key + " : \"" + x.Value + "\"\n";
            }

            return hocon;
        }
    }
}
