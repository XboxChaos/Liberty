using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Liberty.IO;
using Liberty.SaveManager.Games.Blam;

namespace Liberty.SaveManager.Games.Halo3
{
    public class Player
    {
        /// <summary>
        /// Constructs a new Player object containing player information.
        /// </summary>
        /// <param name="reader">The EndianReader to read from. It must be positioned at the start of the players table.</param>
        /// <param name="objectResolver">The IDatumIndexResolver to use to resolve object indices.</param>
        public Player(EndianReader reader, IDatumIndexResolver<GameObject> objectResolver)
        {
            // Read the first entry from the players table
            Table playerTable = new Table(reader);
            if(playerTable.Name != "players")
                throw new ArgumentException("Player info must be read from the \"players\" table. The save file may be corrupt.");
            playerTable.ReadEntries(reader, ProcessPlayer);

            _biped = objectResolver.ResolveIndex(_bipedIndex) as BipedObject;
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

        private bool ProcessPlayer(Table table, EndianReader reader, DatumIndex index, uint size, long offset)
        {
            if (!index.IsValid)
                return true;

            ReadPlayerInfo(reader, offset);

            return false;
        }

        private void ReadPlayerInfo(EndianReader reader, long baseOffset)
        {
            reader.SeekTo(baseOffset + PlayerBipedOffset);
            _bipedIndex = DatumIndex.ReadFrom(reader);

            reader.SeekTo(baseOffset + 0x50);
            _gamertag = reader.ReadUTF16();

            reader.SeekTo(baseOffset + 0x86);
            _serviceTag = reader.ReadUTF16();
        }

        private DatumIndex _bipedIndex;
        private BipedObject _biped;
        private string _gamertag;
        private string _serviceTag;


        // Offsets
        private const int PlayerBipedOffset = 0x28;
    }
}
