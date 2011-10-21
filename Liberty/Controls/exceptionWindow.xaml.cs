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
	/// Interaction logic for exceptionWindow.xaml
	/// </summary>
	public partial class exceptionWindow : Window
	{
        private bool _exit = false;

		public exceptionWindow(string message)
		{
			this.InitializeComponent();

            lblException.Text = message;
		}
		
		private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            FormFadeOut.Begin();
            classInfo.applicationExtra.disableInput(this);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            _exit = true;
            FormFadeOut.Begin();
            classInfo.applicationExtra.disableInput(this);
        }

        private void FormFadeOut_Completed(object sender, EventArgs e)
        {
            if (_exit)
                classInfo.applicationExtra.closeApplication();

            this.Close();
        }
	}
}