using System.Threading;
using System.Threading.Tasks;
using Destructurama;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Texnomic.DNS.Servers.Middlewares;
using Texnomic.DNS.Servers.ResponsibilityChain;
using System.Collections.Generic;
using System.Net;
using System;
using Texnomic.FilterLists.Enums;

namespace Texnomic.DNS.Servers.Tests
{
    [TestClass]
    public class Proxy
    {
        private ProxyServer ProxyServer;
        private CancellationTokenSource CancellationTokenSource;

        [TestInitialize]
        public void Initialize()
        {
            //Stop-Service -DisplayName 'Docker Desktop Service'
            //Set-Service -Name 'Internet Connection Sharing (ICS)' -StartupType Disabled
            //Stop-Service -DisplayName 'Internet Connection Sharing (ICS)'
            //Resolve-DnsName -Name mail.google.com -Server 127.0.0.1 -Type A

            //Log.Logger = new LoggerConfiguration()
            //                .Destructure.UsingAttributes()
            //                .WriteTo.Seq("http://127.0.0.1:5341", compact: true)
            //                .CreateLogger();


            //var FilterTags = new Tags[]
            //{
            //    Tags.Malware,
            //    Tags.Phishing,
            //    Tags.Crypto,
            //};

            //var ServiceCollection = new ServiceCollection();
            //ServiceCollection.AddSingleton(Log.Logger);
            //ServiceCollection.AddSingleton(FilterTags);
            //ServiceCollection.AddSingleton<FilterListsMiddleware>();
            //ServiceCollection.AddSingleton<GoogleHTTPsMiddleware>();

            //var ServerMiddlewareActivator = new ServerMiddlewareActivator(ServiceCollection.BuildServiceProvider());

            //var Middlewares = new List<Type>()
            //{
            //    typeof(FilterListsMiddleware),
            //    typeof(GoogleHTTPsMiddleware),
            //};

            //var ServerResponsibilityChain = new ProxyResponsibilityChain(Middlewares, ServerMiddlewareActivator);

            //CancellationTokenSource = new CancellationTokenSource();

            //ProxyServer = new ProxyServer(ServerResponsibilityChain, Log.Logger, IPEndPoint.Parse("0.0.0.0:53"));
        }

        [TestMethod]
        public async Task Start()
        {
            await ProxyServer.StartAsync(CancellationTokenSource.Token);
        }
    }
}
