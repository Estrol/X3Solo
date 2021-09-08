using Estrol.X3Solo.Data;
using Estrol.X3Solo.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace Estrol.X3Solo {
    public class OPIParser {
        public List<ItemData> AvatarOPI { set; get; }

        public OPIParser(MoreSettingWindow wnd) {
            string BaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image");

            byte[] bAvatar = File.ReadAllBytes(Path.Combine(BaseDirectory, wnd.client.Avatar));

            using MemoryStream ms = new(bAvatar);
            using BinaryReader br = new(ms);

            int FileType = br.ReadInt32();
            int FileAmount = br.ReadInt32();

            wnd.Dispatcher.Invoke(() => {
                wnd.ProgressBarLabel.Maximum = FileAmount;
            });

            AvatarOPI = new List<ItemData>(FileAmount);
            if ((FileAmount * 152) + 8 > ms.Length) {
                _ = MessageBox.Show("There more data than expected data", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(2);
            } else {
                ms.Position = ms.Length - (FileAmount * 152);

                // Get The header first
                for (int i = 0; i < FileAmount; i++) {
                    ItemData item = new();
                    item.UNK1 = br.ReadInt32();
                    string fileName = Encoding.UTF8.GetString(br.ReadBytes(128));

                    item.ItemName = fileName.Substring(0, fileName.IndexOf('\0'));
                    item.Offset = br.ReadInt32();
                    item.Size1 = br.ReadInt32();
                    item.Size2 = br.ReadInt32();
                    item.UNK2 = br.ReadBytes(8);

                    if (item.Size1 != item.Size2) {
                        Log.Write($"Size1: {item.Size1} and Size2: {item.Size2} are different, using the largest value");
                    }

                    AvatarOPI.Add(item);
                }

                // Read the data
                for (int i = 0; i < FileAmount; i++) {
                    ItemData item = AvatarOPI[i];

                    wnd.Dispatcher.Invoke(() => {
                        wnd.ProgressBarLabel.Value = i;
                    });

                    ms.Position = item.Offset;
                    item.Data = br.ReadBytes(Math.Max(item.Size1, item.Size2));
                }
            }
        }

        public void Destroy() {
            AvatarOPI = null;
            GC.Collect();
        }

        public byte[] GetRawOJS(string KeyName, string DataFileName) {
            switch (KeyName) {
                case "Avatar": {
                    ItemData result = AvatarOPI.Find(x => x.ItemName == DataFileName);
                    return result is not null ? result.Data : null;
                }

                default: {
                    return null;
                }
            }
        }
    }
}
