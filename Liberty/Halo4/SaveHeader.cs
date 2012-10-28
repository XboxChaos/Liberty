using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Liberty.SaveIO;

namespace Liberty.Halo4
{
    public struct SaveHeader
    {
        /// <summary>
        /// Read the header from the save
        /// </summary>
        /// <param name="reader">The SaveReader stream of the Save</param>
        /// <seealso cref="SaveIO.SaveReader"/>
        public void ReadFrom(SaveReader reader)
        {
            // Verify Valid Container
            byte[] header = new byte[4];
            reader.Seek(0, System.IO.SeekOrigin.Begin);
            reader.ReadBlock(header, 0, 4);
            if (header[0] != 0xAA || header[1] != 0x46 || header[2] != 0xAF || header[3] != 0x2F)
                throw new ArgumentException("The file format is invalid: bad header\r\nShould be 4E B2 C1 86");

            if (reader.Length != 0xAD0000)
                throw new ArgumentException("The file format is invalid: incorrect file size\r\nExpected 0xAD0000 but got 0x" + reader.Length.ToString("X"));

            // Read Map Scenario
            reader.Seek(8, SeekOrigin.Begin);
            _mapScenario = reader.ReadAscii();
            
            // Read Engine Build
            reader.Seek(0x108, SeekOrigin.Begin);
            _engineBuild = reader.ReadAscii();
            
            // Read Engine Map Location
            reader.Seek(0x1C964, SeekOrigin.Begin);
            _engineMapLocation = reader.ReadAscii();

            // Read Difficulty
            reader.Seek(0x1CB03, SeekOrigin.Begin);
            _difficulty = (Difficulty)reader.ReadByte();
            switch (_difficulty)
            {
                case Halo4.Difficulty.Easy:
                    _difficultyName = "Easy";
                    break;
                case Halo4.Difficulty.Normal:
                    _difficultyName = "Normal";
                    break;
                case Halo4.Difficulty.Heroic:
                    _difficultyName = "Heroic";
                    break;
                case Halo4.Difficulty.Legendary:
                    _difficultyName = "Legendary";
                    break;
                default:
                    _difficultyName = "Normal";
                    break;
            }

            // Read Gamertag
            reader.Seek(0x2B438, SeekOrigin.Begin);
            _gamertag = reader.ReadUTF16();

            // Read ServiceTag
            reader.Seek(0x2B47C, SeekOrigin.Begin);
            _serviceTag = reader.ReadUTF16();
        }

        /// <summary>
        /// Write the header of the save
        /// </summary>
        /// <param name="writer">The SaveWriter stream of the save.</param>
        /// <seealso cref="SaveIO.SaveWriter"/>
        public void WriteTo(SaveWriter writer)
        {
            //// Write Map Scenario
            //writer.Seek(8, System.IO.SeekOrigin.Begin);
            //writer.WriteASCII(_mapScenario, 0x100);

            //// Write Engine Build
            //writer.Seek(0x108, System.IO.SeekOrigin.Begin);
            //writer.WriteASCII(_engineBuild, 0x20);

            //// Write Disk Map Location
            //writer.Seek(0x154, System.IO.SeekOrigin.Begin);
            //writer.WriteASCII(_mapDiskLocation, 0x25);

            //// Write Difficulty
            //writer.Seek(0xE72B, SeekOrigin.Begin);
            //writer.WriteByte((byte)_difficulty);

            //// Write Gamertag1
            //writer.Seek(0xE6D8, System.IO.SeekOrigin.Begin);
            //writer.WriteUTF16(_gamertag);

            //// Write ServiceTag
            //writer.Seek(0xE70E, System.IO.SeekOrigin.Begin);
            //writer.WriteUTF16(_serviceTag);

            //// Write Gamertag2
            //writer.Seek(0xE7A0, System.IO.SeekOrigin.Begin);
            //writer.WriteUTF16(_gamertag);
        }

        /// <summary>
        /// Fix the SHA-1 Salted hash in the header of the save.
        /// </summary>
        /// <param name="writer">The SaveWriter stream of the save.</param>
        /// <param name="stream">An Open Stream of the file</param>
        /// <seealso cref="Security.SaveSHA1"/>
        /// <seealso cref="SaveIO.SaveWriter"/>
        public void Resign(SaveWriter writer, Stream stream)
        {
            // Load the whole stream into memory
            MemoryStream memoryStream = new MemoryStream((int)stream.Length);
            memoryStream.SetLength(stream.Length);
            stream.Position = 0;
            stream.Read(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);

            // Hash the contents
            memoryStream.Position = 0x2D25C;
            memoryStream.Write(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0, 20);
            byte[] hash = Security.SaveSHA1.ComputeHash(memoryStream.GetBuffer());

            // Write the new digest
            stream.Position = 0x2D25C;
            stream.Write(hash, 0, 20);
        }

        #region delcarations
        /// <summary>
        /// The Save's Map Scenario Name.
        /// </summary>
        public string Map { get { return _mapScenario; } set { _mapScenario = value; } }

        /// <summary>
        /// The Save's Engine Build.
        /// </summary>
        public string EngineBuild { get { return _engineBuild; } set { _engineBuild = value; } }

        /// <summary>
        /// The Save's Map Location.
        /// </summary>
        public string MapLocation { get { return _engineMapLocation; } set { _engineMapLocation = value; } }
        
        /// <summary>
        /// The difficulty of the save
        /// </summary>
        public Difficulty Difficulty { get { return _difficulty; } set { _difficulty = value; } }

        /// <summary>
        /// The User Friendly difficulty of the save
        /// </summary>
        public string DifficultyString { get { return _difficultyName; } set { _difficultyName = value; } }

        /// <summary>
        /// The Gamertag of the owner of the save.
        /// </summary>
        public string Gamertag { get { return _gamertag; } set { _gamertag = value; } }

        /// <summary>
        /// The Service Tag of the owner of the save.
        /// </summary>
        public string ServiceTag { get { return _serviceTag; } set { _serviceTag = value; } }
        #endregion

        private string _mapScenario;
        private string _engineBuild;
        private string _engineMapLocation;

        private Difficulty _difficulty;
        private string _difficultyName;
        private string _gamertag;
        private string _serviceTag;

    }
}
