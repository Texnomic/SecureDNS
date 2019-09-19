using Microsoft.EntityFrameworkCore;
using Texnomic.SecureDNS.Data.Models;

namespace Texnomic.SecureDNS.Data
{
    public class ConfigurationDatabaseContext : DbContext
    {
        public ConfigurationDatabaseContext(DbContextOptions Options) : base(Options) { }

        public DbSet<Configuration> Configurations { get; set; }
    }
}
