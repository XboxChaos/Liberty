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
using Microsoft.Win32;

namespace Liberty.Controls.Settings
{
    /// <summary>
    /// Interaction logic for themeSettings.xaml
    /// </summary>
    public partial class themeSettings : UserControl
    {
        BrushConverter bc = new BrushConverter();

        public themeSettings()
        {
            InitializeComponent();

            hideAllTris();
            btnThemeSettings_MouseDown(null, null);
        }

        public void loadSettings()
        {
            // Load from RegTable
            RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Xeraxic\\Liberty\\themeSettings\\");

            
        }
        public void saveSettings()
        {
            // Save to RegTable
            RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Xeraxic\\Liberty\\themeSettings");

            
        }

        #region uncleanWPFshit
        #region btnUpdate
        private void btnThemeSettings_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnThemeSettings.Foreground = (Brush)bc.ConvertFrom("#828689");
        }

        private void btnThemeSettings_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnThemeSettings.Foreground = (Brush)bc.ConvertFrom("#FF000000");
        }

        private void btnThemeSettings_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            hideAllTris();
            triThem.Visibility = System.Windows.Visibility.Visible;
            themeSettingsPnl.Visibility = System.Windows.Visibility.Visible;
        }
        #endregion
        #endregion

        void hideAllForms()
        {
            themeSettingsPnl.Visibility = System.Windows.Visibility.Hidden;
        }
        void hideAllTris()
        {
            triThem.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
