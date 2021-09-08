using System.IO;

namespace Estrol.X3Solo.Server.Data {

    // https://github.com/SirusDoma/O2MusicList/blob/master/Source/Encoders/OJNEncoder.cs
    public static class OJNEncoder {
        public static byte[] Encode(OJN header, bool headerOnly = false) {
            byte[] data = new byte[300];
            if (File.Exists(header.Source) && !headerOnly) {
                data = File.ReadAllBytes(header.Source);
            }

            using MemoryStream mstream = new(data);
            using BinaryWriter writer = new(mstream);

            writer.Write(header.Id);
            writer.Write(header.Signature);
            writer.Write(header.EncodingVersion);
            writer.Write((int)header.Genre);
            writer.Write(header.BPM);
            writer.Write(header.LevelEx);
            writer.Write(header.LevelNx);
            writer.Write(header.LevelHx);
            writer.Write(header.Padding);
            writer.Write(header.EventCountEx);
            writer.Write(header.EventCountNx);
            writer.Write(header.EventCountHx);
            writer.Write(header.NoteCountEx);
            writer.Write(header.NoteCountNx);
            writer.Write(header.NoteCountHx);
            writer.Write(header.MeasureCountEx);
            writer.Write(header.MeasureCountNx);
            writer.Write(header.MeasureCountHx);
            writer.Write(header.BlockCountEx);
            writer.Write(header.BlockCountNx);
            writer.Write(header.BlockCountHx);
            writer.Write(header.OldEncodingVersion);
            writer.Write(header.OldSongId);
            writer.Write(header.OldGenre);
            writer.Write(header.ThumbnailSize);
            writer.Write(header.FileVersion);
            writer.Write(header.Title);
            writer.Write(header.Artist);
            writer.Write(header.Pattern);
            writer.Write(header.OJM);
            writer.Write(header.CoverSize);
            writer.Write(header.DurationEx);
            writer.Write(header.DurationNx);
            writer.Write(header.DurationHx);
            writer.Write(header.BlockOffsetEx);
            writer.Write(header.BlockOffsetNx);
            writer.Write(header.BlockOffsetHx);
            writer.Write(header.CoverOffset);

            return mstream.ToArray();
        }
    }
}
