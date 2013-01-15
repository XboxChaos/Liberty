using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Liberty.IO;

namespace Liberty.SaveManager.Games.Blam
{
    /// <summary>
    /// An immutable struct representing a datum index.
    /// </summary>
    public struct DatumIndex
    {
        public DatumIndex(uint value)
        {
            Salt = (ushort)((value >> 16) & 0xFFFF);
            Index = (ushort)(value & 0xFFFF);
        }

        public DatumIndex(ushort salt, ushort index)
        {
            Salt = salt;
            Index = index;
        }

        /// <summary>
        /// Reads a DatumIndex from a EndianReader and returns it.
        /// </summary>
        /// <param name="reader">The EndianReader to read from.</param>
        /// <returns>The DatumIndex that was read.</returns>
        public static DatumIndex ReadFrom(EndianReader reader)
        {
            return new DatumIndex(reader.ReadUInt32());
        }

        /// <summary>
        /// Writes the DatumIndex out to a EndianWriter.
        /// </summary>
        /// <param name="writer">The EndianWriter to write to.</param>
        public void WriteTo(EndianWriter writer)
        {
            writer.WriteUInt32(Value);
        }

        public override bool Equals(object obj)
        {
            return (obj is DatumIndex) && (this == (DatumIndex)obj);
        }

        public override int GetHashCode()
        {
            return (int)Value;
        }

        public static bool operator ==(DatumIndex x, DatumIndex y)
        {
            return (x.Salt == y.Salt && x.Index == y.Index);
        }

        public static bool operator !=(DatumIndex x, DatumIndex y)
        {
            return !(x == y);
        }

        public override string ToString()
        {
            return "0x" + Value.ToString("X");
        }

        /// <summary>
        /// The salt part of the datum index.
        /// </summary>
        public readonly ushort Salt;

        /// <summary>
        /// The index part of the datum index.
        /// </summary>
        public readonly ushort Index;

        /// <summary>
        /// The datum index as a 32-bit unsigned value.
        /// </summary>
        public uint Value
        {
            get { return (uint)((Salt << 16) | Index); }
        }

        /// <summary>
        /// Whether or not this datum index points to valid data.
        /// </summary>
        public bool IsValid
        {
            get { return (Salt != 0 && Index != 0xFFFF); }
        }
    }
}
