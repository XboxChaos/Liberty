/*
* Liberty - http://xboxchaos.com/
*
* Copyright (C) 2011 XboxChaos
* Copyright (C) 2011 ThunderWaffle/AMD
* Copyright (C) 2011 Xeraxic
*
* Liberty is free software; you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published
* by the Free Software Foundation; either version 3 of the License,
* or (at your option) any later version.
*
* Liberty is distributed in the hope that it will be useful, but
* WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
* General Public License for more details.
*
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.classInfo
{
    class loadPackageData
    {
        public static string mapToMissionName(string mapName)
        {
            switch (mapName)
            {
                case @"levels\solo\m05\m05":
                    return "Noble Actual";

                case @"levels\solo\m10\m10":
                    return "Winter Contingency";

                case @"levels\solo\m20\m20":
                    return "ONI: Sword Base";

                case @"levels\solo\m30\m30":
                    return "Nightfall";

                case @"levels\solo\m35\m35":
                    return "Tip of the Spear";

                case @"levels\solo\m45\m45":
                    return "Long Night of Solace";

                case @"levels\solo\m50\m50":
                    return "Exodus";

                case @"levels\solo\m52\m52":
                    return "New Alexandria";

                case @"levels\solo\m60\m60":
                    return "The Package";

                case @"levels\solo\m70\m70":
                    return "The Pillar of Autumn";

                case @"levels\solo\m70_a\m70_a":
                    return "Credits";

                case @"levels\solo\m70_bonus\m70_bonus":
                    return "Lone Wolf";

                default:
                    return mapName;
            }
        }

        public static string[] getPackageData()
        {
            ///packageArrayData
            //- 0: gamertag
            //- 1: servicetag
            //- 2: mapName
            //- 3: missionName
            //- 4: difficulty
            ///packageArrayData

            string[] packageArrayData = new string[5];
            packageArrayData[0] = storage.fileInfoStorage.saveData.Gamertag;
            packageArrayData[1] = storage.fileInfoStorage.saveData.ServiceTag;
            packageArrayData[2] = storage.fileInfoStorage.saveData.Map;
            packageArrayData[3] = mapToMissionName(storage.fileInfoStorage.saveData.Map);
            packageArrayData[4] = storage.fileInfoStorage.saveData.Difficulty.ToString();

            return packageArrayData;
        }

        public static bool isPlayerInvincible()
        {
            return storage.fileInfoStorage.saveData.Player.Biped.Invincible;
        }

        public static int[] getSaveAmmo()
        {
            int[] saveAmmoArray = new int[6];

            //Weapons
            if (classInfo.storage.fileInfoStorage.saveData.Player.PrimaryWeapon != null)
            {
                saveAmmoArray[0] = storage.fileInfoStorage.saveData.Player.PrimaryWeapon.Ammo;
                saveAmmoArray[1] = storage.fileInfoStorage.saveData.Player.PrimaryWeapon.ClipAmmo;
            }

            if (classInfo.storage.fileInfoStorage.saveData.Player.SecondaryWeapon != null)
            {
                saveAmmoArray[2] = storage.fileInfoStorage.saveData.Player.SecondaryWeapon.Ammo;
                saveAmmoArray[3] = storage.fileInfoStorage.saveData.Player.SecondaryWeapon.ClipAmmo;
            }


            //Grenades
            saveAmmoArray[4] = storage.fileInfoStorage.saveData.Player.Biped.FragGrenades;
            saveAmmoArray[5] = storage.fileInfoStorage.saveData.Player.Biped.PlasmaGrenades;

            return saveAmmoArray;
        }

        public static float[] getPlayerCords()
        {
            float[] playerCords = new float[3];
            playerCords[0] = classInfo.storage.fileInfoStorage.saveData.Player.X;
            playerCords[1] = classInfo.storage.fileInfoStorage.saveData.Player.Y;
            playerCords[2] = classInfo.storage.fileInfoStorage.saveData.Player.Z;

            return playerCords;
        }

        public static T NumToEnum<T>(int number)
        {
            return (T)Enum.ToObject(typeof(T), number);
        }

        public static string convertClassToString(Reach.TagGroup type)
        {
            switch (type)
            {
                case Reach.TagGroup.Bipd:
                    return "bipd";
                case Reach.TagGroup.Vehi:
                    return "vehi";
                case Reach.TagGroup.Weap:
                    return "weap";
                case Reach.TagGroup.Eqip:
                    return "eqip";
                case Reach.TagGroup.Term:
                    return "term";
                case Reach.TagGroup.Scen:
                    return "scen";
                case Reach.TagGroup.Mach:
                    return "mach";
                case Reach.TagGroup.Ctrl:
                    return "ctrl";
                case Reach.TagGroup.Ssce:
                    return "ssce";
                case Reach.TagGroup.Bloc:
                    return "bloc";
                case Reach.TagGroup.Crea:
                    return "crea";
                case Reach.TagGroup.Efsc:
                    return "efsc";
            }
            return "unk1";
        }

        public static Reach.TagGroup convertStringToClass(string type)
        {
            switch (type)
            {
                case "bipd":
                    return Reach.TagGroup.Bipd;
                case "vehi":
                    return Reach.TagGroup.Vehi;
                case "weap":
                    return Reach.TagGroup.Weap;
                case "eqip":
                    return Reach.TagGroup.Eqip;
                case "term":
                    return Reach.TagGroup.Term;
                case "scen":
                    return Reach.TagGroup.Scen;
                case "mach":
                    return Reach.TagGroup.Mach;
                case "ctrl":
                    return Reach.TagGroup.Ctrl;
                case "ssce":
                    return Reach.TagGroup.Ssce;
                case "bloc":
                    return Reach.TagGroup.Bloc;
                case "crea":
                    return Reach.TagGroup.Crea;
                case "efsc":
                    return Reach.TagGroup.Efsc;
            }
            return Reach.TagGroup.Unknown;
        }

        public static string getMapName(string map)
        {
            string[] mapImageArray = map.Split('\\');
            return mapImageArray[mapImageArray.Length - 1];
        }
    }
}
