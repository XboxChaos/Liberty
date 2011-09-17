using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FATX.Structs;
using System.Windows;

namespace FATX
{

    public class Partition
    {
        PartitionInfo partInfo = new PartitionInfo();

        public PartitionInfo PartInfo
        {
            get { return partInfo; }
            set { partInfo = value; }
        }
    }

    public class Entry
    {
        public EntryData EData = new EntryData();
        /// <summary>
        /// Information about the partition in which the file is located
        /// </summary>
        public PartitionInfo PartInfo;
        internal Entries e;
        internal Misc Misc;
        private FATX.FATXDrive drive;
        internal uint[] blocksOCCUPADO;
        private bool isDeleted;
        public STFSInfo STFSInformation;
        string path = "";

        public Entry(FATXDrive xDrive, PartitionInfo partinfo)
        {
            drive = xDrive;
            PartInfo = partinfo;
        }

        public string FullPath { get { return path; } }

        internal void SetPath(string Path)
        {
            path = Path;
        }

        /// <summary>
        /// Device ID (used for the FATXDrive)
        /// </summary>
        public int DeviceID
        {
            get { return Drive.DeviceID; }
        }

        public bool IsDeleted
        {
            get { return isDeleted; }
            set { isDeleted = value; }
        }

        public DateTime CreationDate
        {
            get { return Misc.DateTimeFromFATInt(EData.CreationDate, EData.CreationTime); }
        }

        public DateTime ModifiedDate
        {
            get { return Misc.DateTimeFromFATInt(EData.ModifiedDate, EData.ModifiedTime); }
        }

        public DateTime AccessedDate
        {
            get { return Misc.DateTimeFromFATInt(EData.AccessDate, EData.AccessTime); }
        }

        /// <summary>
        /// Name of the folder
        /// </summary>
        public string Name
        {
            get { return EData.Name; }
            set { EData.Name = value; }
        }

        /// <summary>
        /// Offset of the first block
        /// </summary>
        public long BaseOffset
        {
            get { return Misc.GetBlockOffset(StartingCluster, this); }
        }

        /// <summary>
        /// Root block
        /// </summary>
        public uint StartingCluster
        {
            get { return (uint)EData.StartingCluster; }
        }

        /// <summary>
        /// All blocks that the object occupies
        /// </summary>
        public virtual uint[] BlocksOccupied
        {
            get {
                if (!IsDeleted)
                {
                    if (blocksOCCUPADO == null)
                    {
                        blocksOCCUPADO = e.GetBlocksOccupied(StartingCluster);
                    }
                }
                else
                {
                    if (blocksOCCUPADO == null)
                    {
                        if (IsFolder)
                        {
                            blocksOCCUPADO = new uint[] { StartingCluster };
                        }
                    }
                }
                return blocksOCCUPADO;
            }
            set
            {
                blocksOCCUPADO = value;
            }
        }

        public bool BlocksLoaded
        {
            get { if (blocksOCCUPADO == null) { return false; } else { return true; } }
        }

        /// <summary>
        /// Offset for the entry in the file's folder
        /// </summary>
        public long EntryOffset
        {
            get { return EData.EntryOffset; }
        }

        public FATXDrive Drive
        {
            get { return drive; }
            set { drive = value; }
        }

        public bool IsFolder
        {
            get;
            set;
        }

        public string EntryType
        {
            get
            {
                if (IsFolder && IsDeleted)
                {
                    return "Deleted Folder";
                }
                else if (IsFolder)
                {
                    return "File Folder";
                }
                else if (!IsFolder && IsDeleted)
                {
                    return "Deleted File";
                }
                else
                {
                    return "File";
                }
            }
        }

        public bool Rename(string NewName)
        {
            Write w = new Write(Drive);
            return w.Rename(this, NewName);
        }

        public Folder ParentFolder
        {
            get;
            set;
        }


