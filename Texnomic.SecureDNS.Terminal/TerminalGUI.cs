using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Destructurama;
using Serilog;
using Terminal.Gui;
using Texnomic.SecureDNS.Terminal.Options;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.Extensions.Options;
using Texnomic.DNS.Servers;

using Attribute = Terminal.Gui.Attribute;
using Console = Colorful.Console;
using Timer = System.Timers.Timer;

namespace Texnomic.SecureDNS.Terminal
{
    public class TerminalGUI : IHostedService, IDisposable
    {
        private readonly TerminalOptions Options;

        private readonly ProxyServer Server;

        private readonly Timer StatusTimer;

        private readonly CancellationTokenSource CancellationTokenSource;

        public TerminalGUI(IOptionsMonitor<TerminalOptions> TerminalOptions, ProxyServer ProxyServer)
        {
            Console.ReplaceAllColorsWithDefaults();

            Options = TerminalOptions.CurrentValue;

            Server = ProxyServer;

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

            var ServerBindingText = new TextField(Options.ServerIPEndPoint)
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

            var SeqEndPointText = new TextField(Options.SeqUriEndPoint)
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
                Clicked = async () => await StartServerAsync()
            };

            var StopButton = new Button("Stop Server")
            {
                X = Pos.AnchorEnd(16),
                Y = Pos.AnchorEnd(1),
                Clicked = async () => await StopServerAsync()
            };

            var MenuBar = new MenuBar(new[]
            {
                new MenuBarItem ("SecureDNS", new  []
                {
                    new MenuItem ("Start", "Server", async () => await StartServerAsync()),

                    new MenuItem ("Stop", "Server", async () => await StopServerAsync()),

                    new MenuItem ("Quit", "System", Application.RequestStop),
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

            StatusTimer.Elapsed += (Sender, Args) => StatusListView.SetSource(Server.Status().Distinct().ToList());

            Window.Add(ServerBindingLabel,
                ServerBindingText,
                SeqEndPointLabel,
                SeqEndPointText,
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
        }

        public async Task StopAsync(CancellationToken CancellationToken)
        {
            StatusTimer.Stop();

            CancellationTokenSource.Cancel();

            await Server.StopAsync(CancellationTokenSource.Token);

            Application.RequestStop();
        }


        public async Task StartServerAsync()
        {
            try
            {
                var Available = CheckPort(IPEndPoint.Parse(Options.ServerIPEndPoint).Port);

                if (Available)
                {
                    await Server.StartAsync(CancellationTokenSource.Token);

                    MessageBox.Query(40, 7, "Information", "Server Started.", "OK");
                }
                else
                {
                    MessageBox.ErrorQuery(80, 7, "Error", $"Port {IPEndPoint.Parse(Options.ServerIPEndPoint).Port} Already Used.", "OK");
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

                await Server.StopAsync(CancellationTokenSource.Token);

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

            Options.ServerIPEndPoint = IsValid ? Result.ToString() : Options.ServerIPEndPoint;
        }

        private void CheckUri(TextField TextField)
        {
            var IsValid = Uri.TryCreate(TextField.Text.ToString(), UriKind.Absolute, out var Result);

            IsValid = IsValid && Result.Scheme == Uri.UriSchemeHttp;

            TextField.ColorScheme = IsValid ? SuccessColorScheme : FailureColorScheme;

            Options.SeqUriEndPoint = IsValid ? Result.ToString() : Options.SeqUriEndPoint;
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
                Server.Dispose();
                StatusTimer.Dispose();
            }

            IsDisposed = true;
        }

        ~TerminalGUI()
        {
            Dispose(false);
        }
    }
}
