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

namespace Liberty.classInfo
{
    class applicationExtra
    {
        public static void loadApplicationSettings()
        {
            RegistryKey keyApp = Registry.CurrentUser.CreateSubKey("Software\\Xeraxic\\Liberty\\appSettings\\");
            RegistryKey keyTheme = Registry.CurrentUser.CreateSubKey("Software\\Xeraxic\\Liberty\\themeSettings\\");

            // Update
            applicationSettings.checkUpdatesOL = applicationExtra.settingsConvertIntToBool((int)keyApp.GetValue("updOnLaunch", 1));
            applicationSettings.showChangeLog = applicationExtra.settingsConvertIntToBool((int)keyApp.GetValue("updChnLog", 1));

            // Application
            applicationSettings.displaySplash = applicationExtra.settingsConvertIntToBool((int)keyApp.GetValue("appDisSplash", 1));
            applicationSettings.checkDLL = applicationExtra.settingsConvertIntToBool((int)keyApp.GetValue("appMsgDll", 1));
            applicationSettings.enableEasterEggs = applicationExtra.settingsConvertIntToBool((int)keyApp.GetValue("appEstEgg", 0));
            applicationSettings.ausFileType = applicationExtra.settingsConvertIntToBool((int)keyApp.GetValue("appAssFileType", 0));
            applicationSettings.splashTimer = (int)keyApp.GetValue("appSplashTime", 5);

            // Taglist
            applicationSettings.getLatestTagLst = applicationExtra.settingsConvertIntToBool((int)keyApp.GetValue("appDLTagLst", 1));
            applicationSettings.storeTaglistNoMem = applicationExtra.settingsConvertIntToBool((int)keyApp.GetValue("appTgLstNoMem", 0));
            applicationSettings.extTaglistFrmAsc = applicationExtra.settingsConvertIntToBool((int)keyApp.GetValue("appTglstFromAsc", 0));
            applicationSettings.extTaglistFromAscDirec = (string)keyApp.GetValue("appTglstFromAscDirec", "");
        }

        public static string downloadTaglist()
        {
            try
            {
                System.Net.WebClient wb = new System.Net.WebClient();
                return wb.DownloadString("http://xboxchaos.com/reach/liberty/taglist.ini");
            } catch { return Properties.Resources.taglist; }
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

        public static string getTempSaveExtraction(FATX.File x)
        {
            Random ran = new Random();
            string temp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Liberty\\reachSaveBackup\\" + ran.Next(0, 1000) + "\\";
            Directory.CreateDirectory(temp);
            temp = temp + x.Name;
            bool cancel = false;
            x.Extract(temp, ref cancel);

            stfsCheck.checkSTFSPackage(temp);

            return temp;
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
    }
}
