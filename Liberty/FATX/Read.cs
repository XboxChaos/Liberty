using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FATX;
using FATX.Structs;

namespace FATX
{
    public class FATStuff
    {
        FATXDrive ourDrive;
        public FATX.Info.PartitionBit bit = Info.PartitionBit.FATX32;
        int entrySize = 0;
        PartitionInfo Partition;

        /// <summary>
        /// Provides partition/FAT information
        /// </summary>
        public FATStuff(FATXDrive Drive, Folder partition)
        {
            ourDrive = Drive;
            Partition = partition.PartInfo;
        }

        public FATStuff(FATXDrive Drive, long partitionOffset)
        {
            ourDrive = Drive;
            Partition.Name = "Root";
            Partition.Offset = partitionOffset;
        }

        public FATStuff(FATXDrive Drive, Info.HDDFATX.Partitions p)
        {
            ourDrive = Drive;
            Partition.Name = p.ToString();
            Partition.Offset = (long)p;
        }

        public FATStuff(FATXDrive Drive, Info.HDDFATX.DevOffsets p)
        {
            ourDrive = Drive;
            Partition.Name = p.ToString();
            Partition.Offset = (long)p;
        }

        /// <summary>
        /// Provides partition/FAT information
        /// </summary>
        public FATStuff(Entry entry)
        {
            ourDrive = (FATXDrive)entry.Drive;
            Partition = entry.PartInfo;
        }

        public uint Blocks()
        {
            //Take the partition size - data offset because that's the true amount of blocks we have
            //(otherwise we are assuming that the entire file is able to have data written to it)

            //FUCK
            //THAT
            //SHIT
            //get an infinite loop, ya dig?
            return (uint)(PartitionSize() / ClusterSize());
        }

        public uint RootDirectoryCluster()
        {
            // Open our IO
            IOReader io = ourDrive.GetIO();
            // Set the IO position...
            io.BaseStream.Position = Partition.Offset;
            // Read our buffer
            byte[] buffer = io.ReadBytes(0x200);
            // Close the IO
            io.Close();
            // Re-open the IO in to a memory stream
            io = new IOReader(new System.IO.MemoryStream(buffer));
            // Go to the offset that the root dir is located; 0xC
            io.BaseStream.Position = 0xC;
            // Read and return the int there
            return io.ReadUInt32(true);
        }

        public int EntrySize
        {
            get
            {
                if (entrySize == 0)
                {
                    if (Partition.Offset == 0x20000000 && ourDrive.IsUSB)
                    {
                        entrySize = 4;
                        bit = Info.PartitionBit.FATX32;
                    }
                    else
                    {
                        uint blocks = Blocks();
                        if (blocks < 0xFFF0)
                        {
                            bit = Info.PartitionBit.FATX16;
                            entrySize = 2;
                        }
                        else
                        {
                            bit = Info.PartitionBit.FATX32;
                            entrySize = 4;
                        }
                    }
                }
                return entrySize;
            }
        }

        public long RealFATSize()
        {
            long ValToReturn = (((Partition.Offset + PartitionSize()) - DataOffset()) / ClusterSize()) * EntrySize;
            return ValToReturn;
        }

        /// <summary>
        /// Returns an array of free blocks based off of the number of blocks needed
        /// </summary>
        public uint[] GetFreeBlocks(int blocksNeeded, uint StartBlock, long end, bool SecondLoop)
        {
            int Clustersize = 0x1000;
            uint Block = StartBlock;
            if (end == 0)
            {
                end = FATOffset + RealFATSize();
            }
            List<uint> BlockList = new List<uint>();
            Misc m = new Misc();
            for (long i = m.DownToNearest200(m.BlockToFATOffset(StartBlock, Partition)); i < end; i += Clustersize)
            {
                //Create our reader
                FATX.IOReader br = ourDrive.GetIO();
                //Set our position to i
                br.BaseStream.Position = i;
                byte[] buffer = new byte[0];
                if ((end - i) < Clustersize )
                {
                    buffer = m.ReadBytes(ref br, end - i);
                }
                else
                {
                    //Read our buffer
                    buffer = br.ReadBytes(Clustersize);
                }
                br.Close();
                //Re-open our binary reader using the buffer/memory stream
                for (int j = 0; j < Clustersize; j+= (int)Partition.EntrySize, Block += (uint)Partition.EntrySize)
                {
                    br = new FATX.IOReader(new System.IO.MemoryStream(buffer));
                    br.BaseStream.Position = j;
                    //If we've gotten all of our requested blocks...
                    if (BlockList.ToArray().Length == blocksNeeded)
                    {
                        //Close our reader -> break the loop
                        br.Close();
                        break;
                    }
                    //Read the next block entry
                    byte[] reading = br.ReadBytes((int)Partition.EntrySize);
                    //Close our reader - it's no longer needed
                    br.Close();
                    //For each byte in our reading
                    for (int k = 0; k < reading.Length; k++)
                    {
                        //If the byte isn't null (if the block isn't open)
                        if (reading[k] != 0x00)
                        {
                            //Break
                            break;
                        }
                        //If we've reached the end of the array, and the last byte
                        //is 0x00, then the block is free
                        if (k == reading.Length - 1 && reading[k] == 0x00)
                        {
                            //Do some maths to get the block numbah
                            long fOff = FATOffset;
                            long blockPosition = (long)i + j;
                            uint block = (uint)(blockPosition - fOff) / (uint)EntrySize;
                            BlockList.Add(block);
                        }
                    }
                }
                //We're putting in one last check so that we don't loop more than we need to
                if (BlockList.ToArray().Length == blocksNeeded)
                {
                    break;
                }
            }
            //If we found the required amount of free blocks - return our list
            if (BlockList.Count == blocksNeeded)
            {
                return BlockList.ToArray();
            }
            //If we didn't find the amount of blocks required, but we started from a
            //block other than the first one...
            if (BlockList.Count < blocksNeeded && SecondLoop == false)
            {
                BlockList.AddRange(GetFreeBlocks(blocksNeeded - BlockList.Count, 1, m.DownToNearest200(m.BlockToFATOffset(StartBlock, Partition)), true));
                return BlockList.ToArray();
            }
            //We didn't find the amount of free blocks required, meaning we're ref of
            //disk space
            if (BlockList.Count != blocksNeeded)
            {
                throw new Exception("Out of Xbox 360 hard disk space");
            }
            return BlockList.ToArray();
        }

