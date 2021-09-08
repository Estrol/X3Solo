using System;
using System.Globalization;
using System.IO;
using Estrol.X3Solo.Library;

namespace Estrol.X3Solo.Server {
    public class Configuration {
        private readonly INI ini;

        public Configuration() {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\config")) {
                _ = Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\config");
            }

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\config\solo.ini")) {
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\config\solo.ini", Properties.Resources.solo);
            }

            ini = new INI(AppDomain.CurrentDomain.BaseDirectory + @"\config\solo.ini");
        }

        public int Version {
            get {
                string val = ini.IniReadValue("EMUCONFIG", "Version");
                return int.Parse(val, CultureInfo.CurrentCulture);
            }
        }

        public int ServerPort {
            set {
                string val = value.ToString(CultureInfo.CurrentCulture);
                ini.IniWriteValue("EMUCONFIG", "ServerPort", val);
            }
            get {
                string val = ini.IniReadValue("EMUCONFIG", "ServerPort");
                return int.Parse(val, CultureInfo.CurrentCulture);
            }
        }

        public string Name {
            set => ini.IniWriteValue("CHARACTER", "Name", value);
            get => ini.IniReadValue("CHARACTER", "Name");
        }

        public int Level {
            set {
                string val = value.ToString(CultureInfo.CurrentCulture);
                ini.IniWriteValue("CHARACTER", "Level", val);
            }
            get => int.Parse(ini.IniReadValue("CHARACTER", "Level"), CultureInfo.CurrentCulture);
        }

        public int Rank {
            set {
                string val = value.ToString(CultureInfo.CurrentCulture);
                ini.IniWriteValue("CHARACTER", "Rank", val);
            }
            get => int.Parse(ini.IniReadValue("CHARACTER", "Rank"), CultureInfo.CurrentCulture);
        }

        public int Value(string key) {
            return int.Parse(ini.IniReadValue("CHARACTER", key), CultureInfo.CurrentCulture);
        }

        public void Set(string key, int value) {
            ini.IniWriteValue("CHARACTER", key, value.ToString(CultureInfo.CurrentCulture));
        }
    }
}
