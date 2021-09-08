namespace Estrol.X3Solo.Data {
    public class ItemData {
        public string ItemName { set; get; }
        public int Offset { set; get; }
        public int Size1 { set; get; }
        public int Size2 { set; get; }

        public int UNK1 { set; get; }
        public byte[] UNK2 { set; get; }

        public byte[] Data { set; get; }
    }
}
