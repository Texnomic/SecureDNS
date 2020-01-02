using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using PipelineNet.MiddlewareResolver;
using Serilog;
using Terminal.Gui;
using Texnomic.DNS.Servers;
using Texnomic.DNS.Servers.ResponsibilityChain;

namespace Texnomic.SecureDNS.CLI
{
    public static class App
    {
        public static ActivatorMiddlewareResolver ActivatorMiddlewareResolver = new ActivatorMiddlewareResolver();
        public static ProxyResponsibilityChain ServerResponsibilityChain = new ProxyResponsibilityChain(ActivatorMiddlewareResolver);
        public static ProxyServer ProxyServer = new ProxyServer(ServerResponsibilityChain, Log.Logger);
        public static CancellationToken CancellationToken = new CancellationToken();

        public static void Run()
        {
            Application.Init();

            var Window = new Window("Texnomic SecureDNS", 1)
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            Application.Top.Add(Window);


            var ServerBindingLabel = new Label("Server Binding: ")
            {
                X = 3,
                Y = 2,
            };

            var ServerBindingText = new TextField("0.0.0.0:53")
            {
                X = Pos.Right(ServerBindingLabel) + 2,
                Y = Pos.Top(ServerBindingLabel),
                Width = 30
            };

            var SeqEndPointLabel = new Label("Seq EndPoint: ")
            {
                X = Pos.Left(ServerBindingLabel),
                Y = Pos.Top(ServerBindingLabel) + 2,
            };

            var SeqEndPointText = new TextField("http://127.0.0.1:5341")
            {
                X = Pos.Right(SeqEndPointLabel) + 4,
                Y = Pos.Top(SeqEndPointLabel),
                Width = 30
            };

            var StartButton = new Button("Start Server", true)
            {
                X = Pos.Left(SeqEndPointLabel),
                Y = 14,
                Clicked = async () =>
                {
                    Log.Logger = new LoggerConfiguration()
                                .WriteTo.Seq(SeqEndPointText.Text.ToString(), compact: true)
                                .CreateLogger();

                    await ProxyServer.StartAsync(CancellationToken);

                    MessageBox.Query(40, 7, "SecureDNS", "Server Started.", "OK");
                }
            };

            var StopButton = new Button("Stop Server")
            {
                X = Pos.Right(StartButton) + 2,
                Y = 14,
                Clicked = async () =>
                {
                    await ProxyServer.StopAsync(CancellationToken);

                    MessageBox.Query(40, 7, "SecureDNS", "Server Stopped.", "OK");
                }
            };

            var MenuBar = new MenuBar(new []
            {
                new MenuBarItem ("Server", new  []
                {
                    new MenuItem ("Start", "", async () =>
                    {
                        Log.Logger = new LoggerConfiguration()
                            .WriteTo.Seq(SeqEndPointText.Text.ToString(), compact: true)
                            .CreateLogger();

                        await ProxyServer.StartAsync(CancellationToken);

                        MessageBox.Query(40, 7, "SecureDNS", "Server Started.", "OK");
                    }),

                    new MenuItem ("Stop", "", async () =>
                    {
                        await ProxyServer.StopAsync(CancellationToken);

                        MessageBox.Query(40, 7, "SecureDNS", "Server Stopped.", "OK");
                    }),
                }),

                new MenuBarItem ("Seq", new  []
                {
                    new MenuItem ("Browse", "", () =>
                    {
                        var Seq = new ProcessStartInfo(SeqEndPointText.Text.ToString())
                        {
                            UseShellExecute = true,
                            Verb = "open"
                        };

                        Process.Start(Seq);
                    }),
                })
            });

            Application.Top.Add(MenuBar);


            Window.Add(ServerBindingLabel,
                ServerBindingText,
                SeqEndPointLabel,
                SeqEndPointText,
                StartButton,
                StopButton);

            Application.Run();
        }
    }
}
