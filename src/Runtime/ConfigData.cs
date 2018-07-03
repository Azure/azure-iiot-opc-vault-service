// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Microsoft.Azure.IIoT.OpcUa.Services.Gds.Exceptions;
using Microsoft.Azure.IoTSolutions.GdsVault.Common.Diagnostics;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.Runtime
{
    public interface IConfigData
    {
#if mist
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
#endif
        /// <summary>
        /// Read variable and replace environment variable if needed
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        string GetString(string key, string defaultValue = "");

        /// <summary>
        /// Read boolean
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        bool GetBool(string key, bool defaultValue = false);

        /// <summary>
        /// Read int value from configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        int GetInt(string key, int defaultValue = 0);

        /// <summary>
        /// Get log level
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        LogLevel GetLogLevel(string key, LogLevel defaultValue);
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
#if mist
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
#endif

        /// <summary>
        /// Read string key and replace environment variable if needed.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetString(string key, string defaultValue = "")
        {
            var value = this.configuration.GetValue(key, defaultValue);
            ReplaceEnvironmentVariables(ref value, defaultValue);
            return value;
        }

        /// <summary>
        /// Read boolean key and replace environment variable if needed.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public bool GetBool(string key, bool defaultValue = false)
        {
            var value = GetString(key, defaultValue.ToString()).ToLowerInvariant();
            var knownTrue = new HashSet<string> { "true", "t", "yes", "y", "1", "-1" };
            var knownFalse = new HashSet<string> { "false", "f", "no", "n", "0" };
            if (knownTrue.Contains(value))
            {
                return true;
            }
            if (knownFalse.Contains(value))
            {
                return false;
            }
            return defaultValue;
        }

        /// <summary>
        /// Read int key and replace environment variable if needed.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public int GetInt(string key, int defaultValue = 0)
        {
            try
            {
                return Convert.ToInt32(GetString(key, defaultValue.ToString()));
            }
            catch (Exception e)
            {
                throw new InvalidConfigurationException(
                    $"Unable to load configuration value for '{key}'", e);
            }
        }

        /// <summary>
        /// Read log level from configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public LogLevel GetLogLevel(string key, LogLevel defaultValue)
        {
            var level = GetString(key);
            if (!string.IsNullOrEmpty(level))
            {
                switch (level.ToLowerInvariant())
                {
                    case "Warning":
                        return LogLevel.Warn;
                    case "Trace":
                    case "Debug":
                        return LogLevel.Debug;
                    case "Information":
                        return LogLevel.Info;
                    case "Error":
                    case "Critical":
                        return LogLevel.Error;
                }
            }
            return defaultValue;
        }

#if mist
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
#endif
        /// <summary>
        /// Replace all placeholders
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        private void ReplaceEnvironmentVariables(ref string value, string defaultValue)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = defaultValue;
                return;
            }
            // Search for optional replacements: ${?VAR_NAME}
            var keys = Regex.Matches(value, @"\${\?([a-zA-Z_][a-zA-Z0-9_]*)}").Cast<Match>()
                .Select(m => m.Groups[1].Value).Distinct().ToArray();
            // Replace
            foreach (var key in keys)
            {
                value = value.Replace("${?" + key + "}", GetString(key, string.Empty));
            }

            // Pattern for mandatory replacements: ${VAR_NAME}
            const string PATTERN = @"\${([a-zA-Z_][a-zA-Z0-9_]*)}";
            // Search
            keys = Regex.Matches(value, PATTERN).Cast<Match>()
                .Select(m => m.Groups[1].Value).Distinct().ToArray();
            // Replace
            foreach (var key in keys)
            {
                var replacement = GetString(key, null);
                if (replacement != null)
                {
                    value = value.Replace("${" + key + "}", replacement);
                }
            }
            // Non replaced placeholders cause an exception
            keys = Regex.Matches(value, PATTERN).Cast<Match>()
                .Select(m => m.Groups[1].Value).ToArray();
            if (keys.Length > 0)
            {
                var varsNotFound = keys.Aggregate(", ", (current, k) => current + k);
                throw new InvalidConfigurationException(
                    "Environment variables not found: " + varsNotFound);
            }
            value.Trim();
            if (string.IsNullOrEmpty(value))
            {
                value = defaultValue;
            }
        }

    }
}
