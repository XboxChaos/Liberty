using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FATX_Browser.Win32;
using FATX.Structs;

namespace FATX
{
    public class FATXDrive
    {
        public string DumpPath = "";
        FATX.Misc r = new FATX.Misc();
        //private bool isUSB = false;
        private int deviceID;
        private long dSize;
        private string sizeConverted;
        public FATX.Info.DriveType DriveType;
        Folder[] partitions;
        private IO.HDDFATX io;
        public FATX.IOReader br;
        public FATX.IOWriter bw;
        private PartitionInfo[] partinfo;
        private string dLabel = null;
        bool closed = false;
        Microsoft.Win32.SafeHandles.SafeFileHandle xHandle;
        string driveName = "";

        private bool dump;
        /// <summary>
        /// Provides information about a specific Xbox 360 Drive
        /// </summary>
        public FATXDrive(int ID, Info.DriveType dtype)
        {
            deviceID = ID;
            DriveType = dtype;
            ReadSize();
        }

        public string DriveName
        {
            get
            {
                // Return that shit if it's cached
                if (driveName != "")
                {
                    return driveName;
                }

                // Oh no, it's not cached...
                foreach (Folder f in Partitions)
                {
                    if (f.Name == "Data")
                    {
                        foreach (File Files in f.Files(false))
                        {
                            if (Files.Name.ToLower() == "name.txt")
                            {
                                IOReader io = new IOReader(Files.GetStream());
                                io.BaseStream.Position = 2;
                                driveName = io.ReadUnicodeString((int)io.BaseStream.Length - 2);// +" " + DriveSizeConverted;
                                io.Close();
                                return driveName;
                            }
                        }
                    }
                }

                // Hax: fall back to the volume label if there is one
                if (dLabel != null && dLabel.Length > 0)
                    return dLabel;

                // That shit wasn't found... just return the drive type
                return DriveType.ToString();// +" " + DriveSizeConverted;
            }
            private set
            {
                driveName = value;
            }
        }

        public long RemainingSpace()
        {
            // Size up to the first user-partition (non-cache, non-gay)
            long size = 0;
            for (int i = 0; i < Partitions.Length; i++)
            {
                size += new FATStuff(this, Partitions[i]).GetFreeSpace();
            }
            return size;
        }

        public long PartitionSizeTotal()
        {
            long size = 0;
            for (int i = 0; i < Partitions.Length; i++)
            {
                size += Partitions[i].PartInfo.Size - (Partitions[i].PartInfo.DataOffset - Partitions[i].PartInfo.Offset);
            }
            return size;
        }

        public Microsoft.Win32.SafeHandles.SafeFileHandle GetHandle()
        {
            try { xHandle.Close(); }
            catch { }
            CloseIO();
            // Gotta multi-line this shit so I'm not scrolling all the way across my screen
            xHandle = API.CreateFile(@"\\.\PhysicalDrive" + this.DeviceID.ToString(),
                System.IO.FileAccess.ReadWrite,
                System.IO.FileShare.ReadWrite,
                IntPtr.Zero, System.IO.FileMode.Open,
                API.FlagsAndAttributes.Device | API.FlagsAndAttributes.NoBuffering | API.FlagsAndAttributes.Write_Through,
                IntPtr.Zero);
            return xHandle;
        }

        public FATXDrive(string dumpPath, Info.DriveType dtype, string volumeLabel)
        {
            if (dtype == Info.DriveType.Backup)
            {
                IsDump = true;
            }
            else if (dtype == Info.DriveType.USB)
            {
                IsUSB = true;
            }
            DriveType = dtype;
            DumpPath = dumpPath;
            dLabel = volumeLabel;
            ReadSize();
        }

        public Folder[] Partitions
        {
            get
            {
                if (partitions != null)
                {
                    return partitions;
                }

                partitions = new Partitions().Get(this);
                return partitions;
            }
        }

        public string[] FilePaths()
        {
            System.IO.FileInfo[] fi = new System.IO.DirectoryInfo(DumpPath).GetFiles();
            string[] FilePaths = new string[fi.Length];
            for (int i = 0; i < FilePaths.Length; i++)
            {
                string data = "\\Data";
                if (i.ToString().Length == 1)
                    data += "000";
                else if (i.ToString().Length == 2)
                    data += "00";
                else if (i.ToString().Length == 3)
                    data += "0";
                FilePaths[i] = fi[i].Directory + data + i.ToString();
            }
            return FilePaths;
        }

        public bool IsDev
        {
            get;
            set;
        }
        #region Ugly Code
        public void ReadSize()
        {
            GetIO();
            if (IsDump)
            {
                dSize = io.GetDriveSize();
            }
            else if (IsUSB)
            {
                dSize = br.BaseStream.Length;
            }
            else
            {
                dSize = io.GetDriveSize(DeviceID);
            }
            sizeConverted = r.ByteConversion(dSize);
        }

        internal FATX.IOReader GetIO()
        {
            CloseIO();
            if (DriveType == Info.DriveType.HDD)
            {
                io = new IO.HDDFATX(false, null);
                br = io.br_diskReader(DeviceID);
            }
            else if (DriveType == Info.DriveType.Backup)
            {
                io = new IO.HDDFATX(true, DumpPath);
                br = io.br_dumpReader();
            }
            else if (DriveType == Info.DriveType.USB)
            {
                br = new IOReader(FilePaths());
            }
            return br;
        }

