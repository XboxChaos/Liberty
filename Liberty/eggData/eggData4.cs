using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Liberty.eggData
{
    class eggData4
    {
        public static void enableChecker(MainWindow win)
        {
            if (classInfo.storage.settings.applicationSettings.enableEasterEggs)
            {
                win.showMessage("Seriously? You used Liberty to look at kat's ass giggle at full size? Seriously? You sly dawg.", "Seriously 7.0");
            }
        }
    }
}