        /// <summary>
        /// Partition magic
        /// </summary>
        public string Magic()
        {
            string rVal = "";
            FATX.IOReader br = ourDrive.GetIO();
            br.BaseStream.Position = Partition.Offset;
            //Read the header
            byte[] header = br.ReadBytes(0x4);
            
            br.Close();
            //Convert the header to a string
            rVal = Encoding.ASCII.GetString(header);
            return rVal;
        }

        /// <summary>
        /// Sector size (in bytes)
        /// </summary>
        public long SectorSize
        {
            get { return 0x200; }
        }

        /// <summary>
        /// Cluster size (in bytes)
        /// </summary>
        public long ClusterSize()
        {
            return SectorSize * SectorsPerCluster();
        }

        /// <summary>
        /// Partition ID
        /// </summary>
        public uint PartitionID()
        {
            uint rVal = 0;
            Misc m = new Misc();
            //Open our binary reader
            FATX.IOReader br = ourDrive.GetIO();
            //Seek to the data partition offset
            br.BaseStream.Position = Partition.Offset;
            //Read our buffer
            FATX.IOReader mem = new FATX.IOReader(new System.IO.MemoryStream(br.ReadBytes(0x200)));
            br.Close();
            mem.BaseStream.Position = 0x4;
            rVal = m.ReadUInt32(ref mem);
            mem.Close();
            return rVal;
        }

        /// <summary>
        /// Sectors per cluster
        /// </summary>
        public uint SectorsPerCluster()
        {
            uint rVal = 0;
            Misc m = new Misc();
            //Open our binary reader
            FATX.IOReader br = ourDrive.GetIO();
            //Seek to the data partition offset + 0x8 (cluster size location)
            br.BaseStream.Position = Partition.Offset;
            //Create our mem reader / buffer
            FATX.IOReader mem = new FATX.IOReader(new System.IO.MemoryStream(br.ReadBytes(0x200)));
            br.Close();
            mem.BaseStream.Position = 0x8;
            //Get our value (uses outside class for bigendian)
            rVal = m.ReadUInt32(ref mem);
                mem.Close();
            return rVal;
        }

        /// <summary>
        /// Number of File Allocation Tables
        /// </summary>
        public uint FATCopies()
        {
            uint rVal = 0;
            Misc m = new Misc();
            //Open our binary reader
            FATX.IOReader br = ourDrive.GetIO();
            //Seek to the data partition offset + 0xC (where the FATCopies int is)
            br.BaseStream.Position = Partition.Offset;
            //Create our mem reader / buffer
            FATX.IOReader mem = new FATX.IOReader(new System.IO.MemoryStream(br.ReadBytes(0x200)));
            br.Close();
            mem.BaseStream.Position = 0xC;
            //Get our value (uses outside class for bigendian)
            rVal = m.ReadUInt32(ref mem);
            mem.Close();
            return rVal;
        }

        /// <summary>
        /// TOTAL size (includes padding) of the File Allocation Table (in bytes)
        /// FOR REAL SIZE, CALL TO RealFATSize();
        /// </summary>
        public long FATSize()
        {
            #region old
            //long size = 0;
            //if (Partition.Offset == 0x20000000 && ourDrive.IsUSB)
            //{
            //    System.IO.FileInfo fi = new System.IO.FileInfo(ourDrive.DumpPath + "\\Data0001");
            //    size = fi.Length - 0x1000;
            //    //Return the size.
            //    return size;
            //}
            //else
            //{
            //    //This gets the size
            //    size = ((PartitionSize() / ClusterSize()) * EntrySize);
            //    //We need to round up to the nearest 0x1000 byte boundary.
            //    long sizeToAdd = (0x1000 - (size % 0x1000));
            //    if (!ourDrive.IsUSB)
            //    {
            //        size += sizeToAdd;
            //    }
            //    //Return the size.
            //    return size;
            //}
            #endregion
            //Code that rounds up to nearest cluster...
            long size = 0;
            #region shit
            //if (Partition.Offset == 0x20000000 && ourDrive.IsUSB)
            //{
            //    System.IO.FileInfo fi = new System.IO.FileInfo(ourDrive.DumpPath + "\\Data0001");
            //    size = fi.Length - 0x1000;
            //    //Ghetto
            //    IOReader ir = ourDrive.GetIO();
            //    //Return the size.
            //    return size;
            //}
            //else
            //{
            #endregion
            //This gets the size
                size = (((PartitionSize() / ClusterSize())) * EntrySize);
                //We need to round up to the nearest blabhlabhalkhdflkasdf byte boundary.
                size = new Misc().UpToNearestCluster(size + 0x1000, ClusterSize() / EntrySize) - 0x1000;
                //long sizeToAdd = (0x1000 - (size % 0x1000));
                //size += sizeToAdd;
                //Return the size.
                return size;
            //}
        }

