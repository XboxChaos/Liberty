using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Liberty.HCEX
{
    public struct SaveHeader
    {
        public void ReadFrom(SaveIO.SaveReader reader)
        {
            long offset = reader.Position;

            string saveType = reader.ReadAscii(0x14);
            if (saveType != "non compressed save")
                throw new ArgumentException("Invalid save header - expected a non-compressed HCEX save");
            if (reader.ReadUInt32() != Magic1)
                throw new ArgumentException("Invalid save header - bad magic number 1 (expected 0x92F7E104)");
            _map = reader.ReadAscii();

            reader.SeekTo(offset + 0x1F8);
            if (reader.ReadUInt32() != Magic2)
                throw new ArgumentException("Invalid save header - bad magic number 2 (expected 0xDEADBEEF)");
        }

        public string Map
        {
            get { return _map; }
        }

        private string _map;

        private const uint Magic1 = 0x92F7E104;
        private const uint Magic2 = 0xDEADBEEF;
    }
}
