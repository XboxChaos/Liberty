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
using X360.STFS;
using X360.Profile;
using X360.Security;
using FATX;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace Liberty.Controls
{
	/// <summary>
	/// Interaction logic for step0_2.xaml
	/// </summary>
	public partial class selectSaveOnDevice : UserControl, StepUI.IBranchStep<Util.SaveType>
	{
        private MainWindow _mainWindow = null;
        private Func<string, Util.SaveType> _loadSaveFunc;
        private Folder[] _fileInjectDirec = new Folder[200];
        private int _index = 0;
        private File _selectedFile = null;
        private string _extractPath = null;
        private string _tempDir = null;
        private selectDevice _selectDeviceStep;
        private Util.FATXSaveTransferrer _saveTransferrer;

        public selectSaveOnDevice(selectDevice selectDeviceStep, Util.FATXSaveTransferrer saveTransferrer, Func<string, Util.SaveType> loadSaveFunc)
		{
            _selectDeviceStep = selectDeviceStep;
            _saveTransferrer = saveTransferrer;
            _loadSaveFunc = loadSaveFunc;
			this.InitializeComponent();

            Loaded += new RoutedEventHandler(selectSaveOnDevice_Loaded);
		}

        void selectSaveOnDevice_Loaded(object sender, RoutedEventArgs e)
        {
            _mainWindow = Window.GetWindow(this) as MainWindow;
        }

        public File SelectedFile
        {
            get { return _selectedFile; }
        }

        public string ExtractedFilePath
        {
            get { return _extractPath; }
        }

        public string TempDirectory
        {
            get { return _tempDir; }
        }

        public void Load()
        {
			_index = 0;
			_fileInjectDirec = new Folder[200];
			cBSaves.Items.Clear();

            FATXDrive Drive = _selectDeviceStep.SelectedDevice;
            Drive.ReadData();
            Folder[] Partitions;
            Folder contentDirec;

            if (Drive.IsUSB)
            {
                Partitions = Drive.Partitions;
                contentDirec = Partitions[1];
            }
            else
            {
                Partitions = Drive.Partitions;
                contentDirec = Partitions[2];   //who uses hdd's for modding gamesaves, are you a fucking egit?
                                                // LOL -Xerax
            }

            SortedList<string, ComboBoxItem> items = new SortedList<string, ComboBoxItem>();
            foreach (Folder content in contentDirec.SubFolders(false))
            {
                if (content.Name == "Content")
                {
                    foreach (Folder profiles in content.SubFolders(false))
                    {
                        if (profiles.Name.StartsWith("E"))
                        {
                            foreach (Folder profileContent in profiles.SubFolders(false))
                            {
                                if (profileContent.Name == "4D53085B" || profileContent.Name == "4D5309B1" || profileContent.Name == "4D5307E6")
                                {
                                    foreach (Folder reachSub1 in profileContent.SubFolders(false))
                                    {
                                        foreach (File reachSub2 in reachSub1.Files(false))
                                        {
                                            if (reachSub2.Name.StartsWith("s") || reachSub2.Name.StartsWith("personal"))
                                            {
                                                ComboBoxItem cbi = new ComboBoxItem();
                                                reachSub2.ForceSTFSInfo();
                                                string text = reachSub2.STFSInformation.TitleName + " - " + reachSub2.GetPackageName() + " - " + reachSub2.Name;
                                                cbi.Content = text;
                                                cbi.Tag = reachSub2;
                                                _fileInjectDirec[_index++] = reachSub1;
                                                items.Add(text, cbi);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach (KeyValuePair<string, ComboBoxItem> item in items)
                cBSaves.Items.Add(item.Value);
            cBSaves.SelectedIndex = 0;
        }

        public bool Save()
        {
            int i = cBSaves.SelectedIndex;
            int j = 0;
            foreach (ComboBoxItem cbi in cBSaves.Items)
            {
                if (j == i)
                {
                    try
                    {
                        _selectedFile = (File)cbi.Tag;
                        bool cancel = false;
                        _tempDir = classInfo.extraIO.makeTempSaveDir();
                        _extractPath = _tempDir + _selectedFile.Name;
                        _selectedFile.Extract(_extractPath, ref cancel);
                        Util.SaveType type = _loadSaveFunc(_extractPath);
                        if (type != Util.SaveType.Unknown)
                        {
                            _saveType = type;
                            _saveTransferrer.CancelAll();
                            _saveTransferrer.QueueFile(_extractPath, _selectedFile);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        string message;
                        if (ex.InnerException != null)
                            message = ex.InnerException.Message;
                        else
                            message = ex.Message;

                        _mainWindow.showMessage(message, "ERROR");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        _mainWindow.showException(ex.ToString());
                        return false;
                    }
                }
                j++;
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
		
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Load();
        }

        public Util.SaveType SelectedBranch
        {
            get { return _saveType; }
        }

        private Util.SaveType _saveType = Util.SaveType.Reach;
    }
}