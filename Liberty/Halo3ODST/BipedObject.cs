using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Liberty.SaveIO;
using Liberty.Blam;

namespace Liberty.Halo3ODST
{
    public class BipedObject : GameObject
    {
        public BipedObject(ObjectEntry entry, SaveReader reader)
            : base(entry, reader)
        {
            if (entry.TagGroup != Halo3ODST.TagGroup.Bipd)
                throw new ArgumentException("Cannot construct a BipedObject from a non-Bipd object entry");
        }

        protected override void ReadFrom(SaveReader reader, long baseOffset)
        {
            base.ReadFrom(reader, baseOffset);

            reader.SeekTo(baseOffset + WeaponsOffset);
            _primaryWeaponIndex = DatumIndex.ReadFrom(reader);
            _secondaryWeaponIndex = DatumIndex.ReadFrom(reader);
            _tertiaryWeaponIndex = DatumIndex.ReadFrom(reader);
            _quaternaryWeaponIndex = DatumIndex.ReadFrom(reader);

            reader.SeekTo(baseOffset + GrenadesOffset);
            _fragGrenades = reader.ReadSByte();
            _plasmaGrenades = reader.ReadSByte();
            _spikeGrenades = reader.ReadSByte();
            _firebombGrenades = reader.ReadSByte();
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

            // Grenades
            writer.SeekTo(SourceOffset + GrenadesOffset);
            writer.WriteSByte(_fragGrenades);
            writer.WriteSByte(_plasmaGrenades);
            writer.WriteSByte(_spikeGrenades);
            writer.WriteSByte(_firebombGrenades);
        }

        #region Delcarations
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
        /// The biped's tertiary weapon, if it is carrying one.
        /// </summary>
        public WeaponObject TertiaryWeapon
        {
            get { return _tertiaryWeapon; }
        }

        /// <summary>
        /// The biped's quaternary weapon, if it is carrying one.
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

        /// <summary>
        /// The number of spike grenades the biped is carrying.
        /// </summary>
        public sbyte SpikeGrenades
        {
            get { return _spikeGrenades; }
            set { _spikeGrenades = value; }
        }

        /// <summary>
        /// The number of firebomb grenades the biped is carrying.
        /// </summary>
        public sbyte FirebombGrenades
        {
            get { return _firebombGrenades; }
            set { _firebombGrenades = value; }
        }
        #endregion

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
        private sbyte _spikeGrenades;
        private sbyte _firebombGrenades;

        // Offsets
        private const int WeaponsOffset = 0x260;
        private const int GrenadesOffset = 0x29C;
    }
}
