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
    public class SaveManager<T> : Liberty.Util.ISaveManager where T : ICampaignSave
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
        /// The path to the currently loaded raw save file.
        /// Can be null if no raw save file is loaded.
        /// </summary>
        public string RawDataPath
        {
            get { return _rawPath; }
        }

        /// <summary>
        /// Returns whether or not any save data is loaded.
        /// </summary>
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
        public void LoadSTFS(string path, string rawFileName, string extractDir)
        {
            STFSPackage package = new STFSPackage(path, null);
            LoadSTFS(package, rawFileName, extractDir);
            package.CloseIO();
        }

        public void LoadSTFS(STFSPackage package, string rawFileName, string extractDir)
        {
            X360.STFS.FileEntry file = package.GetFile(rawFileName);

            // Create the extraction directory
            Directory.CreateDirectory(extractDir);
            string newRawPath = extractDir + "\\" + rawFileName;

            // Extract the file and close the package
            if (!file.Extract(newRawPath))
                throw new ArgumentException("Unable to extract " + rawFileName + " to \"" + extractDir + "\"");

            // Load the mmiof.bmf file into the _saveData object
            _saveData = _constructSave(newRawPath);
            _rawPath = newRawPath;
        }

        /// <summary>
        /// Saves any changes made to the save data without resigning the container package (if any).
        /// </summary>
        public void SaveChanges(X360.STFS.STFSPackage package)
        {
            _saveData.Update(_rawPath);
            UpdateSTFS(package, null);
        }

        /// <summary>
        /// Saves any changes made to the save data, resigning the container package with the specified KV.
        /// </summary>
        /// <param name="kvData">The KV data to resign with</param>
        public void SaveChanges(X360.STFS.STFSPackage package, byte[] kvData)
        {
            _saveData.Update(_rawPath);
            UpdateSTFS(package, kvData);
        }

        /// <summary>
        /// Saves any changes made to the save data, resigning the container package with the specified KV file.
        /// </summary>
        /// <param name="kvPath">The path to the KV file to resign with.</param>
        public void SaveChanges(X360.STFS.STFSPackage package, string kvPath)
        {
            SaveChanges(package, File.ReadAllBytes(kvPath));
        }

        /// <summary>
        /// Closes the currently open campaign save.
        /// </summary>
        public void Close()
        {
            _saveData = default(T);
            _rawPath = null;
        }
        
        private void UpdateSTFS(STFSPackage package, byte[] kvData)
        {
            if (package != null)
            {
                // Inject the new mmiof.bmf
                X360.STFS.FileEntry file = package.GetFile("mmiof.bmf");
                file.Inject(_rawPath);

                if (kvData != null)
                {
                    // Resign the package using the KV data
                    DJsIO kvStream = new DJsIO(kvData, true);
                    package.FlushPackage(new X360.STFS.RSAParams(kvStream));
                    kvStream.Close();
                }
            }
        }

        private T _saveData;
        private Func<string, T> _constructSave;
        private string _rawPath = null;
    }
}
