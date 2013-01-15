using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.SaveManager.Games
{
    public interface ISaveViewer
    {
        bool Close();

        void Load();

        void Save();
        //void SendToDevice();
    }
}
