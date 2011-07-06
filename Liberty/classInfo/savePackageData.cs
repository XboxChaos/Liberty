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
    class savePackageData
    {
        public static void setPlayerInvincibility(bool isInvinciable)
        {
            classInfo.storage.fileInfoStorage.saveData.Player.Biped.Invincible = isInvinciable;
            if (classInfo.storage.fileInfoStorage.saveData.Player.Biped.Vehicle != null)
                classInfo.storage.fileInfoStorage.saveData.Player.Biped.Vehicle.Invincible = isInvinciable;
        }

        public static void setPlayerAmmo(int[] saveAmmo)
        {
            if (classInfo.storage.fileInfoStorage.saveData.Player.PrimaryWeapon != null)
            {
                classInfo.storage.fileInfoStorage.saveData.Player.PrimaryWeapon.Ammo = Convert.ToInt16(saveAmmo[0]);
                classInfo.storage.fileInfoStorage.saveData.Player.PrimaryWeapon.ClipAmmo = Convert.ToInt16(saveAmmo[1]);
            }
            if (classInfo.storage.fileInfoStorage.saveData.Player.SecondaryWeapon != null)
            {
                classInfo.storage.fileInfoStorage.saveData.Player.SecondaryWeapon.Ammo = Convert.ToInt16(saveAmmo[2]);
                classInfo.storage.fileInfoStorage.saveData.Player.SecondaryWeapon.ClipAmmo = Convert.ToInt16(saveAmmo[3]);
            }
            classInfo.storage.fileInfoStorage.saveData.Player.Biped.FragGrenades = Convert.ToSByte(saveAmmo[4]);
            classInfo.storage.fileInfoStorage.saveData.Player.Biped.PlasmaGrenades = Convert.ToSByte(saveAmmo[5]);
        }

        public static void setPlayerCords(float[] playerCords)
        {
            classInfo.storage.fileInfoStorage.saveData.Player.X = playerCords[0];
            classInfo.storage.fileInfoStorage.saveData.Player.Y = playerCords[1];
            classInfo.storage.fileInfoStorage.saveData.Player.Z = playerCords[2];
        }

        public static void setAllMaxAmmo()
        {
            foreach (Reach.GameObject obj in classInfo.storage.fileInfoStorage.saveData.Objects)
            {
                Reach.WeaponObject weapon = obj as Reach.WeaponObject;
                if (weapon != null)
                {
                    weapon.Ammo = 32767;
                    weapon.ClipAmmo = 32767;
                }
            }
        }
    }
}
