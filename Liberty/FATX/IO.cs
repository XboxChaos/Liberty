using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using FATX_Browser.Win32;
using Microsoft.Win32.SafeHandles;
using System.IO;
using Extensions;

namespace FATX
{
    class IO
    {

        public class HDDFATX
        {
            private long driveSize;
            protected internal System.IO.Stream fs;
            protected internal FATX.IOReader br;
            protected internal FATX.IOWriter bw;
            SafeFileHandle diskDrive;
            private bool Dump;
            private string DumpPath;

            public HDDFATX(bool dump, string dumpPath)
            {
                if (dump)
                {
                    Dump = dump;
                    DumpPath = dumpPath;
                }
            }

            #region Get FATX Drives

            /// <summary>
            /// Returns an array of valid FATXDrives
            /// </summary>
            public FATX.FATXDrive[] GetFATXDrives(int range)
            {
                List<FATX.FATXDrive> driveList = new List<FATX.FATXDrive>();
                try
                {
                    ///Gets physical disks
                    for (int i = 0; i < range; i++)
                    {
                        try
                        {
                            //Start reading the physical drive
                            fs = fs_diskStream(i);
                            br = br_diskReader(i);
                            //Seek to the FATX partition
                            try
                            {
                                br.BaseStream.Position = (long)FATX.Info.HDDFATX.Partitions.Data;
                                //Read the header
                                byte[] header = br.ReadBytes(0x4);
                                //Convert the header to a string, check if it's fatx
                                if (Encoding.ASCII.GetString(header) == "XTAF")
                                {
                                    FATX.FATXDrive drive = new FATXDrive(i, FATX.Info.DriveType.HDD);
                                    driveList.Add(drive);
                                }
                                // Check to see if it's a dev drive...
                                else
                                {
                                    br.BaseStream.Position = (long)FATX.Info.HDDFATX.DevOffsets.Content;
                                    // Read the header
                                    byte[] devheader = br.ReadBytes(0x4);
                                    // Convert header to a string, check to see if it's fatx
                                    if (Encoding.ASCII.GetString(devheader) == "XTAF")
                                    {
                                        FATX.FATXDrive drive = new FATXDrive(i, FATX.Info.DriveType.HDD);
                                        drive.IsDev = true;
                                        driveList.Add(drive);
                                    }
                                }
                            }
                            catch (Exception e) { br.Close(); fs.Close(); diskDrive.Close(); continue; }
                            br.Close();
                            fs.Close();
                            diskDrive.Close();
                            bool closed = diskDrive.IsClosed;
                        }
                        catch(Exception e) { fs.Close(); br.Close(); diskDrive.Close(); continue; }
                    }
                    ///Gets usb drives
                    DriveInfo[] drives = DriveInfo.GetDrives();
                    ///Check to see if the drive has a subdirectory of "Xbox360"
                    foreach (DriveInfo drive in drives)
                    {
                        if (drive.Name == @"A:\" || drive.Name == @"B:\" ||
                            drive.DriveType == DriveType.NoRootDirectory || drive.DriveType == DriveType.CDRom ||
                            drive.DriveType == DriveType.Network || drive.DriveType == DriveType.Unknown)
                        {
                            continue;
                        }

                        //If the directory does not exist
                        DirectoryInfo di = new DirectoryInfo(drive.Name + "Xbox360");
                        if (!di.Exists)
                        {
                            continue;
                        }
                        else
                        {
                            //Check to make sure that there's actually files in that directory..
                            FileInfo[] fi = di.GetFiles();
                            bool found = false;
                            foreach (FileInfo file in fi)
                            {
                                if (file.Name == "Data0000" || file.Name == "Data0001" || file.Name == "Data0002")
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                                continue;
                        }
                        driveList.Add(new FATXDrive(drive.Name + "Xbox360", Info.DriveType.USB, drive.VolumeLabel));
                    }
                    return driveList.ToArray();
                }
                catch { return driveList.ToArray(); }
            }

            #endregion

            #region Get Drive Size

            /// <summary>
            /// Returns the size of a HDD
            /// </summary>
            public long GetDriveSize(int deviceNumber)
            {
                API api = new API();
                var diskGeo = new API.DISK_GEOMETRY();
                bool result = api.GetDriveGeometry(ref diskGeo, deviceNumber, CreateHandle(deviceNumber));
                return diskGeo.DiskSize;
            }

            public long GetDriveSize()
            {
                return fs.Length;
            }

            #endregion

            #region File Stream/Binary Reader
            /// <summary>
            /// Stream for the disk
            /// </summary>
            protected internal System.IO.Stream fs_diskStream(int deviceNumber)
            {
                CreateHandle(deviceNumber);
                fs = new System.IO.FileStream(diskDrive, System.IO.FileAccess.ReadWrite);
                return fs;
            }

            protected internal System.IO.Stream fs_dumpStream()
            {
                try { fs.Close(); }
                catch { }
                fs = new System.IO.FileStream(DumpPath, FileMode.Open);
                return fs;
            }

            /// <summary>
            /// Returns a binary reader to read the disk
            /// </summary>
            protected internal FATX.IOReader br_diskReader(int deviceNumber)
            {
                br = new FATX.IOReader(fs_diskStream(deviceNumber));
                return br;
            }

            protected internal FATX.IOReader br_dumpReader()
            {
                try { br.Close(); }
                catch { }
                br = new FATX.IOReader(fs_dumpStream());
                try
                {
                    driveSize = br.BaseStream.Length;
                }
                catch { }
                return br;
            }

            /// <summary>
            /// Returns a binary writer to write to the disk
            /// </summary>
            protected internal FATX.IOWriter bw_diskWriter(int deviceNumber)
            {
                try { bw.Close(); }
                catch { }
                bw = new FATX.IOWriter(fs_diskStream(deviceNumber));
                return bw;
            }

            protected internal FATX.IOWriter bw_dumpWriter()
            {
                try { bw.Close(); }
                catch { }
                bw = new FATX.IOWriter(fs_dumpStream());
                try
                {
                    driveSize = bw.BaseStream.Length;
                }
                catch { }
                return bw;
            }

            /// <summary>
            /// Creates and returns a new handle for reading a drive
            /// </summary>
            public SafeFileHandle CreateHandle(int deviceNumber)
            {
                try { diskDrive.Close(); }
                catch { }
                // Gotta multi-line this shit so I'm not scrolling all the way across my screen
                diskDrive = API.CreateFile(@"\\.\PhysicalDrive" + deviceNumber.ToString(),
                    System.IO.FileAccess.ReadWrite,
                    System.IO.FileShare.ReadWrite,
                    IntPtr.Zero, System.IO.FileMode.Open,
                    API.FlagsAndAttributes.Device | API.FlagsAndAttributes.NoBuffering | API.FlagsAndAttributes.Write_Through,
                    IntPtr.Zero);
                return diskDrive;
            }

            #endregion

            /// <summary>
            /// Closes the readers/handles
            /// </summary>
            public bool Close()
            {
                try
                {
                    fs.Close();
                    br.Close();
                    bw.Close();
                    diskDrive.Close();
                    return true;
                }
                catch { return true; }
            }
        }
    }

