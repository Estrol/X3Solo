#pragma warning disable CA1711

using System.Linq;
using System.Text;

namespace Estrol.X3Solo.Server.Data {
    public class OJN {
        public int Id { get; set; }
        public byte[] Signature { get; set; }
        public float EncodingVersion { get; set; }
        public OJNGenre Genre { get; set; }
        public float BPM { get; set; }
        public short LevelEx { get; set; }
        public short LevelNx { get; set; }
        public short LevelHx { get; set; }
        public short Padding { get; set; }
        public int EventCountEx { get; set; }
        public int EventCountNx { get; set; }
        public int EventCountHx { get; set; }
        public int NoteCountEx { get; set; }
        public int NoteCountNx { get; set; }
        public int NoteCountHx { get; set; }
        public int MeasureCountEx { get; set; }
        public int MeasureCountNx { get; set; }
        public int MeasureCountHx { get; set; }
        public int BlockCountEx { get; set; }
        public int BlockCountNx { get; set; }
        public int BlockCountHx { get; set; }
        public short OldEncodingVersion { get; set; }
        public short OldSongId { get; set; }
        public byte[] OldGenre { get; set; }
        public int ThumbnailSize { get; set; }
        public int FileVersion { get; set; }
        public byte[] Title { get; set; }
        public string TitleString => CharacterEncoding.GetString(Title.TakeWhile(c => c != 0).ToArray()).Trim('\0');
        public byte[] Artist { get; set; }
        public string ArtistString => CharacterEncoding.GetString(Artist.TakeWhile(c => c != 0).ToArray()).Trim('\0');
        public byte[] Pattern { get; set; }
        public string PatternString => CharacterEncoding.GetString(Pattern.TakeWhile(c => c != 0).ToArray()).Trim('\0');
        public byte[] OJM { get; set; }
        public string OJMString => CharacterEncoding.GetString(OJM.TakeWhile(c => c != 0).ToArray()).Trim('\0');
        public int CoverSize { get; set; }
        public int DurationEx { get; set; }
        public int DurationNx { get; set; }
        public int DurationHx { get; set; }
        public int BlockOffsetEx { get; set; }
        public int BlockOffsetNx { get; set; }
        public int BlockOffsetHx { get; set; }
        public int CoverOffset { get; set; }
        public byte KeyMode { get; set; } = 7;
        public Encoding CharacterEncoding { get; set; } = Encoding.GetEncoding(949);
        public string Source { get; set; }
        public bool Encrypted { get; set; }

        public OJN() { }
        public override int GetHashCode() {
            return Id;
        }

    }
}
