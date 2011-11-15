using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HCEAGamestateTest.IO;

namespace HCEAGamestateTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void loadSave(string filePath)
        {
            listView1.Clear();


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
    }
}
