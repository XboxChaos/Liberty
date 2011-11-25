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

namespace Liberty.Controls
{
	/// <summary>
	/// Interaction logic for step4_0.xaml
	/// </summary>
	public partial class quickTweaks : UserControl, StepUI.IStep
	{
        private Util.SaveManager<Reach.CampaignSave> _saveManager;

        public quickTweaks(Util.SaveManager<Reach.CampaignSave> saveManager)
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
		}
		
		public bool Save()
		{
            if ((bool)checkAllMaxAmmo.IsChecked)
                Util.EditorSupport.AllWeaponsMaxAmmo(_saveManager.SaveData);
            _saveManager.SaveData.Message = txtStartingMsg.Text;

            _saveManager.SaveChanges(Properties.Resources.KV);
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
	}
}