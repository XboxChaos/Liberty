using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Specialized;

namespace Liberty.Reach
{
    public class UnitObject : GameObject
    {
        /// <summary>
        /// Constructs a new UnitObject.
        /// </summary>
        /// <param name="reader">The SaveReader to read from.</param>
        /// <param name="entry">The object entry data.</param>
        internal UnitObject(Liberty.SaveIO.SaveReader reader, ObjectEntry entry)
            : base(reader, entry)
        {
            _roWeapons = _weapons.AsReadOnly();
        }

        /// <summary>
        /// Changes one of the weapons that this object is holding.
        /// </summary>
        /// <remarks>
        /// If index is larger than the current weapon count, the weapon will be placed into the first free slot.
        /// </remarks>
        /// <param name="index">The zero-based weapon index (0 = 1st weapon, 3 = 4th weapon)</param>
        /// <param name="newWeapon">The new weapon, or null for none.</param>
        public void ChangeWeapon(int index, WeaponObject newWeapon)
        {
            if (index < 0 || index >= 4)
                throw new IndexOutOfRangeException("Trying to change a non-existant weapon slot");
            
            if (index >= _weapons.Count || _backupWeaponIndex < 0)
            {
                // No slot available - just pick it up
                if (newWeapon != null)
                    PickUpWeapon(newWeapon);
                return;
            }

            if (newWeapon != null)
            {
                int listIndex = TranslateIndex(index);
                _weapons[listIndex].ReplaceWith(newWeapon);
                _weapons[listIndex] = newWeapon;
            }
            else
            {
                DropWeapon(index);
            }
        }

        /// <summary>
        /// Returns one of the weapons that this object is holding.
        /// </summary>
        /// <param name="index">The zero-based weapon index (0 = 1st weapon, 3 = 4th weapon)</param>
        /// <returns>The weapon at the specified index. Can be null if no weapon is in the specified slot.</returns>
        public WeaponObject GetWeapon(int index)
        {
            if (index >= 0 && index < _weapons.Count && _backupWeaponIndex >= 0)
                return _weapons[TranslateIndex(index)];
            else
                return null;
        }

        public void DropWeapon(int index)
        {
            if (index >= 0 && index < _weapons.Count && _backupWeaponIndex >= 0)
            {
                WeaponObject weapon = _weapons[TranslateIndex(index)];
                weapon.Drop();
            }
        }

        /// <summary>
        /// Picks up a weapon and places it into an empty slot, if available.
        /// </summary>
        /// <param name="weapon">The weapon to pick up.</param>
        /// <returns>true if the weapon was picked up successfully.</returns>
        public bool PickUpWeapon(WeaponObject weapon)
        {
            if (weapon != null && _weapons.Count < 4)
            {
                PickUp(weapon);
                if (_backupWeaponIndex >= 0)
                    _weapons.Insert(_backupWeaponIndex, weapon);
                else
                    _weapons.Add(weapon);

                _backupWeaponIndex++;
                if (Carrier == null || Carrier.TagGroup != TagGroup.Vehi)
                    _currentWeaponIndex++;

                return true;
            }

            return false;
        }

        public void TransferWeapons(UnitObject receiver)
        {
            while (receiver._weapons.Count > 0)
                receiver.DropWeapon(0);

            while (_weapons.Count > 0)
                receiver.PickUpWeapon(GetWeapon(0));

            _weapons.Clear();
            _backupWeaponIndex = -1;
            if (Carrier == null || Carrier.TagGroup != TagGroup.Vehi)
                _currentWeaponIndex = -1;
        }

        /// <summary>
        /// The first weapon that this UnitObject is holding. Can be null.
        /// </summary>
        public WeaponObject PrimaryWeapon
        {
            get { return GetWeapon(0); }
            set { ChangeWeapon(0, value); }
        }

        /// <summary>
        /// The second weapon that this UnitObject is holding. Can be null.
        /// </summary>
        public WeaponObject SecondaryWeapon
        {
            get { return GetWeapon(1); }
            set { ChangeWeapon(1, value); }
        }

        /// <summary>
        /// The third weapon that this UnitObject is holding. Can be null.
        /// </summary>
        public WeaponObject TertiaryWeapon
        {
            get { return GetWeapon(2); }
            set { ChangeWeapon(2, value); }
        }
        
        /// <summary>
        /// The fourth weapon that this UnitObject is holding. Can be null.
        /// </summary>
        public WeaponObject QuaternaryWeapon
        {
            get { return GetWeapon(3); }
            set { ChangeWeapon(3, value); }
        }

        public IList<WeaponObject> Weapons
        {
            get { return _roWeapons; }
        }

        public Actor Actor
        {
            get { return _actor; }
            set { _actor = value; }
        }

        /// <summary>
        /// The unit's team.
        /// </summary>
        public byte Team
        {
            get { return _team; }
            set
            {
                _team = value;
                if (_actor != null)
                {
                    // Need to change the actor and squad team index as well
                    _actor.Team = value;
                    if (_actor.Squad != null)
                        _actor.Squad.Team = value;
                }
            }
        }

        public bool NightVision
        {
            get { return _unitFlags[UnitFlags.NightVision]; }
            set { _unitFlags[UnitFlags.NightVision] = value; }
        }

