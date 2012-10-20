using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.Util
{
    public enum SaveType
    {
        Unknown,

        Halo3,
        Halo3ODST,
        Reach,
        Anniversary,
        Halo4,

        SomeGame
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

                case Halo3TitleID:
                    if (package.GetFile("mmiof.bmf") != null)
                        return SaveType.Halo3;
                    break;
                case Halo3ODSTTitleID:
                    if (package.GetFile("mmiof.bmf") != null)
                        return SaveType.Halo3ODST;
                    break;

                case Halo4TitleID:
                    if (package.GetFile("mmiof.bmf") != null)
                        return SaveType.Halo4;
                    break;
            }
            return SaveType.Unknown;
        }

        private const uint Halo3TitleID = 0x4D5307E6;
        private const uint Halo3ODSTTitleID = 0x4D530877;
        private const uint ReachTitleID = 0x4D53085B;
        private const uint AnniversaryID = 0x4D5309B1;
        private const uint Halo4TitleID = 0x4D530919;
    }
}
