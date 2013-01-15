﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Liberty.IO;

namespace Liberty.SaveManager.Games.Halo3
{
    public class WeaponObject : GameObject, Blam.IWeapon
    {
        public WeaponObject(ObjectEntry entry, EndianReader reader)
            : base(entry, reader)
        {
            if (entry.TagGroup != Halo3.TagGroup.Weap)
                throw new ArgumentException("Cannot construct a WeaponObject from a non-Weap object entry");
        }

        protected override void ReadFrom(EndianReader reader, long baseOffset)
        {
            base.ReadFrom(reader, baseOffset);

            reader.SeekTo(baseOffset + AmmoOffset);
            _remainingAmmo = reader.ReadInt16();
            reader.Skip(2);
            _clipAmmo = reader.ReadInt16();
        }

        public override void Update(EndianWriter writer)
        {
            base.Update(writer);

            writer.SeekTo(SourceOffset + AmmoOffset);
            writer.WriteInt16(_remainingAmmo);
            writer.Skip(2);
            writer.WriteInt16(_clipAmmo);
        }

        #region Declarations
        /// <summary>
        /// How much unloaded ammo is left in the weapon.
        /// </summary>
        public short Ammo
        {
            get { return _remainingAmmo; }
            set { _remainingAmmo = value; }
        }

        /// <summary>
        /// How much ammo is left in the weapon's clip.
        /// </summary>
        public short ClipAmmo
        {
            get { return _clipAmmo; }
            set { _clipAmmo = value; }
        }

        /// <summary>
        /// Not implemented yet.
        /// </summary>
        public float Energy
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        private short _remainingAmmo;
        private short _clipAmmo;

        // Offsets
        private const int AmmoOffset = 0x23A;
    }
}