        public Info.Flags[] Flags
        {
            get
            {
                List<Info.Flags> FL = new List<Info.Flags>();
                // Read bit zero, mask the rest of that shit
                for (short i = 1, j = 0; i <= 80; i <<= 1, j++)
                {
                    if (((EData.Flags & i) >> j) == 1)
                    {
                        FL.Add((Info.Flags)Enum.Parse(typeof(Info.Flags), Enum.GetName(typeof(Info.Flags), j)));
                    }
                }
                return FL.ToArray();
            }
        }
    }
    
    public sealed class Folder : Entry
    {
        private List<Folder> subFolders = new List<Folder>();
        private List<Folder> deletedFolders = new List<Folder>();
        private List<File> files = new List<File>();
        private List<File> deletedFiles = new List<File>();
        bool DeletedEntriesLoaded = false;
        bool STFSInfoLoaded = false;
        bool EntriesLoaded = false;
        public string CachedGameName;

        public Folder(FATXDrive DRIVE, PartitionInfo partition):base(DRIVE, partition)
        {
            e = new Entries(this);
            Misc = new Misc();
            IsFolder = true;
        }

        public Folder RootPartition
        {
            get
            {
                Folder f = new Folder(Drive, PartInfo);
                f.Name = PartInfo.Name;
                f.EData.StartingCluster = new FATStuff(this).RootDirectoryCluster();
                f.SetPath(PartInfo.Name);
                return f;
            }
        }

        public string GameName()
        {
            if (IsTitleIDFolder)
            {

                // The known title ID's...
                string[] Known = 
                {
                    "000D0000","00009000","00040000","02000000","00080000",
                    "00020000","000A0000","000C0000","00400000","00004000",
                    "000B0000","00002000","000F0000","00000002","00100000",
                    "00300000","00500000","00010000","00000003","00000001",
                    "00050000","00030000","00200000","00090000","00600000",
                    "00070000","00005000","00060000","00001000","00005000",
                    "000E0000","FFFE07D1","00007000","00008000"
                };
                string[] KnownEquivilent =
                {
                    "Arcade Title","Avatar Item","Cache File","Community Game","Game Demo",
                    "Gamer Picture","Game Title","Game Trailer","Game Video","Installed Game",
                    "Installer","IPTV Pause Buffer","License Store","Marketplace Content","Movie",
                    "Music Video","Podcast Video","Profile","Publisher","Saved Game",
                    "Storage Download","Theme","TV","Video","Viral Video",
                    "Xbox Download","Xbox Original Game","Xbox Saved Game","Installed Xbox 360 Title","Xbox Title",
                    "XNA", "Xbox 360 Dashboard","Games on Demand","Storage Pack"
                };
                // Before going through all of this bullshit, let's just see if it's a known Title ID...
                for (int i = 0; i < Known.Length; i++)
                {
                    if (Name == Known[i])
                    {
                        return KnownEquivilent[i];
                    }
                }

                if (CachedGameName != null)
                {
                    return CachedGameName;
                }
                try
                {
                    if (SubFolders(DeletedEntriesLoaded).Length > 0)
                    {
                        string last = "";
                        foreach (Folder f in SubFolders(DeletedEntriesLoaded))
                        {
                            // We need to check to see if the subfolder resembles another 8-digit
                            // ID (for gamesaves, demos, etc)
                            if (f.IsTitleIDFolder)
                            {
                                if (f.Files(DeletedEntriesLoaded).Length > 0)
                                {
                                    foreach (File F in f.files)
                                    {
                                        // The STFS info for this package most likely wasn't loaded
                                        // if we aren't loading STFS information, so let's force load
                                        if (F.GetTitleName() != "")
                                        {
                                            CachedGameName = F.GetTitleName();
                                            return CachedGameName;

                                        }
                                        else
                                        {
                                            last = F.GetPackageName();
                                        }
                                        //if (!f.LoadSTFSInfo)
                                        //{
                                        //    F.ForceSTFSInfo();
                                        //}
                                        //if (F.IsSTFSPackage)
                                        //{
                                        //    if (F.STFSInformation.TitleName != "")
                                        //    {
                                        //        CachedGameName = F.STFSInformation.TitleName;
                                        //        return CachedGameName;
                                        //    }
                                        //}
                                    }
                                }
                            }
                        }
                        CachedGameName = last;
                        return CachedGameName;
                    }
                }
                catch { Console.WriteLine("Error getting game name"); }
            }
            return "";
        }

