using System;
using System.Globalization;
using System.Diagnostics;

namespace Estrol.X3Solo.Library {
    public static class Log {
        public static void Write(string content) {
            OutputToConsole(content);
        }

        public static void Write(string content, params object[] objects) {
            content = string.Format(CultureInfo.CurrentCulture, content, objects);

            OutputToConsole(content);
        }

        private static void OutputToConsole(string content) {
            string time = DateTime.Now.ToString("HH:mm:ss:ff", CultureInfo.CurrentCulture);

            Console.WriteLine("[{0}] {1}", time, content);
            Debug.WriteLine("[{0}] {1}", time, content);
        }
    }
}
