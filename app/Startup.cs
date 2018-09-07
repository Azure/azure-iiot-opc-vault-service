// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace GdsVault.App
{
    public class OpcGdsVaultConfigured : OpcGdsVault
    {
        public OpcGdsVaultConfigured(IConfiguration config)
            :base(new Uri(config["GdsVault"]))

        {
        }
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// Di container - Initialized in `ConfigureServices`
        /// </summary>
        public IContainer ApplicationContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IOpcGdsVault, OpcGdsVaultConfigured>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => Configuration.Bind("AzureAd", options));

            services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Instead of using the default validation (validating against a single issuer value, as we do in
                    // line of business apps), we inject our own multitenant validation logic
                    ValidateIssuer = false,

                    // If the app is meant to be accessed by entire organizations, add your issuer validation logic here.
                    //IssuerValidator = (issuer, securityToken, validationParameters) => {
                    //    if (myIssuerValidationLogic(issuer)) return issuer;
                    //}
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnTicketReceived = context =>
                    {
                         // If your authentication logic is based on users then add your logic here
                         return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.Redirect("/Error");
                        context.HandleResponse(); // Suppress the exception
                         return Task.CompletedTask;
                    }
                    // If your application needs to do authenticate single users, add your user validation below.
                    //OnTokenValidated = context =>
                    //{
                    //    return myUserValidationLogic(context.Ticket.Principal);
                    //}
                };
            });

            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Prepare DI container
            ApplicationContainer = ConfigureContainer(services);

            // Create the IServiceProvider based on the container
            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory, 
            IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // If you want to dispose of resources that have been resolved in the
            // application container, register for the "ApplicationStopped" event.
            appLifetime.ApplicationStopped.Register(ApplicationContainer.Dispose);

        }

        /// <summary>
        /// Autofac configuration. Find more information here:
        /// @see http://docs.autofac.org/en/latest/integration/aspnetcore.html
        /// </summary>
        public IContainer ConfigureContainer(IServiceCollection services)
        {
            ContainerBuilder builder = new ContainerBuilder();

            // Populate from services di
            builder.Populate(services);

            // By default Autofac uses a request lifetime, creating new objects
            // for each request, which is good to reduce the risk of memory
            // leaks, but not so good for the overall performance.
#if mist
            // Register configuration interfaces
            builder.RegisterInstance(ClientConfig)
                .AsImplementedInterfaces().SingleInstance();

            // Register logger
            builder.RegisterInstance(Logger)
                .AsImplementedInterfaces().SingleInstance();
            builder.RegisterInstance(DP)
                .AsImplementedInterfaces().SingleInstance();

            // Register configuration interfaces
            builder.RegisterInstance(Config)
                .AsImplementedInterfaces().SingleInstance();
            builder.RegisterInstance(Config.ServicesConfig)
                .AsImplementedInterfaces().SingleInstance();

            // CORS setup
            builder.RegisterType<CorsSetup>()
                .AsImplementedInterfaces().SingleInstance();

            // Register http client ...
            builder.RegisterType<HttpClient>().SingleInstance()
                .AsImplementedInterfaces();
            builder.RegisterType<HttpHandlerFactory>().SingleInstance()
                .AsImplementedInterfaces();
            builder.RegisterType<HttpClientFactory>().SingleInstance()
                .AsImplementedInterfaces();

            // Register endpoint services and ...
            builder.RegisterType<KeyVaultCertificateGroup>()
                .AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<CosmosDBApplicationsDatabase>()
                .AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<CosmosDBCertificateRequest>()
                .AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<BehalfOfTokenProvider>()
                .AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<DistributedTokenCache>()
                .AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<DistributedTokenCacheService>()
                .AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<DefaultTokenCacheProvider>()
                .AsImplementedInterfaces().SingleInstance();
#endif
#if DEBUG
            builder.RegisterType<NoOpValidator>()
                .AsImplementedInterfaces();
#endif

            return builder.Build();
        }

    }
}
