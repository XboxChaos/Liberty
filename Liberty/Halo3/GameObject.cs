using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Liberty.Blam;
using Liberty.MathUtil;
using Liberty.SaveIO;

namespace Liberty.Halo3
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

            reader.SeekTo(baseOffset + 0x18);
            _zone = reader.ReadUInt16();

            // Read Position
            reader.SeekTo(baseOffset + PositionOffset1);
            _positionMain.X = reader.ReadFloat();
            _positionMain.Y = reader.ReadFloat();
            _positionMain.Z = reader.ReadFloat();

            // Read Position2
            Vector3 position2 = new Vector3();
            reader.SeekTo(baseOffset + PositionOffset2);
            position2.X = reader.ReadFloat();
            position2.Y = reader.ReadFloat();
            position2.Z = reader.ReadFloat();

            // Read Position3
            Vector3 position3 = new Vector3();
            reader.SeekTo(baseOffset + PositionOffset3);
            position3.X = reader.ReadFloat();
            position3.Y = reader.ReadFloat();
            position3.Z = reader.ReadFloat();

            // Read Position4
            Vector3 position4 = new Vector3();
            reader.SeekTo(baseOffset + PositionOffset4);
            position4.X = reader.ReadFloat();
            position4.Y = reader.ReadFloat();
            position4.Z = reader.ReadFloat();

            // Compute position deltas
            _position2Delta = Vector3.Subtract(position2, _positionMain);
            _position3Delta = Vector3.Subtract(position3, _positionMain);
            _position4Delta = Vector3.Subtract(position4, _positionMain);

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
            writer.SeekTo(chunkStartOffset + 0x18);
            writer.WriteUInt16(_zone);

            // Position1
            writer.SeekTo(chunkStartOffset + PositionOffset1);
            writer.WriteFloat(_positionMain.X);
            writer.WriteFloat(_positionMain.Y);
            writer.WriteFloat(_positionMain.Z);

            // Calculate extra position vectors
            Vector3 position2 = Vector3.Add(_position2Delta, _positionMain);
            Vector3 position3 = Vector3.Add(_position3Delta, _positionMain);
            Vector3 position4 = Vector3.Add(_position4Delta, _positionMain);

            // Position2
            writer.SeekTo(chunkStartOffset + PositionOffset2);
            writer.WriteFloat(position2.X);
            writer.WriteFloat(position2.Y);
            writer.WriteFloat(position2.Z);

            // Position3
            writer.SeekTo(chunkStartOffset + PositionOffset3);
            writer.WriteFloat(position3.X);
            writer.WriteFloat(position3.Y);
            writer.WriteFloat(position3.Z);

            // Position4
            writer.SeekTo(chunkStartOffset + PositionOffset4);
            writer.WriteFloat(position4.X);
            writer.WriteFloat(position4.Y);
            writer.WriteFloat(position4.Z);
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
        public ushort Zone
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
        }

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
        /// The object's position.
        /// </summary>
        public Vector3 Position
        {
            get { return _positionMain; }
            set { _positionMain = value; }
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

        private ushort _zone;

        private Vector3 _positionMain;
        private Vector3 _position2Delta;
        private Vector3 _position3Delta;
        private Vector3 _position4Delta;

        private HealthInfo _healthInfo;
        private const float DefaultChiefHealthModifier = 45;
        private const float DefaultChiefShieldModifier = 70;

        private DatumIndex _nextCarriedIndex;
        private DatumIndex _firstCarriedIndex;
        private DatumIndex _carrierIndex;
        private GameObject _nextCarried = null;
        private GameObject _firstCarried = null;
        private GameObject _carrier = null;

        // Offsets
        private const int PositionOffset1 = 0x54;
        private const int PositionOffset2 = 0x20;
        private const int PositionOffset3 = 0x30;
        private const int PositionOffset4 = 0x40;

        private const int CarryInfoOffset = 0x08;
        private const int StrengthInfoOffset = 0xF0;
    }
}
