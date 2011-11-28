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

namespace Liberty.eggData
{
    /// <summary>
    /// Interaction logic for InitCODVerification.xaml
    /// </summary>
    public partial class InitCODVerification : Window
    {
        public InitCODVerification()
        {
            this.InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            FormFadeOut.Begin();
            classInfo.applicationExtra.disableInput(this);
        }

        private void FormFadeOut_Completed(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}