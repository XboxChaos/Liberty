using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Threading;

namespace Liberty.Util
{
    public class FATXSaveTransferrer : ISaveTransferrer
    {
        public void Start()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            _timer.Tick += new EventHandler(timer_Tick);

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Queues a file to be transferred.
        /// </summary>
        /// <param name="drive">The name of the drive which the file will be sent to (used for display purposes).</param>
        /// <param name="path">The path to the file to transfer.</param>
        /// <param name="file">The File to overwrite.</param>
        public void QueueFile(string path, FATX.File file)
        {
            string drive = FormatDeviceName(file.Drive);
            KeyValuePair<string, FATX.File> pair = new KeyValuePair<string, FATX.File>(path, file);
            List<KeyValuePair<string, FATX.File>> driveFiles;

            if (_files.TryGetValue(drive, out driveFiles))
            {
                driveFiles.Add(pair);
            }
            else
            {
                driveFiles = new List<KeyValuePair<string, FATX.File>>();
                driveFiles.Add(pair);
                _files.Add(drive, driveFiles);
            }
        }

        /// <summary>
        /// Cancels all pending transfers.
        /// Note that any transfers in progress will NOT be stopped.
        /// </summary>
        public void CancelAll()
        {
            _files.Clear();
        }

        /// <summary>
        /// Returns a user-friendly name for a FATXDrive.
        /// </summary>
        /// <param name="drive">The FATXDrive to get the name of.</param>
        /// <returns>A user-friendly display string for the device.</returns>
        public static string FormatDeviceName(FATX.FATXDrive drive)
        {
            return drive.DriveType.ToString() + " - " + drive.DriveName.ToString() + " (" + drive.DriveSizeConverted + ")";
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (KeyValuePair<string, List<KeyValuePair<string, FATX.File>>> drive in _files)
            {
                foreach (KeyValuePair<string, FATX.File> file in drive.Value)
                    TransferFile(drive.Key, file.Key, file.Value);
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                throw e.Error;

            OnDone();
        }

        private void TransferFile(string drive, string path, FATX.File file)
        {
            _currentFile = file;
            _currentFileDrive = drive;

            // Dispatch the OnNextFile event
            object[] args = new object[3];
            args[0] = drive;
            args[1] = file.Name;
            args[2] = (int)file.Size;
            _timer.Dispatcher.BeginInvoke(new Action<string, string, int>(OnNextFile), args);

            // Start the timer and overwrite the file
            _timer.Start();
            file.OverWrite(path, ref _overwriteProgress, ref _overwriteTotal, ref _overwriteCancel);
            _timer.Stop();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            // Send an OnProgressChanged event
            int size = (int)_currentFile.Size;
            int clusterSize = (int)_currentFile.PartInfo.ClusterSize;
            OnProgressChanged(Math.Min(_overwriteProgress * clusterSize, size), size);
        }

        protected void OnNextFile(string deviceName, string fileName, int size)
        {
            if (NextFile != null)
            {
                TransferFileEventArgs args = new TransferFileEventArgs(deviceName, fileName, size);
                NextFile(this, args);
            }
        }

        protected void OnProgressChanged(int transferred, int total)
        {
            if (ProgressChanged != null)
            {
                TransferProgressEventArgs args = new TransferProgressEventArgs(transferred, total);
                ProgressChanged(this, args);
            }
        }

        protected void OnDone()
        {
            if (Done != null)
                Done(this, EventArgs.Empty);
        }

        public event EventHandler<TransferFileEventArgs> NextFile;
        public event EventHandler<TransferProgressEventArgs> ProgressChanged;
        public event EventHandler Done;

        private Dictionary<string, List<KeyValuePair<string, FATX.File>>> _files =
            new Dictionary<string,List<KeyValuePair<string,FATX.File>>>();

        private bool _overwriteCancel = false;
        private int _overwriteProgress;
        private int _overwriteTotal;
        private string _currentFileDrive;
        private FATX.File _currentFile;
        private DispatcherTimer _timer;
    }
}
