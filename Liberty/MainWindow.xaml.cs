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
using Liberty.HCEX.UI;

using Liberty.StepUI;
using X360.STFS;
using System.Windows.Threading;

namespace Liberty
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		BrushConverter bc = new BrushConverter();
        private Util.ISaveManager _saveManager = null;
        private Util.SaveManager<Reach.CampaignSave> _reachSaveManager;
        private Util.SaveManager<HCEX.CampaignSave> _hcexSaveManager;
        private Reach.TagListManager _reachTaglists = null;
        private StepViewer _stepViewer = null;
        private StepUI.IStepNode _firstStep;
        private Util.FATXSaveTransferrer _saveTransferrer = new Util.FATXSaveTransferrer();
        private selectMode _stepSelectMode;
        private selectDevice _stepSelectDevice;
        private openSaveFile _stepOpenFile;
        private transferSave _stepTransfer;
        private string _packagePath = null;
        private Util.SaveType _currentGame;
        private DispatcherTimer egg2 = new DispatcherTimer();
        private cexVerifyFile _cexVerifyFile;

        public MainWindow()
        {
            InitializeComponent();

            // Setup egg2 Timers
            egg2.Interval = new TimeSpan(0, 0, 20);
            egg2.Tick += new EventHandler(egg2_Tick);

            settingsMain.ExecuteMethod += new EventHandler(ParentWPF_CloseSettings);
            settingsPanel.Visibility = Visibility.Hidden;

            // Set up reach stuff
            _reachSaveManager = new Util.SaveManager<Reach.CampaignSave>(path => new Reach.CampaignSave(path));
            _reachTaglists = new Reach.TagListManager(_reachSaveManager);

            // Set up HCEX stuff
            _hcexSaveManager = new Util.SaveManager<HCEX.CampaignSave>(path => new HCEX.CampaignSave(path));

            // Set up the step viewer
            _stepViewer = new StepViewer(stepGrid);
            _stepViewer.BeforeViewNode += new EventHandler<ViewNodeEventArgs>(stepViewer_BeforeViewNode);

            // Create steps
            _stepSelectMode = new selectMode();
            _stepOpenFile = new openSaveFile(loadSaveFile);
            _stepSelectDevice = new selectDevice();
            selectSaveOnDevice stepSelectSave = new selectSaveOnDevice(_stepSelectDevice, _saveTransferrer, loadSaveFile);

            saving stepSaving = new saving(updateSaveFile);
            _stepTransfer = new transferSave(_saveTransferrer);
            allDone stepAllDone = new allDone(_stepSelectMode);

            #region Reach
            verifyFile reachVerifyFile = new verifyFile(_reachSaveManager, _reachTaglists);
            editBiped reachBiped = new editBiped(_reachSaveManager, _reachTaglists);
            editWeapons reachWeapons = new editWeapons(_reachSaveManager);
            editGrenades reachGrenades = new editGrenades(_reachSaveManager);
            editObjects reachObjects = new editObjects(_reachSaveManager, _reachTaglists);
            quickTweaks reachTweaks = new quickTweaks(_reachSaveManager);
            #endregion

            #region HCEX
            _cexVerifyFile = new HCEX.UI.cexVerifyFile(_hcexSaveManager);
            cexEditBiped cexEditBiped = new HCEX.UI.cexEditBiped(_hcexSaveManager);
            cexEditWeapons cexEditWeapons = new HCEX.UI.cexEditWeapons(_hcexSaveManager);
            cexEditGrenades cexEditGrenades = new HCEX.UI.cexEditGrenades(_hcexSaveManager);
            cexQuickTweaks cexQuickTweaks = new HCEX.UI.cexQuickTweaks(_hcexSaveManager);
            #endregion

            // FIXME: hax, the StepGraphBuilder can't set up a WorkStepProgressUpdater or else StepViewer.Forward() will get called twice due to two events being attached
            // Maybe I should just throw away that feature where the progress bar can update mid-step so a group reference isn't needed
            IStep workStepSaving = new WorkStepProgressUpdater(new UnnavigableWorkStep(stepSaving, gridButtons, headerControls), null, _stepViewer);
            IStep workStepTransfer = new WorkStepProgressUpdater(new UnnavigableWorkStep(_stepTransfer, gridButtons, headerControls), null, _stepViewer);

            // Add them to the step grid
            addStep(_stepSelectMode);
            addStep(_stepOpenFile);
            addStep(_stepSelectDevice);
            addStep(stepSelectSave);

            #region ReachSteps
            addStep(reachVerifyFile);
            addStep(reachBiped);
            addStep(reachWeapons);
            addStep(reachGrenades);
            addStep(reachObjects);
            addStep(reachTweaks);
            #endregion

            #region CEXSteps
            addStep(_cexVerifyFile);
            addStep(cexEditBiped);
            addStep(cexEditWeapons);
            addStep(cexEditGrenades);
            addStep(cexQuickTweaks);
            #endregion

            addStep(stepSaving);
            addStep(_stepTransfer);
            addStep(stepAllDone);

            // Start building the step graph
            StepGraphBuilder stepGraph = new StepGraphBuilder(progressBar);
            stepGraph.AddBranchStep(_stepSelectMode, "PREPARATION");

            // Step graph: Edit save on computer
            StepGraphBuilder editSaveOnComputer = stepGraph.StartBranch(selectMode.EditingMode.EditSaveComputer, true);
            editSaveOnComputer.AddBranchStep(_stepOpenFile, "SAVE SELECTION");

            // Step graph: Edit save on removable device
            StepGraphBuilder editSaveOnDevice = stepGraph.StartBranch(selectMode.EditingMode.EditSaveDevice, true);
            editSaveOnDevice.AddStep(_stepSelectDevice, "SAVE SELECTION");
            editSaveOnDevice.AddBranchStep(stepSelectSave, "SAVE SELECTION");

            #region ReachSteps
            // Step graph: Edit Halo: Reach save on computer
            StepGraphBuilder reachComputerSave = editSaveOnComputer.StartBranch(Util.SaveType.Reach, true);
            reachComputerSave.AddStep(reachVerifyFile, "SAVE SELECTION");
            reachComputerSave.AddStep(reachBiped, "CHARACTER DATA");
            reachComputerSave.AddStep(reachWeapons, "WEAPON DATA");
            reachComputerSave.AddStep(reachGrenades, "WEAPON DATA");
            reachComputerSave.AddStep(reachObjects, "OBJECT DATA");
            reachComputerSave.AddStep(reachTweaks, "OBJECT DATA");
            reachComputerSave.AddStep(workStepSaving);
            reachComputerSave.AddStep(stepAllDone, "FINISHED");

            // Step graph: Edit Halo: Reach save on removable device
            StepGraphBuilder reachDeviceSave = editSaveOnDevice.StartBranch(Util.SaveType.Reach, true);
            reachDeviceSave.AddStep(reachVerifyFile, "SAVE SELECTION");
            reachDeviceSave.AddStep(reachBiped, "CHARACTER DATA");
            reachDeviceSave.AddStep(reachWeapons, "WEAPON DATA");
            reachDeviceSave.AddStep(reachGrenades, "WEAPON DATA");
            reachDeviceSave.AddStep(reachObjects, "OBJECT DATA");
            reachDeviceSave.AddStep(reachTweaks, "OBJECT DATA");
            reachDeviceSave.AddStep(workStepSaving);
            reachDeviceSave.AddStep(workStepTransfer);
            reachDeviceSave.AddStep(stepAllDone, "FINISHED");
            #endregion

            #region CEXSteps
            // Step graph: Edit HCEX save on computer
            StepGraphBuilder hcexComputerSave = editSaveOnComputer.StartBranch(Util.SaveType.Anniversary, true);
            hcexComputerSave.AddStep(_cexVerifyFile, "SAVE SELECTION");
            hcexComputerSave.AddStep(cexEditBiped, "CHARACTER DATA");
            hcexComputerSave.AddStep(cexEditWeapons, "WEAPON DATA");
            hcexComputerSave.AddStep(cexEditGrenades, "WEAPON DATA");
            hcexComputerSave.AddStep(cexQuickTweaks, "OBJECT DATA");
            hcexComputerSave.AddStep(workStepSaving);
            hcexComputerSave.AddStep(stepAllDone, "FINISHED");

            // Step graph: Edit HCEX save on removable device
            StepGraphBuilder hcexDeviceSave = editSaveOnDevice.StartBranch(Util.SaveType.Anniversary, true);
            hcexDeviceSave.AddStep(_cexVerifyFile, "SAVE SELECTION");
            hcexDeviceSave.AddStep(cexEditBiped, "CHARACTER DATA");
            hcexDeviceSave.AddStep(cexEditWeapons, "WEAPON DATA");
            hcexDeviceSave.AddStep(cexEditGrenades, "WEAPON DATA");
            hcexDeviceSave.AddStep(cexQuickTweaks, "OBJECT DATA");
            hcexDeviceSave.AddStep(workStepSaving);
            hcexDeviceSave.AddStep(stepAllDone, "FINISHED");
            #endregion

            // Add dummy groups so that they show in the progress bar
            // This needs to be improved upon...
            stepGraph.AddGroup("SAVE SELECTION");
            stepGraph.AddGroup("CHARACTER DATA");
            stepGraph.AddGroup("WEAPON DATA");
            stepGraph.AddGroup("OBJECT DATA");
            stepGraph.AddGroup("FINISHED");
            editSaveOnComputer.AddGroup("CHARACTER DATA");
            editSaveOnComputer.AddGroup("WEAPON DATA");
            editSaveOnComputer.AddGroup("OBJECT DATA");
            editSaveOnComputer.AddGroup("FINISHED");
            editSaveOnDevice.AddGroup("CHARACTER DATA");
            editSaveOnDevice.AddGroup("WEAPON DATA");
            editSaveOnDevice.AddGroup("OBJECT DATA");
            editSaveOnDevice.AddGroup("FINISHED");

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

        private Util.SaveType loadSaveFile(string stfsPath)
        {
            _reachTaglists.RemoveMapSpecificTaglists();
            if (_saveManager != null)
                _saveManager.Close();

            STFSPackage package = null;
            try
            {
                package = new STFSPackage(stfsPath, null);
                string rawFileName;
                _currentGame = DetectGame(package, out rawFileName);
                classInfo.storage.settings.applicationSettings.gameIdent.gameID = _currentGame;
                if (_currentGame == Util.SaveType.Unknown)
                {
                    showMessage(package.Header.Title_Display + " saves are not supported yet. Currently, only Halo: Reach and Halo: CE Anniversary saves are supported. Please select a different file.", "GAME NOT SUPPORTED");
                    return Util.SaveType.Unknown;
                }
                else if (_currentGame == Util.SaveType.SomeGame)
                {
                    eggData.egg3Dialog dialog = new eggData.egg3Dialog();
                    dialog.Owner = this;
                    dialog.ShowDialog();
                    return Util.SaveType.Unknown;
                }

                _saveManager.LoadSTFS(package, rawFileName, classInfo.extraIO.makeTempSaveDir());
                _packagePath = stfsPath;

                _stepTransfer.GameName = package.Header.Title_Package + " / " + package.Header.TitleID.ToString("X");
                _stepTransfer.Gamertag = package.Header.Title_Display + " / " + package.Header.ProfileID.ToString("X");
                _cexVerifyFile.Gamertag = package.Header.Title_Display;
            }
            catch (ArgumentException ex)
            {
                showMessage(ex.Message, "ERROR");
                return Util.SaveType.Unknown;
            }
            catch (Exception ex)
            {
                showException(ex.ToString());
                return Util.SaveType.Unknown;
            }
            finally
            {
                if (package != null)
                    package.CloseIO();
            }

            return _currentGame;
        }

        private Util.SaveType DetectGame(STFSPackage package, out string rawFileName)
        {
            if (((~package.Header.TitleID) ^ 0x12345678) == 0xAC9DA14C)
            {
                rawFileName = null;
                return Util.SaveType.SomeGame;
            }

            Util.SaveType game = Util.GameID.IdentifyGame(package);
            switch (game)
            {
                case Util.SaveType.Reach:
                    _saveManager = _reachSaveManager;
                    rawFileName = "mmiof.bmf";
                    break;

                case Util.SaveType.Anniversary:
                    _saveManager = _hcexSaveManager;
                    rawFileName = "saves.cfg";
                    break;

                default:
                    rawFileName = null;
                    break;
            }
            return game;
        }

        private void updateSaveFile()
        {
            STFSPackage package = new STFSPackage(_packagePath, null);
            _saveManager.SaveChanges(package, Properties.Resources.KV);
            package.CloseIO();
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
            isMouseDownEgg2 = false;
        }

        private void btnSettings_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isMouseDownEgg2 = true;
            egg2.Start();
        }

        private void btnSettings_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            btnSettings.Foreground = (Brush)bc.ConvertFrom(classInfo.AccentCodebase.AccentStorage.CodesideStorage.AccentTextDark);
            settingsPanel.Visibility = Visibility.Visible;
            isMouseDownEgg2 = false;
        }
        #endregion

        private bool isMouseDownEgg2 = false;
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!_stepViewer.Forward())
            {
                FormFadeOut.Begin();
                classInfo.applicationExtra.disableInput(this);
                return;
            }
        }
        void egg2_Tick(object sender, EventArgs e)
        {
            egg2.Stop();
            if (isMouseDownEgg2)
                eggData.egg2Data.enableFuckingRainbows();
        }

        private void stepViewer_BeforeViewNode(object sender, ViewNodeEventArgs e)
        {
            btnBack.Visibility = e.CanGoBack ? Visibility.Visible : Visibility.Hidden;

            if (!e.CanGoForward)
            {
                if (_stepSelectDevice.SelectedDevice != null)
                    _stepSelectDevice.SelectedDevice.Close();

                btnOK.Content = "Close";
                btnBack.Content = "Restart";
                if (_stepSelectMode.SelectedBranch == selectMode.EditingMode.EditSaveComputer)
                {
                    string argument = @"/select, " + _packagePath;
                    Process.Start("explorer.exe", argument);
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (!_stepViewer.CanGoForward)
            {
                // Restart
                _saveManager.Close();
                _stepOpenFile.FileUnloaded();
                _reachTaglists.RemoveMapSpecificTaglists();
                _stepViewer.ViewNode(_firstStep);
                btnOK.Content = "Next";
                btnBack.Content = "Back";
            }
            else
            {
                _stepViewer.Back();
            }
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

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !btnClose.IsEnabled;
        }
    }
}
