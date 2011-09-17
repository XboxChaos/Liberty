using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FATX;
using FATX.Structs;

namespace FATX
{
    class Write
    {

        FATXDrive ourDrive;
        Misc m;
        public Write(FATXDrive drive)
        {
            ourDrive = drive;
            m = new Misc();
        }

        public List<ExistingEntry> InjectFolder(string Path, Folder root, bool Merge, bool MergeSubs, ref bool Cancel)
        {
            string s = "";
            int cock = 0;
            int what = 0;
            return InjectFolder(Path, root, ref s, ref cock, ref what, Merge, MergeSubs, ref Cancel);
        }

        public List<ExistingEntry> InjectFolder(string Path, Folder root, ref string CurrentFile, ref int CurrentProgress, ref int MaxProgress, bool Merge, bool MergeSubs, ref bool Cancel)
        {
            /* Create our entry list (will be returning folders to be merged, files
             * to be overwritten)*/
            List<ExistingEntry> eList = new List<ExistingEntry>();
            // Our folder to inject the files in to
            Folder newFolder = null;
            // Check if the path is a folder
            if (m.IsFolder(Path))
            {
                // Path was a folder, check the name to make sure it's valid for FATX
                if (m.CheckFileName(new System.IO.DirectoryInfo(Path).Name))
                {
                    if (Cancel)
                    {
                        return eList;
                    }
                    foreach (Folder f in root.SubFolders(false))
                    {
                        if (Cancel)
                        {
                            return eList;
                        }
                        // If the folder already exists...
                        if (f.Name.ToLower() == System.IO.Path.GetFileName(Path).ToLower() && !Merge)
                        {
                            // Create our ExistingEntry
                            ExistingEntry ex = new ExistingEntry();
                            ex.EntryType = Info.EntryType.Folder;
                            ex.Existing = f;
                            ex.NewPath = Path;
                            // Add that to the list that exists
                            eList.Add(ex);
                            return eList;
                        }
                        if (Cancel)
                        {
                            return eList;
                        }
                        else if (f.Name.ToLower() == System.IO.Path.GetFileName(Path).ToLower())
                        {
                            newFolder = f;
                            break;
                        }
                    }
                    /* If the folder doesn't exist, create a new one
                     * if we don't touch anything here, newFolder will already
                     * be set in the statement above*/
                    if (newFolder == null)
                    {
                        if (Cancel)
                        {
                            return eList;
                        }
                        newFolder = root.NewFolder(new System.IO.DirectoryInfo(Path).Name);
                    }
                    if (Cancel)
                    {
                        return eList;
                    }
                    // For each file in the folder, inject
                    System.IO.FileInfo[] fia = new System.IO.DirectoryInfo(Path).GetFiles();
                    foreach (System.IO.FileInfo fi in fia)
                    {
                        if (Cancel)
                        {
                            return eList;
                        }
                        // If the file name checks ref...
                        if (m.CheckFileName(fi.Name))
                        {
                            bool exist = false;
                            // Loop for each file in this folder...
                            foreach (File f in newFolder.Files(false))
                            {
                                if (Cancel)
                                {
                                    return eList;
                                }
                                // If there's a file in this folder with the same name...
                                if (f.Name.ToLower() == fi.Name.ToLower())
                                {
                                    // Create our new existing entry struct
                                    ExistingEntry ex = new ExistingEntry();
                                    // Set the type
                                    ex.EntryType = Info.EntryType.File;
                                    // Set the existing file
                                    ex.Existing = f;
                                    // Set the path
                                    ex.NewPath = fi.FullName;
                                    eList.Add(ex);
                                    exist = true;
                                    break;
                                }
                            }
                            if (Cancel)
                            {
                                return eList;
                            }
                            // If the file doesn't already exists.....................................
                            if (!exist)
                            {
                                if (Cancel)
                                {
                                    return eList;
                                }
                                // Update the file that we are currently on
                                CurrentFile = fi.Name;
                                // Write the new file
                                WriteNewFile(newFolder, fi.FullName, ref CurrentProgress, ref MaxProgress, ref Cancel);
                            }
                        }
                        // File name didn't check ref.  WHAT
                        else
                        {
                            throw new Exception("File name \"" + fi.Name + "\" contains invalid characters");
                        }
                    }
                    // Call to this function for each folder in this folder
                    if (Cancel)
                    {
                        return eList;
                    }
                    System.IO.DirectoryInfo[] dia = new System.IO.DirectoryInfo(Path).GetDirectories();
                    foreach (System.IO.DirectoryInfo di in dia)
                    {
                        if (Cancel)
                        {
                            return eList;
                        }
                        eList.AddRange(InjectFolder(di.FullName, newFolder, ref CurrentFile, ref CurrentProgress, ref MaxProgress, MergeSubs, MergeSubs, ref Cancel));
                    }
                }
                else
                {
                    throw new Exception("Folder name \"" + new System.IO.DirectoryInfo(Path).Name + "\" contains invalid characters");
                }
            }
            // Path wasn't a folder, tell that bitch what's up
            else
            {
                throw new Exception("Path does not point to a folder");
            }
            return eList;
        }

        public Folder NewFolder(string name, Folder root)
        {
            Folder f = null;
            //try
            //{
                //If the file name is valid
                if (m.CheckFileName(name))
                {
                    // Now that that's ref of the way... let's check to see if a folder with the same
                    // name already exists in the root
                    foreach (Folder folder in root.SubFolders(false))
                    {
                        // Folder with the same name exists; fuck shit up
                        if (folder.Name.ToLower() == name.ToLower())
                        {
                            throw new Exception("A folder with the name \"" + name + "\" already exists in this folder");
                        }
                    }
                    EntryData EData = GetNewEntryData(root, name);
                    f = new Folder(ourDrive, root.PartInfo);
                    f.EData = EData;
                    // Create the entry
                    CreateNewEntry(EData);
                    // Write the fat chain for this
                    WriteFATChain(new uint[] { f.StartingCluster }, f);
                    // Write ref 0xFF amongst the folder's cluster so that when reading, we don't
                    // accidentally read something that's not an entry, and so that it matches
                    // how the xbox does it.
                    byte[] ff = new byte[f.PartInfo.ClusterSize];
                    for (int i = 0; i < ff.Length; i++)
                    {
                        ff[i] = 0xFF;
                    }
                    WriteToCluster(new Misc().GetBlockOffset(f.StartingCluster, f), ff, true);
                    return f;
                }
                throw new Exception("Folder name invalid");
            //}
            //catch (Exception e)
            //{
            //    if (f != null)
            //    { bool Cancel = false; f.Delete(ref Cancel); }
            //    throw e;
            //}
        }

