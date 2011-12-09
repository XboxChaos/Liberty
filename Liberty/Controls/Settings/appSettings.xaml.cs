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
using System.IO;
using System.Reflection;

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

            // Do Build Data
            FileInfo fi = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            DateTime dt = fi.CreationTime;
            appBuildData.Text = string.Format("Build Version:    {0}" + Environment.NewLine +
                "Build Date:       {1}",
                Assembly.GetExecutingAssembly().GetName().Version.ToString(), dt.ToString(System.Globalization.CultureInfo.InvariantCulture));

            // TODO: Fix this when we add a new dll
            //      -Xerax
            string[][] depend = new string[3][];
            depend[0] = new string[] { "X360", "DJ Shepard", "404.0 kb", "GPLv2" };
            depend[1] = new string[] { "System.Windows.Interactivity", "Microsoft", "39.0 kb", "Closed Source" };
            depend[2] = new string[] { "Microsoft.Expression.Drawing", "Microsoft", "120.0 kb", "Closed Source" };

            appExternal.Text = "";
            foreach (string[] tmp in depend)
            {
                string adding = string.Format("Dependency Name:     {0} " + Environment.NewLine +
                                              "Dependency Author:   {1} " + Environment.NewLine +
                                              "Dependency Size:       {2} " + Environment.NewLine +
                                              "Dependency License:  {3} " + Environment.NewLine + Environment.NewLine + Environment.NewLine,

                              tmp[0], tmp[1], tmp[2], tmp[3]);

                appExternal.Text = appExternal.Text + adding;
            }
            appExternal.Text = appExternal.Text.Remove(appExternal.Text.Length - 7);
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
            btnfindTaglist.IsEnabled = (bool)TLTExtAscTaglst.IsChecked;
        }

        public void saveSettings(Util.SaveManager<Reach.CampaignSave> saveManager, Reach.TagListManager taglistManager)
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

            if (classInfo.storage.settings.applicationSettings.extTaglistFromAscDirec != TLTAsvTagLstDirec.Text && saveManager != null)
                classInfo.nameLookup.loadAscensionTaglist(saveManager, taglistManager);

            classInfo.storage.settings.applicationSettings.enableEasterEggs = (bool)LNSenableEggs.IsChecked;
            classInfo.storage.settings.applicationSettings.noWarnings = (bool)LNSnoWarnings.IsChecked;
            classInfo.storage.settings.applicationSettings.lookUpObjectTypes = (bool)TLTtaglstUseTypes.IsChecked;
        }

        #region uncleanWPFshit
        #region btnUpdate
        private void btnUpdateSettings_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnUpdateSettings.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextMid);
        }
        private void btnUpdateSettings_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnUpdateSettings.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextDark);
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
            btnLaunchSettings.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextMid);
        }
        private void btnLaunchSettings_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnLaunchSettings.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextDark);
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
            btnTaglistSettings.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextMid);
        }
        private void btnTaglistSettings_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnTaglistSettings.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextDark);
        }
        private void btnTaglistSettings_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            hideAllTris();
            hideAllForms();
            triTaglist.Visibility = Visibility.Visible;
            taglistSettings.Visibility = Visibility.Visible;
        }
        #endregion

        #region TLTExtAscTaglst
        private void TLTExtAscTaglst_Checked(object sender, RoutedEventArgs e)
        {
            TLTAsvTagLstDirec.IsEnabled = true;
            btnfindTaglist.IsEnabled = true;
        }

        private void TLTExtAscTaglst_Unchecked(object sender, RoutedEventArgs e)
        {
            TLTAsvTagLstDirec.IsEnabled = false;
            btnfindTaglist.IsEnabled = false;
        }
        #endregion

        #region btnappAbout
        private void btnAppAbout_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnAppAbout.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextMid);
        }
        private void btnAppAbout_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnAppAbout.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextDark);
        }
        private void btnAppAbout_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            hideAllTris();
            hideAllForms();
            triAboutApp.Visibility = Visibility.Visible;
            appAbout.Visibility = Visibility.Visible;
        }
        #endregion

        void hideAllForms()
        {
            updateSettings.Visibility = Visibility.Hidden;
            launchSettings.Visibility = Visibility.Hidden;
            taglistSettings.Visibility = Visibility.Hidden;
            appAbout.Visibility = Visibility.Hidden;
        }
        void hideAllTris()
        {
            triLaunch.Visibility = Visibility.Hidden;
            triTaglist.Visibility = Visibility.Hidden;
            triUpd.Visibility = Visibility.Hidden;
            triAboutApp.Visibility = Visibility.Hidden;
        }
        #endregion


        // Now some bullshit -_-
        private void LNSsplashTime_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (LNSsplashTimelbl != null)
                LNSsplashTimelbl.Content = "Display Splash for: " + (int)Math.Round(e.NewValue);
        }

        private void btnfindTaglist_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "Select the directory where your Ascension taglists are located:";
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
                TLTAsvTagLstDirec.Text = dialog.SelectedPath;
        }
    }
}
