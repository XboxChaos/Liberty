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

namespace Liberty.Reach
{
    /// <summary>
    /// Describes chunk data in an mmiof.bmf file.
    /// </summary>
    /// <seealso cref="Process"/>
    public class Chunk
    {
        /// <summary>
        /// A callback used by the EnumEntries method.
        /// </summary>
        /// <param name="reader">The SaveReader to read from. It will be positioned after the datum index.</param>
        /// <param name="active">Whether or not this entry is active.</param>
        /// <param name="datumIndex">The entry's datum index.</param>
        /// <param name="flags">The entry's flags.</param>
        /// <param name="size">The entry's size.</param>
        /// <param name="offset">The entry's start offset.</param>
        /// <returns>true if enumeration should continue.</returns>
        public delegate bool EnumEntriesCallback(SaveIO.SaveReader reader, bool active, uint datumIndex, ushort flags, uint size, long offset);

        /// <summary>
        /// Constructs a new Chunk object, reading header data out of a SaveReader.
        /// </summary>
        /// <param name="reader">The SaveReader to read from.</param>
        public Chunk(SaveIO.SaveReader reader, ChunkOffset offset)
        {
            long baseAddress = (long)offset;
            reader.Seek(baseAddress, SeekOrigin.Begin);

            // Read header data
            _name = reader.ReadAscii(32);
            _entrySize = reader.ReadUInt32();
            reader.Seek(baseAddress + 0x28, SeekOrigin.Begin);
            _entryCount = reader.ReadUInt32();
            reader.Seek(baseAddress + 0x34, SeekOrigin.Begin);
            _firstDeleted = reader.ReadUInt32();
            _nextFree = reader.ReadUInt32();
            _activeEntries = reader.ReadUInt32();
            reader.Seek(baseAddress + 0x50, SeekOrigin.Begin);
            int entryListSize = reader.ReadInt32() - 0x54;
            _entryListStart = baseAddress + 0x54;
        }

        /// <summary>
        /// Updates any changes made to this chunk.
        /// </summary>
        /// <param name="writer">The SaveWriter to write to.</param>
        public void Update(SaveIO.SaveWriter writer)
        {
            long baseAddress = _entryListStart - 0x54;

            writer.Seek(baseAddress + 0x34, SeekOrigin.Begin);
            writer.WriteUInt32(_firstDeleted);
            writer.WriteUInt32(_nextFree);
            writer.WriteUInt32(_activeEntries);

			// Null out deleted datum indices by setting their high word to 0
			foreach (uint index in _deletedEntries)
			{
				writer.Seek(baseAddress + 0x54 + index * _entrySize, SeekOrigin.Begin);
				writer.WriteUInt16(0);
			}
        }

        /// <summary>
        /// Enumerates through the entry list and fires a callback for each entry.
        /// When the callback is fired, the SaveReader will be positioned 4 bytes from the start of the entry.
        /// </summary>
        /// <param name="reader">The SaveReader to read from</param>
        /// <param name="callback">The callback to call for each entry</param>
        public void EnumEntries(SaveIO.SaveReader reader, EnumEntriesCallback callback)
        {
            for (uint i = 0; i < _nextFree; i++)
            {
                long offset = _entryListStart + i * _entrySize;
                reader.Seek(offset, SeekOrigin.Begin);

                ushort salt = reader.ReadUInt16();
                ushort flags = reader.ReadUInt16();
                uint datumIndex = ((uint)salt << 16) | i;
                bool active = (salt != 0);
                if (!callback(reader, active, datumIndex, flags, _entrySize, offset))
                    break;
            }
        }

		/// <summary>
		/// Deletes an entry in this chunk.
		/// </summary>
		/// <param name="index">The zero-based index of the entry to delete.</param>
		public void DeleteEntry(uint index)
		{
			_deletedEntries.Add(index);
			if (index < _firstDeleted)
				_firstDeleted = index;
			if (index == _nextFree - 1)
				_nextFree = index;
			_activeEntries--;
		}

        /// <summary>
        /// The name of the chunk.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The size of each entry.
        /// </summary>
        public uint EntrySize
        {
            get { return _entrySize; }
        }

        private string _name;
        private long _entryListStart;
        private uint _entryCount;
        private uint _entrySize;
        private uint _firstDeleted;
        private uint _nextFree;
        private uint _activeEntries;
        private List<uint> _deletedEntries = new List<uint>();
    }
}
