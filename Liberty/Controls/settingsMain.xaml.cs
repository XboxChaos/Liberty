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
        BrushConverter bc = new BrushConverter();

        public event EventHandler SettingsChanged;
        public event EventHandler Closed;

        public settingsMain()
        {
            InitializeComponent();
        }

        public void Reload()
        {
            softCode.loadSettings();
            themeCode.loadSettings();
        }

        #region uncleanWPFstuff
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
