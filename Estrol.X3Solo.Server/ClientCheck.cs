using Estrol.X3Solo.Library.Server;
using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Estrol.X3Solo.Server {
    public class ClientCheck {
        private MemoryStream ms;
        private BinaryReader br;

        /// <summary>
        /// This client check client, get OJNList and music path
        /// </summary>
        public ClientCheck() { }

        /// <summary>
        /// Manually set client check thing
        /// </summary>
        /// <param name="ver"></param>
        /// <param name="OJNList"></param>
        /// <param name="Music"></param>
        /// <param name="Interface"></param>
        /// <param name="Avatar"></param>
        /// <param name="Playing"></param>
        public ClientCheck(string ver, string OJNList, string Music, string Interface, string Avatar, string Playing) {
            Version = ver;

            this.OJNList = OJNList;
            this.Music = Music;
            this.Interface = Interface;
            this.Avatar = Avatar;
            this.Playing = Playing;
        }

        public void GetVersion(string fileName) {
            byte[] file = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + $"\\{fileName}");
            ms = new(file);
            br = new(ms);

            _ = br.BaseStream.Seek(0x138, SeekOrigin.Begin);
            byte[] IsMPRESS = br.ReadBytes(8);

            if (StructuralComparisons.StructuralEqualityComparer.Equals(IsMPRESS, Client15MPRESS)) {
                throw new InvalidDataException("Not support MPRESS!");
            }

            _ = br.BaseStream.Seek(0xE2AA8, SeekOrigin.Begin);
            byte[] signature = br.ReadBytes(8);
            string Client18String = ByteA2String(Client18Hash);
            string Client15String = ByteA2String(Client15Hash);

            // Client version check!
            Version = ByteA2String(signature) == Client18String
                ? "1.8"
                : ByteA2String(signature) == Client15String ? "1.5" : throw new InvalidDataException("Invalid version");

            _ = br.BaseStream.Seek(0xDFDCD, SeekOrigin.Begin);
            byte[] bOJNList = br.ReadBytes(15);
            OJNList = Encoding.UTF8.GetString(bOJNList).Split(new[] { '\0' }, 2)[0];

            _ = br.BaseStream.Seek(0xDFDF9, SeekOrigin.Begin);
            byte[] bMusic = br.ReadBytes(5);
            Music = Encoding.UTF8.GetString(bMusic).Split(new[] { '\0' }, 2)[0];

            _ = br.BaseStream.Seek(0xDFD3A, SeekOrigin.Begin);
            byte[] bPlayingHeader = br.ReadBytes(14);
            Playing = Encoding.UTF8.GetString(bPlayingHeader).Split(new[] { '\0' }, 2)[0];

            _ = br.BaseStream.Seek(0xDFD66, SeekOrigin.Begin);
            byte[] bAvatarHeader = br.ReadBytes(13);
            Avatar = Encoding.UTF8.GetString(bAvatarHeader).Split(new[] { '\0' }, 2)[0];

            _ = br.BaseStream.Seek(0xDFD4E, SeekOrigin.Begin);
            byte[] bInterfaceHeader = br.ReadBytes(14);
            Interface = Encoding.UTF8.GetString(bInterfaceHeader).Split(new[] { '\0' }, 2)[0];
        }

        private static string ByteA2String(byte[] bData) {
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

        /// <summary>
        /// Get client's Playing file name
        /// </summary>
        public string Playing { get; private set; }

        /// <summary>
        /// Get client's Avatar file name
        /// </summary>
        public string Avatar { get; private set; }

        /// <summary>
        /// Get client's Interface file name
        /// </summary>
        public string Interface { get; private set; }

        private static readonly byte[] Client18Hash = {
            0x78, 0x2e, 0x2f, 0x8f, 0x65, 0x8c, 0xd9, 0xd3
        };

        // Empty for some reason
        private static readonly byte[] Client15Hash = {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        private static readonly byte[] Client15MPRESS = {
            0x2E, 0x4D, 0x50, 0x52, 0x45, 0x53, 0x53, 0x31
        };
    }
}
