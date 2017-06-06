// Copyright (c) Microsoft. All rights reserved.

using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Azure.IoTSolutions.ProjectNameHere.Services;
using Microsoft.Azure.IoTSolutions.ProjectNameHere.Services.Runtime;
using Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService.Runtime;
using Owin;

namespace Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService
{
    public class DependencyResolution
    {
        //// <summary>
        /// Provide factory pattern for dependencies that are instantiated
        /// multiple times during the application lifetime.
        /// How to use:
        ///
        /// class MyClass : IMyClass {
        ///     public MyClass(DependencyInjection.IFactory factory) {
        ///         this.factory = factory;
        ///     }
        ///     public SomeMethod() {
        ///         var instance1 = this.factory.Resolve<ISomething>();
        ///         var instance2 = this.factory.Resolve<ISomething>();
        ///         var instance3 = this.factory.Resolve<ISomething>();
        ///     }
        /// }
        /// </summary>
        public interface IFactory
        {
            T Resolve<T>();
        }

        public class Factory : IFactory
        {
            private static IContainer container;

            public static void RegisterContainer(IContainer c)
            {
                container = c;
            }

            public T Resolve<T>()
            {
                return container.Resolve<T>();
            }
        }

        /// <summary>
        /// Autofac configuration. Find more information here:
        /// http://docs.autofac.org/en/latest/integration/owin.html
        /// http://autofac.readthedocs.io/en/latest/register/scanning.html
        /// </summary>
        public static IContainer Setup(IAppBuilder app, HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            AutowireAssemblies(builder);
            SetupCustomRules(builder);
            SetupWebApiRules(builder);

            var container = builder.Build();

            Factory.RegisterContainer(container);

            SetupWebApiResolution(app, config, container);

            return container;
        }

        /// <summary>
        /// Autowire interfaces to classes. Note that the solution assemblies
        /// are explicitly managed here. This could be extended to analyze
        /// all the assemblies directly and indirectly referenced.
        /// </summary>
        private static void AutowireAssemblies(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();

            assembly = typeof(ServicesConfig).Assembly;
            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
        }

        private static void SetupWebApiRules(ContainerBuilder builder)
        {
            // Register Web API controllers in executing assembly.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
        }

        private static void SetupWebApiResolution(IAppBuilder app, HttpConfiguration config, IContainer container)
        {
            // Create and assign a dependency resolver for Web API to use.
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            // The Autofac middleware should be the first middleware added to
            // the IAppBuilder.
            app.UseAutofacMiddleware(container);

            // Make sure the Autofac lifetime scope is passed to Web API.
            app.UseAutofacWebApi(config);
        }

        /// <summary>Setup Custom rules overriding autowired ones.</summary>
        private static void SetupCustomRules(ContainerBuilder builder)
        {
            // Make sure the configuration is read only once.
            var config = new Config(new ConfigData());
            builder.RegisterInstance(config).As<IConfig>().SingleInstance();

            // Service configuration is generated by the entry point, so we
            // prepare the instance here.
            builder.RegisterInstance(config.ServicesConfig).As<IServicesConfig>().SingleInstance();

            // By default Autofac uses a request lifetime, creating new objects
            // for each request, which is good to reduce the risk of memory
            // leaks, but not so good for the overall performance.
            // TODO: revisit when migrating to ASP.NET Core.
            builder.RegisterType<Devices>().As<IDevices>().SingleInstance();
            builder.RegisterType<DeviceTwins>().As<IDeviceTwins>().SingleInstance();
        }
    }
}