        public long GetFreeSpace()
        {
            // Our return
            long Return = 0;
            long ClusterSize = this.ClusterSize();

            // Get our position
            long positionya = FATOffset;

            // Get our end point
            long toBeLessThan = FATOffset + RealFATSize();

            // Get our IO
            IOReader io = ourDrive.GetIO();
            // Set the position
            io.BaseStream.Position = positionya;

            // Start reading!
            for (long dick = io.BaseStream.Position; dick < toBeLessThan; dick += 0x200)
            {
                bool BreakAndShit = false;
                // Set the position
                io.BaseStream.Position = dick;
                // Read our buffer
                byte[] Buffer = io.ReadBytes(0x200);
                // Length to loop for (used for the end so we can read ONLY usable partitions)
                long Length = Buffer.Length;
                if (dick == new Misc().DownToNearest200(toBeLessThan))
                {
                    Length = toBeLessThan - new Misc().DownToNearest200(toBeLessThan);
                    BreakAndShit = true;
                }
                // Check the values
                for (int i = 0; i < Length; i+= EntrySize)
                {
                    // This size will be off by a few megabytes, no big deal in my opinion
                    IOReader ioya = new IOReader(new System.IO.MemoryStream(Buffer));
                    if (EntrySize == 2)
                    {
                        if (ioya.ReadInt16() == 0)
                        {
                            Return += ClusterSize;
                        }
                    }
                    else
                    {
                        if (ioya.ReadInt32() == 0)
                        {
                            Return += ClusterSize;
                        }
                    }
                }
                if (BreakAndShit)
                {
                    break;
                }
            }
            io.Close();

             return Return;
        }


        /// <summary>
        /// Returns the root offset for where data begins in the partition
        /// </summary>
        public long DataOffset()
        {
            //We take the FAT start + the FAT size to get the data offset
            long off =  (Partition.Offset + 0x1000 + FATSize());
            IOReader ir = ourDrive.GetIO();
            ir.BaseStream.Position = off;
            if (ir.ReadInt32() == 0x0)
            {
                off += 0x1000;
            }
            ir.Close();
            return off;
        }

        /// <summary>
        /// Offset of the File Allocation Table
        /// </summary>
        public long FATOffset
        {
            get { return Partition.Offset + 0x1000; }
        }

        /// <summary>
        /// Total size of the partition (in bytes)
        /// </summary>
        public long PartitionSize()
        {
            if (Partition.Size == 0)
            {
                long psize = 0;
                Misc m = new Misc();
                if (ourDrive.DriveType == Info.DriveType.Backup || ourDrive.DriveType == Info.DriveType.HDD)
                {
                    // If we're working with a dev drive
                    if (ourDrive.IsDev)
                    {
                        // If we're on the DEVKIT? partition...
                        if (Partition.Offset == (long)Info.HDDFATX.DevOffsets.DEVKIT_)
                        {
                            // This is simply for some testing purposes... shit's not permanent yet
                            psize = 0x11FFD000;
                        }
                        else
                        {
                            psize = 0x2E22DF000;
                        }
                    }
                    else
                    {
                        switch (Partition.Offset)
                        {
                            case (long)Info.HDDFATX.Partitions.Compatibility:
                                psize = (long)Info.HDDFATX.Lengths.Compatibility;
                                break;
                            case (long)Info.HDDFATX.Partitions.Data:
                                if (ourDrive.DriveType == Info.DriveType.HDD)
                                {
                                    FATX.IO.HDDFATX hddio = new IO.HDDFATX(false, null);
                                    psize = (ourDrive.DriveSize - Partition.Offset);
                                }
                                else
                                {
                                    psize = (ourDrive.DriveSize - Partition.Offset);
                                }
                                break;
                            case (long)Info.HDDFATX.Partitions.GameCache:
                                psize = (long)Info.HDDFATX.Lengths.GameCache;
                                break;
                            case (long)Info.HDDFATX.Partitions.SystemCache:
                                psize = (long)Info.HDDFATX.Lengths.SystemCache;
                                break;
                        }
                    }
                }
                else
                {
                    switch (Partition.Offset)
                    {
                        case (long)Info.USBOffsets.Cache:
                            psize = (long)Info.USBPartitionSizes.Cache;
                            break;
                        case (long)Info.USBOffsets.Data:
                            psize = ourDrive.DriveSize - (long)Info.USBOffsets.Data;
                            break;
                    }
                }
                if (psize == 0)
                {
                    psize = ourDrive.GetIO().BaseStream.Length;
                }
                Partition.Size = psize;
            }
                return Partition.Size;
        }
    }

