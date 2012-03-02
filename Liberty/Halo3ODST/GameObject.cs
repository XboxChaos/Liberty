using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Liberty.SaveIO;
using Liberty.Blam;
using Liberty.MathUtil;

namespace Liberty.Halo3ODST
{
    public class GameObject
    {
        /// <summary>
        /// Constructs a new GameObject based off of an ObjectEntry and data read from a SaveReader.
        /// </summary>
        /// <param name="entry">The ObjectEntry that corresponds to this object.</param>
        /// <param name="reader">The SaveRead to read from. It should be positioned at the start of the object data, after the link data.</param>
        public GameObject(ObjectEntry entry, SaveReader reader)
        {
            _entry = entry;

            // Read data
            long baseOffset = reader.Position;
            _streamOffset = baseOffset;
            ReadFrom(reader, baseOffset);
        }

        /// <summary>
        /// Reads data from a SaveReader. Override this in a derived class to read object-specific data.
        /// </summary>
        /// <param name="reader">The SaveReader to read from. No guarantees can be made about its ending offset.</param>
        /// <param name="baseOffset">The offset of the start of the object data.</param>
        protected virtual void ReadFrom(SaveReader reader, long baseOffset)
        {
            _tag = DatumIndex.ReadFrom(reader);

            reader.SeekTo(baseOffset + StrengthInfoOffset);
            _healthInfo = new HealthInfo(reader, DefaultChiefHealthModifier, DefaultChiefShieldModifier, float.PositiveInfinity);

            //reader.SeekTo(baseOffset + 0x18);
            // TODO: get offset for BSP zone
            //_zone = (ushort)((reader.ReadUInt32() & 0xFFFF0000) >> 16);

            // Read PositionMain
            reader.SeekTo(baseOffset + PositionOffset1);
            _positionMain.X = reader.ReadFloat();
            _positionMain.Y = reader.ReadFloat();
            _positionMain.Z = reader.ReadFloat();

            // Read Position2
            reader.SeekTo(baseOffset + PositionOffset2);
            _position2.X = reader.ReadFloat();
            _position2.Y = reader.ReadFloat();
            _position2.Z = reader.ReadFloat();

            // Read Position3
            reader.SeekTo(baseOffset + PositionOffset3);
            _position3.X = reader.ReadFloat();
            _position3.Y = reader.ReadFloat();
            _position3.Z = reader.ReadFloat();

            // Read Position4
            reader.SeekTo(baseOffset + PositionOffset4);
            _position4.X = reader.ReadFloat();
            _position4.Y = reader.ReadFloat();
            _position4.Z = reader.ReadFloat();

            reader.SeekTo(baseOffset + CarryInfoOffset);
            _nextCarriedIndex = DatumIndex.ReadFrom(reader);
            _firstCarriedIndex = DatumIndex.ReadFrom(reader);
            _carrierIndex = DatumIndex.ReadFrom(reader);
        }

        /// <summary>
        /// Changes the object's invincibility status.
        /// If invincibility is disabled, then the health and shield modifiers will be restored to the values they were last set to.
        /// </summary>
        /// <param name="invincible">true if the object should become invincible</param>
        public void MakeInvincible(bool invincible)
        {
            _healthInfo.MakeInvincible(invincible);
        }

        /// <summary>
        /// Resolves any datum indices that this object refers to.
        /// </summary>
        /// <param name="objectResolver">The IDatumIndexResolver to use for resolving GameObjects.</param>
        public virtual void ResolveDatumIndices(IDatumIndexResolver<GameObject> objectResolver)
        {
            _nextCarried = objectResolver.ResolveIndex(_nextCarriedIndex);
            _firstCarried = objectResolver.ResolveIndex(_firstCarriedIndex);
            _carrier = objectResolver.ResolveIndex(_carrierIndex);
        }

        /// <summary>
        /// Updates any changes made to the object data.
        /// </summary>
        /// <param name="reader">
        /// The SaveWriter to write to.
        /// It should point to the same stream that was used to load the object data, as seeking will be done automatically.
        /// </param>
        public virtual void Update(SaveWriter writer)
        {
            // Strength info
            long chunkStartOffset = _entry.ObjectAddress + (long)TableOffset.ObjectPool;

            writer.SeekTo(chunkStartOffset + StrengthInfoOffset);
            _healthInfo.WriteTo(writer);

            // BSP Zone
            //writer.SeekTo(chunkStartOffset + 0x18);
            //writer.WriteUInt16(_zone);

            // Position1
            writer.SeekTo(chunkStartOffset + PositionOffset1);
            writer.WriteFloat(PositionMain.X);
            writer.WriteFloat(PositionMain.Y);
            writer.WriteFloat(PositionMain.Z);

            // Position2
            writer.SeekTo(chunkStartOffset + PositionOffset2);
            writer.WriteFloat(Position2.X);
            writer.WriteFloat(Position2.Y);
            writer.WriteFloat(Position2.Z);

            // Position3
            writer.SeekTo(chunkStartOffset + PositionOffset3);
            writer.WriteFloat(Position3.X);
            writer.WriteFloat(Position3.Y);
            writer.WriteFloat(Position3.Z);

            // Position4
            writer.SeekTo(chunkStartOffset + PositionOffset4);
            writer.WriteFloat(Position4.X);
            writer.WriteFloat(Position4.Y);
            writer.WriteFloat(Position4.Z);
        }

