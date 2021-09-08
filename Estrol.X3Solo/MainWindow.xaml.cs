#pragma warning disable IDE0058

using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Estrol.X3Solo.Server;
using Estrol.X3Solo.Library;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Estrol.X3Solo.Parser;

namespace Estrol.X3Solo {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly Configuration config;
        private readonly Main server;
        private Process GameProc;

        // UI
        private readonly ConsoleWindow UiConsole;
        private MoreSettingWindow UiMoreSetting;

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
                "s\r\n- 9you O2-JAM 1.8 and 1.5";

            server = new Main();
            f_port.Text = string.Format(CultureInfo.CurrentCulture, "{0}", server.m_config.ServerPort);
            f_name.Text = server.m_config.Name;

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

            server.OnCError += (object o) => {
                MessageBoxResult res = MessageBox.Show("Seems the game crashing when you're enjoying the game -_-\nOpen help page?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.No, MessageBoxOptions.DefaultDesktopOnly);
                if (res == MessageBoxResult.Yes) {
                    ProcessStartInfo ps = new("https://estrol.space/game/x3solo/troubleshot") {
                        UseShellExecute = true,
                        Verb = "open"
                    };

                    Process.Start(ps);
                }
            };
        }

        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private bool IsClicked = false;
        private void Run(object sender, RoutedEventArgs e) {
            if (IsClicked) return;

            IsClicked = true;
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
                conf.GetVersion("OTwo.exe");

                if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", conf.OJNList))) {
                    bool result = OJNListGenerator.GenerateOJNList(conf);
                    if (!result) {
                        return;
                    }
                }

                ProcessStartInfo ps = new() {
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Normal,
                    FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OTwo.exe"),
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
                        bool result = GameProc.WaitForInputIdle();
                        if (!result) {
                            MessageBox.Show("Failed to start the game!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        GameProc.WaitForExit();
                        server.Stop();
                        GameProc = null;
                    } catch (Exception e) {
                        ProcessUtil.SuspendProcess(GameProc.Id);

                        MessageBox.Show(e.Message + "\n\nProgram will now exit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        GameProc?.Kill();
                        Environment.Exit(0);
                    }
                }
            });

            ts.Start();
            IsClicked = false;
        }

        private void ToggleSetting(object sender, RoutedEventArgs e) {
            if (m_setting.Visibility == Visibility.Hidden) {
                m_setting.Visibility = Visibility.Visible;
            } else {
                config.Name = f_name.Text;
                config.ServerPort = int.Parse(f_port.Text, CultureInfo.CurrentCulture);
                m_setting.Visibility = Visibility.Hidden;
            }
        }

        private void ToggleAbout(object sender, RoutedEventArgs e) {
            m_about.Visibility = m_about.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
        }

        private void ToggleCharacter(object sender, RoutedEventArgs e) {
            if (UiMoreSetting == null) {
                UiMoreSetting = new(config);
            }

            UiMoreSetting.Show();
        }

        private void OpenConsole(object sender, RoutedEventArgs e) {
            UiConsole.Show();
        }

        private static readonly Regex regex = new("[^0-9.-]+");
        private void FormPasting(object sender, DataObjectPastingEventArgs e) {
            if (e.DataObject.GetDataPresent(typeof(string))) {
                string txt = (string)e.DataObject.GetData(typeof(string));

                if (!regex.IsMatch(txt)) {
                    e.CancelCommand();
                }
            } else {
                e.CancelCommand();
            }
        }

        private void FormPreviewTextInput(object sender, TextCompositionEventArgs e) {
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
    }

    public class MultiTextWriter : TextWriter {
        private readonly IEnumerable<TextWriter> writers;
        public MultiTextWriter(IEnumerable<TextWriter> writers) {
            this.writers = writers.ToList();
        }
        public MultiTextWriter(params TextWriter[] writers) {
            this.writers = writers;
        }

        public override void Write(char value) {
            foreach (TextWriter writer in writers) {
                writer.Write(value);
            }
        }

        public override void Write(string value) {
            foreach (TextWriter writer in writers) {
                writer.Write(value);
            }
        }

        public override void Flush() {
            foreach (TextWriter writer in writers) {
                writer.Flush();
            }
        }

        public override void Close() {
            foreach (TextWriter writer in writers) {
                writer.Close();
            }
        }

        public override Encoding Encoding => Encoding.ASCII;
    }

    public class ControlWriter : TextWriter {
        private readonly ConsoleWindow textbox;
        public ControlWriter(ConsoleWindow textbox) {
            this.textbox = textbox;
        }

        public override void Write(char value) {
            textbox.Write(value);
        }

        public override void Write(string value) {
            textbox.Write(value);
        }

        public override Encoding Encoding => Encoding.ASCII;
    }

    public class OJNInfo {
        public string Name { set; get; }
        public int OJN { set; get; }
    }
}
