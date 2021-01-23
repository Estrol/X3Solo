using System;
using System.IO;
using Estrol.X3Solo.Library;

namespace Estrol.X3Solo.Server {
    public class Configuration {
        private readonly INI ini;

        public Configuration() {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\config")) {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\config");
            }

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\config\solo.ini")) {
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\config\solo.ini", Properties.Resources.solo);
            }

            ini = new INI(AppDomain.CurrentDomain.BaseDirectory + @"\config\solo.ini");
        }

        public int Version {
            get {
                string val = ini.IniReadValue("EMUCONFIG", "Version");
                return int.Parse(val);
            }
        }

        public int ServerPort {
            set {
                string val = value.ToString();
                ini.IniWriteValue("EMUCONFIG", "ServerPort", val);
            }
            get {
                string val = ini.IniReadValue("EMUCONFIG", "ServerPort");
                return int.Parse(val);
            }
        }

        public string Name {
            set {
                ini.IniWriteValue("CHARACTER", "Name", value);
            }
            get {
                return ini.IniReadValue("CHARACTER", "Name");
            }
        }

        public int Level {
            set {
                string val = value.ToString();
                ini.IniWriteValue("CHARACTER", "Level", val);
            }
            get {
                return int.Parse(ini.IniReadValue("CHARACTER", "Level"));
            }
        }

        public int Rank {
            set {
                string val = value.ToString();
                ini.IniWriteValue("CHARACTER", "Rank", val);
            }
            get {
                return int.Parse(ini.IniReadValue("CHARACTER", "Rank"));
            }
        }

        public int[] Character {
            get {
                return new[] {
                    int.Parse(ini.IniReadValue("CHARACTER", "Instrument")),
                    int.Parse(ini.IniReadValue("CHARACTER", "Hair")),
                    int.Parse(ini.IniReadValue("CHARACTER", "Accessory")),
                    int.Parse(ini.IniReadValue("CHARACTER", "Glove")),
                    int.Parse(ini.IniReadValue("CHARACTER", "Necklace")),
                    int.Parse(ini.IniReadValue("CHARACTER", "Top")),
                    int.Parse(ini.IniReadValue("CHARACTER", "Pant")),
                    int.Parse(ini.IniReadValue("CHARACTER", "Glass")),
                    int.Parse(ini.IniReadValue("CHARACTER", "Earring")),
                    int.Parse(ini.IniReadValue("CHARACTER", "ClothAccessory")),
                    int.Parse(ini.IniReadValue("CHARACTER", "Shoe")),
                    int.Parse(ini.IniReadValue("CHARACTER", "Face")),
                    int.Parse(ini.IniReadValue("CHARACTER", "Wing")),
                    int.Parse(ini.IniReadValue("CHARACTER", "HairAccessory")),
                    int.Parse(ini.IniReadValue("CHARACTER", "InstrumentAccessory")),
                    int.Parse(ini.IniReadValue("CHARACTER", "Pet")),
                };
            }
        }

        public int Value(string key) {
            return int.Parse(ini.IniReadValue("CHARACTER", key));
        }

        public void Set(string key, int value) {
            ini.IniWriteValue("CHARACTER", key, value.ToString());
        }
    }
}
