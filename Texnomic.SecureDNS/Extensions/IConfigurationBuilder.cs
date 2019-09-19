using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Texnomic.SecureDNS.Configurations;

namespace Texnomic.SecureDNS.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddDatabaseConfigurations(this IConfigurationBuilder Builder, Action<DbContextOptionsBuilder> OptionsBuilder)
        {
            return Builder.Add(new DatabaseConfigurationSource(OptionsBuilder));
        }

        public static IConfiguration BuildConfigurations(this IConfigurationBuilder Builder)
        {
            return JsonConfigurationProvider.BuildConfigurations();
        }
    }
}
