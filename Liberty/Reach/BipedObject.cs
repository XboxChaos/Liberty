using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Liberty.Reach
{
    /// <summary>
    /// A BIPD object.
    /// </summary>
    public class BipedObject : WeaponUser
    {
        /// <summary>
        /// Constructs a new BipedObject.
        /// </summary>
        /// <param name="reader">The SaveReader to read from.</param>
        /// <param name="entry">The object entry data.</param>
        internal BipedObject(Liberty.SaveIO.SaveReader reader, ObjectEntry entry)
            : base(reader, entry)
        {
        }

        /// <summary>
        /// How many frag grenades the biped has.
        /// </summary>
        public sbyte FragGrenades
        {
            get { return _fragGrenades; }
            set { _fragGrenades = value; }
        }

        /// <summary>
        /// How many plasma grenades the biped has.
        /// </summary>
        public sbyte PlasmaGrenades
        {
            get { return _plasmaGrenades; }
            set { _plasmaGrenades = value; }
        }

        /// <summary>
        /// The vehicle that the biped is currently driving, if any.
        /// </summary>
        public VehicleObject Vehicle
        {
            get { return Carrier as VehicleObject; }
        }

        /// <summary>
        /// The armor ability the the biped currently has, if any.
        /// </summary>
        public EquipmentObject ArmorAbility
        {
            get
            {
                return _armorAbility;
            }
            set
            {
                if (_armorAbility != null)
                    _armorAbility.ReplaceWith(value, false);
                else
                    PickUpObject(value);
                _armorAbility = value;
            }
        }

        /// <summary>
        /// The index of the seat that this biped is sitting in.
        /// Can be 0xFFFF if not in a vehicle.
        /// </summary>
        public ushort Seat
        {
            get { return _seatIndex; }
            set { _seatIndex = value; }
        }

        /// <summary>
        /// Whether or not night vision is active.
        /// </summary>
        public bool NightVision
        {
            get
            {
                return ((_bipedFlags & BipedFlags.NightVision) != 0);
            }
            set
            {
                if (value)
                    _bipedFlags |= BipedFlags.NightVision;
                else
                    _bipedFlags &= ~BipedFlags.NightVision;
            }
        }

        /// <summary>
        /// This is a low-level placeholder for when proper actor support is added later.
        /// Set this to 0xFFFFFFFF and assign a player to prevent a biped from being AI-controlled.       
        /// </summary>
        /// <remarks>
        /// This is probably related to AI.
        /// When you change GamePlayer.Biped to point to a biped, it automatically sets this to 0xFFFFFFFF.
        /// </remarks>
        internal uint Actor
        {
            get { return _actorId; }
            set { _actorId = value; }
        }

        /// <summary>
        /// The Player object that this biped is associated with.
        /// </summary>
        /// <remarks>
        /// This isn't read from the file because there's never more than one player.
        /// GamePlayer automatically manages this property instead.
        /// </remarks>
        internal GamePlayer Player
        {
            set { _player = value; }
        }

        protected override void DoLoad(SaveIO.SaveReader reader, long start)
        {
            base.DoLoad(reader, start);

            // Actor ID
            reader.Seek(start + 0x1BC, SeekOrigin.Begin);
            _actorId = reader.ReadUInt32();

            // Biped flags
            reader.Seek(start + 0x1C4, SeekOrigin.Begin);
            _bipedFlags = reader.ReadUInt32();

            // Vehicle seat?
            reader.Seek(start + 0x32E, SeekOrigin.Begin);
            _seatIndex = reader.ReadUInt16();

            // Equipment
            reader.Seek(start + 0x36E, SeekOrigin.Begin);
            _armorAbilityId = reader.ReadUInt16();

            // Grenade counts
            reader.Seek(start + 0x378, SeekOrigin.Begin);
            _fragGrenades = reader.ReadSByte();
            _plasmaGrenades = reader.ReadSByte();

            // Vehicle
            reader.Seek(start + 0xA00, SeekOrigin.Begin);
            _currentVehicleId = reader.ReadUInt32();
            _controlledVehicleId = reader.ReadUInt32();
        }

        protected override void DoUpdate(Liberty.SaveIO.SaveWriter writer, long start)
        {
            base.DoUpdate(writer, start);

            // Actor
            writer.Seek(start + 0x1BC, SeekOrigin.Begin);
            writer.WriteUInt32(_actorId);

            // Biped flags
            writer.Seek(start + 0x1C4, SeekOrigin.Begin);
            writer.WriteUInt32(_bipedFlags);

            // Player
            writer.Seek(start + 0x1CC, SeekOrigin.Begin);
            if (_player != null)
                writer.WriteUInt32(_player.ID);
            else
                writer.WriteUInt32(0xFFFFFFFF);

            // Vehicle seat
            writer.Seek(start + 0x32E, SeekOrigin.Begin);
            writer.WriteUInt16(_seatIndex);

            // Armor ability
            writer.Seek(start + 0x36C, SeekOrigin.Begin);
            if (_armorAbility != null)
                writer.WriteUInt32(_armorAbility.ID);
            else
                writer.WriteUInt32(0xFFFFFFFF);

            // Grenade counts
            writer.Seek(start + 0x378, SeekOrigin.Begin);
            writer.WriteSByte(_fragGrenades);
            writer.WriteSByte(_plasmaGrenades);

            // Vehicle
            writer.Seek(start + 0xA00, SeekOrigin.Begin);
            writer.WriteUInt32(_currentVehicleId);
            writer.WriteUInt32(_controlledVehicleId);

            // Rotation and position
            /*WriteRotationAndPosition(writer, 0xD4C);*/
        }

        /// <summary>
        /// Resolves references to object ID's.
        /// </summary>
        /// <param name="objects">The object list to resolve from.</param>
        internal override void ResolveObjectRefs(List<GameObject> objects)
        {
            base.ResolveObjectRefs(objects);

            if (_armorAbilityId != 0xFFFF)
                _armorAbility = (EquipmentObject)objects[(int)_armorAbilityId];
        }

        public override void Drop()
        {
            if (Carrier != null)
            {
                if (_currentVehicleId == Carrier.ID)
                    _currentVehicleId = 0xFFFFFFFF;
                if (_controlledVehicleId == Carrier.ID)
                    _controlledVehicleId = 0xFFFFFFFF;
                _seatIndex = 0xFFFF;
            }

            base.Drop();
        }

        protected override void OnDropUsedObject(GameObject obj)
        {
            base.OnDropUsedObject(obj);

            if (_armorAbility == obj)
                _armorAbility = null;
        }

        public override void ReplaceWith(GameObject newObj, bool deleteCarried)
        {
            ushort seat = _seatIndex;
            BipedObject newBiped = newObj as BipedObject;
            if (newBiped != null)
            {
                newBiped._currentVehicleId = _currentVehicleId;
                newBiped._controlledVehicleId = _controlledVehicleId;
                newBiped._actorId = _actorId;
                _actorId = 0xFFFFFFFF;
                newBiped._bipedFlags = _bipedFlags;
            }

            base.ReplaceWith(newObj, deleteCarried);

            newBiped._seatIndex = seat;
        }

        protected override void OnReplaceUsedObject(GameObject oldObj, GameObject newObj)
        {
            base.OnReplaceUsedObject(oldObj, newObj);

            if (oldObj != null)
            {
                if (_armorAbility == oldObj)
                    _armorAbility = newObj as EquipmentObject;

                if (newObj != null)
                {
                    if (_currentVehicleId == oldObj.ID)
                        _currentVehicleId = newObj.ID;
                    if (_controlledVehicleId == oldObj.ID)
                        _controlledVehicleId = newObj.ID;
                }
            }
        }

        /// <summary>
        /// Constants for _bipedFlags
        /// </summary>
        class BipedFlags
        {
            public const uint NightVision = 0x1000U;
        }

        private uint _bipedFlags;

        private ushort _seatIndex;

        private ushort _armorAbilityId;
        private EquipmentObject _armorAbility = null;

        private uint _currentVehicleId = 0xFFFFFFFF;
        private uint _controlledVehicleId = 0xFFFFFFFF;

        private uint _actorId;
        private GamePlayer _player = null;

        private sbyte _fragGrenades;
        private sbyte _plasmaGrenades;
    }
}
