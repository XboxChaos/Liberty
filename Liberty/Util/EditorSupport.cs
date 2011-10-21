using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.Util
{
    /// <summary>
    /// Provides support functions that might be useful for GUIs.
    /// </summary>
    public static class EditorSupport
    {
        public static void AllWeaponsMaxAmmo(Reach.CampaignSave saveData)
        {
            foreach (Reach.GameObject obj in saveData.Objects)
            {
                Reach.WeaponObject weapon = obj as Reach.WeaponObject;
                if (weapon != null)
                {
                    weapon.Ammo = 32767;
                    weapon.ClipAmmo = 32767;
                }
            }
        }

        public static HashSet<Reach.BipedObject> FindSwappableBipeds(Reach.CampaignSave saveData)
        {
            Reach.BipedObject currentBiped = saveData.Player.Biped;
            HashSet<Reach.BipedObject> availableBipeds = new HashSet<Reach.BipedObject>();
            foreach (Reach.GameObject obj in saveData.Objects)
            {
                if (obj != null && !obj.Deleted && obj.TagGroup == Reach.TagGroup.Bipd && obj.Zone == currentBiped.Zone && obj.IsActive)
                    availableBipeds.Add((Reach.BipedObject)obj);
            }
            return availableBipeds;
        }

        public static string GetMissionName(Reach.CampaignSave saveData)
        {
            switch (saveData.Map)
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
                    return saveData.Map;
            }
        }
    }
}
