using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Liberty.eggData
{
    class egg3Data
    {
        public static void loadingCODHowCouldYou(Window win)
        {
            if (classInfo.storage.settings.applicationSettings.enableEasterEggs)
            {
                InitCODVerification initCod = new InitCODVerification();
                initCod.Owner = win;
                initCod.ShowDialog();
            }
        }
    }
}