        public bool IsTitleIDFolder
        {
            get
            {
                // Title ID's are 8 digits long; this shit isn't a Title ID folder if it's
                // longer or shorter
                if (Name.Length != 8)
                {
                    return false;
                }
                // It was 8 digits long, let's check if the characters are valid hex digits
                char[] Chars = Name.ToArray<char>();
                char[] AcceptableChars = "0123456789ABCDEFabcdef".ToArray<char>();
                for (int i = 0; i < Chars.Length; i++)
                {
                    bool Acceptable = false;
                    for (int j = 0; j < AcceptableChars.Length; j++)
                    {
                        if (Chars[i] == AcceptableChars[j])
                        {
                            Acceptable = true;
                            break;
                        }
                    }
                    if (!Acceptable)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Reloads the blocks, files, and folders.
        /// </summary>
        public void ReloadData(bool DoDeletedEntries)
        {
            Entries e = new Entries(this);
            //This will get our blocks occupied
            BlocksOccupied = e.GetBlocksOccupied(BlocksOccupied[0]);
            // Set entries loaded to false so that they will be reloaded
            EntriesLoaded = false;
            //This will get the files and folders
            Files(DoDeletedEntries);
        }

        /// <summary>
        /// Extracts each subfolder and file in this folder recursively
        /// </summary>
        /// <param name="path">Path to extract to</param>
        /// <param name="updateFolder">Current file being extracted</param>
        /// <param name="updateText">Current file being extracted</param>
        /// <param name="progress">The number of blocks that have been read/written</param>
        /// <param name="maxVal">File size</param>
        /// <param name="CreateRoot">Create root folder</param>
        public bool ExtractRecursive(string path, ref string updateEntry, ref int progress, ref int maxVal, bool CreateRoot, bool DoDeletedEntries, ref bool Cancel)
        {
            bool result = false;
            try
            {
                updateEntry = Name;
                if (CreateRoot == true)
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(path + "\\" + Name);
                    }
                    catch (Exception e) { MessageBox.Show(e.Message); return false; }
                    path += "\\" + EData.Name;
                }

                if (Cancel)
                {
                    return true;
                }

                File[] ourFiles = Files(DoDeletedEntries);
                if (ourFiles.Length != 0)
                {
                    foreach (File f in ourFiles)
                    {
                        if (Cancel)
                        {
                            return true;
                        }
                        updateEntry = f.Name;
                        e.ExtractFile(f, path + "\\" + f.Name, ref progress, ref maxVal, ref Cancel);
                    }
                }

                if (Cancel)
                {
                    return true;
                }
                Folder[] subFolders = SubFolders(DoDeletedEntries);
                if (subFolders.Length != 0)
                {
                    foreach (Folder f in subFolders)
                    {
                        if (Cancel)
                        {
                            return true;
                        }
                        updateEntry = f.Name;
                        result = f.ExtractRecursive(path, ref updateEntry, ref progress, ref maxVal ,true, DoDeletedEntries, ref Cancel);
                    }
                }
                return true;
            }
            catch (Exception E) { throw E; }
        }

        /// <summary>
        /// Extracts each subfolder and file in this folder recursively, skipping desired folder names
        /// </summary>
        /// <param name="path">Path to extract to</param>
        /// <param name="updateFolder">Current file being extracted</param>
        /// <param name="updateText">Current file being extracted</param>
        /// <param name="progress">The number of blocks that have been read/written</param>
        /// <param name="maxVal">File size</param>
        /// <param name="CreateRoot">Create root folder</param>
        public bool ExtractRecursive(string path, ref string updateEntry, ref int progress, ref int maxVal, bool CreateRoot, bool DoDeletedEntries, string[] FoldersToSkip, ref bool Cancel)
        {
            bool result = false;
            try
            {
                // If this folder name is one of the ones we don't want to extract...
                for (int i = 0; i < FoldersToSkip.Length; i++)
                {
                    if (Name == FoldersToSkip[i])
                    {
                        return true;
                    }
                }
                updateEntry = Name;
                if (CreateRoot == true)
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(path + "\\" + Name);
                    }
                    catch (Exception e) { MessageBox.Show(e.Message); return false; }
                    path += "\\" + EData.Name;
                }
                if (Cancel)
                {
                    return true;
                }

                File[] ourFiles = Files(DoDeletedEntries);
                if (ourFiles.Length != 0)
                {
                    foreach (File f in ourFiles)
                    {
                        if (Cancel)
                        {
                            return true;
                        }
                        updateEntry = f.Name;
                        e.ExtractFile(f, path + "\\" + f.Name, ref progress, ref maxVal, ref Cancel);
                    }
                }

                if (Cancel)
                {
                    return true;
                }

                Folder[] subFolders = SubFolders(DoDeletedEntries);
                if (subFolders.Length != 0)
                {
                    foreach (Folder f in subFolders)
                    {
                        if (Cancel)
                        {
                            return true;
                        }
                        updateEntry = f.Name;
                        result = f.ExtractRecursive(path, ref updateEntry, ref progress, ref maxVal, true, DoDeletedEntries, FoldersToSkip, ref Cancel);
                    }
                }
                return true;
            }
            catch (Exception E) { throw E; }
        }

