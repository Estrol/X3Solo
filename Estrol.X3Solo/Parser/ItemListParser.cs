using Estrol.X3Solo.Data;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Estrol.X3Solo {
    public class ItemListParser {
        public static ItemList[] LoadData(byte[] data, MoreSettingWindow wnd) {
            using MemoryStream ms = new(data);
            using BinaryReader br = new(ms);

            int count = br.ReadInt16();
            List<ItemList> lists = new(count);

            wnd.Dispatcher.Invoke(() => {
                wnd.ProgressBarLabel.Value = 0;
                wnd.ProgressBarLabel.Maximum = count;
            });

            for (int i = 0; i < count - 1; i++) {
                bool IsSkip = false;

                ItemList item = new() {
                    Id = br.ReadInt32(),
                    ItemCategory = (ItemCategory)br.ReadByte(),
                    Planet = br.ReadByte(),
                    Flags = br.ReadUInt16(), // Gender...
                    Amount = br.ReadUInt16(),
                    ItemSpecial = br.ReadByte(),
                    ItemFunction = (ItemFunction)br.ReadByte(),
                    PayMethod = br.ReadByte(),
                    PriceGold = br.ReadInt32(),
                    PriceEP = br.ReadInt32(),
                    RoomRenderCategory = br.ReadByte(),
                };

                switch (item.Flags) {
                    // Female
                    case 0: {
                        item.Gender = ItemGender.Female;
                        break;
                    }

                    // Male
                    case 128: {
                        item.Gender = ItemGender.Male;
                        break;
                    }

                    // Female and Male
                    case 256: {
                        item.Gender = ItemGender.Both;
                        break;
                    }

                    // Female and New
                    case 2048: {
                        item.Gender = ItemGender.Female;
                        break;
                    }

                    // Male and New
                    case 2176: {
                        item.Gender = ItemGender.Male;
                        break;
                    }

                    // Both and New
                    case 2304: {
                        item.Gender = ItemGender.Both;
                        break;
                    }

                    default: {
                        item.Gender = ItemGender.Unknown;
                        break;
                    }
                }

                item.Files = i == 0 ? (new string[11]) : (new string[42]);

                int nameLen = br.ReadInt32();

                item.Name = Encoding.UTF8.GetString(br.ReadBytes(nameLen));

                int descLen = br.ReadInt32();

                item.Description = Encoding.UTF8.GetString(br.ReadBytes(descLen));

                int nameCount = nameLen + descLen;
                if (nameCount > 0) {
                    wnd.Dispatcher.Invoke(() => {
                        wnd.ProgressBarLabel.Value = i;
                    });

                    for (int i2 = 0; i2 < item.Files.Length; i2++) {
                        if (i != 0) {
                            _ = br.ReadByte(); // Padding as randam says /shrug
                        }

                        int itemLen = br.ReadInt32();
                        item.Files[i2] = itemLen > 0 ? Encoding.UTF8.GetString(br.ReadBytes(itemLen)) : "";
                    }
                } else {
                    // This implementation is ugly af, but it works
                    // Basically this exist because weird structure in Itemlist_China.dat
                    int positionToFind = i + 1;
                    int position = 0;
                    while (true) {
                        int pos = (int)br.BaseStream.Position;
                        int current = br.ReadInt32();
                        if (current == positionToFind) {
                            position = pos;
                        } else {
                            br.BaseStream.Position = pos + 1;
                        }

                        if (position > 0) {
                            break;
                        }
                    }

                    br.BaseStream.Position = position;

                    IsSkip = true;
                }

                if (!IsSkip) {
                    lists.Add(item);
                }
            }

            return lists.ToArray();
        }
    }
}
