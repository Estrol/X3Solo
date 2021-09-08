using System.IO;

namespace Estrol.X3Solo.Data {
    public class OJSFrame {
        public struct FrameHeader {
            public ushort TransparentColor { set; get; }
            public short X { set; get; }
            public short Y { set; get; }
            public short Width { set; get; }
            public short Height { set; get; }

            public uint DataOffset { set; get; }
            public uint DataLength { set; get; }
            public ushort UNK1 { set; get; }
        }

        public FrameHeader Header { set; get; }
        public MemoryStream Stream { set; get; }
        public BinaryReader Reader { set; get; }
        public byte[] Data { set; get; }
        public int FrameIndex { set; get; }

        public OJSFrame(byte[] bData, int frameIndex) {
            FrameIndex = frameIndex;

            Stream = new(bData);
            Reader = new(Stream);

            Header = new() {
                TransparentColor = Reader.ReadUInt16(),
                X = Reader.ReadInt16(),
                Y = Reader.ReadInt16(),
                Width = Reader.ReadInt16(),
                Height = Reader.ReadInt16(),
                DataOffset = Reader.ReadUInt32(),
                DataLength = Reader.ReadUInt32(),
                UNK1 = Reader.ReadUInt16()
            };
        }
    }
}
