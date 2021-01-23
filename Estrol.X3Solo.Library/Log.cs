using System;

namespace Estrol.X3Solo.Library {
    public static class Log {
        public static void Write(string content) {
            OutputToConsole(content);
        }

        public static void Write(string content, params object[] objects) {
            content = string.Format(content, objects);

            OutputToConsole(content);
        }

        private static void OutputToConsole(string content) {
            string time = DateTime.Now.ToString("HH:mm:ss:ff");

            Console.WriteLine("[{0}] {1}", time, content);
            System.Diagnostics.Debug.WriteLine("[{0}] {1}", time, content);
        }
    }
}
