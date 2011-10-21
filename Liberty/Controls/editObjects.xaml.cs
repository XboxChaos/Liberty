// Uncomment to enable the node editor
//#define ENABLE_NODE_EDITOR

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
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace Liberty.Controls
{
	/// <summary>
	/// Interaction logic for step4.xaml
	/// </summary>
	public partial class editObjects : UserControl, StepUI.IStep
	{
        private Reach.CampaignSave _saveData = null;
        private int currentChunkIndex = -1;
        private string currentParentNodeTag = null;
        public TabItem currentPlugin = null;
        Reach.BipedObject objBipd;
        Reach.WeaponObject objWeap;
        Reach.VehicleObject objVehi;
        private List<TreeViewItem> objectItems = new List<TreeViewItem>();
        private Dictionary<string, TreeViewItem> tagGroupItems = new Dictionary<string, TreeViewItem>();
        private Reach.ModelNode currentNode = null;
        BrushConverter bc = new BrushConverter();
        private MainWindow mainWindow = null;
        private ListBoxItem[] weaponListItems = new ListBoxItem[4];

		public editObjects()
		{
			InitializeComponent();

            this.Loaded += new RoutedEventHandler(step4_Loaded);
		}

        void step4_Loaded(object sender, RoutedEventArgs e)
        {
            // Grab the parent window
            mainWindow = Window.GetWindow(this) as MainWindow;
        }

        public void Load(Util.SaveManager saveManager)
        {
            _saveData = saveManager.SaveData;

            // Reset selected item
            currentChunkIndex = -1;
            if (tVObjects.SelectedItem != null)
                ((TreeViewItem)tVObjects.SelectedItem).IsSelected = false;
            currentNode = null;

            // Hide buttons
            btnDelete.Visibility = Visibility.Hidden;
            btnReplace.Visibility = Visibility.Hidden;
            btnMassMove.Visibility = Visibility.Hidden;

            // Show the instructions
            instructions.Visibility = Visibility.Visible;
            tabs.Visibility = Visibility.Hidden;

#if !ENABLE_NODE_EDITOR
            tabNodes.Visibility = Visibility.Collapsed;
#endif

            // Clear out the object list
            tVObjects.Items.Clear();
            addTagGroupItem("bipd", "Bipeds");
            addTagGroupItem("ctrl", "Controls");
            addTagGroupItem("crea", "Creatures");
            addTagGroupItem("term", "Data Pads");
            addTagGroupItem("efsc", "Effects");
            addTagGroupItem("eqip", "Equipment");
            addTagGroupItem("mach", "Machines");
            addTagGroupItem("bloc", "Obstacles");
            addTagGroupItem("proj", "Projectiles");
            addTagGroupItem("scen", "Scenery");
            addTagGroupItem("ssce", "Scenery Sounds");
            addTagGroupItem("vehi", "Vehicles");
            addTagGroupItem("weap", "Weapons");
            addTagGroupItem("unknown", "Other");

            // Now add everything to the treeview
            Reach.BipedObject playerBiped = _saveData.Player.Biped;
            objectItems.Clear();
            int i = 0;
            bool guessName = classInfo.storage.settings.applicationSettings.lookUpObjectTypes;
            foreach (Reach.GameObject obj in _saveData.Objects)
            {
                if (obj != null && !obj.Deleted)
                {
                    var items = tVObjects.Items;

                    TreeViewItem tvi = new TreeViewItem();
                    tvi.Name = "tVItem" + i.ToString();

                    tvi.Header = "[" + i.ToString() + "] " + saveManager.IdentifyObject(obj, guessName);
                    tvi.Tag = i;

                    if (objectsAreRelated(playerBiped, obj))
                        tvi.FontWeight = FontWeights.Bold;

                    findTagGroupItem(obj.TagGroup).Items.Add(tvi);
                    objectItems.Add(tvi);
                }
                else
                {
                    objectItems.Add(null);
                }

                i++;
            }
        }

        public bool Save(Util.SaveManager saveManager)
        {
            if (currentChunkIndex != -1)
                saveValues();

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

        /*private void changeTab(TextBlock newTab)
        {
            ((UIElement)currentTab.Tag).Visibility = Visibility.Hidden;
            currentTab.FontWeight = FontWeights.Normal;
            currentTab.Foreground = (Brush)bc.ConvertFrom("#FF868686");

            ((UIElement)newTab.Tag).Visibility = Visibility.Visible;
            newTab.Foreground = (Brush)bc.ConvertFrom("#FF000000");
            newTab.FontWeight = FontWeights.ExtraBold;

            currentTab = newTab;
        }*/

        private bool textBoxChanged(TextBox textBox)
        {
            return (bool)textBox.Tag;
        }

        private void savePluginData()
        {
            Reach.GameObject obj = _saveData.Objects[currentChunkIndex];
            switch (obj.TagGroup)
            {
                case Reach.TagGroup.Bipd:
                    objBipd.Invincible = (bool)cBBipdInvici.IsChecked;
                    objBipd.PlasmaGrenades = Convert.ToSByte(txtBipdPlasmaNade.Text);
                    objBipd.FragGrenades = Convert.ToSByte(txtBipdFragNade.Text);
                    break;

                case Reach.TagGroup.Weap:
                    //objWeap.Invincible = (bool)cBWeapInvici.IsChecked;
                    objWeap.Ammo = Convert.ToInt16(txtWeapPluginAmmo.Text);
                    objWeap.ClipAmmo = Convert.ToInt16(txtWeapPluginClipAmmo.Text);
                    break;

                case Reach.TagGroup.Vehi:
                    objVehi.Invincible = (bool)cBVehiInvici.IsChecked;
                    break;
            }
        }

        private void saveValues()
        {
            //Save those sexy values
            Reach.GameObject currentObject = _saveData.Objects[currentChunkIndex];
            if (textBoxChanged(txtObjectXCord)) currentObject.X = Convert.ToSingle(txtObjectXCord.Text);
            if (textBoxChanged(txtObjectYCord)) currentObject.Y = Convert.ToSingle(txtObjectYCord.Text);
            if (textBoxChanged(txtObjectZCord)) currentObject.Z = Convert.ToSingle(txtObjectZCord.Text);
            if (textBoxChanged(txtObjectScale)) currentObject.Scale = Convert.ToSingle(txtObjectScale.Text);

            if (textBoxChanged(txtObjectYaw) ||
                textBoxChanged(txtObjectPitch) ||
                textBoxChanged(txtObjectRoll))
            {
                float yaw = MathUtil.Convert.ToRadians(Convert.ToSingle(txtObjectYaw.Text));
                float pitch = MathUtil.Convert.ToRadians(Convert.ToSingle(txtObjectPitch.Text));
                float roll = MathUtil.Convert.ToRadians(Convert.ToSingle(txtObjectRoll.Text));
                currentObject.Right = MathUtil.Convert.ToRightVector(yaw, pitch, roll);
                currentObject.Up = MathUtil.Convert.ToUpVector(yaw, pitch, roll);
                currentObject.Forward = MathUtil.Vector3.Cross(currentObject.Up, currentObject.Right);
            }

            // Nodes
#if ENABLE_NODE_EDITOR
            if (currentNode != null)
                saveNodeValues();
#endif

            // "Plugin" values
            savePluginData();
        }

        private bool objectsAreRelated(Reach.GameObject obj1, Reach.GameObject obj2)
        {
            if (obj1 == obj2)
                return true;

            if (obj1.Carrier == obj2 || obj2.Carrier == obj1)
                return true;

            if (obj1.TagGroup == Reach.TagGroup.Bipd)
            {
                Reach.BipedObject biped = (Reach.BipedObject)obj1;
                if (biped.PrimaryWeapon == obj2 || biped.SecondaryWeapon == obj2)
                    return true;
            }

            if (obj2.TagGroup == Reach.TagGroup.Bipd)
            {
                Reach.BipedObject biped = (Reach.BipedObject)obj2;
                if (biped.PrimaryWeapon == obj1 || biped.SecondaryWeapon == obj1)
                    return true;
            }

            return false;
        }

        private void addTagGroupItem(string internalName, string friendlyName)
        {
            TreeViewItem item = new TreeViewItem() { Header = friendlyName, Tag = internalName };
            tVObjects.Items.Add(item);
            tagGroupItems[internalName] = item;
        }

        private TreeViewItem findTagGroupItem(Reach.TagGroup group)
        {
            try
            {
                return tagGroupItems[group.ToString().ToLower()];
            }
            catch
            {
                return tagGroupItems["unknown"];
            }
        }

        private void changePlugin(TabItem tab)
        {
            if (tab != currentPlugin)
            {
                currentPlugin = tab;
                tabs.Items.Remove(tabBiped);
                tabs.Items.Remove(tabWeapon);
                tabs.Items.Remove(tabVehicle);
                if (tab != null)
                    tabs.Items.Insert(1, tab);
            }
        }

        private void tVObjects_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                TreeViewItem SelectedItem = tVObjects.SelectedItem as TreeViewItem;
                if (SelectedItem.Items.Count > 0 && (string)SelectedItem.Tag != "bloc")
                {
                    // Item is a mass-movable tag group
                    btnDelete.Visibility = Visibility.Hidden;
                    btnReplace.Visibility = Visibility.Hidden;
                    btnMassMove.Visibility = Visibility.Visible;

                    currentParentNodeTag = (string)SelectedItem.Tag;
                }
                else if (e.NewValue.ToString().Contains("Header:["))
                {
                    if (currentChunkIndex != -1)
                    {
                        saveValues();
                    }
                    try
                    {
                        //Chunk Load Code
                        currentChunkIndex = int.Parse(SelectedItem.Tag.ToString());
                        string[] parentTag = SelectedItem.Parent.ToString().Split(' ');

                        if (e.NewValue.ToString().Contains("Header:["))
                        {
                            btnMassMove.Visibility = Visibility.Hidden;
                            instructions.Visibility = Visibility.Hidden;
                            tabs.Visibility = Visibility.Visible;

                            // Info values
                            Reach.GameObject currentObject = _saveData.Objects[currentChunkIndex];
                            lblMapIdent.Content = "0x" + currentObject.ID.ToString("X");
                            lblResourceIdent.Content = "0x" + currentObject.MapID.ToString("X");
                            lblFileOffset.Content = "0x" + currentObject.FileOffset.ToString("X");
                            lblAddr.Content = "0x" + currentObject.LoadAddress.ToString("X");

                            txtObjectXCord.Text = currentObject.X.ToString();
                            txtObjectYCord.Text = currentObject.Y.ToString();
                            txtObjectZCord.Text = currentObject.Z.ToString();
                            txtObjectXCord.Tag = txtObjectYCord.Tag = txtObjectZCord.Tag = false;

                            // Rotation values
                            float yaw, pitch, roll;
                            MathUtil.Convert.ToYawPitchRoll(currentObject.Right, currentObject.Forward, currentObject.Up, out yaw, out pitch, out roll);
                            txtObjectYaw.Text = MathUtil.Convert.ToDegrees(yaw).ToString();
                            txtObjectPitch.Text = MathUtil.Convert.ToDegrees(pitch).ToString();
                            txtObjectRoll.Text = MathUtil.Convert.ToDegrees(roll).ToString();
                            txtObjectYaw.Tag = txtObjectPitch.Tag = txtObjectRoll.Tag = false;

                            // Scaling
                            txtObjectScale.Text = currentObject.Scale.ToString();
                            txtObjectScale.Tag = false;

                            // Parent button
                            if (currentObject.Carrier != null)
                            {
                                if (currentObject.TagGroup == Reach.TagGroup.Bipd && currentObject.Carrier.TagGroup == Reach.TagGroup.Vehi)
                                    btnParent.Content = "Vehicle";
                                else
                                    btnParent.Content = "Carrier";
                                lblParentIdent.Text = (string)objectItems[(int)(currentObject.Carrier.ID & 0xFFFF)].Header;
                                carriedBy.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                carriedBy.Visibility = Visibility.Collapsed;
                            }

                            // Children button
                            if (currentObject.FirstCarried != null)
                            {
                                int numChildren = 0;
                                Reach.GameObject obj = currentObject.FirstCarried;
                                while (obj != null)
                                {
                                    numChildren++;
                                    obj = obj.NextCarried;
                                }
                                if (numChildren == 1)
                                {
                                    lblNumChildren.FontSize = 8.0 * (96.0 / 72.0);
                                    lblNumChildren.Padding = new Thickness(5, 6, 5, 5);
                                    lblNumChildren.Text = (string)objectItems[(int)(currentObject.FirstCarried.ID & 0xFFFF)].Header;
                                }
                                else
                                {
                                    lblNumChildren.FontSize = 9.75 * (96.0 / 72.0);
                                    lblNumChildren.Padding = new Thickness(5);
                                    lblNumChildren.Text = numChildren.ToString() + " objects";
                                }

                                carrying.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                carrying.Visibility = Visibility.Collapsed;
                            }

                            // "Plugin" stuff
                            switch (currentObject.TagGroup)
                            {
                                case Reach.TagGroup.Bipd:
                                    objBipd = currentObject as Reach.BipedObject;

                                    txtBipdPlasmaNade.Text = Convert.ToString(objBipd.PlasmaGrenades);
                                    txtBipdFragNade.Text = Convert.ToString(objBipd.FragGrenades);

                                    cBBipdInvici.IsChecked = objBipd.Invincible;

                                    changePlugin(tabBiped);
                                    break;

                                case Reach.TagGroup.Weap:
                                    objWeap = currentObject as Reach.WeaponObject;

                                    txtWeapPluginClipAmmo.Text = Convert.ToString(objWeap.ClipAmmo);
                                    txtWeapPluginAmmo.Text = Convert.ToString(objWeap.Ammo);

                                    //cBWeapInvici.IsChecked = objWeap.Invincible;

                                    changePlugin(tabWeapon);
                                    break;

                                case Reach.TagGroup.Vehi:
                                    objVehi = currentObject as Reach.VehicleObject;

                                    cBVehiInvici.IsChecked = objVehi.Invincible;

                                    changePlugin(tabVehicle);
                                    break;

                                default:
                                    changePlugin(null);
                                    break;
                            }

                            // Weapon list
                            btnViewWeapon.Visibility = Visibility.Collapsed;
                            Reach.WeaponUser weaponUser = currentObject as Reach.WeaponUser;
                            if (weaponUser != null)
                            {
                                tabWeapons.Visibility = Visibility.Visible;
                                listWeapons.Items.Clear();
                                for (int i = 0; i < weaponListItems.Length; i++)
                                {
                                    Reach.WeaponObject weapon = weaponUser.GetWeapon(i);
                                    if (weapon != null)
                                    {
                                        ListBoxItem item = new ListBoxItem();
                                        TreeViewItem tvi = objectItems[(int)(weapon.ID & 0xFFFF)];
                                        item.Content = tvi.Header;
                                        item.FontWeight = tvi.FontWeight;
                                        item.Tag = weapon;
                                        listWeapons.Items.Add(item);
                                        weaponListItems[i] = item;
                                    }
                                    else
                                    {
                                        weaponListItems[i] = null;
                                    }
                                }
                            }
                            else
                            {
                                tabWeapons.Visibility = Visibility.Collapsed;
                                if (tabWeapons.IsSelected)
                                    tabInfo.IsSelected = true;
                            }

#if ENABLE_NODE_EDITOR
                            // Nodes
                            listNodes.Items.Clear();
                            for (int i = 0; i < currentObject.Nodes.Count; i++)
                            {
                                ListBoxItem item = new ListBoxItem();
                                item.Tag = currentObject.Nodes[i];
                                item.Content = "Node " + i;
                                listNodes.Items.Add(item);
                            }
                            listNodes.SelectedIndex = 0;
#endif

                            // Replace button
                            if ((currentObject.TagGroup == Reach.TagGroup.Weap ||
                                currentObject.TagGroup == Reach.TagGroup.Vehi ||
                                currentObject.TagGroup == Reach.TagGroup.Eqip) &&
                                ((TreeViewItem)SelectedItem.Parent).Items.Count > 1)
                            {
                                btnReplace.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                btnReplace.Visibility = Visibility.Hidden;
                            }

                            // Delete button
                            if (currentObject != _saveData.Player.Biped)
                                btnDelete.Visibility = Visibility.Visible;
                            else
                                btnDelete.Visibility = Visibility.Hidden;
                        }
                    }
                    catch { }
                }
                else
                {
                    // The selected TreeViewItem has no children and does not represent an object
                    btnMassMove.Visibility = Visibility.Hidden;
                    btnDelete.Visibility = Visibility.Hidden;
                    btnReplace.Visibility = Visibility.Hidden;
                }
            }
        }

        #region wpf bullshit
        private void btnMassMove_Click(object sender, RoutedEventArgs e)
        {
            Reach.TagGroup tagGroup = classInfo.loadPackageData.convertStringToClass(currentParentNodeTag);
            string tagGroupName = ((string)findTagGroupItem(tagGroup).Header).ToLower();
            Controls.massObjectMove massCoord = new Controls.massObjectMove(_saveData, tagGroup, tagGroupName);
            massCoord.Owner = mainWindow;
            massCoord.ShowDialog();

            if (massCoord.result)
            {
                txtObjectXCord.Text = Convert.ToString(massCoord.moveX);
                txtObjectYCord.Text = Convert.ToString(massCoord.moveY);
                txtObjectZCord.Text = Convert.ToString(massCoord.moveZ);
            }
        }

        private void btnBipdFragMax_Click(object sender, RoutedEventArgs e)
        {
            txtBipdFragNade.Text = "127";
        }

        private void btnBipdPlasmaMax_Click(object sender, RoutedEventArgs e)
        {
            txtBipdPlasmaNade.Text = "127";
        }

        private void btnPrimaryMaxWeaponAmmo_Click(object sender, RoutedEventArgs e)
        {
            txtWeapPluginAmmo.Text = "32767";
        }

        private void btnMaxPrimaryClip_Click(object sender, RoutedEventArgs e)
        {
            txtWeapPluginClipAmmo.Text = "32767";
        }

        private void recursiveDelete(Reach.GameObject obj)
        {
            Reach.GameObject current = obj.FirstCarried;
            while (current != null)
            {
                Reach.GameObject next = current.NextCarried;
                if (current.TagGroup == Reach.TagGroup.Bipd)
                    current.Drop();
                else
                    recursiveDelete(current);
                current = next;
            }

            // Remove the TreeViewItem
            TreeViewItem tvi = objectItems[(int)(obj.ID & 0xFFFF)];
            TreeViewItem parent = (TreeViewItem)tvi.Parent;
            parent.Items.Remove(tvi);

            obj.Delete(false);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!mainWindow.showWarning("Deleting an object may cause the game to freeze or behave unexpectedly. Any objects it is carrying will also be deleted. Continue?", "Object Deletion"))
                return;

            TreeViewItem tvi = objectItems[currentChunkIndex];
            TreeViewItem parent = (TreeViewItem)tvi.Parent;
            int currentPos = parent.Items.IndexOf(tvi);

            recursiveDelete(_saveData.Objects[currentChunkIndex]);

            // Select a nearby item
            if (currentPos == parent.Items.Count)
                currentPos--;
            if (currentPos >= 0)
            {
                ((TreeViewItem)parent.Items.GetItemAt(currentPos)).IsSelected = true;
            }
            else
            {
                currentChunkIndex = -1;
                tabs.Visibility = Visibility.Hidden;
                btnDelete.Visibility = Visibility.Hidden;
                btnReplace.Visibility = Visibility.Hidden;
            }
        }

        private void fixTreeForVehicleReplacement(Reach.GameObject obj)
        {
            Reach.GameObject current = obj.FirstCarried;
            while (current != null)
            {
                if (current.TagGroup != Reach.TagGroup.Bipd)
                    fixTreeForVehicleReplacement(current);
                current = current.NextCarried;
            }

            TreeViewItem tvi = objectItems[(int)(obj.ID & 0xFFFF)];
            TreeViewItem parent = (TreeViewItem)tvi.Parent;
            parent.Items.Remove(tvi);
        }

        private void btnReplace_Click(object sender, RoutedEventArgs e)
        {
            // Set up the list of items
            List<ListBoxItem> listboxItems = new List<ListBoxItem>();
            TreeViewItem tvi = objectItems[currentChunkIndex];
            TreeViewItem parent = (TreeViewItem)tvi.Parent;
            foreach (TreeViewItem item in parent.Items)
            {
                if ((int)item.Tag != currentChunkIndex)
                {
                    ListBoxItem lbItem = new ListBoxItem();
                    lbItem.Content = item.Header;
                    lbItem.FontWeight = item.FontWeight;
                    lbItem.Tag = item;
                    listboxItems.Add(lbItem);
                }
            }

            // Ask the user which object this should be replaced with
            string message = "Select an object to replace \"" + tvi.Header + "\".";
            if (!classInfo.storage.settings.applicationSettings.noWarnings)
                message += "\nWARNING: This may cause the game to freeze!";
            ListBoxItem selectedItem = mainWindow.showListBox(message, "REPLACE OBJECT", listboxItems);

            // Replace!
            if (selectedItem != null)
            {
                TreeViewItem newItem = (TreeViewItem)selectedItem.Tag;
                Reach.GameObject oldObj = _saveData.Objects[(int)tvi.Tag];
                Reach.GameObject newObj = _saveData.Objects[(int)newItem.Tag];
                if (oldObj == _saveData.Player.Biped)
                {
                    _saveData.Player.ChangeBiped(newObj as Reach.BipedObject, true);
                }
                else
                {
                    if (oldObj.TagGroup == Reach.TagGroup.Vehi)
                        fixTreeForVehicleReplacement(oldObj);
                    oldObj.ReplaceWith(newObj, true);
                }

                // Select the new item and bold it if necessary
                newItem.IsSelected = true;
                if (objectsAreRelated(newObj, _saveData.Player.Biped))
                    newItem.FontWeight = FontWeights.Bold;

                parent.Items.Remove(tvi);
            }
        }

        #region btnParent
        private void selectItem(TreeViewItem tvi)
        {
            ((TreeViewItem)tvi.Parent).ExpandSubtree();
            tvi.IsSelected = true;
        }

        private void btnParent_Click(object sender, RoutedEventArgs e)
        {
            Reach.GameObject currentObject = _saveData.Objects[currentChunkIndex];
            selectItem(objectItems[(int)(currentObject.Carrier.ID & 0xFFFF)]);
        }
        #endregion

        private void btnChildren_Click(object sender, RoutedEventArgs e)
        {
            Reach.GameObject currentObject = _saveData.Objects[currentChunkIndex];
            Reach.GameObject obj = currentObject.FirstCarried;

            if (obj != null && obj.NextCarried == null)
            {
                // Only one carried object - just jump to it
                selectItem(objectItems[(int)(obj.ID & 0xFFFF)]);
                return;
            }

            List<ListBoxItem> listboxItems = new List<ListBoxItem>();
            while (obj != null)
            {
                ListBoxItem item = new ListBoxItem();
                int index = (int)(obj.ID & 0xFFFF);
                TreeViewItem tvi = objectItems[index];
                string groupPrefix = ((string)((TreeViewItem)tvi.Parent).Header).TrimEnd('s');
                item.Content = "[" + groupPrefix + "] " + tvi.Header;
                item.FontWeight = tvi.FontWeight;
                item.Tag = tvi;
                listboxItems.Add(item);
                obj = obj.NextCarried;
            }
            ListBoxItem selectedItem = mainWindow.showListBox("Select a carried object to edit:", "EDIT CARRIED OBJECT", listboxItems);

            if (selectedItem != null)
            {
                // This is kinda glitchy, but it works
                TreeViewItem tvi = (TreeViewItem)selectedItem.Tag;
                ((TreeViewItem)tvi.Parent).ExpandSubtree();
                tvi.IsSelected = true;
            }
        }

        private void btnExportParts_Click(object sender, RoutedEventArgs e)
        {
            // Ask the user where to save the CSV file
            SaveFileDialog saveCsv = new SaveFileDialog();
            saveCsv.Title = "Export Object Nodes";
            saveCsv.Filter = "Comma Separated Value Files (*.csv)|*.csv";
            saveCsv.DefaultExt = ".csv";
            Nullable<bool> result = saveCsv.ShowDialog();

            if (result == true)
            {
                Reach.GameObject currentObject = _saveData.Objects[currentChunkIndex];

                // TODO: Wrap this in a class
                try
                {
                    // Open and write the column headers
                    StreamWriter writer = new StreamWriter(saveCsv.FileName);
                    writer.WriteLine("X,Y,Z,Yaw,Pitch,Roll,Scale");

                    foreach (Reach.ModelNode node in currentObject.Nodes)
                    {
                        // Calculate yaw, pitch, and roll
                        float yaw, pitch, roll;
                        MathUtil.Convert.ToYawPitchRoll(node.Right, node.Forward, node.Up, out yaw, out pitch, out roll);

                        // Write the row
                        writer.WriteLine("{0},{1},{2},{3},{4},{5},{6}", node.X, node.Y, node.Z, yaw, pitch, roll, node.Scale);
                    }
                    writer.Close();
                }
                catch
                {
                }
            }
        }

        #region textValidation
        private void ValidateFloat(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null)
                return;

            textBox.Tag = true; // Mark as changed

            if (textBox.Text != "")
            {
                float value;
                if (!float.TryParse(textBox.Text, out value))
                {
                    int line = textBox.Text.Length - 1;
                    textBox.Text = textBox.Text.Remove(line, 1);
                    textBox.Select(line, 0);
                }
            }
            else
            {
                textBox.Text = "0";
            }
        }

        private void ValidateWord(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null)
                return;

            textBox.Tag = true; // Mark as changed

            if (textBox.Text != "")
            {
                int value;
                if (int.TryParse(textBox.Text, out value))
                {
                    if (value > 32767)
                        textBox.Text = "32767";
                }
                else
                {
                    int line = textBox.Text.Length - 1;
                    textBox.Text = textBox.Text.Remove(line, 1);
                    textBox.Select(line, 0);
                }
            }
            else
            {
                textBox.Text = "0";
            }
        }

        private void ValidateByte(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null)
                return;

            textBox.Tag = true; // Mark as changed

            if (textBox.Text != "")
            {
                int value;
                if (int.TryParse(textBox.Text, out value))
                {
                    if (value > 127)
                        textBox.Text = "127";
                }
                else
                {
                    int line = textBox.Text.Length - 1;
                    textBox.Text = textBox.Text.Remove(line, 1);
                    textBox.Select(line, 0);
                }
            }
            else
            {
                textBox.Text = "0";
            }
        }
        #endregion

        private void saveNodeValues()
        {
#if ENABLE_NODE_EDITOR
            currentNode.X = Convert.ToSingle(txtPartXCord.Text);
            currentNode.Y = Convert.ToSingle(txtPartYCord.Text);
            currentNode.Z = Convert.ToSingle(txtPartZCord.Text);
            currentNode.Scale = Convert.ToSingle(txtPartScale.Text);

            // Calculate the right and up vectors based on the YPR values
            float yaw = MathUtil.Convert.ToRadians(Convert.ToSingle(txtPartYaw.Text));
            float pitch = MathUtil.Convert.ToRadians(Convert.ToSingle(txtPartPitch.Text));
            float roll = MathUtil.Convert.ToRadians(Convert.ToSingle(txtPartRoll.Text));
            currentNode.Right = MathUtil.Convert.ToRightVector(yaw, pitch, roll);
            currentNode.Up = MathUtil.Convert.ToUpVector(yaw, pitch, roll);
            
            // Calculate the forward vector by taking the cross product of Up and Right
            currentNode.Forward = MathUtil.Vector3.Cross(currentNode.Up, currentNode.Right);
#endif
        }

        private void listNodes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listNodes.SelectedIndex != -1)
            {
                if (currentNode != null)
                {
                    // Save values
                    saveNodeValues();
                }

                Reach.ModelNode node = (Reach.ModelNode)((ListBoxItem)e.AddedItems[0]).Tag;
                txtPartXCord.Text = node.X.ToString();
                txtPartYCord.Text = node.Y.ToString();
                txtPartZCord.Text = node.Z.ToString();
                txtPartScale.Text = node.Scale.ToString();

                // Calculate the rotation values
                float yaw, pitch, roll;
                MathUtil.Convert.ToYawPitchRoll(node.Right, node.Forward, node.Up, out yaw, out pitch, out roll);
                txtPartYaw.Text = MathUtil.Convert.ToDegrees(yaw).ToString();
                txtPartPitch.Text = MathUtil.Convert.ToDegrees(pitch).ToString();
                txtPartRoll.Text = MathUtil.Convert.ToDegrees(roll).ToString();

                currentNode = node;
            }
        }
        #endregion

        private void listWeapons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listWeapons.SelectedIndex != -1)
                btnViewWeapon.Visibility = Visibility.Visible;
            else
                btnViewWeapon.Visibility = Visibility.Collapsed;
        }

        private void btnViewWeapon_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)listWeapons.SelectedItem;
            Reach.WeaponObject weapon = (Reach.WeaponObject)item.Tag;
            selectItem(objectItems[(int)(weapon.ID & 0xFFFF)]);
        }
    }
}
