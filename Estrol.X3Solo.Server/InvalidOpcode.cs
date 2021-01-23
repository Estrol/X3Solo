using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Solo.Server {
    public class InvalidOpcodeException : Exception {
        public ushort Opcode;
        public byte[] FullData;

        public InvalidOpcodeException(ushort Opcode, byte[] FullData) : base() {
            this.Opcode = Opcode;
            this.FullData = FullData;
        }

        public override string Message {
            get {
                return $"Unhandled opcode: {Opcode:X4}\nPlease report this to Estrol#0021";
            }
        }

        public string FullDataString => ToHexString(FullData);

        private static string ToHexString(byte[] bData) {
            return BitConverter.ToString(bData).Replace("-", " ");
        }
    }
}
