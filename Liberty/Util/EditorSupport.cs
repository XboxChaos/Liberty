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
        public static void AllWeaponsMaxAmmo(HCEX.CampaignSave saveData)
        {
            foreach (HCEX.GameObject obj in saveData.Objects)
            {
                HCEX.WeaponObject weapon = obj as HCEX.WeaponObject;
                if (weapon != null)
                {
                    weapon.Ammo = 32767;
                    weapon.ClipAmmo = 32767;
                }
            }
        }
        public static void AllWeaponsMaxAmmo(Halo3.CampaignSave saveData)
        {
            foreach(Halo3.GameObject obj in saveData.Objects)
            {
                Halo3.WeaponObject weap = obj as Halo3.WeaponObject;
                if (weap != null)
                {
                    weap.Ammo = 32767;
                    weap.ClipAmmo = 32767;
                }
            }
        }
        public static void AllWeaponsMaxAmmo(Halo3ODST.CampaignSave saveData)
        {
            foreach (Halo3ODST.GameObject obj in saveData.Objects)
            {
                Halo3ODST.WeaponObject weap = obj as Halo3ODST.WeaponObject;
                if (weap != null)
                {
                    weap.Ammo = 32767;
                    weap.ClipAmmo = 32767;
                }
            }
        }
        public static void AllWeaponsMaxAmmo(Halo4.CampaignSave saveData)
        {
            foreach (Halo4.GameObject obj in saveData.Objects)
            {
                Halo4.WeaponObject weap = obj as Halo4.WeaponObject;
                if (weap != null)
                {
                    weap.Ammo = 32767;
                    weap.ClipAmmo = 32767;
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
        public static HashSet<Halo3.BipedObject> FindSwappableBipeds(Halo3.CampaignSave saveData)
        {
            Halo3.BipedObject currentBiped = saveData.PlayerBiped;
            HashSet<Halo3.BipedObject> avaiableBipeds = new HashSet<Halo3.BipedObject>();
            foreach(Halo3.BipedObject obj in saveData.Objects)
                if (obj != null && obj.TagGroup == Halo3.TagGroup.Bipd && obj.Zone == currentBiped.Zone) // TODO: Deleted (with ObjEditor) and IsActive
                    avaiableBipeds.Add((Halo3.BipedObject)obj);
            return avaiableBipeds;
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
                    return saveData.Map.Substring(saveData.Map.LastIndexOf('\\') + 1).ToLower();
            }
        }
        public static string GetMissionName(HCEX.CampaignSave saveData)
        {
            switch (saveData.Map)
            {
                case @"levels\a10\a10":
                    return "The Pillar of Autumn";

                case @"levels\a30\a30":
                    return "Halo";

                case @"levels\a50\a50":
                    return "The Truth and Reconciliation";

                case @"levels\b30\b30":
                    return "The Silent Cartographer";

                case @"levels\b40\b40":
                    return "Assault on the Control Room";

                case @"levels\c10\c10":
                    return "343 Guilty Spark";

                case @"levels\c20\c20":
                    return "The Library";

                case @"levels\c40\c40":
                    return "Two Betrayals";

                case @"levels\d20\d20":
                    return "Keys";

                case @"levels\d40\d40":
                    return "The Maw";

                default:
                    return saveData.Map.Substring(saveData.Map.LastIndexOf('\\') + 1).ToLower();
            }
        }
        public static string GetMissionName(Halo3.CampaignSave saveData)
        {
            switch (saveData.Header.Map.Substring(saveData.Header.Map.LastIndexOf('\\') + 1).ToLower())
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
                    return saveData.Header.Map.Substring(saveData.Header.Map.LastIndexOf('\\') + 1).ToLower();
            }
        }
        public static string GetMissionName(Halo3ODST.CampaignSave saveData)
        {
            switch (saveData.Header.Map.Substring(saveData.Header.Map.LastIndexOf('\\') + 1).ToLower())
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
                    return saveData.Header.Map.Substring(saveData.Header.Map.LastIndexOf('\\') + 1).ToLower();
            }
        }
        public static string GetMissionName(Halo4.CampaignSave saveData)
        {
            switch (saveData.Header.Map.Substring(saveData.Header.Map.LastIndexOf('\\') + 1).ToLower())
            {
                case @"m020":
                    return "Prologue";

                case @"m05_prologue":
                    return "Dawn";

                case @"m10_crash":
                    return "Requiem";

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
                    return saveData.Header.Map.Substring(saveData.Header.Map.LastIndexOf('\\') + 1).ToLower();
            }
        }
    }
}
