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
	/// Interaction logic for progressStepBar.xaml
	/// </summary>
	public partial class progressStepBar : UserControl
	{
		public progressStepBar()
		{
			this.InitializeComponent();
		}
		
        public void updateStage(int stage)
        {
            BrushConverter bc = new BrushConverter();
            Brush unSelctedHeader;
            Brush selctedHeader;
            unSelctedHeader = (Brush)bc.ConvertFrom("#FFD4D2D2");
            selctedHeader = (Brush)bc.ConvertFrom("#FF868686");
			
            switch(stage)
            {
                case 0:
                    progressBar.Margin = new Thickness(35, 34, 571, 19);
                    lblProgress1.Foreground = selctedHeader;
                    lblProgress2.Foreground = unSelctedHeader;
                    lblProgress3.Foreground = unSelctedHeader;
                    lblProgress4.Foreground = unSelctedHeader;
					lblProgress5.Foreground = unSelctedHeader;
					lblProgress6.Foreground = unSelctedHeader;
                    break;
				case 1:
                    progressBar.Margin = new Thickness(35, 34, 548, 19);
                    lblProgress1.Foreground = selctedHeader;
                    lblProgress2.Foreground = unSelctedHeader;
                    lblProgress3.Foreground = unSelctedHeader;
                    lblProgress4.Foreground = unSelctedHeader;
					lblProgress5.Foreground = unSelctedHeader;
					lblProgress6.Foreground = unSelctedHeader;
                    break;
				case 2:
                    progressBar.Margin = new Thickness(35, 34, 524, 19);
                    lblProgress1.Foreground = selctedHeader;
                    lblProgress2.Foreground = unSelctedHeader;
                    lblProgress3.Foreground = unSelctedHeader;
                    lblProgress4.Foreground = unSelctedHeader;
					lblProgress5.Foreground = unSelctedHeader;
					lblProgress6.Foreground = unSelctedHeader;
                    break;
                case 3:
                    progressBar.Margin = new Thickness(35, 34, 473, 19);
                    lblProgress1.Foreground = unSelctedHeader;
                    lblProgress2.Foreground = selctedHeader;
                    lblProgress3.Foreground = unSelctedHeader;
                    lblProgress4.Foreground = unSelctedHeader;
					lblProgress5.Foreground = unSelctedHeader;
					lblProgress6.Foreground = unSelctedHeader;
                    break;
                case 4:
                    progressBar.Margin = new Thickness(35, 34, 352, 19);
                    lblProgress1.Foreground = unSelctedHeader;
                    lblProgress2.Foreground = unSelctedHeader;
                    lblProgress3.Foreground = selctedHeader;
                    lblProgress4.Foreground = unSelctedHeader;
					lblProgress5.Foreground = unSelctedHeader;
					lblProgress6.Foreground = unSelctedHeader;
                    break;
                case 5:
                    progressBar.Margin = new Thickness(35, 34, 228, 19);
                    lblProgress1.Foreground = unSelctedHeader;
                    lblProgress2.Foreground = unSelctedHeader;
                    lblProgress3.Foreground = unSelctedHeader;
                    lblProgress4.Foreground = selctedHeader;
					lblProgress5.Foreground = unSelctedHeader;
					lblProgress6.Foreground = unSelctedHeader;
                    break;
                case 6:
                    progressBar.Margin = new Thickness(35, 34, 136, 19);
                    lblProgress1.Foreground = unSelctedHeader;
                    lblProgress2.Foreground = unSelctedHeader;
                    lblProgress3.Foreground = unSelctedHeader;
                    lblProgress4.Foreground = unSelctedHeader;
					lblProgress5.Foreground = selctedHeader;
					lblProgress6.Foreground = unSelctedHeader;
                    break;
				case 7:
                    progressBar.Margin = new Thickness(35, 34, 98, 19);
                    lblProgress1.Foreground = unSelctedHeader;
                    lblProgress2.Foreground = unSelctedHeader;
                    lblProgress3.Foreground = unSelctedHeader;
                    lblProgress4.Foreground = unSelctedHeader;
					lblProgress5.Foreground = selctedHeader;
					lblProgress6.Foreground = unSelctedHeader;
                    break;
				case 8:
                    progressBar.Margin = new Thickness(35, 34, 35, 19);
                    lblProgress1.Foreground = unSelctedHeader;
                    lblProgress2.Foreground = unSelctedHeader;
                    lblProgress3.Foreground = unSelctedHeader;
                    lblProgress4.Foreground = unSelctedHeader;
					lblProgress5.Foreground = unSelctedHeader;
					lblProgress6.Foreground = selctedHeader;
                    break;
            }
        }
    }
}