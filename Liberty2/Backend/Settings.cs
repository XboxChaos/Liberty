using Liberty.Metro.Controls.PageTemplates;
using Liberty.Metro.Dialogs;
using Liberty.SaveManager;
using Liberty.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows;

namespace Liberty.Backend
{
    public class Settings
    {
        public static void LoadSettings(bool applyThemeAswell = false)
        {
            // Declare Registry
            RegistryKey keyApp = Registry.CurrentUser.CreateSubKey("Software\\Xeraxic\\Liberty\\ApplicationSettings\\");
            // Create a JSON Seralizer
            JavaScriptSerializer jss = new JavaScriptSerializer();

            applicationAccent = (Accents)keyApp.GetValue("accent", 0);
            applicationEasterEggs = Convert.ToBoolean(keyApp.GetValue("easterEggs", true));
            if (applyThemeAswell)
                ApplyAccent();

            applicationRecents = jss.Deserialize<List<RecentFileEntry>>(keyApp.GetValue("RecentFiles", "").ToString());
            applicationSizeWidth = Convert.ToSingle(keyApp.GetValue("SizeWidth", 1100));
            applicationSizeHeight = Convert.ToSingle(keyApp.GetValue("SizeHeight", 600));
            applicationSizeMaximize = Convert.ToBoolean(keyApp.GetValue("SizeMaxamize", false));

            XDKNameIP = keyApp.GetValue("XDKNameIP", "192.168.1.0").ToString();
        }
        public static void UpdateSettings(bool applyThemeAswell = false)
        {
            // Declare Registry
            RegistryKey keyApp = Registry.CurrentUser.CreateSubKey("Software\\Xeraxic\\Liberty\\ApplicationSettings\\");
            // Create a JSON Seralizer
            JavaScriptSerializer jss = new JavaScriptSerializer();

            keyApp.SetValue("accent", (int)applicationAccent);
            keyApp.SetValue("easterEggs", applicationEasterEggs);
            if (applyThemeAswell)
                ApplyAccent();

            keyApp.SetValue("RecentFiles", jss.Serialize(applicationRecents));
            keyApp.SetValue("SizeWidth", (double)applicationSizeWidth);
            keyApp.SetValue("SizeHeight", (double)applicationSizeHeight);
            keyApp.SetValue("SizeMaxamize", applicationSizeMaximize);

            keyApp.SetValue("XDKNameIP", XDKNameIP);

            //// Update Windows 7/8 Jumplists
            //JumpLists.UpdateJumplists();

            //// Update File Defaults
            //FileDefaults.UpdateFileDefaults();
        }


        public static void ApplyAccent()
        {
            string theme = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Enum.Parse(typeof(Accents), applicationAccent.ToString()).ToString());
            try
            {
                App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Liberty;component/Metro/Themes/" + theme + ".xaml", UriKind.Relative) });
            }
            catch
            {
                App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Liberty;component/Metro/Themes/Blue.xaml", UriKind.Relative) });
            }
        }
        public static Accents applicationAccent = Accents.Blue;
        public static bool applicationEasterEggs = true;

        public static List<RecentFileEntry> applicationRecents = new List<RecentFileEntry>();

        public static double applicationSizeWidth = 1100;
        public static double applicationSizeHeight = 600;
        public static bool applicationSizeMaximize = false;
        
        public static string XDKNameIP = "";

        public static Home HomeWindow = null;
        public static StartPage StartPage = null;
        public static IList<string> OpenedSaves = new List<string>();

        public enum Accents
        {
            Blue,
            Purple,
            Orange,
            Green
        }
        public class RecentFileEntry
        {
            public string FileName { get; set; }
            public Utilities.HaloGames FileGame { get; set; }
            public string FilePath { get; set; }
            public bool FileIsFromFATX { get; set; }
        }
    }

    public class TempStorage
    {
        public static MetroMessageBox.MessageBoxResults MessageBoxButtonStorage;
    }

    //public class RecentFiles
    //{
    //    public static void AddNewEntry(string filename, string filepath, string game, Settings.RecentFileType type)
    //    {
    //        Settings.RecentFileEntry alreadyExistsEntry = null;

    //        if (Settings.applicationRecents == null)
    //            Settings.applicationRecents = new List<Settings.RecentFileEntry>();

    //        foreach (Settings.RecentFileEntry entry in Settings.applicationRecents)
    //            if (entry.FileName == filename && entry.FilePath == filepath && entry.FileGame == game)
    //                alreadyExistsEntry = entry;

    //        if (alreadyExistsEntry == null)
    //        {
    //            // Add New Entry
    //            Settings.RecentFileEntry newEntry = new Settings.RecentFileEntry()
    //            {
    //                FileGame = game,
    //                FileName = filename,
    //                FilePath = filepath,
    //                FileType = type
    //            };
    //            Settings.applicationRecents.Insert(0, newEntry);
    //        }
    //        else
    //        {
    //            // Move existing Entry
    //            Settings.applicationRecents.Remove(alreadyExistsEntry);
    //            Settings.applicationRecents.Insert(0, alreadyExistsEntry);
    //        }

    //        Settings.UpdateSettings();
    //    }

    //    public static void RemoveEntry(Settings.RecentFileEntry entry)
    //    {
    //        Settings.applicationRecents.Remove(entry);
    //        Settings.UpdateSettings();
    //    }
    //}
}