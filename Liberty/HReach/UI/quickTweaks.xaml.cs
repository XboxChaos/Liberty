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
using System.Reflection;
using Liberty.Reach;

namespace Liberty.Controls
{
	/// <summary>
	/// Interaction logic for step4_0.xaml
	/// </summary>
	public partial class quickTweaks : UserControl, StepUI.IStep
	{
        private Util.SaveManager<CampaignSave> _saveManager;

        public quickTweaks(Util.SaveManager<CampaignSave> saveManager)
		{
            _saveManager = saveManager;
			this.InitializeComponent();
		}
		
		public void Load()
		{
            string message = _saveManager.SaveData.Message;
            if (message == "Checkpoint... done")
            {
                string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                message = "Modded with Liberty " + version;
            }
            txtStartingMsg.Text = message;

            Skulls activeSkulls = _saveManager.SaveData.ActiveSkulls;
            cBIron.IsChecked = activeSkulls.HasFlag(Skulls.Iron);
            cBBlackEye.IsChecked = activeSkulls.HasFlag(Skulls.BlackEye);
            cBToughLuck.IsChecked = activeSkulls.HasFlag(Skulls.ToughLuck);
            cBCatch.IsChecked = activeSkulls.HasFlag(Skulls.Catch);
            cBCloud.IsChecked = activeSkulls.HasFlag(Skulls.Cloud);
            cBFamine.IsChecked = activeSkulls.HasFlag(Skulls.Famine);
            cBThunderstorm.IsChecked = activeSkulls.HasFlag(Skulls.Thunderstorm);
            cBTilt.IsChecked = activeSkulls.HasFlag(Skulls.Tilt);
            cBMythic.IsChecked = activeSkulls.HasFlag(Skulls.Mythic);
            cBBlind.IsChecked = activeSkulls.HasFlag(Skulls.Blind);
            cBCowbell.IsChecked = activeSkulls.HasFlag(Skulls.Cowbell);
            cBBirthday.IsChecked = activeSkulls.HasFlag(Skulls.GruntBirthday);
            cBIWHBYD.IsChecked = activeSkulls.HasFlag(Skulls.IWHBYD);
		}
		
		public bool Save()
		{
            if ((bool)checkAllMaxAmmo.IsChecked)
                Util.EditorSupport.AllWeaponsMaxAmmo(_saveManager.SaveData);
            _saveManager.SaveData.Message = txtStartingMsg.Text;
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

        private void checkAll(bool check)
        {
            foreach (UIElement element in skullsPanel.Children)
            {
                CheckBox cb = element as CheckBox;
                if (cb != null)
                    cb.IsChecked = check;
            }
        }

        private void btnLasoSkulls_Click(object sender, RoutedEventArgs e)
        {
            checkAll(true);
        }

        private void btnNoSkulls_Click(object sender, RoutedEventArgs e)
        {
            checkAll(false);
        }
	}
}