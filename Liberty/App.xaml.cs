using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;

namespace Liberty
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (File.Exists("X360.dll") || !File.Exists("MahApps.Metro.dll") || !File.Exists("MahApps.Metro.Controls.dll") || !File.Exists("System.Windows.Interactivity.dll"))
            {
                Controls.dllMissingError dllMissing = new Controls.dllMissingError();
                dllMissing.ShowDialog();
                this.Shutdown();
            }
        }
    }
}
