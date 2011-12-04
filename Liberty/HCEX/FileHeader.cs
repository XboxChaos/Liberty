using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Liberty.classInfo;

namespace Liberty.HCEX
{
    public struct FileHeader
    {
        public void ReadFrom(SaveIO.SaveReader reader)
        {
            // Ignore the CRC32 but read everything else
            reader.Skip(CRC32Size);
            _unknown = reader.ReadUInt32();
            _cfgSize = reader.ReadInt32();
            _dataBlock1Size = reader.ReadInt32();
            _dataBlock2Size = reader.ReadInt32();
            _saveDataSize = reader.ReadInt32();

            // Skip the CFG CRC32 and read the CFG text
            reader.Skip(CRC32Size);
            CFGData = reader.ReadAscii(_cfgSize - CRC32Size);

            // Parse the Lua Datastructure in the header
            string CFGParsed = CFGData.Replace("[","").Replace("]","");
            HaloCFGParse hCFG = new HaloCFGParse();
            hCFG.Parse(CFGParsed);
            //var test = lua.Val["level_id"];
        }

        public void WriteTo(SaveIO.SaveWriter writer)
        {
            WriteMainHeader(writer);
            WriteCFGData(writer);
        }

        private void WriteMainHeader(SaveIO.SaveWriter writer)
        {
            // Write the new header to a MemoryStream for resigning purposes
            MemoryStream tempStream = new MemoryStream(HeaderSize - CRC32Size);
            SaveIO.SaveWriter tempWriter = new SaveIO.SaveWriter(tempStream);
            tempWriter.WriteUInt32(_unknown);
            tempWriter.WriteInt32(_cfgSize);
            tempWriter.WriteInt32(_dataBlock1Size);
            tempWriter.WriteInt32(_dataBlock2Size);
            tempWriter.WriteInt32(_saveDataSize);

            // Grab its CRC32
            CRC32 crc32 = new CRC32();
            byte[] checksum = crc32.ComputeHash(tempStream.GetBuffer());

            // Now write it to the destination stream
            writer.WriteBlock(checksum);
            writer.WriteBlock(tempStream.GetBuffer());
            tempWriter.Close();
        }

        private void WriteCFGData(SaveIO.SaveWriter writer)
        {
            // Get the CRC32 of the data
            byte[] cfgData = new byte[_cfgSize - CRC32Size];
            Encoding.ASCII.GetBytes(CFGData, 0, CFGData.Length, cfgData, 0);

            CRC32 crc32 = new CRC32();
            byte[] checksum = crc32.ComputeHash(cfgData);

            // Write it out
            writer.WriteBlock(checksum);
            writer.WriteBlock(cfgData);
        }

        public string CFGData { get; set; }
        

        private uint _unknown;
        private int _dataBlock1Size;
        private int _dataBlock2Size;
        private int _saveDataSize;
        private int _cfgSize;

        private const int HeaderSize = 0x18;
        private const int CRC32Size = 4;
    }
}