    #region IO
    public class IOWriter : System.IO.BinaryWriter
    {
        public IOWriter(string[] ye)
            : base(new IOStream(ye, System.IO.FileMode.Open))
        {

        }

        public IOWriter(Stream stream)
            : base(stream)
        {

        }
    }

    public class IOReader : System.IO.BinaryReader
    {
        public IOReader(string[] ye)
            : base(new IOStream(ye, System.IO.FileMode.Open))
        {

        }

        public IOReader(Stream stream)
            : base(stream)
        {

        }

        public override void Close()
        {
            base.Close();
        }

        public int ReadUInt16(bool BigEndian)
        {
            if (BigEndian)
            {
                byte[] buffer = ReadBytes(0x2);
                Array.Reverse(buffer);
                return BitConverter.ToUInt16(buffer, 0x0);
            }
            return base.ReadUInt16();
        }

        public int ReadInt32(bool BigEndian)
        {
            if (BigEndian)
            {
                byte[] buffer = ReadBytes(0x4);
                Array.Reverse(buffer);
                return BitConverter.ToInt32(buffer, 0x0);
            }
            return ReadInt32();
        }

        public uint ReadUInt32(bool BigEndian)
        {
            if (BigEndian)
            {
                byte[] Buffer = ReadBytes(0x4);
                Array.Reverse(Buffer);
                return BitConverter.ToUInt32(Buffer, 0x0);
            }
            return ReadUInt32();
        }

