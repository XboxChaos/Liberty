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
using System.ComponentModel;

namespace Liberty
{
	/// <summary>
	/// Interaction logic for saving.xaml
	/// </summary>
	public partial class saving : UserControl, StepUI.IWorkStep
	{
        private MainWindow _mainWindow = null;
        private Action _saveAction;

		public saving(Action saveAction)
		{
            _saveAction = saveAction;
			this.InitializeComponent();
            this.Loaded += new RoutedEventHandler(saving_Loaded);
		}

        void saving_Loaded(object sender, RoutedEventArgs e)
        {
            _mainWindow = Window.GetWindow(this) as MainWindow;
        }

        public void Show()
        {
            Visibility = Visibility.Visible;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.RunWorkerAsync();
        }

        public void Hide()
        {
            Visibility = Visibility.Hidden;
        }

        public void Load()
        {
        }

        public bool Save()
        {
            return true;
        }

        private void doSave()
        {
            _saveAction();
            OnComplete();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            _saveAction();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                _mainWindow.showException(e.Error.ToString(), true);

            OnComplete();
        }

        protected void OnComplete()
        {
            if (Complete != null)
                Complete(this);
        }

        protected void OnProgressChanged(double progress)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, progress);
        }

        public event StepUI.WorkStepProgressEvent ProgressChanged;
        public event StepUI.WorkStepCompletedEvent Complete;
    }
}