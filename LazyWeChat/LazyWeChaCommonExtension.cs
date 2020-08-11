using LazyWeChat.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LazyWeChat
{
    public static class LazyWeChaCommonExtension
    {
        internal static IServiceCollection RegisterServices<T>(
            this IServiceCollection services,
            string sectionName,
            Action<T> configure,
            List<(Type, string, ServiceLifetime)> implementation) where T : class
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            RegisterWeChatConfiguration(services, sectionName, configure);

            foreach (var i in implementation)
            {
                var type = UtilRepository.GetImplementation(i.Item2);
                var serviceDescriptor = new ServiceDescriptor(i.Item1, type, i.Item3);
                services.Add(serviceDescriptor);
            }

            return services;
        }

        private static void RegisterWeChatConfiguration<T>(
            IServiceCollection services,
            string sectionName,
            Action<T> configure) where T : class
        {
            if (configure == null)
            {
                var build = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile(Constant.DEFAULTJSONFILENAME);

                var configuration = build.Build();
                var section = configuration.GetSection(sectionName);
                if (section == null)
                    throw new ArgumentNullException(nameof(section));

                services.Configure<T>(section);
            }
            else
            {
                services.Configure(configure);
            }
        }

        private static bool CheckIfRegisterDependencies(IServiceCollection services, params Type[] dependencies)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var serviceProvider = services.BuildServiceProvider();
            foreach (var type in dependencies)
            {
                var implementation = serviceProvider.GetService(type);
                if (implementation == null)
                    throw new NullReferenceException("Dependenies not Registerred");
            }
            return true;
        }
    }
}
