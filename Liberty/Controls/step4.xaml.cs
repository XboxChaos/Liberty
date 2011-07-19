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
using System.Reflection;

namespace Liberty.Controls
{
	/// <summary>
	/// Interaction logic for step4.xaml
	/// </summary>
	public partial class step4 : UserControl
	{
        private int currentChunkIndex = -1;
        private string currentParentNodeTag = null;
        public event EventHandler ExecuteMethod;
        public event EventHandler ExecuteMethod2;
        public event EventHandler ExecuteMethod3;
        public int currentPlugin = -1;
        Reach.BipedObject objBipd;
        Reach.WeaponObject objWeap;
        Reach.VehicleObject objVehi;
        private List<TreeViewItem> objectItems = new List<TreeViewItem>();

		public step4()
		{
			InitializeComponent();

            lblOpen.Visibility = System.Windows.Visibility.Hidden;
            btnOpen.Visibility = System.Windows.Visibility.Hidden;
		}

        protected virtual void OnExecuteMethod() { if (ExecuteMethod != null) ExecuteMethod(this, EventArgs.Empty); }
        protected virtual void OnExecuteMethod2() { if (ExecuteMethod2 != null) ExecuteMethod2(this, EventArgs.Empty); }
        protected virtual void OnExecuteMethod3() { if (ExecuteMethod3 != null) ExecuteMethod3(this, EventArgs.Empty); }

        public void loadData()
        {
            currentChunkIndex = -1;
            objectInfo.Visibility = System.Windows.Visibility.Hidden;
            btnDelete.Visibility = System.Windows.Visibility.Hidden;
            lblDelete.Visibility = System.Windows.Visibility.Hidden;
            btnReplace.Visibility = System.Windows.Visibility.Hidden;
            lblReplace.Visibility = System.Windows.Visibility.Hidden;
            foreach (TreeViewItem item in tVObjects.Items)
            {
                item.Items.Clear();
            }
            objectItems.Clear();
            int i = 0;
            foreach (Reach.GameObject obj in classInfo.storage.fileInfoStorage.saveData.Objects)
            {
                if (obj != null && !obj.Deleted)
                {
                    var items = tVObjects.Items;

                    TreeViewItem tvi = new TreeViewItem();
                    tvi.Name = "tVItem" + i.ToString();
                    tvi.Header = "[" + i.ToString() + "] " + classInfo.nameLookup.translate(obj.ResourceID);
                    tvi.Tag = i;

                    (items[convertClassToNode(obj.TagGroup)] as TreeViewItem).Items.Add(tvi);
                    objectItems.Add(tvi);
                }
                else
                {
                    objectItems.Add(null);
                }

                i++;
            }
        }

        public void saveData()
        {
            if (currentChunkIndex == -1) { }
            else
            {
                //Save those sexy values
                classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex].X = Convert.ToSingle(txtObjectXCord.Text);
                classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex].Y = Convert.ToSingle(txtObjectYCord.Text);
                classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex].Z = Convert.ToSingle(txtObjectZCord.Text);

                switch (currentPlugin)
                {
                    case 0:
                        objBipd.Invincible = (bool)cBBipdInvici.IsChecked;
                        objBipd.PlasmaGrenades = Convert.ToSByte(txtBipdPlasmaNade.Text);
                        objBipd.FragGrenades = Convert.ToSByte(txtBipdFragNade.Text);

                        classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex] = objBipd;
                        break;

                    case 1:
                        objWeap.Invincible = (bool)cBWeapInvici.IsChecked;
                        objWeap.Ammo = Convert.ToInt16(txtWeapPluginAmmo.Text);
                        objWeap.ClipAmmo = Convert.ToInt16(txtWeapPluginClipAmmo.Text);

