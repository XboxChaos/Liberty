using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Liberty.SaveIO;

namespace H3GamestateTest
{
    public partial class Form1 : Form
    {
        string iniFolderPath;
        Liberty.classInfo.iniFile trueTaglist;
        SaveReader streamReader;
        GamestateHeader gamestateHeader = new GamestateHeader();
        IList<LinkObjectTable> linkObjects = new List<LinkObjectTable>();
        IList<H3GameObject> gameObjects = new List<H3GameObject>();

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
            iniFolderPath = @"H:\Computer Science\x360\halo3\json\";
#endif

            toolStripStatusLabel2.Text = "Load Halo 3 Gamestate Gamesave...";
        }

        void loadSave()
        {
            // Load into Stream
            MemoryStream stream = new MemoryStream(File.ReadAllBytes(textBox1.Text));

            // Load into Aaron's nice Liberty IO
            streamReader = new SaveReader(stream);

            // Verify Valid Container
            byte[] header = new byte[4];
            streamReader.Seek(0, SeekOrigin.Begin);
            streamReader.ReadBlock(header, 0, 4);
            if (header[0] != 0x4E || header[1] != 0xB2 || header[2] != 0xC1 || header[3] != 0x86)
                throw new ArgumentException("The file format is invalid: bad header\r\nShould be 4E B2 C1 86");

            if (stream.Length != 0x7E0000)
                throw new ArgumentException("The file format is invalid: incorrect file size\r\nExpected 0x7E0000 but got 0x" + stream.Length.ToString("X"));

            // Load Header
            streamReader.Seek(0x08, SeekOrigin.Begin);
            gamestateHeader.mapScenarioName = streamReader.ReadAscii();
            streamReader.Seek(0x0108, SeekOrigin.Begin);
            gamestateHeader.relativeEngineBuild = streamReader.ReadAscii();
            streamReader.Seek(0x0154, SeekOrigin.Begin);
            gamestateHeader.mapDiskDirectory1 = streamReader.ReadAscii();
            streamReader.Seek(0xE6D9, SeekOrigin.Begin);
            gamestateHeader.player1GT1 = streamReader.ReadUTF16();
            streamReader.Seek(0xE70F, SeekOrigin.Begin);
            gamestateHeader.player1ST1 = streamReader.ReadUTF16();
            streamReader.Seek(0xE7A1, SeekOrigin.Begin);
            gamestateHeader.player1GT2 = streamReader.ReadUTF16();
            streamReader.Seek(0x3E0290, SeekOrigin.Begin);
            gamestateHeader.mapDiskDirectory2 = streamReader.ReadUTF16();

            string[] tmp = gamestateHeader.mapScenarioName.Split('\\');
            gamestateHeader.trueMapName = tmp[2];
            tmp = null;

            // Get Relative taglist
            trueTaglist = new Liberty.classInfo.iniFile(iniFolderPath + "map-" + gamestateHeader.trueMapName + ".tagCDB");

            // Load LinkObjects
            streamReader.Seek(0x46A0F4, SeekOrigin.Begin);
            linkObjects.Clear();
            for (int i = 0; i < 2048; i++)
            {
                streamReader.Seek(0x46A0F4 + (16 * i), SeekOrigin.Begin);

                LinkObjectTable linkObj = new LinkObjectTable();
                linkObj.MahOffsat = stream.Position;

                linkObj.DatumSaltIndex = streamReader.ReadUInt16();
                linkObj.Unk1 = (byte)streamReader.ReadByte();
                linkObj.TagGroup = (byte)streamReader.ReadByte();
                linkObj.Unk3 = streamReader.ReadUInt16();
                linkObj.Unk4 = streamReader.ReadUInt16();
                linkObj.PoolOffset = streamReader.ReadUInt32();
                linkObj.Unk6 = streamReader.ReadUInt32();

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
                        streamReader.Seek(0x4721F4 + linkedObjects.PoolOffset, SeekOrigin.Begin);
                        H3GameObject h3g = new H3GameObject();

                        h3g.linkedData = linkedObjects;
                        h3g.GameIdent = streamReader.ReadUInt32();

                        streamReader.Seek(0x4721F4 + linkedObjects.PoolOffset + 0x20, SeekOrigin.Begin);
                        h3g.BoundingBoxX1 = streamReader.ReadFloat();
                        h3g.BoundingBoxY1 = streamReader.ReadFloat();
                        h3g.BoundingBoxZ1 = streamReader.ReadFloat();

                        streamReader.Seek(0x4721F4 + linkedObjects.PoolOffset + 0x30, SeekOrigin.Begin);
                        h3g.BoundingBoxX2 = streamReader.ReadFloat();
                        h3g.BoundingBoxY2 = streamReader.ReadFloat();
                        h3g.BoundingBoxZ2 = streamReader.ReadFloat();

                        streamReader.Seek(0x4721F4 + linkedObjects.PoolOffset + 0x40, SeekOrigin.Begin);
                        h3g.PositionX = streamReader.ReadFloat();
                        h3g.PositionY = streamReader.ReadFloat();
                        h3g.PositionZ = streamReader.ReadFloat();

                        switch (h3g.linkedData.TagGroup)
                        {
                            case (byte)TagGroup.Weap:
                                H3WeaponObject h3w = new H3WeaponObject();
                                streamReader.Seek(0x4721F4 + linkedObjects.PoolOffset + 0x23A, SeekOrigin.Begin);
                                h3w.ExternalAmmo = streamReader.ReadInt16();
                                streamReader.Seek(0x4721F4 + linkedObjects.PoolOffset + 0x23E, SeekOrigin.Begin);
                                h3w.ClipAmmo = streamReader.ReadInt16();

                                h3g.weaponData = h3w;
                                h3g.bipedData = null;
                                break;
                            case (byte)TagGroup.Bipd:
                                //H3BipedObject h3b = new H3BipedObject();
                                //streamReader.Seek(0x4721F4 + linkedObjects.PoolOffset + 0x23A, SeekOrigin.Begin);
                                //h3b.ExternalAmmo = streamReader.ReadInt16();
                                //streamReader.Seek(0x4721F4 + linkedObjects.PoolOffset + 0x23E, SeekOrigin.Begin);
                                //h3b.ClipAmmo = streamReader.ReadInt16();

                                //h3g.bipedData = h3b;
                                h3g.bipedData = null;
                                h3g.weaponData = null;
                                break;
                        }
                        gameObjects.Add(h3g);
                    }
                }
            }

            foreach (H3GameObject gameObj in gameObjects)
            {
                TreeNode node = new TreeNode();
                node.Text = gameObj.GameIdent.ToString("X") + " - " + gameObj.linkedData.PoolOffset.ToString("X") + " - " + gameObj.linkedData.TagGroup.ToString();
                node.Tag = gameObj;

                treeView1.Nodes.Add(node);
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
            public Int64 MahOffsat { get; set; }
            
            public UInt16 DatumSaltIndex { get; set; }
            public byte Unk1 { get; set; }
            public byte TagGroup { get; set; }
            public UInt16 Unk3 { get; set; }
            public UInt16 Unk4 { get; set; }
            public UInt32 PoolOffset { get; set; }
            public UInt32 Unk6 { get; set; }
        }
        public class H3GameObject
        {
            public LinkObjectTable linkedData { get; set; }

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

            public H3WeaponObject weaponData { get; set; }
            public H3BipedObject bipedData { get; set; }
        }
        public class H3WeaponObject
        {
            public Int16 ExternalAmmo { get; set; }
            public Int16 ClipAmmo { get; set; }
        }
        public class H3BipedObject
        {
            public sbyte FragNade { get; set; }
            public sbyte PlasmaNade { get; set; }
            public sbyte SpikeNade { get; set; }
            public sbyte FireNade { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Halo 3 gamesave (mmiof.bmf)|*.bmf";
            ofd.Title = "Open Halo 3 gamesave";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = ofd.FileName;
                loadSave();
            }
        }

        private void derpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ye");
        }
        private void openLinkedByteDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            H3GameObject gameObj = (H3GameObject)treeView1.SelectedNode.Tag;

            string byteData = gameObj.linkedData.DatumSaltIndex.ToString("X") + gameObj.linkedData.Unk1.ToString("X") + gameObj.linkedData.TagGroup.ToString("X") +
                gameObj.linkedData.Unk3.ToString("X") + gameObj.linkedData.Unk4.ToString("X") + gameObj.linkedData.PoolOffset.ToString("X") +
                gameObj.linkedData.Unk4.ToString("X") + gameObj.linkedData.Unk6.ToString("X");   

            MessageBox.Show("Chunk linked Bytedata;\n\n" + byteData);
        }
        private void addPoolOffsetToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            H3GameObject gameObj = (H3GameObject)treeView1.SelectedNode.Tag;

            string byteData = gameObj.linkedData.PoolOffset.ToString("X");
            Clipboard.SetText(byteData);

            toolStripStatusLabel2.Text = "Added objectpool offset to Clipboard...";
        }
        private void addByteDataToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            H3GameObject gameObj = (H3GameObject)treeView1.SelectedNode.Tag;

            string byteData = gameObj.linkedData.DatumSaltIndex.ToString("X") + gameObj.linkedData.Unk1.ToString("X") + gameObj.linkedData.TagGroup.ToString("X") +
                gameObj.linkedData.Unk3.ToString("X") + gameObj.linkedData.Unk4.ToString("X") + gameObj.linkedData.PoolOffset.ToString("X") +
                gameObj.linkedData.Unk4.ToString("X") + gameObj.linkedData.Unk6.ToString("X"); Clipboard.SetText(byteData);

            toolStripStatusLabel2.Text = "Added linked bytedata to Clipboard...";
        }
        private void addIdentToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            H3GameObject gameObj = (H3GameObject)treeView1.SelectedNode.Tag;

            string byteData = gameObj.GameIdent.ToString("X");
            Clipboard.SetText(byteData);

            toolStripStatusLabel2.Text = "Added gameIdent to Clipboard...";
        }
        private void addLinkedChunkOffsetToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            H3GameObject gameObj = (H3GameObject)treeView1.SelectedNode.Tag;

            string byteData = gameObj.linkedData.MahOffsat.ToString("X");
            Clipboard.SetText(byteData);

            toolStripStatusLabel2.Text = "Added linked chunk offset to Clipboard...";
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null)
            {
                // Do screwy .net stuff
                TreeNode node = (TreeNode)e.Node;
                H3GameObject obj = (H3GameObject)node.Tag;

                // Load Data to Frontend

                // Load Idents
                txtGameIdent.Text = "0x" + obj.GameIdent.ToString("X");
                txtTagFilename.Text = trueTaglist.IniReadValue(gamestateHeader.trueMapName, obj.GameIdent.ToString("X"));

                // Load BoundingBox (max)
                txtBBX1.Text = Convert.ToString(obj.BoundingBoxX1);
                txtBBY1.Text = Convert.ToString(obj.BoundingBoxY1);
                txtBBZ1.Text = Convert.ToString(obj.BoundingBoxZ1);

                // Load BoundingBox (min)
                txtBBX2.Text = Convert.ToString(obj.BoundingBoxX2);
                txtBBY2.Text = Convert.ToString(obj.BoundingBoxY2);
                txtBBZ2.Text = Convert.ToString(obj.BoundingBoxZ2);

                // Load Parent Position
                txtPosX.Text = Convert.ToString(obj.PositionX);
                txtPosY.Text = Convert.ToString(obj.PositionY);
                txtPosZ.Text = Convert.ToString(obj.PositionZ);
            }
        }
    }
}
