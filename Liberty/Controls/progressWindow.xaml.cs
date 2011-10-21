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
        System.Threading.Thread _worker;
        static System.Windows.Threading.DispatcherTimer _timer = new System.Windows.Threading.DispatcherTimer();
        System.Threading.Thread _timerThread = new System.Threading.Thread(new System.Threading.ThreadStart(_timer.Start));

        int _maxValue = 0;
        int _progressValue = 0;

        selectDevice _deviceSelector;
        selectSaveOnDevice _saveSelector;

		public progressWindow(selectDevice deviceSelector, selectSaveOnDevice saveSelector)
		{
			this.InitializeComponent();

            _deviceSelector = deviceSelector;
            _saveSelector = saveSelector;

            _timer.Tick += new EventHandler(FrontEnd_Tick);
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            System.Threading.ThreadStart ts = delegate { doOveride(); };
            _worker = new System.Threading.Thread(ts);
            _worker.Start();
		}

        bool cancel = false;

        public void doOveride()
        {
            _timer.Start();
            _saveSelector.SelectedFile.OverWrite(_saveSelector.ExtractedFilePath, ref _progressValue, ref _maxValue, ref cancel);
        }

        void FrontEnd_Tick(object sender, EventArgs e)
        {
            try
            {
                // Set the maximium
                ProgressBarMax((int)_maxValue);
                // Set the current value
                ProgressBarValue((int)_progressValue);
            }
            catch { }

            if (_maxValue == _progressValue)
            {
                _timer.Stop();
                this.FormFadeOut.Begin();
                classInfo.applicationExtra.disableInput(this);
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
            if (_maxValue != _progressValue)
                e.Cancel = true;
        }
	}
}