        public bool CreateNewEntry(EntryData Edata)
        {
            FATX.IOReader br = ourDrive.GetIO();
            //Set our position so that we can read the entry location
            br.BaseStream.Position = m.DownToNearest200(Edata.EntryOffset);
            byte[] buffer = br.ReadBytes(0x200);
            br.Close();
            //Create our binary writer
            FATX.IOWriter bw = new FATX.IOWriter(new System.IO.MemoryStream(buffer));
            //Set our position to where the entry is
            long EntryOffset = Edata.EntryOffset - m.DownToNearest200(Edata.EntryOffset);
            bw.BaseStream.Position = EntryOffset;
            //Write our entry
            bw.Write(Edata.NameSize);
            bw.Write(Edata.Flags);
            bw.Write(Encoding.ASCII.GetBytes(Edata.Name));
            int FFLength = 0x2A - Edata.NameSize;
            byte[] FF = new byte[FFLength];
            for (int i = 0; i < FFLength; i++)
            {
                FF[i] = 0xFF;
            }
            bw.Write(FF);
            //Right here, we need to make everything a byte array, as it feels like writing
            //everything in little endian for some reason...
            byte[] StartingCluster = BitConverter.GetBytes(Edata.StartingCluster);
            Array.Reverse(StartingCluster);
            bw.Write(StartingCluster);
            byte[] Size = BitConverter.GetBytes(Edata.Size);
            Array.Reverse(Size);
            bw.Write(Size);
            //Write ref the creation date/time 3 times
            byte[] CreationDate = BitConverter.GetBytes(Edata.CreationDate);
            byte[] CreationTime = BitConverter.GetBytes(Edata.CreationTime);
            byte[] AccessDate = BitConverter.GetBytes(Edata.AccessDate);
            byte[] AccessTime = BitConverter.GetBytes(Edata.AccessTime);
            byte[] ModifiedDate = BitConverter.GetBytes(Edata.ModifiedDate);
            byte[] ModifiedTime = BitConverter.GetBytes(Edata.ModifiedTime);
            Array.Reverse(CreationDate);
            Array.Reverse(CreationTime);
            Array.Reverse(AccessDate);
            Array.Reverse(AccessTime);
            Array.Reverse(ModifiedDate);
            Array.Reverse(ModifiedTime);
            bw.Write(CreationDate);
            bw.Write(CreationTime);
            bw.Write(AccessDate);
            bw.Write(AccessTime);
            bw.Write(ModifiedDate);
            bw.Write(ModifiedTime);
            //Close our writer
            bw.Close();
            //Get our IO
            bw = ourDrive.GetWriterIO();
            bw.BaseStream.Position = m.DownToNearest200(Edata.EntryOffset);
            //Write ref our buffer
            bw.Write(buffer);
            return true;
        }

        private bool WriteFATChain(uint[] blocksOccupied, Entry e)
        {
            //Foreach block in our blocks occupied
            for (int i = 0; i < blocksOccupied.Length; i++)
            {
                //Get the value of the block
                uint block = blocksOccupied[i];
                //Create our byte array for our pointer to the next block
                byte[] Bytes = new byte[0];
                //If we have reached the last block in the array...
                if (i == blocksOccupied.Length - 1)
                {
                    //We need to write ref that it's the last block, so we do so by writing 0xFF (in accordance to entry size)
                    if (e.PartInfo.EntrySize == Info.PartitionBit.FATX16)
                    {
                        //EOB (end of blocks) for FATX16...
                        Bytes = new byte[] { 0xFF, 0xFF };
                    }
                    else
                    {
                        //EOB for FATX32
                        Bytes = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
                    }
                }
                //We have not reached the end block
                else
                {
                    if (e.PartInfo.EntrySize == Info.PartitionBit.FATX16)
                    {
                        Bytes = BitConverter.GetBytes((ushort)blocksOccupied[i + 1]);
                    }
                    else
                    {
                        Bytes = BitConverter.GetBytes(blocksOccupied[i + 1]);
                    }
                    //Reverse the array so that it will be big endian
                    Array.Reverse(Bytes);
                }

                #region Bascially the WriteBlock Function, modified to check for the next block within the same buffer
                //Write ref our next block's byte array at the block's position in the FAT
                //Create our binary reader
                FATX.IOReader br = ourDrive.GetIO();
                //Set our position to the block offset in the FAT
                long pos = m.DownToNearest200(m.BlockToFATOffset(block, e));
                br.BaseStream.Position = pos;
                //Get our position in the buffer
                long OffsetInBuffer = m.BlockToFATOffset(block, e) - br.BaseStream.Position;
                //Read our buffer
                byte[] buffer = br.ReadBytes(0x200);
                //Close our binary reader - we're done with it for the drive for now
                br.Close();
                //Create our binary writer for writing to the buffer
                FATX.IOWriter bw = new FATX.IOWriter(new System.IO.MemoryStream(buffer));
                //Set our position 
                bw.BaseStream.Position = OffsetInBuffer;
                //Write our block
                bw.Write(Bytes);
                bw.Close();
                if (!m.EOF(Bytes, false))
                {
                    i += CheckWriteBuff(ref buffer, m.DownToNearest200(m.BlockToFATOffset(block, e)), blocksOccupied, i + 1, e);
                }
                //Re-open our binary writer in the drive
                bw = ourDrive.GetWriterIO();
                //Set our position
                bw.BaseStream.Position = m.DownToNearest200(m.BlockToFATOffset(block, e));
                //Write our buffer
                bw.Write(buffer);
                #endregion
            }
            return true;
        }

