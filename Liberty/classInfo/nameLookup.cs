using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.classInfo
{
    class nameLookup
    {
        public static string loadTaglist()
        {
            try
            {
                classInfo.storage.fileInfoStorage.tagList = Util.TagList.FromString(classInfo.applicationExtra.downloadTaglist());
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
