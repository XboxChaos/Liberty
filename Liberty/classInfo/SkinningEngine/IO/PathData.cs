using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.classInfo.SkinningEngine.IO
{
    class PathData
    {
        public static string loadIniPath()
        {
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Xeraxic\\Liberty\\Skinning\\stepForms");
            string currentINI = Convert.ToString(key.GetValue("currentskin", null));

            if (currentINI != null)
            {
                if (System.IO.File.Exists(currentINI)) { return currentINI; }
                else { return null; }
            }
            else
            {
                return null;
            }
        }

        public static string createTempThemeINI()
        {
            Random ran = new Random();

            string temp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Liberty\\themes\\create\\";
            string newPath = null;
            bool isOld = false;
            while (isOld == false)
            {
                newPath = temp + "theme" + ran.Next(0, 9999) + ".ini";
                if (System.IO.File.Exists(newPath)) { isOld = false; }
                else { isOld = true; }
            }

            return newPath;
        }
    }
}