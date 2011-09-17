using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MetroLiberty.ClassInfo
{
    class UIX
    {
        public static void stepUI(int step)
        {
            switch (step)
            {
                case 0:
                    Window.Current.Content = new StepUI.Step0();
                    Window.Current.Activate();
                    break;
            }
        }
    }
}
