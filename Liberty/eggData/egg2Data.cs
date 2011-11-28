using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using Liberty.classInfo.storage.settings;
using System.Threading;

namespace Liberty.eggData
{
    class egg2Data
    {
        public static void enableFuckingRainbows()
        {
            if (classInfo.storage.settings.applicationSettings.enableEasterEggs)
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                worker.RunWorkerAsync();
            }
        }

        static void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            ResourceDictionary rd;
            int i = 1;
            while (true)
            {
                switch (i)
                {
                    case 1:
                        rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Orange.xaml", UriKind.Relative) };
                        break;
                    case 2:
                        rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Blue.xaml", UriKind.Relative) };
                        break;
                    case 3:
                        rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Purple.xaml", UriKind.Relative) };
                        break;
                    case 4:
                        rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Pink.xaml", UriKind.Relative) };
                        break;
                    case 5:
                        rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Red.xaml", UriKind.Relative) };
                        break;
                    case 6:
                        rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Green.xaml", UriKind.Relative) };
                        break;
                    case 7:
                        rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Lime.xaml", UriKind.Relative) };
                        break;
                    case 8:
                        rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Silver.xaml", UriKind.Relative) };
                        break;
                    default:
                        rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Orange.xaml", UriKind.Relative) };
                        break;
                }
                App.Current.Resources.MergedDictionaries.Add(rd);

                i++;
                if (i == 9)
                    i = 1;

                Thread.Sleep(500);
            }
        }
    }
}
