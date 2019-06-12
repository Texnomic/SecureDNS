using Hangfire;
using Hangfire.SQLite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;

namespace Texnomic.SecureDNS.Hangfire
{
    public static class HangfireExtentions
    {
        public static IServiceCollection AddHangfire(this IServiceCollection Services)
        {
            using (var Provider = Services.BuildServiceProvider())
            {
                var Configurations = Provider.GetRequiredService<IConfigurationRoot>();

                var Connection = $"Data Source={Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\SecureDNS.sqlite;";

                Services.AddHangfire(Configuration => Configuration.UseSQLiteStorage(Connection, new SQLiteStorageOptions
                {
                    //InvisibilityTimeout = TimeSpan.FromDays(1),
                    JobExpirationCheckInterval = TimeSpan.FromDays(1),
                }));

                return Services;
            }
        }
    }
}
