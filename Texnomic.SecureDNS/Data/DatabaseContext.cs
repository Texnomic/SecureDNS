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

            ModelBuilder.Entity<Resolver>()
                        .Property(Resolver => Resolver.Domain)
                        .HasConversion(Value => Value.ToString(), Value => Domain.FromString(Value));

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
                            Domain = Domain.FromString("cloudflare-dns.com"),
                            IPEndPoint = IPEndPoint.Parse("1.1.1.1:853"),
                            Hash = Hexadecimal.Parse("04C520708C204250281E7D44417C3079291C635E1D449BC5F7713A2BDED2A2A4B16C3D6AC877B8CB8F2E5053FDF418267F6137EDFFC2BEE90B5DB97EE1DF1CE274")
                        },
                        new Resolver()
                        {
                            ID = 2,
                            Name = "Google TLS",
                            Domain = Domain.FromString("dns.google"),
                            IPEndPoint = IPEndPoint.Parse("8.8.8.8:853"),
                            Hash = Hexadecimal.Parse("3082010A02820101009D1FA4EF5D3E883319ABE79A6DC82BF72A3C1D312EAD5DAB4143368F042D45FA819BC8DD1E3F0227A2A2398DB945E0AB3F1AB143A87F83884BFCCB40230DAA673C2A441ECADF392ABBDB7CA3677DCE04BB92480BBC5F64AACA1A5BB295A66A0EDCE7060F05B88BCA08D0AB36290B192815504E58972D60AFF9F7EC8DAB488107E32E2F89B0DBCDF5CE022530EB32D1740826FB7584A81225BACC70005B74453EADCF3EE2A0AE75E9B3D85A3416A10D7670CD1FFE35D43C03A371DDFBE4F687BCF902DECB68552242F0211FA9572E61D3BBBFEEE782219AE5793608EE06800B347D6340732521EEB813AB544BB603148CEAF7C1AAA62EF4DA2AC23415052C16B90203010001")
                        }
                        ,
                        new Resolver()
                        {
                            ID = 3,
                            Name = "Quad9 TLS",
                            Domain = Domain.FromString("dns.quad9.net"),
                            IPEndPoint = IPEndPoint.Parse("9.9.9.9:853"),
                            Hash = Hexadecimal.Parse("047D8BD71D03850D1825B3341C29A127D4AC0125488AA0F1EA02B9D8512C086AAC7256ECFA3DA6A09F4909558EACFEB973175C02FB78CC2491946F4323890E1D66")
                        });
        }
    }
}
