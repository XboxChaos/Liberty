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

        private void ValidateByte(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null)
                return;

            textBox.Tag = true; // Mark as changed

            if (textBox.Text != "")
            {
                int value;
                if (int.TryParse(textBox.Text, out value))
                {
                    if (value > 127)
                        textBox.Text = "127";
                }
                else
                {
                    int line = textBox.Text.Length - 1;
                    textBox.Text = textBox.Text.Remove(line, 1);
                    textBox.Select(line, 0);
                }
            }
            else
            {
                textBox.Text = "0";
            }
        }
    }
}