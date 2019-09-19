using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Texnomic.SecureDNS.Configurations;
using Texnomic.SecureDNS.Data;

namespace Texnomic.SecureDNS.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddDatabaseConfigurations(this IConfigurationBuilder Builder, Action<DbContextOptionsBuilder> OptionsBuilder)
        {
            return Builder.Add(new DatabaseConfigurationSource(OptionsBuilder));
        }
    }
}
