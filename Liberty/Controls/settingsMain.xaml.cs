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

namespace Liberty.Controls
{
    /// <summary>
    /// Interaction logic for settingsMain.xaml
    /// </summary>
    public partial class settingsMain : UserControl
    {
        private bool isApp = true;
        BrushConverter bc = new BrushConverter();
        FontWeight fw = new FontWeight();

        public settingsMain()
        {
            InitializeComponent();

            themePanel.Visibility = System.Windows.Visibility.Hidden;
            themePanel.Visibility = System.Windows.Visibility.Visible;
        }

        #region uncleanWPFstuff
        
        #region btnSoftwpf
        private void btnSoft_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!isApp)
                btnSoft.Foreground = (Brush)bc.ConvertFrom("#FFD4D2D2");
        }

        private void btnSoft_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!isApp)
                btnSoft.Foreground = (Brush)bc.ConvertFrom("#FF868686");
        }

        private void btnSoft_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!isApp)
               btnSoft.Foreground = (Brush)bc.ConvertFrom("#FFD4D2DF");
        }

        private void btnSoft_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!isApp)
            {
                btnSoft.Foreground = (Brush)bc.ConvertFrom("#FF000000");
                btnSoft.FontWeight = FontWeights.ExtraBold;

                btnTheme.FontWeight = FontWeights.Normal;
                btnTheme.Foreground = (Brush)bc.ConvertFrom("#FF868686");
            }

            isApp = true;
            softwarePanel.Visibility = System.Windows.Visibility.Visible;
            themePanel.Visibility = System.Windows.Visibility.Hidden;
        }
        #endregion

        #region btnSoftwpf
        private void btnTheme_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isApp)
                btnTheme.Foreground = (Brush)bc.ConvertFrom("#FFD4D2D2");
        }

        private void btnTheme_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isApp)
                btnTheme.Foreground = (Brush)bc.ConvertFrom("#FF868686");
        }

        private void btnTheme_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isApp)
                btnTheme.Foreground = (Brush)bc.ConvertFrom("#FFD4D2DF");
        }

        private void btnTheme_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isApp)
            {
                btnTheme.Foreground = (Brush)bc.ConvertFrom("#FF000000");
                btnTheme.FontWeight = FontWeights.ExtraBold;

                btnSoft.FontWeight = FontWeights.Normal;
                btnSoft.Foreground = (Brush)bc.ConvertFrom("#FF868686");
            }

            isApp = false;
            themePanel.Visibility = System.Windows.Visibility.Visible;
            softwarePanel.Visibility = System.Windows.Visibility.Hidden;
        }
        #endregion
        #endregion
    }
}
