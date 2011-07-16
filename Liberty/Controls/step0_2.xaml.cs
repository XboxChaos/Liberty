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

namespace Liberty.Controls
{
	/// <summary>
	/// Interaction logic for step0_2.xaml
	/// </summary>
	public partial class step0_2 : UserControl
	{
		public step0_2()
		{
			this.InitializeComponent();
		}

        Folder[] fileInjectDirec = new Folder[200];
        int index = 0;
        public void loadData()
        {
			index = 0;
			fileInjectDirec = new Folder[200];
			cBSaves.Items.Clear();
			
            fileInfoStorage.xChosenDrive.ReadData();
            Folder[] Partitions;
            Folder contentDirec;
            string profileID;
            
            if (fileInfoStorage.xChosenDrive.IsUSB)
            {
                #region USBPartitiooon
                Partitions = fileInfoStorage.xChosenDrive.Partitions;
                contentDirec = Partitions[1];
                #endregion
            }
            else
            {
                #region nonUSBPartitoooon
                Partitions = fileInfoStorage.xChosenDrive.Partitions;
                contentDirec = Partitions[2]; //who uses hdd's for modding gamesaves, are you a fucking egit?
                #endregion
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
                                                fileInjectDirec[index] = reachSub1;
                                                index++;
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

        public void saveData()
        {
            int i = cBSaves.SelectedIndex;
            int j = 0;
            foreach (ComboBoxItem cbi in cBSaves.Items)
            {
                if (j == i)
                {
                    File x = (File)cbi.Tag;
                    classInfo.applicationExtra.getTempSaveExtraction(x);
                    fileInfoStorage.oldFileInFolder = x;
                    fileInfoStorage.folderToInjectDevice = fileInjectDirec[cBSaves.SelectedIndex];
                }
                j++;
            }
        }
		
		#region wpf bullshit
        #region btnRefreshwpf
        private void btnOpen_MouseUp1(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnOpen1.Source = new BitmapImage(source);

            loadData();
        }

        private void btnOpen_IsMouseDirectlyOverChanged1(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnOpen1.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnOpen1.Source = new BitmapImage(source);
            }
        }

        private void btnOpen_MouseDown1(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
            btnOpen1.Source = new BitmapImage(source);
        }
        #endregion
        #endregion
	}
}