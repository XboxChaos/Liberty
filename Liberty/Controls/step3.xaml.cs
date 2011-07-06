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
    public partial class step3 : UserControl
    {
        public step3()
        {
            this.InitializeComponent();
        }

        public void loadData()
        {
            int[] ammoArray = classInfo.loadPackageData.getSaveAmmo();

            if (classInfo.storage.fileInfoStorage.saveData.Player.PrimaryWeapon != null)
            {
                txtPrimaryWeapAmmo.Text = Convert.ToString(ammoArray[0]);
                txtPrimaryWeapClipAmmo.Text = Convert.ToString(ammoArray[1]);
            }
            else
            {
                txtPrimaryWeapAmmo.IsEnabled = false;
                txtPrimaryWeapClipAmmo.IsEnabled = false;
                btnMaxPrimaryClip.Visibility = System.Windows.Visibility.Hidden;
                btnPrimaryMaxWeaponAmmo.Visibility = System.Windows.Visibility.Hidden;
                lblMaxPrimaryClip.Visibility = System.Windows.Visibility.Hidden;
                lblPrimaryMaxWeaponAmmo.Visibility = System.Windows.Visibility.Hidden;
            }

            if (classInfo.storage.fileInfoStorage.saveData.Player.SecondaryWeapon != null)
            {
                txtSecondaryWeapAmmo.Text = Convert.ToString(ammoArray[2]);
                txtSecondaryWeapClipAmmo.Text = Convert.ToString(ammoArray[3]);
            }
            else
            {
                txtSecondaryWeapAmmo.IsEnabled = false;
                txtSecondaryWeapClipAmmo.IsEnabled = false;
                btnMaxSecondaryClip.Visibility = System.Windows.Visibility.Hidden;
                btnSecondaryMaxWeaponAmmo.Visibility = System.Windows.Visibility.Hidden;
                lblMaxSecondaryClip.Visibility = System.Windows.Visibility.Hidden;
                lblSecondaryMaxWeaponAmmo.Visibility = System.Windows.Visibility.Hidden;
            }

            txtFragNadeCount.Text = Convert.ToString(ammoArray[4]);
            txtPlasmaNadeCount.Text = Convert.ToString(ammoArray[5]);
        }

        public bool saveData()
        {
            if (txtFragNadeCount.Text == "" || txtPlasmaNadeCount.Text == "" || txtPrimaryWeapAmmo.Text == "" || txtPrimaryWeapClipAmmo.Text == "")
            {
                return false;
            }
            else
            {
                int[] saveAmmo = new int[6];

                if (classInfo.storage.fileInfoStorage.saveData.Player.PrimaryWeapon != null)
                {
                    saveAmmo[0] = Convert.ToInt16(txtPrimaryWeapAmmo.Text);
                    saveAmmo[1] = Convert.ToInt16(txtPrimaryWeapClipAmmo.Text);
                }
                if (classInfo.storage.fileInfoStorage.saveData.Player.SecondaryWeapon != null)
                {
                    saveAmmo[2] = Convert.ToInt16(txtSecondaryWeapAmmo.Text);
                    saveAmmo[3] = Convert.ToInt16(txtSecondaryWeapClipAmmo.Text);
                }
                saveAmmo[4] = Convert.ToInt16(txtFragNadeCount.Text);
                saveAmmo[5] = Convert.ToInt16(txtPlasmaNadeCount.Text);

                classInfo.savePackageData.setPlayerAmmo(saveAmmo);

                return true;
            }
        }

        #region textValidation
        private void txtPrimaryWeapAmmo_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txtPrimaryWeapAmmo.Text == "") { }
            else
            {
                try
                {
                    int validate = int.Parse(txtPrimaryWeapAmmo.Text);

                    if (validate > 32767)
                    {
                        txtPrimaryWeapAmmo.Text = "32767";
                    }

                }
                catch
                {
                    int line = txtPrimaryWeapAmmo.Text.Length - 1;
                    txtPrimaryWeapAmmo.Text = txtPrimaryWeapAmmo.Text.Remove(line, 1);
                    txtPrimaryWeapAmmo.Select(line, 0);
                }
            }

            if (txtPrimaryWeapAmmo.Text == "") { txtPrimaryWeapAmmo.Text = "0"; }
        }

        private void txtPrimaryWeapClipAmmo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtPrimaryWeapClipAmmo.Text == "") { }
            else
            {
                try
                {
                    int validate = int.Parse(txtPrimaryWeapClipAmmo.Text);

                    if (validate > 32767)
                    {
                        txtPrimaryWeapClipAmmo.Text = "32767";
                    }

                }
                catch
                {
                    int line = txtPrimaryWeapClipAmmo.Text.Length - 1;
                    txtPrimaryWeapClipAmmo.Text = txtPrimaryWeapClipAmmo.Text.Remove(line, 1);
                    txtPrimaryWeapClipAmmo.Select(line, 0);
                }
            }

            if (txtPrimaryWeapClipAmmo.Text == "") { txtPrimaryWeapClipAmmo.Text = "0"; }
        }

        private void txtSecondaryWeapClipAmmo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSecondaryWeapClipAmmo.Text == "") { }
            else
            {
                try
                {
                    int validate = int.Parse(txtSecondaryWeapClipAmmo.Text);

                    if (validate > 32767)
                    {
                        txtSecondaryWeapClipAmmo.Text = "32767";
                    }

                }
                catch
                {
                    int line = txtSecondaryWeapClipAmmo.Text.Length - 1;
                    txtSecondaryWeapClipAmmo.Text = txtSecondaryWeapClipAmmo.Text.Remove(line, 1);
                    txtSecondaryWeapClipAmmo.Select(line, 0);
                }
            }

            if (txtSecondaryWeapClipAmmo.Text == "") { txtSecondaryWeapClipAmmo.Text = "0"; }
        }

        private void txtSecondaryWeapAmmo_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txtSecondaryWeapAmmo.Text == "") { }
            else
            {
                try
                {
                    int validate = int.Parse(txtSecondaryWeapAmmo.Text);

                    if (validate > 32767)
                    {
                        txtSecondaryWeapAmmo.Text = "32767";
                    }

                }
                catch
                {
                    int line = txtSecondaryWeapAmmo.Text.Length - 1;
                    txtSecondaryWeapAmmo.Text = txtSecondaryWeapAmmo.Text.Remove(line, 1);
                    txtSecondaryWeapAmmo.Select(line, 0);
                }
            }

            if (txtSecondaryWeapAmmo.Text == "") { txtSecondaryWeapAmmo.Text = "0"; }
        }

        private void txtFragNadeCount_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txtFragNadeCount.Text == "") { }
            else
            {
                try
                {
                    int validate = int.Parse(txtFragNadeCount.Text);

                    if (validate > 127)
                    {
                        txtFragNadeCount.Text = "127";
                    }

                }
                catch
                {
                    int line = txtFragNadeCount.Text.Length - 1;
                    txtFragNadeCount.Text = txtFragNadeCount.Text.Remove(line, 1);
                    txtFragNadeCount.Select(line, 0);
                }
            }

            if (txtFragNadeCount.Text == "") { txtFragNadeCount.Text = "0"; }
        }

        private void txtPlasmaNadeCount_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txtPlasmaNadeCount.Text == "") { }
            else
            {
                try
                {
                    int validate = int.Parse(txtPlasmaNadeCount.Text);

                    if (validate > 127)
                    {
                        txtPlasmaNadeCount.Text = "127";
                    }

                }
                catch
                {
                    int line = txtPlasmaNadeCount.Text.Length - 1;
                    txtPlasmaNadeCount.Text = txtPlasmaNadeCount.Text.Remove(line, 1);
                    txtPlasmaNadeCount.Select(line, 0);
                }
            }

            if (txtPlasmaNadeCount.Text == "") { txtPlasmaNadeCount.Text = "0"; }
        }
        #endregion

        #region wpf bullshit

        #region maxFragNade
        private void btnMaxFrag_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnMaxFrag.Source = new BitmapImage(source);

            txtFragNadeCount.Text = "127";
        }

        private void btnMaxFrag_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnMaxFrag.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnMaxFrag.Source = new BitmapImage(source);
            }
        }

        private void btnMaxFrag_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnMaxFrag.Source = new BitmapImage(source);
        }
        #endregion

        #region maxPlasmaNade
        private void btnPlasmaMax_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnMaxPlasma.Source = new BitmapImage(source);

            txtPlasmaNadeCount.Text = "127";
        }

        private void btnPlasmaMax_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnMaxPlasma.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnMaxPlasma.Source = new BitmapImage(source);
            }
        }

        private void btnPlasmaMax_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnMaxPlasma.Source = new BitmapImage(source);
        }
        #endregion

        #region maxPrimaryWeap
        private void btnPrimaryMaxWeaponAmmo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnPrimaryMaxWeaponAmmo.Source = new BitmapImage(source);

            txtPrimaryWeapAmmo.Text = "32767";
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

        #region maxPrimaryWeapClip
        private void btnMaxPrimaryClip_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnMaxPrimaryClip.Source = new BitmapImage(source);

            txtPrimaryWeapClipAmmo.Text = "32767";
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

        #region maxSecondaryWeap
        private void btnSecondaryMaxWeaponAmmo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnSecondaryMaxWeaponAmmo.Source = new BitmapImage(source);

            txtSecondaryWeapAmmo.Text = "32767";
        }

        private void btnSecondaryMaxWeaponAmmo_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnSecondaryMaxWeaponAmmo.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnSecondaryMaxWeaponAmmo.Source = new BitmapImage(source);
            }
        }

        private void btnSecondaryMaxWeaponAmmo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnSecondaryMaxWeaponAmmo.Source = new BitmapImage(source);
        }
        #endregion

        #region maxSecondaryWeapClip
        private void btnMaxSecondaryClip_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnMaxSecondaryClip.Source = new BitmapImage(source);

            txtSecondaryWeapClipAmmo.Text = "32767";
        }

        private void btnMaxSecondaryClip_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnMaxSecondaryClip.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnMaxSecondaryClip.Source = new BitmapImage(source);
            }
        }

        private void btnMaxSecondaryClip_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnMaxSecondaryClip.Source = new BitmapImage(source);
        }
        #endregion
        #endregion
    }
}