#pragma warning disable CA5351

using System;
using System.IO;
using System.Security.Cryptography;

namespace Estrol.X3Solo.Library.Server {
    public static class Md5AndDes {
        public class DesKey {
            public DesKey(byte[] key, byte[] iv) {
                Key = key;
                Iv = iv;
            }

            public byte[] Key { get; }
            public byte[] Iv { get; }
        }

        public static DesKey DeriveKey(byte[] password, uint iterationCount = 16) {
            if (password.Length != 16) {
                return null;
            }

            MD5 md5 = MD5.Create();
            for (int i = 0; i < iterationCount; i++) {
                password = md5.ComputeHash(password);
            }

            byte[] key = new byte[8];
            byte[] iv = new byte[8];
            Buffer.BlockCopy(password, 0, iv, 0, 8);
            Buffer.BlockCopy(password, 8, key, 0, 8);
            return new DesKey(key, iv);
        }

        public static byte[] Decrypt(byte[] input, DesKey key) {
            return Decrypt(input, key.Key, key.Iv);
        }

        public static byte[] Decrypt(byte[] input, byte[] key, byte[] iv) {
            DESCryptoServiceProvider cProv = new();
            cProv.Padding = PaddingMode.None;
            cProv.Mode = CipherMode.CBC;
            ICryptoTransform transform = cProv.CreateDecryptor(key, iv);
            MemoryStream inStream = new();
            CryptoStream cStream = new(inStream, transform, CryptoStreamMode.Write);
            cStream.Write(input, 0, input.Length);
            cStream.FlushFinalBlock();
            return inStream.ToArray();
        }
    }
}
