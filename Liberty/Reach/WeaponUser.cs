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
    public class WeaponUser : GameObject
    {
        /// <summary>
        /// Constructs a new WeaponUser.
        /// </summary>
        /// <param name="reader">The SaveReader to read from.</param>
        /// <param name="entry">The object entry data.</param>
        internal WeaponUser(Liberty.SaveIO.SaveReader reader, ObjectEntry entry)
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
            
            if (index >= _weapons.Count)
            {
                // No slot available - just pick it up
                if (newWeapon != null)
                    PickUpWeapon(newWeapon);
                return;
            }

            if (newWeapon != null)
            {
                _weapons[index].ReplaceWith(newWeapon, false);
                _weapons[index] = newWeapon;
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
            if (index < 0 || index >= _weapons.Count)
                throw new IndexOutOfRangeException("Trying to access a non-existant weapon slot");

            return _weapons[index];
        }

        public void DropWeapon(int index)
        {
            if (index < 0 || index >= _weapons.Count)
                throw new IndexOutOfRangeException("Trying to drop from a non-existant weapon slot");

            WeaponObject weapon = _weapons[index];
            weapon.Drop();
            /*if (index < _weapons.Count && _weapons[index] == weapon)
                _weapons.RemoveAt(index);   // is this necessary? derp...*/
        }

        /// <summary>
        /// Picks up a weapon and places it into an empty slot, if available.
        /// </summary>
        /// <param name="weapon">The weapon to pick up.</param>
        /// <returns>true if the weapon was picked up successfully.</returns>
        public bool PickUpWeapon(WeaponObject weapon)
        {
            if (_weapons.Count < 4)
            {
                _weapons.Add(weapon);
                PickUpObject(weapon);
                return true;
            }

            return false;
        }

        public void TransferWeapons(WeaponUser receiver)
        {
            while (receiver._weapons.Count > 0)
                receiver.DropWeapon(0);

            while (_weapons.Count > 0)
                receiver.PickUpWeapon(_weapons[0]);
            _weapons.Clear();
        }

        /// <summary>
        /// The first weapon that this WeaponUser is holding. Can be null.
        /// </summary>
        public WeaponObject PrimaryWeapon
        {
            get { return (_weapons.Count >= 1) ? _weapons[0] : null; }
            set { ChangeWeapon(0, value); }
        }

        /// <summary>
        /// The second weapon that this WeaponUser is holding. Can be null.
        /// </summary>
        public WeaponObject SecondaryWeapon
        {
            get { return (_weapons.Count >= 2) ? _weapons[1] : null; }
            set { ChangeWeapon(1, value); }
        }

        /// <summary>
        /// The third weapon that this WeaponUser is holding. Can be null.
        /// </summary>
        public WeaponObject TertiaryWeapon
        {
            get { return (_weapons.Count >= 3) ? _weapons[2] : null; }
            set { ChangeWeapon(2, value); }
        }
        
        /// <summary>
        /// The fourth weapon that this WeaponUser is holding. Can be null.
        /// </summary>
        public WeaponObject QuaternaryWeapon
        {
            get { return (_weapons.Count >= 4) ? _weapons[3] : null; }
            set { ChangeWeapon(3, value); }
        }

        public IList<WeaponObject> Weapons
        {
            get { return _roWeapons; }
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
            /*int numWeapons = 0;
            WeaponObject[] newList = new WeaponObject[_weapons.Length];
            for (int i = 0; i < _weapons.Length; i++)
            {
                if (_weapons[i] != null)
                    newList[numWeapons++] = _weapons[i];
            }
            _weapons = newList;*/

            // Write weapon count info (twice?)
            writer.Seek(start + 0x340, SeekOrigin.Begin);
            WriteWeaponCountInfo(writer);
            WriteWeaponCountInfo(writer);

            // Write the weapon list
            foreach (WeaponObject weapon in _weapons)
                writer.WriteUInt32(weapon.ID);

            // Write empty spots
            for (int i = _weapons.Count; i < 4; i++)
                writer.WriteUInt32(0xFFFFFFFF);
        }

        private void WriteWeaponCountInfo(SaveIO.SaveWriter writer)
        {
            writer.WriteUInt16((ushort)(_weapons.Count + 2));
            if (_weapons.Count == 0)
                writer.WriteUInt16(0xFFFF);
            else
                writer.WriteUInt16(0x00FF);
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

        protected override void OnDropUsedObject(GameObject obj)
        {
            base.OnDropUsedObject(obj);
            _weapons.Remove(obj as WeaponObject);
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

        private ushort[] _weaponId = new ushort[4];
        private List<WeaponObject> _weapons = new List<WeaponObject>();
        private IList<WeaponObject> _roWeapons;
    }
}
