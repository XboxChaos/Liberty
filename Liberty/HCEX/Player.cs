using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Liberty.SaveIO;
using Liberty.Blam;

namespace Liberty.HCEX
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
            if (playerTable.Name != "players")
                throw new ArgumentException("Player info must be read from the \"players\" table. The save file may be corrupt.");
            playerTable.ReadEntries(reader, ProcessPlayer);

            // Resolve the biped datum index
            _biped = objectResolver.ResolveIndex(_bipedIndex) as BipedObject;
        }

        /// <summary>
        /// The player's biped.
        /// </summary>
        public BipedObject Biped
        {
            get { return _biped; }
        }

        private bool ProcessPlayer(Table table, SaveReader reader, DatumIndex index, uint size, long offset)
        {
            if (!index.IsValid)
                return true;

            ReadPlayerInfo(reader, offset);

            // Return false, we only need the first active entry
            return false;
        }

        private void ReadPlayerInfo(SaveReader reader, long baseOffset)
        {
            reader.SeekTo(baseOffset + PlayerBipedOffset);
            _bipedIndex = DatumIndex.ReadFrom(reader);
        }

        private DatumIndex _bipedIndex;
        private BipedObject _biped;

        // Offsets
        private const int PlayerBipedOffset = 0x34;
    }
}
