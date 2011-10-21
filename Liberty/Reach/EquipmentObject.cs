using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Liberty.Reach
{
    /// <summary>
    /// An equipment object, such as an armor ability.
    /// </summary>
    public class EquipmentObject : GameObject
    {
        /// <summary>
        /// Constructs a new EquipmentObject.
        /// </summary>
        /// <param name="reader">The SaveReader to read from.</param>
        /// <param name="entry">The object entry data.</param>
        internal EquipmentObject(Liberty.SaveIO.SaveReader reader, ObjectEntry entry)
            : base(reader, entry)
        {
        }

        /// <summary>
        /// The Player object that this equipment object is associated with.
        /// </summary>
        /// <remarks>
        /// This isn't read from the file because there's never more than one player.
        /// GamePlayer automatically manages this property instead.
        /// </remarks>
        internal GamePlayer Player
        {
            set { _player = value; }
        }

        protected override void DoUpdate(SaveIO.SaveWriter writer, long start)
        {
            base.DoUpdate(writer, start);

            // Player data
            writer.Seek(start + 0xF0, SeekOrigin.Begin);
            if (_player != null)
                writer.WriteUInt32(_player.ID);
            else
                writer.WriteUInt32(0xFFFFFFFF);
            writer.Seek(start + 0x1B4, SeekOrigin.Begin);
            if (Carrier != null && Carrier.TagGroup == TagGroup.Bipd)
            {
                writer.WriteUInt32(Carrier.ID);
                writer.WriteUInt32(Carrier.ID);
            }
            else
            {
                writer.WriteUInt32(0xFFFFFFFF);
                writer.WriteUInt32(0xFFFFFFFF);
            }

            // This ID
            writer.WriteUInt32(ID);
        }

        GamePlayer _player = null;
    }
}
