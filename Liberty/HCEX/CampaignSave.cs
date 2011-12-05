using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Liberty.SaveIO;

namespace Liberty.HCEX
{
    public class CampaignSave : ICampaignSave
    {
        /// <summary>
        /// Creates a new HCEX CampaignSave based off of data in a saves.cfg file.
        /// </summary>
        /// <param name="path">The path to the saves.cfg file to read from.</param>
        public CampaignSave(string path)
        {
            // TODO: find some clever way to store the path, or keep the file open while still allowing X360 to work.
            Stream fileStream = File.OpenRead(path);
            ReadFromStream(fileStream);
            fileStream.Close();
        }

        /// <summary>
        /// Creates a new HCEX CampaignSave based off of saves.cfg data in a stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        public CampaignSave(Stream stream)
        {
            ReadFromStream(stream);
        }

        /// <summary>
        /// Writes any changes made back to the original saves.cfg file.
        /// </summary>
        /// <param name="path">The path to the original file</param>
        public void Update(string path)
        {
            Stream fileStream = File.Open(path, FileMode.Open, FileAccess.ReadWrite);
            WriteToStream(fileStream);
            fileStream.Close();
        }

        /// <summary>
        /// Writes any changes made back to the original stream.
        /// </summary>
        /// <param name="stream">The original stream</param>
        public void Update(Stream stream)
        {
            WriteToStream(stream);
        }

        /// <summary>
        /// The parsed CFG data that appears in the header of the file
        /// </summary>
        public SaveCFG CFGData
        {
            get { return _fileHeader.CFGData; }
        }

        /// <summary>
        /// The resource name of the current map.
        /// </summary>
        public string Map
        {
            get { return _saveHeader.Map; }
        }

        /// <summary>
        /// The list of objects loaded in the save.
        /// </summary>
        public ObjectList Objects
        {
            get { return _objectList; }
        }

        /// <summary>
        /// The player's biped.
        /// </summary>
        public BipedObject PlayerBiped
        {
            get { return _player.Biped; }
        }

        private void ReadFromStream(Stream stream)
        {
            _fileHeader.ReadFrom(new SaveReader(stream));

            // Construct an OffsetStream which offsets everything relative to the start of the actual save data
            OffsetStream offsetStream = new OffsetStream(stream, stream.Length - SaveDataSize);
            offsetStream.Seek(0, SeekOrigin.Begin);

            // Now read the save header
            SaveIO.SaveReader reader = new SaveReader(offsetStream);
            _saveHeader.ReadFrom(reader);

            // Read the object list
            reader.SeekTo((long)TableOffset.Object);
            _objectList = new ObjectList(reader);

            // Read player info
            reader.SeekTo((long)TableOffset.Players);
            _player = new Player(reader, _objectList);
        }

        private void WriteToStream(Stream stream)
        {
            SaveWriter writer = new SaveWriter(stream);
            _fileHeader.WriteTo(writer);
        }

        private FileHeader _fileHeader;
        private SaveHeader _saveHeader;

        private ObjectList _objectList;
        private Player _player;

        private const int SaveDataSize = 0x40A000;
    }
}
