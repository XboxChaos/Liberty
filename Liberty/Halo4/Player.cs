using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Liberty.SaveIO;
using Liberty.Blam;

namespace Liberty.Halo4
{
    public class Player
    {
        /// <summary>
        /// Constructs a new Player object containing player information.
        /// </summary>
        /// <param name="reader">The SaveReader to read from. It must be positioned at the start of the players table.</param>
        /// <param name="objectResolver">The IDatumIndexResolver to use to resolve object indices.</param>
        public Player(SaveReader reader, IDatumIndexResolver<GameObject> objectResolver)
        {
            // Read the first entry from the players table
            Table playerTable = new Table(reader);
            if(playerTable.Name != "players")
                throw new ArgumentException("Player info must be read from the \"players\" table. The save file may be corrupt.");
            playerTable.ReadEntries(reader, ProcessPlayer);

            _biped = objectResolver.ResolveIndex(_bipedIndex) as BipedObject;

            _bipedPrimaryWeapon = objectResolver.ResolveIndex(_bipedPrimaryWeaponIndex) as WeaponObject;
            _bipedSecondaryWeapon = objectResolver.ResolveIndex(_bipedSecondaryWeaponIndex) as WeaponObject;
            _bipedThirdWeapon = objectResolver.ResolveIndex(_bipedThirdWeaponIndex) as WeaponObject;
            _bipedFourthWeapon = objectResolver.ResolveIndex(_bipedFourthWeaponIndex) as WeaponObject;
        }

        /// <summary>
        /// The player's biped.
        /// </summary>
        public BipedObject Biped
        {
            get { return _biped; }
        }

        /// <summary>
        /// The player's Gamertag
        /// </summary>
        public string Gamertag
        {
            get { return _gamertag; }
        }

        /// <summary>
        /// The player's Service Tag
        /// </summary>
        public string ServiceTag
        {
            get { return _serviceTag; }
        }

        private bool ProcessPlayer(Table table, SaveReader reader, DatumIndex index, uint size, long offset)
        {
            if (!index.IsValid)
                return true;

            ReadPlayerInfo(reader, offset);

            return false;
        }

        private void ReadPlayerInfo(SaveReader reader, long baseOffset)
        {
            reader.SeekTo(baseOffset + PlayerBipedOffset);
            _bipedIndex = DatumIndex.ReadFrom(reader);

            reader.SeekTo(baseOffset + PlayerWeaponOffset);
            _bipedPrimaryWeaponIndex = DatumIndex.ReadFrom(reader);
            _bipedSecondaryWeaponIndex = DatumIndex.ReadFrom(reader);
            _bipedThirdWeaponIndex = DatumIndex.ReadFrom(reader);
            _bipedFourthWeaponIndex = DatumIndex.ReadFrom(reader);

            reader.Seek(baseOffset + 0xB0, System.IO.SeekOrigin.Begin);
            _gamertag = reader.ReadUTF16();

            reader.Seek(baseOffset + 0xF4, System.IO.SeekOrigin.Begin);
            _serviceTag = reader.ReadUTF16();
        }

        private DatumIndex _bipedIndex;
        private DatumIndex _bipedPrimaryWeaponIndex;
        private DatumIndex _bipedSecondaryWeaponIndex;
        private DatumIndex _bipedThirdWeaponIndex;
        private DatumIndex _bipedFourthWeaponIndex;

        private BipedObject _biped;
        private WeaponObject _bipedPrimaryWeapon;
        private WeaponObject _bipedSecondaryWeapon;
        private WeaponObject _bipedThirdWeapon;
        private WeaponObject _bipedFourthWeapon;
        private string _gamertag;
        private string _serviceTag;


        // Offsets
        private const int PlayerBipedOffset = 0x24;
        private const int PlayerWeaponOffset = 0x5C;
    }
}
