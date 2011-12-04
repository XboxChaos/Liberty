using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Liberty.HCEX.UI
{
    /// <summary>
    /// Interaction logic for cexEditWeapons.xaml
    /// </summary>
    public partial class cexEditWeapons : UserControl, StepUI.IStep
    {
        private Util.SaveManager<HCEX.CampaignSave> _saveManager;

        public cexEditWeapons(Util.SaveManager<HCEX.CampaignSave> saveManager)
        {
            _saveManager = saveManager;
            this.InitializeComponent();
        }

        public void Load()
        {
            /*HCEX.CampaignSave saveData = _saveManager.SaveData;
            HCEX.BipedObject playerBiped = saveData.Player.Biped;
            loadWeapon(playerBiped.PrimaryWeapon, gridPrimary, txtPrimaryAmmo, txtPrimaryClip);
            loadWeapon(playerBiped.SecondaryWeapon, gridSecondary, txtSecondaryAmmo, txtSecondaryClip);
            loadWeapon(playerBiped.TertiaryWeapon, gridTertiary, txtTertiaryAmmo, txtTertiaryClip);
            loadWeapon(playerBiped.QuaternaryWeapon, gridQuaternary, txtQuaternaryAmmo, txtQuaternaryClip);*/
        }

        public bool Save()
        {
            /*HCEX.CampaignSave saveData = _saveManager.SaveData;
            HCEX.BipedObject playerBiped = saveData.Player.Biped;
            saveWeapon(playerBiped.PrimaryWeapon, txtPrimaryAmmo, txtPrimaryClip);
            saveWeapon(playerBiped.SecondaryWeapon, txtSecondaryAmmo, txtSecondaryClip);
            saveWeapon(playerBiped.TertiaryWeapon, txtTertiaryAmmo, txtTertiaryClip);
            saveWeapon(playerBiped.QuaternaryWeapon, txtQuaternaryAmmo, txtQuaternaryClip);*/
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

        /*private void loadWeapon(HCEX.WeaponObject weapon, Grid grid, TextBox ammoBox, TextBox clipBox)
        {
            if (weapon != null)
            {
                grid.IsEnabled = true;
                ammoBox.Text = weapon.Ammo.ToString();
                clipBox.Text = weapon.ClipAmmo.ToString();
            }
            else
            {
                grid.IsEnabled = false;
            }
        }

        private void saveWeapon(HCEX.WeaponObject weapon, TextBox ammoBox, TextBox clipBox)
        {
            if (weapon != null)
            {
                weapon.Ammo = Convert.ToInt16(ammoBox.Text);
                weapon.ClipAmmo = Convert.ToInt16(clipBox.Text);
            }
        }*/

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

        private void btnMaxTertiaryClip_Click(object sender, RoutedEventArgs e)
        {
            txtTertiaryClip.Text = "32767";
        }

        private void btnMaxTertiaryAmmo_Click(object sender, RoutedEventArgs e)
        {
            txtTertiaryAmmo.Text = "32767";
        }

        private void btnMaxQuaternaryClip_Click(object sender, RoutedEventArgs e)
        {
            txtQuaternaryClip.Text = "32767";
        }

        private void btnMaxQuaternaryAmmo_Click(object sender, RoutedEventArgs e)
        {
            txtQuaternaryAmmo.Text = "32767";
        }
    }
}