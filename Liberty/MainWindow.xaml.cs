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
using Liberty.HCEX.UI;
using Liberty.Halo3.UI;
using Liberty.Halo3ODST.UI;
using Liberty.Halo4.UI;

using X360.STFS;
using System.Windows.Threading;
using System.Threading;
using System.Reflection;

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
        private Util.SaveManager<Halo3.CampaignSave> _halo3SaveManager;
        private Util.SaveManager<Halo3ODST.CampaignSave> _halo3ODSTSaveManager;
        private Util.SaveManager<Halo4.CampaignSave> _halo4SaveManager;
        private Reach.TagListManager _reachTaglists = null;
        private string _reachTaglistDir = applicationSettings.extTaglistFromAscDirec;
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

            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;

            // Setup egg2 Timers
            egg2.Interval = new TimeSpan(0, 0, 20);
            egg2.Tick += new EventHandler(egg2_Tick);

            // Set settings menu visible state
            settingsMain.SettingsChanged += new EventHandler(settingsMain_SettingsChanged);
            settingsMain.Closed += new EventHandler(settingsMain_Closed);
            settingsPanel.Visibility = Visibility.Hidden;

            // Set up reach stuff
            _reachSaveManager = new Util.SaveManager<Reach.CampaignSave>(path => new Reach.CampaignSave(path));
            _reachTaglists = new Reach.TagListManager(_reachSaveManager);

            // Set up HCEX stuff
            _hcexSaveManager = new Util.SaveManager<HCEX.CampaignSave>(path => new HCEX.CampaignSave(path));

            // Set up Halo3 stuff
            _halo3SaveManager = new Util.SaveManager<Halo3.CampaignSave>(path => new Halo3.CampaignSave(path));

            // Set up Halo3: ODST stuff
            _halo3ODSTSaveManager = new Util.SaveManager<Halo3ODST.CampaignSave>(path => new Halo3ODST.CampaignSave(path));

            // Set up Halo4 stuff
            _halo4SaveManager = new Util.SaveManager<Halo4.CampaignSave>(path => new Halo4.CampaignSave(path));

            // Set up the step viewer
            _stepViewer = new StepViewer(stepGrid);
            _stepViewer.BeforeViewNode += new EventHandler<ViewNodeEventArgs>(stepViewer_BeforeViewNode);

            // Create steps
            _stepSelectMode = new selectMode();
            _stepOpenFile = new openSaveFile(loadSaveFile);
            _stepSelectDevice = new selectDevice();
            selectSaveOnDevice stepSelectSave = new selectSaveOnDevice(_stepSelectDevice, _saveTransferrer, loadSaveFile);

            saving stepSaving = new saving(updateSaveFile);
            _stepTransfer = new transferSave(_saveTransferrer, this);
            allDone stepAllDone = new allDone(_stepSelectMode);

            #region Reach
            verifyFile reachVerifyFile = new verifyFile(_reachSaveManager, _reachTaglists, this);
            editBiped reachBiped = new editBiped(_reachSaveManager, _reachTaglists);
            editWeapons reachWeapons = new editWeapons(_reachSaveManager, _reachTaglists);
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

            #region Halo3
            h3VerifyFile h3VerifyFile = new Halo3.UI.h3VerifyFile(_halo3SaveManager);
            h3EditBiped h3EditBiped = new Halo3.UI.h3EditBiped(_halo3SaveManager);
            h3EditWeapons h3EditWeapons = new Halo3.UI.h3EditWeapons(_halo3SaveManager);
            h3EditGrenades h3EditGrenades = new Halo3.UI.h3EditGrenades(_halo3SaveManager);
            h3QuickTweaks h3QuickTweaks = new Halo3.UI.h3QuickTweaks(_halo3SaveManager);
            #endregion

            #region Halo3ODST
            h3ODSTVerifyFile h3ODSTVerifyFile = new Halo3ODST.UI.h3ODSTVerifyFile(_halo3ODSTSaveManager);
            h3ODSTEditBiped h3ODSTEditBiped = new Halo3ODST.UI.h3ODSTEditBiped(_halo3ODSTSaveManager);
            h3ODSTEditWeapons h3ODSTEditWeapons = new Halo3ODST.UI.h3ODSTEditWeapons(_halo3ODSTSaveManager);
            h3ODSTEditGrenades h3ODSTEditGrenades = new Halo3ODST.UI.h3ODSTEditGrenades(_halo3ODSTSaveManager);
            #endregion

            #region Halo 4
            h4VerifyFile h4VerifyFile = new Halo4.UI.h4VerifyFile(_halo4SaveManager);
            h4EditBiped h4EditBiped = new Halo4.UI.h4EditBiped(_halo4SaveManager);
            h4EditWeapons h4EditWeapons = new Halo4.UI.h4EditWeapons(_halo4SaveManager);
            h4EditGrenades h4EditGrenades = new Halo4.UI.h4EditGrenades(_halo4SaveManager);
            h4QuickTweaks h4QuickTweaks = new Halo4.UI.h4QuickTweaks(_halo4SaveManager);
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

            #region Halo3Steps
            addStep(h3VerifyFile);
            addStep(h3EditBiped);
            addStep(h3EditWeapons);
            addStep(h3EditGrenades);
            addStep(h3QuickTweaks);
            #endregion

            #region Halo3ODSTSteps
            addStep(h3ODSTVerifyFile);
            addStep(h3ODSTEditBiped);
            addStep(h3ODSTEditWeapons);
            addStep(h3ODSTEditGrenades);
            #endregion

            #region Halo4Steps
            addStep(h4VerifyFile);
            addStep(h4EditBiped);
            addStep(h4EditWeapons);
            addStep(h4EditGrenades);
            addStep(h4QuickTweaks);
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
            editSaveOnComputer.AddPlaceholder("SAVE SELECTION");

            // Step graph: Edit save on removable device
            StepGraphBuilder editSaveOnDevice = stepGraph.StartBranch(selectMode.EditingMode.EditSaveDevice, true);
            editSaveOnDevice.AddStep(_stepSelectDevice, "SAVE SELECTION");
            editSaveOnDevice.AddBranchStep(stepSelectSave, "SAVE SELECTION");
            editSaveOnDevice.AddPlaceholder("SAVE SELECTION");

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
            hcexDeviceSave.AddStep(workStepTransfer);
            hcexDeviceSave.AddStep(stepAllDone, "FINISHED");
            #endregion

            #region Halo3Steps
            // Step graph: Edit Halo3 save on computer
            StepGraphBuilder halo3ComputerSave = editSaveOnComputer.StartBranch(Util.SaveType.Halo3, true);
            halo3ComputerSave.AddStep(h3VerifyFile, "SAVE SELECTION");
            halo3ComputerSave.AddStep(h3EditBiped, "CHARACTER DATA");
            halo3ComputerSave.AddStep(h3EditWeapons, "WEAPON DATA");
            halo3ComputerSave.AddStep(h3EditGrenades, "WEAPON DATA");
            halo3ComputerSave.AddStep(h3QuickTweaks, "OBJECT DATA");
            halo3ComputerSave.AddStep(workStepSaving);
            halo3ComputerSave.AddStep(stepAllDone, "FINISHED");

            // Step graph: Edit Halo3 save on removable device
            StepGraphBuilder halo3DeviceSave = editSaveOnDevice.StartBranch(Util.SaveType.Halo3, true);
            halo3DeviceSave.AddStep(h3VerifyFile, "SAVE SELECTION");
            halo3DeviceSave.AddStep(h3EditBiped, "CHARACTER DATA");
            halo3DeviceSave.AddStep(h3EditWeapons, "WEAPON DATA");
            halo3DeviceSave.AddStep(h3EditGrenades, "WEAPON DATA");
            halo3DeviceSave.AddStep(h3QuickTweaks, "OBJECT DATA");
            halo3DeviceSave.AddStep(workStepSaving);
            halo3DeviceSave.AddStep(workStepTransfer);
            halo3DeviceSave.AddStep(stepAllDone, "FINISHED");
            #endregion

            #region Halo3ODSTSteps
            // Step graph: Edit Halo3 save on computer
            StepGraphBuilder halo3ODSTComputerSave = editSaveOnComputer.StartBranch(Util.SaveType.Halo3ODST, true);
            halo3ODSTComputerSave.AddStep(h3ODSTVerifyFile, "SAVE SELECTION");
            halo3ODSTComputerSave.AddStep(h3ODSTEditBiped, "CHARACTER DATA");
            halo3ODSTComputerSave.AddStep(h3ODSTEditWeapons, "WEAPON DATA");
            halo3ODSTComputerSave.AddStep(h3ODSTEditGrenades, "WEAPON DATA");
            halo3ODSTComputerSave.AddStep(workStepSaving);
            halo3ODSTComputerSave.AddStep(stepAllDone, "FINISHED");

            // Step graph: Edit Halo3 save on removable device
            StepGraphBuilder halo3ODSTDeviceSave = editSaveOnDevice.StartBranch(Util.SaveType.Halo3ODST, true);
            halo3ODSTDeviceSave.AddStep(h3ODSTVerifyFile, "SAVE SELECTION");
            halo3ODSTDeviceSave.AddStep(h3ODSTEditBiped, "CHARACTER DATA");
            halo3ODSTDeviceSave.AddStep(h3ODSTEditWeapons, "WEAPON DATA");
            halo3ODSTDeviceSave.AddStep(h3ODSTEditGrenades, "WEAPON DATA");
            halo3ODSTDeviceSave.AddStep(workStepSaving);
            halo3ODSTDeviceSave.AddStep(workStepTransfer);
            halo3ODSTDeviceSave.AddStep(stepAllDone, "FINISHED");
            #endregion

            #region Halo4Steps
            // Step graph: Edit Halo3 save on computer
            StepGraphBuilder halo4ComputerSave = editSaveOnComputer.StartBranch(Util.SaveType.Halo4, true);
            halo4ComputerSave.AddStep(h4VerifyFile, "SAVE SELECTION");
            halo4ComputerSave.AddStep(h4EditBiped, "CHARACTER DATA");
            halo4ComputerSave.AddStep(h4EditWeapons, "WEAPON DATA");
            halo4ComputerSave.AddStep(h4EditGrenades, "WEAPON DATA");
            halo4ComputerSave.AddStep(h4QuickTweaks, "OBJECT DATA");
            halo4ComputerSave.AddStep(workStepSaving);
            halo4ComputerSave.AddStep(stepAllDone, "FINISHED");

            // Step graph: Edit Halo3 save on removable device
            StepGraphBuilder halo4DeviceSave = editSaveOnDevice.StartBranch(Util.SaveType.Halo4, true);
            halo4DeviceSave.AddStep(h4VerifyFile, "SAVE SELECTION");
            halo4DeviceSave.AddStep(h4EditBiped, "CHARACTER DATA");
            halo4DeviceSave.AddStep(h4EditWeapons, "WEAPON DATA");
            halo4DeviceSave.AddStep(h4EditGrenades, "WEAPON DATA");
            halo4DeviceSave.AddStep(h4QuickTweaks, "OBJECT DATA");
            halo4DeviceSave.AddStep(workStepSaving);
            halo4DeviceSave.AddStep(workStepTransfer);
            halo4DeviceSave.AddStep(stepAllDone, "FINISHED");
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
            lblBetaWatermark.Visibility = System.Windows.Visibility.Visible;

            string datetime = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString().Remove(0, 2);
            Security.SaveSHA1 sha1 = new Security.SaveSHA1();
            UTF8Encoding utf = new UTF8Encoding();
            byte[] buildHash = Security.SaveSHA1.ComputeHash(utf.GetBytes(Assembly.GetExecutingAssembly().GetName().Version.ToString() + lblBetaWatermark.Text));


            lblBetaWatermark.Text = string.Format("Liberty Developer Preview" + Environment.NewLine + "Build {0}.tfs_main.{1}-1733.{2}",
                Assembly.GetExecutingAssembly().GetName().Version.ToString().Replace(".", ""),
                datetime,
                BitConverter.ToString(buildHash).ToLower().Replace("-", "").Remove(16));
