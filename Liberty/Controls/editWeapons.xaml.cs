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
        public editWeapons()
        {
            this.InitializeComponent();
        }

        public void Load(Util.SaveEditor saveEditor)
        {
            if (saveEditor.PlayerHasPrimaryWeapon)
            {
                gridPrimary.Visibility = Visibility.Visible;
                txtPrimaryAmmo.Text = saveEditor.PrimaryWeaponAmmo.ToString();
                txtPrimaryClip.Text = saveEditor.PrimaryWeaponClip.ToString();
            }
            else
            {
                gridPrimary.Visibility = Visibility.Collapsed;
            }

            if (saveEditor.PlayerHasSecondaryWeapon)
            {
                gridSecondary.Visibility = Visibility.Visible;
                txtSecondaryAmmo.Text = saveEditor.SecondaryWeaponAmmo.ToString();
                txtSecondaryClip.Text = saveEditor.SecondaryWeaponClip.ToString();
            }
            else
            {
                gridSecondary.Visibility = Visibility.Collapsed;
            }

            txtFragNades.Text = saveEditor.FragGrenades.ToString();
            txtPlasmaNades.Text = saveEditor.PlasmaGrenades.ToString();
        }

        public bool Save(Util.SaveEditor saveEditor)
        {
            if (saveEditor.PlayerHasPrimaryWeapon)
            {
                saveEditor.PrimaryWeaponAmmo = Convert.ToInt16(txtPrimaryAmmo.Text);
                saveEditor.PrimaryWeaponClip = Convert.ToInt16(txtPrimaryClip.Text);
            }
            if (saveEditor.PlayerHasSecondaryWeapon)
            {
                saveEditor.SecondaryWeaponAmmo = Convert.ToInt16(txtSecondaryAmmo.Text);
                saveEditor.SecondaryWeaponClip = Convert.ToInt16(txtSecondaryClip.Text);
            }

            saveEditor.FragGrenades = Convert.ToSByte(txtFragNades.Text);
            saveEditor.PlasmaGrenades = Convert.ToSByte(txtPlasmaNades.Text);

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

        #region textValidation
        private void txtPrimaryWeapAmmo_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            /*if (txtPrimaryWeapAmmo.Text == "") { }
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

            if (txtPrimaryWeapAmmo.Text == "") { txtPrimaryWeapAmmo.Text = "0"; }*/
        }

        private void txtPrimaryWeapClipAmmo_TextChanged(object sender, TextChangedEventArgs e)
        {
            /*if (txtPrimaryWeapClipAmmo.Text == "") { }
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

            if (txtPrimaryWeapClipAmmo.Text == "") { txtPrimaryWeapClipAmmo.Text = "0"; }*/
        }

        private void txtSecondaryWeapClipAmmo_TextChanged(object sender, TextChangedEventArgs e)
        {
            /*if (txtSecondaryWeapClipAmmo.Text == "") { }
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

            if (txtSecondaryWeapClipAmmo.Text == "") { txtSecondaryWeapClipAmmo.Text = "0"; }*/
        }

        private void txtSecondaryWeapAmmo_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            /*if (txtSecondaryWeapAmmo.Text == "") { }
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

            if (txtSecondaryWeapAmmo.Text == "") { txtSecondaryWeapAmmo.Text = "0"; }*/
        }

        private void txtFragNadeCount_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            /*if (txtFragNadeCount.Text == "") { }
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

            if (txtFragNadeCount.Text == "") { txtFragNadeCount.Text = "0"; }*/
        }

        private void txtPlasmaNadeCount_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            /*if (txtPlasmaNadeCount.Text == "") { }
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

            if (txtPlasmaNadeCount.Text == "") { txtPlasmaNadeCount.Text = "0"; }*/
        }
        #endregion

        private void btnMaxPrimaryClip_Click(object sender, RoutedEventArgs e)
        {
            txtPrimaryClip.Text = "32767";
        }

        private void btnMaxPrimaryAmmo_Click(object sender, RoutedEventArgs e)
        {
            txtPrimaryAmmo.Text = "32767";
        }

        private void btnMaxSecondaryClip_Click(object sender, RoutedEventArgs e)
        {
            txtSecondaryClip.Text = "32767";
        }

        private void btnMaxSecondaryAmmo_Click(object sender, RoutedEventArgs e)
        {
            txtSecondaryAmmo.Text = "32767";
        }

        private void btnMaxFragNades_Click(object sender, RoutedEventArgs e)
        {
            txtFragNades.Text = "127";
        }

        private void btnMaxPlasmaNades_Click(object sender, RoutedEventArgs e)
        {
            txtPlasmaNades.Text = "127";
        }
    }
}