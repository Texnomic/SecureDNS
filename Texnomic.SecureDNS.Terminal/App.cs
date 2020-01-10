using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Destructurama;
using Serilog;
using Terminal.Gui;
using Texnomic.DNS.Servers;
using Texnomic.DNS.Servers.Middlewares;
using Texnomic.DNS.Servers.ResponsibilityChain;
using Microsoft.Extensions.DependencyInjection;

using Attribute = Terminal.Gui.Attribute;
using Timer = System.Timers.Timer;
using Texnomic.FilterLists.Enums;

namespace Texnomic.SecureDNS.Terminal
{
    public class App
    {
        public ProxyServer ProxyServer;

        public Settings Settings;

        public Timer StatusTimer;

        public App(Settings Settings)
        {
            this.Settings = Settings;

            StatusTimer = new Timer(500);
        }

        private static readonly ColorScheme SuccessColorScheme = new ColorScheme()
        {
            Normal = Attribute.Make(Color.Black, Color.Green),
            Focus = Attribute.Make(Color.Black, Color.Green)
        };

        private static readonly ColorScheme FailureColorScheme = new ColorScheme()
        {
            Normal = Attribute.Make(Color.White, Color.Red),
            Focus = Attribute.Make(Color.White, Color.Red)
        };

        public void Run()
        {
            Application.Init();

            var Window = new Window("Management", 1)
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

            var ServerBindingText = new TextField(Settings.ServerIPEndPoint)
            {
                X = Pos.Right(ServerBindingLabel) + 2,
                Y = Pos.Top(ServerBindingLabel),
                Width = 30
            };

            ServerBindingText.Changed += (Sender, Args) => CheckIPEndPoint(ServerBindingText);

            var SeqEndPointLabel = new Label("Seq EndPoint: ")
            {
                X = Pos.Left(ServerBindingLabel),
                Y = Pos.Top(ServerBindingLabel) + 2,
            };

            var SeqEndPointText = new TextField(Settings.SeqUriEndPoint)
            {
                X = Pos.Right(SeqEndPointLabel) + 4,
                Y = Pos.Top(SeqEndPointLabel),
                Width = 30,
            };

            SeqEndPointText.Changed += (Sender, Args) => CheckUri(SeqEndPointText);

            var StartButton = new Button("Start Server", true)
            {
                X = Pos.AnchorEnd(37),
                Y = Pos.AnchorEnd(1),
                Clicked = async () => await Start()
            };

            var StopButton = new Button("Stop Server")
            {
                X = Pos.AnchorEnd(16),
                Y = Pos.AnchorEnd(1),
                Clicked = async () => await Stop()
            };

            var MenuBar = new MenuBar(new[]
            {
                new MenuBarItem ("SecureDNS", new  []
                {
                    new MenuItem ("Start", "Server", async () => await Start()),

                    new MenuItem ("Stop", "Server", async () => await Stop()),

                    new MenuItem ("Quite", "System", Application.RequestStop),
                }),

                new MenuBarItem ("Seq", new  []
                {
                    new MenuItem ("Browse", "", () => Browse(SeqEndPointText.Text.ToString())),
                }),

                new MenuBarItem("About", new[]
                {
                    new MenuItem("Browse", "GitHub", () => Browse("https://github.com/Texnomic/SecureDNS")),
                })
            });

            Application.Top.Add(MenuBar);


            var StatusListView = new ListView()
            {
                X = Pos.Left(SeqEndPointLabel),
                Y = Pos.Top(SeqEndPointLabel) + 5,
                Width = 40,
                Height = 15,
            };

            StatusTimer.Elapsed += (Sender, Args) => StatusListView.SetSource(ProxyServer.Status().Distinct().ToList());

            Window.Add(ServerBindingLabel,
                ServerBindingText,
                SeqEndPointLabel,
                SeqEndPointText,
                StartButton,
                StopButton,
                StatusListView,
                StatusListView);

            Application.Run();
        }

        private async Task Start()
        {
            try
            {
                var Available = CheckPort(IPEndPoint.Parse(Settings.ServerIPEndPoint).Port);

                if (Available)
                {
                    Log.Logger = new LoggerConfiguration()
                        .Destructure.UsingAttributes()
                        .WriteTo.Seq(Settings.SeqUriEndPoint.ToString(), compact: true)
                        .CreateLogger();

                    var FilterTags = new Tags[]
                    {
                        Tags.Malware,
                        Tags.Phishing,
                        Tags.Crypto,
                     };

                    var ServiceCollection = new ServiceCollection();
                    ServiceCollection.AddSingleton(Log.Logger);
                    ServiceCollection.AddSingleton(FilterTags);
                    ServiceCollection.AddSingleton<FilterMiddleware>();
                    ServiceCollection.AddSingleton<GoogleHTTPsMiddleware>();

                    var ServerMiddlewareActivator = new ServerMiddlewareActivator(ServiceCollection.BuildServiceProvider());

                    var Middlewares = new List<Type>()
                    {
                        typeof(FilterMiddleware),
                        typeof(GoogleHTTPsMiddleware),
                    };

                    var ServerResponsibilityChain = new ProxyResponsibilityChain(Middlewares, ServerMiddlewareActivator);

                    ProxyServer = new ProxyServer(ServerResponsibilityChain, Log.Logger, IPEndPoint.Parse(Settings.ServerIPEndPoint));

                    await ProxyServer.StartAsync(Settings.CancellationTokenSource.Token);

                    StatusTimer.Start();

                    MessageBox.Query(40, 7, "Information", "Server Started.", "OK");
                }
                else
                {
                    MessageBox.ErrorQuery(80, 7, "Error", $"Port {IPEndPoint.Parse(Settings.ServerIPEndPoint).Port} Already Used.", "OK");
                }
            }
            catch (Exception Error)
            {
                MessageBox.ErrorQuery(80, 7, "Error", Error.Message, "OK");
            }
        }

        private async Task Stop()
        {
            try
            {
                Settings.CancellationTokenSource.Cancel(false);

                await ProxyServer.StopAsync(Settings.CancellationTokenSource.Token);

                StatusTimer.Stop();

                MessageBox.Query(40, 7, "Information", "Server Stopped.", "OK");
            }
            catch (Exception Error)
            {
                MessageBox.ErrorQuery(80, 7, "Error", Error.Message, "OK");
            }
        }

        private static void Browse(string Url)
        {
            var Ps = new ProcessStartInfo(Url)
            {
                UseShellExecute = true,
                Verb = "open"
            };

            Process.Start(Ps);
        }

        private static bool CheckPort(int Port)
        {
            return IPGlobalProperties.GetIPGlobalProperties()
                                     .GetActiveUdpListeners()
                                     .All(Connection => Connection.Port != Port);
        }

        private void CheckIPEndPoint(TextField TextField)
        {
            var IsValid = IPEndPoint.TryParse(TextField.Text.ToString(), out var Result);

            TextField.ColorScheme = IsValid ? SuccessColorScheme : FailureColorScheme;

            Settings.ServerIPEndPoint = IsValid ? Result.ToString() : Settings.ServerIPEndPoint;
        }

        private void CheckUri(TextField TextField)
        {
            var IsValid = Uri.TryCreate(TextField.Text.ToString(), UriKind.Absolute, out var Result);

            IsValid = IsValid && Result.Scheme == Uri.UriSchemeHttp;

            TextField.ColorScheme = IsValid ? SuccessColorScheme : FailureColorScheme;

            Settings.SeqUriEndPoint = IsValid ? Result.ToString() : Settings.SeqUriEndPoint;
        }
    }
}
