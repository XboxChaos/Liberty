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
using Microsoft.Win32;

namespace Liberty
{
	/// <summary>
	/// Interaction logic for resignSave.xaml
	/// </summary>
	public partial class resignSave : UserControl, StepUI.IStep
	{
        private MainWindow _mainWindow = null;
        private Util.SaveEditor _saveEditor;

		public resignSave()
		{
			this.InitializeComponent();

            this.Loaded += new RoutedEventHandler(resignSave_Loaded);
		}

        void resignSave_Loaded(object sender, RoutedEventArgs e)
        {
            _mainWindow = Window.GetWindow(this) as MainWindow;
        }

        public void Load(Util.SaveEditor saveEditor)
        {
            _saveEditor = saveEditor;
        }

        public bool Save(Util.SaveEditor saveEditor)
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

        #region wpf bullshit
        #region btnOpenwpf
        private void btnOpen_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnOpen.Source = new BitmapImage(source);

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Liberty - Open campaign STFS Package";
            ofd.Filter = "Halo Reach Campaign STFS package|*";
            Nullable<bool> result = ofd.ShowDialog();
            if (result.HasValue && result.Value)
            {
                try
                {
                    _saveEditor.LoadSTFS(ofd.FileName, classInfo.extraIO.makeTempSaveDir());
                    lblFileDirec.Content = ofd.FileName;
                }
                catch (Exception ex)
                {
                    // An error occurred trying to load the file, display it to the user
                    _mainWindow.showException(ex.ToString());
                }
            }
        }

        private void btnOpen_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnOpen.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnOpen.Source = new BitmapImage(source);
            }
        }

        private void btnOpen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
            btnOpen.Source = new BitmapImage(source);
        }
        #endregion
        #endregion
    }
}