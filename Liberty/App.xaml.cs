using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;
using Liberty.classInfo.storage.settings;
using Microsoft.Win32;
using System.Reflection;

namespace Liberty
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            RegistryKey keyApp = Registry.CurrentUser.CreateSubKey("Software\\Xeraxic\\Liberty\\");

            // Update
            string isFirstRun = (string)keyApp.GetValue("Version");
            if (isFirstRun != Assembly.GetExecutingAssembly().GetName().Version.ToString())
            {
                keyApp.SetValue("Version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            }

            classInfo.applicationExtra.loadApplicationSettings();

            if (applicationSettings.checkDLL && (!File.Exists("X360.dll") || !File.Exists("MahApps.Metro.dll") || !File.Exists("MahApps.Metro.Controls.dll") || !File.Exists("System.Windows.Interactivity.dll")))
            {
                Controls.dllMissingError dllMissing = new Controls.dllMissingError();
                dllMissing.ShowDialog();
                this.Shutdown();
            }

            if (applicationSettings.displaySplash)
            {
                SplashScreen splash = new SplashScreen();
                splash.ShowDialog();
            }
        }
    }
}
