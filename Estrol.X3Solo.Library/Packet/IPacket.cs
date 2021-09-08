namespace Estrol.X3Solo.Library.Packet {
    public class IPacket {
        public IPacket(Opcodes opcodeId, byte[] data, byte[] fullData) {
            Id = opcodeId;
            Data = data;
            FullData = fullData;
        }

        public Opcodes Id { private set; get; }
        public byte[] Data { private set; get; }
        public byte[] FullData { set; get; }
    }
}
