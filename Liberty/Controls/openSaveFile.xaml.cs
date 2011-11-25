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
        private Util.SaveManager<Reach.CampaignSave> _saveManager;
        private Reach.TagListManager _taglistManager;
        private bool _loaded;

        public openSaveFile(Util.SaveManager<Reach.CampaignSave> saveManager, Reach.TagListManager taglistManager)
        {
            _saveManager = saveManager;
            _taglistManager = taglistManager;
            this.InitializeComponent();

            this.Loaded += new RoutedEventHandler(openSaveFile_Loaded);
        }

        void openSaveFile_Loaded(object sender, RoutedEventArgs e)
        {
            _mainWindow = Window.GetWindow(this) as MainWindow;
        }

        public void Load()
        {
            _loaded = _saveManager.Loaded;
            if (!_loaded)
                lblFileDirec.Text = "please load a file...";
        }

        public bool Save()
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
                    _saveManager.LoadSTFS(ofd.FileName, classInfo.extraIO.makeTempSaveDir());
                    _taglistManager.RemoveMapSpecificTaglists();
                    lblFileDirec.Text = ofd.FileName;
                    _loaded = true;
                }
                catch (ArgumentException ex)
                {
                    string message;
                    if (ex.InnerException != null)
                        message = ex.InnerException.Message;
                    else
                        message = ex.Message;

                    _mainWindow.showMessage(message, "ERROR");
                    _loaded = false;
                }
                catch (Exception ex)
                {
                    _mainWindow.showException(ex.ToString());
                    _loaded = false;
                }
            }
        }
    }
}