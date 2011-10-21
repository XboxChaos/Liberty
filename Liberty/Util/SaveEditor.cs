// NOTE: If you don't want to depend upon X360.dll, comment out the line below.
// Doing so will disable SaveEditor.LoadSTFS and the SaveEditor.SaveChanges KV overloads.
#define USING_X360

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

#if USING_X360
using X360;
using X360.STFS;
using X360.IO;
#endif

namespace Liberty.Util
{
    /// <summary>
    /// Provides helper functions and properties that simplify campaign save editing even more.
    /// </summary>
    /// <remarks>
    /// This class sucks...
    /// </remarks>
    public class SaveEditor
    {
        public void LoadMmiof(string path)
        {
            try
            {
                _mmiofPath = path;
                _saveData = new Reach.CampaignSave(path);
                _tagLists.Clear();
            }
            catch (Exception ex)
            {
                _mmiofPath = null;
                throw new ArgumentException("The save file is invalid.", ex);
            }
        }

#if USING_X360
        public void LoadSTFS(string path, string extractDir)
        {
            STFSPackage package = null;
            try
            {
                // Open the STFS package and find mmiof.bmf so we can extract it
                // Reach.CampaignSave requires an mmiof.bmf file
                package = new STFSPackage(path, null);
                X360.STFS.FileEntry file = package.GetFile("mmiof.bmf");
                _stfsPath = path;

                // Create the extraction directory
                Directory.CreateDirectory(extractDir);
                _mmiofPath = extractDir + "\\mmiof.bmf";

                // Extract the file and close the package
                if (!file.Extract(_mmiofPath))
                    throw new ArgumentException("Unable to extract mmiof.bmf to \"" + extractDir + "\"");
                package.CloseIO();
                package = null;

                // Load the mmiof.bmf file into the _saveData object
                _saveData = new Reach.CampaignSave(_mmiofPath);
                _tagLists.Clear();
            }
            catch (Exception ex)
            {
                if (package != null)
                    package.CloseIO();

                _mmiofPath = null;
                _stfsPath = null;

                throw new ArgumentException("The save file is invalid.", ex);
            }
        }
#endif

        public void SaveChanges()
        {
            _saveData.Update(_mmiofPath);

#if USING_X360
            UpdateSTFS(null);
#endif
        }

#if USING_X360
        public void SaveChanges(byte[] kvData)
        {
            SaveChanges();
            UpdateSTFS(kvData);
        }

        public void SaveChanges(string kvPath)
        {
            SaveChanges(File.ReadAllBytes(kvPath));
        }
#endif

        public void AddTaglist(TagList tagList)
        {
            _tagLists.Add(tagList);
        }

        public void UnloadTaglists()
        {
            _tagLists.Clear();
        }

        public string IdentifyObject(Reach.GameObject obj, bool guess)
        {
            string mapName = MapName;
            foreach (TagList tagList in _tagLists)
            {
                string name = tagList.Translate(mapName, obj.MapID);
                if (!string.IsNullOrEmpty(name))
                    return name;

                if (guess)
                {
                    string group = "bytype " + obj.TagGroup.ToString().ToLower();
                    if (obj.Carrier != null)
                    {
                        string groupWithCarry = group + " carriedby " + obj.Carrier.TagGroup.ToString().ToLower();
                        name = tagList.Translate(groupWithCarry, (uint)obj.Type);

                        if (!string.IsNullOrEmpty(name))
                            return name;
                    }
                    name = tagList.Translate(group, obj.MapID);
                    if (!string.IsNullOrEmpty(name))
                        return name;
                }
            }

            return "0x" + obj.MapID.ToString("X");
        }

        public void AllWeaponsMaxAmmo()
        {
            foreach (Reach.GameObject obj in _saveData.Objects)
            {
                Reach.WeaponObject weapon = obj as Reach.WeaponObject;
                if (weapon != null)
                {
                    weapon.Ammo = 32767;
                    weapon.ClipAmmo = 32767;
                }
            }
        }

        public void SwapBiped(Reach.BipedObject newBiped, bool transferWeapons)
        {
            _saveData.Player.ChangeBiped(newBiped, transferWeapons);
        }

        public HashSet<Reach.BipedObject> FindSwappableBipeds()
        {
            Reach.BipedObject currentBiped = _saveData.Player.Biped;
            HashSet<Reach.BipedObject> availableBipeds = new HashSet<Reach.BipedObject>();
            foreach (Reach.GameObject obj in _saveData.Objects)
            {
                if (obj != null && !obj.Deleted && obj.TagGroup == Reach.TagGroup.Bipd && obj.Zone == currentBiped.Zone && obj.IsActive)
                    availableBipeds.Add((Reach.BipedObject)obj);
            }
            return availableBipeds;
        }

