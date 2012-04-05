using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Liberty.Controls
{
    /// <summary>
    /// Interaction logic for listcordWindow.xaml
    /// </summary>
    public partial class listcordWindow : Window
    {
        private HCEX.GameObject hcex = null;
        private Halo3.GameObject halo3 = null;
        private Reach.GameObject hreach = null;
        private int GID = -1;

        private ListBoxItem _selectedItem;

        public listcordWindow(HCEX.CampaignSave _hcex, HCEX.TagGroup group)
        {
            this.InitializeComponent();

            lblTitle.Text = "COPY COORDINATES";
            lblSubInfo.Text = "Select an object to copy coordinates from";

            GID = 0;

            foreach (HCEX.GameObject obj in _hcex.Objects)
            {
                if (obj.TagGroup == group && obj != null)
                {
                    ListBoxItem lbi = new ListBoxItem();
                    string posData = " -- [X: {0} - Y: {1} - Z: {2}]";
                    lbi.Content = obj.Index.Value.ToString("X") + string.Format(posData, obj.Position.X.ToString(),
                                                                                         obj.Position.Y.ToString(),
                                                                                         obj.Position.Z.ToString());
                    lbi.Tag = obj;

                    listObjects.Items.Add(lbi);
                }
            }
        }
        public listcordWindow(Halo3.CampaignSave _halo3, Halo3.TagGroup group)
        {
            this.InitializeComponent();

            lblTitle.Text = "COPY COORDINATES";
            lblSubInfo.Text = "Select an object to copy coordinates from";

            GID = 2;

            foreach (Halo3.GameObject obj in _halo3.Objects)
            {
                if (obj.TagGroup == group && obj != null)
                {
                    ListBoxItem lbi = new ListBoxItem();
                    string posData = " -- [X: {0} - Y: {1} - Z: {2}]";
                    lbi.Content = obj.Index.Value.ToString("X") + string.Format(posData, obj.Position.X.ToString(),
                                                                                         obj.Position.Y.ToString(),
                                                                                         obj.Position.Z.ToString());
                    lbi.Tag = obj;

                    listObjects.Items.Add(lbi);
                }
            }
        }
        public listcordWindow(Reach.CampaignSave _reach, Reach.TagGroup group, Reach.TagListManager _reachTaglist)
        {
            this.InitializeComponent();

            lblTitle.Text = "COPY COORDINATES";
            lblSubInfo.Text = "Select an object to copy coordinates from";

            GID = 4;

            foreach (Reach.GameObject obj in _reach.Objects)
            {
                if (obj != null && obj.TagGroup == group)
                {
                    ListBoxItem lbi = new ListBoxItem();
                    string posData = " -- [X: {0} - Y: {1} - Z: {2}]";
                    lbi.Content = _reachTaglist.Identify(obj) + string.Format(posData, obj.X.ToString(),
                                                                                       obj.Y.ToString(),
                                                                                       obj.Z.ToString());
                    lbi.Tag = obj;

                    listObjects.Items.Add(lbi);
                }
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            _selectedItem = (ListBoxItem)listObjects.SelectedItem;

            switch (GID)
            {
                case 0:
                    hcex = (HCEX.GameObject)_selectedItem.Tag;
                    break;
                case 2:
                    halo3 = (Halo3.GameObject)_selectedItem.Tag;
                    break;
                case 4:
                    hreach = (Reach.GameObject)_selectedItem.Tag;
                    break;
            }

            FormFadeOut.Begin();
            classInfo.applicationExtra.disableInput(this);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            FormFadeOut.Begin();
            classInfo.applicationExtra.disableInput(this);
        }

        private void FormFadeOut_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count >= 1)
                btnOK.IsEnabled = true;
            else
                btnOK.IsEnabled = false;
        }

        private void listObjects_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Make sure we're actually clicking on an item
            if (listObjects.SelectedItem != null)
            {
                if (((ListBoxItem)listObjects.SelectedItem).IsMouseOver)
                {
                    _selectedItem = (ListBoxItem)listObjects.SelectedItem;
                    FormFadeOut.Begin();
                    classInfo.applicationExtra.disableInput(this);
                }
            }
        }

        public HCEX.GameObject HCEXObject
        {
            get { return hcex; }
        }
        public Halo3.GameObject Halo3Object
        {
            get { return halo3; }
        }
        public Reach.GameObject HReachObject
        {
            get { return hreach; }
        }
    }
}