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
using System.Net;

namespace Liberty.Controls
{
    /// <summary>
    /// Interaction logic for progressUpdaterDownload.xaml
    /// </summary>
    public partial class progressUpdaterDownload : Window
    {
        public progressUpdaterDownload()
        {
            InitializeComponent();

            string temp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Liberty\\update\\";
            using (WebClient wb = new WebClient())
            {
                wb.DownloadFileAsync(new Uri("http://xeraxic.com/downloads/updater.exe"), temp + "update.exe");
                wb.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(wb_DownloadFileCompleted);
                wb.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wb_DownloadProgressChanged);
            }
        }

        void wb_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            pBProgress.Value = e.ProgressPercentage;
        }

        void wb_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            this.FormFadeOut.Begin();
        }

        private void FormFadeOut_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void progBox_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (pBProgress.Value != 100)
                e.Cancel = true;
        }
    }
}