    /// <summary>
    /// A class with one function that I didn't feel belonged in any other class, so I gave it its own.
    /// </summary>
    public class Partitions
    {
        /// <summary>
        /// Returns the readable partitions as folders on the drive
        /// </summary>
        public Folder[] Get(FATXDrive xDrive)
        {
            List<Folder> PIList = new List<Folder>();
            if (xDrive.DriveType == Info.DriveType.HDD || xDrive.DriveType == Info.DriveType.Backup)
            {
                // For dev drives
                if (xDrive.IsDev)
                {
                    foreach (Info.HDDFATX.DevOffsets e in Enum.GetValues(typeof(Info.HDDFATX.DevOffsets)))
                    {
                        FATStuff FS = new FATStuff(xDrive, e);
                        if (FS.Magic() == "XTAF")
                        {
                            PartitionInfo PI = new PartitionInfo();
                            PI.ClusterSize = FS.ClusterSize();
                            PI.DataOffset = FS.DataOffset();
                            PI.FATCopies = FS.FATCopies();
                            PI.FATOffset = FS.FATOffset;
                            PI.FATSize = FS.FATSize();
                            PI.ID = FS.PartitionID();
                            PI.Magic = FS.Magic();
                            PI.Name = e.ToString();
                            PI.Offset = (long)e;
                            PI.SectorsPerCluster = FS.SectorsPerCluster();
                            PI.EntrySize = FS.bit;
                            PI.Size = FS.PartitionSize();
                            PI.Clusters = FS.Blocks();
                            // YEAH
                            PI.RealFATSize = FS.RealFATSize();
                            Folder f = new Folder(xDrive, PI);
                            f.Name = PI.Name;
                            f.EData.StartingCluster = FS.RootDirectoryCluster();
                            f.SetPath(PI.Name);
                            PIList.Add(f);
                        }
                    }
                }
                    // For other drives
                else
                {
                    foreach (Info.HDDFATX.Partitions e in Enum.GetValues(typeof(Info.HDDFATX.Partitions)))
                    {
                        if (e == Info.HDDFATX.Partitions.Data || e == Info.HDDFATX.Partitions.Compatibility)
                        {
                            FATStuff FS = new FATStuff(xDrive, e);
                            if (FS.Magic() == "XTAF")
                            {
                                PartitionInfo PI = new PartitionInfo();
                                PI.ClusterSize = FS.ClusterSize();
                                PI.DataOffset = FS.DataOffset();
                                PI.FATCopies = FS.FATCopies();
                                PI.FATOffset = FS.FATOffset;
                                PI.FATSize = FS.FATSize();
                                PI.ID = FS.PartitionID();
                                PI.Magic = FS.Magic();
                                PI.Name = e.ToString();
                                PI.Offset = (long)e;
                                PI.SectorsPerCluster = FS.SectorsPerCluster();
                                PI.EntrySize = FS.bit;
                                PI.Size = FS.PartitionSize();
                                PI.Clusters = FS.Blocks();
                                PI.RealFATSize = FS.RealFATSize();
                                Folder f = new Folder(xDrive, PI);
                                f.Name = PI.Name;
                                f.EData.StartingCluster = FS.RootDirectoryCluster();
                                f.SetPath(PI.Name);
                                PIList.Add(f);
                            }
                        }
                    }
                }

                // For single-partition files
                if (PIList.Count == 0)
                {
                    FATStuff FS = new FATStuff(xDrive, (long)0);
                    if (FS.Magic() == "XTAF")
                    {
                        PartitionInfo PI = new PartitionInfo();
                        PI.ClusterSize = FS.ClusterSize();
                        PI.DataOffset = FS.DataOffset();
                        PI.FATCopies = FS.FATCopies();
                        PI.FATOffset = FS.FATOffset;
                        PI.FATSize = FS.FATSize();
                        PI.ID = FS.PartitionID();
                        PI.Magic = FS.Magic();
                        PI.Name = "Root";
                        PI.Offset = 0x0;
                        PI.SectorsPerCluster = FS.SectorsPerCluster();
                        PI.EntrySize = FS.bit;
                        PI.Size = FS.PartitionSize();
                        PI.Clusters = FS.Blocks();
                        PI.RealFATSize = FS.RealFATSize();
                        Folder f = new Folder(xDrive, PI);
                        f.EData.StartingCluster = FS.RootDirectoryCluster();
                        f.Name = PI.Name;
                        f.SetPath(PI.Name);
                        PIList.Add(f);
                    }
                }
            }
                // USB Drives
            else if (xDrive.DriveType == Info.DriveType.USB)
            {
                foreach (Info.USBOffsets e in Enum.GetValues(typeof(Info.USBOffsets)))
                {
                    FATStuff FS = new FATStuff(xDrive, (long)e);

                    if (FS.Magic() == "XTAF")
                    {
                        PartitionInfo PI = new PartitionInfo();
                        PI.ClusterSize = FS.ClusterSize();
                        PI.DataOffset = FS.DataOffset();
                        PI.FATCopies = FS.FATCopies();
                        PI.FATOffset = FS.FATOffset;
                        PI.FATSize = FS.FATSize();
                        PI.ID = FS.PartitionID();
                        PI.Magic = FS.Magic();
                        PI.Name = e.ToString();
                        PI.Offset = (long)e;
                        PI.SectorsPerCluster = FS.SectorsPerCluster();
                        PI.EntrySize = FS.bit;
                        PI.Size = FS.PartitionSize();
                        PI.Clusters = FS.Blocks();
                        PI.RealFATSize = FS.RealFATSize();
                        Folder f = new Folder(xDrive, PI);
                        f.EData.StartingCluster = FS.RootDirectoryCluster();
                        f.Name = PI.Name;
                        f.SetPath(PI.Name);
                        PIList.Add(f);
                    }
                }
            }
            // Shit never showed up, don't think anyone used it anyway
            //else if (PIList.Count == 0)
            //{
            //    if (System.Windows.Forms.MessageBox.Show("No partitions were found.  Would you like to use the Manual Partition tool to set the offset yourself?", "No Partitions Found", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            //    {
            //        PartitionInfo PI = new PartitionInfo();
            //        FATX_Browser.Manual_Partition mp = new FATX_Browser.Manual_Partition(ref PI);
            //        if (mp.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //        {
            //            PI = mp.ReturnInfo;
            //            FATStuff FS = new FATStuff(xDrive, PI.Offset);
            //            if (FS.Magic() == "XTAF")
            //            {
            //                PI.ClusterSize = FS.ClusterSize();
            //                PI.DataOffset = FS.DataOffset();
            //                PI.FATCopies = FS.FATCopies();
            //                PI.FATOffset = FS.FATOffset;
            //                PI.FATSize = FS.FATSize();
            //                PI.ID = FS.PartitionID();
            //                PI.Magic = FS.Magic();
            //                PI.Name = "Root";
            //                PI.SectorsPerCluster = FS.SectorsPerCluster();
            //                PI.EntrySize = FS.bit;
            //                PI.Size = FS.PartitionSize();
            //                PI.Clusters = FS.Blocks();
            //                PI.RealFATSize = FS.RealFATSize();
            //                Folder f = new Folder(xDrive, PI);
            //                f.EData.StartingCluster = FS.RootDirectoryCluster();
            //                f.SetPath(PI.Name);
            //                PIList.Add(f);
            //            }
            //        }
            //    }
            //}
            return PIList.ToArray();
        }
    }

