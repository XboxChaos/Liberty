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
using Microsoft.Win32;
using System.IO;

namespace Liberty.Controls
{
	/// <summary>
	/// Interaction logic for step0.xaml
	/// </summary>
	public partial class step0 : UserControl
	{
		public step0()
		{
			this.InitializeComponent();
		}

        public void saveData()
        {
            classInfo.storage.fileInfoStorage.saveIsLocal = (bool)cBSaveOP.IsChecked;
        }

        #region wpf bullshit
        
        #endregion
    }
}