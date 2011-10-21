using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Liberty.Controls;

namespace Liberty.StepUI
{
    public class TransferSaveStep : IWorkStep
    {
        public event WorkStepProgressEvent ProgressChanged;
        public event WorkStepCompletedEvent Complete;

        public TransferSaveStep(Window owner, selectDevice deviceSelector, selectSaveOnDevice saveSelector)
        {
            _owner = owner;
            _deviceSelector = deviceSelector;
            _saveSelector = saveSelector;
        }

        public void Show()
        {
            Controls.progressWindow progress = new progressWindow(_deviceSelector, _saveSelector);
            progress.Owner = _owner;
            progress.ShowDialog();

            OnComplete();
        }

        public void Hide()
        {
        }

        public void Load(Util.SaveEditor saveEditor)
        {
        }

        public bool Save(Util.SaveEditor saveEditor)
        {
            _deviceSelector.SelectedDevice.Close();
            return true;
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

        private Window _owner;
        private selectDevice _deviceSelector;
        private selectSaveOnDevice _saveSelector;
    }
}
