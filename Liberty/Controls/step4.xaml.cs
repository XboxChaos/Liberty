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
	/// Interaction logic for step4.xaml
	/// </summary>
	public partial class step4 : UserControl
	{
        private int currentChunkIndex = -1;
        private string currentParentNodeTag = null;
        public event EventHandler ExecuteMethod;
        public int currentPlugin = -1;
        Reach.BipedObject objBipd;
        Reach.WeaponObject objWeap;


		public step4()
		{
			InitializeComponent();

            lblOpen.Visibility = System.Windows.Visibility.Hidden;
            btnOpen.Visibility = System.Windows.Visibility.Hidden;
		}

        protected virtual void OnExecuteMethod() { if (ExecuteMethod != null) ExecuteMethod(this, EventArgs.Empty); }

        public void loadData()
        {
            if (currentChunkIndex == -1)
                objectInfo.Visibility = System.Windows.Visibility.Hidden;
            foreach (TreeViewItem item in tVObjects.Items)
            {
                item.Items.Clear();
            }
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
                    case 7:
                        objWeap.Invincible = (bool)cBWeapInvici.IsChecked;
                        objWeap.Ammo = Convert.ToInt16(txtWeapPluginAmmo.Text);
                        objWeap.ClipAmmo = Convert.ToInt16(txtWeapPluginClipAmmo.Text);

                        classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex] = objWeap;
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
                case Reach.TagGroup.Vehi:
                    return 8;
                case Reach.TagGroup.Weap:
                    return 7;
                case Reach.TagGroup.Eqip:
                    return 2;
                case Reach.TagGroup.Term:
                    return 6;
                case Reach.TagGroup.Scen:
                    return 5;
                case Reach.TagGroup.Mach:
                    return 4;
                case Reach.TagGroup.Ctrl:
                    return 1;
                case Reach.TagGroup.Ssce:
                    return 3;
                case Reach.TagGroup.Bloc:
                    return 9;
                case Reach.TagGroup.Crea:
                    return 10;
                case Reach.TagGroup.Efsc:
                    return 11;
            }
            return 12;
        }

        public static Reach.TagGroup convertIntToClass(int type)
        {
            switch (type)
            {
                case 0:
                    return Reach.TagGroup.Bipd;
                case 8:
                    return Reach.TagGroup.Vehi;
                case 7:
                    return Reach.TagGroup.Weap;
                case 2:
                    return Reach.TagGroup.Eqip;
                case 6:
                    return Reach.TagGroup.Term;
                case 5:
                    return Reach.TagGroup.Scen;
                case 4:
                    return Reach.TagGroup.Mach;
                case 1:
                    return Reach.TagGroup.Ctrl;
                case 3:
                    return Reach.TagGroup.Ssce;
                case 9:
                    return Reach.TagGroup.Bloc;
                case 10:
                    return Reach.TagGroup.Crea;
                case 11:
                    return Reach.TagGroup.Efsc;
            }
            return Reach.TagGroup.Unknown;
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
                            case 7:
                                objWeap.Invincible = (bool)cBWeapInvici.IsChecked;
                                objWeap.Ammo = Convert.ToInt16(txtWeapPluginAmmo.Text);
                                objWeap.ClipAmmo = Convert.ToInt16(txtWeapPluginClipAmmo.Text);

                                classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex] = objWeap;
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
                            txtObjectXCord.Text = classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex].X.ToString();
                            txtObjectYCord.Text = classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex].Y.ToString();
                            txtObjectZCord.Text = classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex].Z.ToString();

                            lblResourceIdent.Content = "0x" + classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex].ResourceID.ToString("X");
                            lblMapIdent.Content = classInfo.nameLookup.translate(classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex].ResourceID);
                            lblObjectType.Content = classInfo.loadPackageData.convertClassToString(classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex].TagGroup);

                            weapPlugin.Visibility = System.Windows.Visibility.Hidden;
                            bipdPlugin.Visibility = System.Windows.Visibility.Hidden;

                            if ((int)convertClassToNode(classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex].TagGroup) == 0) //Bipd Plugin
                            {
                                bipdPlugin.Visibility = System.Windows.Visibility.Visible;

                                objBipd = classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex] as Reach.BipedObject;

                                txtBipdPlasmaNade.Text = Convert.ToString(objBipd.PlasmaGrenades);
                                txtBipdFragNade.Text = Convert.ToString(objBipd.FragGrenades);

                                cBBipdInvici.IsChecked = objBipd.Invincible;

                                currentPlugin = 0;
                            }
                            else if ((int)convertClassToNode(classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex].TagGroup) == 7) //Weapon Plugin
                            {
                                weapPlugin.Visibility = System.Windows.Visibility.Visible;

                                objWeap = classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex] as Reach.WeaponObject;

                                txtWeapPluginClipAmmo.Text = Convert.ToString(objWeap.ClipAmmo);
                                txtWeapPluginAmmo.Text = Convert.ToString(objWeap.Ammo);

                                cBWeapInvici.IsChecked = objWeap.Invincible;

                                currentPlugin = 2;
                            }
                            else
                            {
                                currentPlugin = -1;
                            }

                            //classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex].
                            if ((int)convertClassToNode(classInfo.storage.fileInfoStorage.saveData.Objects[currentChunkIndex].TagGroup) == 9)
                            {
                                lblOpen.Visibility = System.Windows.Visibility.Hidden;
                                btnOpen.Visibility = System.Windows.Visibility.Hidden;
                            }
                            else
                            {
                                lblOpen.Visibility = System.Windows.Visibility.Visible;
                                btnOpen.Visibility = System.Windows.Visibility.Visible;
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
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
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
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
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
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
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
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
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
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnMaxPrimaryClip.Source = new BitmapImage(source);
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