using System.Collections.Generic;
using System.IO;

namespace Estrol.X3Solo.Data {
    public class OJSData {
        public OJSHeader Header;
        public List<OJSFrame> Frames;

        public OJSData(byte[] bData) {
            using MemoryStream ms = new(bData);
            using BinaryReader br = new(ms);

            Header = new() {
                Format = br.ReadUInt16(),
                ColorFormat = br.ReadUInt16(),
                FrameCount = br.ReadUInt16()
            };

            Frames = new(Header.FrameCount);

            int tmpStartOffset = 6 + (Header.FrameCount * 20);
            for (int i = 0; i < Header.FrameCount; i++) {
                ms.Position = 6 + (i * 20);

                OJSFrame ojs = new(br.ReadBytes(20), i);
                ms.Position = tmpStartOffset + ojs.Header.DataOffset;
                ojs.Data = br.ReadBytes((int)ojs.Header.DataLength);
                Frames.Add(ojs);
            }
        }

        public class OJSHeader {
            public ushort Format { set; get; }
            public ushort ColorFormat { set; get; }
            public ushort FrameCount { set; get; }
        }
    }
}
