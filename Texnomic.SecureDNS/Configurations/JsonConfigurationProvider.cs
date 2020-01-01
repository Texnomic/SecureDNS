using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Texnomic.SecureDNS.Configurations
{
    public static class JsonConfigurationProvider
    {
        public static IConfiguration BuildConfigurations()
        {
            var Stage = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json", false, true)
                .AddJsonFile($"AppSettings.{Stage}.json", false, true)
                .AddUserSecrets<Startup>(true, true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
