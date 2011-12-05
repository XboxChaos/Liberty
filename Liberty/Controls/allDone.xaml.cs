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

namespace Liberty.Controls
{
	/// <summary>
	/// Interaction logic for step5.xaml
	/// </summary>
	public partial class allDone : UserControl, StepUI.IStep
	{
        private selectMode _stepSelectMode;

		public allDone(selectMode stepSelectMode)
		{
            _stepSelectMode = stepSelectMode;
			this.InitializeComponent();
		}

        public void Load()
        {
            if (_stepSelectMode.SelectedBranch != selectMode.EditingMode.EditSaveDevice)
            {
                subBaseHeader_2_1.Content = "Liberty and then just transfer the save back to your hard drive, USB, or memory unit and enjoy some";
                subBaseHeader_2_2.Content = "more Halo!";
            }
            else
            {
                subBaseHeader_2_1.Content = "Liberty and enjoy some more Halo!";
                subBaseHeader_2_2.Content = "";
            }

            switch (classInfo.storage.settings.applicationSettings.gameIdent.gameID)
            {
                case Util.SaveType.Reach:
                    gameWarning.Content = "";
                    break;
                case Util.SaveType.Anniversary:
                    gameWarning.Content = "For Halo Anniversary, when you load your save, press A to overide in the 'failed to load' menu.";
                    break;
                default:
                    gameWarning.Content = "";
                    break;
            }
        }

        public bool Save()
        {
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
	}
}