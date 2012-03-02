using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Liberty.Blam;
using Liberty.MathUtil;
using Liberty.SaveIO;

namespace Liberty.HCEX
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
        /// Reads data from a SaveReader. Override this in a derived class to read object-specific data.
        /// </summary>
        /// <param name="reader">The SaveReader to read from. No guarantees can be made about its ending offset.</param>
        /// <param name="baseOffset">The offset of the start of the object data.</param>
        protected virtual void ReadFrom(SaveReader reader, long baseOffset)
        {
            _tag = DatumIndex.ReadFrom(reader);

            reader.SeekTo(baseOffset + StrengthInfoOffset);
            // Using the default Master Chief values is kind of hackish,
            // but at the same time it would be overkill to have a huge DB of original health/shield values.
            _healthInfo = new HealthInfo(reader, DefaultChiefHealthModifier, DefaultChiefShieldModifier);

            reader.SeekTo(baseOffset + PositionOffset1);
            _position.X = reader.ReadFloat();
            _position.Y = reader.ReadFloat();
            _position.Z = reader.ReadFloat();

            reader.SeekTo(baseOffset + PositionOffset2);
            _unkPosition.X = reader.ReadFloat();
            _unkPosition.Y = reader.ReadFloat();
            _unkPosition.Z = reader.ReadFloat();
            _unkPosition = Vector3.Subtract(_unkPosition, _position);

            reader.SeekTo(baseOffset + CarryInfoOffset);
            _nextCarriedIndex = DatumIndex.ReadFrom(reader);
            _firstCarriedIndex = DatumIndex.ReadFrom(reader);
            _carrierIndex = DatumIndex.ReadFrom(reader);
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
            writer.SeekTo(SourceOffset + StrengthInfoOffset);
            _healthInfo.WriteTo(writer);

            // Position
            writer.SeekTo(SourceOffset + PositionOffset1);
            writer.WriteFloat(Position.X);
            writer.WriteFloat(Position.Y);
            writer.WriteFloat(Position.Z);

            Vector3 position2 = Vector3.Add(_unkPosition, _position);
            writer.SeekTo(SourceOffset + PositionOffset2);
            writer.WriteFloat(position2.X);
            writer.WriteFloat(position2.Y);
            writer.WriteFloat(position2.Z);
        }

        /// <summary>
        /// The object's offset in the source stream.
        /// </summary>
        public long SourceOffset
        {
            get { return _streamOffset; }
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
            get { return _position; }
            set { _position = value; }
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

        private long _streamOffset;
        private ObjectEntry _entry;
        private DatumIndex _tag;

        private Vector3 _position;
        private Vector3 _unkPosition;   // Not entirely sure what this does, it's close to the position but isn't exactly it

        private HealthInfo _healthInfo;
        private const float DefaultChiefHealthModifier = 75;
        private const float DefaultChiefShieldModifier = 75;

        private DatumIndex _nextCarriedIndex;
        private DatumIndex _firstCarriedIndex;
        private DatumIndex _carrierIndex;
        private GameObject _nextCarried = null;
        private GameObject _firstCarried = null;
        private GameObject _carrier = null;

        // Offsets
        private const int PositionOffset1 = 0x5C;
        private const int PositionOffset2 = 0xA0;
        private const int CarryInfoOffset = 0x114;
        private const int StrengthInfoOffset = 0xD8;
    }
}
