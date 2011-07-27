﻿/*
* Liberty - http://xboxchaos.com/
*
* Copyright (C) 2011 XboxChaos
* Copyright (C) 2011 ThunderWaffle/AMD
* Copyright (C) 2011 Xeraxic
*
* Liberty is free software; you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published
* by the Free Software Foundation; either version 2 of the License,
* or (at your option) any later version.
*
* Liberty is distributed in the hope that it will be useful, but
* WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
* General Public License for more details.
*
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Liberty.Reach
{
    /// <summary>
    /// Provides constants for known object tag groups.
    /// </summary>
    public enum TagGroup
    {
        Bipd = 0,
        Vehi = 1,
        Weap = 2,
        Eqip = 3,
        Term = 4,
        Proj = 5,
        Scen = 6,
        Mach = 7,
        Ctrl = 8,
        Ssce = 9,
        Bloc = 10,
        Crea = 11,
        Efsc = 13,
        Unknown = 255    // Not actually a valid value, this is just for use as a placeholder.
    }

    // I'm using classes instead of enums here so I don't have to do a
    // typecast every single time I want to use one of these
    public class ObjectFlags
    {
        public const uint NotCarried = 0x80U;
        public const uint Active = 0x20000000;
    }

    public class PhysicsFlags
    {
        public const ulong Physical = 0x100UL;
        public const ulong Activated = 0x8000000UL;
    }

    public class WeaponFlags
    {
        public const uint InUse = 0x3010000U;     // Not entirely sure what each of these flags does, I haven't had time to look into it
        public const uint Unused = 0x4000000U;
    }

    /// <summary>
    /// Describes an entry in the "object" chunk.
    /// </summary>
    /// <param name="reader">
    /// The SaveReader to read the entry from.
    /// On return, it will be positioned at the start of the next entry.
    /// </param>
    internal class ObjectEntry
    {
        internal ObjectEntry(Chunk chunk, uint index)
        {
            SaveIO.SaveReader reader = chunk.EntryReader;

            _offset = reader.BaseStream.Position + chunk.EntryListStart;
            _id = ((uint)reader.ReadUInt16() << 16) | index;
            _flags = reader.ReadUInt16();
            _tagGroup = (TagGroup)(reader.ReadUInt16() >> 8);
            _type = reader.ReadUInt16();
            _poolOffset = reader.ReadUInt32();
            _memAddress = reader.ReadUInt32();
        }

        internal ObjectEntry(uint id, ushort flags, TagGroup tagGroup, ushort type, uint poolOffset, uint loadAddress, uint writeOffset)
        {
            _id = id;
            _flags = flags;
            _tagGroup = tagGroup;
            _type = type;
            _poolOffset = poolOffset;
            _memAddress = loadAddress;
            _offset = writeOffset;
        }

        public void Update(SaveIO.SaveWriter writer)
        {
            writer.Seek(_offset, SeekOrigin.Begin);

            writer.WriteUInt16((ushort)(_id >> 16));
            writer.WriteUInt16(_flags);
            writer.WriteUInt16((ushort)((ushort)_tagGroup << 8));
            writer.WriteUInt16(_type);
            writer.WriteUInt32(_poolOffset);
            writer.WriteUInt32(_memAddress);
        }

        public uint ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public ushort Flags
        {
            get { return _flags; }
            set { _flags = value; }
        }

        public TagGroup TagGroup
        {
            get { return _tagGroup; }
            set { _tagGroup = value; }
        }

        public ushort Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public uint PoolOffset
        {
            get { return _poolOffset; }
            set { _poolOffset = value; }
        }

        public uint LoadAddress
        {
            get { return _memAddress; }
            set { _memAddress = value; }
        }

        /// <summary>
        /// Whether or not this entry points to a valid object.
        /// </summary>
        public bool Exists
        {
            get { return (_memAddress != 0); }
        }

        private long _offset;
        private uint _id;
        private ushort _flags;
        private TagGroup _tagGroup;
        private ushort _type;
        private uint _poolOffset;
        private uint _memAddress;
    }

    /// <summary>
    /// Represents object link information, stored 16 bytes before the beginning of each object's data.
    /// </summary>
    internal struct ObjectLinkData
    {
        /// <summary>
        /// Reads link data out of a SaveReader from its current position.
        /// </summary>
        /// <param name="reader">The SaveReader to read link data from.</param>
        public ObjectLinkData(SaveIO.SaveReader reader)
        {
            Size = reader.ReadUInt32();
            ID = reader.ReadUInt32();
            NextLinkOffset = reader.ReadUInt32();
            PreviousLinkOffset = reader.ReadUInt32();
        }

        /// <summary>
        /// Writes the link data to a SaveWriter at its current position.
        /// </summary>
        /// <param name="writer">The SaveWriter to write the new link data to.</param>
        public void Update(SaveIO.SaveWriter writer)
        {
            writer.WriteUInt32(Size);
            writer.WriteUInt32(ID);
            writer.WriteUInt32(NextLinkOffset);
            writer.WriteUInt32(PreviousLinkOffset);
        }

        public uint Size;
        public uint ID;
        public uint NextLinkOffset;
        public uint PreviousLinkOffset;
    }

    /// <summary>
    /// Describes object data.
    /// </summary>
    public class GameObject
    {
        /// <summary>
        /// Constructs a new Object, given a stream to read from and object entry data.
        /// </summary>
        /// <param name="reader">The SaveReader to read the data from.</param>
        /// <param name="entry">An ObjectEntry struct containing basic info about the object.</param>
        internal GameObject(SaveIO.SaveReader reader, ObjectEntry entry)
        {
            _entry = entry;
            _dataPosition = reader.BaseStream.Position;

            DoLoad(reader, _dataPosition);
        }

        /// <summary>
        /// Writes out any altered fields.
        /// </summary>
        /// <param name="writer">The SaveWriter to write to.</param>
        internal void Update(Liberty.SaveIO.SaveWriter writer)
        {
            if (_relink)
            {
                _entry.Update(writer);
                writer.Seek(_dataPosition - 16, SeekOrigin.Begin);
                _linkData.Update(writer);
            }

            DoUpdate(writer, _dataPosition);
        }

        /// <summary>
        /// Deletes the object, marking it in the save file as being unused.
        /// Any carried items will be dropped.
        /// </summary>
        /// <remarks>
        /// This does not update the object list!
        /// Update() needs to know the object's info so that it can properly mark it as unused.
        /// </remarks>
        public virtual void Delete(bool deleteCarried = false)
        {
            DropAll(deleteCarried);
            Drop();
            _entry.ID = 0;
            _entry.Flags = 0x22;
            _entry.LoadAddress = 0;

            if (_previousObject != null)
            {
                _previousObject._linkData.NextLinkOffset = _linkData.NextLinkOffset;
                _previousObject._nextObject = _nextObject;
                _previousObject._relink = true;
            }
            if (_nextObject != null)
            {
                _nextObject._linkData.PreviousLinkOffset = _linkData.PreviousLinkOffset;
                _nextObject._previousObject = _previousObject;
                _nextObject._relink = true;
            }

            _linkData.Size = 0xFFFFFFFF;
            _linkData.ID = 0xFFFFFFFF;
            _linkData.NextLinkOffset = 0xFFFFFFFF;
            _linkData.PreviousLinkOffset = 0xFFFFFFFF;
            _relink = true;
            _deleted = true;
        }

        /// <summary>
        /// Drops the object from its carrier.
        /// </summary>
        public virtual void Drop()
        {
            if (_carrier != null)
                _carrier.DropCarriedObject(this);
        }

        /// <summary>
        /// Drops all objects that the object is carrying.
        /// Optionally, you can delete them as well.
        /// </summary>
        public void DropAll(bool delete = false)
        {
            GameObject obj = _firstCarried;
            while (obj != null)
            {
                GameObject next = obj._nextCarried;
                obj.Drop();
                if (delete)
                    obj.Delete(true);
                obj = next;
            }
        }

        /// <summary>
        /// Replaces this object with another one.
        /// The old object will be deleted.
        /// </summary>
        /// <param name="newObj">The object to replace this object with.</param>
        public virtual void ReplaceWith(GameObject newObj, bool deleteCarried)
        {
            float oldX = _x;
            float oldY = _y;
            float oldZ = _z;
            bool oldPhysics = PhysicsEnabled;
            uint oldFlags = _flags;

            // Replace!
            if (_carrier != null)
                _carrier.ReplaceCarriedObject(this, newObj);
            else if (newObj != null && newObj.Carrier != null)
                newObj.Drop();

            if (newObj != null)
            {
                // Move the new object to our old position
                newObj.X = oldX;
                newObj.Y = oldY;
                newObj.Z = oldZ;
                newObj.PhysicsEnabled = oldPhysics;

                // Adjust flags
                newObj._flags = (newObj._flags & ~ObjectFlags.NotCarried) | (oldFlags & ObjectFlags.NotCarried);
            }

            Delete(deleteCarried);
        }

        /// <summary>
        /// The object's unique ID in the save file.
        /// </summary>
        public uint ID
        {
            get { return _entry.ID; }
        }

        /// <summary>
        /// The tag that the object belongs to.
        /// </summary>
        /// <example>
        /// ObjectTag.Bipd denotes a biped object.
        /// </example>
        /// <seealso cref="TagGroup"/>
        public TagGroup TagGroup
        {
            get { return _entry.TagGroup; }
        }

        /// <summary>
        /// Identifies the object in a non-map-specific way.
        /// </summary>
        public ushort Type
        {
            get { return _entry.Type; }
        }

        /// <summary>
        /// The resource ID of the object as used in the .map file.
        /// </summary>
        public uint ResourceID
        {
            get { return _resourceId; }
            //set { _resourceId = value; }
        }

        /// <summary>
        /// The zone that the object is currently in.
        /// </summary>
        public uint Zone
        {
            get { return _zone; }
        }

        /// <summary>
        /// Whether or not this object has been deleted by a call to Delete().
        /// </summary>
        public bool Deleted
        {
            get { return _deleted; }
        }

        /// <summary>
        /// The object's X position in 3D space.
        /// </summary>
        public float X
        {
            get { return _x; }
            set { _x = value; _boundsX1 = value; _boundsX2 = value; }
        }

        /// <summary>
        /// The object's Y position in 3D space.
        /// </summary>
        public float Y
        {
            get { return _y; }
            set { _y = value; _boundsY1 = value; _boundsY2 = value; }
        }

        /// <summary>
        /// The object's Z position in 3D space.
        /// </summary>
        public float Z
        {
            get { return _z; }
            set { _z = value; _boundsZ1 = value; _boundsZ2 = value; }
        }

        public float BoundsX1
        {
            get { return _boundsX1; }
            set { _boundsX1 = value; }
        }

        public float BoundsY1
        {
            get { return _boundsY1; }
            set { _boundsY1 = value; }
        }

        public float BoundsZ1
        {
            get { return _boundsZ1; }
            set { _boundsZ1 = value; }
        }

        public float BoundsW1
        {
            get { return _boundsW1; }
            set { _boundsW1 = value; }
        }

        public float BoundsX2
        {
            get { return _boundsX2; }
            set { _boundsX2 = value; }
        }

        public float BoundsY2
        {
            get { return _boundsY2; }
            set { _boundsY2 = value; }
        }

        public float BoundsZ2
        {
            get { return _boundsZ2; }
            set { _boundsZ2 = value; }
        }

        public float BoundsW2
        {
            get { return _boundsW2; }
            set { _boundsW2 = value; }
        }

        /// <summary>
        /// The object following this object in memory. Can be null.
        /// </summary>
        public GameObject Next
        {
            get { return _nextObject; }
        }

        /// <summary>
        /// The object before this object in memory. Can be null.
        /// </summary>
        public GameObject Previous
        {
            get { return _previousObject; }
        }

        /// <summary>
        /// The object that is carrying this object. Can be null if not being carried.
        /// </summary>
        public GameObject Carrier
        {
            get { return _carrier; }
            set { _carrier = value; }
        }

        /// <summary>
        /// The first object that is being carried by this object.
        /// Can be null if nothing is being carried.
        /// </summary>
        public GameObject FirstCarried
        {
            get { return _firstCarried; }
            set { _firstCarried = value; }
        }

        /// <summary>
        /// The next object that is being carried by the carrier.
        /// Can be null if not being carried or if this is the last carried object.
        /// </summary>
        public GameObject NextCarried
        {
            get { return _nextCarried; }
            set { _nextCarried = value; }
        }

        /// <summary>
        /// The object data's offset in the mmiof.bmf file.
        /// </summary>
        public uint FileOffset
        {
            get { return (uint)_dataPosition; }
        }

        /// <summary>
        /// Whether or not physics should be simulated for this object.
        /// </summary>
        public bool PhysicsEnabled
        {
            get
            {
                return ((_physicsFlags & PhysicsFlags.Physical) == PhysicsFlags.Physical);
            }
            set
            {
                if (value)
                    _physicsFlags |= PhysicsFlags.Physical;
                else
                    _physicsFlags &= ~PhysicsFlags.Physical;
            }
        }

        public bool IsActive
        {
            get { return ((_flags & ObjectFlags.Active) == ObjectFlags.Active); }
        }

        /// <summary>
        /// Override this to load any extra properties the object might have.
        /// </summary>
        /// <param name="reader">The SaveReader to read from</param>
        /// <param name="start">The start of the object's data</param>
        protected virtual void DoLoad(SaveIO.SaveReader reader, long start)
        {
            // Link data
            reader.Seek(start - 16, SeekOrigin.Begin);
            _linkData = new ObjectLinkData(reader);

            // Resource ID
            _resourceId = reader.ReadUInt32();
            
            // Flags
            _flags = reader.ReadUInt32();

            // Zone?
            _zone = reader.ReadUInt32();

            // Carry info
            _nextCarriedId = (ushort)(reader.ReadUInt32() & 0xFFFF);
            _firstCarriedId = (ushort)(reader.ReadUInt32() & 0xFFFF);
            _carrierId = (ushort)(reader.ReadUInt32() & 0xFFFF);

            // Position data
            reader.Seek(start + 0x20, SeekOrigin.Begin);
            _boundsX1 = reader.ReadFloat();
            _boundsY1 = reader.ReadFloat();
            _boundsZ1 = reader.ReadFloat();
            _boundsW1 = reader.ReadFloat();
            _boundsX2 = reader.ReadFloat();
            _boundsY2 = reader.ReadFloat();
            _boundsZ2 = reader.ReadFloat();
            _boundsW2 = reader.ReadFloat();
            reader.Seek(4, SeekOrigin.Current);
            _x = reader.ReadFloat();
            _y = reader.ReadFloat();
            _z = reader.ReadFloat();
            _w = reader.ReadFloat();

            // Flags
            reader.Seek(start + 0xD8, SeekOrigin.Begin);
            _physicsFlags = reader.ReadUInt64();

            // Previous/next object ID's
            if (_linkData.PreviousLinkOffset != 0)
            {
                reader.Seek(0x76844C + _linkData.PreviousLinkOffset + 6, SeekOrigin.Begin);
                _previousObjectId = reader.ReadUInt16();
            }
            if (_linkData.NextLinkOffset != 0)
            {
                reader.Seek(0x76844C + _linkData.NextLinkOffset + 6, SeekOrigin.Begin);
                _nextObjectId = reader.ReadUInt16();
            }
        }

        /// <summary>
        /// Override this to write out any changed properties.
        /// </summary>
        /// <param name="writer">The SaveWriter to write to</param>
        /// <param name="start">The start of the object's data</param>
        protected virtual void DoUpdate(SaveIO.SaveWriter writer, long start)
        {
            writer.Seek(start + 0x4, SeekOrigin.Begin);
            writer.WriteUInt32(_flags);
            writer.WriteUInt32(_zone);
            
            if (_nextCarried != null)
                writer.WriteUInt32(_nextCarried.ID);
            else
                writer.WriteUInt32(0xFFFFFFFF);

            if (_firstCarried != null)
                writer.WriteUInt32(_firstCarried.ID);
            else
                writer.WriteUInt32(0xFFFFFFFF);

            if (_carrier != null)
                writer.WriteUInt32(_carrier.ID);
            else
                writer.WriteUInt32(0xFFFFFFFF);

            writer.Seek(start + 0x20, SeekOrigin.Begin);
            writer.WriteFloat(_boundsX1);
            writer.WriteFloat(_boundsY1);
            writer.WriteFloat(_boundsZ1);
            writer.WriteFloat(_boundsW1);
            writer.WriteFloat(_boundsX2);
            writer.WriteFloat(_boundsY2);
            writer.WriteFloat(_boundsZ2);
            writer.WriteFloat(_boundsW2);
            writer.Seek(4, SeekOrigin.Current);
            writer.WriteFloat(_x);
            writer.WriteFloat(_y);
            writer.WriteFloat(_z);
            writer.WriteFloat(_w);

            writer.Seek(start + 0xD8, SeekOrigin.Begin);
            writer.WriteUInt64(_physicsFlags);
        }

        /// <summary>
        /// Resolves object ID's stored within this object to Object pointers.
        /// This is done so that objects pointing to ID's later on in the table can resolve them properly.
        /// </summary>
        /// <param name="objects">The object list to resolve pointers from.</param>
        internal virtual void ResolveObjectRefs(List<GameObject> objects)
        {
            if (_previousObjectId != 0xFFFF)
                _previousObject = objects[(int)_previousObjectId];
            if (_nextObjectId != 0xFFFF)
                _nextObject = objects[(int)_nextObjectId];
            if (_carrierId != 0xFFFF)
                _carrier = objects[(int)_carrierId];
            if (_firstCarriedId != 0xFFFF)
                _firstCarried = objects[(int)_firstCarriedId];
            if (_nextCarriedId != 0xFFFF)
                _nextCarried = objects[(int)_nextCarriedId];
        }

        /// <summary>
        /// Drops a carried object.
        /// </summary>
        /// <param name="obj">The object that should be dropped.</param>
        protected void DropCarriedObject(GameObject obj)
        {
            if (_firstCarried == obj)
            {
                OnDropUsedObject(obj);
                _firstCarried = obj._nextCarried;
            }
            else
            {
                GameObject current = _firstCarried;
                while (current != null && current._nextCarried != obj)
                    current = current._nextCarried;
                if (current != null)
                {
                    OnDropUsedObject(obj);
                    current._nextCarried = obj._nextCarried;
                }
            }

            // Traverse the carry hierarchy to find what the object's zone should be
            GameObject zoneHolder = obj._carrier;
            while (zoneHolder._carrier != null)
                zoneHolder = zoneHolder._carrier;
            obj._zone = zoneHolder._zone;

            // Adjust the object's position to use absolute coordinates
            obj.X += X;
            obj.Y += Y;
            obj.Z += Z;
            obj.PhysicsEnabled = true;

            obj._flags |= ObjectFlags.NotCarried;
            obj._carrier = null;
            obj._nextCarried = null;
        }

        // hax
        protected static void DropUsedObject(GameObject user, GameObject obj)
        {
            user.OnDropUsedObject(obj);
        }

        protected virtual void OnDropUsedObject(GameObject obj)
        {
        }

        /// <summary>
        /// Picks up and starts carrying another object.
        /// </summary>
        /// <remarks>
        /// This should only be used by derived classes when object reference properties are changed.
        /// </remarks>
        /// <param name="obj">The object to pick up.</param>
        internal void PickUpObject(GameObject obj)
        {
            obj.Drop();
            obj._flags &= ~ObjectFlags.NotCarried;
            obj._zone = 0xFFFF0000;
            obj._carrier = this;
            obj._nextCarried = _firstCarried;
            obj.PhysicsEnabled = false;
            _firstCarried = obj;
            obj.OnPickUp();
        }

        protected virtual void OnPickUp()
        {
        }

        /// <summary>
        /// Replaces a carried object with another one.
        /// </summary>
        /// <param name="oldObj">The old object</param>
        /// <param name="newObj">The object it will be replaced with</param>
        protected void ReplaceCarriedObject(GameObject oldObj, GameObject newObj)
        {
            OnReplaceUsedObject(oldObj, newObj);

            if (oldObj != null && oldObj.Carrier == this)
                oldObj.Drop();
            if (newObj != null && newObj.Carrier != this)
                PickUpObject(newObj);
        }

        // moar hax
        protected static void ReplaceUsedObject(GameObject user, GameObject oldObj, GameObject newObj)
        {
            user.OnReplaceUsedObject(oldObj, newObj);
        }

        protected virtual void OnReplaceUsedObject(GameObject oldObj, GameObject newObj)
        {
        }

        public override bool Equals(object obj)
        {
            GameObject gameObject = obj as GameObject;
            if (gameObject == null)
                return false;
            return (_entry.ID == gameObject._entry.ID);
        }

        public override int GetHashCode()
        {
            return (int)_entry.ID;
        }

        private bool _relink = false;
        private ObjectLinkData _linkData;
        private bool _deleted = false;
        private ObjectEntry _entry;
        private long _dataPosition;

        private uint _resourceId;
        private uint _flags;
        private uint _zone;

        private ushort _carrierId;
        private GameObject _carrier = null;
        private ushort _firstCarriedId;
        private GameObject _firstCarried = null;
        private ushort _nextCarriedId;
        private GameObject _nextCarried = null;

        private float _boundsX1, _boundsY1, _boundsZ1, _boundsW1;
        private float _boundsX2, _boundsY2, _boundsZ2, _boundsW2;
        private float _x, _y, _z, _w;

        private ulong _physicsFlags;

        private ushort _previousObjectId = 0xFFFF;
        private GameObject _previousObject = null;
        private ushort _nextObjectId = 0xFFFF;
        private GameObject _nextObject = null;
    }

    /// <summary>
    /// An object that has HLMT data.
    /// </summary>
    public class HlmtObject : GameObject
    {
        /// <summary>
        /// Constructs a new HLMT object.
        /// </summary>
        /// <param name="reader">The SaveReader to read from.</param>
        /// <param name="entry">The object entry data.</param>
        internal HlmtObject(Liberty.SaveIO.SaveReader reader, ObjectEntry entry)
            : base(reader, entry)
        {
        }

        /// <summary>
        /// Whether or not this object should be invincible.
        /// </summary>
        public bool Invincible
        {
            get { return _makeInvincible; }
            set { _makeInvincible = value; }
        }

        protected override void DoLoad(SaveIO.SaveReader reader, long start)
        {
            base.DoLoad(reader, start);

            // Check invincibility status
            reader.Seek(start + 0x110, SeekOrigin.Begin);
            _oldHealthModifier = reader.ReadFloat();
            _oldShieldModifier = reader.ReadFloat();
            if (float.IsNaN(_oldHealthModifier) || float.IsNaN(_oldShieldModifier) || (_oldHealthModifier == 0 && _oldShieldModifier == 0))
            {
                _makeInvincible = true;
                _canUseOldValues = false;
            }
        }

        protected override void DoUpdate(SaveIO.SaveWriter writer, long start)
        {
            base.DoUpdate(writer, start);

            // Invincibility
            // TODO: fix hax
            writer.Seek(start + 0x110, SeekOrigin.Begin);
            if (_makeInvincible)
            {
                if (_oldHealthModifier != 0)
                    writer.WriteUInt32(0xFFFFFFFF);
                else
                    writer.WriteUInt32(0x00000000);
                if (_oldShieldModifier != 0)
                    writer.WriteUInt32(0xFFFFFFFF);
            }
            else
            {
                if (_canUseOldValues)
                {
                    // Write the old health and shield values
                    writer.WriteFloat(_oldHealthModifier);
                    writer.WriteFloat(_oldShieldModifier);
                }
                else
                {
                    // TODO: FIX THIS!!!
                    if (_oldHealthModifier != 0)
                        writer.WriteUInt32(0x42340000);
                    else
                        writer.WriteUInt32(0x00000000);
                    if (_oldShieldModifier != 0)
                        writer.WriteUInt32(0x428C0000);
                }
            }
        }

        private bool _makeInvincible = false;
        private bool _canUseOldValues = true;
        private float _oldHealthModifier;
        private float _oldShieldModifier;
    }

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

        /// <summary>
        /// The third weapon that this WeaponUser is holding. Can be null.
        /// </summary>
        public WeaponObject TertiaryWeapon
        {
            get { return _weapons[2]; }
            set { SetWeapon(2, value); }
        }

        /// <summary>
        /// The fourth weapon that this WeaponUser is holding. Can be null.
        /// </summary>
        public WeaponObject QuarternaryWeapon
        {
            get { return _weapons[3]; }
            set { SetWeapon(3, value); }
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
            int j = 0;
            WeaponObject[] newList = new WeaponObject[_weapons.Length];
            for (int i = 0; i < _weapons.Length; i++)
            {
                if (_weapons[i] != null)
                    newList[j++] = _weapons[i];
            }
            _weapons = newList;

            // Now write it out
            writer.Seek(start + 0x348, SeekOrigin.Begin);
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

    /// <summary>
    /// A weapon object.
    /// </summary>
    public class WeaponObject : HlmtObject
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
    }

    /// <summary>
    /// An equipment object, such as an armor ability.
    /// </summary>
    public class EquipmentObject : GameObject
    {
        /// <summary>
        /// Constructs a new EquipmentObject.
        /// </summary>
        /// <param name="reader">The SaveReader to read from.</param>
        /// <param name="entry">The object entry data.</param>
        internal EquipmentObject(Liberty.SaveIO.SaveReader reader, ObjectEntry entry)
            : base(reader, entry)
        {
        }

        /// <summary>
        /// The Player object that this equipment object is associated with.
        /// </summary>
        /// <remarks>
        /// This isn't read from the file because there's never more than one player.
        /// GamePlayer automatically manages this property instead.
        /// </remarks>
        internal GamePlayer Player
        {
            set { _player = value; }
        }

        protected override void DoUpdate(SaveIO.SaveWriter writer, long start)
        {
            base.DoUpdate(writer, start);

            // Player data
            writer.Seek(start + 0xF0, SeekOrigin.Begin);
            if (_player != null)
                writer.WriteUInt32(_player.ID);
            else
                writer.WriteUInt32(0xFFFFFFFF);
            writer.Seek(start + 0x1B4, SeekOrigin.Begin);
            if (Carrier != null && Carrier.TagGroup == TagGroup.Bipd)
            {
                writer.WriteUInt32(Carrier.ID);
                writer.WriteUInt32(Carrier.ID);
            }
            else
            {
                writer.WriteUInt32(0xFFFFFFFF);
                writer.WriteUInt32(0xFFFFFFFF);
            }

            // This ID
            writer.WriteUInt32(ID);
        }

        GamePlayer _player = null;
    }

    /// <summary>
    /// A vehicle object.
    /// </summary>
    public class VehicleObject : WeaponUser
    {
        /// <summary>
        /// Constructs a new VehicleObject.
        /// </summary>
        /// <param name="reader">The SaveReader to read from.</param>
        /// <param name="entry">The object entry data.</param>
        internal VehicleObject(Liberty.SaveIO.SaveReader reader, ObjectEntry entry)
            : base(reader, entry)
        {
        }

        protected override void DoLoad(SaveIO.SaveReader reader, long start)
        {
            base.DoLoad(reader, start);

            reader.Seek(start + 0x388, SeekOrigin.Begin);
            _driverObjectId = (ushort)(reader.ReadUInt32() & 0xFFFF);
            _controllerObjectId = (ushort)(reader.ReadUInt32() & 0xFFFF);
        }

        protected override void DoUpdate(SaveIO.SaveWriter writer, long start)
        {
            base.DoUpdate(writer, start);

            writer.Seek(start + 0x388, SeekOrigin.Begin);
            if (_driver != null)
                writer.WriteUInt32(_driver.ID);
            else
                writer.WriteUInt32(0xFFFFFFFF);
            if (_controller != null)
                writer.WriteUInt32(_controller.ID);
            else
                writer.WriteUInt32(0xFFFFFFFF);
        }

        internal override void ResolveObjectRefs(List<GameObject> objects)
        {
            base.ResolveObjectRefs(objects);

            if (_driverObjectId != 0xFFFF)
                _driver = objects[(int)_driverObjectId];
            if (_controllerObjectId != 0xFFFF)
                _controller = objects[(int)_controllerObjectId];
        }

        protected override void OnDropUsedObject(GameObject obj)
        {
            base.OnDropUsedObject(obj);

            if (_driver == obj)
                _driver = null;
            if (_controller == obj)
                _controller = null;
        }

        protected override void OnReplaceUsedObject(GameObject oldObj, GameObject newObj)
        {
            base.OnReplaceUsedObject(oldObj, newObj);

            if (_driver == oldObj)
                _driver = newObj;
            if (_controller == oldObj)
                _controller = newObj;
        }

        public override void ReplaceWith(GameObject newObj, bool deleteCarried)
        {
            GameObject driver = _driver;
            GameObject controller = _controller;

            if (_driver != null)
                ReplaceUsedObject(_driver, this, newObj);
            if (_controller != null)
                ReplaceUsedObject(_controller, this, newObj);

            // Drop all bipd's from the new vehicle
            GameObject current = newObj.FirstCarried;
            while (current != null)
            {
                GameObject next = current.NextCarried;
                if (current.TagGroup == TagGroup.Bipd)
                    current.Drop();
                current = next;
            }

            // Now move all bipd's from this vehicle to the new one
            current = FirstCarried;
            while (current != null)
            {
                GameObject next = current.NextCarried;
                if (current.TagGroup == TagGroup.Bipd)
                    newObj.PickUpObject(current);
                current = next;
            }

            if (newObj.Carrier != null && newObj.Carrier.TagGroup == TagGroup.Vehi)
            {
                // The new vehicle is a turret, so hack around the base ReplaceWith and just delete us
                Delete(deleteCarried);
            }
            else
            {
                base.ReplaceWith(newObj, deleteCarried);
            }

            // Adjust pointers
            VehicleObject newVehicle = newObj as VehicleObject;
            if (newVehicle != null)
            {
                newVehicle._driver = driver;
                newVehicle._controller = controller;
            }
        }

        private ushort _driverObjectId = 0xFFFF;
        private GameObject _driver = null;
        private ushort _controllerObjectId = 0xFFFF;
        private GameObject _controller = null;
    }

    /// <summary>
    /// Holds and manages campaign save data.
    /// </summary>
    public partial class CampaignSave
    {
        /// <summary>
        /// Returns a List containing all of the Objects in the save.
        /// Note that some entries in this list may be null if an object was deleted.
        /// </summary>
        /// <seealso cref="GameObject"/>
        /// <seealso cref="ReadObjects"/>
        public List<GameObject> Objects
        {
            get { return _objects; }
        }

        /// <summary>
        /// Reads the object list from a Chunk.
        /// </summary>
        /// <param name="reader">The SaveReader to read from.</param>
        /// <param name="chunk">The "object" chunk to read from.</param>
        private void ReadObjects(Liberty.SaveIO.SaveReader reader, Chunk chunk)
        {
            if (chunk.EntrySize != 16)
                throw new ArgumentException("The file format is invalid: bad object subentry size\r\nExpected 0x10 but got 0x" + chunk.EntrySize.ToString("X"));

            // Check that the object pool exists
            // 0x706F6F6C = 'pool' in hex
            reader.Seek(_objectPoolStart, SeekOrigin.Begin);
            if (reader.ReadUInt32() != 0x706F6F6C)
                throw new ArgumentException("The file format is invalid: the object pool is missing or is at the wrong offset");

            // Read each object entry
            for (uint i = 0; i < chunk.EntryCount; i++)
            {
                // Read the object entry
                ObjectEntry entry = new ObjectEntry(chunk, i);
                if (!entry.Exists)
                {
                    _objects.Add(null);
                    continue;
                }

                // Seek to the start of the object data
                reader.Seek(_objectPoolStart + entry.PoolOffset, SeekOrigin.Begin);

                // Process it based on object class
                GameObject obj;
                switch (entry.TagGroup)
                {
                    case TagGroup.Bipd:
                        obj = new BipedObject(reader, entry);
                        break;

                    case TagGroup.Vehi:
                        obj = new VehicleObject(reader, entry);
                        break;

                    case TagGroup.Weap:
                        obj = new WeaponObject(reader, entry);
                        break;

                    case TagGroup.Eqip:
                        obj = new EquipmentObject(reader, entry);
                        break;

                    default:
                        obj = new GameObject(reader, entry);
                        break;
                }
                _objects.Add(obj);

                if (obj != null)
                    _nextFreeObjectIndex = _objects.Count;
                if (obj.Next == null)
                    _lastPoolObject = obj;
            }

            // Resolve any object references inside each object
            foreach (GameObject obj in _objects)
            {
                if (obj != null)
                    obj.ResolveObjectRefs(_objects);
            }
            chunk.Close();
        }

        private List<GameObject> _objects = new List<GameObject>();
        private int _nextFreeObjectIndex = 0;
        private GameObject _lastPoolObject = null;
        private const long _objectPoolStart = 0x76844C;
    }
}
