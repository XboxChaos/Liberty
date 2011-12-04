using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Liberty.SaveIO;
using Liberty.Blam;

namespace Liberty.HCEX
{
    public class Table
    {
        /// <summary>
        /// A callback used by the ReadEntries method.
        /// </summary>
        /// <param name="table">The Table that the entry belongs to.</param>
        /// <param name="reader">The SaveReader to read from. It will be positioned after the datum index.</param>
        /// <param name="index">The entry's datum index.</param>
        /// <param name="size">The entry's size.</param>
        /// <param name="offset">The entry's start offset.</param>
        /// <returns>true if enumeration should continue.</returns>
        public delegate bool ReadEntriesCallback(Table table, SaveReader reader, DatumIndex index, uint size, long offset);

        /// <summary>
        /// Constructs a new Table and reads the header.
        /// </summary>
        /// <param name="reader">
        /// The SaveReader to read from. It should be positioned at the start of the table header.
        /// When the function finishes, it will point to the first entry in the entry list (see the HeaderSize constant).
        /// </param>
        public Table(SaveIO.SaveReader reader)
        {
            _name = reader.ReadAscii(0x1E);
            reader.Skip(2);  // Index of last entry - 1, not needed
            _entryCount = reader.ReadUInt16();
            _entrySize = reader.ReadUInt16();
            reader.Skip(2 + 2 + 4); // 2 unknown uint16's + uint32 magic number
            _firstDeleted = reader.ReadUInt16();
            _nextFree = reader.ReadUInt16();
            _activeEntries = reader.ReadUInt16();
            _nextSalt = reader.ReadUInt16();
            _memAddress = reader.ReadUInt32();
        }

        /// <summary>
        /// Reads the entry list and calls a callback function for each entry.
        /// </summary>
        /// <param name="reader">The SaveReader to read from. It must be positioned at the start of the entry list.</param>
        /// <param name="callback">The ReadEntriesCallback to call for each entry.</param>
        public void ReadEntries(SaveIO.SaveReader reader, ReadEntriesCallback callback)
        {
            long entryListStart = reader.Position;
            long currentEntryOffset = entryListStart;
            for (ushort i = 0; i < _nextFree; i++)
            {
                reader.SeekTo(currentEntryOffset);
                ushort salt = reader.ReadUInt16();
                DatumIndex index = new DatumIndex(salt, i);
                if (!callback(this, reader, index, _entrySize, currentEntryOffset))
                    break;

                currentEntryOffset += _entrySize;
            }
        }

        /// <summary>
        /// The table's name
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The memory address of the first entry in this table.
        /// </summary>
        public uint Address
        {
            get { return _memAddress; }
        }

        /// <summary>
        /// The size of the table header in bytes.
        /// </summary>
        public const int HeaderSize = 0x38;

        private string _name;
        private ushort _entryCount;
        private ushort _entrySize;
        private ushort _firstDeleted;
        private ushort _nextFree;
        private ushort _activeEntries;
        private ushort _nextSalt;
        private uint _memAddress;
    }
}
