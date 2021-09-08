using System.IO;
using System.Text;

namespace Estrol.X3Solo.Server.Data {
    public static class OJNListDecoder {
        private const string ChineseEncoding = "big5";

        public static OJNList Decode(string FileName) {
            return Decode(File.Open(FileName, FileMode.Open), false);
        }

        public static OJNList Decode(Stream stream, bool keepOpen = false) {
            OJNList headers = new();

            using BinaryReader reader = new(stream, Encoding.Unicode, keepOpen);
            _ = stream.Seek(0, SeekOrigin.Begin);
            int songCount = reader.ReadInt32();

            headers.Version = stream.Length > 4 + (songCount * 300) ? OJNFileFormat.New : OJNFileFormat.Old;
            string charset = ChineseEncoding;

            for (int i = 0; i < songCount; i++) {
                _ = headers.Add(OJNDecoder.Decode(reader.ReadBytes(300), Encoding.GetEncoding(charset)));
            }

            return headers;
        }
    }
}
