using System;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Syncfusion.Blazor;
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
            Services.AddSyncfusionBlazor();
            Services.AddBlazorise(Options => { Options.ChangeTextOnKeyPress = true; });
            Services.AddBootstrapProviders();
            Services.AddFontAwesomeIcons();
            Services.AddJsonConfigurations();
            Services.AddLogging();
            Services.AddDatabase();
            Services.AddIdentity();
            Services.AddTypes();
            Services.AddControllers();
            Services.AddRazorWithJsonSerialization();
            Services.AddServerSideBlazor();
            Services.AddHttpClient();
            Services.AddProxyServer();
            Services.AddHangfire();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder App, IWebHostEnvironment Env, IServiceProvider ServiceProvider, IConfiguration Configuration, DatabaseContext DatabaseContext)
        {
            if (Env.IsDevelopment())
            {
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

            //https://help.syncfusion.com/common/essential-studio/licensing/license-key
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(Configuration["Syncfusion:LicenseKey"]);

            //Configure Hangfire to Use Our JobActivator with ASP.NET IoC Containers
            //http://docs.hangfire.io/en/latest/background-methods/using-ioc-containers.html
            GlobalConfiguration.Configuration.UseSerilogLogProvider()
                                             .UseActivator(new HangfireJobActivator(ServiceProvider));

            App.UseSerilogRequestLogging();
            App.UseHttpsRedirection();
            App.UseHangfireDashboard();
            App.UseHangfireServer();
            App.UseStaticFiles();
            App.UseRouting();
            App.ApplicationServices.UseBootstrapProviders();
            App.ApplicationServices.UseFontAwesomeIcons();
            App.UseAuthentication();
            App.UseAuthorization();

            App.UseEndpoints(Endpoints =>
            {
                Endpoints.MapControllers();
                Endpoints.MapBlazorHub();
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
