using System;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Texnomic.SecureDNS.Data;
using Texnomic.SecureDNS.Hangfire;

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
        public void Configure(IApplicationBuilder App, IWebHostEnvironment Env, IServiceProvider ServiceProvider, DatabaseContext DatabaseContext)
        {
            if (Env.IsDevelopment())
            {
                DatabaseContext.Database.EnsureDeleted();
                DatabaseContext.Database.EnsureCreated();
                App.UseDeveloperExceptionPage();
            }
            else
            {
                DatabaseContext.Database.EnsureCreated();
            }

            //Configure Hangfire to Use Our JobActivator with ASP.NET IoC Containers
            //http://docs.hangfire.io/en/latest/background-methods/using-ioc-containers.html
            GlobalConfiguration.Configuration.UseActivator(new HangfireJobActivator(ServiceProvider));

            App.UseHangfireDashboard();

            App.UseHangfireServer();

            App.UseStaticFiles();

            App.UseRouting();

            App.UseEndpoints(Endpoints =>
            {
                Endpoints.MapBlazorHub();
                Endpoints.MapFallbackToPage("/_Host");
            });

            Electronize();
        }

        public async void Electronize()
        {
            var Options = new BrowserWindowOptions
            {
                DarkTheme = true,
                AutoHideMenuBar = true,
                //Show = false
            };

            var Window = await Electron.WindowManager.CreateWindowAsync(Options);

            //Window.OnReadyToShow += () => Window.Show();
        }
    }
}
