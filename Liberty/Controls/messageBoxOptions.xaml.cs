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
    public partial class messageBoxOptions : Window
    {
        private bool _result = false;

        public messageBoxOptions(string message, string title)
        {
            this.InitializeComponent();

            lblSubInfo.Text = message;
            lblTitle.Text = title.ToUpper();
        }

        public bool result
        {
            get { return _result; }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            _result = true;
            FormFadeOut.Begin();
            classInfo.applicationExtra.disableInput(this);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _result = false;
            FormFadeOut.Begin();
            classInfo.applicationExtra.disableInput(this);
        }

        private void FormFadeOut_Completed(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}