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
	/// Interaction logic for aboutBox.xaml
	/// </summary>
	public partial class aboutBox : Window
	{
		public aboutBox()
		{
			InitializeComponent();			
		}

		private void lblXboxChaos_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            Process.Start("http://www.xboxchaos.com/");
		}

		private void lblXeraxic_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            Process.Start("http://www.xeraxic.com/");
		}

        private void btnOK_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnOK.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnOK.Source = new BitmapImage(source);
            }
        }

        private void btnOK_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnOK.Source = new BitmapImage(source);
        }

        private void btnOK_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnOK.Source = new BitmapImage(source);

            FormFadeOut.Begin();
        }

        private void FormFadeOut_Completed(object sender, EventArgs e)
        {
            this.Close();
        }
	}
}