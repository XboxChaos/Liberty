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
    /// </summary>
    public class SaveManager<T> where T : ICampaignSave
    {
        public SaveManager(Func<string, T> constructSave)
        {
            _constructSave = constructSave;
        }

        /// <summary>
        /// The currently loaded campaign save data.
        /// </summary>
        public T SaveData
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
        /// The path to the currently loaded raw save file.
        /// Can be null if no raw save file is loaded.
        /// </summary>
        public string RawDataPath
        {
            get { return _rawPath; }
        }

        public bool Loaded
        {
            get { return (_saveData != null); }
        }

        /// <summary>
        /// Loads the save data from a raw file.
        /// </summary>
        /// <param name="path">The path to the raw save file to load.</param>
        public void LoadRaw(string path)
        {
            try
            {
                _rawPath = path;
                _saveData = _constructSave(path);
            }
            catch (Exception ex)
            {
                _rawPath = null;
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
                _saveData = _constructSave(newMmiofPath);
                _rawPath = newMmiofPath;
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
            _saveData.Update(_rawPath);
            UpdateSTFS(null);
        }

        /// <summary>
        /// Saves any changes made to the save data, resigning the container package with the specified KV.
        /// </summary>
        /// <param name="kvData">The KV data to resign with</param>
        public void SaveChanges(byte[] kvData)
        {
            _saveData.Update(_rawPath);
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

        /// <summary>
        /// Closes the currently open campaign save.
        /// </summary>
        public void Close()
        {
            _saveData = default(T);
            _rawPath = null;
            _stfsPath = null;
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
                    file.Inject(_rawPath);

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

        private T _saveData;
        private Func<string, T> _constructSave;
        private string _stfsPath = null;
        private string _rawPath = null;
    }
}
