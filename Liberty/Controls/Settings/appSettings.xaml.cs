﻿using System;
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
        private MainWindow mainWindow = null;
        BrushConverter bc = new BrushConverter();

        public appSettings()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(appSettings_Loaded);

            hideAllTris();
            btnUpdateSettings_MouseDown(null, null);
        }

        void appSettings_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this) as MainWindow;
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
            //LNScheckDLL.IsChecked = applicationExtra.settingsConvertIntToBool((int)key.GetValue("appMsgDll", 1));
            LNSenableEggs.IsChecked = applicationExtra.settingsConvertIntToBool((int)key.GetValue("appEstEgg", 1));
            LNSnoWarnings.IsChecked = applicationExtra.settingsConvertIntToBool((int)key.GetValue("appNoWarnings", 0));
            LNSsplashTime.Value = (int)key.GetValue("appSplashTime", 5);

            // Taglist
            TLTdlLatestTaglst.IsChecked = applicationExtra.settingsConvertIntToBool((int)key.GetValue("appDLTagLst", 1));
            TLTtaglstNoMem.IsChecked = applicationExtra.settingsConvertIntToBool((int)key.GetValue("appTgLstNoMem", 0));
            TLTtaglstUseTypes.IsChecked = applicationExtra.settingsConvertIntToBool((int)key.GetValue("appTgLstUseTypes", 0));
            TLTExtAscTaglst.IsChecked = applicationExtra.settingsConvertIntToBool((int)key.GetValue("appTglstFromAsc", 0));
            TLTAsvTagLstDirec.Text = (string)key.GetValue("appTglstFromAscDirec", "");

            // Update UI on changes
            LNSsplashTimelbl.Content = "Display Splash for: " + LNSsplashTime.Value;
            TLTAsvTagLstDirec.IsEnabled = (bool)TLTExtAscTaglst.IsChecked;
            btnFind.IsEnabled = (bool)TLTExtAscTaglst.IsChecked;
            lblFind.IsEnabled = (bool)TLTExtAscTaglst.IsChecked;
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
            //key.SetValue("appMsgDll", applicationExtra.settingsConvertBoolToInt(LNScheckDLL));
            key.SetValue("appEstEgg", applicationExtra.settingsConvertBoolToInt(LNSenableEggs));
            key.SetValue("appNoWarnings", applicationExtra.settingsConvertBoolToInt(LNSnoWarnings));
            key.SetValue("appSplashTime", (int)LNSsplashTime.Value, RegistryValueKind.DWord);

            // Taglist
            key.SetValue("appDLTagLst", applicationExtra.settingsConvertBoolToInt(TLTdlLatestTaglst));
            key.SetValue("appTgLstNoMem", applicationExtra.settingsConvertBoolToInt(TLTtaglstNoMem));
            key.SetValue("appTgLstUseTypes", applicationExtra.settingsConvertBoolToInt(TLTtaglstUseTypes));
            key.SetValue("appTglstFromAsc", applicationExtra.settingsConvertBoolToInt(TLTExtAscTaglst));
            key.SetValue("appTglstFromAscDirec", TLTAsvTagLstDirec.Text);

            if (classInfo.storage.settings.applicationSettings.extTaglistFromAscDirec != TLTAsvTagLstDirec.Text)
                mainWindow.loadTaglists();

            classInfo.storage.settings.applicationSettings.enableEasterEggs = (bool)LNSenableEggs.IsChecked;
            classInfo.storage.settings.applicationSettings.noWarnings = (bool)LNSnoWarnings.IsChecked;
            classInfo.storage.settings.applicationSettings.lookUpObjectTypes = (bool)TLTtaglstUseTypes.IsChecked;
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
            triUpd.Visibility = Visibility.Visible;
            updateSettings.Visibility = Visibility.Visible;
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
            triLaunch.Visibility = Visibility.Visible;
            launchSettings.Visibility = Visibility.Visible;
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
            triTaglist.Visibility = Visibility.Visible;
            taglistSettings.Visibility = Visibility.Visible;
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
            dialog.Description = "Select the directory where your Ascension taglists are located:";
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
                TLTAsvTagLstDirec.Text = dialog.SelectedPath;
        }
        #endregion

        #region TLTExtAscTaglst
        private void TLTExtAscTaglst_Checked(object sender, RoutedEventArgs e)
        {
            TLTAsvTagLstDirec.IsEnabled = true;
            btnFind.IsEnabled = true;
            lblFind.IsEnabled = true;
        }

        private void TLTExtAscTaglst_Unchecked(object sender, RoutedEventArgs e)
        {
            TLTAsvTagLstDirec.IsEnabled = false;
            btnFind.IsEnabled = false;
            lblFind.IsEnabled = false;
        }
        #endregion

        void hideAllForms()
        {
            updateSettings.Visibility = Visibility.Hidden;
            launchSettings.Visibility = Visibility.Hidden;
            taglistSettings.Visibility = Visibility.Hidden;
        }
        void hideAllTris()
        {
            triLaunch.Visibility = Visibility.Hidden;
            triTaglist.Visibility = Visibility.Hidden;
            triUpd.Visibility = Visibility.Hidden;
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
