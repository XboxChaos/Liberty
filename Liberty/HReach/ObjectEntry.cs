/*
* Liberty - http://xboxchaos.com/
*
* Copyright (C) 2011 XboxChaos
* Copyright (C) 2011 ThunderWaffle/AMD
* Copyright (C) 2011 Xeraxic
*
* Liberty is free software; you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published
* by the Free Software Foundation; either version 2 of the License,
* or (at your option) any later version.
*
* Liberty is distributed in the hope that it will be useful, but
* WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
* General Public License for more details.
*
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Specialized;

namespace Liberty.Reach
{
    /// <summary>
    /// Provides constants for known object tag groups.
    /// </summary>
    public enum TagGroup : ushort
    {
        Bipd = 0,
        Vehi = 1,
        Weap = 2,
        Eqip = 3,
        Term = 4,
        Proj = 5,
        Scen = 6,
        Mach = 7,
        Ctrl = 8,
        Ssce = 9,
        Bloc = 10,
        Crea = 11,
        Unk1 = 12,
        Efsc = 13,

        Unknown = 255    // Not actually a valid value, this is just for use as a placeholder.
    }

    /// <summary>
    /// Describes an entry in the "object" chunk.
    /// </summary>
    internal class ObjectEntry
    {
		internal ObjectEntry(Chunk chunk, SaveIO.SaveReader reader, uint datumIndex, ushort flags, long offset)
		{
			_chunk = chunk;
			_id = datumIndex;
            _flags = new BitVector32((int)flags);
			_tagGroup = (TagGroup)(reader.ReadUInt16() >> 8);
			_type = reader.ReadUInt16();
			_poolOffset = reader.ReadUInt32();
			_memAddress = reader.ReadUInt32();
            _offset = offset;
		}

        internal ObjectEntry(Chunk chunk, uint id, ushort flags, TagGroup tagGroup, ushort type, uint poolOffset, uint loadAddress, long writeOffset)
        {
			_chunk = chunk;
            _id = id;
            _flags = new BitVector32((int)flags);
            _tagGroup = tagGroup;
            _type = type;
            _poolOffset = poolOffset;
            _memAddress = loadAddress;
            _offset = writeOffset;
        }

        public void Update(SaveIO.SaveWriter writer)
        {
            writer.Seek(_offset, SeekOrigin.Begin);

            writer.WriteUInt16((ushort)(_id >> 16));
            writer.WriteUInt16((ushort)_flags.Data);
            writer.WriteUInt16((ushort)((ushort)_tagGroup << 8));
            writer.WriteUInt16(_type);
            writer.WriteUInt32(_poolOffset);
            writer.WriteUInt32(_memAddress);
        }

		public void Delete()
		{
			_chunk.DeleteEntry(_id & 0xFFFF);
			_flags = new BitVector32(0x22);
			_memAddress = 0;
		}

        public uint ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public TagGroup TagGroup
        {
            get { return _tagGroup; }
            set { _tagGroup = value; }
        }

        public ushort Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public uint PoolOffset
        {
            get { return _poolOffset; }
            set { _poolOffset = value; }
        }

        public uint LoadAddress
        {
            get { return _memAddress; }
            set { _memAddress = value; }
        }

        public bool Awake
        {
            get { return _flags[EntryFlags.Awake]; }
            set { _flags[EntryFlags.Awake] = value; }
        }

        class EntryFlags
        {
            public const int Awake = 1 << 1;
        }

		private Chunk _chunk;
        private long _offset;
        private uint _id;
        private BitVector32 _flags;
        private TagGroup _tagGroup;
        private ushort _type;
        private uint _poolOffset;
        private uint _memAddress;
    }
}