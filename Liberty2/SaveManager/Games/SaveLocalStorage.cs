using Liberty.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.SaveManager.Games
{
    public class SaveLocalStorage
    {
        public string SavePackageName { get; set; }
        public string SaveLocalPath { get; set; }
        public bool IsSaveFromFATX { get; set; }
        public CLKsFATXLib.Drive FATXDrive { get; set; }
        public string FATXPath { get; set; }
        public string USID { get; set; }
    }
}