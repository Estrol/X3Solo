using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Solo.Server.Data {
    public static class OJNDecoder {
        public static bool Validate(string FileName) {
            return Validate(File.Open(FileName, FileMode.Open));
        }

        public static bool Validate(Stream stream, bool keepOpen = false) {
            var encoding = Encoding.UTF8;
            using (var reader = new BinaryReader(stream, encoding, keepOpen)) {
                stream.Seek(0, SeekOrigin.Begin);
                string encryptSign = encoding.GetString(reader.ReadBytes(3));

                stream.Seek(4, SeekOrigin.Begin);
                string fileSign = encoding.GetString(reader.ReadBytes(3));

                return encryptSign == "new" || fileSign == "ojn";
            }
        }

        public static bool IsEncrypted(string FileName) {
            return IsEncrypted(File.Open(FileName, FileMode.Open));
        }

        public static bool IsEncrypted(Stream stream, bool keepOpen = false) {
            var encoding = Encoding.UTF8;
            using (var reader = new BinaryReader(stream, encoding, keepOpen)) {
                stream.Seek(0, SeekOrigin.Begin);
                string encryptSign = encoding.GetString(reader.ReadBytes(3));

                return encryptSign == "new";
            }
        }

        public static OJN Decode(string filename, Encoding encoding = default(Encoding)) {
            var header = Decode(File.Open(filename, FileMode.Open), encoding, false);
            header.Source = filename;

            return header;
        }

        public static OJN Decode(Stream stream, Encoding encoding = default(Encoding), bool keepOpen = false) {
            byte[] inputData = new byte[0];
            bool encrypted = false;
            using (var reader = new BinaryReader(stream, Encoding.Unicode, keepOpen)) {
                stream.Seek(0, SeekOrigin.Begin);
                string encryptSign = Encoding.UTF8.GetString(reader.ReadBytes(3));
                if (encryptSign == "new") {
                    inputData = Decrypt(stream);
                    encrypted = true;
                } else {
                    stream.Seek(0, SeekOrigin.Begin);
                    inputData = reader.ReadBytes((int)stream.Length);
                }
            }

            var header = Decode(inputData, encoding);
            header.Encrypted = encrypted;

            return header;
        }

        public static OJN Decode(byte[] inputData, Encoding encoding = default(Encoding)) {
            if (encoding == default(Encoding)) {
                encoding = Encoding.UTF8;
            }

            using (var mstream = new MemoryStream(inputData))
            using (var reader = new BinaryReader(mstream)) {
                var header = new OJN {
                    Id = reader.ReadInt32(),
                    Signature = reader.ReadBytes(4),
                    EncodingVersion = reader.ReadSingle(),
                    Genre = (OJNGenre)reader.ReadInt32(),
                    BPM = reader.ReadSingle(),
                    LevelEx = reader.ReadInt16(),
                    LevelNx = reader.ReadInt16(),
                    LevelHx = reader.ReadInt16(),
                    Padding = reader.ReadInt16(),
                    EventCountEx = reader.ReadInt32(),
                    EventCountNx = reader.ReadInt32(),
                    EventCountHx = reader.ReadInt32(),
                    NoteCountEx = reader.ReadInt32(),
                    NoteCountNx = reader.ReadInt32(),
                    NoteCountHx = reader.ReadInt32(),
                    MeasureCountEx = reader.ReadInt32(),
                    MeasureCountNx = reader.ReadInt32(),
                    MeasureCountHx = reader.ReadInt32(),
                    BlockCountEx = reader.ReadInt32(),
                    BlockCountNx = reader.ReadInt32(),
                    BlockCountHx = reader.ReadInt32(),
                    OldEncodingVersion = reader.ReadInt16(),
                    OldSongId = reader.ReadInt16(),
                    OldGenre = reader.ReadBytes(20),
                    ThumbnailSize = reader.ReadInt32(),
                    FileVersion = reader.ReadInt32(),
                    Title = reader.ReadBytes(64),
                    Artist = reader.ReadBytes(32),
                    Pattern = reader.ReadBytes(32),
                    OJM = reader.ReadBytes(32),
                    CoverSize = reader.ReadInt32(),
                    DurationEx = reader.ReadInt32(),
                    DurationNx = reader.ReadInt32(),
                    DurationHx = reader.ReadInt32(),
                    BlockOffsetEx = reader.ReadInt32(),
                    BlockOffsetNx = reader.ReadInt32(),
                    BlockOffsetHx = reader.ReadInt32(),
                    CoverOffset = reader.ReadInt32(),
                    CharacterEncoding = encoding
                };

                return header;
            }
        }

        public static byte[] Decrypt(Stream stream) {
            using (var reader = new BinaryReader(stream, Encoding.Unicode, true)) {
                stream.Seek(0, SeekOrigin.Begin);
                byte[] input = reader.ReadBytes((int)stream.Length);

                stream.Seek(3, SeekOrigin.Begin);
                byte blockSize = reader.ReadByte();
                byte mainKey = reader.ReadByte();
                byte midKey = reader.ReadByte();
                byte initialKey = reader.ReadByte();

                var encryptKeys = Enumerable.Repeat(mainKey, blockSize).ToArray();
                encryptKeys[0] = initialKey;
                encryptKeys[(int)Math.Floor(blockSize / 2f)] = midKey;

                byte[] output = new byte[stream.Length - stream.Position];
                for (int i = 0; i < output.Length; i += blockSize) {
                    for (int j = 0; j < blockSize; j++) {
                        int offset = i + j;
                        if (offset >= output.Length) {
                            return output;
                        }

                        output[offset] = (byte)(input[input.Length - (offset + 1)] ^ encryptKeys[j]);
                    }
                }

                return output;
            }
        }
    }
}
