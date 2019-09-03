using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Texnomic.DNS;
using Texnomic.SecureDNS.Areas.Identity;
using Texnomic.SecureDNS.Data;
using Texnomic.SecureDNS.Data.Identity;

namespace Texnomic.SecureDNS.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonConfigurations(this IServiceCollection Services)
        {
            using var Provider = Services.BuildServiceProvider();

            var Environment = Provider.GetRequiredService<IHostEnvironment>();

            var Configurations = new ConfigurationBuilder()
                .AddJsonFile("AppSettings.json", true, true)
                .AddJsonFile($"AppSettings.{Environment.EnvironmentName}.json", true, true)
                .Build();

            Services.AddSingleton(Configurations);

            return Services;
        }

        public static IServiceCollection AddRazorWithJsonSerialization(this IServiceCollection Services)
        {
            Services.AddRazorPages()
                      .AddNewtonsoftJson(Options => Options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

            return Services;
        }

        public static IServiceCollection AddProxyServer(this IServiceCollection Services)
        {
            //Services.AddScoped<DnsOverTls>();

            //Services.AddScoped<ProxyServer<DnsOverTls>>();

            return Services;
        }

        public static IServiceCollection AddIdentity(this IServiceCollection Services)
        {
            Services.AddDefaultIdentity<User>()
                    .AddEntityFrameworkStores<DatabaseContext>();

            Services.AddScoped<AuthenticationStateProvider, RevalidatingAuthenticationStateProvider<User>>();

            return Services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection Services)
        {
            Services.AddEntityFrameworkSqlite();

            var Directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Services.AddDbContext<DatabaseContext>(OptionsBuilder => OptionsBuilder.UseSqlite($"Data Source={Directory}\\SecureDNS.sqlite;"));

            return Services;
        }

        public static IServiceCollection AddTypes(this IServiceCollection Services)
        {
            var Asm = Assembly.GetExecutingAssembly();

            var Namespaces = new SortedSet<string>()
            {
                "Texnomic.SecureDNS.Services",
            };

            foreach (var Type in Asm.GetTypes())
            {
                if (Namespaces.Contains(Type.Namespace))
                {
                    Services.AddScoped(Type);
                }
            }

            return Services;
        }
    }
}
