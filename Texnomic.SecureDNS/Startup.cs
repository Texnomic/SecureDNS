using System;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Texnomic.SecureDNS.Data;
using Texnomic.SecureDNS.Extensions;
using Texnomic.SecureDNS.Hangfire;

namespace Texnomic.SecureDNS
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection Services)
        {

            Services.AddDatabase();
            Services.AddIdentity();
            Services.AddTypes();
            Services.AddRazorPages();
            Services.AddServerSideBlazor();
            Services.AddTelerikBlazor();
            Services.AddJsonConfigurations();
            Services.AddHttpClient();
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
                App.UseDatabaseErrorPage();
            }
            else
            {
                DatabaseContext.Database.EnsureCreated();
                App.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                App.UseHsts();
            }

            //Configure Hangfire to Use Our JobActivator with ASP.NET IoC Containers
            //http://docs.hangfire.io/en/latest/background-methods/using-ioc-containers.html
            GlobalConfiguration.Configuration.UseActivator(new HangfireJobActivator(ServiceProvider));

            App.UseHangfireDashboard();
            App.UseHangfireServer();
            App.UseHttpsRedirection();
            App.UseStaticFiles();
            App.UseRouting();
            App.UseAuthentication();
            App.UseAuthorization();

            App.UseEndpoints(Endpoints =>
            {
                Endpoints.MapControllers();
                Endpoints.MapBlazorHub<App>(selector: "App");
                Endpoints.MapFallbackToPage("/_Host");
            });

            ElectronizeAsync().ConfigureAwait(false);
        }

        public async Task ElectronizeAsync()
        {
            var Options = new BrowserWindowOptions
            {
                DarkTheme = true,
                AutoHideMenuBar = true,
            };

            await Electron.WindowManager.CreateWindowAsync(Options);
        }
    }
}
