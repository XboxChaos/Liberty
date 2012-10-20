using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Liberty.Blam;
using Liberty.SaveIO;

namespace Liberty.Halo4
{
    public class ObjectEntry
    {
        public ObjectEntry(DatumIndex index, SaveReader reader)
        {
            _index = index;
            _flags = reader.ReadInt16();
            _tagGroup = (TagGroup)reader.ReadByte();
            reader.Skip(0x03);
            _chunkOffset = reader.ReadUInt32();
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
        public Int16 Flags
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
        /// The object's file offset.
        /// </summary>
        public uint FileOffset
        {
            get { return _chunkOffset; }
        }

        /// <summary>
        /// The object's memory address.
        /// </summary>
        public uint ObjectAddress
        {
            get { return _chunkAddress; }
        }

        private DatumIndex _index;
        private Int16 _flags;
        private TagGroup _tagGroup;
        private uint _chunkOffset;
        private uint _chunkAddress;
    }
}
