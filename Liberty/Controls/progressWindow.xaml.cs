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
using Liberty.classInfo.storage;
using System.Threading;
using System.ComponentModel;
using FATX;

namespace Liberty.Controls
{
	/// <summary>
	/// Interaction logic for progressWindow.xaml
	/// </summary>
	public partial class progressWindow : Window
	{
        System.Threading.Thread Worker;
        static System.Windows.Threading.DispatcherTimer FrontEnd = new System.Windows.Threading.DispatcherTimer();
        System.Threading.Thread TimerThread = new System.Threading.Thread(new System.Threading.ThreadStart(FrontEnd.Start));

        int maxValue = 0;
        int progressValue = 0;

		public progressWindow()
		{
			this.InitializeComponent();

            FrontEnd.Tick +=new EventHandler(FrontEnd_Tick);
            FrontEnd.Interval = new TimeSpan(0, 0, 0, 0, 100);
            System.Threading.ThreadStart ts = delegate { doOveride(); };
            Worker = new System.Threading.Thread(ts);
            FrontEnd.Start();
            Worker.Start();
		}

        bool cancel = false;

        public void doOveride()
        {
            fileInfoStorage.oldFileInFolder.OverWrite(fileInfoStorage.fileOriginalDirectory, ref progressValue, ref maxValue, ref cancel);
        }

        void FrontEnd_Tick(object sender, EventArgs e)
        {
            try
            {
                // Set the maximium
                ProgressBarMax((int)maxValue);
                // Set the current value
                ProgressBarValue((int)progressValue);
            }
            catch { }

            if (maxValue == progressValue)
            {
                this.FormFadeOut.Begin();
            }
            else
            {
                FrontEnd.Start();
            }
        }

        #region Stuff for invoking
        private delegate void ProgressBarValue_Callback(int Value);
        private delegate void call_FormClose();
        void ProgressBarValue(int value)
        {
            pBProgress.Value = value;
        }

        void ProgressBarMax(int value)
        {
            pBProgress.Maximum = value;
        }
        #endregion

        private void FormFadeOut_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void progBox_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (maxValue != progressValue)
                e.Cancel = true;
        }
	}
}