        public bool NoFallDamage
        {
            get { return _unitFlags[UnitFlags.NoFallDamage]; }
            set { _unitFlags[UnitFlags.NoFallDamage] = value; }
        }

        public bool PlayerCantEnter
        {
            get { return _unitFlags[UnitFlags.PlayerCantEnter]; }
            set { _unitFlags[UnitFlags.PlayerCantEnter] = value; }
        }

        public override void MakeInvincible(bool invincible)
        {
            base.MakeInvincible(invincible);

            NoFallDamage = invincible;
        }

        /*public override void PickUp(GameObject obj)
        {
            base.PickUp(obj);

            WeaponObject weapon = obj as WeaponObject;
            if (weapon != null && _weapons.Count < 4)
                _weapons.Add(weapon);
        }*/

        protected override void DoLoad(SaveIO.SaveReader reader, long start)
        {
            base.DoLoad(reader, start);

            reader.Seek(start + 0x1BE, SeekOrigin.Begin);
            _actorId = reader.ReadUInt16();

            reader.Seek(start + 0x1C4, SeekOrigin.Begin);
            _unitFlags = new BitVector32(reader.ReadInt32());

            reader.Seek(start + 0x1C8, SeekOrigin.Begin);
            _team = reader.ReadByte();

            reader.Seek(start + 0x342, SeekOrigin.Begin);
            _currentWeaponIndex = reader.ReadSByte();
            reader.Seek(start + 0x346, SeekOrigin.Begin);
            _backupWeaponIndex = reader.ReadSByte();

            reader.Seek(start + 0x348, SeekOrigin.Begin);
            for (int i = 0; i < 4; i++)
                _weaponId[i] = (ushort)(reader.ReadUInt32() & 0xFFFF);
        }

        protected override void DoUpdate(SaveIO.SaveWriter writer, long start)
        {
            base.DoUpdate(writer, start);

            // Actor
            writer.Seek(start + 0x1BC, SeekOrigin.Begin);
            if (_actor != null)
                writer.WriteUInt32(_actor.DatumIndex);
            else
                writer.WriteUInt32(0xFFFFFFFF);

            // Unit flags
            writer.Seek(start + 0x1C4, SeekOrigin.Begin);
            writer.WriteInt32(_unitFlags.Data);

            // Team
            writer.Seek(start + 0x1C8, SeekOrigin.Begin);
            writer.WriteByte(_team);

            // Write weapon index info
            writer.Seek(start + 0x340, SeekOrigin.Begin);
            writer.WriteUInt16((ushort)(_weapons.Count + 2));
            writer.WriteSByte(_currentWeaponIndex);
            writer.Skip(1);

            writer.WriteUInt16((ushort)(_weapons.Count + 2));
            writer.WriteSByte(_backupWeaponIndex);
            writer.Skip(1);

            // Write the weapon list
            foreach (WeaponObject weapon in _weapons)
                writer.WriteUInt32(weapon.ID);

            // Write empty spots
            for (int i = _weapons.Count; i < 4; i++)
                writer.WriteUInt32(0xFFFFFFFF);
        }

        internal override void ResolveObjectRefs(List<GameObject> objects)
        {
            base.ResolveObjectRefs(objects);

            for (int i = 0; i < _weaponId.Length; i++)
            {
                if (_weaponId[i] != 0xFFFF)
                    _weapons.Add(objects[(int)_weaponId[i]] as WeaponObject);
            }
        }

        internal void ResolveActor(List<Actor> actors)
        {
            if (_actorId < actors.Count)
                _actor = actors[(int)_actorId];
        }

        protected override void OnDropUsedObject(GameObject obj)
        {
            base.OnDropUsedObject(obj);
            _weapons.Remove(obj as WeaponObject);

            if (_backupWeaponIndex >= _weapons.Count)
            {
                _backupWeaponIndex--;
                if (Carrier == null || Carrier.TagGroup != TagGroup.Vehi)
                    _currentWeaponIndex--;
            }
        }

        protected override void OnReplaceUsedObject(GameObject oldObj, GameObject newObj)
        {
            base.OnReplaceUsedObject(oldObj, newObj);

            if (oldObj != null)
            {
                for (int i = 0; i < _weapons.Count; i++)
                {
                    if (_weapons[i] == oldObj)
                        _weapons[i] = newObj as WeaponObject;
                }
            }
        }

        private int TranslateIndex(int index)
        {
            return (index + _backupWeaponIndex) % _weapons.Count;
        }

        /// <summary>
        /// Constants for _unitFlags
        /// </summary>
        class UnitFlags
        {
            public const int NoFallDamage = 1 << 7;
            public const int PlayerCantEnter = 1 << 10;
            public const int NightVision = 1 << 12;
        }

        private BitVector32 _unitFlags;
        private byte _team = 0;

        private sbyte _currentWeaponIndex;
        private sbyte _backupWeaponIndex;
        private ushort[] _weaponId = new ushort[4];
        private Actor _actor = null;
        private ushort _actorId;
        private List<WeaponObject> _weapons = new List<WeaponObject>();
        private IList<WeaponObject> _roWeapons;
    }
}
