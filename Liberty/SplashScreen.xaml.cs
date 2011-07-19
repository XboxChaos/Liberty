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
using System.Windows.Forms;

namespace Liberty
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        Timer tmr = new Timer();

        public SplashScreen()
        {
            InitializeComponent();
            tmr.Interval = classInfo.storage.settings.applicationSettings.splashTimer * 1000;
            tmr.Tick += new EventHandler(tmr_Tick);

            tmr.Start();
        }

        void tmr_Tick(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