        private int CheckWriteBuff(ref byte[] Buffer, long BufferOffset, uint[] allBlocks, int index, Entry e)
        {
            if (index == allBlocks.Length)
            {
                return 0;
            }
            //This will be our return value so that we can tell our other function how
            //many blocks were written
            int blocksWritten = 0;
            //If the offset for the next block's buffer is the same as our current buffer's offset
            if (m.DownToNearest200(m.BlockToFATOffset(allBlocks[index], e)) == BufferOffset)
            {
                //Get our offset in our buffer for the block we're writing to
                long OffsetinBuffer = m.BlockToFATOffset(allBlocks[index], e) - BufferOffset;
                //Create our binary writer
                FATX.IOWriter bw = new FATX.IOWriter(new System.IO.MemoryStream(Buffer));
                //Go to the offset in the buffer
                bw.BaseStream.Position = OffsetinBuffer;
                //Create our byte array (we do new byte[1] just as a placeholder)
                byte[] BlocktoWrite = new byte[1];
                //If we're on the last block...
                if (index == allBlocks.Length - 1)
                {
                    //If we're on a 2-byte block entry
                    if (e.PartInfo.EntrySize == Info.PartitionBit.FATX16)
                    {
                        //Our block to write is the ending block
                        BlocktoWrite = new byte[] { 0xFF, 0xFF };
                    }
                    else
                    {
                        //Our block to write is the ending block
                        BlocktoWrite = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
                    }
                }
                //If we're not on the last block...
                else
                {
                    //Get the next block's value as a byte array
                    if (e.PartInfo.EntrySize == Info.PartitionBit.FATX16)
                    {
                        BlocktoWrite = BitConverter.GetBytes((ushort)allBlocks[index + 1]);
                    }
                    else
                    {
                        BlocktoWrite = BitConverter.GetBytes(allBlocks[index + 1]);
                    }
                    //Reverse the array so that it's now in bigendian
                    Array.Reverse(BlocktoWrite);
                }

                //Write ref our array
                bw.Write(BlocktoWrite);
                //Close our writer
                bw.Close();
                blocksWritten++;
                //If we didn't just write the last block...
                if (index != allBlocks.Length)
                {
                    //Repeat
                    blocksWritten += CheckWriteBuff(ref Buffer, BufferOffset, allBlocks, index + blocksWritten, e);
                }
            }
            return blocksWritten;
        }

        private object[] CheckIfBlocksNeeded(Folder f)
        {
            f.ReloadData(true);
            //Create our object array that will hold our Bool and Entry for if
            //we need an open block, and if there's a deleted file

            //Create our entry reader so that we can get a return of entries...
            Entries e = new Entries(f);
            //Get our entries in the last block
            EntryData[] eData = e.GetEntries(f.BlocksOccupied[f.BlocksOccupied.Length - 1]);
            //Files span upon multiple blocks... Here we go to the last block that it occupies
            //(the most recent block created), and check if it has any open entries

            //Check for deleted entries
            foreach (File file in f.Files(true))
            {
                if (file.IsDeleted)
                {
                    return new object[] { true, file.EData };
                }
            }

            foreach (Folder folder in f.SubFolders(true))
            {
                if (folder.IsDeleted)
                {
                    return new object[] { true, folder.EData };
                }
            }
            //We didn't find a deleted entry, but we have room in the last block of the folder
            //for a new entry
            if (eData.Length < (f.PartInfo.ClusterSize / 0x40) && eData.Length > 0)
            {
                EntryData newEntry = new EntryData();
                newEntry.EntryOffset = eData[eData.Length - 1].EntryOffset + 0x40;
                newEntry.StartingCluster = eData[eData.Length - 1].StartingCluster;
                return new object[] { false, newEntry };
            }
            else if (eData.Length == 0)
            {
                EntryData newEntry = new EntryData();
                newEntry.EntryOffset = f.BaseOffset;
                newEntry.StartingCluster = new FATStuff(f).GetFreeBlocks(1, f.BlocksOccupied[f.BlocksOccupied.Length - 1], 0, false)[0];
                return new object[] { false, newEntry };
            }
            //We don't have any deleted entries, and don't have enough room in the last block,
            //so let's create a new block for the folder, add it to the FAT chain, etc.
            if (eData.Length == (f.PartInfo.ClusterSize / 0x40))
            {
                //Get our new block...
                uint nextBlock = new FATStuff(f).GetFreeBlocks(1, f.BlocksOccupied[f.BlocksOccupied.Length - 1], 0, false)[0];
                //Write the fat chain
                WriteFATChain(new uint[] { f.BlocksOccupied[f.BlocksOccupied.Length - 1], nextBlock }, f);
                //Write ref 0xFF among the cluster to expand the folder
                byte[] Buffer = new byte[f.PartInfo.ClusterSize];
                for (int i = 0; i < Buffer.Length; i++)
                {
                    Buffer[i] = 0xFF;
                }
                WriteToCluster(m.GetBlockOffset(nextBlock, f), Buffer, true);
                //Create our new entrydata
                EntryData EntryNew = new EntryData();
                EntryNew.EntryOffset = m.GetBlockOffset(nextBlock, f);
                return new object[] { false, EntryNew };
            }
            throw new Exception("What");
        }

        /* NOTE: NEXT THREE FUNCTIONS
         * I was honestly too lazy to make my other functions flexible, and
         * include functionality to support null files, so I decided to whip
         * these up.  Ye...
         */

        private File WriteNullFile(Folder parent, string FilePath)
        {
            int hm = 0, asdfasdfasdfasdfsdfas = 0;
            return WriteNullFile(parent, FilePath, ref hm, ref asdfasdfasdfasdfsdfas);
        }

        private File WriteNullFile(Folder parent, string FilePath, ref int Progress, ref int ProgressMax)
        {
            // For a null file, all we have to do is create a new entry
            // Because my function automatically assigns that entry a new cluster
            // in the FAT, we'll have to clear that up...
            EntryData e = GetNewEntryData(parent, System.IO.Path.GetFileName(FilePath));
            // Clear the FAT chain
            ClearFATChain(new uint[] { e.StartingCluster }, parent);
            // Re-write the entry
            e.Size = 0;
            e.StartingCluster = 0;
            e.Flags = 0;
            CreateNewEntry(e);
            // Shit's done, return the new file
            File f = new File(parent.Drive, parent.PartInfo);
            f.EData = e;
            return f;
        }

        private File OverWriteNullFile(Folder parent, File FileToOverWrite, string FilePath, ref bool Cancel)
        {
            int garbage = 0, coffee = 0x00000; // see what I did there?  Coffee is black,
            // black is 0x0 -- haaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
            return OverWriteNullFile(parent, FileToOverWrite, FilePath, ref garbage, ref coffee, ref Cancel);
        }

