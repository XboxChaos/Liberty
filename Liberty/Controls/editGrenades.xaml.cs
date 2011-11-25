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

namespace Liberty
{
    /// <summary>
    /// Interaction logic for editGrenades.xaml
    /// </summary>
    public partial class editGrenades : UserControl, StepUI.IStep
    {
        Util.SaveManager<Reach.CampaignSave> _saveManager;

        public editGrenades(Util.SaveManager<Reach.CampaignSave> saveManager)
        {
            _saveManager = saveManager;
            this.InitializeComponent();
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            Visibility = Visibility.Hidden;
        }

        public void Load()
        {
            Reach.CampaignSave saveData = _saveManager.SaveData;
            txtFragNades.Text = saveData.Player.Biped.FragGrenades.ToString();
            txtPlasmaNades.Text = saveData.Player.Biped.PlasmaGrenades.ToString();
        }

        public bool Save()
        {
            Reach.CampaignSave saveData = _saveManager.SaveData;
            saveData.Player.Biped.FragGrenades = Convert.ToSByte(txtFragNades.Text);
            saveData.Player.Biped.PlasmaGrenades = Convert.ToSByte(txtPlasmaNades.Text);

            return true;
        }

        private void btnMaxFragNades_Click(object sender, RoutedEventArgs e)
        {
            txtFragNades.Text = "127";
        }

        private void btnMaxPlasmaNades_Click(object sender, RoutedEventArgs e)
        {
            txtPlasmaNades.Text = "127";
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
    }
}