using System;
using System.Collections.Generic;
using System.IO;
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
using Liberty.Backend;
using Liberty.Metro.Dialogs;
using Liberty.Windows;
using System.Windows.Media.Animation;
using System.Threading;
using System.ComponentModel;
using CLKsFATXLib;
using System.Collections.ObjectModel;
using Liberty.ViewModels;
using Liberty.IO;
using Liberty.SaveManager;
using Liberty.SaveManager.Flexibility;
using X360.STFS;
using Liberty.SaveManager.Games;

namespace Liberty.Metro.Controls.PageTemplates
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : UserControl
    {
        private IList<Drive> _physDrives = null;
        private Drive _selectedDrive = null;
        public StartPageViewModel PageViewModel = new StartPageViewModel();

        public StartPage()
        {
            InitializeComponent();

            // Save the Start Page
            Backend.Settings.StartPage = this;

            // Setup the UI
            saveSelector.Visibility = Visibility.Collapsed;
            mainContent.Visibility = Visibility.Visible;
            mainContent.Opacity = 0.0;

            // Set ViewModel
            this.DataContext = PageViewModel;

            // Call the nice Introduction Animation
            (FindResource("ShowWelcomePage") as Storyboard).Begin();
        }
        public bool Close() { return true; }

        public void LoadRecentItem(object sender, RoutedEventArgs e)
        {

        }
        public void UpdateRecents()
        {

        }

        private void btnOpenLocalSave_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnOpenExternalSave_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sbWelcomePage = FindResource("HideWelcomePage") as Storyboard;
            sbWelcomePage.Completed += (o, args) =>
                {
                    LoadSavesFromDevices();

                    Storyboard sbSaveSelector = FindResource("ShowSaveSelector") as Storyboard;
                    sbSaveSelector.Begin();
                };
            sbWelcomePage.Begin();
        }

        public void LoadSavesFromDevices()
        {
            // Do Save Selector Code
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (o2, args2) =>
            {
                Dispatcher.Invoke(new Action(delegate 
                {
                    // Setup Pending UI stuff
                    progressPendingRefresh.Visibility = System.Windows.Visibility.Visible;

                    // Close Existing Streams
                    foreach (StartPageViewModel.FATXGameSave save in PageViewModel.FATXGameSaves)
                        save.Stream.Close();

                    // Clear ObservableCollection
                    PageViewModel.FATXGameSaves.Clear();

                }));

                if (_physDrives != null)
                {
                    foreach (Drive drive in _physDrives)
                        drive.Close();
                    _physDrives = null;
                }

                // Get Drives 
                _physDrives = StartHere.GetFATXDrives().ToList();

                // Get Saves from said drives
                foreach (Drive drive in _physDrives)
                {
                    Folder xboxContent = drive.FolderFromPath("Data\\Content");

                    foreach (Folder content in xboxContent.Folders())
                    {
                        if (content.Name.StartsWith("E"))
                        {
                            // We're in a profile, lets go right into a game file
                            foreach (Manager.SupportedGames.SupportedGame supportedGame in Manager.SupportedGame.Games)
                            {
                                string filePath = string.Format("{0}\\Content\\{1}\\{2}\\00000001", "Data", content.Name, supportedGame.TitleID.ToString("X"));
                                if (drive.DirectoryExists(filePath))
                                {
                                    Folder profileGamesaveContent = drive.FolderFromPath(filePath);

                                    // Look though the profile folder for a campaign save
                                    foreach (CLKsFATXLib.File gamesaveContent in profileGamesaveContent.Files())
                                    {
                                        if (gamesaveContent.Name.StartsWith(supportedGame.STFSStartsWith))
                                        {
                                            try
                                            {
                                                // Is a campaign save, woop!
                                                // Add to the ObservableCollection and move out!
                                                StartPageViewModel.FATXGameSave fatxGamesave = new StartPageViewModel.FATXGameSave();

                                                fatxGamesave.FATXPath = gamesaveContent.FullPath;
                                                fatxGamesave.PackageName = gamesaveContent.Name;
                                                fatxGamesave.GameName = supportedGame.Name;
                                                fatxGamesave.SafeGameName = supportedGame.SafeName;
                                                fatxGamesave.SupportedGame = supportedGame;
                                                fatxGamesave.FATXDrive = drive;

                                                // Check the save isn't already open
                                                string USID = Utilities.CreateUniqueSaveIdentification(gamesaveContent.Name, gamesaveContent.FullPath, true);
                                                if (!Backend.Settings.OpenedSaves.Contains(USID))
                                                {
                                                    // Extract gamestate to get data about the save
                                                    STFSPackage package = new STFSPackage(new X360.IO.DJsIO(gamesaveContent.GetStream(), true), null);
                                                    FileEntry gamestateHeader = package.GetFile("gamestate.hdr");
                                                    string extractionPath = Backend.VariousFunctions.CreateTemporaryFile(Backend.VariousFunctions.GetTemporaryExtractionLocation());
                                                    gamestateHeader.Extract(extractionPath);

                                                    // Create Stream, lol
                                                    EndianStream endianStream = new EndianStream(new MemoryStream(System.IO.File.ReadAllBytes(extractionPath)), Endian.BigEndian);

                                                    // Get Header Layout
                                                    StructureValueCollection saveHeader = StructureReader.ReadStructure(endianStream, supportedGame.GetLayout("header"));

                                                    fatxGamesave.Stream = endianStream;
                                                    fatxGamesave.USID = USID;
                                                    fatxGamesave.MapName = SaveManager.Utilities.GetFriendlyMissionName(saveHeader.GetString("scenario"), (Utilities.HaloGames)Enum.Parse(typeof(Utilities.HaloGames), supportedGame.SafeName));
                                                    fatxGamesave.MapScenario = saveHeader.GetString("scenario");
                                                    string libertyImagePath = string.Format(@"/Liberty;component/Metro/Images/Games/MapImages/{0}/{1}.jpg", supportedGame.SafeName, saveHeader.GetString("scenario").Substring(saveHeader.GetString("scenario").LastIndexOf('\\') + 1).ToLower());
                                                    fatxGamesave.MapImage = libertyImagePath;
                                                    fatxGamesave.GamerTag = saveHeader.GetString("gamer tag");
                                                    fatxGamesave.ServiceTag = saveHeader.GetString("service tag");

                                                    Dispatcher.Invoke(new Action(delegate
                                                    {
                                                        // Add to ObservableCollection, has to be invoked to do this
                                                        PageViewModel.FATXGameSaves.Add(fatxGamesave);

                                                    }));
                                                }
                                            }
                                            catch { }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            bw.RunWorkerCompleted += (o, args) =>
            {
                // Setup Pending UI stuff
                Dispatcher.Invoke(new Action(delegate { progressPendingRefresh.Visibility = System.Windows.Visibility.Collapsed; }));
            };
            bw.RunWorkerAsync();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sbWelcomePage = FindResource("HideSaveSelector") as Storyboard;
            sbWelcomePage.Completed += (o, args) =>
            {
                Storyboard sbSaveSelector = FindResource("ShowWelcomePage") as Storyboard;
                sbSaveSelector.Begin();
            };
            sbWelcomePage.Begin();
        }
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadSavesFromDevices();
        }

        private void fatxSaveSelector_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (fatxSaveSelector.SelectedItem != null && fatxSaveSelector.SelectedItem is StartPageViewModel.FATXGameSave)
            {
                // Get Selected Save
                StartPageViewModel.FATXGameSave fatxGameSave = (StartPageViewModel.FATXGameSave)fatxSaveSelector.SelectedItem;
                SaveLocalStorage gamesaveStorage = new SaveLocalStorage();

                // Write selected save data
                gamesaveStorage.FATXDrive = fatxGameSave.FATXDrive;
                gamesaveStorage.IsSaveFromFATX = true;
                gamesaveStorage.FATXPath = gamesaveStorage.FATXPath;
                gamesaveStorage.SavePackageName = fatxGameSave.PackageName;
                gamesaveStorage.USID = fatxGameSave.USID;

                // Create extraction location
                string extractionLocation = Backend.VariousFunctions.CreateTemporaryFile(Backend.VariousFunctions.GetTemporaryExtractionLocation());

                // Extract STFS Package from Device
                fatxGameSave.FATXDrive.FileFromPath(fatxGameSave.FATXPath).Extract(extractionLocation);

                // Close Stream
                fatxGameSave.Stream.Close();

                // Write Extraction Path
                gamesaveStorage.SaveLocalPath = extractionLocation;

                // Load Game Viewer
                Settings.HomeWindow.AddGameSaveModule(gamesaveStorage, (Utilities.HaloGames)Enum.Parse(typeof(Utilities.HaloGames), fatxGameSave.SafeGameName));
            }
        }
    }
}
