using System;
using System.Collections.Generic;
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
    public partial class selectObjectWindow : Window
    {
        public selectObjectWindow(IEnumerable<TreeViewItem> items)
        {
            this.InitializeComponent();
            foreach (TreeViewItem item in items)
                tVObjects.Items.Add(item);
        }

        public TreeViewItem selectedItem { get; private set; }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            selectedItem = tVObjects.SelectedItem as TreeViewItem;

            FormFadeOut.Begin();
            classInfo.applicationExtra.disableInput(this);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            selectedItem = null;

            FormFadeOut.Begin();
            classInfo.applicationExtra.disableInput(this);
        }

        private void FormFadeOut_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tVObjects_SelectedItemChanged(object sender, RoutedEventArgs e)
        {
            btnOK.IsEnabled = (tVObjects.SelectedItem != null);
        }

        private void tVObjects_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = tVObjects.SelectedItem as TreeViewItem;
            if (item != null && item.IsMouseOver && item.Parent is TreeViewItem)
                btnOK_Click(sender, e);
        }
    }
}