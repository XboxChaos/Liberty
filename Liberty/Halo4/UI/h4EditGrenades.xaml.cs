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

namespace Liberty.Halo4.UI
{
    /// <summary>
    /// Interaction logic for h4EditGrenades.xaml
    /// </summary>
    public partial class h4EditGrenades : UserControl, StepUI.IStep
    {
        Util.SaveManager<Halo4.CampaignSave> _saveManager;
        private MainWindow mainWindow = null;

        public h4EditGrenades(Util.SaveManager<Halo4.CampaignSave> saveManager)
        {
            _saveManager = saveManager;
            this.InitializeComponent();
            this.Loaded += new RoutedEventHandler(cexEditGrenades_Loaded);
        }

        void cexEditGrenades_Loaded(object sender, RoutedEventArgs e)
        {
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
            Halo4.CampaignSave saveData = _saveManager.SaveData;
            txtFragNades.Text = saveData.PlayerBiped.FragGrenades.ToString();
            txtPlasmaNades.Text = saveData.PlayerBiped.PlasmaGrenades.ToString();
            txtPulseNades.Text = saveData.PlayerBiped.PulseGrenades.ToString();
        }

        public bool Save()
        {
            Halo4.CampaignSave saveData = _saveManager.SaveData;
            try
            {
                int validateF = Math.Max(0, Math.Min(127, int.Parse(txtFragNades.Text)));
                int validateP = Math.Max(0, Math.Min(127, int.Parse(txtPlasmaNades.Text)));
                int validateS = Math.Max(0, Math.Min(127, int.Parse(txtPulseNades.Text)));

                saveData.PlayerBiped.FragGrenades = Convert.ToSByte(validateF);
                saveData.PlayerBiped.PlasmaGrenades = Convert.ToSByte(validateP);
                saveData.PlayerBiped.PulseGrenades = Convert.ToSByte(validateS);
            }
            catch
            {
                mainWindow.showMessage("Invalid grenade count, you can only have a maximum of 127, and a minimum of 0", "INVALID COUNT");
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
        private void btnMaxPulseNades_Click(object sender, RoutedEventArgs e)
        {
            txtPulseNades.Text = "127";
        }
    }
}