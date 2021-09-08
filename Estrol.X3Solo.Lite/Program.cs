using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Estrol.X3Solo.Server;

namespace Estrol.X3Jam.ConsoleWindow {
    public class Program {
        public static void Main() {
            Console.Title = "X3Solo - O2-JAM 1.8 Server Emulation";

            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OTwo.exe"))) {
                _ = MessageBox(0, "Cannot find OTwo.exe in current directory", "Error", (uint)(0x00000000L | 0x00000010L));
                return;
            }

            using Mutex mutex = new(true, "X3JAMSERVER", out bool createNew);
            if (createNew) {
                ClientCheck config = new();
                config.GetVersion("OTwo.exe");

                Main server = new();
                server.Intialize(config);

                Task NeverReturn = new(() => { while (true) { _ = Console.Read(); } });
                NeverReturn.Start();
                NeverReturn.GetAwaiter().GetResult();
            } else {
                Console.WriteLine("Error, The server program already open!");
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBox(int hWnd, string Text, string Caption, uint Type);
    }
}
