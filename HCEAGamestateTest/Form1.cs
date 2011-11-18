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


        public Form1()
        {
            InitializeComponent();
        }

        private void loadSave(string filePath)
        {
            listView1.Items.Clear();

            // Load into Stream
            MemoryStream stream = new MemoryStream(File.ReadAllBytes(filePath));

            // Load into Aaron's nice Liberty IO
            streamReader = new SaveReader(stream);

            streamReader.Seek((streamReader.Length - 0x40A000), SeekOrigin.Begin);
            streamReader.ReadBlock(uncompressedSaveDate, 0, 0x040A000);

            stream = new MemoryStream(uncompressedSaveDate);
            streamReader = new SaveReader(stream);

            // Verify Valid Container
            byte[] header = new byte[4];
            streamReader.Seek(0, SeekOrigin.Begin);
            streamReader.ReadBlock(header, 0, 4);
            if (header[0] != 0x6E || header[1] != 0x6F || header[2] != 0x6E || header[3] != 0x20)
                throw new ArgumentException("The file format is invalid: bad header\r\nShould be 6E 6F 6E 20");

            if (stream.Length != 0x40A000)
                throw new ArgumentException("The file format is invalid: incorrect file size\r\nExpected 0x40A000 but got 0x" + stream.Length.ToString("X"));

            // Load LinkObjects
            streamReader.Seek(0x05305C, SeekOrigin.Begin);

            streamReader.Seek(0x53090, SeekOrigin.Begin);
            UInt32 baseAddress = streamReader.ReadUInt32();
            UInt32 startAddress = 0x53094;

            objEntrys.Clear();
            for (int i = 0; i < 2048; i++)
            {
                streamReader.Seek(0x53094 + (12 * i), SeekOrigin.Begin);
                long pos = streamReader.Position;

                HCEXObjectEntry objEntry = new HCEXObjectEntry();
                objEntry.DatumIndex = streamReader.ReadUInt16();
                objEntry.offset = pos;
                streamReader.Seek(0x53094 + (12 * i) + 0x08, SeekOrigin.Begin);
                objEntry.ObjectAddress = streamReader.ReadUInt32();
                objEntry.ObjectAddress = (objEntry.ObjectAddress - baseAddress) + startAddress;

                objEntrys.Add(objEntry);
            }

            poolChunks.Clear();
            foreach (HCEXObjectEntry objEntry in objEntrys)
            {
                if (objEntry.ObjectAddress > 0 && objEntry.ObjectAddress < streamReader.Length)
                {
                    streamReader.Seek(objEntry.ObjectAddress, SeekOrigin.Begin);
                    HCEXPoolChunk poolChunk = new HCEXPoolChunk();
                    {
                        poolChunk.GameIdent = streamReader.ReadUInt32();
                        poolChunk.objectEntry = objEntry;
                    }
                    poolChunks.Add(poolChunk);
                }
            }

            foreach (HCEXPoolChunk poolChunk in poolChunks)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = "Tagnames comming soon...";//trueTaglist.IniReadValue(gamestateHeader.trueMapName, gameObj.GameIdent.ToString("X"));
                lvi.SubItems.Add(poolChunk.GameIdent.ToString("X"));
                lvi.SubItems.Add("Tag Groups comming soon..."); //Add(gameObj.linkedData.TagGroup.ToString());
                lvi.Tag = poolChunk;

                listView1.Items.Add(lvi);
            }

            textBox1.Text = filePath;
            toolStripStatusLabel2.Text = "Loaded gamestate file... hehe :3";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Halo Combat Evolved Anniversar Save (saves.cfg)|saves.cfg";
            ofd.Title = "Open a valid Halo: Combat Evolved Anniversary save";
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

            public UInt16 DatumIndex { get; set; }
            public UInt32 ObjectAddress { get; set; }

            public byte[] CompleteEntry { get; set; }
        }
        public class HCEXPoolChunk
        {
            public UInt32 GameIdent { get; set; }

            public HCEXObjectEntry objectEntry { get; set; }
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Do screwy .net stuff
                ListViewItem lvi = (ListViewItem)listView1.SelectedItems[0];
                HCEXPoolChunk obj = (HCEXPoolChunk)lvi.Tag;

                // Load Data to Frontend

                // Load Idents
                txtGameIdent.Text = "0x" + obj.GameIdent.ToString("X");
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
    }
}
