/*
* Liberty - http://xboxchaos.com/
*
* Copyright (C) 2011 XboxChaos
* Copyright (C) 2011 ThunderWaffle/AMD
* Copyright (C) 2011 Xeraxic
*
* Liberty is free software; you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published
* by the Free Software Foundation; either version 3 of the License,
* or (at your option) any later version.
*
* Liberty is distributed in the hope that it will be useful, but
* WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
* General Public License for more details.
*
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using FATX;
using System.Windows.Controls;
using Liberty.classInfo.storage.settings;
using Microsoft.Win32;
using System.Windows;
using System.Net;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Media;

namespace Liberty.classInfo
{
    class applicationExtra
    {
        public static void loadApplicationSettings()
        {
            RegistryKey keyApp = Registry.CurrentUser.CreateSubKey("Software\\Xeraxic\\Liberty\\appSettings\\");
            RegistryKey keyTheme = Registry.CurrentUser.CreateSubKey("Software\\Xeraxic\\MetroThemes\\");

            // Update
            applicationSettings.checkUpdatesOL = applicationExtra.settingsConvertIntToBool((int)keyApp.GetValue("updOnLaunch", 1));
            applicationSettings.showChangeLog = applicationExtra.settingsConvertIntToBool((int)keyApp.GetValue("updChnLog", 1));

            // Application
            applicationSettings.displaySplash = applicationExtra.settingsConvertIntToBool((int)keyApp.GetValue("appDisSplash", 1));
            applicationSettings.checkDLL = applicationExtra.settingsConvertIntToBool((int)keyApp.GetValue("appMsgDll", 1));
            applicationSettings.enableEasterEggs = applicationExtra.settingsConvertIntToBool((int)keyApp.GetValue("appEstEgg", 0));
            applicationSettings.noWarnings = applicationExtra.settingsConvertIntToBool((int)keyApp.GetValue("appNoWarnings", 0));
            applicationSettings.splashTimer = (int)keyApp.GetValue("appSplashTime", 5);

            // Taglist
            applicationSettings.getLatestTagLst = applicationExtra.settingsConvertIntToBool((int)keyApp.GetValue("appDLTagLst", 1));
            applicationSettings.storeTaglistNoMem = applicationExtra.settingsConvertIntToBool((int)keyApp.GetValue("appTgLstNoMem", 0));
            applicationSettings.extTaglistFrmAsc = applicationExtra.settingsConvertIntToBool((int)keyApp.GetValue("appTglstFromAsc", 0));
            applicationSettings.extTaglistFromAscDirec = (string)keyApp.GetValue("appTglstFromAscDirec", "");

            // Themes
            applicationSettings.AccentColour = (int)keyTheme.GetValue("accentColour", 1);
            applicationSettings.ThemeColour = (int)keyTheme.GetValue("themeColour", 1);

            updateAccent();
            updateTheme();
        }

        public static void updateTheme()
        {
            ResourceDictionary rd;
            switch (applicationSettings.ThemeColour)
            {
                case 1:
                    rd = new ResourceDictionary { Source = new Uri("Themes/Colour/Light.xaml", UriKind.Relative) };
                    break;
                case 2:
                    rd = new ResourceDictionary { Source = new Uri("Themes/Colour/Dark.xaml", UriKind.Relative) };

                    classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextDark = "#FFCCCCCC";
                    classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextMid = "#FF868686";
                    classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextLight = "#FF949494";
                    classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentControlsBG = "#FF2D2D2D";
                    classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentBG = "#FF333333";

                    break;
                default:
                    rd = new ResourceDictionary { Source = new Uri("Themes/Colour/Light.xaml", UriKind.Relative) };
                    break;
            }

            App.Current.Resources.MergedDictionaries.Add(rd);
        }
        public static void updateAccent()
        {
            ResourceDictionary rd;
            switch (applicationSettings.AccentColour)
            {
                case 1:
                    rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Orange.xaml", UriKind.Relative) };
                    break;
                case 2:
                    rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Blue.xaml", UriKind.Relative) };
                    break;
                case 3:
                    rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Purple.xaml", UriKind.Relative) };
                    break;
                case 4:
                    rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Pink.xaml", UriKind.Relative) };
                    break;
                case 5:
                    rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Red.xaml", UriKind.Relative) };
                    break;
                case 6:
                    rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Green.xaml", UriKind.Relative) };
                    break;
                case 7:
                    rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Lime.xaml", UriKind.Relative) };
                    break;
                case 8:
                    rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Silver.xaml", UriKind.Relative) };
                    break;
                default:
                    rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Orange.xaml", UriKind.Relative) };
                    break;
            }

            App.Current.Resources.MergedDictionaries.Add(rd);
        }

        public static string downloadTaglists()
        {
            try
            {
                Dns.GetHostEntry("xboxchaos.com");
                System.Net.WebClient wb = new System.Net.WebClient();
                return wb.DownloadString("http://www.xboxchaos.com/reach/liberty/taglists.ini");
            } catch { return Properties.Resources.taglists; }
        }

        public static void closeApplication()
        {
            Application.Current.Shutdown();
        }

        public static void cleanUpOldSaves()
        {
            
            string temp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Liberty\\reachSaveBackup";
            if (Directory.Exists(temp))
            {
                string[] oldDirecs = System.IO.Directory.GetDirectories(temp);
                foreach (string direc in oldDirecs)
                {
                    string[] oldFiles = Directory.GetFiles(direc);
                    foreach (string file in oldFiles)
                    {
                        FileInfo fi = new FileInfo(file);
                        fi.Delete();
                    }

                    Directory.Delete(direc);
                }
            }
        }

        public static int settingsConvertBoolToInt(CheckBox cb)
        {
            if ((bool)cb.IsChecked)
                return 1;
            else
                return 0;
        }

        public static bool settingsConvertIntToBool(int regVal)
        {
            if (regVal == 1)
                return true;
            else
                return false;
        }

        public static void disableInput(Window window)
        {
            window.IsHitTestVisible = false;
            window.Focusable = false;
            window.PreviewKeyDown += disableInput_PreviewKeyDown;
        }

        private static void disableInput_PreviewKeyDown(object sender, KeyboardEventArgs e)
        {
            e.Handled = true;
        }

        public static void fitTextBlock(TextBlock textBlock)
        {
            Typeface fontFace = new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch);
            FormattedText text = new FormattedText(textBlock.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, fontFace, textBlock.FontSize, textBlock.Foreground);
            textBlock.Width = text.Width;
        }
    }
}
