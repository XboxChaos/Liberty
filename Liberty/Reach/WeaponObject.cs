using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Liberty.Reach
{
    /// <summary>
    /// A weapon object.
    /// </summary>
    public class WeaponObject : GameObject
    {
        /// <summary>
        /// Constructs a new WeaponObject.
        /// </summary>
        /// <param name="reader">The SaveReader to read from.</param>
        /// <param name="entry">The object entry data.</param>
        internal WeaponObject(Liberty.SaveIO.SaveReader reader, ObjectEntry entry)
            : base(reader, entry)
        {
        }

        /// <summary>
        /// How much ammo this weapon has left.
        /// </summary>
        public short Ammo
        {
            get { return _ammo; }
            set { _ammo = value; }
        }

        /// <summary>
        /// How much ammo this weapon has left in its clip.
        /// </summary>
        public short ClipAmmo
        {
            get { return _clipAmmo; }
            set { _clipAmmo = value; }
        }

        protected override void DoLoad(SaveIO.SaveReader reader, long start)
        {
            base.DoLoad(reader, start);

            reader.Seek(start + 0xD4, SeekOrigin.Begin);
            _usageInfo = reader.ReadUInt32();

            // Is this value present in other objects as well?
            reader.Seek(start + 0x1A8, SeekOrigin.Begin);
            _weaponFlags = reader.ReadUInt32();

            // User ID
            reader.Seek(start + 0x1B6, SeekOrigin.Begin);
            _userId = reader.ReadUInt16();

            // Ammo
            reader.Seek(start + 0x2C6, SeekOrigin.Begin);
            _ammo = reader.ReadInt16();
            reader.Seek(2, SeekOrigin.Current);
            _clipAmmo = reader.ReadInt16();
        }

        protected override void DoUpdate(Liberty.SaveIO.SaveWriter writer, long start)
        {
            base.DoUpdate(writer, start);

            writer.Seek(start + 0xD4, SeekOrigin.Begin);
            writer.WriteUInt32(_usageInfo);

            writer.Seek(start + 0x1A8, SeekOrigin.Begin);
            writer.WriteUInt32(_weaponFlags);

            writer.Seek(start + 0x1B4, SeekOrigin.Begin);
            if (_user != null)
            {
                // hax
                writer.WriteUInt32(_user.ID);
                writer.WriteUInt32(_user.ID);
            }
            else
            {
                writer.WriteUInt32(0xFFFFFFFF);
                writer.WriteUInt32(0xFFFFFFFF);
            }

            writer.Seek(start + 0x2C6, SeekOrigin.Begin);
            writer.WriteInt16(_ammo);
            writer.Seek(2, SeekOrigin.Current);
            writer.WriteInt16(_clipAmmo);
        }

        public override void Drop()
        {
            if (Carrier == null && _user != null)
                DropUsedObject(_user, this);

            _user = null;
            _weaponFlags &= ~WeaponFlags.InUse;
            _weaponFlags |= WeaponFlags.Unused;

            base.Drop();
        }

        protected override void OnPickUp()
        {
            base.OnPickUp();

            if (_user == null && (Carrier.TagGroup == TagGroup.Vehi || Carrier.TagGroup == TagGroup.Bipd))
                _user = Carrier;

            _weaponFlags &= ~WeaponFlags.Unused;
            _weaponFlags |= WeaponFlags.InUse;
        }

        public override void ReplaceWith(GameObject newObj, bool deleteCarried)
        {
            GameObject user = _user;
            if (Carrier == null && _user != null)
                ReplaceUsedObject(_user, this, newObj);

            WeaponObject newWeapon = newObj as WeaponObject;
            if (newWeapon != null)
                newWeapon._weaponFlags = _weaponFlags;

            base.ReplaceWith(newObj, deleteCarried);

            if (newWeapon != null)
                newWeapon._user = user;

            newWeapon._usageInfo = _usageInfo;
        }

        internal override void ResolveObjectRefs(List<GameObject> objects)
        {
            base.ResolveObjectRefs(objects);

            if (_userId != 0xFFFF)
                _user = objects[(int)_userId];
        }

        private uint _weaponFlags;

        private short _ammo;
        private short _clipAmmo;

        private ushort _userId;
        private GameObject _user = null;

        private uint _usageInfo;  // I hardly even know what this value means, it's just needed for weapon carrying
    }
}