        private File OverWriteNullFile(Folder parent, File FileToOverWrite, string FilePath, ref int Progress, ref int ProgressMax, ref bool Cancel)
        {
            // They're basically writing the same file... The hell are they thinking
            if (FileToOverWrite.IsNull && new System.IO.FileInfo(FilePath).Length == 0)
            {
                return FileToOverWrite;
            }
            // They're not writing the same file, so let's just delete this entry,
            // call to the "WriteNewFile" function using this, but let's first make
            // sure that the file IS null.
            if (FileToOverWrite.IsNull)
            {
                FileToOverWrite.Delete();
                WriteNewFile(parent, FilePath, ref Progress, ref ProgressMax, ref Cancel);
            }
            // File wasn't null, throw an exception
            throw new Exception("File not null!");
        }

        private File OverWriteFileWithNull(Folder parent, File FileToOverWrite, string FilePath, ref bool Cancel)
        {
            int a = 0, b = 0;
            return OverWriteFileWithNull(parent, FileToOverWrite, FilePath, ref a, ref b, ref Cancel);
        }

        private File OverWriteFileWithNull(Folder parent, File FileToOverWrite, string FilePath, ref int Progress, ref int ProgressMax, ref bool Cancel)
        {
            // Before anything, let's make sure that this file isn't null!
            if (FileToOverWrite.IsNull)
            {
                // File was null, this is the wrong function...
                return OverWriteNullFile(parent, FileToOverWrite, FilePath, ref Progress, ref ProgressMax, ref Cancel);
            }

            if (FileToOverWrite.IsNull && new System.IO.FileInfo(FilePath).Length == 0)
            {
                return FileToOverWrite;
            }

            // Get our entry data from the file we're overwriting...
            EntryData e = FileToOverWrite.EData;
            e.StartingCluster = 0;
            e.Size = 0;
            // Clear the old file's FAT chain
            ClearFATChain(FileToOverWrite.BlocksOccupied, FileToOverWrite);
            // Write the new entry
            CreateNewEntry(e);
            // Return our new, beautiful file
            File f = new File(FileToOverWrite.Drive, parent.PartInfo);
            f.EData = e;
            return f;
        }

        public bool Delete(File f)
        {
            int ProgressUpdate = 0, ProgressMax = 0;
            string CurrentFile = "";
            return DeleteInternal(f, ref ProgressUpdate, ref ProgressMax, ref CurrentFile);
        }

        public bool Delete(File f, ref int ProgressUpdate, ref int ProgressMax, ref string CurrentFile)
        {
            return DeleteInternal(f, ref ProgressUpdate, ref ProgressMax, ref CurrentFile);
        }

        private bool DeleteInternal(File f, ref int progressUpdate, ref int progressMax, ref string CurrentFile)
        {
            if (f.IsNull)
            {
                return MarkEntryAsDeleted(f);
            }
            uint[] Occupied = f.BlocksOccupied;
            //Set the max progress
            progressMax = 2;
            progressUpdate = 0;
            //Clear the entry
            MarkEntryAsDeleted(f);
            progressUpdate++;
            //Mark the blocks as empty
            ClearFATChain(Occupied, f);
            progressUpdate++;
            return true;
        }

        public bool Delete(Folder f, ref bool Cancel)
        {
            int max = 0;
            int update = 0;
            string s = "";
            return Delete(f, ref update, ref max, ref s, ref Cancel);
        }

        public bool Delete(Folder f, ref int ProgressUpdate, ref int ProgressMax, ref string CurrentEntry, ref bool Cancel)
        {
            return DeleteInternal(f, ref ProgressUpdate, ref ProgressMax, ref CurrentEntry, ref Cancel);
        }

        private bool DeleteInternal(Folder f, ref int progressUpdate, ref int progressMax, ref string CurrentFile, ref bool Cancel)
        {
            //Set the max progress
            progressMax = 1;
            if (Cancel)
            {
                return true;
            }
            foreach (File g in f.Files(false))
            {
                progressMax++;
            }
            if (Cancel)
            {
                return true;
            }
            foreach (Folder g in f.SubFolders(false))
            {
                progressMax++;
            }
            //Set progress update
            progressUpdate = 0;
            if (Cancel)
            {
                return true;
            }
            foreach (Folder g in f.SubFolders(false))
            {
                if (Cancel)
                {
                    return true;
                }
                CurrentFile = g.Name;
                DeleteInternal(g, ref progressUpdate, ref progressMax, ref CurrentFile, ref Cancel);
            }
            if (Cancel)
            {
                return true;
            }
            foreach (File g in f.Files(false))
            {
                if (Cancel)
                {
                    return true;
                }
                CurrentFile = g.Name;
                ClearFATChain(g.BlocksOccupied, g);
                g.Delete();
                progressUpdate++;
            }

            if (Cancel)
            {
                return true;
            }
            CurrentFile = f.Name;
            //Clear the entry
            MarkEntryAsDeleted(f);
            //Mark the blocks as empty
            ClearFATChain(f.BlocksOccupied, f);
            progressUpdate++;

            return true;
        }

        private bool MarkEntryAsDeleted(Entry e)
        {
            FATX.IOReader br = ourDrive.GetIO();
            long position = m.DownToNearest200(e.EntryOffset);
            //Get our offset in our buffer
            long OffsetInBuffer = e.EntryOffset - position;
            br.BaseStream.Position = position;
            //Read our buffer
            byte[] buffer = br.ReadBytes(0x200);
            //Close our binary reader - not needed now
            br.Close();
            //Create our binary writer
            FATX.IOWriter bw = new FATX.IOWriter(new System.IO.MemoryStream(buffer));
            //Seek to the position in the buffer
            bw.BaseStream.Position = OffsetInBuffer;
            //Write the flag
            bw.Write((byte)Info.Flags.Deleted);
            //Close our binary writer
            bw.Close();
            //Re-open our binary writer for the drive
            bw = ourDrive.GetWriterIO();
            //Set our position
            bw.BaseStream.Position = m.DownToNearest200(e.EntryOffset);
            //Write ref our buffer
            bw.Write(buffer);
            return true;
        }
        ///We need to make the writing of the blocks and shit little endian so that it will
        ///write it in big endian.  what the fuck.
        //For some reason is the function that writes a FAT chain
        private bool WriteBlock(uint ui, byte[] value, Entry e)
        {
            //Create our binary reader
            FATX.IOReader br = ourDrive.GetIO();
            //Set our position to the block offset in the FAT
            br.BaseStream.Position = m.DownToNearest200(m.BlockToFATOffset(ui, e));
            //Get our position in the buffer
            long OffsetInBuffer = m.BlockToFATOffset(ui, e) - br.BaseStream.Position;
            //Read our buffer
            byte[] buffer = br.ReadBytes(0x200);
            //Close our binary reader - we're done with it for the drive for now
            br.Close();
            //Create our binary writer for writing to the buffer
            FATX.IOWriter bw = new FATX.IOWriter(new System.IO.MemoryStream(buffer));
            //Set our position 
            bw.BaseStream.Position = OffsetInBuffer;
            //Write our block
            bw.Write(value);
            bw.Close();
            //Re-open our binary writer in the drive
            bw = ourDrive.GetWriterIO();
            //Set our position
            bw.BaseStream.Position = m.DownToNearest200(m.BlockToFATOffset(ui, e));
            //Write our buffer
            bw.Write(buffer);
            return true;
        }

