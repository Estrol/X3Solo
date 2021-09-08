using System;

namespace Estrol.X3Solo.Server {
    public class InvalidOpcodeException : Exception {
        public ushort Opcode { set; get; }
        public byte[] FullData { set; get; }

        public InvalidOpcodeException(ushort Opcode, byte[] FullData) : base() {
            this.Opcode = Opcode;
            this.FullData = FullData;
        }

        public override string Message => $"Unhandled opcode: {Opcode:X4}\nPlease report this to Estrol#0021";

        public string FullDataString => ToHexString(FullData);

        private static string ToHexString(byte[] bData) {
            return BitConverter.ToString(bData).Replace("-", " ");
        }
    }
}
