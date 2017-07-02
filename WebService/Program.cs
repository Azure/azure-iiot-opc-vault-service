// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService.Runtime;

namespace Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService
{
    /// <summary>Application entry point</summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new Config(new ConfigData());

            /*
            Print some information to help development and debugging, like
            runtime and configuration settings
            */
            Console.WriteLine($"[{Uptime.ProcessId}] Starting web service started, process ID: " + Uptime.ProcessId);
            Console.WriteLine($"[{Uptime.ProcessId}] Web service listening on port " + config.Port);
            Console.WriteLine($"[{Uptime.ProcessId}] Web service health check at: http://127.0.0.1:" + config.Port + "/" + v1.Version.Path + "/status");
            Console.WriteLine($"[{Uptime.ProcessId}] IoT Hub manager API at " + config.ServicesConfig.IoTHubManagerApiUrl);

            /*
            Kestrel is a cross-platform HTTP server based on libuv, a
            cross-platform asynchronous I/O library.
            https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers
            */
            var host = new WebHostBuilder()
                .UseUrls("http://*:" + config.Port)
                .UseKestrel(options => { options.AddServerHeader = false; })
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
