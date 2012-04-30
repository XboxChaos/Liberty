using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using XCI.Backend.Cryptography;

namespace Liberty.Controls
{
    /// <summary>
    /// Interaction logic for dat_super_secret_app.xaml
    /// </summary>
    public partial class dat_super_secret_app : Window
    {
        private string _AESKey;
        private string _DDL = "XC1zVfVGt3bIJU4m3Z6GZw0x+QUlHBUmi+DF7v4tNp8=";

        public dat_super_secret_app(string AESKey)
        {
            this.InitializeComponent();

            _AESKey = AESKey;

            lblSubInfo.Text = AESCrypto.DecryptData(lblSubInfo.Text, _AESKey);
            lblTitle.Text = AESCrypto.DecryptData(lblTitle.Text, _AESKey).ToUpper();
            _DDL = AESCrypto.DecryptData(_DDL, _AESKey);
        }

        private void btnDL_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(_DDL);
            FormFadeOut.Begin();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            FormFadeOut.Begin();
        }

        private void FormFadeOut_Completed(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}