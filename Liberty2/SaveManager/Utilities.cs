using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.SaveManager
{
    public class Utilities
    {
        public enum HaloGames
        {
            Halo3,
            Halo3_ODST,
            HaloReach,
            HaloCEX,
            Halo4
        }
        
        public static string GetFriendlyMissionName(string mapScenario, HaloGames gameIdent)
        {
            mapScenario = mapScenario.Substring(mapScenario.LastIndexOf('\\') + 1).ToLower();

            switch (gameIdent)
            {
                #region Halo 3
                case HaloGames.Halo3:
                    switch (mapScenario)
                    {
                        case @"005_intro":
                            return "Arrival";

                        case @"010_jungle":
                            return "Sierra 117";

                        case @"020_base":
                            return "Crow's Nest";

                        case @"030_outskirts":
                            return "Tsavo Highway";

                        case @"040_voi":
                            return "The Storm";

                        case @"050_floodvoi":
                            return "Floodgate";

                        case @"070_waste":
                            return "The Ark";

                        case @"100_citadel":
                            return "The Covenant";

                        case @"110_hc":
                            return "Cortana";

                        case @"120_halo":
                            return "Halo";

                        default:
                            return mapScenario;
                    }
                #endregion

                #region Halo 3: ODST
                case HaloGames.Halo3_ODST:
                    switch (mapScenario)
                    {
                        case @"c100":
                            return "Prepare To Drop";

                        case @"c200":
                            return "Coastal Highway";

                        case @"h100":
                            return "Mombassa Streets";

                        case @"l200":
                            return "Data Hive";

                        case @"l300":
                            return "Coastal Highway";

                        case @"sc100":
                            return "Tayari Plaza";

                        case @"sc110":
                            return "Uplift Reserve";

                        case @"sc120":
                            return "Kizingo Blvd.";

                        case @"sc130":
                            return "Oni Alpha Base";

                        case @"sc140":
                            return "NMPD HQ";

                        case @"sc150":
                            return "Kikiwani Station";

                        default:
                            return mapScenario;
                    }
                #endregion

                #region Halo Reach
                case HaloGames.HaloReach:
                    switch (mapScenario)
                    {
                        case @"m05":
                            return "Noble Actual";

                        case @"m10":
                            return "Winter Contingency";

                        case @"m20":
                            return "ONI: Sword Base";

                        case @"m30":
                            return "Nightfall";

                        case @"m35":
                            return "Tip of the Spear";

                        case @"m45":
                            return "Long Night of Solace";

                        case @"m50":
                            return "Exodus";

                        case @"m52":
                            return "New Alexandria";

                        case @"m60":
                            return "The Package";

                        case @"m70":
                            return "The Pillar of Autumn";

                        case @"m70_a":
                            return "Credits";

                        case @"m70_bonus":
                            return "Lone Wolf";

                        default:
                            return mapScenario;
                    }
                #endregion

                #region Halo Anniversary
                case HaloGames.HaloCEX:
                    switch (mapScenario)
                    {
                        case @"a10":
                            return "The Pillar of Autumn";

                        case @"a30":
                            return "Halo";

                        case @"a50":
                            return "The Truth and Reconciliation";

                        case @"b30":
                            return "The Silent Cartographer";

                        case @"b40":
                            return "Assault on the Control Room";

                        case @"c10":
                            return "343 Guilty Spark";

                        case @"c20":
                            return "The Library";

                        case @"c40":
                            return "Two Betrayals";

                        case @"d20":
                            return "Keys";

                        case @"d40":
                            return "The Maw";

                        default:
                            return mapScenario;
                    }
                #endregion

                #region Halo 4
                case HaloGames.Halo4:
                    switch (mapScenario)
                    {
                        case @"m020":
                            return "Requiem";

                        case @"m05_prologue":
                            return "Prologue";

                        case @"m10_crash":
                            return "Dawn";

                        case @"m30_cryptum":
                            return "Forerunner";

                        case @"m40_invasion":
                            return "Infinity";

                        case @"m60_rescue":
                            return "Reclaimer";

                        case @"m70_liftoff":
                            return "Shutdown";

                        case @"m80_delta":
                            return "Composer";

                        case @"m90_sacrifice":
                            return "Midnight";

                        case @"m95_epilogue":
                            return "Epilogue";

                        default:
                            return mapScenario;
                    }
                #endregion

                default:
                    return mapScenario;
            }
        }

        public static string CreateUniqueSaveIdentification(string packageName, string packagePath, bool isOnFATX)
        {
            string unHashedString = string.Format("{0}.{1}.{2}", packageName, packagePath, isOnFATX.ToString());

            return Backend.Cryptography.MD5Crypto.ComputeHashToString(unHashedString);
        }
    }
}
