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
using System.Reflection;

namespace Liberty.Controls
{
	/// <summary>
	/// Interaction logic for aboutBox.xaml
	/// </summary>
	public partial class aboutBox : Window
	{
		public aboutBox()
		{
			InitializeComponent();

            lblTitle.Text = String.Format(lblTitle.Text, Assembly.GetExecutingAssembly().GetName().Version.ToString());
		}

		private void lblXboxChaos_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            Process.Start("http://www.xboxchaos.com/");
		}

		private void lblXeraxic_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            Process.Start("http://www.xeraxic.com/");
		}

        private void lblCodePlex_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("http://liberty.codeplex.com/");
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            classInfo.applicationExtra.disableInput(this);
            FormFadeOut.Begin();
        }

        private void FormFadeOut_Completed(object sender, EventArgs e)
        {
            this.Close();
        }
	}
}