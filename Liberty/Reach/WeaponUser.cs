using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Liberty.Reach
{
    /// <summary>
    /// Implements a consistent weapon interface for objects that can carry and use weapons.
    /// </summary>
    public class WeaponUser : HlmtObject
    {
        /// <summary>
        /// Constructs a new WeaponUser.
        /// </summary>
        /// <param name="reader">The SaveReader to read from.</param>
        /// <param name="entry">The object entry data.</param>
        internal WeaponUser(Liberty.SaveIO.SaveReader reader, ObjectEntry entry)
            : base(reader, entry)
        {
        }

        /// <summary>
        /// Changes one of the weapons that this object is holding.
        /// </summary>
        /// <param name="index">The zero-based weapon index (0 = 1st weapon, 3 = 4th weapon)</param>
        /// <param name="newWeapon">The new weapon. Use null to mean "no weapon."</param>
        public void SetWeapon(int index, WeaponObject newWeapon)
        {
            if (index < 0 || index >= _weapons.Length)
                throw new IndexOutOfRangeException("WeaponUsers can only hold 4 weapons at a time.");

            if (newWeapon != null)
            {
                if (_weapons[index] != null)
                    _weapons[index].ReplaceWith(newWeapon, false);
                else
                    PickUpObject(newWeapon);
            }

            _weapons[index] = newWeapon;
        }

        /// <summary>
        /// Returns one of the weapons that this object is holding.
        /// </summary>
        /// <param name="index">The zero-based weapon index (0 = 1st weapon, 3 = 4th weapon)</param>
        /// <returns>The weapon at the specified index. Can be null if no weapon is in the specified slot.</returns>
        public WeaponObject GetWeapon(int index)
        {
            if (index < 0 || index >= _weapons.Length)
                throw new IndexOutOfRangeException("WeaponUsers can only hold 4 weapons at a time.");

            return _weapons[index];
        }

        /// <summary>
        /// Picks up a weapon and places it into an empty slot, if available.
        /// </summary>
        /// <param name="weapon">The weapon to pick up.</param>
        /// <returns>true if the weapon was picked up successfully.</returns>
        public bool PickUpWeapon(WeaponObject weapon)
        {
            for (int i = 0; i < _weapons.Length; i++)
            {
                if (_weapons[i] == null)
                {
                    SetWeapon(i, weapon);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The first weapon that this WeaponUser is holding. Can be null.
        /// </summary>
        public WeaponObject PrimaryWeapon
        {
            get { return _weapons[0]; }
            set { SetWeapon(0, value); }
        }

        /// <summary>
        /// The second weapon that this WeaponUser is holding. Can be null.
        /// </summary>
        public WeaponObject SecondaryWeapon
        {
            get { return _weapons[1]; }
            set { SetWeapon(1, value); }
        }

        protected override void DoLoad(SaveIO.SaveReader reader, long start)
        {
            base.DoLoad(reader, start);

            reader.Seek(start + 0x348, SeekOrigin.Begin);
            for (int i = 0; i < 4; i++)
                _weaponId[i] = (ushort)(reader.ReadUInt32() & 0xFFFF);
        }

        protected override void DoUpdate(SaveIO.SaveWriter writer, long start)
        {
            base.DoUpdate(writer, start);

            // Fill null spots in the weapons list
            int numWeapons = 0;
            WeaponObject[] newList = new WeaponObject[_weapons.Length];
            for (int i = 0; i < _weapons.Length; i++)
            {
                if (_weapons[i] != null)
                    newList[numWeapons++] = _weapons[i];
            }
            _weapons = newList;

            // Write weapon count info
            writer.Seek(start + 0x340, SeekOrigin.Begin);
            writer.WriteUInt16((ushort)(numWeapons + 2));
            if (numWeapons == 0)
                writer.WriteUInt16(0xFFFF);
            else
                writer.WriteUInt16(0x00FF);

            // Write it again?
            writer.WriteUInt16((ushort)(numWeapons + 2));
            if (numWeapons == 0)
                writer.WriteUInt16(0xFFFF);
            else
                writer.WriteUInt16(0x00FF);

            // Write the weapon list
            for (int i = 0; i < _weapons.Length; i++)
            {
                if (_weapons[i] != null)
                    writer.WriteUInt32(_weapons[i].ID);
                else
                    writer.WriteUInt32(0xFFFFFFFF);
            }
        }

        internal override void ResolveObjectRefs(List<GameObject> objects)
        {
            base.ResolveObjectRefs(objects);

            for (int i = 0; i < _weaponId.Length; i++)
            {
                if (_weaponId[i] != 0xFFFF)
                    _weapons[i] = objects[(int)_weaponId[i]] as WeaponObject;
            }
        }

        protected override void OnDropUsedObject(GameObject obj)
        {
            base.OnDropUsedObject(obj);

            for (int i = 0; i < _weapons.Length; i++)
            {
                if (_weapons[i] == obj)
                    _weapons[i] = null;
            }
        }

        protected override void OnReplaceUsedObject(GameObject oldObj, GameObject newObj)
        {
            base.OnReplaceUsedObject(oldObj, newObj);

            if (oldObj != null)
            {
                for (int i = 0; i < _weapons.Length; i++)
                {
                    if (_weapons[i] == oldObj)
                        _weapons[i] = newObj as WeaponObject;
                }
            }
        }

        private ushort[] _weaponId = new ushort[4];
        private WeaponObject[] _weapons = new WeaponObject[4];
    }
}
