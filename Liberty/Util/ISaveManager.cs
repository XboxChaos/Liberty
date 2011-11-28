using System;

namespace Liberty.Util
{
    public interface ISaveManager
    {
        bool Loaded { get; }
        string RawDataPath { get; }

        void LoadRaw(string path);
        void LoadSTFS(string path, string rawFileName, string extractDir);
        void LoadSTFS(X360.STFS.STFSPackage package, string rawFileName, string extractDir);
        void Close();
        
        void SaveChanges(X360.STFS.STFSPackage package);
        void SaveChanges(X360.STFS.STFSPackage package, byte[] kvData);
        void SaveChanges(X360.STFS.STFSPackage package, string kvPath);
    }
}