        public string ReadUnicodeString(int length)
        {
            string ss = "";
            for (int i = 0; i < length; i += 2)
            {
                char c = (char)ReadUInt16(true);
                if (c != '\0')
                {
                    ss += c;
                }
            }
            return ss;
        }
    }

    public class IOStream : System.IO.Stream
    {
        int Current = 0;
        System.IO.FileStream[] Streams;
        public IOStream(string[] filePaths, System.IO.FileMode mode)
            : base()
        {
            Streams = new FileStream[filePaths.Length];
            for (int i = 0; i < Streams.Length; i++)
            {
                Streams[i] = new FileStream(filePaths[i], mode);
            }
        }

        public override bool CanRead
        {
            get { return Streams[Current].CanRead; }
        }

        public override bool CanSeek
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanWrite
        {
            get { return Streams[Current].CanRead; }
        }

        public override void Flush()
        {
            Streams[Current].Flush();
        }

        public override long Length
        {
            get 
            { 
                long length = 0;
                for (int i = 0; i < Streams.Length; i++)
                {
                    length += Streams[i].Length;
                }
                return length;
            }
        }

        public override long Position
        {
            get
            {
                // Loop through each stream before this one, and add that
                // to the return position
                long r3 = 0;
                for (int i = 0; i < Current; i++)
                {
                    // Add the length
                    r3 += Streams[i].Length;
                }
                // Add the position in our current stream
                return r3 + Streams[Current].Position;
            }
            set
            {
                // Reset the position in each stream
                for (int i = 0; i < Streams.Length; i++)
                {
                    Streams[i].Position = 0;
                }

                // Determine which stream we need to be on...
                long Remaining = value;
                for (int i = 0; i < Streams.Length; i++)
                {
                    if (Streams[i].Length < Remaining)
                    {
                        Remaining -= Streams[i].Length;
                    }
                    else
                    {
                        Current = i;
                        break;
                    }
                }

                // Check to see if we're at the end of a file...
                if (Remaining == Streams[Current].Length)
                {
                    // We were, so let's bump the current stream up one
                    Current++;
                    return;
                }
                Streams[Current].Position = Remaining;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            // If the amount of data they're wanting to be read can be
            // read within the current stream...

            // Oh yeah, this is the position before hand... we add the
            // count to this, just to make sure that all of our position are aligned
            long bPos = Position;

            if (Streams[Current].Length - Streams[Current].Position >= count)
            {
                // We can read the data, yaaayyyy
                Streams[Current].Read(buffer, offset, count);
            }
                // We can't read it... OH NO! Gotta do some trickery
            else
            {
                // Let's declare out ints here.  First, the data we have to read,
                // then the streams that we have to read from (count), then
                // the amount of data that we can read from this current stream
                long DataLeft = count, streams = 0, DataCurrent = Streams[Current].Length - Streams[Current].Position;

                // Loop through each higher stream, getting the amount of data we can read
                // from each, and if the amount of data is still higher than the data left,
                // then loop again
                for (long i = Current + 1, Remaining = DataLeft - DataCurrent; i < Streams.Length; i++)
                {
                    // Bump up our streams
                    streams++;

                    // If the stream length is smaller than the remaining data...
                    if (Streams[i].Length >= Remaining)
                    {
                        // We can break!
                        break;
                    }
                }

                // Read our beginning data
                DataLeft -= Streams[Current].Read(buffer, offset, count);

                // Loop through each stream, reading the rest of the data
                for (int i = 0, cS = (Current + 1); i < streams; i++, cS++)
                {
                    byte[] Temp = new byte[0];
                    if (i == streams - 1)
                    {
                        Temp = new byte[DataLeft];
                    }
                    else
                    {
                        Temp = new byte[Streams[cS].Length];
                    }

                    // Read the data in to our temp array
                    Streams[cS].Read(Temp, 0, Temp.Length);

                    // Copy that in to the pointed array
                    Array.Copy(Temp, 0, buffer, count - DataLeft, Temp.Length);

                    DataLeft -= Streams[cS].Length;
                }
            }

            Position += bPos + count;

            // Return count.  Hax.
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            /* COPYPASTA FROM READ FUNCTION! */


            // If the amount of data they're wanting to be read can be
            // read within the current stream...

            // Oh yeah, this is the position before hand... we add the
            // count to this, just to make sure that all of our position are aligned
            long bPos = Position;

            if (Streams[Current].Length - Streams[Current].Position >= count)
            {
                // We can write the data, yaaayyyy
                Streams[Current].Write(buffer, offset, count);
            }
            // We can't read it... OH NO! Gotta do some trickery
            else
            {
                // Let's declare out ints here.  First, the data we have to read,
                // then the streams that we have to read from (count), then
                // the amount of data that we can read from this current stream
                long DataLeft = count, streams = 0, DataCurrent = Streams[Current].Length - Streams[Current].Position;

                // Loop through each higher stream, getting the amount of data we can read
                // from each, and if the amount of data is still higher than the data left,
                // then loop again
                for (long i = Current + 1, Remaining = DataLeft - DataCurrent; i < Streams.Length; i++)
                {
                    // Bump up our streams
                    streams++;

                    // If the stream length is smaller than the remaining data...
                    if (Streams[i].Length >= Remaining)
                    {
                        // We can break!
                        break;
                    }
                }
                
                // Copy the first wave of data in to a temp array
                byte[] Temp = new byte[DataCurrent];
                Array.Copy(buffer, 0, Temp, 0, Temp.Length);

                // Write our beginning data
                Streams[Current].Write(buffer, 0, Temp.Length);
                DataLeft -= Temp.Length;

                // Loop through each stream, reading the rest of the data
                for (int i = 0, cS = (Current + 1); i < streams; i++, cS++)
                {
                    Temp = new byte[0];
                    if (i == streams - 1)
                    {
                        Temp = new byte[DataLeft];
                    }
                    else
                    {
                        Temp = new byte[Streams[cS].Length];
                    }

                    Array.Copy(buffer, count - DataLeft, Temp, 0, Temp.Length);

                    // Read the data in to our temp array
                    Streams[cS].Write(Temp, 0, Temp.Length);

                    DataLeft -= Streams[cS].Length;
                }
            }

            Position += bPos + count;
        }

        public override void Close()
        {
            for (int i = 0; i < Streams.Length; i++)
            {
                Streams[i].Close();
            }
        }
    }

