// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Azure.IoTSolutions.OpcGdsVault.Services.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.Runtime
{
    public interface IConfigData
    {
        /// <summary>
        /// Read a string value from configuration.
        /// </summary>
        /// <param name="key">The key of the configuration string.</param>
        string GetString(string key);
        /// <summary>
        /// Read an integer value from configuration.
        /// </summary>
        /// <param name="key">The key of the configuration integer.</param>
        int GetInt(string key);
    }

    public class ConfigData : IConfigData
    {
        private readonly IConfigurationRoot configuration;

        /// <summary>
        /// More info about configuration at
        /// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration
        /// </summary>
        /// <param name="configRoot">The IConfigurationRoot</param>
        public ConfigData(IConfigurationRoot configRoot)
        {
            this.configuration = configRoot;
        }

        /// <summary>
        /// Read a string value from configuration.
        /// </summary>
        /// <param name="key">The key of the configuration string.</param>
        public string GetString(string key)
        {
            var value = this.configuration.GetValue<string>(key);
            return ReplaceEnvironmentVariables(value);
        }

        /// <summary>
        /// Read an integer value from configuration.
        /// </summary>
        /// <param name="key">The key of the configuration integer.</param>
        public int GetInt(string key)
        {
            try
            {
                return Convert.ToInt32(this.GetString(key));
            }
            catch (Exception e)
            {
                throw new InvalidConfigurationException($"Unable to load configuration value for '{key}'", e);
            }
        }

        private static string ReplaceEnvironmentVariables(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;

            // Extract the name of all the substitutions required
            // using the following pattern, e.g. ${VAR_NAME}
            const string pattern = @"\${(?'key'[a-zA-Z_][a-zA-Z0-9_]*)}";
            var keys = (from Match m
                        in Regex.Matches(value, pattern)
                        select m.Groups[1].Value).ToArray();

            foreach (DictionaryEntry x in Environment.GetEnvironmentVariables())
            {
                if (keys.Contains(x.Key))
                {
                    value = value.Replace("${" + x.Key + "}", x.Value.ToString());
                }
            }

            return value;
        }
    }
}