        /// <summary>
        /// Returns an array of files that this folder holds
        /// </summary>
        public File[] Files(bool ShowDeletedEntries)
        {
            if (BlocksOccupied != null)
            {
                goto _Load;
            }
            else { blocksOCCUPADO = e.GetBlocksOccupied(StartingCluster); goto _Load; }
        _Load:
            {
            // Change in v1.0.0.20
                if (EntriesLoaded)
                {
                    // Check if the STFS information was previously loaded...
                    if (STFSInfoLoaded != LoadSTFSInfo && LoadSTFSInfo == true)
                    {
                        // STFS info wasn't loaded, but they want it now... let's give
                        // them what they want
                        for (int i = 0; i < files.Count; i++)
                        {
                            files[i].ForceSTFSInfo();
                        }
                        // Although this probably won't work most of the time, whatever.
                        for (int i = 0; i < deletedFiles.Count; i++)
                        {
                            deletedFiles[i].ForceSTFSInfo();
                        }
                    }
                    // If we aren't returning deleted entries
                    if (!ShowDeletedEntries)
                    {
                        // Only return non-deleted files
                        return files.ToArray();
                    }
                        // We are returning deleted files
                    else
                    {
                        // Return both non-deleted files and deleted files
                        List<File> toReturn = new List<File>();
                        toReturn.AddRange(files);
                        toReturn.AddRange(deletedFiles);
                        return toReturn.ToArray();
                    }
                }
                //This function gets files and folders in one fair swoop
                object[] objects = e.LoadEntries(this, true, LoadSTFSInfo);
                files = ((File[])objects[1]).ToList();;
                subFolders = ((Folder[])objects[0]).ToList();;
                DeletedEntriesLoaded = ShowDeletedEntries;
            // Create our lists
                List<File> fileList = new List<File>();
                List<File> deletedfileList = new List<File>();
                List<Folder> folderList = new List<Folder>();
                List<Folder> deletedfolderList = new List<Folder>();
            // Loop for each file that we've gotten
                foreach (File f in files)
                {
                    f.ParentFolder = this;
                    f.SetPath(this.FullPath + "\\" + f.Name);
                    if (!f.IsDeleted)
                    {
                        fileList.Add(f);
                    }
                    else
                    {
                        deletedfileList.Add(f);
                    }
                }
                foreach (Folder f in subFolders)
                {
                    f.ParentFolder = this;
                    f.LoadSTFSInfo = LoadSTFSInfo;
                    f.SetPath(this.FullPath + "\\" + f.Name);
                    if (!f.IsDeleted)
                    {
                        folderList.Add(f);
                    }
                    else
                    {
                        deletedfolderList.Add(f);
                    }
                }
            // Set the lists we have
                files = fileList;
                deletedFiles = deletedfileList;
                subFolders = folderList;
                deletedFolders = deletedfolderList;
            }
        EntriesLoaded = true;
            // If we aren't returning deleted entries
            if (!ShowDeletedEntries)
            {
                // Only return non-deleted files
                return files.ToArray();
            }
            // We are returning deleted files
            else
            {
                // Return both non-deleted files and deleted files
                List<File> toReturn = new List<File>();
                toReturn.AddRange(files);
                toReturn.AddRange(deletedFiles);
                return toReturn.ToArray();
            }
        }

