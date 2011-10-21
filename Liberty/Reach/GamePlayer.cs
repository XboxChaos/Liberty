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
    /// <summary>
    /// Describes player data.
    /// </summary>
    public class GamePlayer
    {
        internal GamePlayer(SaveIO.SaveReader reader, List<GameObject> objectList, uint datumIndex, long offset)
        {
            _dataPosition = offset;

            _id = datumIndex;

            // Find the biped object
            reader.Seek(offset + 0x2A, SeekOrigin.Begin);
            ushort bipedId = reader.ReadUInt16();
            _biped = (BipedObject)objectList[(int)bipedId];

            // Read gamertag
            reader.Seek(offset + 0xB0, SeekOrigin.Begin);
            _gamertag = reader.ReadUTF16();

            // Read service tag
            reader.Seek(offset + 0xF4, SeekOrigin.Begin);
            _serviceTag = reader.ReadUTF16();
        }

        /// <summary>
        /// Helper function for changing the player's biped.
        /// </summary>
        /// <param name="newBiped">The new biped to use.</param>
        public void ChangeBiped(BipedObject newBiped, bool transferWeapons)
        {
            if (newBiped != _biped && newBiped != null && !newBiped.Deleted)
            {
                if (transferWeapons)
                {
                    if (newBiped.PrimaryWeapon != null)
                        newBiped.PrimaryWeapon.Delete();
                    if (newBiped.SecondaryWeapon != null)
                        newBiped.SecondaryWeapon.Delete();
                    if (newBiped.ArmorAbility != null)
                        newBiped.ArmorAbility.Delete();

                    newBiped.PrimaryWeapon = _biped.PrimaryWeapon;
                    newBiped.SecondaryWeapon = _biped.SecondaryWeapon;
                    newBiped.ArmorAbility = _biped.ArmorAbility;
                    newBiped.FragGrenades = _biped.FragGrenades;
                    newBiped.PlasmaGrenades = _biped.PlasmaGrenades;
                }
                /*newBiped.Drop();
                _biped.Delete(true);*/
                _biped.ReplaceWith(newBiped, true);
                _biped = newBiped;
            }
        }

        internal void ResolvePlayerRefs()
        {
            _biped.Player = this;
            _biped.Actor = 0xFFFFFFFF;
            if (_biped.ArmorAbility != null)
                _biped.ArmorAbility.Player = this;
        }

        internal void Update(SaveIO.SaveWriter writer)
        {
            writer.Seek(_dataPosition + 0x28, SeekOrigin.Begin);
            writer.WriteUInt32(_biped.ID);

            writer.Seek(_dataPosition + 0x34, SeekOrigin.Begin);
            writer.WriteUInt32(_biped.ID);

            writer.Seek(_dataPosition + 0x5C, SeekOrigin.Begin);
            if (PrimaryWeapon != null)
                writer.WriteUInt32(PrimaryWeapon.ID);
            else
                writer.WriteUInt32(0xFFFFFFFF);
            if (SecondaryWeapon != null)
                writer.WriteUInt32(SecondaryWeapon.ID);
            else
                writer.WriteUInt32(0xFFFFFFFF);
        }

        /// <summary>
        /// The player's ID.
        /// </summary>
        public uint ID
        {
            get { return _id; }
        }

        /// <summary>
        /// The player's biped object.
        /// </summary>
        public BipedObject Biped
        {
            get { return _biped; }
            set { _biped = value; }
        }

        /// <summary>
        /// The player's primary weapon object (can be null!).
        /// </summary>
        public WeaponObject PrimaryWeapon
        {
            get { return _biped.PrimaryWeapon; }
            set { _biped.PrimaryWeapon = value; }
        }

        /// <summary>
        /// The player's secondary weapon object (can be null!).
        /// </summary>
        public WeaponObject SecondaryWeapon
        {
            get { return _biped.SecondaryWeapon; }
            set { _biped.SecondaryWeapon = value; }
        }

        /// <summary>
        /// The player's armor ability (can be null!).
        /// </summary>
        public EquipmentObject ArmorAbility
        {
            get { return _biped.ArmorAbility; }
            set { _biped.ArmorAbility = value; }
        }

        /// <summary>
        /// The player's gamertag.
        /// </summary>
        public string Gamertag
        {
            get { return _gamertag; }
        }

        /// <summary>
        /// The player's service tag.
        /// </summary>
        public string ServiceTag
        {
            get { return _serviceTag; }
        }

        /// <summary>
        /// The player's X position in 3D space.
        /// </summary>
        public float X
        {
            get { return _biped.X; }
            set { _biped.X = value; }
        }

        /// <summary>
        /// The player's Y position in 3D space.
        /// </summary>
        public float Y
        {
            get { return _biped.Y; }
            set { _biped.Y = value; }
        }

        /// <summary>
        /// The player's Z position in 3D space.
        /// </summary>
        public float Z
        {
            get { return _biped.Z; }
            set { _biped.Z = value; }
        }

        private long _dataPosition;
        private uint _id;
        private BipedObject _biped = null;
        /*private WeaponObject _primaryWeapon = null;
        private WeaponObject _secondaryWeapon = null;*/
        private string _gamertag;
        private string _serviceTag;
    }

    public partial class CampaignSave
    {
        /// <summary>
        /// Reads player info from a "players" chunk.
        /// </summary>
        /// <param name="reader">The SaveReader to read from.</param>
        /// <param name="chunk">The "players" chunk to read from.</param>
        private void ReadPlayers(SaveIO.SaveReader reader, Chunk chunk)
        {
            /*if (chunk.EntryCount < 1)
                throw new ArgumentException("The file format is invalid: player data is missing");*/
            if (chunk.EntrySize != 0x578)
                throw new ArgumentException("The file format is invalid: bad player entry size\r\nExpected 0x578 but got 0x" + chunk.EntrySize.ToString("X"));

            chunk.EnumEntries(reader, ProcessPlayer);
        }

        private bool ProcessPlayer(SaveIO.SaveReader reader, bool active, uint datumIndex, ushort flags, uint size, long offset)
        {
            _player = new GamePlayer(reader, _objects, datumIndex, offset);

            // Return false so we only read the first player
            return false;
        }
    }
}
