using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.Reach
{
    public class Actor
    {
        public Actor(SaveIO.SaveReader reader, Squad squad, uint datumIndex, long offset)
        {
            _dataPosition = offset;
            _datumIndex = datumIndex;
            _squad = squad;

            reader.SeekTo(offset + 0x2C);
            _team = reader.ReadByte();

            reader.SeekTo(offset + 0x343);
            _blind = (reader.ReadByte() == 1);
            _deaf = (reader.ReadByte() == 1);
        }

        internal void Update(SaveIO.SaveWriter writer)
        {
            writer.SeekTo(_dataPosition + 0x2C);
            writer.WriteByte(_team);

            writer.SeekTo(_dataPosition + 0x343);
            writer.WriteByte(_blind ? (byte)1 : (byte)0);
            writer.WriteByte(_deaf ? (byte)1 : (byte)0);
        }

        public uint DatumIndex
        {
            get { return _datumIndex; }
        }

        public Squad Squad
        {
            get { return _squad; }
        }

        public byte Team
        {
            get { return _team; }
            set { _team = value; }
        }

        public bool Blind
        {
            get { return _blind; }
            set
            {
                _blind = value;
                if (_squad != null)
                    _squad.Blind = value;
            }
        }

        public bool Deaf
        {
            get { return _deaf; }
            set
            {
                _deaf = value;
                if (_squad != null)
                    _squad.Deaf = value;
            }
        }

        private Squad _squad;
        private long _dataPosition;
        private uint _datumIndex;
        private byte _team;
        private bool _blind;
        private bool _deaf;
    }
}
