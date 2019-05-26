using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Texnomic.DNS.Server;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Texnomic.SecureDNS.Data;
using Texnomic.SecureDNS.Services;

namespace Texnomic.SecureDNS
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection Services)
        {
            Services.AddRazorPages();
            Services.AddServerSideBlazor();
            Services.AddTelerikBlazor();
            Services.AddJsonConfigurations();
            Services.AddEntityFrameworkSqlite();
            Services.AddHttpClient();
            Services.AddServices();
            Services.AddDnsServer();
            Services.AddHangfire();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder App, IWebHostEnvironment Env, IBackgroundJobClient BackgroundJobs, DnsServer DnsServer, DatabaseContext DatabaseContext)
        {
            if (Env.IsDevelopment())
            {
                App.UseDeveloperExceptionPage();
                //DatabaseContext.Database.EnsureDeleted();
            }

            DatabaseContext.Database.EnsureCreated();

            DnsServer.Listen();

            App.UseHangfireDashboard();

            App.UseHangfireServer();

            App.UseStaticFiles();

            App.UseRouting();

            App.UseEndpoints(Endpoints =>
            {
                Endpoints.MapBlazorHub();
                Endpoints.MapFallbackToPage("/_Host");
            });

            //BackgroundJobs.Enqueue(() => Extentions.StartServer());
        }

    }
}
