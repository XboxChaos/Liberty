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
		public quickTweaks()
		{
			this.InitializeComponent();
		}
		
		public void Load(Util.SaveEditor saveEditor)
		{
            string message = saveEditor.CheckpointText;
            if (message == "Checkpoint... done")
            {
                string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                message = "Modded with Liberty " + version;
            }
            txtStartingMsg.Text = message;
		}
		
		public bool Save(Util.SaveEditor saveEditor)
		{
            if ((bool)checkAllMaxAmmo.IsChecked)
                saveEditor.AllWeaponsMaxAmmo();

            saveEditor.CheckpointText = txtStartingMsg.Text;

            saveEditor.SaveChanges(Properties.Resources.KV);
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