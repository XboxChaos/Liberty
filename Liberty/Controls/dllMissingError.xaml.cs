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
using System.Windows.Shapes;

namespace Liberty.Controls
{
	/// <summary>
	/// Interaction logic for dllMissingError.xaml
	/// </summary>
	public partial class dllMissingError : Window
	{	
		public dllMissingError()
		{
			this.InitializeComponent();
		}
		
		private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            FormFadeOut.Begin();
            classInfo.applicationExtra.disableInput(this);
        }
		
		private void FormFadeOut_Completed(object sender, EventArgs e)
        {
            this.Close();
        }
	}
}