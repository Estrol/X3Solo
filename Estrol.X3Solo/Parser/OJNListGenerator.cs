using Estrol.X3Solo.Library;
using Estrol.X3Solo.Server;
using Estrol.X3Solo.Server.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Estrol.X3Solo.Parser {
    public static class OJNListGenerator {
        public static bool GenerateOJNList(ClientCheck Client) {
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Client.Music))) {
                _ = Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Client.Music));
            }

            DirectoryInfo Folder = new(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Client.Music)
            );

            FileInfo[] Files = Folder.GetFiles("o2ma*.ojn");
            if (Files.Length < 1) {
                _ = MessageBox.Show($"There no known files \"o2ma*.ojn\" in folder \"{Client.Music}\", check your folder if there any songs!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            int MAX_SONG_FILES_COUNT = 620;
            if (Files.Length > MAX_SONG_FILES_COUNT) {
                _ = MessageBox.Show($"Maximum client song count reached! ({Files.Length}/{MAX_SONG_FILES_COUNT})", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }

            Dictionary<string, OJN> ojnList = new();
            foreach (FileInfo file in Files) {
                Log.Write($"OJNRead -> {file.Name}");
                byte[] rawData = File.ReadAllBytes(file.FullName);
                OJN ojn = OJNDecoder.Decode(rawData);

                ojnList.Add(file.Name, ojn);
            }

            List<KeyValuePair<string, OJN>> list = ojnList.ToList();
            list.Sort((KeyValuePair<string, OJN> pair1, KeyValuePair<string, OJN> pair2) => {
                return pair1.Value.Id.CompareTo(pair2.Value.Id);
            });

            using MemoryStream ms = new();
            using BinaryWriter br = new(ms);

            br.Write(ojnList.Count);
            foreach (KeyValuePair<string, OJN> itr in ojnList) {
                Log.Write($"OJNParse -> {itr.Key}");

                br.Write(OJNEncoder.Encode(itr.Value, true));
            }

            File.WriteAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", Client.OJNList), ms.ToArray());
            return true;
        }
    }
}
