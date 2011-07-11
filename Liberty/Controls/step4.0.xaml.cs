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
	public partial class step4_0 : UserControl
	{
		public step4_0()
		{
			this.InitializeComponent();
		}
		
		public void loadData()
		{
            string message = classInfo.storage.fileInfoStorage.saveData.Message;
            if (message == "Checkpoint... done")
            {
                string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                message = "Modded with Liberty " + version;
            }
            txtStartingMsg.Text = message;
		}
		
		public void saveData()
		{
            if ((bool)checkAllMaxAmmo.IsChecked)
            {
                classInfo.savePackageData.setAllMaxAmmo();
            }
            classInfo.storage.fileInfoStorage.saveData.Message = txtStartingMsg.Text;
		}
	}
}