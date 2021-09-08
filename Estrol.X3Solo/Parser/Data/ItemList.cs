namespace Estrol.X3Solo.Data {
    public class ItemList {
        public int Id { set; get; }
        public ItemCategory ItemCategory { set; get; }
        public byte Planet { set; get; }
        public ushort Flags { set; get; }
        public ItemGender Gender { set; get; }
        public ushort Amount { set; get; }
        public byte ItemSpecial { set; get; }
        public ItemFunction ItemFunction { set; get; }
        public byte PayMethod { set; get; }
        public int PriceGold { set; get; }
        public int PriceEP { set; get; }
        public byte RoomRenderCategory { set; get; }

        public string Name { set; get; }
        public string Description { set; get; }
        public string[] Files { set; get; }
    }
}
