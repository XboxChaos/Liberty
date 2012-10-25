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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Microsoft.Win32;

namespace Liberty.Controls
{
    /// <summary>
    /// Interaction logic for openSaveFile.xaml
    /// </summary>
    public partial class openSaveFile : UserControl, StepUI.IBranchStep<Util.SaveType>
    {
        private MainWindow _mainWindow;
        private Func<string, Util.SaveType> _loadSaveFunc;
        private bool _loaded = false;

        // NOTE: loadSaveFunc will be run in a separate thread!
        // Be sure to make use of Dispatcher if you need to update UI controls.
        public openSaveFile(Func<string, Util.SaveType> loadSaveFunc)
        {
            _loadSaveFunc = loadSaveFunc;

            this.InitializeComponent();
            this.Loaded += new RoutedEventHandler(openSaveFile_Loaded);
        }

        void openSaveFile_Loaded(object sender, RoutedEventArgs e)
        {
            _mainWindow = Window.GetWindow(this) as MainWindow;
        }

        /// <summary>
        /// Call this when the save file is unloaded.
        /// </summary>
        public void FileUnloaded()
        {
            _loaded = false;
        }

        public void Load()
        {
            if (!_loaded)
            {
                lblFileDirec.Text = "please load a file...";
                _mainWindow.enableNextButton(false);
            }
        }

        public bool Save()
        {
            return true;
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            Visibility = Visibility.Hidden;
        }

        public Util.SaveType SelectedBranch
        {
            get { return _saveType; }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open Campaign Save";
            ofd.Filter = "STFS Packages|*";
            bool? result = ofd.ShowDialog();
            if (result.HasValue && result.Value)
                LoadSave(ofd.FileName);
        }

        private void LoadSave(string path)
        {
            lblFileDirec.Text = "processing your save...";
            btnOpen.IsEnabled = false;
            _mainWindow.enableNextButton(false);
            _mainWindow.enableBackButton(false);

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.RunWorkerAsync(path);
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string fileName = (string)e.Argument;
            _saveType = _loadSaveFunc(fileName);
            e.Result = fileName;
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                string fileName = (string)e.Result;
                if (_saveType != Util.SaveType.Unknown)
                {
                    _loaded = true;
                    lblFileDirec.Text = fileName;
                }
                else
                {
                    _loaded = false;
                    _saveType = Util.SaveType.Unknown;
                    lblFileDirec.Text = "please load a file...";
                }
            }
            else
            {
                _mainWindow.showException(e.Error.ToString(), true);

                lblFileDirec.Text = "please load a file...";
                _saveType = Util.SaveType.Unknown;
                _loaded = false;
            }

            btnOpen.IsEnabled = true;
            _mainWindow.enableNextButton(_loaded);
            _mainWindow.enableBackButton(true);
        }

        private Util.SaveType _saveType = Util.SaveType.Reach;
    }
}