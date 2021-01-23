using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Solo.Server {
    public class Message {
        public DateTime Time { set; get; }
        public Packets Opcode { set; get; }
        public ushort _opcode { set; get; }
        public byte[] data { set; get; }
        public byte[] fullD { set; get; }

        private readonly MemoryStream ms;
        private readonly BinaryReader br;

        public Message(string version, byte[] raw_data) {
            ms = new MemoryStream(raw_data);
            br = new BinaryReader(ms);

            if (version == "1.8") {
                Parse18();
            } else if (version == "1.5") {
                Parse15();
            }
        }

        public void Parse18() {
            br.BaseStream.Seek(2, SeekOrigin.Begin);
            int Timestamp = br.ReadInt32();
            Time = new DateTime(1970, 1, 1).AddSeconds(Timestamp);

            br.BaseStream.Seek(26, SeekOrigin.Begin);
            int OffsetLength = br.ReadInt32();
            int len = br.ReadInt32();

            data = br.ReadBytes(OffsetLength);

            byte[] bLen = BitConverter.GetBytes((short)len);
            fullD = bLen.Concat(data).ToArray();

            _opcode = BitConverter.ToUInt16(data, 0);
            Opcode = (Packets)_opcode;
        }

        public void Parse15() {
            fullD = ms.ToArray();
            data = new byte[fullD.Length - 2];
            Buffer.BlockCopy(fullD, 2, data, 0, fullD.Length - 2);

            _opcode = BitConverter.ToUInt16(fullD, 2);
            Opcode = (Packets)_opcode;
        }
    }
}