        public bool Rename(Entry e, string newName)
        {
            return Rename(e.EntryOffset, newName);
        }


        private bool Rename(long entryOffset, string newName)
        {
            if (m.CheckFileName(newName))
            {
                //Create our binary reader/writer
                FATX.IOReader br = ourDrive.GetIO();
                //Round our offset down to the nearest 0x200 boundary so we can read
                //the entry
                //Set our reader to do the same thing
                br.BaseStream.Position = m.DownToNearest200(entryOffset);
                //Split the difference so we can see how far in to our buffer
                //we need to be
                long offsetInBuffer = entryOffset - (m.DownToNearest200(entryOffset));
                //Create our buffer
                byte[] buffer = br.ReadBytes(0x200);
                //Close our reader - we don't need it any more
                br.Close();
                //Create a new binary reader that reads the buffer in memory
                FATX.IOWriter memWriter = new FATX.IOWriter(new System.IO.MemoryStream(buffer));
                //Set our position to the entry position
                memWriter.BaseStream.Position = offsetInBuffer;
                //Write the file name size
                memWriter.Write((byte)newName.Length);
                //Jump forward one byte and clear the old name
                memWriter.BaseStream.Position++;
                for (int i = 0; i < 0x2A; i++)
                {
                    memWriter.Write((byte)0xFF);
                }
                //Go back to where the file name is
                memWriter.BaseStream.Position = offsetInBuffer + 0x2;
                //Write the new name
                memWriter.Write((byte[])Encoding.ASCII.GetBytes(newName));
                memWriter.Close();
                //Open / set our writer's stuff
                FATX.IOWriter bw = ourDrive.GetWriterIO();
                bw.BaseStream.Position = m.DownToNearest200(entryOffset);
                //Write our edited buffer to the drive
                bw.Write((byte[])buffer);
                bw.Close();
                return true;
            }
            else
            {
                throw new Exception("File name not valid");
            }
        }

        public File WriteNewFile(Folder Root, string FilePath, ref bool Cancel)
        {
            int Garbage = 0;
            int Garbage2 = 0;
            return WriteNewFile(Root, FilePath, ref Garbage, ref Garbage2, ref Cancel);
        }

        private EntryData GetNewEntryData(Folder root, string Name)
        {
            if (m.CheckFileName(Name))
            {
                EntryData EData = new EntryData();
                object[] ourObject = CheckIfBlocksNeeded(root);
                //If we need to create a new entry...
                if (!(bool)ourObject[0])
                {
                    //Create our new entrydata that will serve as the EData for the new folder
                    Int32 timeStamp = m.DateTimeToFATInt(DateTime.Now);
                    byte[] ourArray = BitConverter.GetBytes(timeStamp);
                    byte[] CD = new byte[] { ourArray[2], ourArray[3] };
                    byte[] CT = new byte[] { ourArray[0], ourArray[1] };
                    EData.CreationDate = BitConverter.ToUInt16(CD, 0);
                    EData.CreationTime = BitConverter.ToUInt16(CT, 0);
                    EData.Name = Name;
                    EData.NameSize = (byte)Name.Length;
                    EData.Flags = 0x10;
                    EData.Size = 0x0;
                    //Uint for our blocks..
                    uint[] Blocks = new FATStuff(root).GetFreeBlocks(1, ((EntryData)ourObject[1]).StartingCluster, 0, false);
                    //Set our starting cluster using the "GetFreeBlocks" method - tell it we need one block, and the starting block is the block of the previous entry
                    EData.StartingCluster = Blocks[0];
                    //If we're using a block that we just created (the current block for the parent folder
                    //has free space
                    EData.EntryOffset = ((EntryData)ourObject[1]).EntryOffset;
                    //Create a new folder
                    Folder f = new Folder(ourDrive, root.PartInfo);
                    f.EData = EData;
                    f.BlocksOccupied = Blocks;
                    CreateNewEntry(EData);
                    WriteFATChain(new uint[] { f.StartingCluster}, f);
                    //Write ref 0xFF among the cluster
                    byte[] Buffer = new byte[f.PartInfo.ClusterSize];
                    for (int i = 0; i < Buffer.Length; i++)
                    {
                        Buffer[i] = 0xFF;
                    }
                    WriteToCluster(m.GetBlockOffset(f.BlocksOccupied[f.BlocksOccupied.Length - 1], f), Buffer, true);
                }
                //We are using a deleted entry
                else
                {
                    Int32 timeStamp = m.DateTimeToFATInt(DateTime.Now);
                    byte[] ourArray = BitConverter.GetBytes(timeStamp);
                    byte[] CD = new byte[] { ourArray[2], ourArray[3] };
                    byte[] CT = new byte[] { ourArray[0], ourArray[1] };
                    EData.CreationDate = BitConverter.ToUInt16(CD, 0);
                    EData.CreationTime = BitConverter.ToUInt16(CT, 0);
                    EData.Name = Name;
                    EData.NameSize = (byte)Name.Length;
                    EData.Flags = 0x10;
                    EData.Size = 0x0;
                    EData.StartingCluster = new FATStuff(root).GetFreeBlocks(1, ((EntryData)ourObject[1]).StartingCluster, 0, false)[0];
                    EData.EntryOffset = ((EntryData)ourObject[1]).EntryOffset;
                    Folder f = new Folder(ourDrive, root.PartInfo);
                    f.EData = EData;
                    CreateNewEntry(EData);
                    WriteFATChain(new uint[] { f.StartingCluster }, f);
                    //Write ref 0xFF among the cluster
                    byte[] Buffer = new byte[f.PartInfo.ClusterSize];
                    for (int i = 0; i < Buffer.Length; i++)
                    {
                        Buffer[i] = 0xFF;
                    }
                    WriteToCluster(m.GetBlockOffset(f.BlocksOccupied[f.BlocksOccupied.Length - 1], f), Buffer, true);
                }
                return EData;
            }
            throw new Exception("File name not valid");
        }

