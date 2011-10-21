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
using X360.FATX;
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
	public partial class selectSaveOnDevice : UserControl, StepUI.IStep
	{
        private Folder[] _fileInjectDirec = new Folder[200];
        private int _index = 0;
        private File _selectedFile = null;
        private string _extractPath = null;

		public selectSaveOnDevice()
		{
			this.InitializeComponent();
		}

        public File SelectedFile
        {
            get { return _selectedFile; }
        }

        public string ExtractedFilePath
        {
            get { return _extractPath; }
        }

        public static readonly DependencyProperty DriveProperty =
            DependencyProperty.Register("Drive", typeof(FATX.FATXDrive), typeof(selectSaveOnDevice));

        public FATX.FATXDrive Drive
        {
            get { return (FATX.FATXDrive)GetValue(DriveProperty); }
            set { SetValue(DriveProperty, value); }
        }

        public void Load(Util.SaveEditor saveEditor)
        {
			_index = 0;
			_fileInjectDirec = new Folder[200];
			cBSaves.Items.Clear();
			
            Drive.ReadData();
            Folder[] Partitions;
            Folder contentDirec;
            string profileID;

            if (Drive.IsUSB)
            {
                Partitions = Drive.Partitions;
                contentDirec = Partitions[1];
            }
            else
            {
                Partitions = Drive.Partitions;
                contentDirec = Partitions[2]; //who uses hdd's for modding gamesaves, are you a fucking egit?
            }

            foreach (Folder content in contentDirec.SubFolders(false))
            {
                if (content.Name == "Content")
                {
                    foreach (Folder profiles in content.SubFolders(false))
                    {
                        if (profiles.Name.StartsWith("E"))
                        {
                            profileID = profiles.Name;
                            foreach (Folder profileContent in profiles.SubFolders(false))
                            {
                                if (profileContent.Name == "4D53085B")
                                {
                                    foreach (Folder reachSub1 in profileContent.SubFolders(false))
                                    {
                                        foreach (File reachSub2 in reachSub1.Files(false))
                                        {
                                            if (reachSub2.Name.StartsWith("s"))
                                            {
                                                ComboBoxItem cbi = new ComboBoxItem();
                                                cbi.Content = profileID + " - " + reachSub2.Name;
                                                cbi.Tag = reachSub2;
                                                _fileInjectDirec[_index++] = reachSub1;
                                                cBSaves.Items.Add(cbi);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            cBSaves.SelectedIndex = 0;
        }

        public bool Save(Util.SaveEditor saveEditor)
        {
            int i = cBSaves.SelectedIndex;
            int j = 0;
            foreach (ComboBoxItem cbi in cBSaves.Items)
            {
                if (j == i)
                {
                    File x = (File)cbi.Tag;
                    
                    bool cancel = false;
                    string tempDir = classInfo.extraIO.makeTempSaveDir();
                    _extractPath = tempDir + x.Name;
                    x.Extract(_extractPath, ref cancel);
                    
                    saveEditor.LoadSTFS(_extractPath, tempDir);
                    _selectedFile = x;
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
            Load(null);
        }
    }
}