                        classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex] = objWeap;
                        break;

                    case 2:
                        objVehi.Invincible = (bool)cBVehiInvici.IsChecked;
                        classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex] = objVehi;
                        break;
                }
            }
        }

        public static int convertClassToNode(Reach.TagGroup type)
        {
            switch (type)
            {
                case Reach.TagGroup.Bipd:
                    return 0;
                case Reach.TagGroup.Bloc:
                    return 1;
                case Reach.TagGroup.Crea:
                    return 2;
                case Reach.TagGroup.Ctrl:
                    return 3;
                case Reach.TagGroup.Efsc:
                    return 4;
                case Reach.TagGroup.Eqip:
                    return 5;
                case Reach.TagGroup.Mach:
                    return 6;
                case Reach.TagGroup.Proj:
                    return 7;
                case Reach.TagGroup.Scen:
                    return 8;
                case Reach.TagGroup.Ssce:
                    return 9;
                case Reach.TagGroup.Term:
                    return 10;
                case Reach.TagGroup.Vehi:
                    return 11;
                case Reach.TagGroup.Weap:
                    return 12;
            }
            return 13;
        }

        #region textValidation
        private void txtObjectXCord_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txtObjectXCord.Text == "") { }
            else
            {
                try
                {
                    float validate = float.Parse(txtObjectXCord.Text);
                }
                catch
                {
                    int line = txtObjectXCord.Text.Length - 1;
                    txtObjectXCord.Text = txtObjectXCord.Text.Remove(line, 1);
                    txtObjectXCord.Select(line, 0);
                }
            }

            if (txtObjectXCord.Text == "") { txtObjectXCord.Text = "0"; }
        }

        private void txtObjectYCord_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txtObjectYCord.Text == "") { }
            else
            {
                try
                {
                    float validate = float.Parse(txtObjectYCord.Text);
                }
                catch
                {
                    int line = txtObjectYCord.Text.Length - 1;
                    txtObjectYCord.Text = txtObjectYCord.Text.Remove(line, 1);
                    txtObjectYCord.Select(line, 0);
                }
            }

            if (txtObjectYCord.Text == "") { txtObjectYCord.Text = "0"; }
        }

        private void txtObjectZCord_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txtObjectZCord.Text == "") { }
            else
            {
                try
                {
                    float validate = float.Parse(txtObjectZCord.Text);
                }
                catch
                {
                    int line = txtObjectZCord.Text.Length - 1;
                    txtObjectZCord.Text = txtObjectZCord.Text.Remove(line, 1);
                    txtObjectZCord.Select(line, 0);
                }
            }

            if (txtObjectZCord.Text == "") { txtObjectZCord.Text = "0"; }
        }

        private void tVObjects_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                if (e.NewValue.ToString().Contains("Header:["))
                {
                    if (currentChunkIndex == -1) { }
                    else
                    {
                        //Save those sexy values
                        classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex].X = Convert.ToSingle(txtObjectXCord.Text);
                        classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex].Y = Convert.ToSingle(txtObjectYCord.Text);
                        classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex].Z = Convert.ToSingle(txtObjectZCord.Text);

                        switch (currentPlugin)
                        {
                            case 0:
                                objBipd.Invincible = (bool)cBBipdInvici.IsChecked;
                                objBipd.PlasmaGrenades = Convert.ToSByte(txtBipdPlasmaNade.Text);
                                objBipd.FragGrenades = Convert.ToSByte(txtBipdFragNade.Text);

                                classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex] = objBipd;
                                break;

                            case 1:
                                objWeap.Invincible = (bool)cBWeapInvici.IsChecked;
                                objWeap.Ammo = Convert.ToInt16(txtWeapPluginAmmo.Text);
                                objWeap.ClipAmmo = Convert.ToInt16(txtWeapPluginClipAmmo.Text);

                                classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex] = objWeap;
                                break;

                            case 2:
                                objVehi.Invincible = (bool)cBVehiInvici.IsChecked;
                                classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex] = objVehi;
                                break;
                        }
                    }
                    try
                    {
                        //Chunk Load Code

                        TreeViewItem SelectedItem = tVObjects.SelectedItem as TreeViewItem;
                        currentChunkIndex = int.Parse(SelectedItem.Tag.ToString());
                        string[] parentTag = SelectedItem.Parent.ToString().Split(' ');
                        currentParentNodeTag = parentTag[1].Replace("Header:", "");


                        if (e.NewValue.ToString().Contains("Header:["))
                        {
                            Reach.GameObject currentObject = classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex];
                            txtObjectXCord.Text = currentObject.X.ToString();
                            txtObjectYCord.Text = currentObject.Y.ToString();
                            txtObjectZCord.Text = currentObject.Z.ToString();

                            lblMapIdent.Content = "0x" + currentObject.ID.ToString("X");
                            lblResourceIdent.Content = "0x" + currentObject.ResourceID.ToString("X");
                            lblFileOffset.Content = "0x" + currentObject.FileOffset.ToString("X");

                            // Parent button
                            if (currentObject.Carrier != null)
                            {
                                lblParentIdent.Content = "0x" + currentObject.Carrier.ID.ToString("X");
                                lblParentIdent.Visibility = System.Windows.Visibility.Visible;
                                btnParent.Visibility = System.Windows.Visibility.Visible;
                                lblParent.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                lblParentIdent.Visibility = System.Windows.Visibility.Hidden;
                                btnParent.Visibility = System.Windows.Visibility.Hidden;
                                lblParent.Visibility = System.Windows.Visibility.Hidden;
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
                                    lblNumChildren.Content = "1 child";
                                else
                                    lblNumChildren.Content = numChildren.ToString() + " children";

                                lblNumChildren.Visibility = System.Windows.Visibility.Visible;
                                btnChildren.Visibility = System.Windows.Visibility.Visible;
                                lblChildren.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                lblNumChildren.Visibility = System.Windows.Visibility.Hidden;
                                btnChildren.Visibility = System.Windows.Visibility.Hidden;
                                lblChildren.Visibility = System.Windows.Visibility.Hidden;
                            }

                            weapPlugin.Visibility = System.Windows.Visibility.Hidden;
                            bipdPlugin.Visibility = System.Windows.Visibility.Hidden;
                            vehiPlugin.Visibility = System.Windows.Visibility.Hidden;

                            switch (currentObject.TagGroup)
                            {
                                case Reach.TagGroup.Bipd:
                                    bipdPlugin.Visibility = System.Windows.Visibility.Visible;

                                    objBipd = currentObject as Reach.BipedObject;

                                    txtBipdPlasmaNade.Text = Convert.ToString(objBipd.PlasmaGrenades);
                                    txtBipdFragNade.Text = Convert.ToString(objBipd.FragGrenades);

                                    cBBipdInvici.IsChecked = objBipd.Invincible;

                                    currentPlugin = 0;
                                    break;

                                case Reach.TagGroup.Weap:
                                    weapPlugin.Visibility = System.Windows.Visibility.Visible;

                                    objWeap = currentObject as Reach.WeaponObject;

                                    txtWeapPluginClipAmmo.Text = Convert.ToString(objWeap.ClipAmmo);
                                    txtWeapPluginAmmo.Text = Convert.ToString(objWeap.Ammo);

                                    cBWeapInvici.IsChecked = objWeap.Invincible;

                                    currentPlugin = 1;
                                    break;

                                case Reach.TagGroup.Vehi:
                                    vehiPlugin.Visibility = System.Windows.Visibility.Visible;

                                    objVehi = currentObject as Reach.VehicleObject;

                                    cBVehiInvici.IsChecked = objVehi.Invincible;

                                    currentPlugin = 2;
                                    break;

                                default:
                                    currentPlugin = -1;
                                    break;
                            }

                            // Mass object move
                            if (currentObject.TagGroup == Reach.TagGroup.Bloc)
                            {
                                lblOpen.Visibility = System.Windows.Visibility.Hidden;
                                btnOpen.Visibility = System.Windows.Visibility.Hidden;
                            }
                            else
                            {
                                lblOpen.Visibility = System.Windows.Visibility.Visible;
                                btnOpen.Visibility = System.Windows.Visibility.Visible;
                            }

                            // Replace button
                            if ((currentObject.TagGroup == Reach.TagGroup.Weap ||
                                currentObject.TagGroup == Reach.TagGroup.Eqip) &&
                                ((TreeViewItem)SelectedItem.Parent).Items.Count > 1)
                            {
                                btnReplace.Visibility = System.Windows.Visibility.Visible;
                                lblReplace.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                btnReplace.Visibility = System.Windows.Visibility.Hidden;
                                lblReplace.Visibility = System.Windows.Visibility.Hidden;
                            }

                            // Delete button
                            if (currentObject != classInfo.storage.fileInfoStorage.saveData.Player.Biped)
                            {
                                btnDelete.Visibility = System.Windows.Visibility.Visible;
                                lblDelete.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                btnDelete.Visibility = System.Windows.Visibility.Hidden;
                                lblDelete.Visibility = System.Windows.Visibility.Hidden;
                            }

                            objectInfo.Visibility = System.Windows.Visibility.Visible;
                        }
                    }
                    catch { }
                }
            }
        }
        #endregion

        #region wpf bullshit
        private void removeObjectFromTreeView(int index)
        {
            
        }

        #region btnOpenwpf
        private void btnOpen_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnOpen.Source = new BitmapImage(source);

            classInfo.storage.fileInfoStorage.massCordMoveType = classInfo.loadPackageData.convertStringToClass(currentParentNodeTag);

            OnExecuteMethod();
        }

        private void btnOpen_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnOpen.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnOpen.Source = new BitmapImage(source);
            }
        }

        private void btnOpen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
            btnOpen.Source = new BitmapImage(source);
        }
        #endregion

        #region maxPrimaryWeap
        private void btnPrimaryMaxWeaponAmmo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnPrimaryMaxWeaponAmmo.Source = new BitmapImage(source);

            txtWeapPluginAmmo.Text = "32767";
        }

        private void btnPrimaryMaxWeaponAmmo_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnPrimaryMaxWeaponAmmo.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnPrimaryMaxWeaponAmmo.Source = new BitmapImage(source);
            }
        }

        private void btnPrimaryMaxWeaponAmmo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
            btnPrimaryMaxWeaponAmmo.Source = new BitmapImage(source);
        }
        #endregion

        #region maxFragNades
        private void btnBipdFragMax_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnBipdFragMax.Source = new BitmapImage(source);

            txtBipdFragNade.Text = "127";
        }

        private void btnBipdFragMax_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnBipdFragMax.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnBipdFragMax.Source = new BitmapImage(source);
            }
        }

        private void btnBipdFragMax_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
            btnBipdFragMax.Source = new BitmapImage(source);
        }
        #endregion

        #region maxPlasmaNades
        private void btnBipdPlasmaMax_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnBipdPlasmaMax.Source = new BitmapImage(source);

            txtBipdPlasmaNade.Text = "127";
        }

        private void btnBipdPlasmaMax_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnBipdPlasmaMax.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnBipdPlasmaMax.Source = new BitmapImage(source);
            }
        }

        private void btnBipdPlasmaMax_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
            btnBipdPlasmaMax.Source = new BitmapImage(source);
        }
        #endregion

        #region maxPrimaryWeapClip
        private void btnMaxPrimaryClip_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnMaxPrimaryClip.Source = new BitmapImage(source);

            txtWeapPluginClipAmmo.Text = "32767";
        }

        private void btnMaxPrimaryClip_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnMaxPrimaryClip.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnMaxPrimaryClip.Source = new BitmapImage(source);
            }
        }

        private void btnMaxPrimaryClip_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
            btnMaxPrimaryClip.Source = new BitmapImage(source);
        }
        #endregion

        #region btnDelete
        private void btnDelete_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnDelete.Source = new BitmapImage(source);

            Reach.GameObject currentObject = classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex];
            currentObject.Delete();

            // Remove the TreeViewItem
            TreeViewItem tvi = objectItems[currentChunkIndex];
            TreeViewItem parent = (TreeViewItem)tvi.Parent;
            int currentPos = parent.Items.IndexOf(tvi);
            parent.Items.Remove(tvi);

            // Select a nearby one
            if (currentPos == parent.Items.Count)
                currentPos--;
            if (currentPos >= 0)
            {
                ((TreeViewItem)parent.Items.GetItemAt(currentPos)).IsSelected = true;
            }
            else
            {
                currentChunkIndex = -1;
                objectInfo.Visibility = System.Windows.Visibility.Hidden;
                btnDelete.Visibility = System.Windows.Visibility.Hidden;
                lblDelete.Visibility = System.Windows.Visibility.Hidden;
                btnReplace.Visibility = System.Windows.Visibility.Hidden;
                lblReplace.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void btnDelete_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnDelete.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnDelete.Source = new BitmapImage(source);
            }
        }

        private void btnDelete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
            btnDelete.Source = new BitmapImage(source);
        }
        #endregion

        #region btnReplace
        private void btnReplace_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnReplace.Source = new BitmapImage(source);

            // Set up the list of items
            classInfo.storage.fileInfoStorage.listboxItems = new List<ListBoxItem>();
            TreeViewItem tvi = objectItems[currentChunkIndex];
            TreeViewItem parent = (TreeViewItem)tvi.Parent;
            foreach (TreeViewItem item in parent.Items)
            {
                if ((int)item.Tag != currentChunkIndex)
                {
                    ListBoxItem lbItem = new ListBoxItem();
                    lbItem.Content = item.Header;
                    lbItem.Tag = item;
                    classInfo.storage.fileInfoStorage.listboxItems.Add(lbItem);
                }
            }
            classInfo.storage.fileInfoStorage.replaceObjectName = (string)tvi.Header;
            
            OnExecuteMethod2();
            ListBoxItem selectedItem = classInfo.storage.fileInfoStorage.selectedListboxItem;
            if (selectedItem != null)
            {
                TreeViewItem newItem = (TreeViewItem)selectedItem.Tag;
                Reach.GameObject oldObj = classInfo.storage.fileInfoStorage.saveData.Objects[(int)tvi.Tag];
                Reach.GameObject newObj = classInfo.storage.fileInfoStorage.saveData.Objects[(int)newItem.Tag];
                oldObj.ReplaceWith(newObj);

                newItem.IsSelected = true;
                parent.Items.Remove(tvi);
            }
        }

        private void btnReplace_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnReplace.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnReplace.Source = new BitmapImage(source);
            }
        }

        private void btnReplace_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
            btnReplace.Source = new BitmapImage(source);
        }
        #endregion

        #region btnParent
        private void btnParent_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnParent.Source = new BitmapImage(source);

            Reach.GameObject currentObject = classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex];
            TreeViewItem tvi = objectItems[(int)(currentObject.Carrier.ID & 0xFFFF)];
            ((TreeViewItem)tvi.Parent).ExpandSubtree();
            tvi.IsSelected = true;
        }

        private void btnParent_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnParent.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnParent.Source = new BitmapImage(source);
            }
        }

        private void btnParent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
            btnParent.Source = new BitmapImage(source);
        }
        #endregion

        #region btnChildren
        private void btnChildren_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnChildren.Source = new BitmapImage(source);

            Reach.GameObject currentObject = classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex];
            Reach.GameObject obj = currentObject.FirstCarried;
            classInfo.storage.fileInfoStorage.listboxItems = new List<ListBoxItem>();
            while (obj != null)
            {
                ListBoxItem item = new ListBoxItem();
                int index = (int)(obj.ID & 0xFFFF);
                TreeViewItem tvi = objectItems[index];
                item.Content = "[" + ((TreeViewItem)tvi.Parent).Header + "] " + tvi.Header;
                item.Tag = tvi;
                classInfo.storage.fileInfoStorage.listboxItems.Add(item);
                obj = obj.NextCarried;
            }
            OnExecuteMethod3();

            ListBoxItem selectedItem = classInfo.storage.fileInfoStorage.selectedListboxItem;
            if (selectedItem != null)
            {
                // This is kinda glitchy, but it works
                TreeViewItem tvi = (TreeViewItem)selectedItem.Tag;
                ((TreeViewItem)tvi.Parent).ExpandSubtree();
                tvi.IsSelected = true;
            }
        }

        private void btnChildren_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnChildren.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnChildren.Source = new BitmapImage(source);
            }
        }

        private void btnChildren_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
            btnChildren.Source = new BitmapImage(source);
        }
        #endregion

        #region textValidation
        #region weapPugin
        private void txtWeapPluginClipAmmo_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txtWeapPluginClipAmmo.Text == "") { }
            else
            {
                try
                {
                    int validate = int.Parse(txtWeapPluginClipAmmo.Text);

                    if (validate > 32767)
                    {
                        txtWeapPluginClipAmmo.Text = "32767";
                    }

                }
                catch
                {
                    int line = txtWeapPluginClipAmmo.Text.Length - 1;
                    txtWeapPluginClipAmmo.Text = txtWeapPluginClipAmmo.Text.Remove(line, 1);
                    txtWeapPluginClipAmmo.Select(line, 0);
                }
            }

            if (txtWeapPluginClipAmmo.Text == "") { txtWeapPluginClipAmmo.Text = "0"; }
        }

        private void txtWeapPluginAmmo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtWeapPluginAmmo.Text == "") { }
            else
            {
                try
                {
                    int validate = int.Parse(txtWeapPluginAmmo.Text);

                    if (validate > 32767)
                    {
                        txtWeapPluginAmmo.Text = "32767";
                    }

                }
                catch
                {
                    int line = txtWeapPluginAmmo.Text.Length - 1;
                    txtWeapPluginAmmo.Text = txtWeapPluginAmmo.Text.Remove(line, 1);
                    txtWeapPluginAmmo.Select(line, 0);
                }
            }

            if (txtWeapPluginAmmo.Text == "") { txtWeapPluginAmmo.Text = "0"; }
        }
        #endregion

        #region bipdPlugin
        private void txtBipdPlasmaNade_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txtBipdPlasmaNade.Text == "") { }
            else
            {
                try
                {
                    int validate = int.Parse(txtBipdPlasmaNade.Text);

                    if (validate > 127)
                    {
                        txtBipdPlasmaNade.Text = "127";
                    }

                }
                catch
                {
                    int line = txtBipdPlasmaNade.Text.Length - 1;
                    txtBipdPlasmaNade.Text = txtBipdPlasmaNade.Text.Remove(line, 1);
                    txtBipdPlasmaNade.Select(line, 0);
                }
            }

            if (txtBipdPlasmaNade.Text == "") { txtBipdPlasmaNade.Text = "0"; }
        }

        private void txtBipdFragNade_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txtBipdFragNade.Text == "") { }
            else
            {
                try
                {
                    int validate = int.Parse(txtBipdFragNade.Text);

                    if (validate > 127)
                    {
                        txtBipdFragNade.Text = "127";
                    }

                }
                catch
                {
                    int line = txtBipdFragNade.Text.Length - 1;
                    txtBipdFragNade.Text = txtBipdFragNade.Text.Remove(line, 1);
                    txtBipdFragNade.Select(line, 0);
                }
            }

            if (txtBipdFragNade.Text == "") { txtBipdFragNade.Text = "0"; }
        }
        #endregion
        #endregion
        #endregion
    }
}