    public class FATXFileStream : Stream
    {
        FATX.File xFile;
        long xPositionInFile = 0;
        Misc m;
        Stream Underlying;
        byte[] PreviouslyRead = new byte[0];
        long PreviouslyReadOffset = -1;

        public FATXFileStream(string[] InPaths, FATX.File file)
        {
            if (file.Size == 0)
            {
                Close();
                throw new Exception("Null files not supported");
            }
            xFile = file;
            m = new Misc();
            // Set our position to the beginning of the file
            long off = m.GetBlockOffset(xFile.BlocksOccupied[0], xFile);
            Underlying = new IOStream(InPaths, FileMode.Open);
            Underlying.Position = off;
        }

        public FATXFileStream(SafeFileHandle handle, System.IO.FileAccess fa, FATX.File file)
        {
            //Underlying.Close();
            if (file.Size == 0)
            {
                Close();
                throw new Exception("Null files not supported");
            }
            xFile = file;
            m = new Misc();
            // Set our position to the beginning of the file
            long off = m.GetBlockOffset(xFile.BlocksOccupied[0], xFile);
            Underlying = new FileStream(handle, fa);
            Underlying.Position = off;
        }

        public FATXFileStream(string Path, System.IO.FileMode fmode, FATX.File file)
        {
            //Underlying.Close();
            if (file.Size == 0)
            {
                Close();
                throw new Exception("Null files not supported");
            }
            xFile = file;
            m = new Misc();
            // Set our position
            long off = m.GetBlockOffset(xFile.BlocksOccupied[0], xFile);
            Underlying = new FileStream(Path, fmode);
            Underlying.Position = off;
        }

