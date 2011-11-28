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

        private void ReadFromStream(Stream stream)
        {
            SaveReader reader = new SaveReader(stream);
            _header.ReadFrom(reader);
        }

        private void WriteToStream(Stream stream)
        {
            SaveWriter writer = new SaveWriter(stream);
            _header.WriteTo(writer);
        }

        private SaveHeader _header;
    }
}
