#pragma warning disable CA5351

using Estrol.X3Solo.Library;
using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace Estrol.X3Solo.Server.Data {
    public class MusicListCaching {
        private readonly MD5CryptoServiceProvider MD5;

        private readonly string SourceAddr;
        private readonly string DestinationAddr;

        private bool IsAvailable; // True = available to read, False = not available to read (or need write it first)
        private string ErrorMessage; // using GetCacheMessage() if one of these function return false
        private string CachedCached; // Idk (why I am wrote this?)

        public MusicListCaching(string srcAddr) {
            MD5 = new();

            SourceAddr = srcAddr;
            DestinationAddr = srcAddr.TrimEnd() + ".x3a";

            IsAvailable = File.Exists(DestinationAddr);
        }

        public byte[] LoadCache() {
            if (!VerifyCache()) {
                throw new InvalidDataException("Cache file is not X3Archive or empty");
            }

            using Stream fs = File.OpenRead(DestinationAddr);
            using BinaryReader br = new(fs);

            // Skip Header since it already checked
            int BytesToSkip = 9 + 1 + CachedCached.Length + 3;
            _ = br.BaseStream.Seek(BytesToSkip, SeekOrigin.Begin);

            int length = br.ReadInt32();
            byte[] ZipData = br.ReadBytes(length);
            byte[] data = UnZip(ZipData);

            return data;
        }

        public bool CreateCache() {
            if (VerifyCache()) {
                ErrorMessage = "Cache already made for that hash";
                return false;
            }

            using MemoryStream ms = new();
            using BinaryWriter bw = new(ms);
            bw.Write(new byte[] { // "x3archive" File Header
                0x78, 0x33, 0x61, 0x72,
                0x63, 0x68, 0x69, 0x76,
                0x65
            });

            bw.Write((byte)0xaa); // 0xaa = cache, 0xbb = archive
            bw.Write(Hash()); // Hash
            bw.Write((short)0); // Padding

            // SECTION MusicList data
            OJNList list = OJNListDecoder.Decode(SourceAddr);
            OJN[] OJNList = list.GetHeaders();

            using EBuffer buf = new();
            short length = (short)(6 + (OJNList.Length * 12) + 12);
            buf.Write(length);
            buf.Write((short)0x0fbf);
            buf.Write((short)OJNList.Length);

            foreach (OJN ojn in OJNList) {
                buf.Write((short)ojn.Id);
                buf.Write((short)ojn.NoteCountEx);
                buf.Write((short)ojn.NoteCountNx);
                buf.Write((short)ojn.NoteCountHx);
                buf.Write(0);
            }

            buf.Write((long)0);
            buf.Write(0);
            // END SECTION

            byte[] CompData = Zip(buf.ToArray());
            bw.Write(CompData.Length);
            bw.Write(CompData);

            IsAvailable = true;
            File.WriteAllBytes(DestinationAddr, ms.ToArray()); // Save cache file for future use
            return true;
        }

        private string Hash() {
            using Stream stream = File.OpenRead(SourceAddr);
            byte[] tmpHash = MD5.ComputeHash(stream);

            return Convert.ToBase64String(tmpHash);
        }

        public bool VerifyCache() {
            if (!IsAvailable) {
                return false;
            }

            using Stream fs = File.OpenRead(DestinationAddr);
            using BinaryReader br = new(fs);
            byte[] header = br.ReadBytes(9);

            if (ByteArrayToString(header) != "78 33 61 72 63 68 69 76 65") {
                return false;
            }

            if (br.ReadByte() != 0xaa) {
                return false;
            }

            string cacHash = br.ReadString();
            string srcHash = Hash();

            CachedCached = srcHash;
            return srcHash == cacHash;
        }

        public string GetLastMessage() {
            return ErrorMessage;
        }

        private static void CopyTo(Stream src, Stream dest) {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0) {
                dest.Write(bytes, 0, cnt);
            }
        }

        private static byte[] Zip(byte[] bytes) {
            using MemoryStream msi = new(bytes);
            using MemoryStream mso = new();
            using (GZipStream gs = new(mso, CompressionMode.Compress)) {
                CopyTo(msi, gs);
            }

            return mso.ToArray();
        }

        private static byte[] UnZip(byte[] bytes) {
            using MemoryStream msi = new(bytes);
            using MemoryStream mso = new();
            using (GZipStream gs = new(msi, CompressionMode.Decompress)) {
                CopyTo(gs, mso);
            }

            return mso.ToArray();
        }

        private static string ByteArrayToString(byte[] ba) {
            return BitConverter.ToString(ba).Replace("-", " ");
        }
    }
}