        public override long Position
        {
            get
            {
                /* If we return the Underlying position, then we're returning the offset
                 * for the entire thing, not just the individual file we're trying
                 * to read*/
                return xPositionInFile;
            }
            set
            {
                if (value > xFile.Size)
                {
                    return;
                    throw new Exception("Can not read beyond end of file. Tard");
                }
                xPositionInFile = value;
                Underlying.Position = GetRealSectorOffset(value);
            }
        }

        public override void WriteByte(byte value)
        {
            Underlying.WriteByte(value);
        }

        public override void Write(byte[] array, int offset, int count)
        {
            Underlying.Write(array, offset, count);
        }

        public override long Length
        {
            get
            {
                return xFile.Size;
            }
        }

        // I dont' know why I'm even going to bother with this function.
        // I don't think I'm going to ever use it.
        public override int ReadByte()
        {
            // Check if we're at the edge of a cluster...
            if (RealOffset == m.UpToNearestClusterForce(RealSectorOffset, xFile.PartInfo.ClusterSize))
            {
                Underlying.Position = m.GetBlockOffset(xFile.BlocksOccupied[DetermineBlockIndex(m.UpToNearestClusterForce(RealSectorOffset, xFile.PartInfo.ClusterSize))], xFile);
                xPositionInFile++;
                return Underlying.ReadByte();
            }
            // Check if we're at the beginning of a sector...
            if (RealOffset == m.DownToNearest200(RealOffset))
            {
                xPositionInFile++;
                return Underlying.ReadByte();
            }
            // We aren't at the beginning of a sector, and we're not at the end of a cluster
            // We must be somewhere in-between, so we've got to do some hax.
            byte[] b = new byte[1];
            Read(b, 0, 1);
            return (int)b[0];
            // I think I made it return that first byte for some reason, but idk
            // oh yeeeuh, I wanted it to read from the nearest 0x200 byte boundary
            // so if we keep calling .ReadByte() it would have that shit cached
            // idk why i didn't do that
            int index = (int)(RealOffset - RealSectorOffset);
            if (Position.DownToNearest200() == PreviouslyReadOffset && index < PreviouslyRead.Length)
            {
                xPositionInFile++;
                return (int)PreviouslyRead[index];
            }
            else
            {
                byte[] buffer = new byte[0];
                // Read the buffer
                if (Length - Position >= 0x200)
                {
                    buffer = new byte[0x200];
                }
                else
                {
                    buffer = new byte[(Length - Position)];
                }
                index = (int)(RealOffset - RealSectorOffset);
                Read(buffer, 0, buffer.Length);
                try
                {
                    Position -= buffer.Length - 1;
                }
                catch { }
                // Set the previously read to thissssssssss
                PreviouslyRead = buffer;
                PreviouslyReadOffset = Position.DownToNearest200();
                // Return the value at the index we should be at
                return (int)buffer[index];
            }
        }

        #region Notes on these read functions

        /* So Microsoft likes to be weird and return an int for how many
         * bytes were read, vs. the array that they read... so basically
         * the array comes in as all null bytes, then comes back filled
         * in (sort of like how when you leave your drink around at a bar
         * and come back, you find it filled up by some nice guy who wants
         * to drug you and take you back to his apartment).  SO, instead of
         * loading the array at the beginning of reading, we need to load
         * everything in to a different array each time, add that to a list,
         * then finally set the byte array.  I'm on drugs.*/

        #endregion

