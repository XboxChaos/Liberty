using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Liberty.SaveIO;
using System.IO;

namespace Liberty.Halo3ODST
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
            if (header[0] != 0x4C || header[1] != 0x7D || header[2] != 0xEB || header[3] != 0x9F)
                throw new ArgumentException("The file format is invalid: bad header\r\nShould be 4C 7D EB 9F");

            if (reader.Length != 0x007F0000)
                throw new ArgumentException("The file format is invalid: incorrect file size\r\nExpected 0x7F0000 but got 0x" + reader.Length.ToString("X"));

            // Read Map Scenario
            reader.Seek(8, System.IO.SeekOrigin.Begin);
            _mapScenario = reader.ReadAscii();

            // Read Engine Build
            reader.Seek(0x108, System.IO.SeekOrigin.Begin);
            _engineBuild = reader.ReadAscii();

            // Read Disk Map Location
            reader.Seek(0x154, System.IO.SeekOrigin.Begin);
            _mapDiskLocation = reader.ReadAscii();

            // Read Difficulty
            reader.Seek(0x26D, SeekOrigin.Begin);
            _difficulty = (Difficulty)reader.ReadByte();
            switch (_difficulty)
            {
                case Halo3ODST.Difficulty.Easy:
                    _difficultyName = "Easy";
                    break;
                case Halo3ODST.Difficulty.Normal:
                    _difficultyName = "Normal";
                    break;
                case Halo3ODST.Difficulty.Heroic:
                    _difficultyName = "Heroic";
                    break;
                case Halo3ODST.Difficulty.Legendary:
                    _difficultyName = "Legendary";
                    break;
                default:
                    _difficultyName = "Normal";
                    break;
            }

            // Read Gamertag
            reader.Seek(0xE790, SeekOrigin.Begin);
            _gamertag = reader.ReadUTF16();

            // Read ServiceTag
            reader.Seek(0xE7C6, SeekOrigin.Begin);
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
            //writer.Seek(0x26C, SeekOrigin.Begin);
            //writer.WriteByte((byte)_difficulty);

            //// Write Gamertag1
            //writer.Seek(0xE790, System.IO.SeekOrigin.Begin);
            //writer.WriteUTF16(_gamertag);

            //// Write ServiceTag
            //writer.Seek(0xE7C6, System.IO.SeekOrigin.Begin);
            //writer.WriteUTF16(_serviceTag);

            //// Write Gamertag2
            //writer.Seek(0xE990, System.IO.SeekOrigin.Begin);
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
            memoryStream.Position = 0xF138;
            memoryStream.Write(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0, 20);
            byte[] hash = Security.SaveSHA1.ComputeHash(memoryStream.GetBuffer());

            // Write the new digest
            stream.Position = 0xF138;
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
        /// The Location of the Map on the disk.
        /// </summary>
        public string DiskMapLocation { get { return _mapDiskLocation; } set { _mapDiskLocation = value; } }

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
        private string _mapDiskLocation;

        private Difficulty _difficulty;
        private string _difficultyName;
        private string _gamertag;
        private string _serviceTag;
    }
}
