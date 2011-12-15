using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Liberty.Reach
{
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

        /// <summary>
        /// The object that is driving this vehicle.
        /// </summary>
        public GameObject Driver
        {
            get { return _driver; }
        }

        /// <summary>
        /// The object that is controlling this vehicle.
        /// </summary>
        public GameObject Controller
        {
            get { return _controller; }
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

        public override void Drop()
        {
            if (_driver == Carrier)
                _driver = null;
            if (_controller == Carrier)
                _controller = null;

            base.Drop();
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
                    newObj.PickUp(current);
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
}
