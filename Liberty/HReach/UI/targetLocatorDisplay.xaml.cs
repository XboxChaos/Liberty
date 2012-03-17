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

namespace Liberty
{
	/// <summary>
	/// Interaction logic for targetLocatorDisplay.xaml
	/// </summary>
    public partial class targetLocatorDisplay : UserControl, Util.IAmmoDisplay
	{
        Reach.CampaignSave _saveData;

		public targetLocatorDisplay(Reach.CampaignSave saveData)
		{
			this.InitializeComponent();

            _saveData = saveData;
            Airstrikes = saveData.Airstrikes;
		}

        private short Airstrikes
        {
            get { return short.Parse(txtAirstrikes.Text); }
            set { txtAirstrikes.Text = value.ToString(); }
        }

        private void btnMaxAirstrikes_Click(object sender, RoutedEventArgs e)
        {
            txtAirstrikes.Text = "32767";
        }

        public void Save()
        {
            _saveData.Airstrikes = Airstrikes;
        }
    }
}