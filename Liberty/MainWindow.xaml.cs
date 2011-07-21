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
using System.Windows.Controls.Primitives;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using Liberty.classInfo.storage;
using Liberty.classInfo.storage.settings;

namespace Liberty
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		BrushConverter bc = new BrushConverter();
        private int step = -1;

        public MainWindow()
        {
            InitializeComponent();

            goForward();

            step0_1.ExecuteMethod += new EventHandler(ParentWPF_messageBoxFATX);
            step0_1.ExecuteMethodLocal += new EventHandler<Controls.MessageEventArgs>(ParentWPF_fileException);
            step2.ExecuteMethod += new EventHandler(ParentWPF_bipdSwapAlert);
            step4.ExecuteMethod += new EventHandler(ParentWPF_massCordMove);
            step4.ExecuteMethod2 += new EventHandler(ParentWPF_replaceObject);
            step4.ExecuteMethod3 += new EventHandler(ParentWPF_childObjects);
            settingsMain.ExecuteMethod += new EventHandler(ParentWPF_CloseSettings);

            btnCFU.MouseDown += new MouseButtonEventHandler(btnCFU_MouseDown);
            btnCFU.MouseEnter += new MouseEventHandler(btnCFU_MouseEnter);
            btnCFU.MouseLeave += new MouseEventHandler(btnCFU_MouseLeave);
            btnCFU.MouseUp += new MouseButtonEventHandler(btnCFU_MouseUp);

            settingsPanel.Visibility = System.Windows.Visibility.Hidden;
            btnSettings.Visibility = System.Windows.Visibility.Visible;
            lblDevider3.Visibility = System.Windows.Visibility.Visible;
        }

        private void mainWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            App app = (App)Application.Current;

            // loadDialog won't work properly until the window has been loaded
            if (app.initException != null)
                loadDialog(4, app.initException, null);

            if (app.svrBuild > app.pcBuild)
            {
                if (applicationSettings.showChangeLog)
                    loadDialog(3, app.descData, null);
                else
                    loadDialog(2, null, null);
            }
        }

        protected void ParentWPF_bipdSwapAlert(object sender, EventArgs e)
        {
            if (classInfo.storage.fileInfoStorage.leavingStep2) { loadDialog(5, "http://liberty.codeplex.com/discussions/264198", "CodePlex"); }
            else { loadDialog(8, "Swapping your biped may cause the game to freeze or behave unexpectedly. Your old biped will also be deleted. Continue?", "Biped Swap"); }
        }

        protected void ParentWPF_CloseSettings(object sender, EventArgs e)
        {
            settingsPanel.Visibility = System.Windows.Visibility.Hidden;
        }

        protected void ParentWPF_massCordMove(object sender, EventArgs e)
        {
            loadDialog(6, null, null);
        }

        protected void ParentWPF_messageBoxFATX(object sender, EventArgs e)
        {
            loadDialog(1, "Libery could not detect any FATX devices, try re-connecting and pressing 'refresh'. Also check you don't have any other FATX browsers open.", "NO FATX DEVICES");
        }

        protected void ParentWPF_fileException(object sender, Controls.MessageEventArgs e)
        {
            loadDialog(4, e.Message, null);
        }

        protected void ParentWPF_replaceObject(object sender, EventArgs e)
        {
            loadDialog(9, "Select an object to replace \"" + classInfo.storage.fileInfoStorage.replaceObjectName + "\":", "REPLACE OBJECT");
        }

        protected void ParentWPF_childObjects(object sender, EventArgs e)
        {
            loadDialog(9, "Select a child object to edit:", "EDIT CHILD OBJECT");
        }

        private void goForward()
        {
            if (step != 8)
            {
                step0.Visibility = System.Windows.Visibility.Hidden;
                step0_1.Visibility = System.Windows.Visibility.Hidden;
                step0_2.Visibility = System.Windows.Visibility.Hidden;
                step1.Visibility = System.Windows.Visibility.Hidden;
                step2.Visibility = System.Windows.Visibility.Hidden;
                step3.Visibility = System.Windows.Visibility.Hidden;
                step4.Visibility = System.Windows.Visibility.Hidden;
                step4_0.Visibility = System.Windows.Visibility.Hidden;
                step5.Visibility = System.Windows.Visibility.Hidden;

                btnBack.Visibility = System.Windows.Visibility.Visible;
                lblBack.Visibility = System.Windows.Visibility.Visible;
            }

            switch (step)
            {
                case -1:
                    step++;
                    progressBar.updateStage(step);
                    btnBack.Visibility = System.Windows.Visibility.Hidden;
                    lblBack.Visibility = System.Windows.Visibility.Hidden;
                    step0.Visibility = System.Windows.Visibility.Visible;
                    break;
                case 0:
                    step0.saveData();
                    step++;
                    progressBar.updateStage(step);
                    step0_1.loadData();
                    step0_1.Visibility = System.Windows.Visibility.Visible;
                    break;
                case 1:
                    if (fileInfoStorage.saveIsLocal)
                    {
                        step++;
                        goForward();
                    }
                    else
                    {
                        if (step0_1.saveData())
                        {
                            progressBar.updateStage(++step);
                            step0_2.loadData();
                            step0_2.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            step0_1.Visibility = System.Windows.Visibility.Visible;
                        }
                    }
                    break;
                case 2:
                    if (!fileInfoStorage.saveIsLocal)
                        step0_2.saveData();
                    if (fileInfoStorage.fileExtractDirectory == null)
                    {
                        loadDialog(1, "You must select a valid Halo Reach campaign gamesave before you can continue.", "Error");
                        step -= 2;
                        goForward();
                    }
                    else
                    {
                        // Load ascension taglist
                        string excepLoadTag = classInfo.nameLookup.loadAscensionTaglist();
                        if (excepLoadTag != null)
                            loadDialog(4, excepLoadTag, null);

                        step1.loadData();
                        step++;
                        progressBar.updateStage(step);
                        btnBack.IsEnabled = true;
                        step1.Visibility = System.Windows.Visibility.Visible;

                        //Clear TV code from last session
                        {
                            TreeViewItem node1 = new TreeViewItem() { Header = "bipd" };
                            TreeViewItem node2 = new TreeViewItem() { Header = "bloc" };
                            TreeViewItem node3 = new TreeViewItem() { Header = "crea" };
                            TreeViewItem node4 = new TreeViewItem() { Header = "ctrl" };
                            TreeViewItem node5 = new TreeViewItem() { Header = "efsc" };
                            TreeViewItem node6 = new TreeViewItem() { Header = "eqip" };
                            TreeViewItem node7 = new TreeViewItem() { Header = "mach" };
                            TreeViewItem node8 = new TreeViewItem() { Header = "proj" };
                            TreeViewItem node9 = new TreeViewItem() { Header = "scen" };
                            TreeViewItem node10 = new TreeViewItem() { Header = "ssce" };
                            TreeViewItem node11 = new TreeViewItem() { Header = "term" };
                            TreeViewItem node12 = new TreeViewItem() { Header = "vehi" };
                            TreeViewItem node13 = new TreeViewItem() { Header = "weap" };
                            TreeViewItem node14 = new TreeViewItem() { Header = "unknown" };

                            step4.tVObjects.Items.Clear();
                            step4.tVObjects.Items.Add(node1);
                            step4.tVObjects.Items.Add(node2);
                            step4.tVObjects.Items.Add(node3);
                            step4.tVObjects.Items.Add(node4);
                            step4.tVObjects.Items.Add(node5);
                            step4.tVObjects.Items.Add(node6);
                            step4.tVObjects.Items.Add(node7);
                            step4.tVObjects.Items.Add(node8);
                            step4.tVObjects.Items.Add(node9);
                            step4.tVObjects.Items.Add(node10);
                            step4.tVObjects.Items.Add(node11);
                            step4.tVObjects.Items.Add(node12);
                            step4.tVObjects.Items.Add(node13);
                            step4.tVObjects.Items.Add(node14);
                        }
                    }
                    break;
                case 3:
                    step2.loadData();
                    step++;
                    progressBar.updateStage(step);
                    btnBack.IsEnabled = true;
                    step2.Visibility = System.Windows.Visibility.Visible;
                    break;
                case 4:
                    step2.saveData();
                    step3.loadData();
                    step++;
                    progressBar.updateStage(step);
                    btnBack.IsEnabled = true;
                    step3.Visibility = System.Windows.Visibility.Visible;
                    break;
                case 5:
                    bool error = step3.saveData();
                    if (error)
                    {
                        step4.loadData();
                        step++;
                        progressBar.updateStage(step);
                        btnBack.IsEnabled = true;
                        step4.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        loadDialog(1, "Please make sure you have filled in each ammo/grenade count textbox.", "Error");
                        step--;
                        goForward();
                    }
                    break;
                case 6:
                    step4.saveData();
                    step4_0.loadData();
                    step++;
                    progressBar.updateStage(step);
                    step4_0.Visibility = System.Windows.Visibility.Visible;
                    break;
                case 7:
                    step4_0.saveData();
                    string ex = step5.loadData();
                    if (!fileInfoStorage.saveIsLocal)
                    {
                        loadDialog(7, null, null);
                    }
                    if (ex != "yes") { loadDialog(4, ex, null); }
                    step++;
                    progressBar.updateStage(step);
                    btnBack.Visibility = System.Windows.Visibility.Hidden;
                    lblBack.Visibility = System.Windows.Visibility.Hidden;
                    step5.Visibility = System.Windows.Visibility.Visible;
                    break;
                case 8:
                    if (fileInfoStorage.saveIsLocal)
                    {
                        string argument = @"/select, " + fileInfoStorage.fileOriginalDirectory;
                        Process.Start("explorer.exe", argument);
                    }
                    this.FormFadeOut.Begin();
                    break;
            }
        }

        public void loadDialog(int dialog, string message, string title)
        {
            recMask.Visibility = System.Windows.Visibility.Visible;
            switch (dialog)
            {
                case 0:
                    Controls.aboutBox aboutBox = new Controls.aboutBox();
                    aboutBox.Owner = this;
                    aboutBox.ShowDialog();
                    break;
                case 1:
                    Controls.messageBox msgBox = new Controls.messageBox();
                    msgBox.lblSubInfo.Text = message;
                    msgBox.lblTitle.Text = title.ToUpper();
                    msgBox.Owner = this;
                    msgBox.ShowDialog();
                    break;
                case 2:
                    Controls.updater update = new Controls.updater();
                    update.Owner = this;
                    update.ShowDialog();
                    if (classInfo.storage.fileInfoStorage.updStart)
                    {
                        Controls.progressUpdaterDownload upd = new Controls.progressUpdaterDownload();
                        upd.Owner = this;
                        upd.ShowDialog();
                        classInfo.storage.fileInfoStorage.updStart = false;
                        string temp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Liberty\\update\\";
                        Process.Start(temp + "update.exe");
                        classInfo.applicationExtra.closeApplication();
                    }
                    break;
                case 3:
                    Controls.uploadOnLoad updateOL = new Controls.uploadOnLoad();
                    updateOL.Owner = this;
                    updateOL.lblBuildChanges.Text = message;
                    updateOL.ShowDialog();
                    if (classInfo.storage.fileInfoStorage.updStart)
                    {
                        Controls.progressUpdaterDownload upd = new Controls.progressUpdaterDownload();
                        upd.Owner = this;
                        upd.ShowDialog();
                        classInfo.storage.fileInfoStorage.updStart = false;
                        string temp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Liberty\\update\\";
                        Process.Start(temp + "update.exe");
                        classInfo.applicationExtra.closeApplication();
                    }
                    break;
                case 4:
                    Controls.exceptionWindow exp = new Controls.exceptionWindow();
                    exp.lblException.Text = message;
                    exp.Owner = this;
                    exp.ShowDialog();
                    break;
                case 5:
                    Controls.leavingLiberty llib = new Controls.leavingLiberty(title, message);
                    llib.Owner = this;
                    llib.ShowDialog();
                    break;
                case 6:
                    Controls.massObjectMove massCoord = new Controls.massObjectMove();
                    massCoord.Owner = this;
                    massCoord.ShowDialog();
                    if (fileInfoStorage._massCordX == 0 && fileInfoStorage._massCordY == 0 && fileInfoStorage._massCordX == 0) { }
                    else
                    {
                        step4.txtObjectXCord.Text = Convert.ToString(fileInfoStorage._massCordX);
                        step4.txtObjectYCord.Text = Convert.ToString(fileInfoStorage._massCordY);
                        step4.txtObjectZCord.Text = Convert.ToString(fileInfoStorage._massCordZ);
                    }
                    break;
                case 7:
                    Controls.progressWindow progBar = new Controls.progressWindow();
                    progBar.Owner = this;
                    progBar.ShowDialog();
                    break;
                case 8:
                    Controls.messageBoxOptions msgBoxOpt = new Controls.messageBoxOptions();
                    msgBoxOpt.lblSubInfo.Text = message;
                    msgBoxOpt.lblTitle.Text = title.ToUpper();
                    msgBoxOpt.Owner = this;
                    msgBoxOpt.ShowDialog();
                    break;
                case 9:
                    Controls.listboxWindow listbox = new Controls.listboxWindow();
                    listbox.lblTitle.Text = title.ToUpper();
                    listbox.lblSubInfo.Text = message;
                    listbox.Owner = this;
                    listbox.ShowDialog();
                    break;
            }
            recMask.Visibility = System.Windows.Visibility.Hidden;
        }

        #region uncleanBullshitforWFP
        private void FormFadeOut_Completed(object sender, EventArgs e)
        {
            classInfo.applicationExtra.closeApplication();
        }

        private void headerThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Left = Left + e.HorizontalChange;
            Top = Top + e.VerticalChange;
        }

        #region btnClosewpf
        private void btnClose_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/h-close.png", UriKind.Relative);
                btnClose.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/l-close.png", UriKind.Relative);
                btnClose.Source = new BitmapImage(source);
            }
        }

        private void btnClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/c-close.png", UriKind.Relative);
            btnClose.Source = new BitmapImage(source);
        }
		
		private void btnClose_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/l-close.png", UriKind.Relative);
            btnClose.Source = new BitmapImage(source);
            this.FormFadeOut.Begin();
        }
        #endregion

        #region btnMinwpf
        private void btnMinimize_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/l-minimize.png", UriKind.Relative);
            btnMinimize.Source = new BitmapImage(source);
			
			this.WindowState = System.Windows.WindowState.Minimized;
        }
		
		private void btnMinimize_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/h-minimize.png", UriKind.Relative);
                btnMinimize.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/l-minimize.png", UriKind.Relative);
                btnMinimize.Source = new BitmapImage(source);
            }
        }

        private void btnMinimize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/c-minimize.png", UriKind.Relative);
            btnMinimize.Source = new BitmapImage(source);
        }
        #endregion

        #region btnAboutwpf
        private void btnAbout_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
        	btnAbout.Foreground = (Brush)bc.ConvertFrom("#FFD4D2D2");
        }

        private void btnAbout_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
        	btnAbout.Foreground = (Brush)bc.ConvertFrom("#FF868686");
        }

        private void btnAbout_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	btnAbout.Foreground = (Brush)bc.ConvertFrom("#FFD4D2DF");
        }

        private void btnAbout_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	btnAbout.Foreground = (Brush)bc.ConvertFrom("#FF868686");

            loadDialog(0, null, null);
        }
        #endregion

        #region btnCFUwpf
        private void btnCFU_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnCFU.Foreground = (Brush)bc.ConvertFrom("#FFD4D2D2");
        }

        private void btnCFU_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnCFU.Foreground = (Brush)bc.ConvertFrom("#FF868686");
        }

        private void btnCFU_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            btnCFU.Foreground = (Brush)bc.ConvertFrom("#FFD4D2DF");
        }

        private void btnCFU_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            btnCFU.Foreground = (Brush)bc.ConvertFrom("#FF868686");

            loadDialog(2, null, null);
        }
        #endregion

        #region btnBugReportwpf
        private void btnBugReport_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnBugReport.Foreground = (Brush)bc.ConvertFrom("#FFD4D2D2");
        }

        private void btnBugReport_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnBugReport.Foreground = (Brush)bc.ConvertFrom("#FF868686");
        }

        private void btnBugReport_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            btnBugReport.Foreground = (Brush)bc.ConvertFrom("#FFD4D2DF");
        }

        private void btnBugReport_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            btnBugReport.Foreground = (Brush)bc.ConvertFrom("#FF868686");

            loadDialog(5, "http://liberty.codeplex.com/workitem/list/basic", "CodePlex");
        }
        #endregion

        #region btnSettingswpf
        private void btnSettings_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnSettings.Foreground = (Brush)bc.ConvertFrom("#FFD4D2D2");
        }

        private void btnSettings_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnSettings.Foreground = (Brush)bc.ConvertFrom("#FF868686");
        }

        private void btnSettings_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            btnSettings.Foreground = (Brush)bc.ConvertFrom("#FFD4D2DF");
        }

        private void btnSettings_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            btnSettings.Foreground = (Brush)bc.ConvertFrom("#FF868686");

            settingsPanel.Visibility = System.Windows.Visibility.Visible;
            settingsMain.load();
        }
        #endregion

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
            var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
            btnOK.Source = new BitmapImage(source);

            goForward();
        }
        #endregion

        #region btnBackwpf
        private void btnBack_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnBack.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnBack.Source = new BitmapImage(source);
            }
        }

        private void btnBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnBack.Source = new BitmapImage(source);
        }

        private void btnBack_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
            btnBack.Source = new BitmapImage(source);

            if (fileInfoStorage.saveIsLocal && step == 3)
            {
                step = 0;
            }
            else
            {
                step--;
                step--;
            }
            goForward();
        }
        #endregion
		
		private void Rectangle_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (eggData.egg1Data.checkEggExists())
            {
                egg1.Source = new Uri(eggData.egg1Data.eggDirectory(), UriKind.RelativeOrAbsolute);
                egg1.Play();
            }
        }
        #endregion
    }
}
