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
        public static Util.TagList loadTaglist()
        {
            if (applicationSettings.getLatestTagLst)
            {
                return Util.TagList.FromString(applicationExtra.downloadTaglist());
            }
            else
            {
                return Util.TagList.FromString(Liberty.Properties.Resources.taglist);
            }
        }

        public static string loadAscensionTaglist(Util.SaveManager saveManager)
        {
            try
            {
                if (applicationSettings.extTaglistFrmAsc && !string.IsNullOrWhiteSpace(applicationSettings.extTaglistFromAscDirec))
                {
                    string mapName = saveManager.SaveData.Map;
                    mapName = mapName.Substring(mapName.LastIndexOf('\\') + 1);
                    string fileName = applicationSettings.extTaglistFromAscDirec + "\\" + mapName + ".taglist";
                    saveManager.RemoveMapSpecificTaglists();
                    if (File.Exists(fileName))
                        saveManager.AddMapSpecificTaglist(Util.TagList.FromFile(fileName, Util.TagListMode.Ascension));
                }

                return null;
            }
            catch (Exception exception)
            {
                return exception.ToString();
            }
        }
    }
}
