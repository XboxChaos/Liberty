using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.Util
{
    public enum SaveType
    {
        Unknown,
        Reach,
        Anniversary
    }

    public sealed class GameID
    {
        /// <summary>
        /// Identifies the game which owns a package.
        /// </summary>
        /// <param name="package">The STFSPackage to examine</param>
        /// <returns>The game that the save belongs to</returns>
        public static SaveType IdentifyGame(X360.STFS.STFSPackage package)
        {
            switch (package.Header.TitleID)
            {
                case ReachTitleID:
                    if (package.GetFile("mmiof.bmf") != null)
                        return SaveType.Reach;
                    break;

                case AnniversaryID:
                    if (package.GetFile("saves.cfg") != null)
                        return SaveType.Anniversary;
                    break;
            }
            return SaveType.Unknown;
        }

        private const uint ReachTitleID = 0x4D53085B;
        private const uint AnniversaryID = 0x4D5309B1;
    }
}
