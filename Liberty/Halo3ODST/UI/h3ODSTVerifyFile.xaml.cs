using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Liberty.Halo3ODST.UI
{
    /// <summary>
    /// Interaction logic for h3ODSTVerifyFile.xaml
    /// </summary>
    public partial class h3ODSTVerifyFile : UserControl, StepUI.IStep
    {
        private Util.SaveManager<Halo3ODST.CampaignSave> _saveManager;

        public h3ODSTVerifyFile(Util.SaveManager<Halo3ODST.CampaignSave> saveManager)
        {
            InitializeComponent();
            _saveManager = saveManager;
        }

        public string Gamertag { get; set; }

        public void Load()
        {
            Halo3ODST.CampaignSave saveData = _saveManager.SaveData;
            lblGamertag.Content = saveData.Player.Gamertag + " (" + saveData.Player.ServiceTag + ")";
            lblMapName.Text = Util.EditorSupport.GetMissionName(saveData) + " - " + saveData.Header.Map;
            lblDifficulty.Content = saveData.Header.DifficultyString;

            // Try to load the mission image/difficulty
            try
            {
                string mapName = saveData.Header.Map;
                mapName = mapName.Substring(mapName.LastIndexOf('\\') + 1);
                var source = new Uri(@"/Liberty;component/Images/h3ODSTMaps/" + mapName.ToLower() + ".jpg", UriKind.Relative);
                imgMapImage.Source = new BitmapImage(source);

                int diff = (int)saveData.Header.Difficulty + 1;
                source = new Uri(@"/Liberty;component/Images/Difficulty/Blam_ODST/" + diff.ToString() + ".png", UriKind.Relative);
                imgDifficulty.Source = new BitmapImage(source);
            }
            catch { }
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
