using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Liberty.Controls
{
    /// <summary>
    /// Interaction logic for step3.xaml
    /// </summary>
    public partial class editWeapons : UserControl, StepUI.IStep
    {
        private Util.SaveManager<Reach.CampaignSave> _saveManager;
        private Reach.TagListManager _tagList;
        private ComboBox[] _weaponBoxes;
        private Dictionary<Reach.WeaponObject, int> _weaponIndices = new Dictionary<Reach.WeaponObject, int>();
        private Dictionary<uint, SortedSet<int>> _freeWeapons = new Dictionary<uint, SortedSet<int>>();
        private bool _loading = false;

        // Noble 6 node constants
        private static sbyte NobleSixPrimaryWeaponNode = 50;
        private static sbyte NobleSixSecondaryWeaponNode = 6;

        public editWeapons(Util.SaveManager<Reach.CampaignSave> saveManager, Reach.TagListManager tagList)
        {
            this.InitializeComponent();

            _saveManager = saveManager;
            _tagList = tagList;
            _weaponBoxes = new ComboBox[] { cbPrimaryWeapon, cbSecondaryWeapon, cbTertiaryWeapon, cbQuaternaryWeapon };

            cbPrimaryWeapon.Tag = gridPrimaryAmmo;
            cbSecondaryWeapon.Tag = gridSecondaryAmmo;
            cbTertiaryWeapon.Tag = gridTertiaryAmmo;
            cbQuaternaryWeapon.Tag = gridQuaternaryAmmo;
        }

        public void Load()
        {
            _loading = true;
            BuildWeaponLists();

            for (int i = 0; i < _weaponBoxes.Length; i++)
                EnableComboBox(i, false);

            Reach.BipedObject playerBiped = _saveManager.SaveData.Player.Biped;
            LoadWeapon(cbPrimaryWeapon, playerBiped.PrimaryWeapon);
            LoadWeapon(cbSecondaryWeapon, playerBiped.SecondaryWeapon);
            LoadWeapon(cbTertiaryWeapon, playerBiped.TertiaryWeapon);
            LoadWeapon(cbQuaternaryWeapon, playerBiped.QuaternaryWeapon);
            _loading = false;
        }

        public bool Save()
        {
            Reach.BipedObject playerBiped = _saveManager.SaveData.Player.Biped;
            for (int i = 0; i < _weaponBoxes.Length; i++)
            {
                ComboBox box = _weaponBoxes[i];
                Grid grid = (Grid)box.Tag;
                if (grid.Children.Count > 0)
                {
                    // Replace the weapon if it does not match
                    ComboBoxItem item = (ComboBoxItem)box.SelectedItem;
                    WeaponItem weapon = (WeaponItem)item.Tag;
                    if (weapon.Object != playerBiped.GetWeapon(i))
                    {
                        playerBiped.ChangeWeapon(i, weapon.Object);
                        if (_tagList.Identify(playerBiped, false) == "Noble 6")
                        {
                            if (i == 0)
                                weapon.Object.ParentNode = NobleSixPrimaryWeaponNode;
                            else
                                weapon.Object.ParentNode = NobleSixSecondaryWeaponNode;

                            weapon.Object.X = 0;
                            weapon.Object.Y = 0;
                            weapon.Object.Z = 0;
                        }
                    }

                    // Save its ammo info
                    Util.IAmmoDisplay display = (Util.IAmmoDisplay)grid.Children[0];
                    display.Save();
                }
            }
            return true;
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            Visibility = Visibility.Hidden;
        }

        private void EnableComboBox(int index, bool enable)
        {
            _weaponBoxes[index].IsEnabled = enable;
            Grid grid = (Grid)_weaponBoxes[index].Tag;
            grid.IsEnabled = enable;
        }

        private class WeaponItem : IComparable<WeaponItem>
        {
            public string Name { get; set; }
            public Reach.WeaponObject Object { get; set; }
            public int Index { get; set; }

            public int CompareTo(WeaponItem other)
            {
                int nameComp = Name.ToLower().CompareTo(other.Name.ToLower());
                if (nameComp != 0)
                    return nameComp;
                return Object.ID.CompareTo(other.Object.ID);
            }
        }

        private void BuildWeaponLists()
        {
            _freeWeapons.Clear();
            _weaponIndices.Clear();

            foreach (ComboBox box in _weaponBoxes)
            {
                box.Items.Clear();

                ComboBoxItem emptyItem = new ComboBoxItem();
                emptyItem.Content = "(nothing)";
                box.Items.Add(emptyItem);
            }

            // Sort the weapons by name into a set
            Reach.CampaignSave saveData = _saveManager.SaveData;
            SortedSet<WeaponItem> weapons = new SortedSet<WeaponItem>();
            foreach (Reach.GameObject obj in saveData.Objects)
            {
                if (obj != null && obj.TagGroup == Reach.TagGroup.Weap && !obj.Deleted && (obj.Carrier == null || obj.Carrier.TagGroup != Reach.TagGroup.Vehi))
                {
                    string name = _tagList.Identify(obj, false);
                    if (name.StartsWith("Spartan"))
                        continue;

                    weapons.Add(new WeaponItem() { Name = name, Object = (Reach.WeaponObject)obj });
                }
            }

            // Now add them to the combo boxes
            foreach (WeaponItem weapon in weapons)
            {
                // Grab the free indices set for the combo box
                SortedSet<int> freeIndices;
                if (!_freeWeapons.TryGetValue(weapon.Object.MapID, out freeIndices))
                {
                    freeIndices = new SortedSet<int>();
                    _freeWeapons[weapon.Object.MapID] = freeIndices;
                }

                // Mark the weapon as unused
                int index = _weaponBoxes[0].Items.Count;
                freeIndices.Add(index);
                weapon.Index = index;
                _weaponIndices[weapon.Object] = index;

                // Add it to each combo box
                foreach (ComboBox box in _weaponBoxes)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = weapon.Name;
                    item.Tag = weapon;
                    if (freeIndices.Count > 1)
                        item.Visibility = Visibility.Collapsed;
                    box.Items.Add(item);
                }
            }
        }

        private void LoadWeapon(ComboBox typeBox, Reach.WeaponObject weapon)
        {
            if (weapon != null)
                typeBox.SelectedIndex = ChangeWeapon(typeBox, 0, _weaponIndices[weapon]);
            else
                typeBox.SelectedIndex = ChangeWeapon(typeBox, 0, 0);
        }

        /// <summary>
        /// Propagates a weapon selection change to all of the ComboBoxes.
        /// </summary>
        /// <param name="freeWeapons">The queue to add free weapons from</param>
        /// <param name="mapId">The map ID of the affected item</param>
        /// <param name="index">The index of the affected item</param>
        /// <param name="ignore">The ComboBox to ignore when propagating changes</param>
        private void PropagateWeaponChange(SortedSet<int> freeWeapons, uint mapId, int index, ComboBox ignore)
        {
            /* The basic idea behind this is that if a weapon is not selected in
             * a combo box, then all combo boxes are given items for the same
             * weapon (preventing the issue of having less available weapons
             * than combo boxes). Once an item is selected, all other combo
             * boxes have their items hidden and replaced with an unused weapon
             * item. This ensures that all combo boxes will show the weapon type
             * as long as all of the weapons are not selected.
             *
             * Something tells me there's a better way to do this though :P */

            int newIndex = freeWeapons.Min;
            foreach (ComboBox box in _weaponBoxes)
            {
                if (box != ignore)
                {
                    ComboBoxItem targetItem = (ComboBoxItem)box.Items[index];
                    targetItem.Visibility = Visibility.Collapsed;

                    // Don't add anything to the combo box if a weapon of this class is already in the box
                    bool alreadyIn = false;
                    foreach (ComboBoxItem item in box.Items)
                    {
                        if (item != null && item.Tag != null)
                        {
                            WeaponItem weapon = (WeaponItem)item.Tag;
                            if (weapon.Object.MapID == mapId && item.Visibility == Visibility.Visible)
                                alreadyIn = true;
                        }
                    }

                    if (!alreadyIn && freeWeapons.Count > 0)
                    {
                        ComboBoxItem newItem = (ComboBoxItem)box.Items[newIndex];
                        newItem.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void EnableNextComboBox(int thisIndex, bool enable)
        {
            if (thisIndex < _weaponBoxes.Length - 1)
                EnableComboBox(thisIndex + 1, enable);

            // Hide/unhide the (nothing) item
            if (thisIndex > 0)
            {
                ComboBoxItem prevNothingItem = (ComboBoxItem)_weaponBoxes[thisIndex - 1].Items[0];
                prevNothingItem.Visibility = enable ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private int ChangeWeapon(ComboBox typeBox, int oldIndex, int newIndex)
        {
            Grid ammoGrid = (Grid)typeBox.Tag;
            ammoGrid.Children.Clear();

            if (oldIndex > 0)
            {
                // Free the deselected item and allocate new items for any ComboBox without the weapon type
                ComboBoxItem item = (ComboBoxItem)typeBox.Items[oldIndex];
                WeaponItem weapon = (WeaponItem)item.Tag;
                SortedSet<int> freeWeapons = _freeWeapons[weapon.Object.MapID];
                freeWeapons.Add(oldIndex);
                PropagateWeaponChange(freeWeapons, weapon.Object.MapID, oldIndex, null);
            }

            // Find our index in the weapon box list (for enabling/disabling)
            int boxIndex = 0;
            for (int i = 0; i < _weaponBoxes.Length; i++)
            {
                if (_weaponBoxes[i] == typeBox)
                    boxIndex = i;
            }

            if (newIndex > 0)
            {
                // Mark the weapon as used and allocate new items for each ComboBox
                WeaponItem item = (WeaponItem)((ComboBoxItem)typeBox.Items[newIndex]).Tag;
                SortedSet<int> freeWeapons = _freeWeapons[item.Object.MapID];
                freeWeapons.Remove(newIndex);
                PropagateWeaponChange(freeWeapons, item.Object.MapID, newIndex, typeBox);

                string name = _tagList.Identify(item.Object, true);
                name = name.ToLower();

                // Display the weapon details
                UIElement display;
                if (name.Contains("plasma") || name.Contains("energy") || name.Contains("laser"))
                    display = new plasmaAmmoDisplay(item.Object);
                else if (name.Contains("target") || name.Contains("locator"))
                    display = new targetLocatorDisplay(_saveManager.SaveData);
                else
                    display = new regularAmmoDisplay(item.Object);
                ammoGrid.Children.Add(display);

                EnableComboBox(boxIndex, true);
                EnableNextComboBox(boxIndex, true);
                return newIndex;
            }
            else
            {
                EnableNextComboBox(boxIndex, false);
                return 0;
            }
        }

        private void WeaponSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_loading)
                return;

            // Get the old index
            int oldIndex = -1;
            if (e.RemovedItems.Count > 0)
            {
                ComboBoxItem oldItem = (ComboBoxItem)e.RemovedItems[0];
                if (oldItem.Tag != null)
                    oldIndex = ((WeaponItem)oldItem.Tag).Index;
            }

            ComboBox box = (ComboBox)sender;
            ChangeWeapon(box, oldIndex, box.SelectedIndex);
        }
    }
}