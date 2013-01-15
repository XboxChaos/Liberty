﻿/* Copyright 2012 Aaron Dierking, TJ Tunnell, Jordan Mueller, Alex Reed
 * 
 * This file is part of ExtryzeDLL.
 * 
 * Extryze is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Extryze is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with ExtryzeDLL.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Liberty.IO;

namespace Liberty.SaveManager.Flexibility
{
    /// <summary>
    /// Provides a means to read values from an IReader based off of a predefined
    /// structure layout.
    /// </summary>
    public class StructureReader : IStructureLayoutVisitor
    {
        private IReader _reader;                         // The stream to read from
        private StructureLayout _layout;                 // The structure layout to follow
        private long _offset;                            // The offset that the reader is currently at
        private long _baseOffset;                        // The offset that the reader was at when reading began
        private StructureValueCollection _collection;    // The values that have been read so far

        /// <summary>
        /// Reads a structure from a stream by following a predefined structure layout.
        /// </summary>
        /// <param name="reader">The IReader to read the structure from.</param>
        /// <param name="layout">The structure layout to follow.</param>
        /// <returns>A collection of the values that were read.</returns>
        /// <seealso cref="StructureLayout"/>
        public static StructureValueCollection ReadStructure(IReader reader, StructureLayout layout)
        {
            StructureReader structReader = new StructureReader(reader, layout);
            layout.Accept(structReader);
            return structReader._collection;
        }

        /// <summary>
        /// (private) Constructs a new StructureReader.
        /// </summary>
        /// <param name="reader">The IReader to read from.</param>
        /// <param name="layout">The structure layout to follow.</param>
        private StructureReader(IReader reader, StructureLayout layout)
        {
            _reader = reader;
            _layout = layout;
            _baseOffset = reader.Position;
            _offset = _baseOffset;
            _collection = new StructureValueCollection();
        }

        /// <summary>
        /// Reads a basic value from the stream and adds it to the value
        /// collection which is currently being built.
        /// </summary>
        /// <param name="name">The name to store the value under.</param>
        /// <param name="type">The type of the value to read.</param>
        /// <param name="offset">The value's offset (in bytes) from the beginning of the structure.</param>
        /// <seealso cref="IStructureLayoutVisitor.VisitBasicField"/>
        public void VisitBasicField(string name, StructureValueType type, int offset)
        {
            // Seeking is SLOW - only seek if we have to
            if (_offset != _baseOffset + offset)
            {
                _offset = _baseOffset + offset;
                _reader.SeekTo(_offset);
            }

            switch (type)
            {
                case StructureValueType.Byte:
                    _collection.SetNumber(name, _reader.ReadByte());
                    _offset++;
                    break;
                case StructureValueType.SByte:
                    _collection.SetNumber(name, (uint)_reader.ReadSByte());
                    _offset++;
                    break;
                case StructureValueType.UInt16:
                    _collection.SetNumber(name, _reader.ReadUInt16());
                    _offset += 2;
                    break;
                case StructureValueType.Int16:
                    _collection.SetNumber(name, (uint)_reader.ReadInt16());
                    _offset += 2;
                    break;
                case StructureValueType.UInt32:
                    _collection.SetNumber(name, _reader.ReadUInt32());
                    _offset += 4;
                    break;
                case StructureValueType.Int32:
                    _collection.SetNumber(name, (uint)_reader.ReadInt32());
                    _offset += 4;
                    break;
                case StructureValueType.Asciiz:
                    _collection.SetString(name, _reader.ReadAscii());
                    _offset = _reader.Position;
                    break;
                case StructureValueType.Utf16z:
                    _collection.SetString(name, _reader.ReadUTF16());
                    _offset = _reader.Position;
                    break;
            }
        }

        /// <summary>
        /// Reads an array of values from the stream and adds it to the value
        /// collection which is currently being built.
        /// </summary>
        /// <param name="name">The name to store the array under.</param>
        /// <param name="offset">The array's offset (in bytes) from the beginning of the structure.</param>
        /// <param name="count">The number of elements to read into the array.</param>
        /// <param name="entrySize">The size (in bytes) of each element in the array.</param>
        /// <param name="entryLayout">The layout to follow for each entry in the array.</param>
        public void VisitArrayField(string name, int offset, int count, int entrySize, StructureLayout entryLayout)
        {
            StructureValueCollection[] arrayValue = new StructureValueCollection[count];
            for (int i = 0; i < count; i++)
            {
                _reader.SeekTo(_baseOffset + offset + i * entrySize);
                arrayValue[i] = ReadStructure(_reader, entryLayout);
            }
            _collection.SetArray(name, arrayValue);
        }
    }
}
