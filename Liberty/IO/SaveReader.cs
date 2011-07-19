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

namespace Liberty.SaveIO
{
    /// <summary>
    /// Reads big-endian data from a stream.
    /// Designed to support the various types that Bungie uses.
    /// </summary>
    public class SaveReader
    {
        /// <summary>
        /// Constructs a new SaveReader object given a base stream.
        /// </summary>
        /// <param name="baseStream">The stream that data should be read from.</param>
        public SaveReader(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Closes this SaveReader and the underlying stream.
        /// </summary>
        public void Close()
        {
            _stream.Close();
        }

        /// <summary>
        /// Reads a byte from the underlying stream and advances its position by 1.
        /// </summary>
        public byte ReadByte()
        {
            return (byte)_stream.ReadByte();
        }

        /// <summary>
        /// Reads a signed byte from the underlying stream and advances its position by 1.
        /// </summary>
        public sbyte ReadSByte()
        {
            return (sbyte)_stream.ReadByte();
        }

        /// <summary>
        /// Returns the next available byte in the underlying stream, or -1 if none
        /// The position will be left unchanged.
        /// </summary>
        public int PeekChar()
        {
            long startPos = _stream.Position;
            int result = _stream.ReadByte();
            _stream.Position = startPos;
            return result;
        }

        /// <summary>
        /// Reads a 16-bit unsigned integer from the underlying stream and advances its position by 2.
        /// </summary>
        public ushort ReadUInt16()
        {
            _stream.Read(_buffer, 0, 2);
            return (ushort)((_buffer[0] << 8) | _buffer[1]);
        }

        /// <summary>
        /// Reads a 16-bit signed integer from the underlying stream and advances its position by 2.
        /// </summary>
        public short ReadInt16()
        {
            _stream.Read(_buffer, 0, 2);
            return (short)((_buffer[0] << 8) | _buffer[1]);
        }

        /// <summary>
        /// Reads a 32-bit unsigned integer from the underlying stream and advances its position by 4.
        /// </summary>
        public uint ReadUInt32()
        {
            _stream.Read(_buffer, 0, 4);
            return (uint)((_buffer[0] << 24) | (_buffer[1] << 16) | (_buffer[2] << 8) | _buffer[3]);
        }

        /// <summary>
        /// Reads a 32-bit signed integer from the underlying stream and advances its position by 4.
        /// </summary>
        public int ReadInt32()
        {
            _stream.Read(_buffer, 0, 4);
            return (int)((_buffer[0] << 24) | (_buffer[1] << 16) | (_buffer[2] << 8) | _buffer[3]);
        }

        /// <summary>
        /// Reads a 64-bit unsigned integer from the underlying stream and advances its position by 8.
        /// </summary>
        /// <returns>The value that was read</returns>
        public ulong ReadUInt64()
        {
            /*_stream.Read(_buffer, 0, 8);
            return (ulong)((_buffer[0] << 56) | (_buffer[1] << 48) | (_buffer[2] << 40) | (_buffer[3] << 32) |
                           (_buffer[4] << 24) | (_buffer[5] << 16) | (_buffer[6] << 8) | _buffer[7]);*/
            ulong upper = (ulong)ReadUInt32();
            ulong lower = (ulong)ReadUInt32();
            return (upper << 32) | lower;
        }

        /// <summary>
        /// Reads a 64-bit signed integer from the underlying stream and advances its position by 8.
        /// </summary>
        /// <returns>The value that was read</returns>
        public long ReadInt64()
        {
            /*_stream.Read(_buffer, 0, 8);
            return (long)((_buffer[0] << 56) | (_buffer[1] << 48) | (_buffer[2] << 40) | (_buffer[3] << 32) |
                          (_buffer[4] << 24) | (_buffer[5] << 16) | (_buffer[6] << 8) | _buffer[7]);*/
            return (long)ReadUInt64();
        }

        /// <summary>
        /// Reads a 32-bit float from the underlying stream and advances its position by 4.
        /// </summary>
        /// <returns>The float value that was read.</returns>
        public float ReadFloat()
        {
            _stream.Read(_buffer, 0, 4);
            if (BitConverter.IsLittleEndian)
            {
                // Is there a faster way to do this?
                byte temp = _buffer[0];
                _buffer[0] = _buffer[3];
                _buffer[3] = temp;
                temp = _buffer[1];
                _buffer[1] = _buffer[2];
                _buffer[2] = temp;
            }
            return BitConverter.ToSingle(_buffer, 0);
        }

        /// <summary>
        /// Changes the position of the underlying stream.
        /// </summary>
        /// <param name="offset">The new offset, relative to the selected origin.</param>
        /// <param name="origin">The origin to seek from.</param>
        public void Seek(long offset, SeekOrigin origin)
        {
            _stream.Seek(offset, origin);
        }

        /// <summary>
        /// Reads a null-terminated ASCII string.
        /// </summary>
        /// <returns>The string that was read.</returns>
        public string ReadAscii()
        {
            string result = "";
            int ch;
            while (true)
            {
                ch = _stream.ReadByte();
                if (ch == 0 || ch == -1)
                    break;
                result += (char)ch;
            }
            return result;
        }

        /// <summary>
        /// Reads a null-terminated ASCII string of a specific size.
        /// The position of the underlying stream will be advanced by the string size.
        /// </summary>
        /// <param name="size">The size of the string to be read.</param>
        /// <returns>The string that was read.</returns>
        public unsafe string ReadAscii(int size)
        {
            sbyte[] chars = new sbyte[size];
            string result;
            fixed (sbyte* str = chars)
            {
                _stream.Read((byte[])(Array)chars, 0, (int)size);
                result = new string(str);
            }
            return result;
        }

        /// <summary>
        /// Reads a null-terminated UTF16-encoded string.
        /// </summary>
        /// <returns>The string that was read.</returns>
        public string ReadUTF16()
        {
            string result = "";
            int ch;
            while (true)
            {
                ch = (_stream.ReadByte() << 8) | _stream.ReadByte();
                if (ch == 0)
                {
                    // TODO: check for -1
                    break;
                }
                result += (char)ch;
            }
            return result;
        }

        /// <summary>
        /// Returns whether or not we are at the end of the stream.
        /// </summary>
        public bool EOF
        {
            get
            {
                return (_stream.Position == _stream.Length);
            }
        }

        /// <summary>
        /// The stream that this SaveReader is based off of.
        /// </summary>
        public Stream BaseStream
        {
            get { return _stream; }
        }

        private Stream _stream = null;
        private byte[] _buffer = new byte[8];
    }
}
