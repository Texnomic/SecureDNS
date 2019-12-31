using System;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using Hangfire;
using Hangfire.SQLite;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Texnomic.SecureDNS.Areas.Identity;
using Texnomic.SecureDNS.Configurations;
using Texnomic.SecureDNS.Data;
using Texnomic.SecureDNS.Data.Identity;

namespace Texnomic.SecureDNS.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHangfire(this IServiceCollection Services)
        {
            var Connection = $"Data Source={Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\SecureDNS.sqlite;";

            Services.AddHangfire(Configuration => Configuration.UseSQLiteStorage(Connection, new SQLiteStorageOptions
            {
                //InvisibilityTimeout = TimeSpan.FromDays(1),
                JobExpirationCheckInterval = TimeSpan.FromDays(1),
            }));

            return Services;
        }

        public static IServiceCollection AddJsonConfigurations(this IServiceCollection Services)
        {
            Services.AddSingleton(JsonConfigurationProvider.BuildConfigurations());

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

            Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<User>>();

            return Services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection Services)
        {
            Services.AddEntityFrameworkSqlite();

            var Directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Services.AddDbContext<DatabaseContext>(OptionsBuilder =>
            {
                OptionsBuilder.UseLazyLoadingProxies()
                              .UseSqlite($"Data Source={Directory}\\SecureDNS.sqlite;");
            });

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
