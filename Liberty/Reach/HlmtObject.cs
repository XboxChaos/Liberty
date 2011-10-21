using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Liberty.Reach
{
    /// <summary>
    /// An object that has HLMT data.
    /// </summary>
    public class HlmtObject : GameObject
    {
        /// <summary>
        /// Constructs a new HLMT object.
        /// </summary>
        /// <param name="reader">The SaveReader to read from.</param>
        /// <param name="entry">The object entry data.</param>
        internal HlmtObject(SaveIO.SaveReader reader, ObjectEntry entry)
            : base(reader, entry)
        {
        }

        /// <summary>
        /// Whether or not this object should be invincible.
        /// </summary>
        public bool Invincible
        {
            get { return _makeInvincible; }
            set { _makeInvincible = value; }
        }

        protected override void DoLoad(SaveIO.SaveReader reader, long start)
        {
            base.DoLoad(reader, start);

            // Check invincibility status
            reader.Seek(start + 0x110, SeekOrigin.Begin);
            _oldHealthModifier = reader.ReadFloat();
            _oldShieldModifier = reader.ReadFloat();
            if (float.IsNaN(_oldHealthModifier) || float.IsNaN(_oldShieldModifier) || (_oldHealthModifier == 0 && _oldShieldModifier == 0))
            {
                _makeInvincible = true;
                _canUseOldValues = false;
            }
        }

        protected override void DoUpdate(SaveIO.SaveWriter writer, long start)
        {
            base.DoUpdate(writer, start);

            // Invincibility
            // TODO: fix hax
            writer.Seek(start + 0x110, SeekOrigin.Begin);
            if (_makeInvincible)
            {
                if (_oldHealthModifier != 0)
                    writer.WriteUInt32(0xFFFFFFFF);
                else
                    writer.WriteUInt32(0x00000000);
                if (_oldShieldModifier != 0)
                    writer.WriteUInt32(0xFFFFFFFF);
            }
            else
            {
                if (_canUseOldValues)
                {
                    // Write the old health and shield values
                    writer.WriteFloat(_oldHealthModifier);
                    writer.WriteFloat(_oldShieldModifier);
                }
                else
                {
                    // TODO: FIX THIS!!!
                    writer.WriteUInt32(0x42340000);
                    writer.WriteUInt32(0x428C0000);
                }
            }
        }

        private bool _makeInvincible = false;
        private bool _canUseOldValues = true;
        private float _oldHealthModifier;
        private float _oldShieldModifier;
    }
}
