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
	/// Interaction logic for step1.xaml
	/// </summary>
	public partial class verifyFile : UserControl, StepUI.IStep
	{
		public verifyFile()
		{
			this.InitializeComponent();
		}

        public void Load(Util.SaveEditor saveEditor)
        {
            lblGamertag.Content = saveEditor.Gamertag;
            lblServiceTag.Content = saveEditor.ServiceTag;
            lblMapName.Text = saveEditor.MissionName + " (" + saveEditor.MapName + ")";
            lblDifficulty.Content = saveEditor.Difficulty.ToString();

            // Try to load the mission image
            try
            {
                var source = new Uri(@"/Liberty;component/Images/mapImages/" + saveEditor.MapName + ".jpg", UriKind.Relative);
                imgMapImage.Source = new BitmapImage(source);
            }
            catch { }
        }

        public bool Save(Util.SaveEditor saveEditor)
        {
            // Load taglists
            App app = (App)Application.Current;
            saveEditor.UnloadTaglists();
            saveEditor.AddTaglist(app.tagList);
            classInfo.nameLookup.loadAscensionTaglist(saveEditor);
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