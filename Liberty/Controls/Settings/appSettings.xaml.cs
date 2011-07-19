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
using Liberty.classInfo;

namespace Liberty.Controls.Settings
{
    /// <summary>
    /// Interaction logic for appSettings.xaml
    /// </summary>
    public partial class appSettings : UserControl
    {
        BrushConverter bc = new BrushConverter();

        public appSettings()
        {
            InitializeComponent();

            hideAllTris();
            btnUpdateSettings_MouseDown(null, null);
        }

        public void loadSettings()
        {
            // Load from RegTable
            RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Xeraxic\\Liberty\\appSettings\\");
                
                // Update
                UPScheckOLHeader.IsChecked = applicationExtra.settingsConvertIntToBool((int)key.GetValue("updOnLaunch", 1));
                UPSshowChangeLog.IsChecked = applicationExtra.settingsConvertIntToBool((int)key.GetValue("updChnLog", 1));

                // Application
                LNSdisplaySplash.IsChecked = applicationExtra.settingsConvertIntToBool((int)key.GetValue("appDisSplash", 1));
                LNScheckDLL.IsChecked = applicationExtra.settingsConvertIntToBool((int)key.GetValue("appMsgDll", 1));
                LNSenableEggs.IsChecked = applicationExtra.settingsConvertIntToBool((int)key.GetValue("appEstEgg", 1));
                LNSausFileType.IsChecked = applicationExtra.settingsConvertIntToBool((int)key.GetValue("appAssFileType", 1));
                LNSsplashTime.Value = (int)key.GetValue("appSplashTime", 5);

                // Taglist
                TLTdlLatestTaglst.IsChecked = applicationExtra.settingsConvertIntToBool((int)key.GetValue("appDLTagLst", 1));
                TLTtaglstNoMem.IsChecked = applicationExtra.settingsConvertIntToBool((int)key.GetValue("appTgLstNoMem", 1));
                TLTExtAscTaglst.IsChecked = applicationExtra.settingsConvertIntToBool((int)key.GetValue("appTglstFromAsc", 1));
                TLTAsvTagLstDirec.Text = (string)key.GetValue("appTglstFromAscDirec", "");

            // Update UI on changes
            LNSsplashTimelbl.Content = "Display Splash for: " + LNSsplashTime.Value;
        }
        public void saveSettings()
        {
            // Save to RegTable
            RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Xeraxic\\Liberty\\appSettings");

                // Update
                key.SetValue("updOnLaunch", applicationExtra.settingsConvertBoolToInt(UPScheckOLHeader));
                key.SetValue("updChnLog", applicationExtra.settingsConvertBoolToInt(UPSshowChangeLog));

                // Application
                key.SetValue("appDisSplash", applicationExtra.settingsConvertBoolToInt(LNSdisplaySplash));
                key.SetValue("appMsgDll", applicationExtra.settingsConvertBoolToInt(LNScheckDLL));
                key.SetValue("appEstEgg", applicationExtra.settingsConvertBoolToInt(LNSenableEggs));
                key.SetValue("appAssFileType", applicationExtra.settingsConvertBoolToInt(LNSausFileType));
                key.SetValue("updChnLog", LNSsplashTime.Value);

                // Taglist
                key.SetValue("appDLTagLst", applicationExtra.settingsConvertBoolToInt(TLTdlLatestTaglst));
                key.SetValue("appTgLstNoMem", applicationExtra.settingsConvertBoolToInt(TLTtaglstNoMem));
                key.SetValue("appTglstFromAsc", applicationExtra.settingsConvertBoolToInt(TLTExtAscTaglst));
                key.SetValue("appTglstFromAsc", TLTAsvTagLstDirec.Text);
        }

        #region uncleanWPFshit
        #region btnUpdate
        private void btnUpdateSettings_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnUpdateSettings.Foreground = (Brush)bc.ConvertFrom("#828689");
        }

        private void btnUpdateSettings_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnUpdateSettings.Foreground = (Brush)bc.ConvertFrom("#FF000000");
        }

        private void btnUpdateSettings_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            hideAllTris();
            hideAllForms();
            triUpd.Visibility = System.Windows.Visibility.Visible;
            updateSettings.Visibility = System.Windows.Visibility.Visible;
        }
        #endregion

        #region btnLaunch
        private void btnLaunchSettings_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnLaunchSettings.Foreground = (Brush)bc.ConvertFrom("#828689");
        }

        private void btnLaunchSettings_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnLaunchSettings.Foreground = (Brush)bc.ConvertFrom("#FF000000");
        }

        private void btnLaunchSettings_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            hideAllTris();
            hideAllForms();
            triLaunch.Visibility = System.Windows.Visibility.Visible;
            launchSettings.Visibility = System.Windows.Visibility.Visible;
        }
        #endregion

        #region btnTaglist
        private void btnTaglistSettings_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnTaglistSettings.Foreground = (Brush)bc.ConvertFrom("#828689");
        }

        private void btnTaglistSettings_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnTaglistSettings.Foreground = (Brush)bc.ConvertFrom("#FF000000");
        }

        private void btnTaglistSettings_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            hideAllTris();
            hideAllForms();
            triTaglist.Visibility = System.Windows.Visibility.Visible;
            taglistSettings.Visibility = System.Windows.Visibility.Visible;
        }
        #endregion

        #region brnTaglistFind
        private void btnFind_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnFind.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnFind.Source = new BitmapImage(source);
            }
        }

        private void btnFind_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnFind.Source = new BitmapImage(source);
        }

        private void btnFind_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
            btnFind.Source = new BitmapImage(source);

            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            TLTAsvTagLstDirec.Text = dialog.SelectedPath;
        }
        #endregion

        void hideAllForms()
        {
            updateSettings.Visibility = System.Windows.Visibility.Hidden;
            launchSettings.Visibility = System.Windows.Visibility.Hidden;
            taglistSettings.Visibility = System.Windows.Visibility.Hidden;
        }
        void hideAllTris()
        {
            triLaunch.Visibility = System.Windows.Visibility.Hidden;
            triTaglist.Visibility = System.Windows.Visibility.Hidden;
            triUpd.Visibility = System.Windows.Visibility.Hidden;
        }
        #endregion


        // Now some bullshit -_-
        private void LNSsplashTime_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (LNSsplashTimelbl != null)
                LNSsplashTimelbl.Content = "Display Splash for: " + (int)Math.Round(e.NewValue);
        }
    }
}
