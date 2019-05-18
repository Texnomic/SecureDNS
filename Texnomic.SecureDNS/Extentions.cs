using DNS.Server;
using Hangfire;
using Hangfire.SQLite;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;
using Texnomic.DNS.Data;
using Texnomic.DNS.Resolvers;

namespace Texnomic.DNS
{
    public static class Extentions
    {
        public static IServiceCollection AddJsonConfigurations(this IServiceCollection Services)
        {
            using (var Provider = Services.BuildServiceProvider())
            {
                var Environment = Provider.GetRequiredService<IHostEnvironment>();

                var Configurations = new ConfigurationBuilder()
                                        .AddJsonFile("AppSettings.json", true, true)
                                        .AddJsonFile($"AppSettings.{Environment.EnvironmentName}.json", true, true)
                                        .Build();

                Services.AddSingleton(Configurations);

                return Services;
            }
        }


        public static IServiceCollection AddSqlite(this IServiceCollection Services)
        {
            using (var Provider = Services.BuildServiceProvider())
            {
                var Configurations = Provider.GetRequiredService<IConfigurationRoot>();

                Services.AddEntityFrameworkSqlite();

                Services.AddDbContext<DatabaseContext>(Options => Options.UseSqlite(Configurations.GetConnectionString("DefaultConnection")));

                //var Env = Provider.GetRequiredService<IWebHostEnvironment>();

                //if (Env.IsDevelopment())
                //{
                //    var DatabaseContext = Provider.GetRequiredService<DatabaseContext>();

                //    DatabaseContext.Database.EnsureDeleted();

                //    DatabaseContext.Database.EnsureCreated();
                //}

                return Services;
            }
        }

        public static IServiceCollection AddDnsServer(this IServiceCollection Services)
        {
            // Proxy to google's DNS
            var MasterFile = new MasterFile();
            var Resolver = new TlsRequestResolver(new IPEndPoint(IPAddress.Parse("1.1.1.1"), 853));
            var Server = new DnsServer(Resolver);

            // Resolve these domain to localhost
            //MasterFile.AddIPAddressResourceRecord("google.com", "127.0.0.1");
            //MasterFile.AddIPAddressResourceRecord("github.com", "127.0.0.1");

            Server.Listening += (sender, e) => Console.WriteLine("Listening");
            Server.Requested += (sender, e) => Console.WriteLine(e.Request);
            Server.Responded += (sender, e) => Console.WriteLine("{0} => {1}", e.Request, e.Response);
            Server.Errored += (sender, e) => Console.WriteLine(e.Exception.Message);

            Services.AddSingleton(MasterFile);
            Services.AddSingleton(Resolver);
            Services.AddSingleton(Server);
            //Server.Listen();
            //Services.AddSingleton(Server.Listen());

            return Services;
        }

        public static IServiceCollection AddHangfire(this IServiceCollection Services)
        {
            using (var Provider = Services.BuildServiceProvider())
            {
                var Configurations = Provider.GetRequiredService<IConfigurationRoot>();

                Services.AddHangfire(Configuration => Configuration.UseSQLiteStorage(Configurations.GetConnectionString("DefaultConnection")));
                Services.AddHangfireServer();

                return Services;
            }
        }

        [DisableConcurrentExecution(60)]
        public static async Task StartServer()
        {
            try
            {
                var MasterFile = new MasterFile();
                var Resolver = new TlsRequestResolver(new IPEndPoint(IPAddress.Parse("1.1.1.1"), 853));
                var Server = new DnsServer(Resolver);

                Server.Listening += (sender, e) => Console.WriteLine("Listening");
                Server.Requested += (sender, e) => Console.WriteLine(e.Request);
                Server.Responded += (sender, e) => Console.WriteLine("{0} => {1}", e.Request, e.Response);
                Server.Errored += (sender, e) => Console.WriteLine(e.Exception.Message);

                await Server.Listen();
            }
            catch (Exception Error)
            {
                Console.WriteLine(Error.Message);
            }
        }
    }
}
