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
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Liberty
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public string initException = null;
        public string descData = "";
        public int svrBuild = 0;
        public int pcBuild = 0;
        public Util.TagList tagList = null;

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

            if (!File.Exists("X360.dll") || !File.Exists("System.Windows.Interactivity.dll") || !File.Exists("Microsoft.Expression.Drawing.dll"))
            {
                Controls.dllMissingError dllMissing = new Controls.dllMissingError();
                dllMissing.ShowDialog();
                this.Shutdown();
            }

            // Initialize while the splash screen (if any) is running
            Task initTask = Task.Factory.StartNew(initialize);

            if (applicationSettings.displaySplash)
            {
                SplashScreen splash = new SplashScreen();
                splash.ShowDialog();
            }
            try
            {
                initTask.Wait();
            }
            catch (AggregateException exception)
            {
                // An exception occurred during initialization
                initException = exception.InnerException.ToString();
            }
            catch (Exception exception)
            {
                initException = exception.ToString();
            }
        }

        private void initialize()
        {
            classInfo.applicationExtra.cleanUpOldSaves();

            if (applicationSettings.checkUpdatesOL)
            {
                try
                {
                    Dns.GetHostEntry("xeraxic.com");
                    WebClient wb = new WebClient();
                    string downloadedInfo = wb.DownloadString(new Uri("http://xeraxic.com/downloads/checkVersionInfo.php?appName=Liberty&proDesc=1"));
                    downloadedInfo = downloadedInfo.Replace("\r", "");
                    string[] updateData = downloadedInfo.Split('\n');
                    svrBuild = Convert.ToInt16(updateData[0].Replace(".", ""));
                    pcBuild = Convert.ToInt16(Assembly.GetExecutingAssembly().GetName().Version.ToString().Replace(".", ""));

                    if (applicationSettings.showChangeLog)
                    {
                        int i = 0;
                        foreach (string line in updateData)
                        {
                            if (i == 0) { }
                            else { descData += line + "\n"; }
                            i++;
                        }
                    }
                }
                catch
                {
                }
            }

            tagList = classInfo.nameLookup.loadTaglist();
        }
    }
}
