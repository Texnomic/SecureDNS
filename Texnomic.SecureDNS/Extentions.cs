using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using System.Collections.Generic;
using Texnomic.SecureDNS.Resolvers;
using Texnomic.DNS;

namespace Texnomic.SecureDNS
{
    public static class Extentions
    {
        public static IServiceCollection AddJsonConfigurations(this IServiceCollection Services)
        {
            using (var Provider = Services.BuildServiceProvider())
            {
                var Environment = Provider.GetRequiredService<IHostEnvironment>();

                var Configurations = new ConfigurationBuilder()
                                        .AddJsonFile("AppSettings.json", true, true)
                                        .AddJsonFile($"AppSettings.{Environment.EnvironmentName}.json", true, true)
                                        .Build();

                Services.AddSingleton(Configurations);

                return Services;
            }
        }

        public static IServiceCollection AddDnsServer(this IServiceCollection Services)
        {
            Services.AddSingleton<DnsOverTls>();

            Services.AddSingleton<DnsServer<DnsOverTls>>();

            return Services;
        }



        public static IServiceCollection AddServices(this IServiceCollection Services)
        {
            var Asm = Assembly.GetExecutingAssembly();

            var Namespaces = new SortedSet<string>()
            {
                "Texnomic.SecureDNS.Services",
                "Texnomic.SecureDNS.Data",
            };

            foreach (Type Type in Asm.GetTypes())
            {
                if (Namespaces.Contains(Type.Namespace))
                {
                    Services.AddSingleton(Type);
                }
            }

            return Services;
        }
    }
}
