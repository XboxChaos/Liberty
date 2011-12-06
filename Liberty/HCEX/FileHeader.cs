using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Liberty.classInfo;
using Liberty.Security;

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
            CFGText = reader.ReadAscii(_cfgSize - CRC32Size);

            // Parse the CFG data
            CFGData = new SaveCFG(CFGText);
        }

        /// <summary>
        /// Updates any changed header information and resigns the file.
        /// </summary>
        /// <param name="writer">The SaveWriter to write to.</param>
        /// <param name="resignStream">The Stream to read from, used for resigning the file.</param>
        public void Update(SaveIO.SaveWriter writer, Stream resignStream)
        {
            long baseOffset = writer.BaseStream.Position;   // TODO: this is sooo inconsistent with SaveReader, I need to improve upon this...
            WriteMainHeader(writer);
            WriteCFGData(writer);
            Resign(writer, resignStream, baseOffset);
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
            Encoding.ASCII.GetBytes(CFGText, 0, CFGText.Length, cfgData, 0);

            CRC32 crc32 = new CRC32();
            byte[] checksum = crc32.ComputeHash(cfgData);

            // Write it out
            writer.WriteBlock(checksum);
            writer.WriteBlock(cfgData);
        }

        private void Resign(SaveIO.SaveWriter writer, Stream resignStream, long baseOffset)
        {
            long saveCrcOffset = baseOffset + HeaderSize + _cfgSize + _dataBlock1Size + _dataBlock2Size;

            // Resign the main save data
            resignStream.Seek(saveCrcOffset + CRC32Size, SeekOrigin.Begin);
            CRC32 crc32 = new CRC32();
            byte[] checksum = crc32.ComputeHash(resignStream);

            // Write it out
            writer.SeekTo(saveCrcOffset);
            writer.WriteBlock(checksum);
        }

        /// <summary>
        /// The parsed CFG data that appears at the top of the file.
        /// </summary>
        public SaveCFG CFGData { get; private set; }

        /// <summary>
        /// The raw CFG text that appears at the top of the file.
        /// </summary>
        public string CFGText { get; private set; }

        private uint _unknown;
        private int _dataBlock1Size;
        private int _dataBlock2Size;
        private int _saveDataSize;
        private int _cfgSize;

        private const int HeaderSize = 0x18;
        private const int CRC32Size = 4;
    }
}
