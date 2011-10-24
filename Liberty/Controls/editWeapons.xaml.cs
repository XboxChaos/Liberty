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

        public void Load(Util.SaveManager saveManager)
        {
            Reach.CampaignSave saveData = saveManager.SaveData;
            if (saveData.Player.PrimaryWeapon != null)
            {
                gridPrimary.Visibility = Visibility.Visible;
                txtPrimaryAmmo.Text = saveData.Player.PrimaryWeapon.Ammo.ToString();
                txtPrimaryClip.Text = saveData.Player.PrimaryWeapon.ClipAmmo.ToString();
            }
            else
            {
                gridPrimary.Visibility = Visibility.Collapsed;
            }

            if (saveData.Player.SecondaryWeapon != null)
            {
                gridSecondary.Visibility = Visibility.Visible;
                txtSecondaryAmmo.Text = saveData.Player.SecondaryWeapon.Ammo.ToString();
                txtSecondaryClip.Text = saveData.Player.SecondaryWeapon.ClipAmmo.ToString();
            }
            else
            {
                gridSecondary.Visibility = Visibility.Collapsed;
            }
        }

        public bool Save(Util.SaveManager saveManager)
        {
            Reach.CampaignSave saveData = saveManager.SaveData;
            if (saveData.Player.PrimaryWeapon != null)
            {
                saveData.Player.PrimaryWeapon.Ammo = Convert.ToInt16(txtPrimaryAmmo.Text);
                saveData.Player.PrimaryWeapon.ClipAmmo = Convert.ToInt16(txtPrimaryClip.Text);
            }

            if (saveData.Player.SecondaryWeapon != null)
            {
                saveData.Player.SecondaryWeapon.Ammo = Convert.ToInt16(txtSecondaryAmmo.Text);
                saveData.Player.SecondaryWeapon.ClipAmmo = Convert.ToInt16(txtSecondaryClip.Text);
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
    }
}