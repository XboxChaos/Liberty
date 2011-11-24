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
using Liberty.Security;

namespace HCEAGamestateTest
{
    public partial class Form1 : Form
    {
        public UInt32 cfgCRC;
        public UInt32 postCFG;
        public UInt32 postpostCFG;
        public UInt32 firstStream;

        public SaveReader streamReader;
        public SaveWriter streamWriter;
        IList<HCEXObjectEntry> objEntrys = new List<HCEXObjectEntry>();
        IList<HCEXPoolChunk> poolChunks = new List<HCEXPoolChunk>();
        byte[] uncompressedSaveDate = new byte[0x40A000];
        long uncompressedDataStart;

        HCEXPoolChunk currentObject = null;
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

            streamReader.Seek(0x08, SeekOrigin.Begin);
            cfgCRC = streamReader.ReadUInt32();
            postCFG = streamReader.ReadUInt32();
            postpostCFG = streamReader.ReadUInt32();

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
                        poolChunk.ObjectEntry = objEntry;

                        streamReader.Seek(objEntry.ObjectAddress + 0xD8, SeekOrigin.Begin);
                        poolChunk.HealthModifier = streamReader.ReadFloat();
                        poolChunk.ShieldModifier = streamReader.ReadFloat();

                        streamReader.Seek(objEntry.ObjectAddress + 0x114, SeekOrigin.Begin);
                        poolChunk.NextCarried = streamReader.ReadUInt32();
                        poolChunk.FirstCarried = streamReader.ReadUInt32();
                        poolChunk.Carrier = streamReader.ReadUInt32();

                        streamReader.Seek(objEntry.ObjectAddress + 0x2B6, SeekOrigin.Begin);
                        poolChunk.WeaponAmmo = streamReader.ReadInt16();
                        poolChunk.WeaponClipAmmo = streamReader.ReadInt16();

                        streamReader.Seek(objEntry.ObjectAddress + 0x2F8, SeekOrigin.Begin);
                        poolChunk.PrimaryWeapon = streamReader.ReadUInt32();
                        poolChunk.SecondaryWeapon = streamReader.ReadUInt32();
                        poolChunk.TertiaryWeapon = streamReader.ReadUInt32();
                        poolChunk.QuaternaryWeapon = streamReader.ReadUInt32();

                        streamReader.Seek(objEntry.ObjectAddress + 0x5C, SeekOrigin.Begin);
                        poolChunk.PositionCordX = streamReader.ReadFloat();
                        poolChunk.PositionCordY = streamReader.ReadFloat();
                        poolChunk.PositionCordZ = streamReader.ReadFloat();

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
                lvi.Text = poolChunk.ObjectEntry.DatumIndex.ToString("X");
                lvi.SubItems.Add(poolChunk.MapIdent.ToString("X"));
                lvi.SubItems.Add(poolChunk.ObjectEntry.TagGroup.ToString());
                lvi.SubItems.Add("Tagnames coming soon...");
                lvi.SubItems.Add(poolChunk.ObjectEntry.DataSize.ToString("X"));
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
            splitContainer1.Enabled = true;
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

            public UInt32 NextCarried { get; set; }
            public UInt32 FirstCarried { get; set; }
            public UInt32 Carrier { get; set; }

            public Int16 WeaponAmmo { get; set; }
            public Int16 WeaponClipAmmo { get; set; }

            public byte FragNades { get; set; }
            public byte PlasmaNades { get; set; }

            public float HealthModifier { get; set; }
            public float ShieldModifier { get; set; }

            public float PositionCordX { get; set; }
            public float PositionCordY { get; set; }
            public float PositionCordZ { get; set; }

            public UInt32 PrimaryWeapon { get; set; }
            public UInt32 SecondaryWeapon { get; set; }
            public UInt32 TertiaryWeapon { get; set; }
            public UInt32 QuaternaryWeapon { get; set; }

