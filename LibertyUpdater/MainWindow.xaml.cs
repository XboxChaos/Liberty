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
using System.Runtime.InteropServices;

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

        [DllImport("gdi32", EntryPoint = "AddFontResource")]
        public static extern int AddFontResourceA(string lpFileName);

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
            Liberty.classInfo.iniFile iniFile = new Liberty.classInfo.iniFile("update.ini");
            string PIDAppName = iniFile.IniReadValue("AppInfo", "appName");

            //Tell the user we are checking for their derpness
            output = "Checking if " + PIDAppName + " is closed...";
            System.Threading.Thread.Sleep(threadTime);

            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                if (process.ProcessName == PIDAppName || process.ProcessName == PIDAppName + ".vshost")
                {
                    //Closing what the user re-opened -__-
                    output = "Closing " + PIDAppName + "...";
                    System.Threading.Thread.Sleep(threadTime);
                    process.Kill();
                }
            }

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
                string downloadedInfo = wb.DownloadString(new Uri("http://xeraxic.com/downloads/checkVersionInfo.php?appName=" + PIDAppName + "&appVer=1"));
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

                        if (File.Exists(appDir)) { File.Delete(appDir); }
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

                output = "Installing Fonts...";
                System.Threading.Thread.Sleep(threadTime);

                tempFonts(PIDAppName);
                string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\" + PIDAppName + "\\fonts\\";
                string[] fonts = new string[17];
                #region fontnaming
                {
                    File.WriteAllBytes(fontPath + "segoeui.ttf", LibertyUpdater.Properties.Resources.segoeui);
                    fonts[0] = fontPath + "segoeui.ttf";

                    File.WriteAllBytes(fontPath + "segoeuib.ttf", LibertyUpdater.Properties.Resources.segoeuib);
                    fonts[1] = fontPath + "segoeuib.ttf";

                    File.WriteAllBytes(fontPath + "segoeuii.ttf", LibertyUpdater.Properties.Resources.segoeuii);
                    fonts[2] = fontPath + "segoeuii.ttf";

                    File.WriteAllBytes(fontPath + "segoeuil.ttf", LibertyUpdater.Properties.Resources.segoeuil);
                    fonts[3] = fontPath + "segoeuil.ttf";

                    File.WriteAllBytes(fontPath + "SegoeUIMono_Bold.ttf", LibertyUpdater.Properties.Resources.SegoeUIMono_Bold);
                    fonts[4] = fontPath + "SegoeUIMono_Bold.ttf";

                    File.WriteAllBytes(fontPath + "SegoeUIMono_Regular.ttf", LibertyUpdater.Properties.Resources.SegoeUIMono_Regular);
                    fonts[5] = fontPath + "SegoeUIMono_Regular.ttf";

                    File.WriteAllBytes(fontPath + "segoeuiz.ttf", LibertyUpdater.Properties.Resources.segoeuiz);
                    fonts[6] = fontPath + "segoeuiz.ttf";

                    File.WriteAllBytes(fontPath + "SegoeWP.ttf", LibertyUpdater.Properties.Resources.SegoeWP);
                    fonts[7] = fontPath + "SegoeWP.ttf";

                    File.WriteAllBytes(fontPath + "SegoeWP_Black.ttf", LibertyUpdater.Properties.Resources.SegoeWP_Black);
                    fonts[8] = fontPath + "SegoeWP_Black.ttf";

                    File.WriteAllBytes(fontPath + "SegoeWP_Bold.ttf", LibertyUpdater.Properties.Resources.SegoeWP_Bold);
                    fonts[9] = fontPath + "SegoeWP_Bold.ttf";

                    File.WriteAllBytes(fontPath + "SegoeWP_Light.ttf", LibertyUpdater.Properties.Resources.SegoeWP_Light);
                    fonts[10] = fontPath + "SegoeWP_Light.ttf";

                    File.WriteAllBytes(fontPath + "SegoeWP_Semibold.ttf", LibertyUpdater.Properties.Resources.SegoeWP_Semibold);
                    fonts[11] = fontPath + "SegoeWP_Semibold.ttf";

                    File.WriteAllBytes(fontPath + "SegoeWP_Semilight.ttf", LibertyUpdater.Properties.Resources.SegoeWP_Semilight);
                    fonts[12] = fontPath + "SegoeWP_Semilight.ttf";

                    File.WriteAllBytes(fontPath + "seguisb.ttf", LibertyUpdater.Properties.Resources.seguisb);
                    fonts[13] = fontPath + "seguisb.ttf";

                    File.WriteAllBytes(fontPath + "seguisym.ttf", LibertyUpdater.Properties.Resources.seguisym);
                    fonts[14] = fontPath + "seguisym.ttf";

                    File.WriteAllBytes(fontPath + "Semilight.ttf", LibertyUpdater.Properties.Resources.Semilight);
                    fonts[15] = fontPath + "Semilight.ttf";
                }
                #endregion


                for (int j = 0; j < 16; j++)
                {
                    fi = new FileInfo(fonts[j]);
                    output = "Installing Font: " + fi.Name;
                    System.Threading.Thread.Sleep(250);
                    try { AddFontResourceA(fonts[j]); } catch { }
                }
                try { Directory.Delete(fontPath, true); } catch{  }

                output = "Loading " + PIDAppName + "...";
                System.Threading.Thread.Sleep(threadTime);

                Process.Start(appDir);

                output = "Closing Updater...";
                System.Threading.Thread.Sleep(threadTime);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: \n\n" + ex.Message, PIDAppName + " Updater - Error");
                System.Threading.Thread.Sleep(1000);
            }

            finished = true;
        }

        public void tempFonts(string pid)
        {
            string temp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\" + pid + "\\fonts\\";
            if (!Directory.Exists(temp)) { Directory.CreateDirectory(temp); }
        }


    }
}