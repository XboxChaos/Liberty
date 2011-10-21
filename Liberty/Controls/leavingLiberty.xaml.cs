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
using System.Diagnostics;

namespace Liberty.Controls
{
	/// <summary>
	/// Interaction logic for leavingLiberty.xaml
	/// </summary>
	public partial class leavingLiberty : Window
	{
        private string _url;

		public leavingLiberty(string siteName, string url)
		{
			this.InitializeComponent();
            message.Text = message.Text.Replace("{0}", siteName);
            _url = url;
		}
		
        private void btnLeave_Click(object sender, RoutedEventArgs e)
        {
            FormFadeOut.Begin();
            classInfo.applicationExtra.disableInput(this);

            Process.Start(_url);
        }

        private void btnStay_Click(object sender, RoutedEventArgs e)
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