            public HCEXObjectEntry ObjectEntry { get; set; }
        }
        private void ShowCarriedWeapons()
        {
            groupCarriedWeapons.Visible = true;
            txtPrimaryWeap.Text = "0x" + currentObject.PrimaryWeapon.ToString("X");
            txtSecondaryWeap.Text = "0x" + currentObject.SecondaryWeapon.ToString("X");
            txtTertiaryWeap.Text = "0x" + currentObject.TertiaryWeapon.ToString("X");
            txtQuaternaryWeap.Text = "0x" + currentObject.QuaternaryWeapon.ToString("X");
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count == 0)
                    return;
                ListViewItem selectedItem = listView1.SelectedItems[0];
                currentObject = (HCEXPoolChunk)selectedItem.Tag;

                // Load Idents
                txtDatumIndex.Text = "0x" + currentObject.ObjectEntry.DatumIndex.ToString("X");
                txtGameIdent.Text = "0x" + currentObject.MapIdent.ToString("X");
                txtFileOffset.Text = "0x" + (uncompressedDataStart + currentObject.ObjectEntry.ObjectAddress).ToString("X");
                txtChunkSize.Text = "0x" + currentObject.ObjectEntry.DataSize.ToString("X");

                txtFirstChild.Text = "0x" + currentObject.FirstCarried.ToString("X");
                txtNextChild.Text = "0x" + currentObject.NextCarried.ToString("X");
                txtCarrier.Text = "0x" + currentObject.Carrier.ToString("X");

                txtXCord.Text = currentObject.PositionCordX.ToString();
                txtYCord.Text = currentObject.PositionCordY.ToString();
                txtZCord.Text = currentObject.PositionCordZ.ToString();

                groupWeapon.Visible = false;
                groupBiped.Visible = false;
                groupCarriedWeapons.Visible = false;

                switch (currentObject.ObjectEntry.TagGroup)
                {
                    case TagGroup.Weap:
                        groupWeapon.Visible = true;
                        txtWeapAmmo.Text = currentObject.WeaponAmmo.ToString();
                        txtWeapClip.Text = currentObject.WeaponClipAmmo.ToString();
                        break;

                    case TagGroup.Bipd:
                        groupBiped.Visible = true;
                        txtFragNades.Text = currentObject.FragNades.ToString();
                        txtPlasmaNades.Text = currentObject.PlasmaNades.ToString();
                        txtBipedHealth.Text = currentObject.HealthModifier.ToString();
                        txtBipedShields.Text = currentObject.ShieldModifier.ToString();
                        ShowCarriedWeapons();
                        break;

                    case TagGroup.Vehi:
                        ShowCarriedWeapons();
                        break;
                }

                tabControl1.Enabled = true;
            }
            catch { }
        }

        private void SelectChunk(HCEXPoolChunk chunk)
        {
            listView1.Focus();
            ListViewItem lvi = poolChunkLvi[chunk];
            lvi.Selected = true;
            lvi.EnsureVisible();
        }

        private void SelectChunk(uint datumIndex)
        {
            int tableIndex = (int)(datumIndex & 0xFFFF);
            if (tableIndex < 0 || tableIndex > poolChunks.Count)
                return;
            SelectChunk(poolChunks[tableIndex]);
        }

        private void btnPlayerBiped_Click(object sender, EventArgs e)
        {
            SelectChunk(playerBiped);
        }

        private void btnFirstChild_Click(object sender, EventArgs e)
        {
            SelectChunk(currentObject.FirstCarried);
        }

        private void btnNextChild_Click(object sender, EventArgs e)
        {
            SelectChunk(currentObject.NextCarried);
        }

        private void btnCarrier_Click(object sender, EventArgs e)
        {
            SelectChunk(currentObject.Carrier);
        }

        private void btnPrimaryWeap_Click(object sender, EventArgs e)
        {
            SelectChunk(currentObject.PrimaryWeapon);
        }

        private void btnSecondaryWeap_Click(object sender, EventArgs e)
        {
            SelectChunk(currentObject.SecondaryWeapon);
        }

        private void btnTertiaryWeap_Click(object sender, EventArgs e)
        {
            SelectChunk(currentObject.TertiaryWeapon);
        }

        private void btnQuaternaryWeap_Click(object sender, EventArgs e)
        {
            SelectChunk(currentObject.QuaternaryWeapon);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Resign teh save
        }
    }
}
