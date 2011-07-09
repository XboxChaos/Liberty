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
using Liberty.classInfo.storage;
using X360.FATX;

namespace Liberty.Controls
{
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(string message)
        {
            Message = message;
        }

        public string Message;
    }

	/// <summary>
	/// Interaction logic for step0_1.xaml
	/// </summary>
	public partial class step0_1 : UserControl
	{
        public event EventHandler ExecuteMethod;
        public event EventHandler<MessageEventArgs> ExecuteMethodLocal;
        FATX.FATXDrive[] physDrives = null;
		public step0_1()
		{
			this.InitializeComponent();
		}

        protected virtual void OnExecuteMethod()
        {
            if (ExecuteMethod != null)
                ExecuteMethod(this, EventArgs.Empty);
        }
        protected virtual void OnExecuteMethodLocal(string message)
        {
            if (ExecuteMethodLocal != null)
                ExecuteMethodLocal(this, new MessageEventArgs(message));
        }

		public void loadData()
		{
            saveIsLocal.Visibility = System.Windows.Visibility.Hidden;
            saveIsNotLocal.Visibility = System.Windows.Visibility.Hidden;

            if (classInfo.storage.fileInfoStorage.saveIsLocal)
            {
                title.Content = title.Content.ToString().Replace("{0}", "Computer");
                subTitle.Content = subTitle.Content.ToString().Replace("{0}", "Computer");
                saveIsLocal.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                title.Content = title.Content.ToString().Replace("{0}", "External Storage Device");
                subTitle.Content = subTitle.Content.ToString().Replace("{0}", "External Storage Device");
                saveIsNotLocal.Visibility = System.Windows.Visibility.Visible;

                updateFATX();
            }
		}
		
		public bool saveData()
		{
            if (cBStorageType.SelectedIndex == -1)
            {
                return false;
            }
            else
            {
                if (!fileInfoStorage.saveIsLocal)
                {
                    fileInfoStorage.xChosenDrive = physDrives[cBStorageType.SelectedIndex];

                    if (physDrives[cBStorageType.SelectedIndex].IsUSB)
                    {
                        fileInfoStorage.driveType = 0;
                        fileInfoStorage.driveName = physDrives[cBStorageType.SelectedIndex].DriveName.ToString();
                    }
                    else
                        fileInfoStorage.driveType = 1;

                    for (int i = 0; i < physDrives.Length; i++)
                    {
                        if (i != cBStorageType.SelectedIndex)
                            physDrives[i].Close();
                    }
                }
                return true;
            }
		}


        public void updateFATX()
        {
			cBStorageType.Items.Clear();
            if (physDrives != null)
            {
                foreach (FATX.FATXDrive drive in physDrives)
                    drive.Close();
                physDrives = null;
            }
			
            //Create a new instance of the IO class
            FATX.IO.HDDFATX io = new FATX.IO.HDDFATX(false, null);
            //Get the drives
            physDrives = io.GetFATXDrives(32);
            FATX.Misc r = new FATX.Misc();

            //For each index in our drives
            int i=0;
            while (i < physDrives.Length)
            {
                string Type = physDrives[i].DriveName;
                ComboBoxItem hdd = new ComboBoxItem();
                hdd.Content = physDrives[i].DriveType.ToString() + " - " + physDrives[i].DriveName.ToString() + " (" + physDrives[i].DriveSizeConverted + ")";
                hdd.Tag = physDrives[i];
                cBStorageType.Items.Add(hdd);
                i++;
            }

            if (i == 0)
                OnExecuteMethod();
            else
                cBStorageType.SelectedIndex = 0;
        }

		#region wpf bullshit
        #region btnOpenwpf
        private void btnOpen_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnOpen.Source = new BitmapImage(source);

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Liberty - Open campaign STFS Package";
            ofd.Filter = "Halo Reach Campaign STFS package|*";

            Nullable<bool> result = ofd.ShowDialog();

            if ((bool)result)
            {
                string isValid = classInfo.stfsCheck.checkSTFSPackage(ofd.FileName);
                if (isValid == "yes")
                {
                    lblFileDirec.Content = ofd.FileName;
                }
                else
                {
                    OnExecuteMethodLocal(isValid);
                }
            }
        }

        private void btnOpen_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnOpen.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnOpen.Source = new BitmapImage(source);
            }
        }

        private void btnOpen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnOpen.Source = new BitmapImage(source);
        }
        #endregion

        #region btnRefreshwpf
        private void btnOpen_MouseUp1(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnOpen1.Source = new BitmapImage(source);

            updateFATX();
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
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnOpen1.Source = new BitmapImage(source);
        }
        #endregion
        #endregion
    }
}