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
    /// Interaction logic for messageBox.xaml
    /// </summary>
    public partial class messageBoxOptions : Window
    {
        public string msgTitle;
        public string msgContent;

        public messageBoxOptions()
        {
            this.InitializeComponent();

            //lblTitle.Text = lblTitle.Text.ToUpper();
            //lblSubInfo.Text = lblSubInfo.Text.ToUpper();
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

            classInfo.storage.fileInfoStorage.messageOpt = true;

            FormFadeOut.Begin();
        }

        private void btnCan_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnCan.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnCan.Source = new BitmapImage(source);
            }
        }

        private void btnCan_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnCan.Source = new BitmapImage(source);
        }

        private void btnCan_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnCan.Source = new BitmapImage(source);

            classInfo.storage.fileInfoStorage.messageOpt = false;

            FormFadeOut.Begin();
        }

        private void FormFadeOut_Completed(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}