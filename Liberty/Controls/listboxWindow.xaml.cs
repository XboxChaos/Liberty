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
        public listboxWindow()
        {
            this.InitializeComponent();

            foreach (ListBoxItem item in classInfo.storage.fileInfoStorage.listboxItems)
                listObjects.Items.Add(item);

            btnOK.IsEnabled = false;
            lblOK.IsEnabled = false;
        }

        private void btnOK_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnOK.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnOK.Source = new BitmapImage(source);
            }
        }

        private void btnOK_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnOK.Source = new BitmapImage(source);
        }

        private void btnOK_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnOK.Source = new BitmapImage(source);

            classInfo.storage.fileInfoStorage.selectedListboxItem = (ListBoxItem)listObjects.SelectedItem;

            FormFadeOut.Begin();
        }

        private void btnCan_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnCan.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnCan.Source = new BitmapImage(source);
            }
        }

        private void btnCan_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnCan.Source = new BitmapImage(source);
        }

        private void btnCan_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnCan.Source = new BitmapImage(source);

            classInfo.storage.fileInfoStorage.selectedListboxItem = null;

            FormFadeOut.Begin();
        }

        private void FormFadeOut_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count >= 1)
            {
                btnOK.IsEnabled = true;
                lblOK.IsEnabled = true;
            }
            else
            {
                btnOK.IsEnabled = false;
                lblOK.IsEnabled = false;
            }
        }

        private void listObjects_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Make sure we're actually clicking on an item
            if (listObjects.SelectedItem != null)
            {
                if (((ListBoxItem)listObjects.SelectedItem).IsMouseOver)
                {
                    classInfo.storage.fileInfoStorage.selectedListboxItem = (ListBoxItem)listObjects.SelectedItem;
                    FormFadeOut.Begin();
                }
            }
        }
    }
}