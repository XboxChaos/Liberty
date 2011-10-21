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
using System.IO;
using System.Reflection;

namespace Liberty.Controls
{
	/// <summary>
	/// Interaction logic for step0.xaml
	/// </summary>
	public partial class selectMode : UserControl, StepUI.IBranchStep<selectMode.EditingMode>
	{
        public enum EditingMode
        {
            EditSaveComputer,
            EditSaveDevice,
            ReSignSave
        }

		public selectMode()
		{
			this.InitializeComponent();
		}

        public EditingMode SelectedBranch
        {
            get { return _selectedBranch; }
        }

        public void Load(Util.SaveEditor saveEditor)
        {
        }

        public bool Save(Util.SaveEditor saveEditor)
        {
            if ((bool)cBSaveOP.IsChecked)
                _selectedBranch = EditingMode.EditSaveComputer;
            else if ((bool)cBSaveOD.IsChecked)
                _selectedBranch = EditingMode.EditSaveDevice;
            else if ((bool)cBSaveResign.IsChecked)
                _selectedBranch = EditingMode.ReSignSave;

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

        private EditingMode _selectedBranch = EditingMode.EditSaveComputer;
    }
}