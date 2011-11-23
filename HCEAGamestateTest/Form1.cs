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
                objEntry.Flags = streamReader.ReadByte();
                objEntry.TagGroup = (TagGroup)streamReader.ReadByte();
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

                        streamReader.Seek(objEntry.ObjectAddress + 0xD8, SeekOrigin.Begin);
                        poolChunk.HealthModifier = streamReader.ReadFloat();
                        poolChunk.ShieldModifier = streamReader.ReadFloat();

                        streamReader.Seek(objEntry.ObjectAddress + 0x2B6, SeekOrigin.Begin);
                        poolChunk.WeaponAmmo = streamReader.ReadInt16();
                        poolChunk.WeaponClipAmmo = streamReader.ReadInt16();

                        streamReader.Seek(objEntry.ObjectAddress + 0x31E, SeekOrigin.Begin);
                        poolChunk.FragNades = streamReader.ReadByte();
                        poolChunk.PlasmaNades = streamReader.ReadByte();
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
                lvi.SubItems.Add(poolChunk.objectEntry.TagGroup.ToString());
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
                try
                {
                    loadSave(ofd.FileName);
                }
                catch (Exception ex)
                { 
                    MessageBox.Show(ex.Message, "HCEAGamestateTest", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            NativeWinRT.ListViewSorter Sorter = new NativeWinRT.ListViewSorter();
            listView1.ListViewItemSorter = Sorter;
            if (!(listView1.ListViewItemSorter is NativeWinRT.ListViewSorter))
                return;
            Sorter = (NativeWinRT.ListViewSorter)listView1.ListViewItemSorter;

            Sorter.ByColumn = e.Column;
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
        }
        public enum TagGroup : byte
        {
            Bipd = 0,
            Vehi = 1,
            Weap = 2,
            Eqip = 3,
            Unk4 = 4,
            Unk5 = 5,
            Scen = 6,
            Mach = 7,
            Ctrl = 8,
            Lifi = 9,
            Unk10 = 10,
            Ssce = 11,
            Unk12 = 12,
            Unk13 = 13,
            Unk14 = 14,
            Unk15 = 15
        }
        public class HCEXObjectEntry
        {
            public long offset { get; set; }

            public UInt32 DatumIndex { get; set; }
            public Byte Flags { get; set; }
            public TagGroup TagGroup { get; set; }
            public UInt16 Unknown { get; set; }
            public UInt16 DataSize { get; set; }
            public UInt32 ObjectAddress { get; set; }

            public byte[] CompleteEntry { get; set; }
        }
        public class HCEXPoolChunk
        {
            public UInt32 MapIdent { get; set; }

            public Int16 WeaponAmmo { get; set; }
            public Int16 WeaponClipAmmo { get; set; }

            public byte FragNades { get; set; }
            public byte PlasmaNades { get; set; }

            public float HealthModifier { get; set; }
            public float ShieldModifier { get; set; }

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

                weaponPanel.Visible = false;
                bipedPanel.Visible = false;

                switch (obj.objectEntry.TagGroup)
                {
                    case TagGroup.Weap:
                        weaponPanel.Visible = true;
                        txtWeapAmmo.Text = obj.WeaponAmmo.ToString();
                        txtWeapClip.Text = obj.WeaponClipAmmo.ToString();
                        break;

                    case TagGroup.Bipd:
                        bipedPanel.Visible = true;
                        txtFragNades.Text = obj.FragNades.ToString();
                        txtPlasmaNades.Text = obj.PlasmaNades.ToString();
                        txtBipedHealth.Text = obj.HealthModifier.ToString();
                        txtBipedShields.Text = obj.ShieldModifier.ToString();
                        break;
                }
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
            catch { }
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