    public class Entries
    {
        FATX.Info.PartitionBit bit = Info.PartitionBit.FATX32;
        int EntrySize = (int)Info.PartitionBit.FATX32;
        //Info.Partitions Partition;
        PartitionInfo Partition;
        Entry Holder;
        Exception ClusterOutOfRange = new Exception("Cluster was ref of range");

        public Entries(Entry e)
        {
            Partition = e.PartInfo;
            Holder = e;
            EntrySize =(int) e.PartInfo.EntrySize;
            bit = e.PartInfo.EntrySize;
        }

        private FATXDrive xDrive
        {
            get { return Holder.Drive; }
        }

        void Log(object Text)
        {
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(new System.IO.FileStream("log.txt", System.IO.FileMode.Open));
            bw.BaseStream.Position = bw.BaseStream.Length;
            bw.Write(Encoding.ASCII.GetBytes("\r\n" + Text.ToString()));
            bw.Close();
        }

        internal bool ExtractFileInternal(File f, FATX.IOWriter outIO, ref int sizeWritten, ref int maxVal, ref bool Cancel)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //Create our binaryreader
            FATX.IOReader br=null;
            try
            {

                //Create our new misc class
                Misc m = new Misc();
                maxVal = f.BlocksOccupied.Length;
                if (f.BlocksOccupied.Length >= 1)
                {
                    if (Cancel)
                    {
                        outIO.Close();
                        return true;
                    }
                    //Get our IO...
                    br = xDrive.GetIO();
                    //For each loop increase our sizewritten, and keep looping
                    //until we've reached the block BEFORE the final (size may not
                    //be 0x4000 bytes)
                    for (int i = 0; i < f.BlocksOccupied.Length - 1; i++, sizeWritten++)
                    {
                        if (Cancel)
                        {
                            br.Close();
                            outIO.Close();
                            return true;
                        }
                        /*
                        if (i == 0)
                        {
                            sw.Start();
                            //Get and set our reader position
                            long position = m.GetBlockOffset(f.BlocksOccupied[i], f.PartInfo);
                            br.BaseStream.Position = position;
                            sw.Stop();
                            Log(sw.Elapsed.Seconds);
                            sw.Reset();
                            //Write to our output file what we just read
                            sw.Start();//
                            outIO.Write(br.ReadBytes((int)f.PartInfo.ClusterSize));
                            sw.Stop();//
                            Log(sw.Elapsed.Seconds);
                            sw.Reset();//
                        }
                        else
                        {
                         */
                            //Get and set our reader position
                            long position = m.GetBlockOffset(f.BlocksOccupied[i], f.PartInfo);
                            br.BaseStream.Position = position;
                            outIO.Write(br.ReadBytes((int)f.PartInfo.ClusterSize));
                        //}
                        if (Cancel)
                        {
                            br.Close();
                            outIO.Close();
                            return true;
                        }
                    }
                    if (Cancel)
                    {
                        br.Close();
                        outIO.Close();
                        return true;
                    }
                    //sw.Start();
                    //KAY, now we've reached the final block in the series
                    br.BaseStream.Position = m.GetBlockOffset(f.BlocksOccupied[f.BlocksOccupied.Length - 1], f);
                    //sw.Stop();
                    //Log(sw.Elapsed.Seconds);
                    //sw.Reset();
                    //Right here, we read the remaining data, and write it all
                    //in one swoop
                    //sw.Start();
                    outIO.Write(m.ReadBytes(ref br, m.RemainingData(f)));
                    //sw.Stop();
                    //Log(sw.Elapsed.Seconds);
                    //sw.Reset();
                    //Close our IO's
                    br.Close();
                    outIO.Close();
                    sizeWritten++;
                    return true;
                }
                return false;
            }
            catch (Exception e) { outIO.Close(); br.Close(); throw e; }
        }

        /// <summary>
        /// Extracts your file to the specified path
        /// </summary>
        /// <param name="f">File to extract</param>
        /// <param name="path">Path to extract the file to</param>
        /// <param name="sizeWritten">Used for updating progress on the file(not necessary)</param>
        /// <param name="maxVal">Used for updating the file size (not necessary)</param>
        public bool ExtractFile(File f, string path, ref int sizeWritten, ref int maxVal, ref bool Cancel)
        {
            /* First thing first, check if the file is null.  If it is, we can
             * skip all of the bullshit */
            if (f.IsNull)
            {
                System.IO.File.Create(path);
                return true;
            }
            Misc m = new Misc();
            //Create our binary writer
            FATX.IOWriter bw = new FATX.IOWriter(new System.IO.FileStream(path, System.IO.FileMode.Create));
            //Set our maxVal
            maxVal = f.BlocksOccupied.Length;
            sizeWritten = 0;
            return ExtractFileInternal(f, bw, ref sizeWritten, ref maxVal, ref Cancel);
        }

