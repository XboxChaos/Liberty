﻿using System;
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
        private Util.SaveManager<Reach.CampaignSave> _saveManager;
        private Reach.TagListManager _taglistManager;

        public verifyFile(Util.SaveManager<Reach.CampaignSave> saveManager, Reach.TagListManager taglistManager)
		{
            _saveManager = saveManager;
            _taglistManager = taglistManager;
			this.InitializeComponent();
		}

        public void Load()
        {
            Reach.CampaignSave saveData = _saveManager.SaveData;
            lblGamertag.Content = saveData.Gamertag;
            lblServiceTag.Content = saveData.ServiceTag;
            lblMapName.Text = Util.EditorSupport.GetMissionName(saveData) + " (" + saveData.Map + ")";
            lblDifficulty.Content = saveData.Difficulty.ToString();

            // Try to load the mission image
            try
            {
                string mapName = saveData.Map;
                mapName = mapName.Substring(mapName.LastIndexOf('\\') + 1);
                var source = new Uri(@"/Liberty;component/Images/mapImages/" + mapName + ".jpg", UriKind.Relative);
                imgMapImage.Source = new BitmapImage(source);
            }
            catch { }
        }

        public bool Save()
        {
            classInfo.nameLookup.loadAscensionTaglist(_saveManager, _taglistManager);
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