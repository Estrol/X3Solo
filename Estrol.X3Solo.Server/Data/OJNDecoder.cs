using System.IO;
using System.Text;

namespace Estrol.X3Solo.Server.Data {

    // https://github.com/SirusDoma/O2MusicList/blob/master/Source/Encoders/OJNDecoder.cs
    public static class OJNDecoder {
        public static OJN Decode(byte[] inputData, Encoding encoding = default) {
            if (encoding == default) {
                encoding = Encoding.UTF8;
            }

            using var mstream = new MemoryStream(inputData);
            using var reader = new BinaryReader(mstream);
            OJN header = new() {
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
}