using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Liberty.Blam;
using Liberty.SaveIO;

namespace Liberty.Halo3
{
    public class ObjectEntry
    {
        public ObjectEntry(DatumIndex index, SaveReader reader)
        {
            _index = index;
            //_flags = reader.ReadByte();
            reader.Skip(1);
            _tagGroup = (TagGroup)reader.ReadByte();
            reader.Skip(4);
            _chunkAddress = reader.ReadUInt32();
        }

        /// <summary>
        /// The entry's datum index.
        /// </summary>
        public DatumIndex Index
        {
            get { return _index; }
        }

        /// <summary>
        /// Object flags
        /// </summary>
        public byte Flags
        {
            get { return _flags; }
        }

        /// <summary>
        /// The object's tag group.
        /// </summary>
        public TagGroup TagGroup
        {
            get { return _tagGroup; }
        }

        /// <summary>
        /// The size of the object's data, excluding link information.
        /// </summary>
        public ushort ObjectSize
        {
            get { return _chunkSize; }
        }

        /// <summary>
        /// The object's memory address.
        /// </summary>
        public uint ObjectAddress
        {
            get { return _chunkAddress; }
        }

        private DatumIndex _index;
        private byte _flags;
        private TagGroup _tagGroup;
        private ushort _chunkSize;
        private uint _chunkAddress;
    }
}