        private EntryData GetNewEntryDataOFFSET(Folder root, EntryData existing)
        {
            if (m.CheckFileName(existing.Name))
            {
                object[] ourObject = CheckIfBlocksNeeded(root);
                existing.EntryOffset = ((EntryData)ourObject[1]).EntryOffset;
                return existing;
            }
            throw new Exception("File name not valid");
        }

        // This shit's fuckin removed.  BROUGHT SOME BAD STUFF.
        //public bool WriteToCluster(uint Cluster, byte[] Buffer, bool AutoRound)
        //{
        //    System.Windows.Forms.MessageBox.Show("FUCK YOU");
        //    return false;
        //    return WriteToCluster((long)(Cluster * 0x4000), Buffer, true);
        //}

        public bool WriteToCluster(long Offset, byte[] Buffer, bool AutoRound)
        {
            FATX.IOWriter bw = ourDrive.GetWriterIO();
            try
            {
                if (AutoRound)
                {
                    List<byte> BufferList = Buffer.ToList();
                    long newLengthToAdd = m.UpToNearest200((int)BufferList.Count) - Buffer.Length;
                    BufferList.AddRange(new byte[newLengthToAdd]);
                    Buffer = BufferList.ToArray();
                    BufferList = null;
                }
                bw.BaseStream.Position = Offset;
                if (bw.BaseStream.Position < 0x80000 && (ourDrive.IsUSB || ourDrive.DriveType == Info.DriveType.HDD))
                {
                    throw new Exception("WHOA!  We almost wrote to the beginning of the drive...  PLEASE report this to me ASAP @ clkxu5@gmail.com");
                }
                bw.Write(Buffer);
                bw.Close();
            }
            catch (Exception e) { bw.Close(); throw e; }
            return true;
        }

        //public bool WriteToSector(int Sector, byte[] Buffer)
        //{
        //    return WriteToSector(Sector * 0x200, Buffer);
        //}

        //public bool WriteToSector(long Offset, byte[] Buffer)
        //{
        //    if (Buffer.Length != 0x200)
        //    {
        //        throw new Exception("Buffer is not the proper sector size");
        //    }
        //    FATX.IOWriter bw = ourDrive.GetWriterIO();
        //    bw.BaseStream.Position = Offset;
        //    bw.Write(Buffer);
        //    bw.Close();
        //    return true;
        //}

        public File WriteNewFile(Folder Root, string FilePath, ref int Progress, ref int BlocksToWrite, ref bool Cancel)
        {
            // If they're trying to write a null file...
            if (new System.IO.FileInfo(FilePath).Length == 0)
            {
                return WriteNullFile(Root, FilePath, ref Progress, ref BlocksToWrite);
            }
            FATX.IOReader br = null;
            File newFile = null;
            //Do a try...
            try
            {
                // Before we do ANYTHING at all, check if the folder already contains a file
                // with this name.
                foreach (File f in Root.Files(false))
                {
                    // A file already appears with this name, throw an error telling them that
                    if (f.Name.ToLower() == new System.IO.FileInfo(FilePath).Name.ToLower())
                    {
                        throw new Exception("A file with the name \"" + f.Name + "\" already exists in this folder");
                    }
                }
                //Get our entry
                EntryData EData = GetNewEntryData(Root, new System.IO.FileInfo(FilePath).FullName);
                EData.Flags = 0x0;
                EData.Size = (uint)new System.IO.FileInfo(FilePath).Length;
                // Create our new file
                newFile = new File(Root.Drive, Root.PartInfo);
                // Set our file entry data
                newFile.EData = EData;
                //Create our new fatstuff to get our free blocks
                FATStuff fs = new FATStuff(Root);
                //Get our free blocks
                //Get blocks needed
                int blocksNeeded = (int)(m.UpToNearestCluster(new System.IO.FileInfo(FilePath).Length, Root.PartInfo.ClusterSize) / Root.PartInfo.ClusterSize);
                uint[] blocks = new uint[0];
                if (Cancel)
                {
                    //br.Close();
                    return newFile;
                }
                if ((blocksNeeded - 1) != 0)
                {
                    blocks = fs.GetFreeBlocks(blocksNeeded - 1, EData.StartingCluster, 0, false);
                }
                //Make a new list for the blocks...
                List<uint> COCKS = blocks.ToList<uint>();
                //Insert the beginning block at the 0 index
                COCKS.Insert(0, EData.StartingCluster);
                //Make the cocks an array
                blocks = COCKS.ToArray();
                if (Cancel)
                {
                    br.Close();
                    return newFile;
                }
                //Write the FAT chain
                WriteFATChain(blocks, Root);

                // Create our entry in the folder (we waited up until this point to do it it,
                // because if we write it beforehand and we run ref of storage, then the
                // entry is there, but no data is.
                CreateNewEntry(EData);

                // Update the current progress (did a double-space here to make this look more badass)
                Progress = 0;
                // Update the max value
                BlocksToWrite = blocks.Length;

                if (Cancel)
                {
                    br.Close();
                    return newFile;
                }
                //Create our binary reader to read our file
                br = new FATX.IOReader(new System.IO.FileStream(FilePath, System.IO.FileMode.Open));
                for (int i = 0; i < blocks.Length - 1; i++, Progress++)
                {
                    if (Cancel)
                    {
                        br.Close();
                        return newFile;
                    }
                    WriteToCluster(m.GetBlockOffset(blocks[i], Root), br.ReadBytes((int)Root.PartInfo.ClusterSize), true);
                }
                if (Cancel)
                {
                    br.Close();
                    return newFile;
                }
                //The last cluster may not necessarily be the cluster size, so we have to read exactly the size of the remaining portion
                int sizeRemaining = (int)EData.Size - ((blocks.Length - 1) * (int)Root.PartInfo.ClusterSize);
                WriteToCluster(m.GetBlockOffset(blocks[blocks.Length - 1], Root), br.ReadBytes(sizeRemaining), true);
                Progress++;
                br.Close();
                return newFile;
            }
            catch (Exception e)
            {
                try
                {
                    br.Close();
                }
                catch { }
                if (newFile != null)
                {
                    newFile.Delete();
                }
                throw e;
            }
        }

