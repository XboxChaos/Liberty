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
using Liberty.Controls;
using Liberty.StepUI;

namespace Liberty
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		BrushConverter bc = new BrushConverter();
        private Util.SaveManager<Reach.CampaignSave> _reachSaveManager =
            new Util.SaveManager<Reach.CampaignSave>(path => new Reach.CampaignSave(path));
        private Reach.TagListManager _reachTaglists = null;
        private StepViewer _stepViewer = null;
        private StepUI.IStepNode _firstStep;
        selectMode _stepSelectMode = null;

        public MainWindow()
        {
            InitializeComponent();

            settingsMain.ExecuteMethod += new EventHandler(ParentWPF_CloseSettings);
            settingsPanel.Visibility = Visibility.Hidden;

            _reachTaglists = new Reach.TagListManager(_reachSaveManager);

            // Create steps
            _stepSelectMode = new selectMode();
            openSaveFile stepOpenFile = new openSaveFile(_reachSaveManager, _reachTaglists);
            selectDevice stepSelectDevice = new selectDevice();
            selectSaveOnDevice stepSelectSave = new selectSaveOnDevice(stepSelectDevice, _reachSaveManager, _reachTaglists);
            verifyFile stepVerifyFile = new verifyFile(_reachSaveManager, _reachTaglists);
            editBiped stepBiped = new editBiped(_reachSaveManager, _reachTaglists);
            editWeapons stepWeapons = new editWeapons(_reachSaveManager);
            editGrenades stepGrenades = new editGrenades(_reachSaveManager);
            editObjects stepObjects = new editObjects(_reachSaveManager, _reachTaglists);
            quickTweaks stepTweaks = new quickTweaks(_reachSaveManager);
            allDone stepAllDone = new allDone(_stepSelectMode);

            // Add them
            addStep(_stepSelectMode);
            addStep(stepOpenFile);
            addStep(stepSelectDevice);
            addStep(stepSelectSave);
            addStep(stepVerifyFile);
            addStep(stepBiped);
            addStep(stepWeapons);
            addStep(stepGrenades);
            addStep(stepObjects);
            addStep(stepTweaks);
            addStep(stepAllDone);

            // Set up the step viewer
            _stepViewer = new StepViewer(stepGrid);

            // Start building the step graph
            StepGraphBuilder stepGraph = new StepGraphBuilder(progressBar);
            stepGraph.AddBranchStep(_stepSelectMode, "PREPARATION");

            // Step graph: Edit save on computer
            StepGraphBuilder editSaveOnComputer = stepGraph.StartBranch(selectMode.EditingMode.EditSaveComputer, true);
            editSaveOnComputer.AddStep(stepOpenFile, "SAVE SELECTION");
            editSaveOnComputer.AddStep(stepVerifyFile, "SAVE SELECTION");
            editSaveOnComputer.AddStep(stepBiped, "CHARACTER DATA");
            editSaveOnComputer.AddStep(stepWeapons, "WEAPON DATA");
            editSaveOnComputer.AddStep(stepGrenades, "WEAPON DATA");
            editSaveOnComputer.AddStep(stepObjects, "OBJECT DATA");
            editSaveOnComputer.AddStep(stepTweaks, "OBJECT DATA");
            editSaveOnComputer.AddStep(stepAllDone, "FINISHED");

            // Step graph: Edit save on removable device
            StepGraphBuilder editSaveOnDevice = stepGraph.StartBranch(selectMode.EditingMode.EditSaveDevice, true);
            editSaveOnDevice.AddStep(stepSelectDevice, "SAVE SELECTION");
            editSaveOnDevice.AddStep(stepSelectSave, "SAVE SELECTION");
            editSaveOnDevice.AddStep(stepVerifyFile, "SAVE SELECTION");
            editSaveOnDevice.AddStep(stepBiped, "CHARACTER DATA");
            editSaveOnDevice.AddStep(stepWeapons, "WEAPON DATA");
            editSaveOnDevice.AddStep(stepGrenades, "WEAPON DATA");
            editSaveOnDevice.AddStep(stepObjects, "OBJECT DATA");
            editSaveOnDevice.AddStep(stepTweaks, "OBJECT DATA");
            TransferSaveStep stepTransfer = new TransferSaveStep(this, stepSelectDevice, stepSelectSave);
            editSaveOnDevice.AddWorkStep(stepTransfer, _stepViewer);
            editSaveOnDevice.AddStep(stepAllDone, "FINISHED");

            // Add dummy groups to the mode selection step so that they show in the progress bar
            // This needs to be improved upon...
            stepGraph.AddGroup("SAVE SELECTION");
            stepGraph.AddGroup("CHARACTER DATA");
            stepGraph.AddGroup("WEAPON DATA");
            stepGraph.AddGroup("OBJECT DATA");
            stepGraph.AddGroup("FINISHED");

            _firstStep = stepGraph.BuildGraph();
            _stepViewer.ViewNode(_firstStep);
            btnBack.Visibility = _stepViewer.CanGoBack ? Visibility.Visible : Visibility.Hidden;

#if DEBUG
            btnBetaPlayground.Visibility = System.Windows.Visibility.Visible;
#else
            btnBetaPlayground.Visibility = System.Windows.Visibility.Hidden;
#endif
        }

        private void addStep(UIElement step)
        {
            step.Visibility = Visibility.Collapsed;
            stepGrid.Children.Add(step);
        }

        public void showMessage(string message, string title)
        {
            messageBox msgBox = new messageBox(message, title);
            msgBox.Owner = this;
            msgBox.ShowDialog();
        }

        public void showException(string message)
        {
            exceptionWindow exp = new exceptionWindow(message);
            exp.Owner = this;
            exp.ShowDialog();
        }

        public void showLeavingDialog(string url, string siteName)
        {
            leavingLiberty llib = new leavingLiberty(siteName, url);
            llib.Owner = this;
            llib.ShowDialog();
        }

        public bool showQuestion(string message, string title)
        {
            messageBoxOptions msgBoxOpt = new messageBoxOptions(message, title);
            msgBoxOpt.Owner = this;
            msgBoxOpt.ShowDialog();

            return msgBoxOpt.result;
        }

        public ListBoxItem showListBox(string message, string title, List<ListBoxItem> items)
        {
            listboxWindow listbox = new listboxWindow(items);
            listbox.lblTitle.Text = title.ToUpper();
            listbox.lblSubInfo.Text = message;
            listbox.Owner = this;
            listbox.ShowDialog();

            return listbox.selectedItem;
        }

        public bool showWarning(string message, string title)
        {
            if (!applicationSettings.noWarnings)
                return showQuestion(message, title);
            else
                return true;
        }

        private void about()
        {
            recMask.Visibility = Visibility.Visible;

            aboutBox aboutBox = new aboutBox();
            aboutBox.Owner = this;
            aboutBox.ShowDialog();

            recMask.Visibility = Visibility.Hidden;
        }

        private void startUpdater()
        {
            progressUpdaterDownload upd = new progressUpdaterDownload();
            upd.Owner = this;
            upd.ShowDialog();
            string temp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Liberty\\update\\";
            Process.Start(temp + "update.exe");
            classInfo.applicationExtra.closeApplication();
        }

        private void checkForUpdates()
        {
            updater update = new updater();
            update.Owner = this;
            update.ShowDialog();

            if (update.startUpdate)
                startUpdater();
        }

        private void showUpdateDescription(string description)
        {
            uploadOnLoad updateOL = new uploadOnLoad(description);
            updateOL.Owner = this;
            updateOL.ShowDialog();

            if (updateOL.startUpdate)
                startUpdater();
        }

        private void mainWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            App app = (App)Application.Current;

            if (app.initException != null)
                showException(app.initException);

            if (app.svrBuild > app.pcBuild)
            {
                if (applicationSettings.showChangeLog)
                    showUpdateDescription(app.descData);
                else
                    checkForUpdates();
            }

            if (app.tagList != null)
                _reachTaglists.AddGenericTaglist(app.tagList);
        }

        protected void ParentWPF_CloseSettings(object sender, EventArgs e)
        {
            settingsPanel.Visibility = Visibility.Hidden;
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
            classInfo.applicationExtra.disableInput(this);
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
            btnAbout.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextMid);
        }

        private void btnAbout_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnAbout.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextDark);
        }

        private void btnAbout_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) { }

        private void btnAbout_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            btnAbout.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextDark);

            about();
        }
        #endregion

        #region btnCFUwpf
        private void btnCFU_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnCFU.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextMid);
        }

        private void btnCFU_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnCFU.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextDark);
        }

        private void btnCFU_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) { }

        private void btnCFU_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            btnCFU.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextDark);

            checkForUpdates();
        }
        #endregion

        #region btnBugReportwpf
        private void btnBugReport_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnBugReport.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextMid);
        }

        private void btnBugReport_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnBugReport.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextDark);
        }

        private void btnBugReport_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) { }

        private void btnBugReport_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            btnBugReport.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextDark);

            showLeavingDialog("http://liberty.codeplex.com/workitem/list/basic", "CodePlex");
        }
        #endregion

        #region btnSettingswpf
        private void btnSettings_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnSettings.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextMid);
        }

        private void btnSettings_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnSettings.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextDark);
        }

        private void btnSettings_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) { }

        private void btnSettings_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            btnSettings.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextDark);

            settingsPanel.Visibility = Visibility.Visible;
        }
        #endregion

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!_stepViewer.Forward())
            {
                FormFadeOut.Begin();
                classInfo.applicationExtra.disableInput(this);
                return;
            }

            btnBack.Visibility = _stepViewer.CanGoBack ? Visibility.Visible : Visibility.Hidden;
            if (!_stepViewer.CanGoForward)
            {
                btnOK.Content = "Close";
                btnBack.Content = "Restart";
                if (_stepSelectMode.SelectedBranch == selectMode.EditingMode.EditSaveComputer)
                {
                    string argument = @"/select, " + _reachSaveManager.STFSPath;
                    Process.Start("explorer.exe", argument);
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (!_stepViewer.CanGoForward)
            {
                // Restart
                _reachSaveManager.Close();
                _reachTaglists.RemoveMapSpecificTaglists();
                _stepViewer.ViewNode(_firstStep);
                btnOK.Content = "Next";
                btnBack.Content = "Back";
            }
            else
            {
                _stepViewer.Back();
            }
            btnBack.Visibility = _stepViewer.CanGoBack ? Visibility.Visible : Visibility.Hidden;
        }
		
		private void Rectangle_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (eggData.egg1Data.checkEggExists())
            {
                egg1.Source = new Uri(eggData.egg1Data.eggDirectory(), UriKind.RelativeOrAbsolute);
                egg1.Play();
            }
        }
        #endregion

        private void btnBetaPlayground_Click(object sender, RoutedEventArgs e)
        {
            if (stepBeta.Visibility != Visibility.Visible)
            {
                stepBeta.Visibility = Visibility.Visible;
                stepGrid.Visibility = Visibility.Hidden;
            }
            else
            {
                stepBeta.Visibility = Visibility.Hidden;
                stepGrid.Visibility = Visibility.Visible;
            }
        }
    }
}
