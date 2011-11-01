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
        SaveReader streamReader;
        IList<LinkObjectTable> linkObjects = new List<LinkObjectTable>();
        IList<H3GameObject> gameObjects = new List<H3GameObject>();

        int[] IdentParents = new int[137];

        public Form1()
        {
            InitializeComponent();

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

            //public Int16 ExistingAmmo { get; set; }
            //public Int16 ClipAmmo { get; set; }
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
                gameObj.linkedData.Unk4.ToString("X") + gameObj.linkedData.Unk6.ToString("X"); 
            Clipboard.SetText(byteData);

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
    }
}