        public bool Loaded
        {
            get { return (_saveData != null); }
        }

        public string Gamertag
        {
            get { return _saveData.Gamertag; }
        }

        public string ServiceTag
        {
            get { return _saveData.ServiceTag; }
        }

        public string MapName
        {
            get { return _saveData.Map.Substring(_saveData.Map.LastIndexOf('\\') + 1); }
        }

        public string MissionName
        {
            get { return MapToMissionName(_saveData.Map); }
        }

        public Reach.Difficulty Difficulty
        {
            get { return _saveData.Difficulty; }
        }

        public List<Reach.GameObject> Objects
        {
            get { return _saveData.Objects; }
        }

        public Reach.BipedObject Biped
        {
            get { return _saveData.Player.Biped; }
        }

        public Reach.VehicleObject Vehicle
        {
            get { return _saveData.Player.Biped.Vehicle; }
        }

        public bool Invincible
        {
            get
            {
                return _saveData.Player.Biped.Invincible;
            }
            set
            {
                _saveData.Player.Biped.Invincible = value;
                if (_saveData.Player.Biped.Vehicle != null)
                    _saveData.Player.Biped.Vehicle.Invincible = value;
            }
        }

        public bool PlayerHasPrimaryWeapon
        {
            get { return (_saveData.Player.PrimaryWeapon != null); }
        }

        public short PrimaryWeaponAmmo
        {
            get { return _saveData.Player.PrimaryWeapon.Ammo; }
            set { _saveData.Player.PrimaryWeapon.Ammo = value; }
        }

        public short PrimaryWeaponClip
        {
            get { return _saveData.Player.PrimaryWeapon.ClipAmmo; }
            set { _saveData.Player.PrimaryWeapon.ClipAmmo = value; }
        }

        public bool PlayerHasSecondaryWeapon
        {
            get { return (_saveData.Player.SecondaryWeapon != null); }
        }

        public short SecondaryWeaponAmmo
        {
            get { return _saveData.Player.SecondaryWeapon.Ammo; }
            set { _saveData.Player.SecondaryWeapon.Ammo = value; }
        }

        public short SecondaryWeaponClip
        {
            get { return _saveData.Player.SecondaryWeapon.ClipAmmo; }
            set { _saveData.Player.SecondaryWeapon.ClipAmmo = value; }
        }

        public sbyte FragGrenades
        {
            get { return _saveData.Player.Biped.FragGrenades; }
            set { _saveData.Player.Biped.FragGrenades = value; }
        }

        public sbyte PlasmaGrenades
        {
            get { return _saveData.Player.Biped.PlasmaGrenades; }
            set { _saveData.Player.Biped.PlasmaGrenades = value; }
        }

        public float X
        {
            get { return _saveData.Player.X; }
            set { _saveData.Player.X = value; }
        }

        public float Y
        {
            get { return _saveData.Player.Y; }
            set { _saveData.Player.Y = value; }
        }

        public float Z
        {
            get { return _saveData.Player.Z; }
            set { _saveData.Player.Z = value; }
        }

        public bool Noclip
        {
            get { return !_saveData.Player.Biped.PhysicsEnabled; }
            set { _saveData.Player.Biped.PhysicsEnabled = !value; }
        }

        public string CheckpointText
        {
            get { return _saveData.Message; }
            set { _saveData.Message = value; }
        }

        public string STFSPath
        {
            get { return _stfsPath; }
        }

        private class MapIdentComparer : IComparer<Reach.BipedObject>
        {
            public int Compare(Reach.BipedObject x, Reach.BipedObject y)
            {
                return x.MapID.CompareTo(y.MapID);
            }
        }

        private string MapToMissionName(string mapName)
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

#if USING_X360
        private void UpdateSTFS(byte[] kvData)
        {
            if (_stfsPath != null)
            {
                STFSPackage package = null;
                try
                {
                    // Re-open the STFS package and inject the new mmiof.bmf
                    package = new STFSPackage(_stfsPath, null);
                    X360.STFS.FileEntry file = package.GetFile("mmiof.bmf");
                    file.Inject(_mmiofPath);

                    if (kvData != null)
                    {
                        // Resign the package using the KV data
                        DJsIO kvStream = new DJsIO(kvData, true);
                        package.FlushPackage(new X360.STFS.RSAParams(kvStream));
                        kvStream.Close();
                    }
                    package.CloseIO();
                }
                catch
                {
                    if (package != null)
                        package.CloseIO();

                    throw;
                }
            }
        }
#endif

        private Reach.CampaignSave _saveData = null;
        private string _stfsPath = null;
        private string _mmiofPath = null;
        private List<TagList> _tagLists = new List<TagList>();
    }
}
