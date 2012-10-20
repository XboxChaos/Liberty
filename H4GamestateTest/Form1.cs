using H4GamestateTest.IO;
using Liberty.SaveIO;
using Liberty.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H4GamestateTest
{
    public partial class Form1 : Form
    {
        string iniFolderPath;
        Liberty.classInfo.iniFile trueTaglist;
        SaveReader streamReader;
        MemoryStream _stream;
        SaveWriter streamWriter;
        GamestateHeader gamestateHeader = new GamestateHeader();
        IList<LinkObjectTable> linkObjects = new List<LinkObjectTable>();
        IList<H4GameObject> gameObjects = new List<H4GameObject>();

        int[] IdentParents = new int[137];

        /// <summary>
        /// Provides constants for known object tag groups. (from Project Liberty)
        /// </summary>
        public enum TagGroup : ushort
        {
            Bipd = 0,
            Vehi = 1,
            Weap = 2,
            Eqip = 3,
            Term = 4,
            Proj = 5,
            Scen = 6,
            Mach = 7,
            Ctrl = 8,
            Ssce = 9,
            Bloc = 10,
            Crea = 11,
            Unk1 = 12,
            Efsc = 13,

            Unknown = 255    // Not actually a valid value, this is just for use as a placeholder.
        }

        public Form1()
        {
            InitializeComponent();

#if DEBUG
            iniFolderPath = @"C:\Users\rape_000\Desktop\";
#endif

            toolStripStatusLabel2.Text = "Load Halo 4 Gamestate Gamesave...";
            listView1.ColumnClick += new ColumnClickEventHandler(listView1_ColumnClick);
        }
        void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            NativeWinRT.ListViewSorter Sorter = new NativeWinRT.ListViewSorter();
            listView1.ListViewItemSorter = Sorter;
            if (!(listView1.ListViewItemSorter is NativeWinRT.ListViewSorter))
                return;
            Sorter = (NativeWinRT.ListViewSorter)listView1.ListViewItemSorter;

            if (Sorter.LastSort == e.Column)
            {
                if (listView1.Sorting == SortOrder.Ascending)
                    listView1.Sorting = SortOrder.Descending;
                else
                    listView1.Sorting = SortOrder.Ascending;
            }
            else
            {
                listView1.Sorting = SortOrder.Descending;
            }
            Sorter.ByColumn = e.Column;
        }

        void loadSave()
        {
            // Load into Stream
            _stream = new MemoryStream(File.ReadAllBytes(textBox1.Text));

            // Load into Aaron's nice Liberty IO
            streamReader = new SaveReader(_stream);

            // Verify Valid Container
            byte[] header = new byte[4];
            streamReader.Seek(0, SeekOrigin.Begin);
            streamReader.ReadBlock(header, 0, 4);
            if (header.Equals(new byte[] { 0xAA, 0x46, 0xAF, 0x2F }))
                throw new ArgumentException("The file format is invalid: bad header\r\nShould be 4E B2 C1 86");

            if (streamReader.Length != 0xAD0000)
                throw new ArgumentException("The file format is invalid: incorrect file size\r\nExpected 0xAD0000 but got 0x" + streamReader.Length.ToString("X"));

            // Load Header
            streamReader.Seek(0x08, SeekOrigin.Begin);
            gamestateHeader.mapScenarioName = streamReader.ReadAscii();
            streamReader.Seek(0x108, SeekOrigin.Begin);
            gamestateHeader.relativeEngineBuild = streamReader.ReadAscii();
            streamReader.Seek(0x1C964, SeekOrigin.Begin);
            gamestateHeader.mapDiskDirectory1 = streamReader.ReadAscii();
            streamReader.Seek(0x2B438, SeekOrigin.Begin);
            gamestateHeader.player1GT1 = streamReader.ReadUTF16();
            streamReader.Seek(0x2B47C, SeekOrigin.Begin);
            gamestateHeader.player1ST1 = streamReader.ReadUTF16();

            string[] tmp = gamestateHeader.mapScenarioName.Split('\\');
            gamestateHeader.trueMapName = tmp[2];
            tmp = null;

            // Get Relative taglist
            trueTaglist = new Liberty.classInfo.iniFile(iniFolderPath + gamestateHeader.trueMapName + ".taglist");

            // Load LinkObjects
            streamReader.Seek(0x7074FC, SeekOrigin.Begin);
            linkObjects.Clear();
            for (int i = 0; i < 2048; i++)
            {
                streamReader.Seek(0x7074FC + (16 * i), SeekOrigin.Begin);

                LinkObjectTable linkObj = new LinkObjectTable();
                linkObj.GlobalOffset = (int)streamReader.Position;
                linkObj.RelativeOffset = linkObj.GlobalOffset - 0x7074FC;

                linkObj.DatumSaltIndex = (UInt16)(streamReader.ReadUInt16() << 16 | i);
                linkObj.Flags = streamReader.ReadInt16();
                linkObj.TagGroup = (byte)streamReader.ReadByte();
                streamReader.Seek(linkObj.GlobalOffset + 0x08, SeekOrigin.Begin);
                linkObj.PoolOffset = streamReader.ReadUInt32();
                linkObj.MemoryAddress = streamReader.ReadUInt32();

                streamReader.Seek(0x70F5FC + linkObj.PoolOffset, SeekOrigin.Begin);
                UInt32 gid = streamReader.ReadUInt32();
                if (gid != 0x0)
                    linkObjects.Add(linkObj);
            }

            IdentParents[0] = 0;
            IdentParents[1] = 1;
            IdentParents[2] = 2;
            IdentParents[3] = 3;
            IdentParents[4] = 4;
            IdentParents[5] = 5;
            IdentParents[6] = 6;
            IdentParents[7] = 7;
            IdentParents[8] = 8;
            IdentParents[9] = 9;
            IdentParents[10] = 10;
            IdentParents[11] = 11;
            IdentParents[13] = 13;

            foreach (LinkObjectTable linkedObjects in linkObjects)
            {
                //if (!((IList<int>)IdentParents).Contains((int)linkedObjects.TagGroup))
                {
                    if (!linkedObjects.PoolOffset.Equals(0))
                    {
                        H4GameObject h4g = new H4GameObject();
                        streamReader.Seek(0x70F5FC + linkedObjects.PoolOffset - 16, SeekOrigin.Begin);
                        h4g.objectSizeWithLink = streamReader.ReadUInt32();
                        h4g.objectDatumIndex = streamReader.ReadUInt32();

                        streamReader.Seek(0x70F5FC + linkedObjects.PoolOffset, SeekOrigin.Begin);
                        h4g.linkedData = linkedObjects;
                        h4g.GameIdent = streamReader.ReadUInt32();

                        streamReader.Seek(0x70F5FC + linkedObjects.PoolOffset + 0x40, SeekOrigin.Begin);
                        h4g.BoundingBoxX1 = streamReader.ReadFloat();
                        h4g.BoundingBoxY1 = streamReader.ReadFloat();
                        h4g.BoundingBoxZ1 = streamReader.ReadFloat();

                        streamReader.Seek(0x70F5FC + linkedObjects.PoolOffset + 0x50, SeekOrigin.Begin);
                        h4g.BoundingBoxX2 = streamReader.ReadFloat();
                        h4g.BoundingBoxY2 = streamReader.ReadFloat();
                        h4g.BoundingBoxZ2 = streamReader.ReadFloat();

                        streamReader.Seek(0x70F5FC + linkedObjects.PoolOffset + 0x64, SeekOrigin.Begin);
                        h4g.PositionX = streamReader.ReadFloat();
                        h4g.PositionY = streamReader.ReadFloat();
                        h4g.PositionZ = streamReader.ReadFloat();

                        switch (h4g.linkedData.TagGroup)
                        {
                            //case (byte)TagGroup.Weap:
                            //    H4WeaponObject h4w = new H4WeaponObject();
                            //    streamReader.Seek(0x70F5FC + linkedObjects.PoolOffset + 0x23A, SeekOrigin.Begin);
                            //    h4w.ExternalAmmo = streamReader.ReadInt16();
                            //    streamReader.Seek(0x70F5FC + linkedObjects.PoolOffset + 0x23E, SeekOrigin.Begin);
                            //    h4w.ClipAmmo = streamReader.ReadInt16();

                            //    h4g.weaponData = h4w;
                            //    h4g.bipedData = null;
                            //    break;
                            case (byte)TagGroup.Bipd:
                                H4BipedObject h4b = new H4BipedObject();
                                //streamReader.Seek(0x70F5FC + linkedObjects.PoolOffset + 0x17C, SeekOrigin.Begin);
                                //h4b.PlayerIndex = streamReader.ReadUInt16();

                                streamReader.Seek(0x70F5FC + linkedObjects.PoolOffset + 0x666, SeekOrigin.Begin);
                                h4b.FragNade = (sbyte)streamReader.ReadSByte();
                                h4b.PlasmaNade = (sbyte)streamReader.ReadSByte();
                                h4b.SpikeNade = (sbyte)streamReader.ReadSByte();
                                h4b.FireNade = (sbyte)streamReader.ReadSByte();

                                h4g.bipedData = h4b;
                                h4g.weaponData = null;
                                break;
                        }
                        gameObjects.Add(h4g);
                    }
                }
            }

            foreach (H4GameObject gameObj in gameObjects)
            {
                ListViewItem lvi = new ListViewItem();
                string ident = "0x" + gameObj.GameIdent.ToString("X");
                lvi.Text = trueTaglist.IniReadValue("yo", ident);
                lvi.SubItems.Add(gameObj.GameIdent.ToString("X"));
                lvi.SubItems.Add(gameObj.linkedData.TagGroup.ToString());
                lvi.SubItems.Add(gameObj.linkedData.DatumSaltIndex.ToString("X"));
                lvi.Tag = gameObj;

                listView1.Items.Add(lvi);
            }

            toolStripStatusLabel2.Text = "Loaded gamestate file... hehe :3";
        }

        public class GamestateHeader
        {
            public string mapScenarioName { get; set; }
            public string relativeEngineBuild { get; set; }
            public string mapDiskDirectory1 { get; set; }
            public string player1GT1 { get; set; }
            public string player1ST1 { get; set; }
            public string player1GT2 { get; set; }
            public string mapDiskDirectory2 { get; set; }

            public string trueMapName { get; set; }
        }
        public class LinkObjectTable
        {
            public int RelativeOffset { get; set; }
            public int GlobalOffset { get; set; }

            public UInt16 DatumSaltIndex { get; set; }
            public Int16 Flags { get; set; }
            public byte TagGroup { get; set; }
            public UInt32 PoolOffset { get; set; }
            public UInt32 MemoryAddress { get; set; }
        }
        public class H4PlayerObject
        {
            public UInt16 DatumSaltIndex { get; set; }
            //public UInt32 BipedOject { get; set; }
            public string Gamertag1 { get; set; }
            public string ServiceTag1 { get; set; }

            public string Gamertag2 { get; set; }
            public string Gamertag3 { get; set; }

            public string ServiceTag2 { get; set; }

            public string Gamertag4 { get; set; }
        }
        public class H4GameObject
        {
            public LinkObjectTable linkedData { get; set; }

            public UInt32 objectSizeWithLink { get; set; }
            public UInt32 objectDatumIndex { get; set; }

            public UInt32 GameIdent { get; set; }
            public float BoundingBoxX1 { get; set; }
            public float BoundingBoxY1 { get; set; }
            public float BoundingBoxZ1 { get; set; }
            public float BoundingBoxX2 { get; set; }
            public float BoundingBoxY2 { get; set; }
            public float BoundingBoxZ2 { get; set; }
            public float PositionX { get; set; }
            public float PositionY { get; set; }
            public float PositionZ { get; set; }

            public H4WeaponObject weaponData { get; set; }
            public H4BipedObject bipedData { get; set; }
        }
        public class H4WeaponObject
        {
            public Int16 ExternalAmmo { get; set; }
            public Int16 ClipAmmo { get; set; }
        }
        public class H4BipedObject
        {
            public UInt16 PlayerIndex { get; set; }
            public sbyte FragNade { get; set; }
            public sbyte PlasmaNade { get; set; }
            public sbyte SpikeNade { get; set; }
            public sbyte FireNade { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Halo 4 gamesave (mmiof.bmf)|*.*";
            ofd.Title = "Open Halo 4 gamesave";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = ofd.FileName;
                loadSave();
            }
        }

        private void btnFix_Click(object sender, EventArgs e)
        {
            if (_stream != null && _stream.CanWrite && streamReader != null && textBox1.Text != "")
            {
                streamWriter = new SaveWriter(new FileStream(textBox1.Text, FileMode.OpenOrCreate));

                // Load the whole stream into memory
                MemoryStream memoryStream = new MemoryStream((int)streamReader.Length);
                memoryStream.SetLength(streamReader.Length);
                streamReader.Seek(0x00, SeekOrigin.Begin);
                streamReader.ReadBlock(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);

                // Hash the contents
                memoryStream.Position = 0x2D25C;
                memoryStream.Write(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0, 20);
                byte[] hash = SaveSHA1.ComputeHash(memoryStream.GetBuffer());

                // Write the new digest
                streamWriter.Seek(0x2D25C, SeekOrigin.Begin);
                foreach (byte hashPart in hash)
                    streamWriter.WriteByte(hashPart);

                streamWriter.Close();

                MessageBox.Show("Save resigned.");
            }
        }
    }
}
