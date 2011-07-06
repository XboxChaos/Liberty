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

namespace Liberty.Controls
{
	/// <summary>
	/// Interaction logic for step5.xaml
	/// </summary>
	public partial class step5 : UserControl
	{		
		public step5()
		{
			this.InitializeComponent();
		}

        public string loadData()
        {
            return classInfo.stfsCheck.finishFileEditing();
        }

        public void saveData()
        {
            //code to save data here
        }
		
	}
}