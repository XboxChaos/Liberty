using Liberty.IO;
using Liberty.SaveManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Liberty.ViewModels
{
    public class StartPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<FATXGameSave> _fatxGameSaves = new ObservableCollection<FATXGameSave>();
        public ObservableCollection<FATXGameSave> FATXGameSaves
        {
            get { return _fatxGameSaves; }
            set { _fatxGameSaves = value; NotifyPropertyChanged("FATXGameSaves"); }
        }

        public class FATXGameSave
        {
            public string SafeGameName { get; set; }
            public string GameName { get; set; }
            public string PackageName { get; set; }
            public string GamerTag { get; set; }
            public string ServiceTag { get; set; }

            public string MapImage { get; set; }
            public string MapName { get; set; }
            public string MapScenario { get; set; }
            public string Difficulty { get; set; }

            public string USID { get; set; }

            public Manager.SupportedGames.SupportedGame SupportedGame { get; set; }
            public CLKsFATXLib.Drive FATXDrive { get; set; }
            public string FATXPath { get; set; }
            public EndianStream Stream { get; set; }
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
