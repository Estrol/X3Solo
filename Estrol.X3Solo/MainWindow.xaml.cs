using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Estrol.X3Solo.Modules;
using Estrol.X3Solo.Server;
using Estrol.X3Solo.Library;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Estrol.X3Solo {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly Configuration config;
        private readonly Main server;
        private Process GameProc;
        private Presence presence;

        // Flag
        private bool loop;

        // UI
        private ConsoleWindow UiConsole;

        public MainWindow() {
            InitializeComponent();

            config = new();
            UiConsole = new();
            UiConsole.OnMessage += (object o, string message) => {
                if (!server.Ready) {
                    Log.Write("Server not yet running!");
                    return;
                }

                server.SendMessage(message);
            };

            Console.SetOut(new MultiTextWriter(new ControlWriter(UiConsole), Console.Out));

            m_main.Visibility = Visibility.Visible;
            m_setting.Visibility = Visibility.Hidden;
            m_about.Visibility = Visibility.Hidden;
            m_about_us.Text = "                            X3Solo\r\n   " +
                "               X3-JAM Solo Version\r\n         Not supp" +
                "ort Multiplayer Session\r\n\r\nSupported Client Version" +
                "s\r\n- 9you O2-JAM 1.8 and 1.5\r\n\r\nDeveloper:\r\n - " +
                "Estrol (Programing)\r\n - MatVeiQaaa (Reverse engineeri" +
                "ng)";

            server = new Main();
            f_port.Text = server.m_config.ServerPort.ToString();
            f_name.Text = server.m_config.Name.ToString();
            f_rank.Text = server.m_config.Rank.ToString();
            f_level.Text = server.m_config.Level.ToString();

            server.OnError += (object o, Exception e) => {
                if (GameProc != null) {
                    ProcessUtil.SuspendProcess(GameProc.Id);
                }

                if (e is InvalidOpcodeException err) {
                    if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\crashlog")) {
                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\crashlog");
                    }

                    string log = "";
                    log += "[Crash Summary]\n";
                    log += $"Opcode: {err.Opcode:X4}\n";
                    log += $"Full Data: {err.FullDataString}\n\n";
                    log += "Report this to Estrol#0021 (He might busy or idk, but he should respond)";

                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + $"\\crashlog\\crash-{Guid.NewGuid()}.txt", log);
                }

                MessageBox.Show(e.Message + "\n\nProgram will now exit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                GameProc?.Kill();
                Environment.Exit(500);
            };
        }

        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private void Run(object sender, RoutedEventArgs e) {
            if (GameProc != null) {
                MessageBox.Show("The Game Already Started!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\OTwo.exe")) {
                MessageBox.Show("OTwo.exe not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Task ts = new(() => {
                string IPAddr = "127.0.0.1";
                int GamePort = config.ServerPort;
                ClientCheck conf = new();
                conf.GetVersion();

                ProcessStartInfo ps = new() {
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Normal,
                    FileName = "OTwo.exe",
                    Arguments = "1 127.0.0.1 o2jam/patch "
                        + $"{IPAddr}:80 "
                        + "1 1 1 1 1 1 1 1 "
                        + $"{IPAddr} {GamePort} "
                        + $"{IPAddr} {GamePort} "
                        + $"{IPAddr} {GamePort} "
                        + $"{IPAddr} {GamePort} "
                        + $"{IPAddr} {GamePort} "
                        + $"{IPAddr} {GamePort} "
                        + $"{IPAddr} {GamePort} "
                        + $"{IPAddr} {GamePort} "
                };

                server.Intialize(conf);
                GameProc = Process.Start(ps);

                if (GameProc == null) {
                    MessageBox.Show("Game failed to start");
                    server.Stop();
                } else {
                    try {
                        GameProc.WaitForInputIdle();

                        if (server.Version == "1.8") {
                            Log.Write("Detected client version 1.8, success patching!");
                            bool success = CryptModule.RektO2JamCrypt(GameProc);
                            if (!success) {
                                bool retry = CryptModule.RektO2JamCrypt(GameProc);
                                if (!retry) {
                                    throw CryptModule.Error;
                                }
                            }
                        } else if (server.Version == "1.5") {
                            Log.Write("Detected client version 1.5, not required to patch!");
                        } else {
                            throw new PatchException("Unknown client version!");
                        }

                        bool result = SetWindowText(GameProc.MainWindowHandle, $"{GameProc.MainWindowTitle} [with X3Solo Beta-0.5 for {server.Version} Client]");
                        if (!result) {
                            int err = Marshal.GetLastWin32Error();
                            Log.Write($"Window title rename failed: {result} {err}");
                        }

                        loop = true;
                        presence = new(GameProc, conf.Music);
                        Task.Run(LoopTask);

                        GameProc.WaitForExit();

                        loop = false;
                        presence = null;
                        server.Stop();
                        GameProc = null;
                    } catch (Exception e) {
                        ProcessUtil.SuspendProcess(GameProc.Id);

                        if (e is PatchException err) {
                            MessageBox.Show("Failed to patch:\n" + err.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                            GameProc?.Kill();
                            server.Stop();
                            GameProc = null;
                        } else {
                            MessageBox.Show(e.Message + "\n\nProgram will now exit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                            GameProc?.Kill();
                            Environment.Exit(0);
                        }
                    }
                }
            });

            ts.Start();
        }

        private void LoopTask() {
            var defcached = $"{GameProc.MainWindowTitle} [with X3Solo Beta-0.5 for {server.Version} Client]";
            var cached = "";
            while (loop) {
                try {
                    var state = presence.Update();

                    if (state.IsPlaying) {
                        if (cached != state.Chart.Title) {
                            cached = state.Chart.Title;

                            bool result = SetWindowText(GameProc.MainWindowHandle, $"{GameProc.MainWindowTitle} - {state.Chart.Artist} - {state.Chart.Title} [{state.Chart.DifficultyText}] [with X3Solo Beta-0.5 for {server.Version} Client]");
                            if (!result) {
                                int err = Marshal.GetLastWin32Error();
                                Log.Write($"Window title rename failed: {result} {err}");
                            }
                        }
                    } else {
                        if (cached != defcached) {
                            cached = defcached;

                            bool result = SetWindowText(GameProc.MainWindowHandle, cached);
                            if (!result) {
                                int err = Marshal.GetLastWin32Error();
                                Log.Write($"Window title rename failed: {result} {err}");
                            }
                        }
                    }
                } catch (Exception) {
                    Log.Write("Presence failed!");
                }

                Thread.Sleep(1000);
            }
        }

        private void ToggleSetting(object sender, RoutedEventArgs e) {
            if (m_setting.Visibility == Visibility.Hidden) {
                m_setting.Visibility = Visibility.Visible;
            } else {
                m_setting.Visibility = Visibility.Hidden;
            }
        }

        private void ToggleAbout(object sender, RoutedEventArgs e) {
            if (m_about.Visibility == Visibility.Hidden) {
                m_about.Visibility = Visibility.Visible;
            } else {
                m_about.Visibility = Visibility.Hidden;
            }
        }

        private void ToggleCharacter(object sender, RoutedEventArgs e) {

        }

        private void OpenConsole(object sender, RoutedEventArgs e) {
            UiConsole.Show();
        }

        private static readonly Regex regex = new("[^0-9.-]+");

        private void f_Pasting(object sender, DataObjectPastingEventArgs e) {
            if (e.DataObject.GetDataPresent(typeof(string))) {
                string txt = (string)e.DataObject.GetData(typeof(string));

                if (!regex.IsMatch(txt)) {
                    e.CancelCommand();
                }
            } else {
                e.CancelCommand();
            }
        }

        private void f_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BtnExit(object sender, RoutedEventArgs e) {
            GameProc?.Kill();
            Environment.Exit(0);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            Log.Write("Forcing close detected.. Closing all windows.");
            GameProc?.Kill();
            Environment.Exit(0);
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern bool SetWindowText(IntPtr hWnd, string text);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    }

    public class MultiTextWriter : TextWriter {
        private IEnumerable<TextWriter> writers;
        public MultiTextWriter(IEnumerable<TextWriter> writers) {
            this.writers = writers.ToList();
        }
        public MultiTextWriter(params TextWriter[] writers) {
            this.writers = writers;
        }

        public override void Write(char value) {
            foreach (var writer in writers)
                writer.Write(value);
        }

        public override void Write(string value) {
            foreach (var writer in writers)
                writer.Write(value);
        }

        public override void Flush() {
            foreach (var writer in writers)
                writer.Flush();
        }

        public override void Close() {
            foreach (var writer in writers)
                writer.Close();
        }

        public override Encoding Encoding {
            get { return Encoding.ASCII; }
        }
    }

    public class ControlWriter : TextWriter {
        private ConsoleWindow textbox;
        public ControlWriter(ConsoleWindow textbox) {
            this.textbox = textbox;
        }

        public override void Write(char value) {
            textbox.Write(value);
        }

        public override void Write(string value) {
            textbox.Write(value);
        }

        public override Encoding Encoding {
            get { return Encoding.ASCII; }
        }
    }
}
