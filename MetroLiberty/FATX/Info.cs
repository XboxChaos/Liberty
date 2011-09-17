using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FATX
{
    public static class Info
    {
        public enum EntryType
        {
            DeletedFolder,
            DeletedFile,
            Folder,
            File,
            Partition,
        }
        public enum IOType
        {
            Read,
            Write,
            Delete,
            Overwrite,
            WriteFolder,
            WriteFile,
            WriteFiles,
        }
        public enum USBOffsets
        {
            Cache = 0x8000400,
            Data = 0x20000000,
        }

        public enum USBPartitionSizes
        {
            Cache = 0x47FF000,
        }

        public enum NANDOffsets
        {
            System = 0x3190800,
            Cache = 0x3CE8800,
            Data = 0x1E600300,
        }

        public enum PartitionBit
        {
            FATX32 = 4,
            FATX16 = 2,
            None = 0x00,
        }

        public enum DriveType
        {
            /// <summary>
            /// USB drive
            /// </summary>
            USB,
            /// <summary>
            /// Physical disk
            /// </summary>
            HDD,
            /// <summary>
            /// File?
            /// </summary>
            File,
            /// <summary>
            /// Disk Backup
            /// </summary>
            Backup,
            /// <summary>
            /// NAND Backup
            /// </summary>
            NAND,
        }

        public enum Flags : byte
        {
            ReadOnly,
            Hidden,
            System,
            Volume,
            Subdirectory,
            Archive,
            Device,
            Unused,
            Deleted = 0xE5,
        }

        public class HDDFATX
        {

            public enum EntryOffsets : int
            {
                /// <summary>
                /// Offset for the size of the file name
                /// </summary>
                NameSize = 0x0,
                /// <summary>
                /// Offset for file flags
                /// </summary>
                Flags = 0x1,
                /// <summary>
                /// Offset for file name
                /// </summary>
                FileName = 0x2,
                /// <summary>
                /// Offset for starting cluster
                /// </summary>
                StartingCluster = 0x2C,
                /// <summary>
                /// Offset for the file size
                /// </summary>
                Size = 0x30,
                CreationDate = 0x34,
                CreationTime = 0x36,
                AccessDate = 0x38,
                AccessTime = 0x3A,
                ModifiedDate = 0x3C,
                ModifiedTime = 0x3E,
            }

            public enum Lengths : long
            {
                /// <summary>
                /// Length of the System Cache partition
                /// </summary>
                SystemCache = 0x80000000,
                /// <summary>
                /// Length of the Game Cache partition
                /// </summary>
                GameCache = 0xA0E30000,
                /// <summary>
                /// Length of the Compatibility partition
                /// </summary>
                Compatibility = 0x10000000,
                /// <summary>
                /// The maximum file size allowed by FATX
                /// </summary>
                MaxFileSize = 0x100000000,
                /// <summary>
                /// Sector size
                /// </summary>
                SectorSize = 0x200,
            }

            public enum FATX16BlockType
            {
                /// <summary>
                /// Root block for the partition
                /// </summary>
                Root = 0xFFF8,
                /// <summary>
                /// Free block
                /// </summary>
                Unused = 0x0000,
                /// <summary>
                /// Bad block
                /// </summary>
                Bad = 0xFFF7,
                /// <summary>
                /// End of a series of blocks
                /// </summary>
                End = 0xFFFF,
            }

            public enum FATX32BlockType : uint
            {
                /// <summary>
                /// Root block for the partition
                /// </summary>
                Root = 0xFFFFFFF8,
                /// <summary>
                /// Free block
                /// </summary>
                Unused = 0x00000000,
                /// <summary>
                /// Bad block
                /// </summary>
                Bad = 0xFFFFFFF7,
                /// <summary>
                /// End of a series of blocks
                /// </summary>
                End = 0xFFFFFFFF,
            }

            public enum DevOffsets : long
            {
                DEVKIT_ = 0xB6600000,
                Content = 0xC6600000,
            }

            public enum Partitions : long
            {
                /// <summary>
                /// Offset of the data partition
                /// </summary>
                Data = 0x130EB0000,
                /// <summary>
                /// Offset of the Josh partition
                /// </summary>
                Josh = 0x800,
                /// <summary>
                /// Offset of the Security Sector
                /// </summary>
                SecuritySector = 0x2000,
                /// <summary>
                /// Offset of the System Cache partition
                /// </summary>
                SystemCache = 0x80000,
                /// <summary>
                /// Offset of the Game Cache partition
                /// </summary>
                GameCache = 0x80080000,
                /// <summary>
                /// Offset of the Compatibility partition
                /// </summary>
                Compatibility = 0x120EB0000,
            }

        }
    }
}
