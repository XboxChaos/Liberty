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
	/// <summary>
	/// Interaction logic for step0_1.xaml
	/// </summary>
	public partial class selectDevice : UserControl, StepUI.IStep
	{
        private MainWindow _mainWindow = null;
        private FATX.FATXDrive[] _physDrives = null;
        private FATX.FATXDrive _selectedDrive = null;

		public selectDevice()
		{
			this.InitializeComponent();

            this.Loaded += new RoutedEventHandler(selectDevice_Loaded);
        }

        void selectDevice_Loaded(object sender, RoutedEventArgs e)
        {
            _mainWindow = Window.GetWindow(this) as MainWindow;
        }

        public FATX.FATXDrive SelectedDevice
        {
            get { return _selectedDrive; }
        }

		public void Load()
		{
            updateFATX();
		}
		
		public bool Save()
		{
            for (int i = 0; i < _physDrives.Length; i++)
            {
                if (i == cBStorageType.SelectedIndex)
                    _selectedDrive = _physDrives[i];
                else
                    _physDrives[i].Close();
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

        private void updateFATX()
        {
			    cBStorageType.Items.Clear();
                if (_physDrives != null)
                {
                    foreach (FATX.FATXDrive drive in _physDrives)
                        drive.Close();
                    _physDrives = null;
                }
			
                //Create a new instance of the IO class
                FATX.IO.HDDFATX io = new FATX.IO.HDDFATX(false, null);
                //Get the drives
                _physDrives = io.GetFATXDrives(32);
                FATX.Misc r = new FATX.Misc();

                //For each index in our drives
                int i=0;
                while (i < _physDrives.Length)
                {
                    string Type = _physDrives[i].DriveName;
                    ComboBoxItem hdd = new ComboBoxItem();
                    hdd.Content = Util.FATXSaveTransferrer.FormatDeviceName(_physDrives[i]);
                    hdd.Tag = _physDrives[i];
                    cBStorageType.Items.Add(hdd);
                    i++;
                }

                if (i == 0)
                {
                    cBStorageType.IsEnabled = false;
                    _mainWindow.showMessage("Liberty could not detect any FATX devices. Try re-connecting them and pressing Refresh. Also, check that you don't have any other FATX browsers open.", "NO FATX DEVICES");
                    _mainWindow.enableNextButton(false);
                }
                else
                {
                    cBStorageType.IsEnabled = true;
                    cBStorageType.SelectedIndex = 0;
                    _mainWindow.enableNextButton(true);
                }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            updateFATX();
        }
    }
}