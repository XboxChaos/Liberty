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
	public partial class openSaveFile : UserControl, StepUI.IStep
	{
        private MainWindow _mainWindow = null;
        private Util.SaveEditor _saveEditor = null;
        private bool _loaded;

		public openSaveFile()
		{
			this.InitializeComponent();

            this.Loaded += new RoutedEventHandler(openSaveFile_Loaded);
		}

        void openSaveFile_Loaded(object sender, RoutedEventArgs e)
        {
            _mainWindow = Window.GetWindow(this) as MainWindow;
        }

        public void Load(Util.SaveEditor saveEditor)
        {
            _loaded = saveEditor.Loaded;
            if (!_loaded)
                lblFileDirec.Text = "please load a file...";
            _saveEditor = saveEditor;
        }

        public bool Save(Util.SaveEditor saveEditor)
        {
            return _loaded;
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            Visibility = Visibility.Hidden;
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open Campaign Save";
            ofd.Filter = "Halo: Reach Campaign STFS Packages|*";
            Nullable<bool> result = ofd.ShowDialog();
            if (result.HasValue && result.Value)
            {
                try
                {
                    _saveEditor.LoadSTFS(ofd.FileName, classInfo.extraIO.makeTempSaveDir());
                    lblFileDirec.Text = ofd.FileName;
                    _loaded = true;
                }
                catch (Exception ex)
                {
                    // An error occurred trying to load the file, display it to the user
                    _mainWindow.showException(ex.ToString());
                    _loaded = false;
                }
            }
        }
    }
}