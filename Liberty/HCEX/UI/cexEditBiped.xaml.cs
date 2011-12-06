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
    /// Interaction logic for cexEditBiped.xaml
    /// </summary>
    public partial class cexEditBiped : UserControl, StepUI.IStep
    {
        private MainWindow mainWindow = null;
        private Util.SaveManager<HCEX.CampaignSave> _saveManager;

        public cexEditBiped(Util.SaveManager<HCEX.CampaignSave> saveManager)
        {
            _saveManager = saveManager;
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(step2_Loaded);
        }

        void step2_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this) as MainWindow;
        }

        public void Load()
        {
            HCEX.CampaignSave saveData = _saveManager.SaveData;
            HCEX.BipedObject playerBiped = saveData.PlayerBiped;
            checkInvincible.IsChecked = playerBiped.Invincible;

            txtPlayerXCord.Text = playerBiped.Position.X.ToString();
            txtPlayerYCord.Text = playerBiped.Position.Y.ToString();
            txtPlayerZCord.Text = playerBiped.Position.Z.ToString();
        }

        public bool Save()
        {
            HCEX.CampaignSave saveData = _saveManager.SaveData;

            HCEX.BipedObject playerBiped = saveData.PlayerBiped;
            playerBiped.MakeInvincible((bool)checkInvincible.IsChecked);
            
            // TODO: Actually detect if this is a vehicle, and not some homosexual dragon (fixhax)
            if (playerBiped.Carrier != null)
                playerBiped.Carrier.MakeInvincible((bool)checkInvincible.IsChecked);

            float newX = Convert.ToSingle(txtPlayerXCord.Text);
            float newY = Convert.ToSingle(txtPlayerYCord.Text);
            float newZ = Convert.ToSingle(txtPlayerZCord.Text);
            playerBiped.Position = new MathUtil.Vector3(newX, newY, newZ);

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

        #region wpf
        #region textValidation
        private void txtPlayerXCord_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txtPlayerXCord.Text == "") { }
            else
            {
                try
                {
                    float validate = float.Parse(txtPlayerXCord.Text);
                }
                catch
                {
                    int line = txtPlayerXCord.Text.Length - 1;
                    txtPlayerXCord.Text = txtPlayerXCord.Text.Remove(line, 1);
                    txtPlayerXCord.Select(line, 0);
                }
            }

            if (txtPlayerXCord.Text == "") { txtPlayerXCord.Text = "0"; }
        }

        private void txtPlayerYCord_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txtPlayerYCord.Text == "") { }
            else
            {
                try
                {
                    float validate = float.Parse(txtPlayerYCord.Text);
                }
                catch
                {
                    int line = txtPlayerYCord.Text.Length - 1;
                    txtPlayerYCord.Text = txtPlayerYCord.Text.Remove(line, 1);
                    txtPlayerYCord.Select(line, 0);
                }
            }

            if (txtPlayerYCord.Text == "") { txtPlayerYCord.Text = "0"; }
        }

        private void txtPlayerZCord_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txtPlayerZCord.Text == "") { }
            else
            {
                try
                {
                    float validate = float.Parse(txtPlayerZCord.Text);
                }
                catch
                {
                    int line = txtPlayerZCord.Text.Length - 1;
                    txtPlayerZCord.Text = txtPlayerZCord.Text.Remove(line, 1);
                    txtPlayerZCord.Select(line, 0);
                }
            }

            if (txtPlayerZCord.Text == "") { txtPlayerZCord.Text = "0"; }
        }
        #endregion
        #endregion
    }
}