        public bool ClearFATChain(uint[] Blocks, Entry e)
        {
            for (int i = 0; i < Blocks.Length; i++)
            {
                //Open our binary reader to read our buffer
                FATX.IOReader br = ourDrive.GetIO();
                //Get the position we should be at for our buffer
                long BufferOffset = m.DownToNearest200(m.BlockToFATOffset(Blocks[i], e));
                //Set our position to the buffer offset
                br.BaseStream.Position = BufferOffset;
                //Read our buffer
                byte[] Buffer = br.ReadBytes(0x200);
                //Close our reader -- we don't need it any more
                br.Close();
                //Open our binary writer in to a memory stream
                FATX.IOWriter bw = new FATX.IOWriter(new System.IO.MemoryStream(Buffer));
                //Set our position in the buffer to where the actual block is
                long blockoffsetinfat = m.BlockToFATOffset(Blocks[i], e);
                bw.BaseStream.Position = blockoffsetinfat - BufferOffset;
                //Write ref free block
                if (e.PartInfo.EntrySize == Info.PartitionBit.FATX16)
                {
                    bw.Write(new byte[] { 0x00, 0x00 });
                }
                else
                {
                    bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                }
                //Close our binary writer
                bw.Close();
                //Check to see if we can write ref more blocks from within the buffer, so we 
                //don't have to re-read what we already have
                i += CheckWriteBuffDel(ref Buffer, BufferOffset, Blocks, i + 1, e);
                //Re-open our binary writer, on the drive
                bw = ourDrive.GetWriterIO();
                //Set the position
                bw.BaseStream.Position = BufferOffset;
                //Write ref the buffer
                bw.Write(Buffer);
                bw.Close();
            }
            return true;
        }

        public File OverWriteFile(File FileToOverwrite, string FilePath, ref bool Cancel)
        {
            int p = 0;
            int d = 0;
            return OverWriteFile(FileToOverwrite, FilePath, ref p, ref d, ref Cancel);
        }

        public File OverWriteFile(File FileToOverwrite, string FilePath, ref int Progress, ref int BlocksToWrite, ref bool Cancel)
        {
            // If they're trying to overwrite a null file, or if the file
            // that they're wanting to overwrite the existing file with is null...
            if (FileToOverwrite.IsNull && new System.IO.FileInfo(FilePath).Length != 0)
            {
                return OverWriteNullFile(FileToOverwrite.ParentFolder, FileToOverwrite, FilePath, ref Progress, ref BlocksToWrite, ref Cancel);
            }
            // If the current file is not null, and the new one is...
            else if (!FileToOverwrite.IsNull && new System.IO.FileInfo(FilePath).Length == 0)
            {
                return OverWriteFileWithNull(FileToOverwrite.ParentFolder, FileToOverwrite, FilePath, ref Progress, ref BlocksToWrite, ref Cancel);
            }
            // If both files are null...
            else if (FileToOverwrite.IsNull && new System.IO.FileInfo(FilePath).Length == 0)
            {
                // Do nothing, the outcome would be the same, minus modification date
                return FileToOverwrite;
            }
            FATX.IOReader br = null;
            //Do a try...
            try
            {
                if (Cancel)
                {
                    //br.Close();
                    return FileToOverwrite;
                }
                //Create our new fatstuff to get our free blocks
                FATStuff fs = new FATStuff(FileToOverwrite);
                //Get our entry
                EntryData EData = FileToOverwrite.EData;
                // Set the modified date to today
                EData.ModifiedDate = m.DateTimeToFATShort(DateTime.Now, true);
                EData.ModifiedTime = m.DateTimeToFATShort(DateTime.Now, false);
                EData.Size = (uint)new System.IO.FileInfo(FilePath).Length;
                File newFile = new File(FileToOverwrite.Drive, FileToOverwrite.PartInfo);
                newFile.EData = EData;

                if (Cancel)
                {
                    //br.Close();
                    return FileToOverwrite;
                }

                //Get our blocks needed
                int BlocksNeeded = (int)(m.UpToNearestCluster(new System.IO.FileInfo(FilePath).Length, FileToOverwrite.PartInfo.ClusterSize) / FileToOverwrite.PartInfo.ClusterSize);
                //Create our block array for the blocks we do have
                uint[] BlocksWeHave = FileToOverwrite.BlocksOccupied;
                //If we have more blocks than we need already...
                if ((int)FileToOverwrite.BlocksOccupied.Length > BlocksNeeded)
                {
                    if (Cancel)
                    {
                        //br.Close();
                        return FileToOverwrite;
                    }
                    //Get our blocks that we're going to clear...
                    List<uint> BlocksList = FileToOverwrite.BlocksOccupied.ToList<uint>();
                    //Remove the blocks we need from the list of blocks to overwrite
                    BlocksList.RemoveRange(0x0, (int)((int)FileToOverwrite.BlocksOccupied.Length - BlocksNeeded));
                    //Finalize
                    uint[] BlocksToFree = BlocksList.ToArray();
                    //Clears the blocks.
                    if (Cancel)
                    {
                        //br.Close();
                        return newFile;
                    }
                    ClearFATChain(BlocksToFree, FileToOverwrite);
                    //Make the final block in the series the ending block by writing 0xFFFF to it
                    uint EndBlock = FileToOverwrite.BlocksOccupied[(FileToOverwrite.BlocksOccupied.Length - 1) - BlocksNeeded];
                    if (FileToOverwrite.PartInfo.EntrySize == Info.PartitionBit.FATX16)
                    {
                        WriteBlock(EndBlock, new byte[] { 0xFF, 0xFF }, FileToOverwrite);
                    }
                    else
                    {
                        WriteBlock(EndBlock, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, FileToOverwrite);
                    }
                    BlocksList = FileToOverwrite.BlocksOccupied.ToList<uint>();
                    BlocksList.RemoveRange(0x0, (FileToOverwrite.BlocksOccupied.Length - 1) - BlocksNeeded);
                    BlocksWeHave = BlocksList.ToArray();
                }
                else if ((int)FileToOverwrite.BlocksOccupied.Length < BlocksNeeded)
                {
                    //Get the number of blocks we REALLY need
                    int RealBlocksNeeded = BlocksNeeded - FileToOverwrite.BlocksOccupied.Length;
                    //Write ref the FAT chain from that last block
                    List<uint> bl = new List<uint>();
                    bl.Add(FileToOverwrite.BlocksOccupied[FileToOverwrite.BlocksOccupied.Length - 1]);
                    uint[] newBlocks = new FATStuff(FileToOverwrite).GetFreeBlocks(RealBlocksNeeded, bl[0], 0, false);
                    bl.AddRange(newBlocks);
                    //Set the BlocksWeHave
                    BlocksWeHave = bl.ToArray();
                }

                if (Cancel)
                {
                    //br.Close();
                    return newFile;
                }
                // Create our entry in the folder (we waited up until this point to do it it,
                // because if we write it beforehand and we run ref of storage, then the
                // entry is there, but no data is.
                CreateNewEntry(EData);

                // Set our progress max
                BlocksToWrite = BlocksWeHave.Length;
                // Set our progress
                Progress = 0;
                //Create our binary reader to read our file
                br = new FATX.IOReader(new System.IO.FileStream(FilePath, System.IO.FileMode.Open));
                for (int i = 0; i < BlocksWeHave.Length - 1; i++, Progress++)
                {
                    if (Cancel)
                    {
                        br.Close();
                        return newFile;
                    }
                    WriteToCluster(m.GetBlockOffset(BlocksWeHave[i], FileToOverwrite), br.ReadBytes((int)FileToOverwrite.PartInfo.ClusterSize), true);
                }
                if (Cancel)
                {
                    br.Close();
                    return newFile;
                }
                //The last cluster may not necessarily be the cluster size, so we have to read exactly the size of the remaining portion
                int sizeRemaining = (int)EData.Size - ((BlocksWeHave.Length - 1) * (int)FileToOverwrite.PartInfo.ClusterSize);
                WriteToCluster(m.GetBlockOffset(BlocksWeHave[BlocksWeHave.Length - 1], FileToOverwrite), br.ReadBytes(sizeRemaining), true);
                Progress++;
                br.Close();
                return newFile;
            }
            catch (Exception e)
            {
                try
                {
                    br.Close();
                }
                catch { }
                throw e;
            }
        }

