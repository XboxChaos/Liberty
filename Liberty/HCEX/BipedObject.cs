using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Liberty.SaveIO;
using Liberty.Blam;

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

            reader.SeekTo(baseOffset + HealthModifierOffset);
            _healthModifier = reader.ReadFloat();
            _shieldModifier = reader.ReadFloat();

            reader.SeekTo(baseOffset + WeaponsOffset);
            _primaryWeaponIndex = DatumIndex.ReadFrom(reader);
            _secondaryWeaponIndex = DatumIndex.ReadFrom(reader);

            reader.SeekTo(baseOffset + GrenadesOffset);
            _fragGrenades = reader.ReadSByte();
            _plasmaGrenades = reader.ReadSByte();
        }

        public override void ResolveDatumIndices(IDatumIndexResolver<GameObject> objectResolver)
        {
            base.ResolveDatumIndices(objectResolver);

            _primaryWeapon = objectResolver.ResolveIndex(_primaryWeaponIndex) as WeaponObject;
            _secondaryWeapon = objectResolver.ResolveIndex(_secondaryWeaponIndex) as WeaponObject;
        }

        /// <summary>
        /// The biped's health modifier. Set to NaN for invincibility.
        /// </summary>
        public float HealthModifier
        {
            get { return _healthModifier; }
            set { _healthModifier = value; }
        }

        /// <summary>
        /// The biped's shield modifier. Set to NaN for invincibility.
        /// </summary>
        public float ShieldModifier
        {
            get { return _shieldModifier; }
            set { _shieldModifier = value; }
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

        private float _healthModifier;
        private float _shieldModifier;

        private DatumIndex _primaryWeaponIndex;
        private DatumIndex _secondaryWeaponIndex;
        private WeaponObject _primaryWeapon;
        private WeaponObject _secondaryWeapon;

        private sbyte _fragGrenades;
        private sbyte _plasmaGrenades;

        // Offsets
        private const int HealthModifierOffset = 0xD8;
        private const int WeaponsOffset = 0x2F8;
        private const int GrenadesOffset = 0x31E;
    }
}