        /// <summary>
        /// Returns an array of files and folders for a folder
        /// Index 0 in the array is folders, index 1 is files
        /// </summary>
        /// <param name="block">The block to read from</param>
        public object[] LoadEntries(Folder f, bool ShowDeletedEntries, bool LoadSTFS)
        {
            // Build our list of entry data
            List<EntryData> eList = new List<EntryData>();
            // Build our list for files
            List<File> fileList = new List<File>();
            // Build our list for folders
            List<Folder> folderList = new List<Folder>();
            // For each block this shit occupies
            for (int i = 0; i < f.BlocksOccupied.Length; i++)
            {
                // Builds our list of entry data
                eList.AddRange(GetEntries(f.BlocksOccupied[i]));
            }
            // For each entry in our entry data
            EntryData[] eArray = eList.ToArray();
            foreach (EntryData e in eArray)
            {
                // If the entry is a folder
                if (e.Size == 0 && e.StartingCluster != 0)
                {
                    Folder folder = new Folder(xDrive, Partition);
                    folder.Drive = xDrive;
                    folder.EData = e;
                    folder.PartInfo = ((Folder)Holder).PartInfo;
                    if (e.NameSize == (byte)FATX.Info.Flags.Deleted)
                    {
                        folder.IsDeleted = true;
                        try
                        {
                            folder.EData.Name = folder.EData.Name.Remove(folder.EData.Name.IndexOf('?'));
                        }
                        catch { continue; }
                    }
                    folderList.Add(folder);
                }

                /* If the shit is a file -> what.  12/22/2010 revise, why did i decide
                 *that if the file name size is bigger than 0, it's a file...  wtf*/
                else if (e.NameSize > 0x00)
                {
                    //If the entry is a folder
                    // This may not be true... I've got to look up dirent flags
                    if (e.Flags == 0x10 || e.Flags == 0x30)
                    {
                        //Do nothing
                        continue;
                    }
                    else
                    {
                        // Create our file that we're going to add to our list
                        File file = new File(xDrive, Partition);
                        file.Drive = xDrive;
                        file.EData = e;
                        file.PartInfo = ((Folder)Holder).PartInfo;
                        // If the size is equal to 0xE5 (deleted), mark it as deleted
                        if (e.NameSize == (byte)FATX.Info.Flags.Deleted)
                        {
                            file.IsDeleted = true;
                            /* The question mark is an invalid character in FATX,
                             * and because we don't have the actual filename size,
                             * we can just remove from the first instance of an invalid
                             * character!*/
                            //try
                            //{
                                file.EData.Name = file.EData.Name.Remove(file.EData.Name.IndexOf('?'));
                            //}
                            //catch { continue; }
                        }
                        // If we're loading STFSInformation information
                        if (LoadSTFS)
                        {
                            /* STFSInformation packages have to be at least 0xB000 in size,
                             * so if it's smaller than that, it's not an STFSInformation package.
                             * We're also checking to make sure it's not an XEX/XBE*/
                            if (file.Size >= 0xA000 && !file.Name.Contains(".xex") && !file.Name.Contains(".xbe"))
                            {
                                // Double-check the magic to make sure it's a valid STFSInformation package
                                STFS.STFSInfo stfsloader = new STFS.STFSInfo(file);
                                byte[] CON = Encoding.ASCII.GetBytes("CON");
                                /* I have no idea why I decided to return a byte array..
                                 * should have done a uint32*/
                                List<byte> con = CON.ToList<byte>();
                                con.Add(0x20);
                                CON = con.ToArray();
                                byte[] LIVE = Encoding.ASCII.GetBytes("LIVE");
                                byte[] PIRS = Encoding.ASCII.GetBytes("PIRS");
                                byte[] Magic = stfsloader.Magic();
                                // If the magic is valid
                                if (ArrayComparer(Magic, CON) || ArrayComparer(Magic, LIVE) || ArrayComparer(Magic, PIRS))
                                {
                                    // Get the STFSInformation information, assign it to the file
                                    file.STFSInformation = stfsloader.GetInfo();
                                    // Mark the package as an STFSInformation package.
                                    file.IsSTFSPackage = true;
                                }
                                stfsloader.Close();
                            }
                        }
                        // Add the file to our list
                        fileList.Add(file);
                    }
                }
            }

            return new object[] { folderList.ToArray(), fileList.ToArray() };
        }

