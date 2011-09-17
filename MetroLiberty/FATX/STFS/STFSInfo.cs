using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FATX;
using System.Windows.Controls;

namespace STFS
{
    public class STFSInfo
    {
        FATXDrive xDrive;
        File xFile;
        byte[] Buffer;
        Misc m;
        IOReader IO;
        FATX.Structs.STFSInfo info;
        public STFSInfo(File f)
        {
            string name = f.Name;
            xFile = f;
            xDrive = f.Drive;
            m = new Misc();
            info = new FATX.Structs.STFSInfo();
        }

        public FATX.Structs.STFSInfo GetInfo()
        {
            info.ConsoleID = ConsoleID();
            info.ContentName = DisplayName();
            info.DeviceID = DeviceID();
            info.Magic = Magic();
            info.TitleID = TitleID();
            info.TitleName = GameName();
            info.ProfileID = ProfileID();
            return info;
        }

        public enum Offsets
        {
            TitleID = 0x360,
            ConsoleID = 0x36c,
            DeviceID = 0x3FD,
            DisplayName = 0x411,
            TitleName = 0x1691,
            ProfileID = 0x371,
            ContentImage = 0x171A,
            ContentImageSize = 0x1712,
            TitleImageSize = 0x1716,
            TitleImage = 0x571A,
        }

        public byte[] Magic()
        {
            try
            {
                if (xFile.IsDeleted)
                {
                    return new byte[4];
                }
                // We didn't return the null array, so let's boogie
                IO = xFile.Drive.GetIO();
                IO.BaseStream.Position = new Misc().GetBlockOffset(xFile.StartingCluster, xFile);
                byte[] Header = IO.ReadBytes(0x200);
                byte[] buffer = new byte[] { Header[0], Header[1], Header[2], Header[3] };
                IO.Close();
                return buffer;
            }
            catch { Close(); return new byte[4]; }
        }

        public string GameName()
        {
            try
            {
                IO = GetIO();
                IO.BaseStream.Position = (int)Offsets.TitleName;
                byte[] b = IO.ReadBytes(0x80);
                IO.Close();
                Buffer = null;
                string ss = "";
                //naw return Encoding.Unicode.GetString(b);
                IO = new IOReader(new System.IO.MemoryStream(b));
                for (int i = 0; i < b.Length; i += 2)
                {
                    char c = (char)IO.ReadUInt16(true);
                    if (c != '\0')
                    {
                        ss += c;
                    }
                }
                IO.Close();
                return ss;
            }
            catch { Close(); return ""; }
        }

        public uint TitleID()
        {
            try
            {
                IO = GetIO();
                IO.BaseStream.Position = (int)Offsets.TitleID;
                uint ui = IO.ReadUInt32(true);
                IO.Close();
                return ui;
            }
            catch { Close(); return 0; }
        }

        public byte[] ConsoleID()
        {
            try
            {
                IO = GetIO();
                IO.BaseStream.Position = (int)Offsets.ConsoleID;
                byte[] buffer = IO.ReadBytes(0x5);
                IO.Close();
                return buffer;
            }
            catch { Close(); return new byte[0x5]; }
        }

        public byte[] DeviceID()
        {
            try
            {
                IO = GetIO();
                IO.BaseStream.Position = (int)Offsets.DeviceID;
                byte[] buffer = IO.ReadBytes(0x14);
                IO.Close();
                return buffer;
            }
            catch { Close(); return new byte[0x14]; }
        }

        public byte[] ProfileID()
        {
            try
            {
                IO = GetIO();
                IO.BaseStream.Position = (int)Offsets.ProfileID;
                byte[] buffer = IO.ReadBytes(0x8);
                IO.Close();
                return buffer;
            }
            catch { Close(); return new byte[8]; }
        }

        public string DisplayName()
        {
            try
            {
            IO = GetIO();
            IO.BaseStream.Position = (int)Offsets.DisplayName;
            string ss = "";
            for (int i = 0; i < 0x80; i += 2)
            {
                char c = (char)IO.ReadUInt16(true);
                if (c != '\0')
                {
                    ss += c;
                }
            }
            IO.Close();
            return ss;
            }
            catch { Close(); return ""; }
        }

        private IOReader GetIO()
        {
            try
            {
                IO.Close();
            }
            catch { }
            CheckBuffer();
            IO = new IOReader(new System.IO.MemoryStream(Buffer));
            //IO = new IOReader(xFile.GetStream());
            return IO;
        }

        private IOReader GetFileDiskIO()
        {
            try
            {
                IO.Close();
            }
            catch { }
            IO = new IOReader(xFile.GetStream());
            return IO;
        }

        private void CheckBuffer()
        {
            try
            {
                if (Buffer == null)
                {
                    IO = new IOReader(xFile.GetStream());
                    Buffer = IO.ReadBytes(0x2200);
                    //Close our IO
                    IO.Close();
                }
            }
            catch { Close(); }
        }

        private uint GuessBlock(long offset)
        {
            return (uint)(m.DownToNearestCluster(offset, xFile.PartInfo.ClusterSize) / xFile.PartInfo.ClusterSize);
        }

        public void Close()
        {
            try
            {
                IO.Close();
            }
            catch { }
        }
    }
}