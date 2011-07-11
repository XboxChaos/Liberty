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
	public partial class step1 : UserControl
	{
		public step1()
		{
			this.InitializeComponent();
		}

        public void loadData()
        {
            string[] arrayData = classInfo.loadPackageData.getPackageData();

            lblGamertag.Content = arrayData[0];
            lblServiceTag.Content = arrayData[1];
            lblMapName.Content = arrayData[3] + " - " + arrayData[2];
            lblDifficulty.Content = arrayData[4];

            string mapImage = classInfo.loadPackageData.getMapName(arrayData[2]);

            try
            {
                var source = new Uri(@"/Liberty;component/Images/mapImages/" + mapImage + ".jpg", UriKind.Relative);
                imgMapImage.Source = new BitmapImage(source);
            }
            catch { }

        }
	}
}