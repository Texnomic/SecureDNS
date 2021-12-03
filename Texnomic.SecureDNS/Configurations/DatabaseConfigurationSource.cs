using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Texnomic.SecureDNS.Configurations
{
    public class DatabaseConfigurationSource : IConfigurationSource
    {
        private readonly Action<DbContextOptionsBuilder> OptionsBuilder;

        public DatabaseConfigurationSource(Action<DbContextOptionsBuilder> OptionsBuilder)
        {
            this.OptionsBuilder = OptionsBuilder;
        }

        public IConfigurationProvider Build(IConfigurationBuilder Builder)
        {
            return new DatabaseConfigurationProvider(OptionsBuilder);
        }
    }
}
