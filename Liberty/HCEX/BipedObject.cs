using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Liberty.SaveIO;
using Liberty.Blam;
using System.IO;

namespace Liberty.HCEX
{
    public class BipedObject : GameObject
    {
        public BipedObject(ObjectEntry entry, SaveReader reader)
            : base(entry, reader)
        {
            if (entry.TagGroup != HCEX.TagGroup.Bipd)
                throw new ArgumentException("Cannot construct a BipedObject from a non-Bipd object entry");
        }

        protected override void ReadFrom(SaveReader reader, long baseOffset)
        {
            base.ReadFrom(reader, baseOffset);

            reader.SeekTo(baseOffset + HealthModifiersOffset);
            _oldHealthModifier = reader.ReadFloat();
            _oldShieldModifier = reader.ReadFloat();
            if (float.IsNaN(_oldHealthModifier) || float.IsNaN(_oldShieldModifier) || (_oldHealthModifier == 0.0 && _oldShieldModifier == 0.0))
            {
                _makeInvincible = true;
                _canUseOldValues = false;
            }

            reader.SeekTo(baseOffset + WeaponsOffset);
            _primaryWeaponIndex = DatumIndex.ReadFrom(reader);
            _secondaryWeaponIndex = DatumIndex.ReadFrom(reader);
            _tertiaryWeaponIndex = DatumIndex.ReadFrom(reader);
            _quaternaryWeaponIndex = DatumIndex.ReadFrom(reader);

            reader.SeekTo(baseOffset + GrenadesOffset);
            _fragGrenades = reader.ReadSByte();
            _plasmaGrenades = reader.ReadSByte();
        }

        public override void ResolveDatumIndices(IDatumIndexResolver<GameObject> objectResolver)
        {
            base.ResolveDatumIndices(objectResolver);

            _primaryWeapon = objectResolver.ResolveIndex(_primaryWeaponIndex) as WeaponObject;
            _secondaryWeapon = objectResolver.ResolveIndex(_secondaryWeaponIndex) as WeaponObject;
            _tertiaryWeapon = objectResolver.ResolveIndex(_tertiaryWeaponIndex) as WeaponObject;
            _quaternaryWeapon = objectResolver.ResolveIndex(_quaternaryWeaponIndex) as WeaponObject;
        }

        public override void Update(SaveWriter writer)
        {
            base.Update(writer);

            // Invincibility
            // TODO: fix hax
            writer.SeekTo(SourceOffset + HealthModifiersOffset);
            if (_makeInvincible)
            {
                if (_oldHealthModifier != 0.0)
                    writer.WriteFloat(0xFFFFFFFF);
                else
                    writer.WriteFloat(0x00000000);
                if (_oldShieldModifier != 0.0)
                    writer.WriteFloat(0xFFFFFFFF);
                else
                    writer.WriteFloat(0x00000000);
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
                    writer.WriteUInt32(DefaultChiefHealthModifier);
                    writer.WriteUInt32(DefaultChiefShieldModifier);
                }
            }

            // Position
            writer.SeekTo(SourceOffset + PositionOffset1);
            writer.WriteFloat(Position.X);
            writer.WriteFloat(Position.Y);
            writer.WriteFloat(Position.Z);

            writer.Seek(0x38, SeekOrigin.Current);
            writer.WriteFloat(Position.X);
            writer.WriteFloat(Position.Y);
            writer.WriteFloat(Position.Z);

            // Grenades
            writer.SeekTo(SourceOffset + GrenadesOffset);
            writer.WriteSByte(_fragGrenades);
            writer.WriteSByte(_plasmaGrenades);
        }

        /// <summary>
        /// The biped's primary weapon, if it is carrying one.
        /// </summary>
        public WeaponObject PrimaryWeapon
        {
            get { return _primaryWeapon; }
        }

        /// <summary>
        /// The biped's secondary weapon, if it is carrying one.
        /// </summary>
        public WeaponObject SecondaryWeapon
        {
            get { return _secondaryWeapon; }
        }

        /// <summary>
        /// The biped's secondary weapon, if it is carrying one.
        /// </summary>
        public WeaponObject TertiaryWeapon
        {
            get { return _tertiaryWeapon; }
        }

        /// <summary>
        /// The biped's secondary weapon, if it is carrying one.
        /// </summary>
        public WeaponObject QuaternaryWeapon
        {
            get { return _quaternaryWeapon; }
        }

        /// <summary>
        /// The number of frag grenades the biped is carrying.
        /// </summary>
        public sbyte FragGrenades
        {
            get { return _fragGrenades; }
            set { _fragGrenades = value; }
        }

        /// <summary>
        /// The number of plasma grenades the biped is carrying.
        /// </summary>
        public sbyte PlasmaGrenades
        {
            get { return _plasmaGrenades; }
            set { _plasmaGrenades = value; }
        }

        private bool _makeInvincible = false;
        private bool _canUseOldValues = true;
        private float _oldHealthModifier;
        private float _oldShieldModifier;

        private DatumIndex _primaryWeaponIndex;
        private DatumIndex _secondaryWeaponIndex;
        private DatumIndex _tertiaryWeaponIndex;
        private DatumIndex _quaternaryWeaponIndex;
        private WeaponObject _primaryWeapon;
        private WeaponObject _secondaryWeapon;
        private WeaponObject _tertiaryWeapon;
        private WeaponObject _quaternaryWeapon;

        private sbyte _fragGrenades;
        private sbyte _plasmaGrenades;

        // Offsets
        private const int HealthModifiersOffset = 0xD8;
        private const int PositionOffset1 = 0x5C;
        private const int WeaponsOffset = 0x2F8;
        private const int GrenadesOffset = 0x31E;

        // Default modifiers
        private const uint DefaultChiefHealthModifier = 0x42960000;
        private const uint DefaultChiefShieldModifier = 0x42960000;
    }
}
