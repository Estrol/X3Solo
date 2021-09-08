using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using Estrol.X3Solo.Data;
using Estrol.X3Solo.Library;
using Estrol.X3Solo.Server;
using Estrol.X3Solo.Server.Data;
using Estrol.X3Solo.Parser;

namespace Estrol.X3Solo {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MoreSettingWindow : Window {
        private readonly Configuration config;
        private ItemGender gender;
        private OPIParser parser;
        private ItemList[] ItemLists;
        private bool IsHide;

        public ClientCheck client;

        public MoreSettingWindow(Configuration conf) {
            config = conf;
            IsHide = true;

            InitializeComponent();
        }

        public new void Show() {
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OTwo.exe"))) {
                _ = MessageBox.Show("Cannot find game executable! (OTwo.exe)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Hide();
                return;
            }

            base.Show();

            if (client == null) {
                client = new();
                client.GetVersion("OTwo.exe");

                MusicList.ItemsSource = null;
            }

            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", client.Avatar))) {
                _ = MessageBox.Show(
                    string.Format(CultureInfo.CurrentCulture, "Cannot find game avatar file! ({0})", client.Avatar),
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                Hide();
                return;
            }

            if (parser != null) {
                IsHide = false;
                ReloadComboBox();
                return;
            }

            gender = config.Value("Gender") == 1 ? ItemGender.Male : ItemGender.Female;
            _ = Instrument.Items.Add(new ComboBoxItem().Content = "None");
            _ = Hair.Items.Add(new ComboBoxItem().Content = "None");
            _ = Accessory.Items.Add(new ComboBoxItem().Content = "None");
            _ = Glove.Items.Add(new ComboBoxItem().Content = "None");
            _ = Necklace.Items.Add(new ComboBoxItem().Content = "None");
            _ = Glass.Items.Add(new ComboBoxItem().Content = "None");
            _ = Hat.Items.Add(new ComboBoxItem().Content = "None");
            _ = Pant.Items.Add(new ComboBoxItem().Content = "None");
            _ = Earring.Items.Add(new ComboBoxItem().Content = "None");
            _ = Shoe.Items.Add(new ComboBoxItem().Content = "None");
            _ = Wing.Items.Add(new ComboBoxItem().Content = "None");
            _ = Pet.Items.Add(new ComboBoxItem().Content = "None");

            parser = new(this);
            byte[] data = parser.GetRawOJS("Avatar", "Itemdata_China.dat");
            if (data == null) {
                data = parser.GetRawOJS("Avatar", "ItemData_China.dat");
            }

            parser.Destroy();
            ItemLists = ItemListParser.LoadData(data, this);

            foreach (ItemList item in ItemLists) {
                if (item.Gender == ItemGender.Unknown) {
                    continue;
                }

                if (item.ItemCategory is ItemCategory.InstrumentBass or
                    ItemCategory.InstrumentDrum or
                    ItemCategory.InstrumentGuitar or
                    ItemCategory.InstrumentPiano) {
                    _ = Instrument.Items.Add(new ComboBoxItem().Content = item.Name);
                }

                if (item.ItemCategory == ItemCategory.Hair) {
                    if (item.Gender != ItemGender.Both) {
                        if (item.Gender == gender) {
                            _ = Hair.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    } else {
                        if (item.Gender == ItemGender.Both) {
                            _ = Hair.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    }
                }

                if (item.ItemCategory is ItemCategory.Accessory or ItemCategory.Accessory2) {
                    if (item.Gender != ItemGender.Both) {
                        if (item.Gender == gender) {
                            _ = Accessory.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    } else {
                        if (item.Gender == ItemGender.Both) {
                            _ = Accessory.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    }
                }

                if (item.ItemCategory == ItemCategory.Glove) {
                    if (item.Gender != ItemGender.Both) {
                        if (item.Gender == gender) {
                            _ = Glove.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    } else {
                        if (item.Gender == ItemGender.Both) {
                            _ = Glove.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    }
                }

                if (item.ItemCategory == ItemCategory.Necklace) {
                    if (item.Gender != ItemGender.Both) {
                        if (item.Gender == gender) {
                            _ = Necklace.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    } else {
                        if (item.Gender == ItemGender.Both) {
                            _ = Necklace.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    }
                }

                if (item.ItemCategory == ItemCategory.Hat) {
                    if (item.Gender != ItemGender.Both) {
                        if (item.Gender == gender) {
                            _ = Hat.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    } else {
                        if (item.Gender == ItemGender.Both) {
                            _ = Hat.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    }
                }

                if (item.ItemCategory == ItemCategory.Pant) {
                    if (item.Gender != ItemGender.Both) {
                        if (item.Gender == gender) {
                            _ = Pant.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    } else {
                        if (item.Gender == ItemGender.Both) {
                            _ = Pant.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    }
                }

                if (item.ItemCategory == ItemCategory.Glass) {
                    if (item.Gender != ItemGender.Both) {
                        if (item.Gender == gender) {
                            _ = Glass.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    } else {
                        if (item.Gender == ItemGender.Both) {
                            _ = Glass.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    }
                }

                if (item.ItemCategory == ItemCategory.Earring) {
                    if (item.Gender != ItemGender.Both) {
                        if (item.Gender == gender) {
                            _ = Earring.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    } else {
                        if (item.Gender == ItemGender.Both) {
                            _ = Earring.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    }
                }

                if (item.ItemCategory == ItemCategory.Shoe) {
                    if (item.Gender != ItemGender.Both) {
                        if (item.Gender == gender) {
                            _ = Shoe.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    } else {
                        if (item.Gender == ItemGender.Both) {
                            _ = Shoe.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    }
                }

                if (item.ItemCategory == ItemCategory.Wing) {
                    if (item.Gender != ItemGender.Both) {
                        if (item.Gender == gender) {
                            _ = Wing.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    } else {
                        if (item.Gender == ItemGender.Both) {
                            _ = Wing.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    }
                }

                if (item.ItemCategory == ItemCategory.Pet) {
                    if (item.Gender != ItemGender.Both) {
                        if (item.Gender == gender) {
                            _ = Pet.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    } else {
                        if (item.Gender == ItemGender.Both) {
                            _ = Pet.Items.Add(new ComboBoxItem().Content = item.Name);
                        }
                    }
                }
            }

            IsHide = false;
            ReloadComboBox();
            LoadOJNList();
        }

        public ComboBox this[string index] => index switch {
            "Instrument" => Instrument,
            "Hair" => Hair,
            "Accessory" => Accessory,
            "Glove" => Glove,
            "Necklace" => Necklace,
            "Top" => Hat,
            "Pant" => Pant,
            "Glass" => Glass,
            "Earring" => Earring,
            "Shoe" => Shoe,
            "Wing" => Wing,
            "Pet" => Pet,
            _ => throw new InvalidDataException($"ComboxBox name {index} does not exist")
        };

        private readonly string[] propNames = {
            "Instrument", "Hair", "Accessory", "Glove",
            "Necklace", "Top", "Pant", "Glass",
            "Earring", "Shoe", "Wing", "Pet"
        };

        public void ReloadComboBox() {
            foreach (string prop in propNames) {
                int val = config.Value(prop);
                int index = 0;

                if (val > 0) {
                    bool IsFail = false;

                    ItemList item = FindItemFromId(val);
                    if (item.Gender != ItemGender.Both) {
                        if (item.Gender != gender) {
                            Log.Write($"ReloadComboBox -> Expected {gender} but got {item.Gender}");

                            _ = MessageBox.Show($"Item with slot: {prop} has equipped with wrong gender");
                            config.Set(prop, 0);

                            IsFail = true;
                        }
                    }

                    if (!IsFail) {
                        index = this[prop].Items.IndexOf(item.Name);
                    }
                }

                this[prop].SelectedIndex = index;
            }
        }

        public ItemList FindItemFromName(string name) {
            return ItemLists.ToList().Find(x => x.Name == name);
        }

        public ItemList FindItemFromId(int Id) {
            return ItemLists.ToList().Find(x => x.Id == Id);
        }

        public void LoadOJNList(bool forceCreateOJNList = false) {
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", client.OJNList)) || forceCreateOJNList) {
                bool result = OJNListGenerator.GenerateOJNList(client);
                if (!result) {
                    return;
                }
            }

            MusicList.ItemsSource = null;

            OJNList ojnlist = OJNListDecoder.Decode(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", client.OJNList));
            OJN[] headers = ojnlist.GetHeaders();

            List<ColumHeader> lists = new();
            for (int i = 0; i < headers.Length; i++) {
                OJN header = headers[i];
                bool IsExist = File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, client.Music, header.OJMString));

                lists.Add(new() {
                    NO = i,
                    ID = header.Id,
                    NAME = header.TitleString,
                    EX = header.LevelEx,
                    NX = header.LevelHx,
                    HX = header.LevelHx,
                    STATUS = IsExist ? "OK" : "Missing"
                });
            }

            MusicList.ItemsSource = lists;
        }

        private void BtnSend(object sender, RoutedEventArgs e) {
            Button button = (Button)sender;
            if (button.Name == "Save") {
                foreach (string prop in propNames) {
                    if (this[prop].Text == "None") {
                        continue;
                    }

                    ItemList item = FindItemFromName(this[prop].Text);
                    if (item.Gender != ItemGender.Both) {
                        if (item.Gender != gender) {
                            _ = MessageBox.Show($"Failed to save: {prop} because expected {gender} but got {item.Gender}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    config.Set(prop, item.Id);
                }

                _ = MessageBox.Show("Character settings has been saved!");
            } else if (button.Name == "Reset") {
                config.Set("Gender", Gender.SelectedIndex == 0 ? 0 : 1);
                config.Set("Face", Gender.SelectedIndex == 0 ? 36 : 35);

                config.Set("Instrument", 0);
                config.Set("Hair", 0);
                config.Set("Accessory", 0);
                config.Set("Glove", 0);
                config.Set("Necklace", 0);
                config.Set("Top", 0);
                config.Set("Pant", 0);
                config.Set("Glass", 0);
                config.Set("Earring", 0);
                config.Set("Shoe", 0);
                config.Set("Wing", 0);
                config.Set("HairAccessory", 0);
                config.Set("InstrumentAccessory", 0);
                config.Set("ClothAccessory", 0);
                config.Set("Pet", 0);

                ReloadComboBox();

                _ = MessageBox.Show("Character settings has been reset!");
            }
        }

        private bool IsInvoke = true;
        private void Gender_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (IsInvoke && !IsHide) {
                MessageBoxResult result = MessageBox.Show("This will reset all clothing as some of clothing were gender specified, continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.No) {
                    ComboBox combo = (ComboBox)sender;
                    IsInvoke = false;
                    combo.SelectedItem = e.RemovedItems[0];
                    return;
                }

                config.Set("Gender", Gender.SelectedIndex == 0 ? 0 : 1);
                config.Set("Face", Gender.SelectedIndex == 0 ? 36 : 35);

                config.Set("Instrument", 0);
                config.Set("Hair", 0);
                config.Set("Accessory", 0);
                config.Set("Glove", 0);
                config.Set("Necklace", 0);
                config.Set("Top", 0);
                config.Set("Pant", 0);
                config.Set("Glass", 0);
                config.Set("Earring", 0);
                config.Set("Shoe", 0);
                config.Set("Wing", 0);
                config.Set("HairAccessory", 0);
                config.Set("InstrumentAccessory", 0);
                config.Set("ClothAccessory", 0);
                config.Set("Pet", 0);

                ReloadComboBox();
            }

            IsInvoke = true;
        }

        private class ColumHeader {
            public int NO { set; get; }
            public int ID { set; get; }
            public string NAME { set; get; }
            public int EX { set; get; }
            public int NX { set; get; }
            public int HX { set; get; }
            public string STATUS { set; get; }
        }

        public new void Hide() {
            IsHide = true;
            base.Hide();
        }

        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private void BtnExit(object sender, RoutedEventArgs e) {
            Hide();
        }

        private void Window_Closing(object sender, CancelEventArgs e) {
            Hide();
            e.Cancel = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            LoadOJNList(true);
        }
    }
}
