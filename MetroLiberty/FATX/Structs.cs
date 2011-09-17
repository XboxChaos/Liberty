using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FATX.Structs
{
    public struct EntryData
    {
        /// <summary>
        /// Size of the name
        /// </summary>
        public byte NameSize;
        /// <summary>
        /// Entry flags
        /// </summary>
        public byte Flags;
        /// <summary>
        /// Entry name
        /// </summary>
        public string Name;
        /// <summary>
        /// Beginning cluster (block) for the entry
        /// </summary>
        public UInt32 StartingCluster;
        /// <summary>
        /// Size of the entry (0x00 for folders)
        /// </summary>
        public UInt32 Size;
        public UInt16 CreationDate;
        public UInt16 CreationTime;
        public UInt16 AccessDate;
        public UInt16 AccessTime;
        public UInt16 ModifiedDate;
        public UInt16 ModifiedTime;
        /// <summary>
        /// Offset for the entry
        /// </summary>
        public long EntryOffset;
    }

    public struct STFSInfo
    {
        // public System.Drawing.Image ContentImage;
        // public System.Drawing.Image TitleImage;
        /// <summary>
        /// The identifier for the console the package was created one
        /// </summary>
        public byte[] ConsoleID;
        /// <summary>
        /// Identifier for the device that the package was assigned to
        /// </summary>
        public byte[] DeviceID;
        /// <summary>
        /// Package header magic
        /// </summary>
        public byte[] Magic;
        /// <summary>
        /// Identifier for the profile which the package is tied to (depending on transfer flags); the profile which the package was created with
        /// </summary>
        public byte[] ProfileID;
        /// <summary>
        /// Title (game/application) name
        /// </summary>
        public string TitleName;
        /// <summary>
        /// Local package name
        /// </summary>
        public string ContentName;
        /// <summary>
        /// Title (game/application) identifier
        /// </summary>
        public uint TitleID;
    }

    public struct PartitionInfo
    {
        /// <summary>
        /// Partition magic
        /// </summary>
        public string Magic;
        /// <summary>
        /// Cluster size
        /// </summary>
        public long ClusterSize;
        /// <summary>
        /// Partition ID
        /// </summary>
        public uint ID;
        /// <summary>
        /// Sectors per cluster
        /// </summary>
        public uint SectorsPerCluster;
        /// <summary>
        /// Number of FAT copies for the partition
        /// </summary>
        public uint FATCopies;
        /// <summary>
        /// FAT size for the partition
        /// </summary>
        public long FATSize;
        /// <summary>
        /// Data offset for the partition
        /// </summary>
        public long DataOffset;
        /// <summary>
        /// FAT offset for the partition
        /// </summary>
        public long FATOffset;
        /// <summary>
        /// Size of the partition
        /// </summary>
        public long Size;
        /// <summary>
        /// The partition itself
        /// </summary>
        public Partitions Partition;
        /// <summary>
        /// Partition offset
        /// </summary>
        public long Offset;
        /// <summary>
        /// Partition name
        /// </summary>
        public string Name;
        /// <summary>
        /// Entry size
        /// </summary>
        public Info.PartitionBit EntrySize;
        /// <summary>
        /// Number of clusters in the partition
        /// </summary>
        public uint Clusters;
        /// <summary>
        /// The REAL size of the file allocation table
        /// </summary>
        public long RealFATSize;
    }

    public struct ExistingEntry
    {
        // The type of entry...
        public Info.EntryType EntryType;
        // The entry that already exists
        public Entry Existing;
        // The path to the entry they tried writing
        public string NewPath;
    }

    public struct Partitions
    {
        public string Name;
        public long Offset;
    }

    public struct ProgressChangedEventArgs
    {
        public string FileName;
        public int Progress;
        public int ProgressToGo;
        public string ProgressAsPercentage
        {
            get
            {
                try
                {
                    return (((decimal)Progress / (decimal)ProgressToGo) * 100).ToString("#") + "%"; 
                }
                catch { return "0%"; } 
            }
        }
    }
}
