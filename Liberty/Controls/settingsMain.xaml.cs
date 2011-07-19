using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for settingsMain.xaml
    /// </summary>
    public partial class settingsMain : UserControl
    {
        private bool isApp = true;
        BrushConverter bc = new BrushConverter();
        FontWeight fw = new FontWeight();
        public event EventHandler ExecuteMethod;
        protected virtual void OnExecuteMethod() { if (ExecuteMethod != null) ExecuteMethod(this, EventArgs.Empty); }

        public settingsMain()
        {
            InitializeComponent();

            themePanel.Visibility = System.Windows.Visibility.Hidden;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            load();
        }

        public void load()
        {
            softCode.loadSettings();
            themeCode.loadSettings();
        }

        #region uncleanWPFstuff
        #region btnSoftwpf
        private void btnSoft_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!isApp)
                btnSoft.Foreground = (Brush)bc.ConvertFrom("#FFD4D2D2");
        }

        private void btnSoft_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!isApp)
                btnSoft.Foreground = (Brush)bc.ConvertFrom("#FF868686");
        }

        private void btnSoft_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!isApp)
               btnSoft.Foreground = (Brush)bc.ConvertFrom("#FFD4D2DF");
        }

        private void btnSoft_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!isApp)
            {
                btnSoft.Foreground = (Brush)bc.ConvertFrom("#FF000000");
                btnSoft.FontWeight = FontWeights.ExtraBold;

                btnTheme.FontWeight = FontWeights.Normal;
                btnTheme.Foreground = (Brush)bc.ConvertFrom("#FF868686");
            }

            isApp = true;
            softwarePanel.Visibility = System.Windows.Visibility.Visible;
            themePanel.Visibility = System.Windows.Visibility.Hidden;
        }
        #endregion

        #region btnThemewpf
        private void btnTheme_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isApp)
                btnTheme.Foreground = (Brush)bc.ConvertFrom("#FFD4D2D2");
        }

        private void btnTheme_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isApp)
                btnTheme.Foreground = (Brush)bc.ConvertFrom("#FF868686");
        }

        private void btnTheme_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isApp)
                btnTheme.Foreground = (Brush)bc.ConvertFrom("#FFD4D2DF");
        }

        private void btnTheme_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isApp)
            {
                btnTheme.Foreground = (Brush)bc.ConvertFrom("#FF000000");
                btnTheme.FontWeight = FontWeights.ExtraBold;

                btnSoft.FontWeight = FontWeights.Normal;
                btnSoft.Foreground = (Brush)bc.ConvertFrom("#FF868686");
            }

            isApp = false;
            themePanel.Visibility = System.Windows.Visibility.Visible;
            softwarePanel.Visibility = System.Windows.Visibility.Hidden;
        }
        #endregion

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
            var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
            btnOK.Source = new BitmapImage(source);

            //Save Code
            softCode.saveSettings();
            themeCode.saveSettings();

            //Leave Code
            OnExecuteMethod();
        }
        #endregion

        #region btnBackwpf
        private void btnBack_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnBack.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnBack.Source = new BitmapImage(source);
            }
        }

        private void btnBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnBack.Source = new BitmapImage(source);
        }

        private void btnBack_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
            btnBack.Source = new BitmapImage(source);
            
            //Close code
            OnExecuteMethod();
        }
        #endregion
        #endregion
    }
}
