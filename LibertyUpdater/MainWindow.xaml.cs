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
using System.Diagnostics;
using System.Net;
using System.IO;

namespace LibertyUpdater
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        System.Threading.Thread Worker;
        static System.Windows.Threading.DispatcherTimer FrontEnd = new System.Windows.Threading.DispatcherTimer();
        System.Threading.Thread TimerThread = new System.Threading.Thread(new System.Threading.ThreadStart(FrontEnd.Start));
        int threadTime = 500;
        string currentData = "";
        bool finishedUpdating;

		public MainWindow()
		{
			this.InitializeComponent();

			// Insert code required on object creation below this point.

            FrontEnd.Tick += new EventHandler(FrontEnd_Tick);
            FrontEnd.Interval = new TimeSpan(0, 0, 0, 0, 700);
            System.Threading.ThreadStart ts = delegate { doOveride(); };
            Worker = new System.Threading.Thread(ts);
            FrontEnd.Start();
            Worker.Start();
		}

        void FrontEnd_Tick(object sender, EventArgs e)
        {
            lblUpdatingText.Text = currentData;

            if (lblTitle.Text == "UPDATING ") { lblTitle.Text = "UPDATING ."; }
            else if (lblTitle.Text == "UPDATING .") { lblTitle.Text = "UPDATING .."; }
            else if (lblTitle.Text == "UPDATING ..") { lblTitle.Text = "UPDATING ..."; }
            else if (lblTitle.Text == "UPDATING ...") { lblTitle.Text = "UPDATING "; }

            if (finishedUpdating) { this.FormFadeOut.Begin(); } else { FrontEnd.Start(); }
        }

        public void doOveride()
        {
            updaterCode(ref currentData, ref finishedUpdating);
        }

        private void FormFadeOut_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        public void updaterCode(ref string output, ref bool finished)
        {
            //Tell the user we are checking for their derpness
            output = "Checking if Liberty is closed...";
            System.Threading.Thread.Sleep(threadTime);

            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                if (process.ProcessName == "Liberty" || process.ProcessName == "Liberty.vshost")
                {
                    //Closing what the user re-opened -__-
                    output = "Closing Liberty...";
                    System.Threading.Thread.Sleep(threadTime);
                    process.Kill();
                }
            }

            string temp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Liberty\\update\\";
            Liberty.classInfo.iniFile iniFile = new Liberty.classInfo.iniFile(temp + "update.ini");

            output = "Reading Update INI File";
            System.Threading.Thread.Sleep(threadTime);

            string appName = iniFile.IniReadValue("AppInfo", "appName");
            string appVer = iniFile.IniReadValue("AppInfo", "appVer");
            string appDir = iniFile.IniReadValue("AppInfo", "appDir");

            output = "Reading Update Database";
            System.Threading.Thread.Sleep(threadTime);
            try
            {
                WebClient wb = new WebClient();
                string downloadedInfo = wb.DownloadString(new Uri("http://xeraxic.com/downloads/checkVersionInfo.php?appName=Liberty&appVer=1"));
                downloadedInfo = downloadedInfo.Replace("\r", "");
                string[] updateData = downloadedInfo.Split('\n');


                output = "Sorting Returned Values...";
                System.Threading.Thread.Sleep(threadTime);

                FileInfo fi = new FileInfo(appDir);
                string appDirec = fi.Directory.ToString();

                output = "Downloading Database Files...";
                System.Threading.Thread.Sleep(threadTime);

                int i = 0;
                foreach (string line in updateData)
                {
                    if (i == 0)
                    {
                        output = "Downloading Application Exe...";
                        System.Threading.Thread.Sleep(threadTime);

                        if (File.Exists(appDir)) { try { File.Delete(appDir); } catch { } }
                        wb.DownloadFile(new Uri(line), appDir);
                    }
                    else
                    {
                        output = "Downloading " + line;
                        System.Threading.Thread.Sleep(threadTime);

                        if (File.Exists(appDirec + "\\" + line)) { try { File.Delete(appDirec + "\\" + line); } catch { } } //this was deleting the app. oops 
                        wb.DownloadFile(new Uri("http://xeraxic.com/downloads/lib/" + line), appDirec + "\\" + line);
                    }
                    i++;
                }

                output = "Loading Liberty...";
                System.Threading.Thread.Sleep(threadTime);

                Process.Start(appDir);

                output = "Closing Updater...";
                System.Threading.Thread.Sleep(threadTime);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: \n\n" + ex.Message, "Liberty Updater - Error");
                System.Threading.Thread.Sleep(1000);
            }

            finished = true;
        }
    }
}