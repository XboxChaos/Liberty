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

        public event EventHandler SettingsChanged;
        public event EventHandler Closed;

        public settingsMain()
        {
            InitializeComponent();

            themePanel.Visibility = Visibility.Hidden;
        }

        public void Reload()
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
                btnSoft.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextDark);
                btnSoft.FontWeight = FontWeights.ExtraBold;

                btnTheme.FontWeight = FontWeights.Normal;
                btnTheme.Foreground = (Brush)bc.ConvertFrom("#FF868686");
            }

            isApp = true;
            softwarePanel.Visibility = Visibility.Visible;
            themePanel.Visibility = Visibility.Hidden;
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
                btnTheme.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextDark);
                btnTheme.FontWeight = FontWeights.ExtraBold;

                btnSoft.FontWeight = FontWeights.Normal;
                btnSoft.Foreground = (Brush)bc.ConvertFrom("#FF868686");
            }

            isApp = false;
            themePanel.Visibility = Visibility.Visible;
            softwarePanel.Visibility = Visibility.Hidden;
        }
        #endregion

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            //Save Code
            softCode.saveSettings();
            themeCode.saveSettings();

            //Leave Code
            OnSettingsChanged();
            OnClosed();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            classInfo.applicationExtra.loadApplicationSettings();
            OnClosed();
        }
        #endregion

        private void OnSettingsChanged()
        {
            if (SettingsChanged != null)
                SettingsChanged(this, EventArgs.Empty);
        }

        private void OnClosed()
        {
            if (Closed != null)
                Closed(this, EventArgs.Empty);
        }
    }
}
