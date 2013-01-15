/* Copyright 2012 Aaron Dierking, TJ Tunnell, Jordan Mueller, Alex Reed
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

namespace Liberty.IO
{
    public interface IReader
    {
        void Close();
        bool EOF { get; }
        long Length { get; }
        long Position { get; }
        string ReadAscii();
        string ReadAscii(int size);
        int ReadBlock(byte[] output, int offset, int size);
        byte[] ReadBlock(int size);
        byte ReadByte();
        float ReadFloat();
        short ReadInt16();
        int ReadInt32();
        long ReadInt64();
        sbyte ReadSByte();
        ushort ReadUInt16();
        uint ReadUInt32();
        ulong ReadUInt64();
        string ReadUTF8();
        string ReadUTF8(int size);
        string ReadUTF16();
        string ReadUTF16(int length);
        bool SeekTo(long offset);
        void Skip(long count);
    }
}      
