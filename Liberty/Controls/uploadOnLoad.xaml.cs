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
	/// Interaction logic for uploadOnLoad.xaml
	/// </summary>
	public partial class uploadOnLoad : Window
	{
		public uploadOnLoad()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
		}

        #region wpfBullshit
        #region btnOKwpf
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
        #endregion

        #region btnUpdatewpf
        private void btnUpdate_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnUpdate.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnUpdate.Source = new BitmapImage(source);
            }
        }

        private void btnUpdate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnUpdate.Source = new BitmapImage(source);
        }

        private void btnUpdate_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnUpdate.Source = new BitmapImage(source);

            FormFadeOut.Begin();

            classInfo.updating.startUpdate();
        }
        #endregion

        private void FormFadeOut_Completed(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}