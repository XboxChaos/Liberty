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
using System.Reflection;
using System.IO;
using System.Net;

namespace Liberty.Controls
{
	/// <summary>
	/// Interaction logic for updater.xaml
	/// </summary>
	public partial class updater : Window
	{
        private bool _startUpdate = false;
        private bool _connected = false;

		public updater()
		{
			this.InitializeComponent();
            try
            {
                lblBuildVer.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                lblBuildHash.Text = classInfo.extraIO.getMD5Hash(System.Environment.GetCommandLineArgs()[0]);
                FileInfo fi = new FileInfo(System.Environment.GetCommandLineArgs()[0]);
                DateTime dateAssembled = fi.CreationTime;
                lblBuildDate.Text = String.Format("{0:dd/MM/yyyy}", dateAssembled);
                WebClient wb = new WebClient();
                string downloadedInfo = wb.DownloadString(new Uri("http://xeraxic.com/downloads/checkVersionInfo.php?appName=Liberty&proVer=1"));

                downloadedInfo = downloadedInfo.Replace("\r", "");
                string[] updateData = downloadedInfo.Split('\n');

                lblSvrBuildHash.Text = updateData[1];
                lblSvrBuildVer.Text = updateData[0];

                int svrBuild = Convert.ToInt16(updateData[0].Replace(".", ""));
                int pcBuild = Convert.ToInt16(Assembly.GetExecutingAssembly().GetName().Version.ToString().Replace(".", ""));

                if (svrBuild > pcBuild)
                {
                    lblSvrBuildNewer.Text = "yes";
                    btnUpdate.IsEnabled = true;
                }
                else
                {
                    lblSvrBuildNewer.Text = "no";
                }
                _connected = true;
            }
            catch
            {
                this.Close();
            }
		}

        public bool startUpdate
        {
            get { return _startUpdate; }
        }

        private void update_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!_connected)
            {
                FormFadeOut.Begin();
                classInfo.applicationExtra.disableInput(this);
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            FormFadeOut.Begin();
            classInfo.applicationExtra.disableInput(this);
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            classInfo.updating.startUpdate();
            _startUpdate = true;

            FormFadeOut.Begin();
            classInfo.applicationExtra.disableInput(this);
        }

        private void FormFadeOut_Completed(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}