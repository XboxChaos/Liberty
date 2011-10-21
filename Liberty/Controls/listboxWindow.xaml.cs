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
    /// <summary>
    /// Interaction logic for messageBox.xaml
    /// </summary>
    public partial class listboxWindow : Window
    {
        private ListBoxItem _selectedItem = null;

        public listboxWindow(List<ListBoxItem> items)
        {
            this.InitializeComponent();

            foreach (ListBoxItem item in items)
                listObjects.Items.Add(item);
        }

        public ListBoxItem selectedItem
        {
            get { return _selectedItem; }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            _selectedItem = (ListBoxItem)listObjects.SelectedItem;

            FormFadeOut.Begin();
            classInfo.applicationExtra.disableInput(this);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _selectedItem = null;

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
    }
}