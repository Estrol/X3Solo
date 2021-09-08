using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;

namespace Estrol.X3Solo {
    public class Program : Application {
        [STAThread]
        public static void Main() => AppRun();

        /// <summary>
        /// For library loader
        /// </summary>
        public static void AppRun() {
            try {
                PreLoadLibrary();

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
            } catch (Exception e) {
                MessageBox.Show(e.Message);
            }
        }

        private static void PreLoadLibrary() {
            System.Text.EncodingProvider provider = System.Text.CodePagesEncodingProvider.Instance;
            System.Text.Encoding.RegisterProvider(provider);

            LoadLibraryEx("D3D9.dll", IntPtr.Zero, LoadLibraryFlags.LOAD_LIBRARY_SEARCH_SYSTEM32);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, LoadLibraryFlags dwFlags);
        [Flags]
        private enum LoadLibraryFlags : uint {
            None = 0,
            DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
            LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
            LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
            LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,
            LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
            LOAD_LIBRARY_SEARCH_APPLICATION_DIR = 0x00000200,
            LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000,
            LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR = 0x00000100,
            LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800,
            LOAD_LIBRARY_SEARCH_USER_DIRS = 0x00000400,
            LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008
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
