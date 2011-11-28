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
                lblFileDirec.Text = "please load a file...";
        }

        public bool Save()
        {
            if (!_loaded)
            {
                _mainWindow.showMessage("You need to select a save file before you can continue.", "HOLD ON!");
                return false;
            }
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
            ofd.Filter = "Halo: Reach Campaign STFS Packages|*";
            Nullable<bool> result = ofd.ShowDialog();
            if (result.HasValue && result.Value)
            {
                Util.SaveType type = _loadSaveFunc(ofd.FileName);
                if (type != Util.SaveType.Unknown)
                {
                    _loaded = true;
                    _saveType = type;
                    lblFileDirec.Text = ofd.FileName;
                }
            }
        }

        private Util.SaveType _saveType = Util.SaveType.Reach;
    }
}