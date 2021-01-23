using System;
using System.IO;
using System.Text;

namespace Estrol.X3Solo.Server {
    public class ClientCheck {
        public MemoryStream ms;
        public BinaryReader br;

        /// <summary>
        /// This client check client, get OJNList and music path
        /// </summary>
        public ClientCheck() {}

        public void GetVersion() {
            byte[] file = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + @"\OTwo.exe");
            ms = new(file);
            br = new(ms);

            br.BaseStream.Seek(0xE63E4, SeekOrigin.Begin);
            byte[] signature = br.ReadBytes(8);

            // Client version check!
            switch (ByteA2String(signature)) {
                case "C1 AC BD D3 B7 FE CE F1": {
                    Version = "1.8";
                    break;
                }

                case "5D 0B 6C A0 90 6D 32 D4": {
                    Version = "1.5";
                    break;
                }

                case "65 63 74 69 6F 6E 20 6C": { // DPJAM client
                    Version = "1.8";
                    break;
                }

                default: {
                    throw new InvalidDataException(
                        "Invalid OTwo.exe data, could be it's packed or different version?"
                    );
                }
            }

            br.BaseStream.Seek(0xDFDCD, SeekOrigin.Begin);
            byte[] bOJNList = br.ReadBytes(15);
            OJNList = Encoding.UTF8.GetString(bOJNList).Trim('\0');

            br.BaseStream.Seek(0xDFDF9, SeekOrigin.Begin);
            byte[] bMusic = br.ReadBytes(5);
            Music = Encoding.UTF8.GetString(bMusic).Trim('\0');
        }

        private string ByteA2String(byte[] bData) {
            string hex = BitConverter.ToString(bData).Replace("-", " ");
            return hex;
        }

        /// <summary>
        /// Get client's OJNList name
        /// </summary>
        public string OJNList { get; private set; }

        /// <summary>
        /// Get client's Music path
        /// </summary>
        public string Music { get; private set; }

        /// <summary>
        /// Get client version 1.8 or 1.5
        /// </summary>
        public string Version { get; private set; }
    }
}
