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
using System.Windows.Shapes;
using System.Diagnostics;

namespace Liberty.Controls
{
	/// <summary>
	/// Interaction logic for leavingLiberty.xaml
	/// </summary>
	public partial class leavingLiberty : Window
	{
        private string _url;

		public leavingLiberty(string siteName, string url)
		{
			this.InitializeComponent();
            textBlock2.Text = textBlock2.Text.Replace("{0}", siteName);
            _url = url;
		}
		
		#region wpfBullshit
        #region btnOKwpf
        private void btnOK_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnOK.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnOK.Source = new BitmapImage(source);
            }
        }

        private void btnOK_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnOK.Source = new BitmapImage(source);
        }

        private void btnOK_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnOK.Source = new BitmapImage(source);

            FormFadeOut.Begin();
            Process.Start(_url);
        }
        #endregion

        #region btnUpdatewpf
        private void btnUpdate_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnUpdate.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnUpdate.Source = new BitmapImage(source);
            }
        }

        private void btnUpdate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnUpdate.Source = new BitmapImage(source);
        }

        private void btnUpdate_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnUpdate.Source = new BitmapImage(source);

            FormFadeOut.Begin();
        }
        #endregion

        private void FormFadeOut_Completed(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
	}
}