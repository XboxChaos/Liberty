using Liberty.Backend;
using Liberty.Metro.Dialogs;
using Liberty.SaveManager;
using Microsoft.Shell;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Liberty
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            if (Settings.HomeWindow != null)
                return true; //Settings.HomeWindow.ProcessCommandLineArgs(args); // TODO: Add Input Command Line Processing to the Home Window
            else
                return true;
        }

        //[STAThread]
        //public static void Main()
        //{
            
        //}

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Load Supported Builds
            Manager.SupportedGame = new Manager.SupportedGames();

            // Load Settings
            Settings.LoadSettings(true);

            // Update Liberty Protocol
            LibertyProtocol.UpdateProtocol();

            // Create Temporary Directories
            VariousFunctions.CreateTemporaryDirectories();

            // Clean-up Old Temporary Files
            VariousFunctions.CleanUpTemporaryFiles();

            // Create Closing Method
            Application.Current.Exit += (o, args) =>
            {
                // Update Settings with Window Width/Height
                Settings.applicationSizeMaximize = (Settings.HomeWindow.WindowState == WindowState.Maximized);
                if (!Settings.applicationSizeMaximize)
                {
                    Settings.applicationSizeWidth = Settings.HomeWindow.Width;
                    Settings.applicationSizeHeight = Settings.HomeWindow.Height;
                }

                // Save Settings
                Settings.UpdateSettings();
            };

            // Global Exception Catching
#if !DEBUG
            Application.Current.DispatcherUnhandledException += (o, args) =>
                {
                    MetroException.Show((Exception)args.Exception);

                    args.Handled = true;
                };
#endif
        }
    }
}