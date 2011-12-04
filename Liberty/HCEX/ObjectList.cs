using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Liberty.SaveIO;
using Liberty.Blam;

namespace Liberty.HCEX
{
    public class ObjectList : ICollection<GameObject>, IDatumIndexResolver<GameObject>
    {
        /// <summary>
        /// Creates a new ObjectList and reads it from a SaveReader.
        /// </summary>
        /// <param name="reader">The SaveReader to read from. It must be positioned at the start of the object table.</param>
        public ObjectList(SaveReader reader)
        {
            Table objectTable = new Table(reader);
            if (objectTable.Name != "object")
                throw new ArgumentException("The object list must be read from the \"object\" table. The save file may be corrupt.");
            _entryTableOffset = reader.Position;
            objectTable.ReadEntries(reader, ProcessObject);

            // Resolve datum indices
            // This needs to be done as a separate step since objects can refer to other objects that come later in the file.
            foreach (GameObject obj in _objects)
            {
                if (obj != null)
                    obj.ResolveDatumIndices(this);
            }
        }

        /// <summary>
        /// Looks up an object from an integer index.
        /// </summary>
        /// <param name="i">The object's index into the list.</param>
        /// <returns>The GameObject at the specified index.</returns>
        public GameObject this[int i]
        {
            get { return _objects[i]; }
        }

        /// <summary>
        /// Looks up an object from a ushort index.
        /// </summary>
        /// <param name="i">The object's index into the list.</param>
        /// <returns>The GameObject at the specified index.</returns>
        public GameObject this[ushort i]
        {
            get { return _objects[(int)i]; }
        }

        /// <summary>
        /// Looks up an object from a datum index.
        /// </summary>
        /// <param name="index">The datum index to look up.</param>
        /// <returns>The GameObject with the specified datum index.</returns>
        public GameObject this[DatumIndex index]
        {
            get
            {
                if (index.Index > _objects.Count)
                    throw new IndexOutOfRangeException("Datum index is invalid or is for a different table");

                return this[(int)index.Index];
            }
        }

        private bool ProcessObject(Table table, SaveReader reader, DatumIndex index, uint size, long offset)
        {
            if (!index.IsValid)
            {
                _objects.Add(null);
                return true;
            }

            // Read the ObjectEntry and seek to the object's file offset
            ObjectEntry entry = new ObjectEntry(index, reader);
            long fileOffset = entry.ObjectAddress - table.Address + _entryTableOffset;
            reader.SeekTo(fileOffset);

            // Construct a specialized GameObject class depending on the tag group
            GameObject obj;
            switch (entry.TagGroup)
            {
                case TagGroup.Bipd:
                    obj = new BipedObject(entry, reader);
                    break;

                case TagGroup.Weap:
                    obj = new WeaponObject(entry, reader);
                    break;

                default:
                    obj = new GameObject(entry, reader);
                    break;
            }
            _objects.Add(obj);
            return true;
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public void Add(GameObject item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public void Clear()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns whether or not a GameObject belongs to this ObjectList.
        /// </summary>
        /// <param name="item">The GameObject to search for.</param>
        /// <returns>Whether or not the GameObject is in this ObjectList.</returns>
        public bool Contains(GameObject item)
        {
            return _objects.Contains(item);
        }

        /// <summary>
        /// Copies the object list into a GameObject array.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The index in the target array to start copying to.</param>
        public void CopyTo(GameObject[] array, int arrayIndex)
        {
            _objects.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns how large the object list is.
        /// </summary>
        public int Count
        {
            get { return _objects.Count; }
        }

        /// <summary>
        /// Whether or not this is read-only. Always returns true.
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public bool Remove(GameObject item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns an IEnumerator that can be used to enumerate the object list.
        /// Note that some objects can be null!
        /// </summary>
        /// <returns>An IEnumerator for enumerating the object list.</returns>
        public IEnumerator<GameObject> GetEnumerator()
        {
            return _objects.GetEnumerator();
        }

        /// <summary>
        /// Returns an IEnumerator that can be used to enumerate the object list.
        /// Note that some objects can be null!
        /// </summary>
        /// <returns>An IEnumerator for enumerating the object list.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _objects.GetEnumerator();
        }

        /// <summary>
        /// Resolves a datum index to a GameObject.
        /// </summary>
        /// <param name="index">The datum index to resolve.</param>
        /// <returns>The GameObject if found, or null otherwise.</returns>
        public GameObject ResolveIndex(DatumIndex index)
        {
            if (index.Index > _objects.Count)
                return null;

            return this[(int)index.Index];
        }

        private List<GameObject> _objects = new List<GameObject>();
        private long _entryTableOffset;
    }
}
