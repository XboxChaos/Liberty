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
                        classInfo.storage.fileInfoStorage.tagList = Util.TagList.FromFile(temp + "taglst.tgl", Util.TagListMode.Liberty);
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
                        classInfo.storage.fileInfoStorage.tagList = Util.TagList.FromFile(temp + "taglst.tgl", Util.TagListMode.Liberty);
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

        public static string loadAscensionTaglist()
        {
            try
            {
                if (applicationSettings.extTaglistFrmAsc && !string.IsNullOrWhiteSpace(applicationSettings.extTaglistFromAscDirec))
                {
                    string mapName = loadPackageData.getMapName(storage.fileInfoStorage.saveData.Map);
                    string fileName = applicationSettings.extTaglistFromAscDirec + "\\" + mapName + ".taglist";
                    if (File.Exists(fileName))
                        storage.fileInfoStorage.ascTagList = Util.TagList.FromFile(fileName, Util.TagListMode.Ascension);
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
                string name = storage.fileInfoStorage.tagList.Translate(mapName, ident);
                if (name != null)
                    return name;
            }

            if (storage.fileInfoStorage.ascTagList != null)
            {
                string name = storage.fileInfoStorage.ascTagList.Translate(mapName, ident);
                if (name != null)
                {
                    if (name.StartsWith("objects\\"))
                        return name.Substring(8);
                    else
                        return name;
                }
            }
            
            return "0x" + ident.ToString("X");
        }
    }
}