        public override int Read(byte[] array, int offset, int count)
        {
            if (this.Position == this.Length)
            {
                return 0;
            }
            long p = xPositionInFile;
            // Before we do anything, we're going to check our cached buffer
            // to see if we can do anything with our previous buffer

            uint CurrentIndex = 0;
            byte[] b_Return;
            // If the number of bytes they're reading is smaller than
            // the cluster size...
            // AND
            // If they want to read a small enough amount of data to where we can
            // read the data without any trickery...

            //Creatively named WHAT becuase I have no idea what I was doing here.
            long what = m.UpToNearestCluster(RealOffset, xFile.PartInfo.ClusterSize) - RealSectorOffset;
            if (count <= xFile.PartInfo.ClusterSize && what >= count)
            {
                // Get the amount to remove off of the beginning of our list...
                long v_bToRemove = RealOffset - RealSectorOffset;
                // Get the amount to remove off the the end of our list
                long up = m.UpToNearest200(RealOffset + count);
                long v_eToRemove = up - (RealOffset + count);
                // Get the total amount of data we have to read
                long v_ToRead = m.UpToNearest200(v_bToRemove + v_eToRemove + count);
                // Set our return value's length
                b_Return = new byte[v_ToRead];
                // Read our shit
                Underlying.Read(b_Return, offset, (int)v_ToRead);
                // Copy our return to the original array
                Array.Copy(b_Return, v_bToRemove, array, 0x0, b_Return.Length - (v_bToRemove + v_eToRemove));
                // Clear the b_Return array
                Array.Clear(b_Return, 0, b_Return.Length);
            }
            // Else, the data they want to read spans across multiple clusters,
            // yet is less than the cluster size itself
            else
            {
				long DataRead = 0;
                /* TODO:
                 * 1.) Get the amount of data we have to read total
                 * 2.) Get the amount of data we have to read for the beginning
                 * and the end of our read
                 * 3.) Get the amount of data we have to remove off of the
                 * beginning and end of our buffer
                 * 4.) Remove that.
                 * 5.) ????*/
                // Data to remove off of the beginning
                long v_bToRemove = RealOffset - RealSectorOffset;
                //long v_eToRemove = m.UpToNearest200(xPositionInFile + count + v_bToRemove) - (xPositionInFile + count + v_bToRemove);
                // Data total to read...
				// Get the amount of data we can read for this beginning cluster
                long v_Cluster = m.UpToNearestCluster(RealSectorOffset, xFile.PartInfo.ClusterSize) - RealSectorOffset;
                // Get the amount of data to skim off of the end.  By doing the number rounded up to the nearest 0x200 byte boundary
                // subtracted by the non-rounded number, we are efficiently getting the difference.  What.  Why did I say efficiently
                long v_eToRemove = m.UpToNearest200((count - v_Cluster) + v_bToRemove) - ((count - v_Cluster) + v_bToRemove);
                // This gets the number of bytes we have to read in the final cluster.
                long v_eToReadNotRounded = ((count - v_Cluster) + v_bToRemove) - m.DownToNearestCluster(count - v_Cluster + v_bToRemove, xFile.PartInfo.ClusterSize);
                // The amount of data to read for each other cluster...
                long v_ToReadBetween = ((count - v_Cluster) + v_bToRemove) - v_eToReadNotRounded;

                b_Return = new byte[v_Cluster];
                // Read the first bit of data...
                Underlying.Read(b_Return, 0, (int)v_Cluster);
                // Copy the return buffer to the actual array
                Array.Copy(b_Return, v_bToRemove, array, 0, b_Return.Length - v_bToRemove);
                // Clear the return
                b_Return = new byte[0];
                DataRead += v_Cluster - v_bToRemove;
                Position += DataRead;

                // Loop for each cluster inbetween
                if (((count - v_Cluster) + v_bToRemove - v_eToReadNotRounded) != 0)
                {
                    for (int i = 0; i < ClusterSpanned((count - v_Cluster) + v_bToRemove - v_eToReadNotRounded); i++)
                    {
                        b_Return = new byte[xFile.PartInfo.ClusterSize];
                        Underlying.Read(b_Return, 0, b_Return.Length);
                        // Copy the return buffer to the actual array
                        Array.Copy(b_Return, 0, array, DataRead, b_Return.Length);
                        DataRead += xFile.PartInfo.ClusterSize;
                        Position += xFile.PartInfo.ClusterSize;
                    }
                }

                // Read our final data...
                //Underlying.Position = m.GetBlockOffset(xFile.BlocksOccupied[CurrentIndex + 1], xFile);
                b_Return = new byte[m.UpToNearest200(count - DataRead)];
                // Read the data for this final cluster
                Underlying.Read(b_Return, 0, (int)m.UpToNearest200(count - DataRead));
                // Copy that to the array
                Array.Copy(b_Return, 0x0, array, DataRead, b_Return.Length - v_eToRemove);
                // Clear the buffer
                b_Return = new byte[0];
            }
            //PreviouslyRead = array;
            //PreviouslyReadOffset = xPositionInFile;
            Position = p + count;
            // Just return the count.  Assholes.
            return array.Length;
        }

