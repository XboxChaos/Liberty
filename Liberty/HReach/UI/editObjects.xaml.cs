#define ENABLE_NODE_EDITOR

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
        private Util.SaveManager<Reach.CampaignSave> _saveManager;
        private Reach.TagListManager _taglistManager;
        private Reach.CampaignSave _saveData = null;
        private int currentChunkIndex = -1;
        private string currentParentNodeTag = null;
        Reach.BipedObject objBipd;
        Reach.WeaponObject objWeap;
        Reach.VehicleObject objVehi;
        private List<TreeViewItem> objectItems = new List<TreeViewItem>();
        private Dictionary<string, TreeViewItem> tagGroupItems = new Dictionary<string, TreeViewItem>();
        private Reach.ModelNode currentNode = null;
        BrushConverter bc = new BrushConverter();
        private MainWindow mainWindow = null;

        public editObjects(Util.SaveManager<Reach.CampaignSave> saveManager, Reach.TagListManager taglistManager)
		{
            _saveManager = saveManager;
            _taglistManager = taglistManager;
			InitializeComponent();

            this.Loaded += new RoutedEventHandler(step4_Loaded);

            CommandBinding copyLabel = new CommandBinding(ApplicationCommands.Copy, copyLabel_Executed);
            lblMapIdent.CommandBindings.Add(copyLabel);
            lblResourceIdent.CommandBindings.Add(copyLabel);
            lblFileOffset.CommandBindings.Add(copyLabel);
            lblAddr.CommandBindings.Add(copyLabel);

            cBTeam.Items.Add("Default");
            cBTeam.Items.Add("Player");
            cBTeam.Items.Add("Human");
            cBTeam.Items.Add("Covenant");
            cBTeam.Items.Add("Brute");
            cBTeam.Items.Add("Mule");
            cBTeam.Items.Add("Spare");
            cBTeam.Items.Add("Covenant Player");
		}

        void step4_Loaded(object sender, RoutedEventArgs e)
        {
            // Grab the parent window
            mainWindow = Window.GetWindow(this) as MainWindow;
        }

        void copyLabel_Executed(object target, ExecutedRoutedEventArgs e)
        {
            Label label = (Label)target;
            Clipboard.SetData(DataFormats.Text, label.Content);
        }

        public void Load()
        {
            _saveData = _saveManager.SaveData;

            // Reset selected item
            currentChunkIndex = -1;
            if (tVObjects.SelectedItem != null)
                ((TreeViewItem)tVObjects.SelectedItem).IsSelected = false;
            currentNode = null;

            // Hide buttons
            btnDelete.Visibility = Visibility.Hidden;
            btnReplace.Visibility = Visibility.Hidden;

            // Show the instructions
            instructions.Visibility = Visibility.Visible;
            tabs.Visibility = Visibility.Hidden;

#if ENABLE_NODE_EDITOR
            tabNodes.Visibility = Visibility.Visible;
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
            foreach (Reach.GameObject obj in _saveData.Objects)
            {
                if (obj != null && !obj.Deleted)
                {
                    var items = tVObjects.Items;

                    TreeViewItem tvi = new TreeViewItem();
                    tvi.Name = "tVItem" + i.ToString();

                    tvi.Header = "[" + i.ToString() + "] " + _taglistManager.Identify(obj);
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

        public bool Save()
        {
            if (currentChunkIndex != -1)
                saveValues("");

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
                    //objBipd.MakeInvincible((bool)cBBipdInvici.IsChecked);
                    objBipd.PlasmaGrenades = Convert.ToSByte(txtBipdPlasmaNade.Text);
                    objBipd.FragGrenades = Convert.ToSByte(txtBipdFragNade.Text);
                    if (objBipd.Actor != null)
                    {
                        objBipd.Actor.Blind = (bool)cBBlind.IsChecked;
                        objBipd.Actor.Deaf = (bool)cBDeaf.IsChecked;
                    }
                    break;

                case Reach.TagGroup.Weap:
                    //objWeap.Invincible = (bool)cBWeapInvici.IsChecked;
                    Util.IAmmoDisplay display = (Util.IAmmoDisplay)ammoGrid.Children[0];
                    display.Save();
                    break;

                case Reach.TagGroup.Vehi:
                    if (rbVehiUncontrolled.IsChecked == true)
                        objVehi.Controller = null;
                    else if (rbVehiOwner.IsChecked == true)
                        objVehi.Controller = objVehi.Carrier;
                    objVehi.PlayerCantEnter = !(bool)cBDrivable.IsChecked;
                    break;
            }

            Reach.UnitObject objUnit = obj as Reach.UnitObject;
            if (objUnit != null)
            {
                objUnit.NoFallDamage = (bool)cBNoFallDamage.IsChecked;
                objUnit.Team = (byte)Math.Max(0, cBTeam.SelectedIndex);
            }
        }

        private void saveValues(string tagName)
        {
            //Save those sexy values
            Reach.GameObject currentObject = _saveData.Objects[currentChunkIndex];
            bool changedX = textBoxChanged(txtObjectXCord);
            bool changedY = textBoxChanged(txtObjectYCord);
            bool changedZ = textBoxChanged(txtObjectZCord);
            if (changedX)
                currentObject.X = Convert.ToSingle(txtObjectXCord.Text);
            if (changedY)
                currentObject.Y = Convert.ToSingle(txtObjectYCord.Text);
            if (changedZ)
                currentObject.Z = Convert.ToSingle(txtObjectZCord.Text);

            float velocityX = Convert.ToSingle(txtXVelocity.Text);
            float velocityY = Convert.ToSingle(txtYVelocity.Text);
            float velocityZ = Convert.ToSingle(txtZVelocity.Text);
            currentObject.Velocity = new MathUtil.Vector3(velocityX, velocityY, velocityZ);

            if (textBoxChanged(txtObjectScale))
            {
                currentObject.Scale = Convert.ToSingle(txtObjectScale.Text);

                if (currentObject.Scale >= 1.1 && tagName.ToLower().Contains("kat"))
                eggData.eggData4.enableChecker(mainWindow);
            }

            /*if (textBoxChanged(txtObjectYaw) ||
                textBoxChanged(txtObjectPitch) ||
                textBoxChanged(txtObjectRoll))
            {
                float yaw = MathUtil.Convert.ToRadians(Convert.ToSingle(txtObjectYaw.Text));
                float pitch = MathUtil.Convert.ToRadians(Convert.ToSingle(txtObjectPitch.Text));
                float roll = MathUtil.Convert.ToRadians(Convert.ToSingle(txtObjectRoll.Text));
                currentObject.Right = MathUtil.Convert.ToRightVector(yaw, pitch, roll);
                currentObject.Up = MathUtil.Convert.ToUpVector(yaw, pitch, roll);
                currentObject.Forward = MathUtil.Vector3.Cross(currentObject.Up, currentObject.Right);
            }*/

            saveHealthInfo(currentObject);
            saveCarryInfo(listCarried.SelectedItem);
            
            currentObject.ParentNode = Convert.ToSByte(txtParentNode.Text);

            currentObject.PhysicsEnabled = (bool)cBSimulatePhysics.IsChecked;

            // Nodes
#if ENABLE_NODE_EDITOR
            if (currentNode != null)
                saveNodeValues();
#endif

            // "Plugin" values
            savePluginData();
        }

        private void saveHealthInfo(Reach.GameObject obj)
        {
            try
            {
                if (obj.Health.HasHealth)
                {
                    float newHealth = Convert.ToSingle(txtMaxHealth.Text);
                    if (!float.IsNaN(newHealth))
                        obj.Health.HealthModifier = newHealth;
                }
                if (obj.Health.HasShields)
                {
                    float newShields = Convert.ToSingle(txtMaxShields.Text);
                    if (!float.IsNaN(newShields))
                        obj.Health.ShieldModifier = newShields;
                }
            }
            catch
            {
            }

            if (rbCantDieExceptKillZones.IsChecked == true)
            {
                obj.CannotDie = false;
                obj.CannotDieExceptKillVolumes = true;
            }
            else if (rbCantDie.IsChecked == true)
            {
                obj.CannotDie = true;
                obj.CannotDieExceptKillVolumes = false;
            }
            else
            {
                obj.CannotDie = false;
                obj.CannotDieExceptKillVolumes = false;
            }
            obj.CannotTakeDamage = (cBCantBeDamaged.IsChecked == true);
            obj.IgnoresEMP = (cBIgnoresEMP.IsChecked == true);
            obj.ImmuneToFriendlyFire = (cBIgnoresFriendlyFire.IsChecked == true);
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
                if (biped.PrimaryWeapon == obj2 || biped.SecondaryWeapon == obj2 || biped.TertiaryWeapon == obj2 || biped.QuaternaryWeapon == obj2)
                    return true;
            }

            if (obj2.TagGroup == Reach.TagGroup.Bipd)
            {
                Reach.BipedObject biped = (Reach.BipedObject)obj2;
                if (biped.PrimaryWeapon == obj1 || biped.SecondaryWeapon == obj1 || biped.TertiaryWeapon == obj1 || biped.QuaternaryWeapon == obj1)
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

        private void tVObjects_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                TreeViewItem selectedItem = tVObjects.SelectedItem as TreeViewItem;
                if (selectedItem.Items.Count > 0)
                {
                    // Item is a mass-movable tag group
                    btnDelete.Visibility = Visibility.Hidden;
                    btnReplace.Visibility = Visibility.Hidden;

                    foreach (TabItem itm in tabs.Items)
                    {
                        if (itm == tabMassMove)
                            itm.Visibility = System.Windows.Visibility.Visible;
                        else
                            itm.Visibility = System.Windows.Visibility.Collapsed;
                    }

                    tabs.SelectedItem = tabMassMove;

                    instructions.Visibility = Visibility.Hidden;
                    tabs.Visibility = Visibility.Visible;

                    currentParentNodeTag = (string)selectedItem.Tag;
                    btnMassMoveMover.Content = "Move All " + selectedItem.Header;
                }
                else if (selectedItem.Parent is TreeViewItem)
                {
                    foreach (TabItem tab in tabs.Items)
                    {
                        if (tab == tabMassMove)
                            tab.Visibility = Visibility.Collapsed;
                        else
                            tab.Visibility = Visibility.Visible;
                    }
                    if (tabs.SelectedItem == tabMassMove)
                        tabs.SelectedIndex = 0;

                    if (currentChunkIndex != -1)
                        saveValues(e.OldValue.ToString());

                    try
                    {
                        //Chunk Load Code
                        currentChunkIndex = (int)selectedItem.Tag;

                        tabMassMove.Visibility = Visibility.Collapsed;
                        instructions.Visibility = Visibility.Hidden;
                        tabInfo.Visibility = tabs.Visibility = Visibility.Visible;
                        tabUnit.Visibility = Visibility.Collapsed;
                        tabBiped.Visibility = Visibility.Collapsed;
                        tabWeapon.Visibility = Visibility.Collapsed;
                        tabVehicle.Visibility = Visibility.Collapsed;

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

                        txtXVelocity.Text = currentObject.Velocity.X.ToString();
                        txtYVelocity.Text = currentObject.Velocity.Y.ToString();
                        txtZVelocity.Text = currentObject.Velocity.Z.ToString();
                        txtXVelocity.Tag = txtYVelocity.Tag = txtZVelocity.Tag = false;

                        // Rotation values
                        // REMOVED BY AMD - These are kinda pointless tbh, since we don't know how to edit them...
                        /*float yaw, pitch, roll;
                        MathUtil.Convert.ToYawPitchRoll(currentObject.Right, currentObject.Forward, currentObject.Up, out yaw, out pitch, out roll);
                        txtObjectYaw.Text = MathUtil.Convert.ToDegrees(yaw).ToString();
                        txtObjectPitch.Text = MathUtil.Convert.ToDegrees(pitch).ToString();
                        txtObjectRoll.Text = MathUtil.Convert.ToDegrees(roll).ToString();
                        txtObjectYaw.Tag = txtObjectPitch.Tag = txtObjectRoll.Tag = false;*/

                        // Scaling
                        txtObjectScale.Text = currentObject.Scale.ToString();
                        txtObjectScale.Tag = false;

                        // Parent button
                        Reach.GameObject parent = getLogicalParent(currentObject);
                        if (parent != null)
                        {
                            if (currentObject.TagGroup == Reach.TagGroup.Bipd && parent.TagGroup == Reach.TagGroup.Vehi)
                                btnParent.Content = "Vehicle";
                            else
                                btnParent.Content = "Carrier";
                            btnParent.Tag = parent;
                            lblParentIdent.Text = (string)objectItems[(int)(parent.ID & 0xFFFF)].Header;
                            carriedBy.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            carriedBy.Visibility = Visibility.Collapsed;
                        }

                        rebuildCarryListbox(currentObject);

                        // Max health/shields
                        gridHealthControls.IsEnabled = currentObject.Health.HasHealth || currentObject.Health.HasShields;
                        txtMaxHealth.Text = currentObject.Health.HealthModifier.ToString();
                        txtMaxHealth.IsEnabled = currentObject.Health.HasHealth && !currentObject.Health.IsInfinite;
                        txtMaxShields.Text = currentObject.Health.ShieldModifier.ToString();
                        txtMaxShields.IsEnabled = currentObject.Health.HasShields && !currentObject.Health.IsInfinite;
                        cBIgnoresEMP.IsChecked = currentObject.IgnoresEMP;
                        cBIgnoresFriendlyFire.IsChecked = currentObject.ImmuneToFriendlyFire;
                        cBCantBeDamaged.IsChecked = currentObject.CannotTakeDamage;

                        if (currentObject.CannotDieExceptKillVolumes)
                            rbCantDieExceptKillZones.IsChecked = true;
                        else if (currentObject.CannotDie)
                            rbCantDie.IsChecked = true;
                        else
                            rbKillable.IsChecked = true;

                        // HAX: This has to be done last because it forces a save of the health info
                        cBInvincible.IsChecked = currentObject.Health.IsInfinite;

                        // Physics
                        cBSimulatePhysics.IsChecked = currentObject.PhysicsEnabled;

                        // Tag group-specific tabs
                        switch (currentObject.TagGroup)
                        {
                            case Reach.TagGroup.Bipd:
                                objBipd = currentObject as Reach.BipedObject;

                                txtBipdPlasmaNade.Text = Convert.ToString(objBipd.PlasmaGrenades);
                                txtBipdFragNade.Text = Convert.ToString(objBipd.FragGrenades);

                                tabUnit.Visibility = Visibility.Visible;
                                tabBiped.Visibility = Visibility.Visible;

                                if (objBipd.Actor != null)
                                {
                                    gridAi.IsEnabled = true;
                                    cBBlind.IsChecked = objBipd.Actor.Blind;
                                    cBDeaf.IsChecked = objBipd.Actor.Deaf;
                                }
                                else
                                {
                                    gridAi.IsEnabled = false;
                                    cBBlind.IsChecked = false;
                                    cBDeaf.IsChecked = false;
                                }
                                break;

                            case Reach.TagGroup.Weap:
                                objWeap = currentObject as Reach.WeaponObject;
                                string name = selectedItem.Header.ToString();
                                name = name.Substring(name.IndexOf(']') + 2);

                                ammoGrid.Children.Clear();
                                ammoGrid.Children.Add(Reach.WeaponEditing.GetAmmoDisplay(_saveData, objWeap, name));

                                //cBWeapInvici.IsChecked = objWeap.Invincible;

                                tabWeapon.Visibility = Visibility.Visible;
                                break;

                            case Reach.TagGroup.Vehi:
                                objVehi = currentObject as Reach.VehicleObject;

                                //cBVehiInvici.IsChecked = objVehi.Invincible;
                                updateVehiControllerInfo();

                                tabUnit.Visibility = Visibility.Visible;
                                tabVehicle.Visibility = Visibility.Visible;
                                cBDrivable.IsChecked = !objVehi.PlayerCantEnter;
                                break;
                        }

                        Reach.UnitObject objUnit = currentObject as Reach.UnitObject;
                        if (objUnit != null)
                        {
                            cBNoFallDamage.IsChecked = objUnit.NoFallDamage;
                            cBTeam.SelectedIndex = objUnit.Team;
                        }

                        // If the selected tab got removed, show the info page
                        if (tabs.SelectedItem == null || ((TabItem)tabs.SelectedItem).Visibility != Visibility.Visible)
                            tabs.SelectedItem = tabInfo;

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
                            ((TreeViewItem)selectedItem.Parent).Items.Count > 1)
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
                    catch (Exception ex)
                    {
                        mainWindow.showException(ex.ToString(), true);
                    }
                }
                else
                {
                    // The selected TreeViewItem has no children and does not represent an object
                    btnDelete.Visibility = Visibility.Hidden;
                    btnReplace.Visibility = Visibility.Hidden;
                    tabs.Visibility = Visibility.Hidden;
                }
            }
        }

        #region wpf bullshit
        private void btnBipdFragMax_Click(object sender, RoutedEventArgs e)
        {
            txtBipdFragNade.Text = "127";
        }

        private void btnBipdPlasmaMax_Click(object sender, RoutedEventArgs e)
        {
            txtBipdPlasmaNade.Text = "127";
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
            if (tvi != null)
            {
                TreeViewItem parent = (TreeViewItem)tvi.Parent;
                parent.Items.Remove(tvi);
            }

            obj.Delete(false);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!mainWindow.showWarning("Deleting an object may cause the game to freeze or behave unexpectedly. Any objects it is carrying will also be deleted. Continue?", "Object Deletion"))
                return;

            TreeViewItem tvi = objectItems[currentChunkIndex];
            if (tvi == null)
                return;
            TreeViewItem parent = (TreeViewItem)tvi.Parent;
            int currentPos = parent.Items.IndexOf(tvi);

            recursiveDelete(_saveData.Objects[currentChunkIndex]);

            // Select a nearby item
            if (currentPos >= parent.Items.Count)
                currentPos = parent.Items.Count - 1;
            if (currentPos >= 0)
            {
                ((TreeViewItem)parent.Items.GetItemAt(currentPos)).IsSelected = true;
            }
            else
            {
                currentChunkIndex = -1;
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
            if (tvi != null)
            {
                TreeViewItem parent = (TreeViewItem)tvi.Parent;
                parent.Items.Remove(tvi);
            }
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
                    if (item.Header != null)
                    {
                        lbItem.Content = item.Header;
                        lbItem.FontWeight = item.FontWeight;
                        lbItem.Tag = item;
                        listboxItems.Add(lbItem);
                    }
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
                    oldObj.ReplaceWith(newObj);
                    oldObj.Delete(true);
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
            tabs.SelectedIndex = 0;
        }

        private void btnParent_Click(object sender, RoutedEventArgs e)
        {
            Reach.GameObject parent = (Reach.GameObject)btnParent.Tag;
            selectItem(objectItems[(int)(parent.ID & 0xFFFF)]);
        }
        #endregion

        private void btnChildren_Click(object sender, RoutedEventArgs e)
        {
            if (listCarried.Items.Count == 1)
            {
                ListBoxItem firstItem = (ListBoxItem)listCarried.Items[0];
                Reach.GameObject obj = (Reach.GameObject)firstItem.Tag;
                TreeViewItem tvi = objectItems[(int)(obj.ID & 0xFFFF)];
                selectItem(tvi);
            }
            else
            {
                tabs.SelectedItem = tabCarrying;
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

        private void cBInvincible_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            Reach.GameObject currentObject = _saveData.Objects[currentChunkIndex];
            saveHealthInfo(currentObject);
            currentObject.Health.MakeInfinite(true);
            txtMaxHealth.IsEnabled = false;
            txtMaxHealth.Text = currentObject.Health.HealthModifier.ToString();
            txtMaxShields.IsEnabled = false;
            txtMaxShields.Text = currentObject.Health.ShieldModifier.ToString();
        }

        private void cBInvincible_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            Reach.GameObject currentObject = _saveData.Objects[currentChunkIndex];
            currentObject.Health.MakeInfinite(false);
            txtMaxHealth.IsEnabled = currentObject.Health.HasHealth;
            txtMaxHealth.Text = currentObject.Health.HealthModifier.ToString();
            txtMaxShields.IsEnabled = currentObject.Health.HasShields;
            txtMaxShields.Text = currentObject.Health.ShieldModifier.ToString();
        }

        #region MassObjectMove
        private void btnMassMoveMover_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem parent = (TreeViewItem)tVObjects.SelectedItem;
            int i=0;
            IList<Reach.GameObject> listObj = new List<Reach.GameObject>();
            Reach.TagGroup tag = (Reach.TagGroup)Enum.Parse(typeof(Reach.TagGroup), parent.Tag.ToString(), true);

            foreach (Reach.GameObject obj in _saveData.Objects)
                if (obj != null && obj.TagGroup == tag && !obj.Deleted)
                    i++;

            int indexBK = i;
            double spread = sliderMassMove.Value;

            double gridSize = Math.Ceiling(Math.Sqrt(i));

            while (gridSize * gridSize < i)
                    gridSize++;

            // Cols = derp
            // Rows = derp
            // Spread = slider.value

            // Calculate the start point.
            float pX = float.Parse(txtMassMoveX.Text);
            float pY = float.Parse(txtMassMoveY.Text);
            float pZ = float.Parse(txtMassMoveZ.Text);

            float StartVert = pX + Convert.ToSingle((gridSize / 2) * spread);
            float StartHor = pY + Convert.ToSingle((gridSize / 2) * spread);
            float CurrentVert = StartVert;
            float CurrentHor = StartHor;

            int k = 0;
            for (double z = 0; z < gridSize; z++)
            {
                CurrentVert = StartVert;

                for (double j = 0; j < gridSize; j++)
                {
                    bool isDone = true;
                    while (isDone)
                    {
                        if (_saveData.Objects.Count < k)
                            isDone = false;

                        if (_saveData.Objects.Count > k && _saveData.Objects[k] != null && !_saveData.Objects[k].Deleted && _saveData.Objects[k].TagGroup == tag)
                        {
                            isDone = false;

                            _saveData.Objects[k].X = CurrentVert;
                            _saveData.Objects[k].Y = CurrentHor;
                            _saveData.Objects[k].Z = pZ;
                            _saveData.Objects[k].IsAwake = true;

                            CurrentVert += Convert.ToSingle(spread);
                        }
                        k++;
                    }
                }
                CurrentHor += Convert.ToSingle(spread);
            }

            string friendlyTagName = (string)parent.Header;
            mainWindow.showMessage("All " + friendlyTagName.ToLower() + (friendlyTagName.EndsWith("s") ? " were" : " was") + " moved successfully.", "MASS MOVE");
        }

        private void btnMassMoveCordSetter_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem parent = (TreeViewItem)tVObjects.SelectedItem;
            Reach.TagGroup tag = (Reach.TagGroup) Enum.Parse(typeof(Reach.TagGroup), parent.Tag.ToString(), true);

            Liberty.Controls.listcordWindow listCord = mainWindow.showListCordWindow(_saveData, tag, _taglistManager);

            Reach.GameObject obj = listCord.HReachObject;

            if (obj != null && !float.IsNaN(obj.X))
            {
                txtMassMoveX.Text = obj.X.ToString();
                txtMassMoveY.Text = obj.Y.ToString();
                txtMassMoveZ.Text = obj.Z.ToString();
            }
        }

        private void sliderMassMove_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string sliderDetail = "spacing type: {0} - {1}";

            if (e.NewValue == 0)
                sliderDetail = string.Format(sliderDetail, 0, "none");
            else if (e.NewValue <= 1)
                sliderDetail = string.Format(sliderDetail, Math.Round(e.NewValue, 1).ToString(), "compact");
            else if (e.NewValue > 1 && e.NewValue <= 3)
                sliderDetail = string.Format(sliderDetail, Math.Round(e.NewValue, 1).ToString(), "fair");
            else if (e.NewValue > 3 && e.NewValue <= 5)
                sliderDetail = string.Format(sliderDetail, Math.Round(e.NewValue, 1).ToString(), "medium");
            else if (e.NewValue > 5 && e.NewValue <= 8)
                sliderDetail = string.Format(sliderDetail, Math.Round(e.NewValue, 1).ToString(), "expanded");
            else if (e.NewValue > 8 && e.NewValue <= 10)
                sliderDetail = string.Format(sliderDetail, Math.Round(e.NewValue, 1).ToString(), "very stretched (only recommended for vehicles/obstacles)");

            lblMassMoveSliderDetail.Text = sliderDetail;
        }
        #endregion

        #region Carry List
        private void addObjectToCarryList(Reach.GameObject obj)
        {
            ListBoxItem item = new ListBoxItem();
            TreeViewItem objTvi = objectItems[(int)(obj.ID & 0xFFFF)];
            TreeViewItem tviParent = (TreeViewItem)objTvi.Parent;
            string groupName = (string)tviParent.Header;
            if (groupName.EndsWith("s"))
                groupName = groupName.Substring(0, groupName.Length - 1);
            item.Content = "[" + groupName + "] " + objTvi.Header;
            item.Tag = obj;
            listCarried.Items.Add(item);
        }

        private void refreshChildrenButton()
        {
            int numChildren = listCarried.Items.Count;
            if (numChildren > 0)
            {
                if (numChildren == 1)
                {
                    lblNumChildren.FontSize = 8.0 * (96.0 / 72.0);
                    lblNumChildren.Padding = new Thickness(5, 6, 5, 5);

                    ListBoxItem firstItem = (ListBoxItem)listCarried.Items[0];
                    Reach.GameObject obj = (Reach.GameObject)firstItem.Tag;
                    TreeViewItem tvi = objectItems[(int)(obj.ID & 0xFFFF)];
                    lblNumChildren.Text = (string)tvi.Header;
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
        }

        private void saveCarryInfo(object item)
        {
            if (item != null)
            {
                ListBoxItem lbItem = (ListBoxItem)item;
                Reach.GameObject obj = (Reach.GameObject)lbItem.Tag;
                obj.ParentNode = Convert.ToSByte(txtParentNode.Text);
            }
        }

        private void refreshCarryButtons()
        {
            if (listCarried.SelectedIndex != -1)
            {
                btnDropObject.Visibility = Visibility.Visible;
                btnViewCarriedObject.Visibility = Visibility.Visible;

                gridAttachSettings.Visibility = Visibility.Visible;
                ListBoxItem item = (ListBoxItem)listCarried.SelectedItem;
                Reach.GameObject obj = (Reach.GameObject)item.Tag;
                txtParentNode.Text = obj.ParentNode.ToString();
            }
            else
            {
                btnDropObject.Visibility = Visibility.Collapsed;
                btnViewCarriedObject.Visibility = Visibility.Collapsed;
                gridAttachSettings.Visibility = Visibility.Hidden;
            }
        }

        private void rebuildCarryListbox(Reach.GameObject obj)
        {
            listCarried.Items.Clear();

            // If the object is a UnitObject, add its weapons to the list
            Reach.UnitObject weaponUser = obj as Reach.UnitObject;
            if (weaponUser != null)
            {
                foreach (Reach.WeaponObject weapon in weaponUser.Weapons)
                    addObjectToCarryList(weapon);
            }

            Reach.GameObject currentObj = obj.FirstCarried;
            while (currentObj != null)
            {
                // Only add the object if it was not added in the weapon pass earlier
                if (weaponUser == null || (currentObj != weaponUser.PrimaryWeapon && currentObj != weaponUser.SecondaryWeapon && currentObj != weaponUser.TertiaryWeapon && currentObj != weaponUser.QuaternaryWeapon))
                    addObjectToCarryList(currentObj);
                currentObj = currentObj.NextCarried;
            }
            refreshCarryButtons();
            refreshChildrenButton();
        }

        private TreeViewItem cloneTreeViewItem(TreeViewItem item, HashSet<Reach.GameObject> skip)
        {
            TreeViewItem newItem = new TreeViewItem();
            newItem.Header = item.Header;
            newItem.FontWeight = item.FontWeight;
            newItem.Tag = item.Tag;
            foreach (TreeViewItem child in item.Items)
            {
                if (!(child.Tag is int && skip.Contains(_saveData.Objects[(int)child.Tag])))
                    newItem.Items.Add(cloneTreeViewItem(child, skip));
            }
            return newItem;
        }

        private List<TreeViewItem> cloneObjectTree(HashSet<Reach.GameObject> skip)
        {
            List<TreeViewItem> items = new List<TreeViewItem>();
            foreach (TreeViewItem item in tVObjects.Items)
            {
                if (!(item.Tag is int && skip.Contains(_saveData.Objects[(int)item.Tag])))
                    items.Add(cloneTreeViewItem(item, skip));
            }
            return items;
        }

        private Reach.GameObject getLogicalParent(Reach.GameObject obj)
        {
            Reach.GameObject parent = obj.Carrier;
            if (parent == null && obj is Reach.WeaponObject)
            {
                Reach.WeaponObject weapon = (Reach.WeaponObject)obj;
                parent = weapon.User;
            }
            return parent;
        }

        private Reach.GameObject showUnrelatedObjectTree(Reach.GameObject baseObject, string description, string title)
        {
            // Build a set of objects to hide from the tree dialog by scanning the carry and weapon lists
            HashSet<Reach.GameObject> skip = new HashSet<Reach.GameObject>();
            skip.Add(baseObject);

            // Ignore parent objects
            Reach.GameObject parent = getLogicalParent(baseObject);
            while (parent != null)
            {
                skip.Add(parent);
                parent = getLogicalParent(parent);
            }

            // Ignore child objects
            Reach.GameObject carried = baseObject.FirstCarried;
            while (carried != null)
            {
                skip.Add(carried);
                carried = carried.NextCarried;
            }

            // Ignore any held weapons
            Reach.UnitObject weaponUser = baseObject as Reach.UnitObject;
            if (weaponUser != null)
            {
                foreach (Reach.WeaponObject weapon in weaponUser.Weapons)
                    skip.Add(weapon);
            }

            TreeViewItem selectedItem = mainWindow.showObjectTree(description, title, cloneObjectTree(skip));
            if (selectedItem == null)
                return null;
            return _saveData.Objects[(int)selectedItem.Tag];
        }

        private void btnPickUpObject_Click(object sender, RoutedEventArgs e)
        {
            Reach.GameObject currentObject = _saveData.Objects[currentChunkIndex];
            Reach.GameObject pickUp = showUnrelatedObjectTree(currentObject, "Select an object to pick up:", "PICK UP OBJECT");

            if (pickUp != null)
            {
                currentObject.PickUp(pickUp);
                pickUp.ParentNode = 0;

                // If this is a weapon and a unit is picking it up, add it to the unit's weapon list
                if (currentObject is Reach.UnitObject && pickUp is Reach.WeaponObject)
                {
                    Reach.UnitObject user = (Reach.UnitObject)currentObject;
                    Reach.WeaponObject weapon = (Reach.WeaponObject)pickUp;
                    if (!user.PickUpWeapon(weapon))
                        mainWindow.showMessage("The object is already carrying four weapons. It will still be picked up, but it will not be usable.", "WEAPON PICK UP");
                }

                // Bold the object's TreeViewItem if it's related to the player now
                if (objectsAreRelated(_saveData.Player.Biped, pickUp))
                    objectItems[(int)(pickUp.ID & 0xFFFF)].FontWeight = FontWeights.Bold;

                rebuildCarryListbox(currentObject);
            }
        }

        private void btnDropObject_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = listCarried.SelectedItem as ListBoxItem;
            if (item != null)
            {
                Reach.GameObject selectedObject = (Reach.GameObject)item.Tag;
                selectedObject.Drop();

                // Remove the list box item and keep something selected
                int oldIndex = listCarried.SelectedIndex;
                listCarried.Items.RemoveAt(listCarried.SelectedIndex);
                if (oldIndex >= listCarried.Items.Count)
                    oldIndex = listCarried.Items.Count - 1;
                listCarried.SelectedIndex = oldIndex;

                // If the item is bolded, then unbold it in the TreeView
                if (item.FontWeight == FontWeights.Bold)
                {
                    TreeViewItem tvi = objectItems[(int)(selectedObject.ID & 0xFFFF)];
                    tvi.FontWeight = FontWeights.Normal;
                }
            }
        }

        private void btnViewCarriedObject_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = listCarried.SelectedItem as ListBoxItem;
            if (item != null)
            {
                Reach.GameObject selectedObject = (Reach.GameObject)item.Tag;
                TreeViewItem tvi = objectItems[(int)(selectedObject.ID & 0xFFFF)];
                selectItem(tvi);
            }
        }

        private void listCarried_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Make sure the mouse is over the selected item,
            // and if it is then just forward to the View button code
            ListBoxItem item = listCarried.SelectedItem as ListBoxItem;
            if (item != null && item.IsMouseOver)
                btnViewCarriedObject_Click(sender, e);
        }

        private void listCarried_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
                saveCarryInfo(e.RemovedItems[0]);
            refreshCarryButtons();
        }

        private void updateVehiControllerInfo()
        {
            Reach.VehicleObject vehicle = _saveData.Objects[currentChunkIndex] as Reach.VehicleObject;
            if (vehicle == null)
                return;

            lblVehiController.Text = "(nothing is selected)";
            rbVehiOther.IsEnabled = false;
            if (vehicle.Controller == null)
            {
                rbVehiUncontrolled.IsChecked = true;
            }
            else if (vehicle.Controller.TagGroup == Reach.TagGroup.Vehi && vehicle.Carrier == vehicle.Controller)
            {
                rbVehiOwner.IsChecked = true;
            }
            else
            {
                rbVehiOther.IsEnabled = true;
                rbVehiOther.IsChecked = true;
                lblVehiController.Text = (string)objectItems[(int)(vehicle.Controller.ID & 0xFFFF)].Header;
            }

            rbVehiOwner.IsEnabled = (vehicle.Carrier != null && vehicle.Carrier.TagGroup == Reach.TagGroup.Vehi);
        }

        private void btnChangeController_Click(object sender, RoutedEventArgs e)
        {
            Reach.VehicleObject vehicle = _saveData.Objects[currentChunkIndex] as Reach.VehicleObject;
            if (vehicle == null)
                return;

            HashSet<Reach.GameObject> skip = new HashSet<Reach.GameObject>();
            skip.Add(vehicle);
            TreeViewItem selectedItem = mainWindow.showObjectTree("Select the object which should control this vehicle:", "CHANGE VEHICLE CONTROLLER", cloneObjectTree(skip));
            if (selectedItem != null)
            {
                Reach.GameObject obj = _saveData.Objects[(int)selectedItem.Tag];
                vehicle.Controller = obj;
                updateVehiControllerInfo();
            }
        }
        #endregion

        private void btnZeroVelocity_Click(object sender, RoutedEventArgs e)
        {
            txtXVelocity.Text = "0";
            txtYVelocity.Text = "0";
            txtZVelocity.Text = "0";
        }

        private void btnQuickMove_Click(object sender, RoutedEventArgs e)
        {
            Reach.GameObject currentObject = _saveData.Objects[currentChunkIndex];
            Reach.GameObject target = showUnrelatedObjectTree(currentObject, "Select an object to move this to:", "QUICK MOVE");
            if (target != null)
            {
                currentObject.X = target.X;
                currentObject.Y = target.Y;
                currentObject.Z = target.Z;
                txtObjectXCord.Text = currentObject.X.ToString();
                txtObjectYCord.Text = currentObject.Y.ToString();
                txtObjectZCord.Text = currentObject.Z.ToString();
            }
        }
    }
}
