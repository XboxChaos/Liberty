using CloseableTabItemDemo;
using Liberty.Backend;
using Liberty.Metro.Controls.PageTemplates;
using Liberty.Metro.Dialogs;
using Liberty.Metro.Native;
using Liberty.SaveManager;
using Liberty.SaveManager.Games;
using Microsoft.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Liberty.Windows
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        public Home()
        {
            InitializeComponent();
            DwmDropShadow.DropShadowToWindow(this);
            this.AddHandler(CloseableTabItem.CloseTabEvent, new RoutedEventHandler(this.CloseTab));
            Settings.HomeWindow = this;

            UpdateTitleText("Home");
            UpdateStatusText("Ready...");

            //Window_StateChanged(null, null);
            ClearTabs();

            AddTabModule(TabGenre.StartPage);

            // Set width/height/state from last session
            if (Settings.applicationSizeHeight != double.NaN)
                this.Height = Settings.applicationSizeHeight;
            if (Settings.applicationSizeWidth != double.NaN)
                this.Width = Settings.applicationSizeWidth;
            if (Settings.applicationSizeMaximize)
                this.WindowState = System.Windows.WindowState.Maximized;
            else
                this.WindowState = System.Windows.WindowState.Normal;
            Window_StateChanged(null, null);
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            System.IntPtr handle = (new System.Windows.Interop.WindowInteropHelper(this)).Handle;
            System.Windows.Interop.HwndSource.FromHwnd(handle).AddHook(new System.Windows.Interop.HwndSourceHook(WindowProc));
        }

        #region Public Access Modifiers
        /// <summary>
        /// Set the title text of Liberty
        /// </summary>
        /// <param name="title">Current Title, Liberty shall add the rest for you.</param>
        public void UpdateTitleText(string title)
        {
            this.Title = title.Trim() + " - Liberty 4.0";
            lblTitle.Text = title.Trim() + " - Liberty 4.0";
        }

        /// <summary>
        /// Set the status text of Liberty
        /// </summary>
        /// <param name="status">Current Status of Liberty</param>
        public void UpdateStatusText(string status)
        {
            this.Status.Text = status;

            _statusUpdateTimer.Stop();
            _statusUpdateTimer.Interval = new TimeSpan(0, 0, 0, 4);
            _statusUpdateTimer.Tick += statusUpdateCleaner_Clear;
            _statusUpdateTimer.Start();
        }
        private void statusUpdateCleaner_Clear(object sender, EventArgs e)
        {
            this.Status.Text = "Ready...";
        }
        private DispatcherTimer _statusUpdateTimer = new DispatcherTimer();
        #endregion
        #region Tab Manager
        public void ClearTabs()
        {
            homeTabControl.Items.Clear();
        }

        private void CloseTab(object source, RoutedEventArgs args)
        {
            TabItem tabItem = args.Source as TabItem;
            if (tabItem != null)
            {
                dynamic tabContent = tabItem.Content;

                if (tabContent.Close())
                {
                    TabControl tabControl = tabItem.Parent as TabControl;
                    if (tabControl != null)
                        tabControl.Items.Remove(tabItem);
                }
            }
        }

        public void ExternalTabClose(TabItem tab)
        {
            homeTabControl.Items.Remove(tab);

            foreach (TabItem datTab in homeTabControl.Items)
                if (datTab.Header.ToString() == "Start Page")
                {
                    homeTabControl.SelectedItem = datTab;
                    return;
                }

            if (homeTabControl.Items.Count > 0)
                homeTabControl.SelectedIndex = homeTabControl.Items.Count - 1;
        }
        public void ExternalTabClose(TabGenre tabGenre)
        {
            string tabHeader = "";
            if (tabGenre == TabGenre.StartPage)
                tabHeader = "Start Page";
            else if (tabGenre == TabGenre.Settings)
                tabHeader = "Settings Page";

            TabItem toRemove = null;
            foreach (TabItem tab in homeTabControl.Items)
                if (tab.Header.ToString() == tabHeader)
                    toRemove = tab;

            if (toRemove != null)
                homeTabControl.Items.Remove(toRemove);
        }

        public enum TabGenre
        {
            StartPage,
            Settings,
            Welcome
        }
        public void AddTabModule(TabGenre tabG)
        {
            CloseableTabItem closeableTabItem = new CloseableTabItem();
            TabItem regularTabItem = new TabItem();
            closeableTabItem.Header = regularTabItem.Header = "swag";
            closeableTabItem.HorizontalAlignment = regularTabItem.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            closeableTabItem.VerticalAlignment = regularTabItem.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;


            switch (tabG)
            {
                case TabGenre.StartPage:
                    regularTabItem.Header = "Start Page";
                    regularTabItem.Content = new StartPage();
                    break;
            }

            if (closeableTabItem.Header != "swag")
            {
                foreach (TabItem tabb in homeTabControl.Items)
                    if (tabb.Header == closeableTabItem.Header)
                    {
                        homeTabControl.SelectedItem = tabb;
                        return;
                    }

                homeTabControl.Items.Add(closeableTabItem);
                homeTabControl.SelectedItem = closeableTabItem;
            }
            if (regularTabItem.Header != "swag")
            {
                foreach (TabItem tabb in homeTabControl.Items)
                    if (tabb.Header == regularTabItem.Header)
                    {
                        homeTabControl.SelectedItem = tabb;
                        return;
                    }

                homeTabControl.Items.Add(regularTabItem);
                homeTabControl.SelectedItem = regularTabItem;
            }
        }

        public void AddGameSaveModule(SaveLocalStorage gameSave, Utilities.HaloGames game)
        {
            // Check it isn't already open
            if (Settings.OpenedSaves.Contains(gameSave.USID))
            {
                // Tell User
                MetroMessageBox.Show("Save Already Open!", "The selected gamesave is already open, let us take you there.");

                foreach (TabItem tab in homeTabControl.Items)
                    if ((string)tab.Tag == gameSave.USID)
                    {
                        homeTabControl.SelectedItem = tab;
                        break;
                    }
            }
            else
            {
                // Lets open this save
                CloseableTabItem tab = new CloseableTabItem();
                tab.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                tab.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                tab.Tag = gameSave.USID;

                switch(game)
                {
                    case Utilities.HaloGames.Halo3:
                        tab.Content = new SaveManager.Games.Halo3.Halo3SaveViewer(gameSave, tab);
                        break;

                    case Utilities.HaloGames.Halo3_ODST: break;
                    case Utilities.HaloGames.HaloReach: break;
                    case Utilities.HaloGames.HaloCEX: break;
                    case Utilities.HaloGames.Halo4: break;
                }

                homeTabControl.Items.Add(tab);
                homeTabControl.SelectedItem = tab;
            }
        }
        #endregion
        #region More WPF Annoyance
        private void headerThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Left = Left + e.HorizontalChange;
            Top = Top + e.VerticalChange;
        }

        private void ResizeDrop_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double yadjust = this.Height + e.VerticalChange;
            double xadjust = this.Width + e.HorizontalChange;

            if (xadjust > this.MinWidth)
                this.Width = xadjust;
            if (yadjust > this.MinHeight)
                this.Height = yadjust;
        }
        private void ResizeRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double xadjust = this.Width + e.HorizontalChange;

            if (xadjust > this.MinWidth)
                this.Width = xadjust;
        }
        private void ResizeBottom_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double yadjust = this.Height + e.VerticalChange;

            if (yadjust > this.MinHeight)
                this.Height = yadjust;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                borderFrame.BorderThickness = new Thickness(1, 1, 1, 23);
                Settings.applicationSizeMaximize = false;
                Settings.applicationSizeHeight = this.Height;
                Settings.applicationSizeWidth = this.Width;
                Settings.UpdateSettings();

                btnActionRestore.Visibility = System.Windows.Visibility.Collapsed;
                btnActionMaxamize.Visibility = ResizeDropVector.Visibility = ResizeDrop.Visibility = ResizeRight.Visibility = ResizeBottom.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.WindowState == System.Windows.WindowState.Maximized)
            {
                borderFrame.BorderThickness = new Thickness(0, 0, 0, 23);
                Settings.applicationSizeMaximize = true;
                Settings.UpdateSettings();

                btnActionRestore.Visibility = System.Windows.Visibility.Visible;
                btnActionMaxamize.Visibility = ResizeDropVector.Visibility = ResizeDrop.Visibility = ResizeRight.Visibility = ResizeBottom.Visibility = System.Windows.Visibility.Collapsed;
            }
            /*
             * ResizeDropVector
             * ResizeDrop
             * ResizeRight
             * ResizeBottom
             */
        }
        private void headerThumb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
                this.WindowState = System.Windows.WindowState.Maximized;
            else if (this.WindowState == System.Windows.WindowState.Maximized)
                this.WindowState = System.Windows.WindowState.Normal;
        }
        private void btnActionSupport_Click(object sender, RoutedEventArgs e)
        {
            // Load support page?
        }
        private void btnActionMinimize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }
        private void btnActionRestore_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Normal;
        }
        private void btnActionMaxamize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Maximized;
        }
        private void btnActionClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        #endregion
        #region Maximize Workspace Workarounds
        private System.IntPtr WindowProc(
              System.IntPtr hwnd,
              int msg,
              System.IntPtr wParam,
              System.IntPtr lParam,
              ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }

            return (System.IntPtr)0;
        }
        private void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {
            Liberty.Metro.Native.Monitor_Workarea.MINMAXINFO mmi = (Liberty.Metro.Native.Monitor_Workarea.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(Liberty.Metro.Native.Monitor_Workarea.MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            System.IntPtr monitor = Liberty.Metro.Native.Monitor_Workarea.MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != System.IntPtr.Zero)
            {
                System.Windows.Forms.Screen scrn = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(this).Handle);

                Liberty.Metro.Native.Monitor_Workarea.MONITORINFO monitorInfo = new Liberty.Metro.Native.Monitor_Workarea.MONITORINFO();
                Liberty.Metro.Native.Monitor_Workarea.GetMonitorInfo(monitor, monitorInfo);
                Liberty.Metro.Native.Monitor_Workarea.RECT rcWorkArea = monitorInfo.rcWork;
                Liberty.Metro.Native.Monitor_Workarea.RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);

                /*
                mmi.ptMaxPosition.x = Math.Abs(scrn.Bounds.Left - scrn.WorkingArea.Left);
                mmi.ptMaxPosition.y = Math.Abs(scrn.Bounds.Top - scrn.WorkingArea.Top);
                mmi.ptMaxSize.x = Math.Abs(scrn.Bounds.Right - scrn.WorkingArea.Left);
                mmi.ptMaxSize.y = Math.Abs(scrn.Bounds.Bottom - scrn.WorkingArea.Top);
                */
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }
        #endregion
        #region Opacity Masking
        public int OpacityIndex = 0;
        public void ShowMask()
        {
            OpacityIndex++;
            OpacityMask.Visibility = System.Windows.Visibility.Visible;
        }
        public void HideMask()
        {
            OpacityIndex--;

            if (OpacityIndex == 0)
                OpacityMask.Visibility = System.Windows.Visibility.Collapsed;
        }
        #endregion
        #region Single Instancing
        //[STAThread]
        //public static void Main()
        //{
        //    if (SingleInstance<App>.InitializeAsFirstInstance("RecivedCommand"))
        //    {
        //        var application = new App();

        //        application.InitializeComponent();
        //        application.Run();

        //        // Allow single instance code to perform cleanup operations
        //        SingleInstance<App>.Cleanup();
        //    }
        //}
        #endregion

        private void homeTabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            TabItem tab = (TabItem)homeTabControl.SelectedItem;

            if (tab != null)
                UpdateTitleText(tab.Header.ToString().Replace("__", "_").Replace(".map", ""));

            if (tab != null && tab.Header.ToString() == "Start Page")
                ((StartPage)tab.Content).UpdateRecents();

            if (tab != null)
            {
                // Update Settings inside the content.
                // (try/catch) this, as not all pages will have this function
                //try { dynamic tabContent = tab.Content; tabContent.RefreshFromSettings(); }
                //catch { }
            }
        }

        private void menuCloseApplication_Click(object sender, RoutedEventArgs e) { Application.Current.Shutdown(); }
    }
}