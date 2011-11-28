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
        Anniversary,
        CocksOfDooty
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
                default:
                    uint tmp = package.Header.TitleID;
                    if (tmp == cod2 || tmp == cod3 || tmp == cod4 || tmp == cod5 || tmp == cod6 || tmp == cod7 || tmp == cod8)
                        return SaveType.CocksOfDooty;
                    break;
            }
            return SaveType.Unknown;
        }

        private const uint ReachTitleID = 0x4D53085B;
        private const uint AnniversaryID = 0x4D5309B1;
        
        // The CoD's
        private const uint cod2 = 0x415607D1;
        private const uint cod3 = 0x415607E1;
        private const uint cod4 = 0x415607E6;
        private const uint cod5 = 0x4156081C;
        private const uint cod6 = 0x41560817;
        private const uint cod7 = 0x41560855;
        private const uint cod8 = 0x415608cb;
    }
}
