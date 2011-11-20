using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HCEAGamestateTest.IO;
using Liberty.SaveIO;

namespace HCEAGamestateTest
{
    public partial class Form1 : Form
    {
        string iniFolderPath;
        Liberty.classInfo.iniFile trueTaglist;
        public SaveReader streamReader;
        IList<HCEXObjectEntry> objEntrys = new List<HCEXObjectEntry>();
        IList<HCEXPoolChunk> poolChunks = new List<HCEXPoolChunk>();
        byte[] uncompressedSaveDate = new byte[0x40A000];
        long uncompressedDataStart;
        HCEXPoolChunk playerBiped = null;
        IDictionary<HCEXPoolChunk, ListViewItem> poolChunkLvi = new Dictionary<HCEXPoolChunk, ListViewItem>();

        public Form1()
        {
            InitializeComponent();
        }

        private void loadSave(string filePath)
        {
            listView1.Items.Clear();

            // Load into Stream
            Stream stream = File.OpenRead(filePath);

            // Load into Aaron's nice Liberty IO
            streamReader = new SaveReader(stream);

            uncompressedDataStart = streamReader.Length - 0x40A000;
            streamReader.Seek(uncompressedDataStart, SeekOrigin.Begin);
            streamReader.ReadBlock(uncompressedSaveDate, 0, 0x040A000);

            stream = new MemoryStream(uncompressedSaveDate);
            streamReader = new SaveReader(stream);

            // Verify Valid Container
            byte[] header = new byte[4];
            if (streamReader.ReadAscii() != "non compressed save")
                throw new ArgumentException("The file format is invalid: bad header\r\nShould be \"non compressed save\"");

            if (stream.Length != 0x40A000)
                throw new ArgumentException("The file format is invalid: incorrect file size\r\nExpected 0x40A000 but got 0x" + stream.Length.ToString("X"));

            // Load LinkObjects
            //streamReader.Seek(0x5305C, SeekOrigin.Begin);

            streamReader.Seek(0x53090, SeekOrigin.Begin);
            UInt32 baseAddress = streamReader.ReadUInt32();
            UInt32 startAddress = 0x53094;

            objEntrys.Clear();
            for (ushort i = 0; i < 2048; i++)
            {
                streamReader.Seek(0x53094 + (12 * i), SeekOrigin.Begin);
                long pos = streamReader.Position;

                HCEXObjectEntry objEntry = new HCEXObjectEntry();
                objEntry.offset = pos;
                objEntry.DatumIndex = (uint)(streamReader.ReadUInt16() << 16 | i);
                objEntry.Flags = streamReader.ReadUInt16();
                objEntry.Unknown = streamReader.ReadUInt16();
                objEntry.DataSize = streamReader.ReadUInt16();
                objEntry.ObjectAddress = streamReader.ReadUInt32();
                objEntry.ObjectAddress = (objEntry.ObjectAddress - baseAddress) + startAddress;

                objEntrys.Add(objEntry);
            }

            poolChunks.Clear();
            foreach (HCEXObjectEntry objEntry in objEntrys)
            {
                if ((objEntry.DatumIndex >> 16) != 0)
                {
                    streamReader.Seek(objEntry.ObjectAddress, SeekOrigin.Begin);
                    HCEXPoolChunk poolChunk = new HCEXPoolChunk();
                    {
                        poolChunk.MapIdent = streamReader.ReadUInt32();
                        poolChunk.objectEntry = objEntry;
                    }
                    poolChunks.Add(poolChunk);
                }
                else
                {
                    poolChunks.Add(null);
                }
            }

            poolChunkLvi.Clear();
            foreach (HCEXPoolChunk poolChunk in poolChunks)
            {
                if (poolChunk == null)
                    continue;

                ListViewItem lvi = new ListViewItem();
                lvi.Text = poolChunk.objectEntry.DatumIndex.ToString("X");
                lvi.SubItems.Add(poolChunk.MapIdent.ToString("X"));
                lvi.SubItems.Add("Tagnames coming soon...");//trueTaglist.IniReadValue(gamestateHeader.trueMapName, gameObj.GameIdent.ToString("X"));
                lvi.SubItems.Add(poolChunk.objectEntry.DataSize.ToString("X")); //Add(gameObj.linkedData.TagGroup.ToString());
                lvi.Tag = poolChunk;
                poolChunkLvi[poolChunk] = lvi;

                listView1.Items.Add(lvi);
            }

            // Read player data
            // TODO: Parse players table properly instead of just assuming the datum index is at this offset
            streamReader.Seek(0x2AD9AA, SeekOrigin.Begin);
            int playerIndex = streamReader.ReadUInt16();
            playerBiped = poolChunks[playerIndex];

            textBox1.Text = filePath;
            toolStripStatusLabel2.Text = "Loaded gamestate file... hehe :3";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Halo Anniversary Campaign Saves|*.cfg";
            ofd.Title = "Open Save File";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                loadSave(ofd.FileName);
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
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
        public class HCEXObjectEntry
        {
            public long offset { get; set; }

            public UInt32 DatumIndex { get; set; }
            public UInt16 Flags { get; set; }
            public UInt16 Unknown { get; set; }
            public UInt16 DataSize { get; set; }
            public UInt32 ObjectAddress { get; set; }

            public byte[] CompleteEntry { get; set; }
        }
        public class HCEXPoolChunk
        {
            public UInt32 MapIdent { get; set; }

            public HCEXObjectEntry objectEntry { get; set; }
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Do screwy .net stuff
                ListViewItem lvi = listView1.SelectedItems[0];
                HCEXPoolChunk obj = (HCEXPoolChunk)lvi.Tag;

                // Load Data to Frontend

                // Load Idents
                txtGameIdent.Text = "0x" + obj.MapIdent.ToString("X");
                txtFileOffset.Text = "0x" + (uncompressedDataStart + obj.objectEntry.ObjectAddress).ToString("X");
                txtChunkSize.Text = "0x" + obj.objectEntry.DataSize.ToString("X");
                //txtTagFilename.Text = trueTaglist.IniReadValue(gamestateHeader.trueMapName, obj.GameIdent.ToString("X"));

                //// Load BoundingBox (max)
                //txtBBX1.Text = Convert.ToString(obj.BoundingBoxX1);
                //txtBBY1.Text = Convert.ToString(obj.BoundingBoxY1);
                //txtBBZ1.Text = Convert.ToString(obj.BoundingBoxZ1);

                //// Load BoundingBox (min)
                //txtBBX2.Text = Convert.ToString(obj.BoundingBoxX2);
                //txtBBY2.Text = Convert.ToString(obj.BoundingBoxY2);
                //txtBBZ2.Text = Convert.ToString(obj.BoundingBoxZ2);

                //// Load Parent Position
                //txtPosX.Text = Convert.ToString(obj.PositionX);
                //txtPosY.Text = Convert.ToString(obj.PositionY);
                //txtPosZ.Text = Convert.ToString(obj.PositionZ);
            }
            catch (Exception ex) { }
        }

        private void btnPlayerBiped_Click(object sender, EventArgs e)
        {
            listView1.Focus();
            ListViewItem lvi = poolChunkLvi[playerBiped];
            lvi.Selected = true;
            lvi.EnsureVisible();
        }
    }
}
