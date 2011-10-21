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
using System.Windows.Shapes;

namespace Liberty.Controls
{
	/// <summary>
	/// Interaction logic for uploadOnLoad.xaml
	/// </summary>
	public partial class uploadOnLoad : Window
	{
        private bool _startUpdate = false;

		public uploadOnLoad(string description)
		{
			this.InitializeComponent();

            lblBuildChanges.Text = description;
		}

        public bool startUpdate
        {
            get { return _startUpdate; }
        }

        private void btnIgnore_Click(object sender, RoutedEventArgs e)
        {
            FormFadeOut.Begin();
            classInfo.applicationExtra.disableInput(this);
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            classInfo.updating.startUpdate();
            _startUpdate = true;

            FormFadeOut.Begin();
            classInfo.applicationExtra.disableInput(this);
        }

        private void FormFadeOut_Completed(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}