        public bool ArrayComparer(byte[] buffer1, byte[] buffer2)
        {
            if (buffer1.Length != buffer2.Length)
            {
                return false;
            }

            for (int i = 0; i < buffer1.Length; i++)
            {
                if (buffer1[i] != buffer2[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns an array of files for the specified block, depreciated
        /// </summary>
        /// <param name="block">The block to read from</param>
        public File[] Files(uint block)
        {
            List<File> fList = new List<File>();
            foreach (EntryData data in GetEntries(block))
            {
                if (data.NameSize != 0xE5 && data.NameSize > 0x00)
                {
                    //If the entry is a folder
                    if (data.Size == 0 && data.StartingCluster != 0)
                    {
                        //Do nothing
                    }
                    else
                    {
                        File file = new File(xDrive, Partition);
                        file.Drive = xDrive;
                        file.EData = data;
                        file.PartInfo = ((Folder)Holder).PartInfo;
                        fList.Add(file);
                    }
                }
            }
            return fList.ToArray();
        }

        /// <summary>
        /// Returns an array of files for the specified blocks, depreciated
        /// </summary>
        /// <param name="block">The array of blocks to read from</param>
        public File[] Files(uint[] block)
        {
            List<File> fList = new List<File>();
            for (int i = 0; i < block.Length; i++)
            {
                foreach (File f in Files(block[i]))
                {
                    fList.Add(f);
                }
            }
            return fList.ToArray(); ;
        }

        /// <summary>
        /// Returns an array of folders for the specified block, depreciated
        /// </summary>
        /// <param name="block">StartingCluster to read from</param>
        public Folder[] Folders(uint block)
        {
            List<Folder> fList = new List<Folder>();
            foreach (EntryData data in GetEntries(block))
            {
                //If the entry is a folder
                if (data.NameSize != 0xE5 && data.Size == 0 && data.StartingCluster != 0)
                {
                    Folder folder = new Folder(xDrive, Partition);
                    folder.Drive = xDrive;
                    folder.EData = data;
                    folder.PartInfo = ((Folder)Holder).PartInfo;
                    fList.Add(folder);
                }
            }
            return fList.ToArray();
        }

        /// <summary>
        /// Returns an array of folders for the specified blocks, depreciated
        /// </summary>
        /// <param name="block">Blocks to read from</param>
        public Folder[] Folders(uint[] block)
        {
            List<Folder> fList = new List<Folder>();
            for (int i = 0; i < block.Length; i++)
            {
                foreach (Folder f in Folders(block[i]))
                {
                    fList.Add(f);
                }
            }
            return fList.ToArray();
        }

        /// <summary>
        /// Returns an array of EntryData for a specified block
        /// </summary>
        /// <param name="block">The block to read from</param>
        public EntryData[] GetEntries(uint block)
        {
            FATX.IOReader br = xDrive.GetIO();
            // Get the block offset
            Misc r = new Misc();
            br.BaseStream.Position = r.GetBlockOffset(block, Holder.PartInfo);
            long position = br.BaseStream.Position;
            // Create another binary reader that will hold our 0x200 bytes to read
            // (we can not seek to something that's not a multiple of 0x200)
            long OOFFSET = br.BaseStream.Position;
            byte[] buffer = br.ReadBytes(0x200);
            //Our placeholder for the bytes in the buffer read
            int bufferRead = 0;
            List<EntryData> l = new List<EntryData>();
            for (uint i = 1; i <= (Holder.PartInfo.ClusterSize / 0x40); i++)
            {
                // If we've reached the end of the buffer
                if (bufferRead == 0x200)
                {
                    OOFFSET = br.BaseStream.Position;
                    //Read the next 0x200 bytes
                    buffer = br.ReadBytes(0x200);
                    bufferRead = 0;
                }
                // Create our stream for the reader
                FATX.IOReader mem = new FATX.IOReader(new System.IO.MemoryStream(buffer));
                mem.BaseStream.Position = bufferRead;
                // Make sure that we're not at the end of the block
                byte[] end = mem.ReadBytes(2);
                if (EOF(end, true))
                {
                    break;
                }
                //We have to reset the reader back to the beginning of the block
                mem.BaseStream.Position -= 0x2;
                EntryData ed = GetEData(OOFFSET, mem, i, block);
                if (ed.NameSize > 0xE5)
                {
                    return l.ToArray();
                }
                else
                {
                    l.Add(ed);
                }
                position += 0x40;
                bufferRead += 0x40;
                br.BaseStream.Position = position;
                mem.Close();
            }
            br.Close();
            return l.ToArray();
        }

        /// <summary>
        /// Returns EntryData for an entry in a block
        /// </summary>
        /// <param name="br">The binary reader to use</param>
        /// <param name="EntryBlock">The entry in the block to read</param>
        /// <param name="StartingCluster">The block to read from</param>
        internal EntryData GetEData(long originalOffset, FATX.IOReader br, uint EntryBlock, uint Block)
        {
            Misc r = new Misc();
            // Create our return
            EntryData data = new EntryData();
            // Read our variables
            data.EntryOffset = originalOffset + br.BaseStream.Position;
            data.NameSize = br.ReadByte();
            if (data.NameSize > 0xE5)
            {
                return data;
            }
            data.Flags = br.ReadByte();
            data.Name = Encoding.ASCII.GetString(br.ReadBytes((int)data.NameSize));
            // Go to the end of the name to continue reading the variables
            br.BaseStream.Position += (0x2A - (int)data.NameSize);
            data.StartingCluster = r.ReadUInt32(ref br);
            data.Size = r.ReadUInt32(ref br);
            data.CreationDate = r.ReadUInt16(ref br);
            data.CreationTime = r.ReadUInt16(ref br);
            data.AccessDate = r.ReadUInt16(ref br);
            data.AccessTime = r.ReadUInt16(ref br);
            data.ModifiedDate = r.ReadUInt16(ref br);
            data.ModifiedTime = r.ReadUInt16(ref br);
            br.Close();
            return data;
        }

        //For getting an array of blocks occupied
        /// <summary>
        /// Returns an array of blocks that a file/folder entry occupies
        /// </summary>
        /// <param name="baseBlock">The base block to read from the FAT chain (the root)</param>
        public uint[] GetBlocksOccupied(uint baseBlock)
        {
            //Create our list to hold the uints
            List<uint> l = new List<uint>();
            l.Add(baseBlock);
            //Create a new byte array with random input
            byte[] ourReturn = { 0xAA, 0xAA };
            //Increase the number of blocks - keep the loop going until we've
            //reached the end of the blocks (0xFFFF or equal)
            for (int i = 0; !EOF(ourReturn, false); i++)
            {
                uint CurrentBlock = l.ToArray()[i];
                //Get our next block in the series
                //Create our list of our 
                Misc m = new Misc();
                //Create our binary reader
                FATX.IOReader br = xDrive.GetIO();
                //Read our buffer to load in to the memory stream
                long BufferOffset = m.BlockToFATOffset(CurrentBlock, Holder);
                BufferOffset = BufferOffset - (BufferOffset % 0x200);
                long BlockOffsetInBuffer = m.BlockToFATOffset(CurrentBlock, Holder) - BufferOffset;
                //Go to the buffer we're reading's offset
                br.BaseStream.Position = BufferOffset;
                //Read our buffer
                byte[] buffer = br.ReadBytes(0x200);
                br.Close();
                //Re-open our reader using the memorystream this time
                FATX.IOReader mem = new FATX.IOReader(new System.IO.MemoryStream(buffer));
                //Close our reader
                mem.BaseStream.Position = BlockOffsetInBuffer;
                ourReturn = mem.ReadBytes(EntrySize);
                mem.Close();
                //If the block is the ending block
                if (EOF(ourReturn, false))
                {
                    //Break
                    break;
                }
                //The block isn't the ending block, reverse the array so we can
                //convert it to big endian uintxx
                Array.Reverse(ourReturn);
                //Create our value to add to the list
                uint addVal;
                if (bit == Info.PartitionBit.FATX32)
                {
                    addVal = BitConverter.ToUInt32(ourReturn, 0);
                }
                else { addVal = BitConverter.ToUInt16(ourReturn, 0); }
                //Add our value to the list
                l.Add(addVal);
                Array.Reverse(ourReturn);
                if (BufferOffset == (long)(m.DownToNearest200(m.BlockToFATOffset(addVal, Holder))))
                {
                    object[] stuff = CheckBlock(ref buffer, ourReturn, BufferOffset);
                    l.AddRange(((List<uint>)stuff[1]).ToArray());
                    i += ((List<uint>)stuff[1]).Count;
                    if ((bool)stuff[0] == true)
                    {
                        break;
                    }
                }
            }
            return l.ToArray();
        }

        private object[] CheckBlock(ref byte[] buffer, byte[] nextVal, long bufferOff)
        {
            //Our list of cocks
            List<uint> blocks = new List<uint>();
            return new object[] { CheckBlock(ref buffer, nextVal, bufferOff, ref blocks), blocks };
        }

        private bool CheckBlock(ref byte[] buffer, byte[] nextVal, long bufferOff, ref List<uint> blocks)
        {
            //Create our byte array for our reading (value will be set, but throws an error
            //if we don't set it to anything now
            byte[] reading = { 0x00 };
            //For determining if we've reached the end of our fileszzz
            bool EOOF = true;
            //Our uint for the next valu
            uint NextVal = 0;
            //Get our position in buffer
            int positionInBuff;
            object[] ReturnObject = new Object[] { EOOF, blocks };
            //If the previous value wasn't EOF
            if (!EOF(nextVal, false))
            {
                EOOF = false;
                Array.Reverse(nextVal);
                if (Holder.PartInfo.EntrySize == Info.PartitionBit.FATX16)
                {
                    NextVal = BitConverter.ToUInt16(nextVal, 0x0);
                }
                else
                {
                    NextVal = BitConverter.ToUInt32(nextVal, 0x0);
                }
                //Get our position in buffer (actually assigning value now)
                positionInBuff = (int)(new Misc().BlockToFATOffset(NextVal, Holder) - bufferOff);
                //Open our binary reader
                FATX.IOReader br = new FATX.IOReader(new System.IO.MemoryStream(buffer));
                //Set our position of our reader
                br.BaseStream.Position = positionInBuff;
                //Sort of silly to loop, but i'm lazy to revise code
                for (int i = positionInBuff; i < buffer.Length; i += EntrySize)
                {
                    //Read our number (if possible)
                    reading = br.ReadBytes(EntrySize);
                    //If what we just read is the same as what we're expecting, and not EOF
                    if (!EOF(reading, false))
                    {
                        //Reverse our array (bitconverter reverses the array, causing it to be
                        //little endian when we convert)
                        Array.Reverse(reading);
                        //Create our bloccstyle value
                        uint blockAddVal;
                        //If we're in a uint32 partition
                        if (EntrySize == 4)
                        {
                            //Convert our shit to a uint32
                            blockAddVal = BitConverter.ToUInt32(reading, 0x0);
                        }
                            //We're in an uint16 partition
                        else
                        {
                            //Let's convert this shit to a uint16 val
                            blockAddVal = BitConverter.ToUInt16(reading, 0x0);
                        }
                        //Add it to our blocklist
                        blocks.Add(blockAddVal);
                        //Reverse the array again so that it's bigendian
                        Array.Reverse(reading);
                        //Get the next block if possible...
                        if (bufferOff == new Misc().DownToNearest200(new Misc().BlockToFATOffset(blockAddVal, Holder)))
                        {
                            EOOF = CheckBlock(ref buffer, reading, bufferOff, ref blocks);
                        }
                        return EOOF;
                    }
                    if (EOF(reading, false))
                    {
                        EOOF = true;
                        return EOOF;
                    }
                }
            }
            EOOF = true;
            return EOOF;
        }

        //Checking if we're at the end of the file
        /// <summary>
        /// Checks for the end of a file/folder
        /// </summary>
        private bool EOF(byte[] buffer, bool Entry)
        {
            int NoOfFF = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == 0xFF)
                {
                    NoOfFF++;
                }
                else if (i == buffer.Length - 1)
                {
                    if (buffer[i] == 0xF8)
                    {
                        NoOfFF++;
                    }
                }
            }

            if (NoOfFF == buffer.Length)
            {
                return true;
            }

            if (Entry)
            {
                if (buffer[0] > 0x2A && buffer[0] != 0xE5)
                {
                    return true;
                }
            }
            return false;
        }

        //For reading a single block
        private byte[] GetOccupiedBlock(uint baseBlock)
        {
            //Create our list of our 
            Misc m = new Misc();
            //Create our binary reader
            FATX.IOReader br = xDrive.GetIO();
            //Read our buffer to load in to the memory stream
            long BufferOffset = m.BlockToFATOffset(baseBlock, Holder);
            BufferOffset = BufferOffset - (BufferOffset % 0x200);
            long BlockOffsetInBuffer = m.BlockToFATOffset(baseBlock, Holder) - BufferOffset;
            //Go to the buffer we're reading's offset
            br.BaseStream.Position = BufferOffset;
            //Read our buffer
            byte[] buffer = br.ReadBytes(0x200);
            br.Close();
            //Re-open our reader using the memorystream this time
            FATX.IOReader mem = new FATX.IOReader(new System.IO.MemoryStream(buffer));
            //Close our reader
            mem.BaseStream.Position = BlockOffsetInBuffer;
            byte[] rVal = mem.ReadBytes(EntrySize);
            //The following checks to see if the next block in the sequence is 
            uint nextBlock;
            if (EntrySize == 4)
            {
                byte[] blockCopy = rVal;
                nextBlock = BitConverter.ToUInt32(blockCopy, 0x0);
            }
            else
            {
                byte[] blockCopy = rVal;
                nextBlock = BitConverter.ToUInt16(blockCopy, 0x0);
            }
            mem.Close();
            return rVal;
        }
    }
}
