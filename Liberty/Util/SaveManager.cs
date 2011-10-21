using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using X360;
using X360.STFS;
using X360.IO;

namespace Liberty.Util
{
    /// <summary>
    /// Wraps a CampaignSave and provides functions for loading/saving to STFS packages.
    /// Also provides functions for managing taglists.
    /// </summary>
    public class SaveManager
    {
        /// <summary>
        /// The currently loaded campaign save data.
        /// </summary>
        public Reach.CampaignSave SaveData
        {
            get { return _saveData; }
        }

        /// <summary>
        /// The path to the currently loaded STFS package.
        /// Can be null if no STFS package is loaded.
        /// </summary>
        public string STFSPath
        {
            get { return _stfsPath; }
        }

        /// <summary>
        /// The path to the currently loaded mmiof.bmf file.
        /// Can be null if no mmiof.bmf file is loaded.
        /// </summary>
        public string MmiofPath
        {
            get { return _mmiofPath; }
        }

        public bool Loaded
        {
            get { return (_saveData != null); }
        }

        /// <summary>
        /// Loads the save data from a mmiof.bmf file.
        /// </summary>
        /// <param name="path">The path to the mmiof.bmf file to load.</param>
        public void LoadMmiof(string path)
        {
            try
            {
                _mmiofPath = path;
                _saveData = new Reach.CampaignSave(path);
                _mapTaglists.Clear();
            }
            catch (Exception ex)
            {
                _mmiofPath = null;
                throw new ArgumentException("The save file is invalid.", ex);
            }
        }

        /// <summary>
        /// Loads the save data from an STFS package.
        /// </summary>
        /// <param name="path">The path to the STFS package to load.</param>
        /// <param name="extractDir">The directory to extract the STFS package to.</param>
        public void LoadSTFS(string path, string extractDir)
        {
            STFSPackage package = null;
            try
            {
                // Open the STFS package and find mmiof.bmf so we can extract it
                // Reach.CampaignSave requires an mmiof.bmf file
                package = new STFSPackage(path, null);
                X360.STFS.FileEntry file = package.GetFile("mmiof.bmf");

                // Create the extraction directory
                Directory.CreateDirectory(extractDir);
                string newMmiofPath = extractDir + "\\mmiof.bmf";

                // Extract the file and close the package
                if (!file.Extract(newMmiofPath))
                    throw new ArgumentException("Unable to extract mmiof.bmf to \"" + extractDir + "\"");
                package.CloseIO();
                package = null;

                // Load the mmiof.bmf file into the _saveData object
                _saveData = new Reach.CampaignSave(newMmiofPath);
                _mapTaglists.Clear();
                _mmiofPath = newMmiofPath;
                _stfsPath = path;
            }
            catch (Exception ex)
            {
                if (package != null)
                    package.CloseIO();

                throw new ArgumentException("The save file is invalid.", ex);
            }
        }

        /// <summary>
        /// Saves any changes made to the save data without resigning the container package (if any).
        /// </summary>
        public void SaveChanges()
        {
            _saveData.Update(_mmiofPath);
            UpdateSTFS(null);
        }

        /// <summary>
        /// Saves any changes made to the save data, resigning the container package with the specified KV.
        /// </summary>
        /// <param name="kvData">The KV data to resign with</param>
        public void SaveChanges(byte[] kvData)
        {
            _saveData.Update(_mmiofPath);
            UpdateSTFS(kvData);
        }

        /// <summary>
        /// Saves any changes made to the save data, resigning the container package with the specified KV file.
        /// </summary>
        /// <param name="kvPath">The path to the KV file to resign with.</param>
        public void SaveChanges(string kvPath)
        {
            SaveChanges(File.ReadAllBytes(kvPath));
        }

        public void Close()
        {
            _saveData = null;
            _mmiofPath = null;
            _stfsPath = null;
            _mapTaglists.Clear();
        }

        /// <summary>
        /// Attempts to identify an object in the campaign save.
        /// </summary>
        /// <param name="obj">The object to identify</param>
        /// <param name="guess">Whether or not name guessing should be used (generic taglists only)</param>
        /// <returns>The object's name</returns>
        public string IdentifyObject(Reach.GameObject obj, bool guess)
        {
            string mapName = _saveData.Map;
            mapName = mapName.Substring(mapName.LastIndexOf('\\') + 1);

            // Generic taglists + name guessing
            foreach (TagList tagList in _genericTaglists)
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

            // Map-specific taglists
            foreach (TagList tagList in _mapTaglists)
            {
                string name = tagList.Translate(mapName, obj.MapID);
                if (!string.IsNullOrEmpty(name))
                    return name;
            }

            // Nothing found, just convert the ID to hex
            return "0x" + obj.MapID.ToString("X");
        }

        public void AddGenericTaglist(TagList tagList)
        {
            _genericTaglists.Add(tagList);
        }

        public void AddMapSpecificTaglist(TagList tagList)
        {
            _mapTaglists.Add(tagList);
        }

        public void RemoveGenericTaglists()
        {
            _genericTaglists.Clear();
        }

        public void RemoveMapSpecificTaglists()
        {
            _mapTaglists.Clear();
        }

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

        private Reach.CampaignSave _saveData = null;
        private string _stfsPath = null;
        private string _mmiofPath = null;
        private List<TagList> _genericTaglists = new List<TagList>();
        private List<TagList> _mapTaglists = new List<TagList>();
    }
}