        private int CheckWriteBuffDel(ref byte[] Buffer, long BufferOffset, uint[] allBlocks, int index, Entry e)
        {
            if (index == allBlocks.Length)
            {
                return 0;
            }
            //This will be our return value so that we can tell our other function how
            //many blocks were written
            int blocksWritten = 0;
            //If the offset for the next block's buffer is the same as our current buffer's offset
            try
            {
                long offset = m.DownToNearest200(m.BlockToFATOffset(allBlocks[index], e));
                if (m.DownToNearest200(m.BlockToFATOffset(allBlocks[index], e)) == BufferOffset)
                {
                    //Get our offset in our buffer for the block we're writing to
                    long OffsetinBuffer = m.BlockToFATOffset(allBlocks[index], e) - BufferOffset;
                    //Create our binary writer
                    FATX.IOWriter bw = new FATX.IOWriter(new System.IO.MemoryStream(Buffer));
                    //Go to the offset in the buffer
                    bw.BaseStream.Position = OffsetinBuffer;
                    //Create our byte array (we do new byte[1] just as a placeholder)
                    byte[] BlocktoWrite = new byte[1];
                    //If we're on a 2-byte block entry
                    if (e.PartInfo.EntrySize == Info.PartitionBit.FATX16)
                    {
                        //Our block to write is the ending block
                        BlocktoWrite = new byte[] { 0x00, 0x00 };
                    }
                    else
                    {
                        //Our block to write is the ending block
                        BlocktoWrite = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                    }
                    //Write ref our array
                    bw.Write(BlocktoWrite);
                    //Close our writer
                    bw.Close();
                    blocksWritten++;
                    //If we didn't just write the last block...
                    if (index != allBlocks.Length)
                    {
                        //Repeat
                        blocksWritten += CheckWriteBuffDel(ref Buffer, BufferOffset, allBlocks, index + blocksWritten, e);
                    }
                }
            }
            catch { return blocksWritten; }
            return blocksWritten;
        }

        // Moves a file or folder to the destination folder
        public void MoveEntry(Entry e, Folder destination)
        {
            System.Diagnostics.Trace.WriteLine("Starting the move of " + e.Name + " to " + destination.Name + "\r\nCreating new entry");
            // Get our new entry data
            EntryData existing = GetNewEntryDataOFFSET(destination, e.EData);
            // Write this shit
            CreateNewEntry(existing);
            // Now delete the old one
            MarkEntryAsDeleted(e);
            // DONE!
            System.Diagnostics.Trace.WriteLine("Move finished");
        }

        public Folder CreateFromPath(Folder parent, string Path)
        {
            // Split our path
            string[] Split = new string[] { Path };
            if (Path.Contains('\\'))
            {
                Split = Path.Split('\\');
            }

            bool Found = false;

            // Check to see if we're on the bottom-most folder
            foreach (Folder f in parent.SubFolders(false))
            {
                if (f.Name.ToLower() == Split[0])
                {
                    // If this was the folder we were looking to create...
                    if (Split.Length == 1)
                    {
                        return f;
                    }
                    string NewPath = "";
                    for (int i = 1; i < Split.Length; i++)
                    {
                        NewPath += (i == Split.Length - 1) ? Split[i] : Split[i] + "\\";
                    }
                    // Call down again
                    return CreateFromPath(f, Path);
                }
            }

            // If the folder is yet to be created...
            if (!Found)
            {
                // If this isn't the last folder...
                if (Split.Length != 1)
                {
                    string NewPath = "";
                    for (int i = 1; i < Split.Length; i++)
                    {
                        NewPath += (i == Split.Length - 1) ? Split[i] : Split[i] + "\\";
                    }
                    // Create it and call down again!
                    return CreateFromPath(parent.NewFolder(Split[0]), NewPath);
                }
                else
                {
                    return parent.NewFolder(Split[0]);
                }
            }

            throw new Exception("Something weird happened...");
        }
    }
}