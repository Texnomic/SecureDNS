using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Terminal.Gui;
using Texnomic.SecureDNS.Terminal.Options;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.Extensions.Options;
using Texnomic.SecureDNS.Servers.Proxy;
using Texnomic.SecureDNS.Servers.Proxy.Options;
using Attribute = Terminal.Gui.Attribute;
using Console = Colorful.Console;
using Timer = System.Timers.Timer;


namespace Texnomic.SecureDNS.Terminal
{
    public class GUI : IHostedService, IDisposable
    {
        private readonly IOptionsMonitor<TerminalOptions> Options;

        private readonly IOptionsMonitor<ProxyServerOptions> ServerOptions;

        private readonly UDPServer2 UDPServer;

        private readonly TCPServer2 TCPServer;

        private readonly Timer StatusTimer;

        private readonly CancellationTokenSource CancellationTokenSource;

        public GUI(IOptionsMonitor<TerminalOptions> TerminalOptions, IOptionsMonitor<ProxyServerOptions> ProxyServerOptions, UDPServer2 UDPServer, TCPServer2 TCPServer)
        {
            Console.ReplaceAllColorsWithDefaults();

            Options = TerminalOptions;

            ServerOptions = ProxyServerOptions;

            this.UDPServer = UDPServer;

            this.TCPServer = TCPServer;

            StatusTimer = new Timer(1000);

            CancellationTokenSource = new CancellationTokenSource();
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

        private void Draw()
        {
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

            var ServerBindingText = new TextField(ServerOptions.CurrentValue.IPEndPoint.ToString())
            {
                X = Pos.Right(ServerBindingLabel) + 2,
                Y = Pos.Top(ServerBindingLabel),
                Width = 30
            };

            var StartButton = new Button("Start Server", true)
            {
                X = Pos.AnchorEnd(37),
                Y = Pos.AnchorEnd(1),
            };

            StartButton.Clicked += () => StartServerAsync().RunSynchronously();

            var StopButton = new Button("Stop Server")
            {
                X = Pos.AnchorEnd(16),
                Y = Pos.AnchorEnd(1),
            };

            StopButton.Clicked += () => StopServerAsync().RunSynchronously();

            var MenuBar = new MenuBar(new[]
            {
                new MenuBarItem ("SecureDNS", new  []
                {
                    new MenuItem ("Start", "Server", StartServerAsync().RunSynchronously),

                    new MenuItem ("Stop", "Server", StopServerAsync().RunSynchronously),

                    new MenuItem ("Quit", "System", () => Application.RequestStop()),
                }),

                //new MenuBarItem ("Seq", new  []
                //{
                //    new MenuItem ("Browse", "", () => Browse(SeqEndPointText.Text.ToString())),
                //}),

                new MenuBarItem("About", new[]
                {
                    new MenuItem("Browse", "GitHub", () => Browse("https://github.com/Texnomic/SecureDNS")),
                })
            });

            Application.Top.Add(MenuBar);


            var StatusListView = new ListView()
            {
                X = Pos.Left(ServerBindingLabel),
                Y = Pos.Top(ServerBindingLabel) + 5,
                Width = 40,
                Height = 15,
            };

            //StatusTimer.Elapsed += (Sender, Args) => StatusListView.SetSource(UDPServer.Status().Distinct().ToList());

            //StatusTimer.Elapsed += (Sender, Args) => StatusListView.SetSource(TCPServer.Status().Distinct().ToList());

            Window.Add(ServerBindingLabel,
                ServerBindingText,
                StartButton,
                StopButton,
                StatusListView,
                StatusListView);

            
        }

        public async Task StartAsync(CancellationToken CancellationToken)
        {
            Application.Init();

            Draw();

            StatusTimer.Start();

            Application.Run();

            await Task.Yield();
        }

        public async Task StopAsync(CancellationToken CancellationToken)
        {
            StatusTimer.Stop();

            CancellationTokenSource.Cancel();

            await UDPServer.StopAsync(CancellationTokenSource.Token).ConfigureAwait(false);

            await TCPServer.StopAsync(CancellationTokenSource.Token).ConfigureAwait(false);

            Application.RequestStop();
        }


        public async Task StartServerAsync()
        {
            try
            {
                var Available = CheckPort(ServerOptions.CurrentValue.IPEndPoint.Port);

                if (Available)
                {
                    await UDPServer.StartAsync(CancellationTokenSource.Token).ConfigureAwait(false);

                    await TCPServer.StartAsync(CancellationTokenSource.Token).ConfigureAwait(false);

                    MessageBox.Query(40, 7, "Information", "Server Started.", "OK");
                }
                else
                {
                    MessageBox.ErrorQuery(80, 7, "Error", $"Port {ServerOptions.CurrentValue.IPEndPoint.Port} Already Used.", "OK");
                }
            }
            catch (Exception Error)
            {
                MessageBox.ErrorQuery(80, 7, "Error", Error.Message, "OK");
            }
        }

        public async Task StopServerAsync()
        {
            try
            {
                CancellationTokenSource.Cancel();

                await UDPServer.StopAsync(CancellationTokenSource.Token).ConfigureAwait(false);

                await TCPServer.StopAsync(CancellationTokenSource.Token).ConfigureAwait(false);

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


        private bool IsDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (IsDisposed) return;

            if (Disposing)
            {
                CancellationTokenSource.Dispose();
                UDPServer.Dispose();
                TCPServer.Dispose();
                StatusTimer.Dispose();
            }

            IsDisposed = true;
        }

        ~GUI()
        {
            Dispose(false);
        }
    }
}
