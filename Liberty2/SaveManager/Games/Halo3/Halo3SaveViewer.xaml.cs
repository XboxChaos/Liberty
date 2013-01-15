using Liberty.Backend;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

namespace Liberty.SaveManager.Games.Halo3
{
    /// <summary>
    /// Interaction logic for Halo3SaveViewer.xaml
    /// </summary>
    public partial class Halo3SaveViewer : UserControl, ISaveViewer, INotifyPropertyChanged
    {
        private SaveLocalStorage _saveStorage;
        private CampaignSave _campaignSave;
        private string _mmiofPath;
        private TabItem _tabParent;

        public CampaignSave CampaignSave
        {
            get { return _campaignSave; }
            set { _campaignSave = value; NotifyPropertyChanged("CampaignSave"); }
        }

        public SaveLocalStorage SaveStorage
        {
            get { return _saveStorage; }
            private set { _saveStorage = value; }
        }

        public Halo3SaveViewer(SaveLocalStorage saveStorage, TabItem tabParent)
        {
            // CampaignSave.PlayerBiped.Position.X.Biped.Position.X

            InitializeComponent();

            // Store SaveStorage
            _saveStorage = saveStorage;

            // Store Parent Tab
            _tabParent = tabParent;

            // Load Campaign Save
            Load();

            // Write Header (Game Name - Level - Gamertag)
            tabParent.Header = string.Format("Halo 3 - {0} - {1}", Utilities.GetFriendlyMissionName(_campaignSave.Header.Map, Utilities.HaloGames.Halo3), _campaignSave.Header.Gamertag);

            // Set DataContext
            this.DataContext = this;
        }


        public bool Close()
        {
            // Remove save from OpenedSaves list :(, leaving so soon?
            Settings.OpenedSaves.Remove(_saveStorage.USID);

            // Now tell the HomeWindow it's all safe to close up shop
            return true;
        }
        public void Load()
        {
            // Extract the mmiof.bmf
            _mmiofPath = VariousFunctions.CreateTemporaryFile(VariousFunctions.GetTemporaryExtractionLocation());
            X360.STFS.STFSPackage package = new X360.STFS.STFSPackage(_saveStorage.SaveLocalPath, null);
            package.GetFile("mmiof.bmf").Extract(_mmiofPath);

            // Open the mmiof.bmf
            _campaignSave = new CampaignSave(_mmiofPath);

            // Update User Interface


            // Add Save to OpenSaves list
            Settings.OpenedSaves.Add(_saveStorage.USID);

            // Reload FATX Devices
            Settings.StartPage.LoadSavesFromDevices();
        }

        #region BasicPlayerInfo
        public void LoadBasicPlayerInfo()
        {

        }

        #endregion

        public void Save()
        {
            if (_campaignSave != null && _mmiofPath != null)
            {
                // Save Campaign Save
                _campaignSave.Update(_mmiofPath);

                // Update STFS Package
                X360.STFS.STFSPackage package = new X360.STFS.STFSPackage(_saveStorage.SaveLocalPath, null);
                package.GetFile("mmiof.bmf").Replace(_mmiofPath);

                // Rehash/Resign Package

                // Do we need to do any FATX stuff?
                if (_saveStorage.IsSaveFromFATX)
                {
                    // Yes :(
                    _saveStorage.FATXDrive.Open();
                    CLKsFATXLib.File saveFile = _saveStorage.FATXDrive.FileFromPath(_saveStorage.FATXPath);
                    CLKsFATXLib.Folder saveFolder = saveFile.Parent;
                    string fileName = saveFile.Name;
                    
                    // TODO: This
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