        long ClusterOffset
        {
            get
            {
                long cluster = m.DownToNearestCluster(xPositionInFile, xFile.PartInfo.ClusterSize);
                // Return the actual block offset + difference
                return m.GetBlockOffset(xFile.BlocksOccupied[DetermineBlockIndex(cluster)], xFile);
            }
        }

        long RealOffset
        {
            get
            {
                // Round the number down to the nearest cluster so that we
                // can easily get the cluster index
                long cluster = m.DownToNearestCluster(xPositionInFile, xFile.PartInfo.ClusterSize);
                uint index = (uint)(cluster / xFile.PartInfo.ClusterSize);
                // Get the difference so we can add it later...
                long dif = xPositionInFile - cluster;
                cluster = m.GetBlockOffset((uint)xFile.BlocksOccupied[index], xFile) + dif;
                // Return the actual block offset + difference
                return cluster;
            }
        }

        long RealSectorOffset
        {
            get
            {
                return GetRealSectorOffset(xPositionInFile);
            }
        }

        long GetRealSectorOffset(long off)
        {
            // Get the size up to the nearest cluster
            // Divide by cluster size
            // That is the block index.
            long SizeInCluster = m.DownToNearest200(off - m.DownToNearestCluster(off, xFile.PartInfo.ClusterSize));//m.GetBlockOffset(xFile.StartingCluster) + 0x4000;            long SizeInCluster = m.DownToNearestCluster(off, xFile.PartInfo.ClusterSize) / xFile.PartInfo.ClusterSize)
            uint Cluster = (uint)(m.DownToNearestCluster(off, xFile.PartInfo.ClusterSize) / xFile.PartInfo.ClusterSize);
            //Cluster = (Cluster == 0) ? 0 : Cluster - 1;
            try
            {
                long Underlying = m.GetBlockOffset(xFile.BlocksOccupied[Cluster], xFile);
                return Underlying + SizeInCluster;
            }
            catch { return m.GetBlockOffset(xFile.BlocksOccupied[Cluster - 1], xFile); }
        }

        uint DetermineBlockIndex(long Off)
        {
            // Pre-planning... I need to figure ref the rounded offset in order
            // to determine the cluster that this bitch is in
            // So now that we have the rounded number, we can 
            long rounded = m.DownToNearestCluster(Off, xFile.PartInfo.ClusterSize);
            // Loop for each cluster, determining if the sizes match
            for (uint i = 0; i < xFile.BlocksOccupied.Length; i++)
            {
                long off = m.GetBlockOffset(xFile.BlocksOccupied[i], xFile);
                if (off == rounded)
                {
                    return i;
                }
            }
            throw new Exception("Block not allocated to this file!");
        }

        // Returns the number of clusters that the value (size?) will span across
        uint ClusterSpanned(long value)
        {
            // Add the cluster size because if we don't, then upon doing this math we
            // will get the actual number - 1
            // EXAMPLE: number = 0x689 or something, and we round it down.  That number
            // is now 0, and 0/x == 0.
            long rounded = m.DownToNearestCluster(value, xFile.PartInfo.ClusterSize) + xFile.PartInfo.ClusterSize;
            // Divide rounded by cluster size to see how many clusters it spans across...
            return (uint)(rounded / xFile.PartInfo.ClusterSize);
        }

        public override void Close()
        {
            Underlying.Close();
            base.Close();
        }

        public override bool CanRead
        {
            get { return Underlying.CanRead; }
        }

        public override bool CanSeek
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanWrite
        {
            get { throw new NotImplementedException(); }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
    }

    public class NANDStream : System.IO.FileStream
    {
        public NANDStream(string path):base(path, System.IO.FileMode.Open)
        {
            // ye, doin nothin.  Like a boss.
        }
    }
    #endregion
}
