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
    /// Interaction logic for cexEditGrenades.xaml
    /// </summary>
    public partial class cexEditGrenades : UserControl, StepUI.IStep
    {
        Util.SaveManager<HCEX.CampaignSave> _saveManager;
        private MainWindow mainWindow = null;

        public cexEditGrenades(Util.SaveManager<HCEX.CampaignSave> saveManager)
        {
            _saveManager = saveManager;
            this.InitializeComponent();

            mainWindow = Window.GetWindow(this) as MainWindow;
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
            HCEX.CampaignSave saveData = _saveManager.SaveData;
            txtFragNades.Text = saveData.PlayerBiped.FragGrenades.ToString();
            txtPlasmaNades.Text = saveData.PlayerBiped.PlasmaGrenades.ToString();
        }

        public bool Save()
        {
            HCEX.CampaignSave saveData = _saveManager.SaveData;
            try
            {
                int validateF = int.Parse(txtFragNades.Text);
                int validateP = int.Parse(txtPlasmaNades.Text);

                if (validateF > 127 && validateF < 0)
                    txtFragNades.Text = "127";
                if (validateF > 127 && validateP < 0)
                    txtPlasmaNades.Text = "127";

                saveData.PlayerBiped.FragGrenades = Convert.ToSByte(validateF);
                saveData.PlayerBiped.PlasmaGrenades = Convert.ToSByte(validateP);
            }
            catch
            {
                mainWindow.showWarning("Invalid grenade count, you can only have a maximum of 127, and a minimum of 0", "INVALID COUNT");
                return false;
            }
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