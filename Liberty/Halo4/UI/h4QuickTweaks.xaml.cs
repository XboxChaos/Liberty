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

namespace Liberty.Halo4.UI
{
    /// <summary>
    /// Interaction logic for h4QuickTweaks.xaml
    /// </summary>
    public partial class h4QuickTweaks : UserControl, StepUI.IStep
    {
        private Util.SaveManager<Halo4.CampaignSave> _saveManager;

        public h4QuickTweaks(Util.SaveManager<Halo4.CampaignSave> saveManager)
        {
            _saveManager = saveManager;
            this.InitializeComponent();
        }

        public void Load() { }

        public bool Save()
        {
            if ((bool)checkAllMaxAmmo.IsChecked)
                Util.EditorSupport.AllWeaponsMaxAmmo(_saveManager.SaveData);
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