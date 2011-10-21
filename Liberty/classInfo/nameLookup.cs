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

        public static string loadAscensionTaglist(Util.SaveEditor saveEditor)
        {
            try
            {
                if (applicationSettings.extTaglistFrmAsc && !string.IsNullOrWhiteSpace(applicationSettings.extTaglistFromAscDirec))
                {
                    string fileName = applicationSettings.extTaglistFromAscDirec + "\\" + saveEditor.MapName + ".taglist";
                    if (File.Exists(fileName))
                        saveEditor.AddTaglist(Util.TagList.FromFile(fileName, Util.TagListMode.Ascension));
                }

                return null;
            }
            catch (Exception exception)
            {
                return exception.ToString();
            }
        }

        private static bool objectGuessMakesSense(string name, Reach.GameObject obj)
        {
            return true;
        }
    }
}
