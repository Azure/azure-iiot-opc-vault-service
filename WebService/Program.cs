// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.Runtime;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService
{
    /// <summary>Application entry point</summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            /*
            Kestrel is a cross-platform HTTP server based on libuv, a
            cross-platform asynchronous I/O library.
            https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers
            */
            // Load hosting configuration
            var configRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddCommandLine(args)
                .AddEnvironmentVariables("ASPNETCORE_")
                .AddJsonFile("hosting.json", true)
                .AddInMemoryCollection(new Dictionary<string, string> {
                    { "urls", "http://*:58801" }
                })
                .Build();

            /*
            Print some information to help development and debugging, like
            runtime and configuration settings
            */
            Console.WriteLine($"[{Uptime.ProcessId}] Starting web service, process ID: " + Uptime.ProcessId);

            var host = new WebHostBuilder()
                .UseConfiguration(configRoot)
                //.ConfigureServices(services => services.AddAutofac())
                .UseKestrel(options => { options.AddServerHeader = false; })
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            //var config = configRoot.
            //Console.WriteLine($"[{Uptime.ProcessId}] Web service listening on port " + configRoot.Port);
            //Console.WriteLine($"[{Uptime.ProcessId}] Web service health check at: http://127.0.0.1:" + config.Port + "/" + v1.ServiceInfo.PATH + "/status");
            //Console.WriteLine($"[{Uptime.ProcessId}] IoT Hub manager API at " + config.ServicesConfig.IoTHubManagerApiUrl);

            host.Run();
        }
    }
}
