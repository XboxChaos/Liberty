using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Liberty.Blam;
using Liberty.SaveIO;

namespace Liberty.Halo3
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
        public Table(SaveReader reader)
        {
            _name = reader.ReadAscii(0x1E);
            _entryCount = reader.ReadUInt32();
            _entrySize = reader.ReadUInt32();
            reader.Skip(0x28);
            _tableSizeWHeader = reader.ReadUInt32();
            _tmpTableStartOffset = reader.Position;
        }

        public void ReadEntries(SaveReader reader, ReadEntriesCallback callback)
        {
            long entryListStart = reader.Position;
            long currentEntryOffset = entryListStart;
            for (ushort i = 0; i < _entryCount; i++)
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

        public long TableStartOffset
        {
            get { return _tmpTableStartOffset; }
        }


        /// <summary>
        /// The size of the table header in bytes
        /// </summary>
        public const int HeaderSize = 0x54;

        private string _name;
        private UInt32 _entryCount;
        private UInt32 _entrySize;
        private UInt32 _tableSizeWHeader;
        //private ushort _firstDeleted;
        //private ushort _nextFree;
        //private ushort _activeEntries;
        //private ushort _nextSalt;
        //private uint _memAddress;
        private long _tmpTableStartOffset;
    }
}