        internal FATX.IOWriter GetWriterIO()
        {
            if (DriveType == Info.DriveType.HDD)
            {
                CloseIO();
                io = new IO.HDDFATX(false, null);
                bw = io.bw_diskWriter(DeviceID);
            }
            else if (DriveType == Info.DriveType.Backup)
            {
                CloseIO();
                io = new IO.HDDFATX(true, DumpPath);
                bw = io.bw_dumpWriter();
            }
            else if (DriveType == Info.DriveType.USB)
            {
                CloseIO();
                System.IO.FileInfo[] fi = new System.IO.DirectoryInfo(DumpPath).GetFiles();
                string[] FilePaths = new string[fi.Length];
                for (int i = 0; i < FilePaths.Length; i++)
                {
                    FilePaths[i] = fi[i].FullName;
                }
                bw = new IOWriter(FilePaths);
            }
            return bw;
        }
        #endregion

        public bool IsDump
        {
            get { return dump; }
            set { dump = value; }
        }

        public bool IsUSB
        {
            get;
            set;
        }

        public void ReadData()
        {
            partitions = new FATX.Partitions().Get(this);
        }

        /// <summary>
        /// ID for the device (used for the Win32 API)
        /// </summary>
        public int DeviceID
        {
            get { return deviceID; }
            set { deviceID = value; }
        }

        /// <summary>
        /// Drive size in bytes
        /// </summary>
        public long DriveSize
        {
            get { return dSize; }
        }

        /// <summary>
        /// Drive size converted to KB/MB/GB
        /// </summary>
        public string DriveSizeConverted
        {
            get { return sizeConverted; }
        }

        public bool IsClosed
        {
            get
            {
                return closed;
            }
        }

        public bool Open()
        {
            if (IsClosed)
            {
                io = new IO.HDDFATX(false, null);
            }
            return true;
        }

        public bool Close()
        {
            try
            {
                io.Close();
            }
            catch{}
            try
            {
                bw.Close();
            }
            catch
            {}
            try
            {
                br.Close();
            }
            catch{}
            closed = true;
            return true;
        }

        void CloseIO()
        {
            try
            {
                io.Close();
            }
            catch { }
            try
            {
                bw.Close();
            }
            catch
            { }
            try
            {
                br.Close();
            }
            catch { }
        }

        public PartitionInfo[] GetPartInfo
        {
            get
            {
                return partinfo;
            }

            set
            {
                partinfo = value;
            }
        }

        public bool BackupDrive(string savePath)
        {
            long garbage = 0;
            return BackupDrive(ref garbage, savePath);
        }

        public bool BackupDrive(ref long Progress, string savePath)
        {
            FATX.IOWriter fileWriter = null;
            FATX.IOReader driveReader = null;
            try
            {
                long driveSize =DriveSize;
                fileWriter = new FATX.IOWriter(new System.IO.FileStream(savePath, System.IO.FileMode.Create));
                driveReader = this.GetIO();
                for (long i = 0; i < driveSize; i += 0x4000, Progress += 0x4000)
                {
                    byte[] buffer = driveReader.ReadBytes(0x4000);
                    fileWriter.Write(buffer);
                }
                fileWriter.Close();
                driveReader.Close();
                return true;
            }
            catch { try { fileWriter.Close(); driveReader.Close(); } catch { } return false; }
        }

        public bool ExtractJosh(string outFile)
        {
            if (DriveType != Info.DriveType.HDD)
            {
                throw new Exception("Drive is not a hard drive");
            }
            //Create our io for the drive
            IOReader io = GetIO();
            //Go to the location of the security sector
            io.BaseStream.Position = 0x800;
            //Create our ref io for the file
            IOWriter bw = new IOWriter(new System.IO.FileStream(outFile, System.IO.FileMode.Create));
            //Read the sector.  The size is an estimation, since I have no idea how big it really is
            bw.Write(io.ReadBytes(0x400));
            //Close our io's
            io.Close();
            bw.Close();
            return true;
        }

        public bool ExtractSS(string outFile)
        {
            if (DriveType != Info.DriveType.HDD)
            {
                throw new Exception("Drive is not a hard drive");
            }
            //Create our io for the drive
            IOReader io = GetIO();
            //Go to the location of the security sector
            io.BaseStream.Position = 0x2000;
            //Create our ref io for the file
            IOWriter bw = new IOWriter(new System.IO.FileStream(outFile, System.IO.FileMode.Create));
            //Read the sector.  The size is an estimation, since I have no idea how big it really is
            bw.Write(io.ReadBytes(0xE00));
            //Close our io's
            io.Close();
            bw.Close();
            return true;
        }

        public bool RestoreSS(string inFile)
        {
            return true;
        }

        public bool RestoreBackup(string inFile)
        {
            long p = 0;
            return RestoreBackup(ref p, inFile);
        }

        public bool RestoreBackup(ref long Progress, string inFile)
        {
            FATX.IOReader fileReader = null;
            FATX.IOWriter driveWriter = null;
            try
            {
                long driveSize = DriveSize;
                fileReader = new FATX.IOReader(new System.IO.FileStream(inFile, System.IO.FileMode.Open));
                driveWriter = this.GetWriterIO();
                for (long i = 0; i < driveSize; i += 0x4000, Progress += 0x4000)
                {
                    byte[] buffer = fileReader.ReadBytes(0x4000);
                    driveWriter.Write(buffer);
                }
                driveWriter.Close();
                fileReader.Close();
                return true;
            }
            catch { try { fileReader.Close(); driveWriter.Close(); } catch { } return false; }
        }
    }
}