#else
            btnBetaPlayground.Visibility = System.Windows.Visibility.Hidden;
            lblBetaWatermark.Visibility = System.Windows.Visibility.Hidden;
#endif
        }

        void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            showException(e.Exception.ToString(), true);
            e.Handled = true;
        }

        private void addStep(UIElement step)
        {
            step.Visibility = Visibility.Collapsed;
            stepGrid.Children.Add(step);
        }

        private Util.SaveType loadSaveFile(string stfsPath)
        {
            // lolmultithreading

            _reachTaglists.RemoveMapSpecificTaglists();
            if (_saveManager != null)
                _saveManager.Close();

            STFSPackage package = null;
            try
            {
                // Open the STFS package
                string rawFileName;
                try
                {
                    package = new STFSPackage(stfsPath, null);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("The selected file is not a valid STFS package.", ex);
                }

                // Detect the save's game
                _currentGame = detectGame(package, out rawFileName);
                classInfo.storage.settings.applicationSettings.gameIdent.gameID = _currentGame;

                if (_currentGame == Util.SaveType.Unknown)
                {
                    Action notSupportedAction = new Action(() =>
                        {
                            showMessage(package.Header.Title_Display + " saves are not supported yet. Currently, only Halo 3, Halo 3: ODST, Halo: Reach, and Halo: CE Anniversary saves are supported. Please select a different file.", "GAME NOT SUPPORTED");
                        }
                    );
                    Dispatcher.Invoke(notSupportedAction);
                    return Util.SaveType.Unknown;
                }
                else if (_currentGame == Util.SaveType.SomeGame)
                {
                    Action eggAction = new Action(() =>
                        {
                            eggData.egg3Dialog dialog = new eggData.egg3Dialog();
                            dialog.Owner = this;
                            dialog.ShowDialog();
                        }
                    );
                    Dispatcher.Invoke(eggAction);
                    return Util.SaveType.Unknown;
                }

                _saveManager.LoadSTFS(package, rawFileName, classInfo.extraIO.makeTempSaveDir());
                _packagePath = stfsPath;

                // Update some UI controls with package info
                Dispatcher.Invoke(new Action<string, uint, string, long>(setPackageInfo),
                    new object[] {package.Header.Title_Package,
                        package.Header.TitleID,
                        package.Header.Title_Display,
                        package.Header.ProfileID});
            }
            catch (ArgumentException ex)
            {
                Dispatcher.Invoke(new Action<string, string>(showMessage), new object[] { ex.Message, "ERROR" });
                return Util.SaveType.Unknown;
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action<string, bool>(showException), new object[] { ex.ToString(), true });
                return Util.SaveType.Unknown;
            }
            finally
            {
                if (package != null)
                    package.CloseIO();
            }

            return _currentGame;
        }

        private void setPackageInfo(string package, uint titleId, string display, long profileId)
        {
            _stepTransfer.GameName = package + " / " + titleId.ToString("X");
            _stepTransfer.Gamertag = display + " / " + profileId.ToString("X");
            _cexVerifyFile.Gamertag = display;
        }

        private Util.SaveType detectGame(STFSPackage package, out string rawFileName)
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

                case Util.SaveType.Halo3:
                    _saveManager = _halo3SaveManager;
                    rawFileName = "mmiof.bmf";
                    break;
                case Util.SaveType.Halo3ODST:
                    _saveManager = _halo3ODSTSaveManager;
                    rawFileName = "mmiof.bmf";
                    break;

                case Util.SaveType.Halo4:
                    _saveManager = _halo4SaveManager;
                    rawFileName = "mmiof.bmf";
                    break;

                default:
                    rawFileName = null;
                    break;
            }
            return game;
        }

        private void makeBackup()
        {
            if (!showQuestion("Would you like to make a backup of your save file before it's overwritten? It only takes a few seconds and can be useful in case you need to report a problem.", "BACKUP CREATION"))
                return;

            SaveFileDialog ofd = new SaveFileDialog();
            ofd.Title = "Save Backup As";
            ofd.FileName = "backup_" + System.IO.Path.GetFileName(_packagePath);
            if ((bool)ofd.ShowDialog())
            {
                try
                {
                    File.Copy(_packagePath, ofd.FileName, true);
                }
                catch (Exception ex)
                {
                    showException(ex.ToString(), true);
                }
            }
        }

        private void updateSaveFile()
        {
            // FIXME: This is hax, you have to invoke the dispatcher from a STA thread so that showQuestion will work -_-
            Thread backupThread = new Thread(() =>
                {
                    Dispatcher.Invoke(new Action(makeBackup));
                });
            backupThread.SetApartmentState(ApartmentState.STA);
            backupThread.Start();
            backupThread.Join();

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

        public void showException(string message, bool canContinue)
        {
            exceptionWindow exp = new exceptionWindow(message, canContinue);
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

        #region MultiGameCoordGetter
        public listcordWindow showListCordWindow(HCEX.CampaignSave obj1, HCEX.TagGroup obj2)
        {
            listcordWindow msgListCord = new listcordWindow(obj1, obj2);
            msgListCord.Owner = this;
            msgListCord.ShowDialog();

            return msgListCord;
        }
        public listcordWindow showListCordWindow(Halo3.CampaignSave obj1, Halo3.TagGroup obj2)
        {
            listcordWindow msgListCord = new listcordWindow(obj1, obj2);
            msgListCord.Owner = this;
            msgListCord.ShowDialog();

            return msgListCord;
        }
        public listcordWindow showListCordWindow(Reach.CampaignSave obj1, Reach.TagGroup obj2, Reach.TagListManager obj3)
        {
            listcordWindow msgListCord = new listcordWindow(obj1, obj2, obj3);
            msgListCord.Owner = this;
            msgListCord.ShowDialog();

            return msgListCord;
        }

        #endregion

        public ListBoxItem showListBox(string message, string title, List<ListBoxItem> items)
        {
            listboxWindow listbox = new listboxWindow(items);
            listbox.lblTitle.Text = title.ToUpper();
            listbox.lblSubInfo.Text = message;
            listbox.Owner = this;
            listbox.ShowDialog();

            return listbox.selectedItem;
        }

        public TreeViewItem showObjectTree(string message, string title, IEnumerable<TreeViewItem> items)
        {
            selectObjectWindow selectObject = new selectObjectWindow(items);
            selectObject.lblTitle.Text = title.ToUpper();
            selectObject.lblSubInfo.Text = message;
            selectObject.Owner = this;
            selectObject.ShowDialog();

            return selectObject.selectedItem;
        }

        public bool showWarning(string message, string title)
        {
            if (!applicationSettings.noWarnings)
                return showQuestion(message, title);
            else
                return true;
        }

        public void enableNextButton(bool enabled)
        {
            btnOK.IsEnabled = enabled;
        }

        public void enableBackButton(bool enabled)
        {
            btnBack.IsEnabled = enabled;
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

        private void showSecretStuff(string Key)
        {
            // Should we show it?
            RegistryKey keyApp = Registry.CurrentUser.CreateSubKey("Software\\Xeraxic\\Liberty\\");

            if ((string)keyApp.GetValue("secret", "0") == "0")
            {
                dat_super_secret_app secret = new dat_super_secret_app(Key);
                secret.Owner = this;
                secret.ShowDialog();

                keyApp.SetValue("secret", "1");
            }
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
            if (IsVisible)
            {
                App app = (App)Application.Current;

                if (app.initException != null)
                    showException(app.initException, true);

                if (app._secretAES != null)
                    showSecretStuff(app._secretAES);

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
        }

        private void settingsMain_SettingsChanged(object sender, EventArgs e)
        {
            if (_reachSaveManager.Loaded)
            {
                if (applicationSettings.extTaglistFrmAsc)
                {
                    string newTaglistDir = applicationSettings.extTaglistFromAscDirec.ToLower();
                    if (newTaglistDir != _reachTaglistDir)
                    {
                        try
                        {
                            classInfo.nameLookup.loadAscensionTaglist(_reachSaveManager, _reachTaglists);
                        }
                        catch (Exception ex)
                        {
                            showException("Unable to load this map's Ascension taglist:\n\n" + ex.Message, true);
                        }
                        _reachTaglistDir = newTaglistDir;
                    }
                }
                else
                {
                    _reachTaglists.RemoveMapSpecificTaglists();
                }
            }
        }

        private void settingsMain_Closed(object sender, EventArgs e)
        {
            settingsPanel.Visibility = System.Windows.Visibility.Hidden;
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
        private void btnAbout_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            about();
        }
        #endregion

        #region btnCFUwpf
        private void btnCFU_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            checkForUpdates();
        }
        #endregion

        #region btnBugReportwpf
        private void btnBugReport_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            showLeavingDialog("http://liberty.codeplex.com/workitem/list/basic", "CodePlex");
        }
        #endregion

        #region btnSettingswpf
        private void btnSettings_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            isMouseDownEgg2 = false;
        }

        private void btnSettings_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isMouseDownEgg2 = true;
            egg2.Start();
        }

        private void btnSettings_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (settingsPanel.Visibility != Visibility.Visible)
            {
                settingsMain.Reload();
                settingsPanel.Visibility = Visibility.Visible;
                isMouseDownEgg2 = false;
            }
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

            if (e.NextNode is SimpleStepNode && e.NextNode.Next == null)
            {
                if (_stepSelectDevice.SelectedDevice != null)
                    _stepSelectDevice.SelectedDevice.Close();

                btnOK.Content = "Close";
                btnBack.Content = "Restart";

                //if (_stepSelectMode.SelectedBranch == selectMode.EditingMode.EditSaveComputer)
                //{
                //    string argument = @"/select, " + _packagePath;
                //    Process.Start("explorer.exe", argument);
                //}
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if ((string)btnBack.Content == "Restart")
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
                btnOK.IsEnabled = true;
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