        #region Declarations
        /// <summary>
        /// The object's offset in the source stream.
        /// </summary>
        public long SourceOffset
        {
            get { return _streamOffset; }
        }

        /// <summary>
        /// The zone that the object is currently in.
        /// </summary>
        /*public ushort Zone
        {
            get
            {
                GameObject obj = _carrier;
                if (obj == null)
                    return _zone;
                while (obj._carrier != null)
                    obj = obj._carrier;
                return obj._zone;
            }
        }*/

        /// <summary>
        /// The object's datum index.
        /// </summary>
        public DatumIndex Index
        {
            get { return _entry.Index; }
        }

        /// <summary>
        /// The object's tag ID.
        /// </summary>
        public DatumIndex Tag
        {
            get { return _tag; }
        }

        /// <summary>
        /// The object's tag group.
        /// </summary>
        public TagGroup TagGroup
        {
            get { return _entry.TagGroup; }
        }

        /// <summary>
        /// The object's position (Main).
        /// </summary>
        public Vector3 PositionMain
        {
            get { return _positionMain; }
            set { _positionMain = value; }
        }

        /// <summary>
        /// The object's position (Main).
        /// </summary>
        public Vector3 Position2
        {
            get { return _position2; }
            set { _position2 = value; }
        }

        /// <summary>
        /// The object's position (Main).
        /// </summary>
        public Vector3 Position3
        {
            get { return _position3; }
            set { _position3 = value; }
        }

        /// <summary>
        /// The object's position (Main).
        /// </summary>
        public Vector3 Position4
        {
            get { return _position4; }
            set { _position4 = value; }
        }

        /// <summary>
        /// The next object that the carrier (if any) is carrying.
        /// Can be null if nothing is carrying this or if this is the last object that the carrier is carrying.
        /// </summary>
        public GameObject NextCarried
        {
            get { return _nextCarried; }
        }

        /// <summary>
        /// The first object that this object is carrying.
        /// Can be null if this object is not carrying anything.
        /// </summary>
        public GameObject FirstCarried
        {
            get { return _firstCarried; }
        }

        /// <summary>
        /// The object that is carrying this object.
        /// Can be null if nothing is carrying this.
        /// </summary>
        public GameObject Carrier
        {
            get { return _carrier; }
        }

        /// <summary>
        /// Whether or not the object is invincible.
        /// </summary>
        public bool Invincible
        {
            get { return _healthInfo.IsInvincible; }
        }

        /// <summary>
        /// Whether or not the object supports health information.
        /// </summary>
        public bool HasHealth
        {
            get { return _healthInfo.HasHealth; }
        }

        /// <summary>
        /// Whether or not the object supports shields information.
        /// </summary>
        public bool HasShields
        {
            get { return _healthInfo.HasShields; }
        }

        public float HealthModifier
        {
            get { return _healthInfo.HealthModifier; }
            set { _healthInfo.HealthModifier = value; }
        }

        public float ShieldModifier
        {
            get { return _healthInfo.ShieldModifier; }
            set { _healthInfo.ShieldModifier = value; }
        }
        #endregion

        private long _streamOffset;
        private ObjectEntry _entry;
        private DatumIndex _tag;

        //private ushort _zone;

        private Vector3 _positionMain;
        private Vector3 _position2;
        private Vector3 _position3;
        private Vector3 _position4;

        private HealthInfo _healthInfo;
        private const float DefaultChiefHealthModifier = 80;
        private const float DefaultChiefShieldModifier = 80;

        private DatumIndex _nextCarriedIndex;
        private DatumIndex _firstCarriedIndex;
        private DatumIndex _carrierIndex;
        private GameObject _nextCarried = null;
        private GameObject _firstCarried = null;
        private GameObject _carrier = null;

        // Offsets
        private const int PositionOffset1 = 0x50;
        private const int PositionOffset2 = 0x2C;
        private const int PositionOffset3 = 0x3C;
        private const int PositionOffset4 = 0x1C;
        private const int CarryInfoOffset = 0x08;
        private const int StrengthInfoOffset = 0xEC;
    }
}
