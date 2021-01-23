using System;
using System.Threading;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;

namespace Estrol.X3Solo {
    public class Program : Application {
        [STAThread]
        public static void Main() {
            using Mutex mutex = new Mutex(true, "X3SOLO-APP-RELEASE-BUILD-V1", out var created);
            if (created) {
                Program app = new Program();
                app.InitializeComponent();
                app.Run();
            } else {
                Process current = Process.GetCurrentProcess();
                foreach (Process process in Process.GetProcessesByName(current.ProcessName)) {
                    if (process.Id != current.Id) {
                        SetForegroundWindow(process.MainWindowHandle);
                        break;
                    }
                }
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.0.0")]
        public void InitializeComponent() {

            #line 5 "..\..\..\App.xaml"
            this.StartupUri = new System.Uri("MainWindow.xaml", System.UriKind.Relative);

            #line default
            #line hidden
        }
    }
}
