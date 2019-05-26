using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Policy;
using System.Threading.Tasks;
using Texnomic.DNS.Protocol;
using Texnomic.SecureDNS.Models;

namespace Texnomic.SecureDNS.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Cache> Cache { get; set; }
        public DbSet<Resolver> Resolvers { get; set; }
        public DbSet<Host> Hosts { get; set; }
        public DbSet<Blacklist> Blacklists { get; set; }

        public DatabaseContext() : base()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder OptionsBuilder)
        {
            OptionsBuilder.UseSqlite($"Data Source={Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\SecureDNS.sqlite;");
        }

        protected override void OnModelCreating(ModelBuilder ModelBuilder)
        {
            ModelBuilder.Entity<Cache>()
                        .HasIndex(Cache => Cache.Domain)
                        .IsUnique();

            ModelBuilder.Entity<Cache>()
                        .Property(Cache => Cache.Domain)
                        .HasConversion(Value => Value.ToString(), Value => Domain.FromString(Value));

            ModelBuilder.Entity<Cache>()
                        .Property(Cache => Cache.Response)
                        .HasConversion(Value => Value.ToArray(), Value => Response.FromArray(Value));

            ModelBuilder.Entity<Cache>()
                        .Property(Cache => Cache.Timestamp)
                        .HasConversion(Value => Value.ToString(), Value => DateTime.Parse(Value));

            ModelBuilder.Entity<Resolver>()
                        .HasIndex(Resolver => Resolver.IPEndPoint)
                        .IsUnique();

            ModelBuilder.Entity<Resolver>()
                        .Property(Resolver => Resolver.IPEndPoint)
                        .HasConversion(Value => Value.ToString(), Value => IPEndPoint.Parse(Value));

            ModelBuilder.Entity<Resolver>()
                        .Property(Resolver => Resolver.Hash)
                        .HasConversion(Value => Value.Raw, Value => new Hexadecimal(Value));

            ModelBuilder.Entity<Host>()
                        .HasIndex(Host => Host.Domain)
                        .IsUnique();

            ModelBuilder.Entity<Host>()
                        .Property(Host => Host.IPAddress)
                        .HasConversion(Value => Value.ToString(), Value => IPAddress.Parse(Value));

            ModelBuilder.Entity<Host>()
                        .Property(Host => Host.Domain)
                        .HasConversion(Value => Value.ToString(), Value => Domain.FromString(Value));

            ModelBuilder.Entity<Blacklist>()
                        .HasIndex(Blacklist => Blacklist.Domain)
                        .IsUnique();

            ModelBuilder.Entity<Blacklist>()
                        .Property(Blacklist => Blacklist.Domain)
                        .HasConversion(Value => Value.ToString(), Value => Domain.FromString(Value));

            ModelBuilder.Entity<Blacklist>()
                        .Property(Blacklist => Blacklist.Timestamp)
                        .HasConversion(Value => Value.ToString(), Value => DateTime.Parse(Value));

            ModelBuilder.Entity<Host>()
                        .HasData(new Host()
                        {
                            ID = 1,
                            Domain = Domain.FromString("www.secure.dns"),
                            IPAddress = IPAddress.Parse("127.0.0.1")
                        });

            ModelBuilder.Entity<Resolver>()
                        .HasData(new Resolver()
                        {
                            ID = 1,
                            Name = "CloudFlare TLS",
                            IPEndPoint = IPEndPoint.Parse("1.1.1.1:853"),
                            Hash = Hexadecimal.Parse("")
                        });
        }
    }
}
