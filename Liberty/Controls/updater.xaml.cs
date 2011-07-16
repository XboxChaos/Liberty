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
using System.Reflection;
using System.IO;
using System.Net;

namespace Liberty.Controls
{
	/// <summary>
	/// Interaction logic for updater.xaml
	/// </summary>
	public partial class updater : Window
	{
		public updater()
		{
			this.InitializeComponent();
            try
            {
                lblBuildVer.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                lblBuildHash.Text = classInfo.extraIO.getMD5Hash(System.Environment.GetCommandLineArgs()[0]);
                FileInfo fi = new FileInfo(System.Environment.GetCommandLineArgs()[0]);
                DateTime dateAssembled = fi.CreationTime;
                lblBuildDate.Text = String.Format("{0:dd/MM/yyyy}", dateAssembled);
                WebClient wb = new WebClient();
                string downloadedInfo = wb.DownloadString(new Uri("http://xeraxic.com/downloads/checkVersionInfo.php?appName=Liberty&proVer=1"));

                downloadedInfo = downloadedInfo.Replace("\r", "");
                string[] updateData = downloadedInfo.Split('\n');

                lblSvrBuildHash.Text = updateData[1];
                lblSvrBuildVer.Text = updateData[0];

                int svrBuild = Convert.ToInt16(updateData[0].Replace(".", ""));
                int pcBuild = Convert.ToInt16(Assembly.GetExecutingAssembly().GetName().Version.ToString().Replace(".", ""));

                if (svrBuild > pcBuild)
                {
                    lblSvrBuildNewer.Text = "yes";

                    lblUpdate.IsMouseDirectlyOverChanged += new DependencyPropertyChangedEventHandler(btnUpdate_IsMouseDirectlyOverChanged);
                    lblUpdate.MouseDown += new MouseButtonEventHandler(btnUpdate_MouseDown);
                    lblUpdate.MouseUp += new MouseButtonEventHandler(btnUpdate_MouseUp);

                    btnUpdate.IsMouseDirectlyOverChanged += new DependencyPropertyChangedEventHandler(btnUpdate_IsMouseDirectlyOverChanged);
                    btnUpdate.MouseDown += new MouseButtonEventHandler(btnUpdate_MouseDown);
                    btnUpdate.MouseUp += new MouseButtonEventHandler(btnUpdate_MouseUp);

                    BrushConverter bc = new BrushConverter();
                    lblUpdate.Foreground = (Brush)bc.ConvertFrom("#FF373A3D");
                }
                else
                { lblSvrBuildNewer.Text = "no"; }
                classInfo.storage.fileInfoStorage.connectedToUpdate = true;
            }
            catch
            {
                classInfo.storage.fileInfoStorage.connectedToUpdate = false;
                this.Close();
            }
		}

        private void update_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (classInfo.storage.fileInfoStorage.connectedToUpdate) { }
            else { this.FormFadeOut.Begin(); }
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

            classInfo.updating.startUpdate();
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