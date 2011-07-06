using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Liberty.eggData
{
    class egg1Data
    {
        public static bool checkEggExists()
        {
            if (classInfo.extraIO.getMD5HashCatch(eggDirectory()) == "D60D88B5DCAC160D26426C494531609F")
                return true;
            else
                downloadEgg();  return false;
        }

        public static void downloadEgg()
        {
            try
            {
                WebClient wb = new WebClient();
                File.Delete(eggDirectory());
                wb.DownloadFileAsync(new Uri("http://www.xeraxic.com/downloads/0/haloreachliberty//egg/01/egg1.mp3"), eggDirectory());
            }
            catch { }
        }

        public static string eggDirectory()
        {
            string temp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Liberty\\eggs\\1\\";
            try { Directory.CreateDirectory(temp); } catch { }
            temp = temp + "egg.mp3";
            return temp;
        }
    }
}