        /// <summary>
        /// Returns an array of subfolders that this folder holds
        /// </summary>
        public Folder[] SubFolders(bool ShowDeletedEntries)
        {
            if (BlocksOccupied != null)
            {
                goto _Load;
            }
            else { blocksOCCUPADO = e.GetBlocksOccupied(StartingCluster); goto _Load; }
        _Load:
            {
                // Change in v1.0.0.20
                if (EntriesLoaded)
                {
                    // If we aren't returning deleted entries
                    if (!ShowDeletedEntries)
                    {
                        // Only return non-deleted files
                        return subFolders.ToArray();
                    }
                    // We are returning deleted files
                    else
                    {
                        // Return both non-deleted files and deleted files
                        List<Folder> toReturn = new List<Folder>();
                        toReturn.AddRange(subFolders);
                        toReturn.AddRange(deletedFolders);
                        return toReturn.ToArray();
                    }
                }
                //This function gets files and folders in one fair swoop
                object[] objects = e.LoadEntries(this, true, LoadSTFSInfo);
                files = ((File[])objects[1]).ToList(); ;
                subFolders = ((Folder[])objects[0]).ToList(); ;
                DeletedEntriesLoaded = ShowDeletedEntries;
                // Create our lists
                List<File> fileList = new List<File>();
                List<File> deletedfileList = new List<File>();
                List<Folder> folderList = new List<Folder>();
                List<Folder> deletedfolderList = new List<Folder>();
                // Loop for each file that we've gotten
                foreach (File f in files)
                {
                    f.ParentFolder = this;
                    f.SetPath(this.FullPath + "\\" + f.Name);
                    if (!f.IsDeleted)
                    {
                        fileList.Add(f);
                    }
                    else
                    {
                        deletedfileList.Add(f);
                    }
                }
                foreach (Folder f in subFolders)
                {
                    f.ParentFolder = this;
                    f.LoadSTFSInfo = LoadSTFSInfo;
                    f.SetPath(this.FullPath + "\\" + f.Name);
                    if (!f.IsDeleted)
                    {
                        folderList.Add(f);
                    }
                    else
                    {
                        deletedfolderList.Add(f);
                    }
                }
                // Set the lists we have
                files = fileList;
                deletedFiles = deletedfileList;
                subFolders = folderList;
                deletedFolders = deletedfolderList;
            // If we just loaded the STFS info for each of the files above, set it so that
            // we know we just loaded it.  Man, I'm so tired right now and it's only 8:04.
            // what the hell...
                STFSInfoLoaded = LoadSTFSInfo;
                EntriesLoaded = true;
            }
            // If we aren't returning deleted entries
            if (!ShowDeletedEntries)
            {
                // Only return non-deleted files
                return subFolders.ToArray();
            }
            // We are returning deleted files
            else
            {
                // Return both non-deleted files and deleted files
                List<Folder> toReturn = new List<Folder>();
                toReturn.AddRange(subFolders);
                toReturn.AddRange(deletedFolders);
                return toReturn.ToArray();
            }
        }

