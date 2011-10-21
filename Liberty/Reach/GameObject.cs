/*
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

		public void Delete()
		{
			Size = 0xFFFFFFFF;
			ID = 0xFFFFFFFF;
			NextLinkOffset = 0xFFFFFFFF;
			PreviousLinkOffset = 0xFFFFFFFF;
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
            _dataPosition = reader.Position;

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
			_entry.Delete();

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

			_linkData.Delete();
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
        /// The ID of the object as used in the .map file.
        /// </summary>
        public uint MapID
        {
            get { return _mapId; }
        }

        public uint LoadAddress
        {
            get { return _entry.LoadAddress; }
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
            set { _x = value; }
        }

        /// <summary>
        /// The object's Y position in 3D space.
        /// </summary>
        public float Y
        {
            get { return _y; }
            set { _y = value; }
        }

        /// <summary>
        /// The object's Z position in 3D space.
        /// </summary>
        public float Z
        {
            get { return _z; }
            set { _z = value; }
        }

        /// <summary>
        /// The object's right vector, converted to a right-handed system.
        /// </summary>
        public MathUtil.Vector3 Right
        {
            get { return _right; }
            set { _right = value; }
        }

        /// <summary>
        /// The object's forward vector, converted to a right-handed system.
        /// </summary>
        public MathUtil.Vector3 Forward
        {
            get { return _forward; }
            set { _forward = value; }
        }

        /// <summary>
        /// The object's up vector, converted to a right-handed system.
        /// </summary>
        public MathUtil.Vector3 Up
        {
            get { return _up; }
            set { _up = value; }
        }

        public uint Size
        {
            get { return _linkData.Size; }
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
            get
            {
                return ((_flags & ObjectFlags.Active) == ObjectFlags.Active);
            }
            set
            {
                if (value)
                    _flags |= ObjectFlags.Active;
                else
                    _flags &= ~ObjectFlags.Active;
            }
        }

        public NodeCollection Nodes
        {
            get { return _nodes; }
        }

        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
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
            _mapId = reader.ReadUInt32();
            
            // Flags
            _flags = reader.ReadUInt32();

            // Zone?
            _zone = (ushort)((reader.ReadUInt32() & 0xFFFF0000) >> 16);

            // Carry info
            _nextCarriedId = (ushort)(reader.ReadUInt32() & 0xFFFF);
            _firstCarriedId = (ushort)(reader.ReadUInt32() & 0xFFFF);
            _carrierId = (ushort)(reader.ReadUInt32() & 0xFFFF);

            // Position data
            reader.Seek(start + 0x20, SeekOrigin.Begin);
            _boundsX1 = reader.ReadFloat();
            _boundsY1 = reader.ReadFloat();
            _boundsZ1 = reader.ReadFloat();
            reader.Seek(4, SeekOrigin.Current);
            _boundsX2 = reader.ReadFloat();
            _boundsY2 = reader.ReadFloat();
            _boundsZ2 = reader.ReadFloat();
            reader.Seek(8, SeekOrigin.Current);
            _x = reader.ReadFloat();
            _y = reader.ReadFloat();
            _z = reader.ReadFloat();

            // Rotation data
            _right.X = reader.ReadFloat();
            _right.Y = -reader.ReadFloat(); // hax
            _right.Z = reader.ReadFloat();
            _up.X = reader.ReadFloat();
            _up.Y = -reader.ReadFloat();    // hax
            _up.Z = reader.ReadFloat();

            // Calculate the forward vector with a cross product
            _forward = MathUtil.Vector3.Cross(_up, _right);

            // Scale
            reader.Seek(start + 0x80, SeekOrigin.Begin);
            _scale = reader.ReadFloat();

            // Flags
            reader.Seek(start + 0xD8, SeekOrigin.Begin);
            _physicsFlags = reader.ReadUInt64();

            // Node data
            reader.Seek(start + 0x17C, SeekOrigin.Begin);
            ushort _nodeDataSize = reader.ReadUInt16();
            _nodeDataOffset = reader.ReadUInt16();

            if (_nodeDataOffset != 0xFFFF)
            {
                reader.Seek(start + _nodeDataOffset, SeekOrigin.Begin);

                // Read all of the nodes into a list
                List<ModelNode> nodes = new List<ModelNode>();
                for (ushort i = 0; i < _nodeDataSize; i += ModelNode.DataSize)
                    nodes.Add(ModelNode.FromSaveReader(reader));

                // Now make a NodeCollection out of them
                _nodes = new NodeCollection(nodes);
            }

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
            writer.WriteUInt16(_zone);
            writer.Seek(2, SeekOrigin.Current);
            
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
            writer.WriteFloat(_x);
            writer.WriteFloat(_y);
            writer.WriteFloat(_z);
            /*writer.WriteFloat(_boundsX1);
            writer.WriteFloat(_boundsY1);
            writer.WriteFloat(_boundsZ1);*/
            writer.Seek(4, SeekOrigin.Current);
            writer.WriteFloat(_x);
            writer.WriteFloat(_y);
            writer.WriteFloat(_z);
            /*writer.WriteFloat(_boundsX2);
            writer.WriteFloat(_boundsY2);
            writer.WriteFloat(_boundsZ2);*/
            writer.Seek(8, SeekOrigin.Current);
            writer.WriteFloat(_x);
            writer.WriteFloat(_y);
            writer.WriteFloat(_z);

            writer.WriteFloat(_right.X);
            writer.WriteFloat(-_right.Y);   // hax
            writer.WriteFloat(_right.Z);
            writer.WriteFloat(_up.X);
            writer.WriteFloat(-_up.Y);  // hax
            writer.WriteFloat(_up.Z);

            writer.Seek(start + 0x80, SeekOrigin.Begin);
            // Scale is stored twice??
            writer.WriteFloat(_scale);
            writer.WriteFloat(_scale);

            writer.Seek(start + 0xD8, SeekOrigin.Begin);
            writer.WriteUInt64(_physicsFlags);

            if (_nodeDataOffset != 0xFFFF)
            {
                writer.Seek(start + _nodeDataOffset, SeekOrigin.Begin);
                foreach (ModelNode node in _nodes)
                    node.Update(writer, this);
            }
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
            obj._zone = 0xFFFF;
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

        private uint _mapId;
        private uint _flags;
        private ushort _zone;

        private ushort _carrierId;
        private GameObject _carrier = null;
        private ushort _firstCarriedId;
        private GameObject _firstCarried = null;
        private ushort _nextCarriedId;
        private GameObject _nextCarried = null;

        private float _boundsX1, _boundsY1, _boundsZ1;
        private float _boundsX2, _boundsY2, _boundsZ2;
        private float _x, _y, _z;
        private MathUtil.Vector3 _right;
        private MathUtil.Vector3 _forward;
        private MathUtil.Vector3 _up;

        private ulong _physicsFlags;

        private float _scale;

        private ushort _previousObjectId = 0xFFFF;
        private GameObject _previousObject = null;
        private ushort _nextObjectId = 0xFFFF;
        private GameObject _nextObject = null;

        private ushort _nodeDataOffset = 0xFFFF;
        private NodeCollection _nodes = null;
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
        private void ReadObjects(SaveIO.SaveReader reader, Chunk chunk)
        {
            if (chunk.EntrySize != 16)
                throw new ArgumentException("The file format is invalid: bad object entry size\r\nExpected 0x10 but got 0x" + chunk.EntrySize.ToString("X"));

            // Check that the object pool exists
            // 0x706F6F6C = 'pool' in hex
            reader.Seek(_objectPoolStart, SeekOrigin.Begin);
            if (reader.ReadUInt32() != 0x706F6F6C)
                throw new ArgumentException("The file format is invalid: the object pool is missing or is at the wrong offset");
            
            // Process the entries
            _objectChunk = chunk;
            chunk.EnumEntries(reader, ProcessObjectEntry);

            // Resolve any object references inside each object
            foreach (GameObject obj in _objects)
            {
                if (obj != null)
                    obj.ResolveObjectRefs(_objects);
            }

            // Convert all of the nodes to use relative coordinates
            // Object references need to be resolved before this occurs
            foreach (GameObject obj in _objects)
            {
                if (obj != null)
                    obj.Nodes.MakeRelative(obj);
            }
        }

        private bool ProcessObjectEntry(SaveIO.SaveReader reader, bool active, uint datumIndex, ushort flags, uint size, long offset)
        {
            if (!active)
            {
                _objects.Add(null);
                return true;
            }

            // Read the object entry
            ObjectEntry entry = new ObjectEntry(_objectChunk, reader, datumIndex, flags, offset);

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

            if (obj.Next == null)
                _lastPoolObject = obj;

            return true;
        }

        private List<GameObject> _objects = new List<GameObject>();
        private GameObject _lastPoolObject = null;
        private const long _objectPoolStart = 0x76844C;
        private Chunk _objectChunk = null;
    }
}
