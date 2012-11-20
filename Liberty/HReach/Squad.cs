using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Liberty.Reach
{
    public class Squad
    {
        public Squad(SaveIO.SaveReader reader, uint datumIndex, long offset)
        {
            _dataPosition = offset;
            _datumIndex = datumIndex;

            reader.SeekTo(offset + 2);
            _flags = new BitVector32((int)reader.ReadInt16());

            reader.SeekTo(offset + 0xA0);
            _actorIndex = reader.ReadUInt32();

            reader.SeekTo(offset + 0xE7);
            _team = reader.ReadByte();
        }

        internal void Update(SaveIO.SaveWriter writer)
        {
            writer.SeekTo(_dataPosition + 2);
            writer.WriteInt16((short)_flags.Data);

            /*writer.SeekTo(_dataPosition + 0xA0);
            writer.WriteUInt32(_actorIndex);*/

            writer.SeekTo(_dataPosition + 0xE7);
            writer.WriteByte(_team);
        }

        public uint DatumIndex
        {
            get { return _datumIndex; }
        }

        public uint ActorIndex
        {
            get { return _actorIndex; }
        }

        public byte Team
        {
            get { return _team; }
            set { _team = value; }
        }

        public bool Blind
        {
            get { return _flags[SquadFlags.Blind]; }
            set { _flags[SquadFlags.Blind] = value; }
        }

        public bool Deaf
        {
            get { return _flags[SquadFlags.Deaf]; }
            set { _flags[SquadFlags.Deaf] = value; }
        }

        class SquadFlags
        {
            public const int Blind = 1 << 0;
            public const int Deaf = 1 << 1;
        }

        private long _dataPosition;
        private uint _datumIndex;
        private uint _actorIndex;
        private byte _team;
        private BitVector32 _flags;
    }
}