        /// <summary>
        /// Injects a specified file in to this folder
        /// </summary>
        public File InjectFile(string FilePath, ref bool Cancel)
        {
            Write w = new Write(Drive);
            return w.WriteNewFile(this, FilePath, ref Cancel);
        }

        /// <summary>
        /// Injects a specified file in to this folder
        /// </summary>
        public File InjectFile(string FilePath, ref int Progress, ref int MaxProgress, ref bool Cancel)
        {
            Write w = new Write(Drive);
            return w.WriteNewFile(this, FilePath, ref Progress, ref MaxProgress, ref Cancel);
        }

        /// <summary>
        /// Injects a specified folder with all of its files in to this folder
        /// </summary>
        public List<ExistingEntry> InjectFolder(string FolderPath, bool Merge, bool MergeSubFolders, ref bool Cancel)
        {
            Write w = new Write(Drive);
            return w.InjectFolder(FolderPath, this, Merge, MergeSubFolders, ref Cancel);
        }

        /// <summary>
        /// Injects a specified folder with all of its files in to this folder
        /// </summary>
        public List<ExistingEntry> InjectFolder(string FolderPath, ref string CurrentFile, ref int Progress, ref int ProgressMax, bool Merge, bool MergeSubFolders, ref bool Cancel)
        {
            Write w = new Write(Drive);
            return w.InjectFolder(FolderPath, this, ref CurrentFile, ref Progress, ref ProgressMax, Merge, MergeSubFolders, ref Cancel);
        }

        /// <summary>
        /// Deletes all files, subfolders, and this folder
        /// </summary>
        public bool Delete(ref bool Cancel)
        {
            if (IsDeleted)
            {
                return true;
            }
            Write w = new Write(Drive);
            return w.Delete(this, ref Cancel);
        }

        /// <summary>
        /// Deletes all files, subfolders, and this folder
        /// </summary>
        public bool Delete(ref int ProgressUpdate, ref int ProgressMax, ref string CurrentEntry, ref bool Cancel)
        {
            if (IsDeleted)
            {
                return true;
            }
            Write w = new Write(Drive);
            return w.Delete(this, ref ProgressUpdate, ref ProgressMax, ref CurrentEntry, ref Cancel);
        }

        /// <summary>
        /// Deletes all subfolders within this folder, as well as their files
        /// </summary>
        public bool DeleteSubFolders(ref bool Cancel)
        {
            foreach (Folder f in SubFolders(false))
            {
                if (Cancel)
                {
                    return true;
                }
                if (!f.IsDeleted)
                {
                    f.DeleteSubFolders(ref Cancel);
                    f.DeleteFiles();
                    f.Delete(ref Cancel);
                }
            }
            return true;
        }

        public bool LoadSTFSInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Deletes all of the files within this folder
        /// </summary>
        public bool DeleteFiles()
        {
            foreach (File f in Files(false))
            {
                if (!f.IsDeleted)
                {
                    f.Delete();
                }
            }
            return true;
        }

        /// <summary>
        /// Creates a new folder within this folder
        /// </summary>
        public Folder NewFolder(string FolderName)
        {
            Write w = new Write(Drive);
            return w.NewFolder(FolderName, this);
        }

        /// <summary>
        /// Does some shit and returns a file from the given path
        /// </summary>
        //public File GetFileFromPath(string FullPath)
        //{
            
