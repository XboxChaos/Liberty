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

namespace Liberty.classInfo
{
    class applicationExtra
    {
        public static void closeApplication()
        {
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                if (process.ProcessName == "Liberty" || process.ProcessName == "Liberty.vshost")
                {
                    process.Kill();
                }
            }
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
    }
}
