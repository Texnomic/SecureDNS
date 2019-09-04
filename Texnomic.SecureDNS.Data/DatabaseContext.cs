using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using Texnomic.SecureDNS.Data.Identity;
using Texnomic.DNS.Models;
using Texnomic.SecureDNS.Data.Models;

namespace Texnomic.SecureDNS.Data
{
    public class DatabaseContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public DbSet<Cache> Cache { get; set; }
        public DbSet<Resolver> Resolvers { get; set; }
        public DbSet<Host> Hosts { get; set; }
        public DbSet<Blacklist> Blacklists { get; set; }

        public DatabaseContext() { }

        public DatabaseContext(DbContextOptions<DatabaseContext> Options) : base(Options) { }


        protected override void OnConfiguring(DbContextOptionsBuilder OptionsBuilder)
        {
            if (OptionsBuilder.IsConfigured) return;

            var Directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            OptionsBuilder.UseSqlite($"Data Source={Directory}\\SecureDNS.sqlite;");
        }

        protected override void OnModelCreating(ModelBuilder ModelBuilder)
        {
            base.OnModelCreating(ModelBuilder);

            ModelBuilder.Entity<Role>().ToTable("Roles");
            ModelBuilder.Entity<Role>().Property(Property => Property.Id).HasColumnName("RoleID");

            ModelBuilder.Entity<RoleClaim>().ToTable("RoleClaims");
            ModelBuilder.Entity<RoleClaim>().Property(Property => Property.Id).HasColumnName("RoleClaimID");

            ModelBuilder.Entity<User>().ToTable("Users");
            ModelBuilder.Entity<User>().Property(Property => Property.Id).HasColumnName("UserID");

            ModelBuilder.Entity<UserRole>().ToTable("UserRoles");
            ModelBuilder.Entity<UserRole>().Property(Property => Property.RoleId).HasColumnName("RoleID");
            ModelBuilder.Entity<UserRole>().Property(Property => Property.UserId).HasColumnName("UserID");

            ModelBuilder.Entity<UserLogin>().ToTable("UserLogins");
            ModelBuilder.Entity<UserLogin>().Property(Property => Property.UserId).HasColumnName("UserID");

            ModelBuilder.Entity<UserClaim>().ToTable("UserClaims");
            ModelBuilder.Entity<UserClaim>().Property(Property => Property.Id).HasColumnName("UserClaimID");
            ModelBuilder.Entity<UserClaim>().Property(Property => Property.UserId).HasColumnName("UserID");

            ModelBuilder.Entity<UserToken>().ToTable("UserTokens");
            ModelBuilder.Entity<UserToken>().Property(Property => Property.UserId).HasColumnName("UserID");

            ModelBuilder.Entity<Cache>()
                        .HasIndex(Cache => Cache.Domain)
                        .IsUnique();

            ModelBuilder.Entity<Cache>()
                        .Property(Cache => Cache.Domain)
                        .HasConversion(Value => Value.ToString(), Value => Domain.FromString(Value));

            ModelBuilder.Entity<Cache>()
                        .Property(Cache => Cache.Response)
                        .HasConversion(Value => Value.ToArray(), Value => Message.FromArray(Value));

            ModelBuilder.Entity<Cache>()
                        .Property(Cache => Cache.Timestamp)
                        .HasConversion(Value => Value.ToString(), Value => DateTime.Parse(Value));

            ModelBuilder.Entity<Resolver>()
                        .HasIndex(Resolver => Resolver.IPAddress)
                        .IsUnique();

            ModelBuilder.Entity<Resolver>()
                        .Property(Resolver => Resolver.IPAddress)
                        .HasConversion(Value => Value.ToString(), Value => IPAddress.Parse(Value));

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
                            Name = "CloudFlare #1",
                            Domain = Domain.FromString("cloudflare-dns.com"),
                            IPAddress = IPAddress.Parse("1.1.1.1"),
                            Hash = Hexadecimal.Parse("04C520708C204250281E7D44417C3079291C635E1D449BC5F7713A2BDED2A2A4B16C3D6AC877B8CB8F2E5053FDF418267F6137EDFFC2BEE90B5DB97EE1DF1CE274"),
                            UDP = true,
                            TCP = true,
                            TLS = true,
                            HTTPS = true,
                            CRYPT = false
                        },
                        new Resolver()
                        {
                            ID = 2,
                            Name = "CloudFlare #2",
                            Domain = Domain.FromString("cloudflare-dns.com"),
                            IPAddress = IPAddress.Parse("1.0.0.1"),
                            Hash = Hexadecimal.Parse("04C520708C204250281E7D44417C3079291C635E1D449BC5F7713A2BDED2A2A4B16C3D6AC877B8CB8F2E5053FDF418267F6137EDFFC2BEE90B5DB97EE1DF1CE274"),
                            UDP = true,
                            TCP = true,
                            TLS = true,
                            HTTPS = true,
                            CRYPT = false
                        },
                        new Resolver()
                        {
                            ID = 3,
                            Name = "Google #1",
                            Domain = Domain.FromString("google-public-dns-a.google.com"),
                            IPAddress = IPAddress.Parse("8.8.8.8"),
                            Hash = Hexadecimal.Parse("3082010A02820101009D1FA4EF5D3E883319ABE79A6DC82BF72A3C1D312EAD5DAB4143368F042D45FA819BC8DD1E3F0227A2A2398DB945E0AB3F1AB143A87F83884BFCCB40230DAA673C2A441ECADF392ABBDB7CA3677DCE04BB92480BBC5F64AACA1A5BB295A66A0EDCE7060F05B88BCA08D0AB36290B192815504E58972D60AFF9F7EC8DAB488107E32E2F89B0DBCDF5CE022530EB32D1740826FB7584A81225BACC70005B74453EADCF3EE2A0AE75E9B3D85A3416A10D7670CD1FFE35D43C03A371DDFBE4F687BCF902DECB68552242F0211FA9572E61D3BBBFEEE782219AE5793608EE06800B347D6340732521EEB813AB544BB603148CEAF7C1AAA62EF4DA2AC23415052C16B90203010001"),
                            UDP = true,
                            TCP = true,
                            TLS = true,
                            HTTPS = true,
                            CRYPT = false
                        },
                        new Resolver()
                        {
                            ID = 4,
                            Name = "Google #2",
                            Domain = Domain.FromString("google-public-dns-b.google.com"),
                            IPAddress = IPAddress.Parse("8.8.4.4"),
                            Hash = Hexadecimal.Parse("3082010A02820101009D1FA4EF5D3E883319ABE79A6DC82BF72A3C1D312EAD5DAB4143368F042D45FA819BC8DD1E3F0227A2A2398DB945E0AB3F1AB143A87F83884BFCCB40230DAA673C2A441ECADF392ABBDB7CA3677DCE04BB92480BBC5F64AACA1A5BB295A66A0EDCE7060F05B88BCA08D0AB36290B192815504E58972D60AFF9F7EC8DAB488107E32E2F89B0DBCDF5CE022530EB32D1740826FB7584A81225BACC70005B74453EADCF3EE2A0AE75E9B3D85A3416A10D7670CD1FFE35D43C03A371DDFBE4F687BCF902DECB68552242F0211FA9572E61D3BBBFEEE782219AE5793608EE06800B347D6340732521EEB813AB544BB603148CEAF7C1AAA62EF4DA2AC23415052C16B90203010001"),
                            UDP = true,
                            TCP = true,
                            TLS = true,
                            HTTPS = true,
                            CRYPT = false
                        },
                        new Resolver()
                        {
                            ID = 5,
                            Name = "Quad9",
                            Domain = Domain.FromString("dns.quad9.net"),
                            IPAddress = IPAddress.Parse("9.9.9.9"),
                            Hash = Hexadecimal.Parse("047D8BD71D03850D1825B3341C29A127D4AC0125488AA0F1EA02B9D8512C086AAC7256ECFA3DA6A09F4909558EACFEB973175C02FB78CC2491946F4323890E1D66"),
                            UDP = true,
                            TCP = true,
                            TLS = true,
                            HTTPS = true,
                            CRYPT = false
                        },
                        new Resolver()
                        {
                            ID = 6,
                            Name = "OpenDNS #1",
                            Domain = Domain.FromString("resolver1-fs.opendns.com"),
                            IPAddress = IPAddress.Parse("208.67.222.123"),
                            Hash = null,
                            UDP = true,
                            TCP = false,
                            TLS = false,
                            HTTPS = false,
                            CRYPT = true
                        },
                        new Resolver()
                        {
                            ID = 7,
                            Name = "OpenDNS #2",
                            Domain = Domain.FromString("resolver2-fs.opendns.com"),
                            IPAddress = IPAddress.Parse("208.67.220.123"),
                            Hash = null,
                            UDP = true,
                            TCP = false,
                            TLS = false,
                            HTTPS = false,
                            CRYPT = true
                        },
                        new Resolver()
                        {
                            ID = 8,
                            Name = "Cloudflare #1",
                            Domain = Domain.FromString("one.one.one.one"),
                            IPAddress = IPAddress.Parse("1.1.1.1"),
                            Hash = null,
                            UDP = true,
                            TCP = false,
                            TLS = false,
                            HTTPS = false,
                            CRYPT = true
                        });
        }
    }
}