        //}
    }

    public sealed class File : Entry
    {
        System.IO.Stream stream;
        public bool IsSTFSPackage;
        public File(FATXDrive DRIVE, PartitionInfo partition):base(DRIVE, partition)
        {
            e = new Entries(this);
            Misc = new Misc();
        }

        public System.IO.Stream GetStream()
        {
            try
            {
                // Try closing the stream
                stream.Close();
            }
            catch { }
            // Re-intialize the stream
            if (Drive.DriveType == Info.DriveType.HDD)
            {
                stream = new FATXFileStream(Drive.GetHandle(), System.IO.FileAccess.ReadWrite, this);
            }
            else if (Drive.DriveType == Info.DriveType.Backup)
            {
                stream = new FATXFileStream(Drive.DumpPath, System.IO.FileMode.Open, this);
            }
            else if (Drive.DriveType == Info.DriveType.USB)
            {
                stream = new FATXFileStream(Drive.FilePaths(), this);
            }
            return stream;
        }

        public string GetTitleName()
        {
            // If the STFS info was already loaded...
            if (IsSTFSPackage && STFSInformation.Magic != null)
            {
                // Return it.
                return STFSInformation.TitleName;
            }
                // If the file appears to match the requirements for an STFS package...
            else if (Size >= 0xB000 && !Name.ToLower().Contains(".xex") && !Name.ToLower().Contains(".xbe"))
            {
                byte[] Magic = new STFS.STFSInfo(this).Magic();
                byte[] CON = Encoding.ASCII.GetBytes("CON ");
                byte[] LIVE = Encoding.ASCII.GetBytes("LIVE");
                byte[] PIRS = Encoding.ASCII.GetBytes("PIRS");
                if (ArrayComparer(Magic, CON) | ArrayComparer(Magic, LIVE) | ArrayComparer(Magic, PIRS))
                {
                    // Get the IO
                    IOReader io = new IOReader(GetStream());
                    io.BaseStream.Position = (long)STFS.STFSInfo.Offsets.TitleName;
                    string ss = "";
                    for (int i = 0; i < 0x80; i += 2)
                    {
                        char c = (char)io.ReadUInt16(true);
                        if (c != '\0')
                        {
                            ss += c;
                        }
                    }
                    io.Close();
                    return ss;
                }
            }
            return "";
        }

        public string GetPackageName()
        {
            // If the STFS info was already loaded...
            if (IsSTFSPackage && STFSInformation.Magic != null)
            {
                // Return it.
                return STFSInformation.ContentName;
            }
            // If the file appears to match the requirements for an STFS package...
            else if (Size >= 0xB000 && !Name.ToLower().Contains(".xex") && !Name.ToLower().Contains(".xbe"))
            {
                byte[] Magic = new STFS.STFSInfo(this).Magic();
                byte[] CON = Encoding.ASCII.GetBytes("CON ");
                byte[] LIVE = Encoding.ASCII.GetBytes("LIVE");
                byte[] PIRS = Encoding.ASCII.GetBytes("PIRS");
                if (ArrayComparer(Magic, CON) | ArrayComparer(Magic, LIVE) | ArrayComparer(Magic, PIRS))
                {
                    // Get the IO
                    IOReader io = new IOReader(GetStream());
                    io.BaseStream.Position = (long)STFS.STFSInfo.Offsets.DisplayName;
                    string ss = "";
                    for (int i = 0; i < 0x80; i += 2)
                    {
                        char c = (char)io.ReadUInt16(true);
                        if (c != '\0')
                        {
                            ss += c;
                        }
                    }
                    io.Close();
                    return ss;
                }
            }
            return "";
        }

        public bool IsNull
        {
            get
            {
                if (Size == 0)
                {
                    return true;
                }
                return false;
            }
        }

        public bool ArrayComparer(byte[] buffer1, byte[] buffer2)
        {
            if (buffer1.Length != buffer2.Length)
            {
                return false;
            }

            bool rval = true;
            for (int i = 0; i < buffer1.Length; i++)
            {
                if (buffer1[i] != buffer2[i])
                {
                    rval = false;
                    break;
                }
            }
            return rval;
        }

        public string SizeFriendly
        {
            get
            {
                return Misc.ByteConversion(Size);
            }
        }

        public void ForceSTFSInfo()
        {
            if (Size >= 0xA000 && !Name.ToLower().Contains(".xex") && !Name.ToLower().Contains(".xbe"))
            {

                STFS.STFSInfo stfsloader = new STFS.STFSInfo(this);
                byte[] CON = Encoding.ASCII.GetBytes("CON ");
                byte[] LIVE = Encoding.ASCII.GetBytes("LIVE");
                byte[] PIRS = Encoding.ASCII.GetBytes("PIRS");
                byte[] Magic = stfsloader.Magic();
                if (ArrayComparer(Magic, CON) | ArrayComparer(Magic, LIVE) | ArrayComparer(Magic, PIRS))
                {
                    STFSInformation = stfsloader.GetInfo();
                    IsSTFSPackage = true;
                }
            }
        }

        /// <summary>
        /// Size of the file in bytes
        /// </summary>
        public long Size
        {
            get { return EData.Size; }
        }

        /// <summary>
        /// Deletes the file from its parent folder
        /// </summary>
        public bool Delete()
        {
            if (IsDeleted)
            {
                return true;
            }
            Write w = new Write(Drive);
            return w.Delete(this);
        }

        /// <summary>
        /// Deletes the file from its parent folder
        /// </summary>
        public bool Delete(ref int ProgressUpdate, ref int ProgressMax, ref string CurrentFile)
        {
            if (IsDeleted)
            {
                return true;
            }
            Write w = new Write(Drive);
            return w.Delete(this, ref ProgressUpdate, ref ProgressMax, ref CurrentFile);
        }

        /// <summary>
        /// Overwrites this file with a new file (reduces time required to allocate new blocks depending on current file size, and new file size)
        /// </summary>
        public File OverWrite(string NewFilePath, ref bool Cancel)
        {
            Write w = new Write(Drive);
            return w.OverWriteFile(this, NewFilePath, ref Cancel);
        }

        /// <summary>
        /// Overwrites this file with a new file (reduces time required to allocate new blocks depending on current file size, and new file size)
        /// </summary>
        public File OverWrite(string NewFilePath, ref int Progress, ref int ProgressMax, ref bool Cancel)
        {
            Write w = new Write(Drive);
            return w.OverWriteFile(this, NewFilePath, ref Progress, ref ProgressMax, ref Cancel);
        }

        /// <summary>
        /// Extracts the file to a specified path
        /// </summary>
        public bool Extract(string OutPath, ref bool Cancel)
        {
            int garbage1 = 0;
            int garbage2 = 0;
            return Extract(OutPath, ref garbage1, ref garbage2, ref Cancel);
        }

        /// <summary>
        /// Extracts the file to a specified path
        /// </summary>
        public bool Extract(string OutPath, ref int SizeWritten, ref int MaxSize, ref bool Cancel)
        {
            Entries e = new Entries(this);
            return e.ExtractFile(this, OutPath, ref SizeWritten, ref MaxSize, ref Cancel);
        }

        public override uint[] BlocksOccupied
        {
            get {
                // Before we do any of this bullshit, let's first check to see
                // if the file is null
                if (IsNull)
                {
                    // The file is null, it has no allocated blocks
                    return new uint[0];
                }
                if (!IsDeleted)
                {
                    if (blocksOCCUPADO == null)
                    {
                        blocksOCCUPADO = e.GetBlocksOccupied(StartingCluster);
                    }
                }
                else
                {
                    if (blocksOCCUPADO == null)
                    {
                        long size = Size;
                        size = new Misc().UpToNearestCluster(size, this.PartInfo.ClusterSize);
                        uint[] blocks = new uint[size / PartInfo.ClusterSize];
                        blocks[0] = StartingCluster;
                        for (uint i = 1; i < blocks.Length; i++)
                        {
                            blocks[i] = blocks[0] + i;
                        }
                        blocksOCCUPADO = blocks;
                    }
                }
                return blocksOCCUPADO;
            }
            set
            {
                blocksOCCUPADO = value;
            }
        }
    }
}
