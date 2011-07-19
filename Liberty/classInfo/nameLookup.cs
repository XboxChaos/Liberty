using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Liberty.classInfo.storage.settings;
using System.IO;

namespace Liberty.classInfo
{
    class nameLookup
    {
        public static string loadTaglist()
        {
            try
            {
                if (applicationSettings.getLatestTagLst)
                {
                    if (applicationSettings.storeTaglistNoMem)
                    {
                        string temp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Liberty\\taglist\\";
                        Directory.CreateDirectory(temp);
                        File.WriteAllText(temp + "taglst.tgl", applicationExtra.downloadTaglist());
                        classInfo.storage.fileInfoStorage.tagList = Util.TagList.FromFile(temp + "taglst.tgl");
                    }
                    else
                    {
                        classInfo.storage.fileInfoStorage.tagList = Util.TagList.FromString(applicationExtra.downloadTaglist());
                    }
                }
                else
                {
                    if (applicationSettings.storeTaglistNoMem)
                    {
                        string temp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Liberty\\taglist\\";
                        Directory.CreateDirectory(temp);
                        File.WriteAllText(temp + "taglst.tgl", Liberty.Properties.Resources.taglist);
                        classInfo.storage.fileInfoStorage.tagList = Util.TagList.FromFile(temp + "taglst.tgl");
                    }
                    else
                    {
                        classInfo.storage.fileInfoStorage.tagList = Util.TagList.FromString(Liberty.Properties.Resources.taglist);
                    }
                }
                return null;
            }
            catch (Exception exception)
            {
                return exception.ToString();
            }
        }

        public static string translate(uint ident)
        {
            string mapName = loadPackageData.getMapName(storage.fileInfoStorage.saveData.Map);
            if (storage.fileInfoStorage.tagList != null)
            {
                return storage.fileInfoStorage.tagList.Translate(mapName, ident);
            }
            else
            {
                return "0x" + ident.ToString("X");
            }
        }
    }
}
