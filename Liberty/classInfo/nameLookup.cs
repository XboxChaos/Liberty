using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Liberty.classInfo.storage.settings;
using System.IO;
using Liberty.Util;

namespace Liberty.classInfo
{
    class nameLookup
    {
        public static TagList loadTaglist()
        {
            if (applicationSettings.getLatestTagLst)
            {
                return INITagList.LoadFromString(applicationExtra.downloadTaglists());
            }
            else
            {
                return INITagList.LoadFromString(Liberty.Properties.Resources.taglists);
            }
        }

        public static void loadAscensionTaglist(SaveManager<Reach.CampaignSave> saveManager, Reach.TagListManager taglistManager)
        {
            if (applicationSettings.extTaglistFrmAsc && !string.IsNullOrWhiteSpace(applicationSettings.extTaglistFromAscDirec))
            {
                string mapName = saveManager.SaveData.Map;
                mapName = mapName.Substring(mapName.LastIndexOf('\\') + 1);
                string fileName = applicationSettings.extTaglistFromAscDirec + "\\" + mapName + ".taglist";
                taglistManager.RemoveMapSpecificTaglists();
                if (File.Exists(fileName))
                    taglistManager.AddMapSpecificTaglist(AscensionTagList.LoadFromFile(fileName));
            }
        }
    }
}
