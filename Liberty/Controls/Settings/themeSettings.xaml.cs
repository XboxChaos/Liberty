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
        public int lastLoadThemeID = -1;
        public int lastLoadAccentID = -1;

        public themeSettings()
        {
            InitializeComponent();

            hideAllTris();
            btnThemeSettings_MouseDown(null, null);
        }

        public void loadSettings()
        {
            // Load from RegTable
            RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Xeraxic\\MetroThemes\\");

            int accentColour = (int)key.GetValue("accentColour", 1);
            int themeColour = (int)key.GetValue("themeColour", 1);

            rbAccent1.IsChecked = false;
            rbAccent2.IsChecked = false;
            rbAccent3.IsChecked = false;
            rbAccent4.IsChecked = false;
            rbAccent5.IsChecked = false;
            rbAccent6.IsChecked = false;
            rbAccent7.IsChecked = false;
            rbAccent8.IsChecked = false;

            rbLightTheme.IsChecked = false;
            rbDarkTheme.IsChecked = false;

            switch (themeColour)
            {
                case 1:
                    rbLightTheme.IsChecked = true;
                    break;
                case 2:
                    rbDarkTheme.IsChecked = true;
                    break;
            }
            lastLoadThemeID = themeColour;

            switch (accentColour)
            {
                case 1:
                    rbAccent1.IsChecked = true;
                    break;
                case 2:
                    rbAccent2.IsChecked = true;
                    break;
                case 3:
                    rbAccent3.IsChecked = true;
                    break;
                case 4:
                    rbAccent4.IsChecked = true;
                    break;
                case 5:
                    rbAccent5.IsChecked = true;
                    break;
                case 6:
                    rbAccent6.IsChecked = true;
                    break;
                case 7:
                    rbAccent7.IsChecked = true;
                    break;
                case 8:
                    rbAccent8.IsChecked = true;
                    break;
                default:
                    rbAccent1.IsChecked = true;
                    break;
            }
            lastLoadAccentID = accentColour;

        }
        public void saveSettings()
        {
            // Save to RegTable
            RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Xeraxic\\MetroThemes\\");
            int AccentColour = 1;
            int ThemeColour = 1;

            if (rbAccent1.IsChecked == true)
                AccentColour = 1;
            else if (rbAccent2.IsChecked == true)
                AccentColour = 2;
            else if (rbAccent3.IsChecked == true)
                AccentColour = 3;
            else if (rbAccent4.IsChecked == true)
                AccentColour = 4;
            else if (rbAccent5.IsChecked == true)
                AccentColour = 5;
            else if (rbAccent6.IsChecked == true)
                AccentColour = 6;
            else if (rbAccent7.IsChecked == true)
                AccentColour = 7;
            else if (rbAccent8.IsChecked == true)
                AccentColour = 8;

            if (rbLightTheme.IsChecked == true)
                ThemeColour = 1;
            else if (rbDarkTheme.IsChecked == true)
                ThemeColour = 2;

            key.SetValue("accentColour", AccentColour);
            key.SetValue("themeColour", ThemeColour);
        }

        #region uncleanWPFshit
        #region btnUpdate
        private void btnThemeSettings_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            hideAllTris();
            triThem.Visibility = Visibility.Visible;
            themeSettingsPnl.Visibility = Visibility.Visible;
        }
        #endregion

        #region realTime
        private void theme_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton cb = (RadioButton)sender;

            if (cb.Content != null && cb.Content.ToString().EndsWith(")"))
            {
                string theme = cb.Content.ToString().Remove(5);
                theme = theme.Replace(" ", "");

                ResourceDictionary rd = new ResourceDictionary { Source = new Uri("Themes/Colour/"+theme+".xaml", UriKind.Relative) };
                App.Current.Resources.MergedDictionaries.Add(rd);
            }
            else if (cb.Content != null)
            {
                string accent = cb.Content.ToString().Replace("Liberty ", "");

                ResourceDictionary rd = new ResourceDictionary { Source = new Uri("Themes/Accents/" + accent + ".xaml", UriKind.Relative) };
                App.Current.Resources.MergedDictionaries.Add(rd);
            }
        }
        #endregion
        #endregion

        void hideAllForms()
        {
            themeSettingsPnl.Visibility = Visibility.Hidden;
        }
        void hideAllTris()
        {
            triThem.Visibility = Visibility.Hidden;
        }
    }
}
