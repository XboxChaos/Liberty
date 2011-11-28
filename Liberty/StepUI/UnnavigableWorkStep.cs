using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Liberty.StepUI
{
    public class UnnavigableWorkStep : IWorkStep
    {
        public UnnavigableWorkStep(IWorkStep step, UIElement buttonContainer, UIElement closeButton)
        {
            _step = step;
            _buttonContainer = buttonContainer;
            _closeButton = closeButton;

            step.ProgressChanged += new WorkStepProgressEvent(step_ProgressChanged);
            step.Complete += new WorkStepCompletedEvent(step_Complete);
        }

        public void Show()
        {
            _buttonContainer.Visibility = Visibility.Collapsed;
            _closeButton.IsEnabled = false;
            _step.Show();
        }

        public void Hide()
        {
            _step.Hide();
            _buttonContainer.Visibility = Visibility.Visible;
            _closeButton.IsEnabled = true;
        }

        public void Load()
        {
            _step.Load();
        }

        public bool Save()
        {
            return _step.Save();
        }

        void step_ProgressChanged(IWorkStep step, double progress)
        {
            if (ProgressChanged != null)
                ProgressChanged(step, progress);
        }

        void step_Complete(IWorkStep step)
        {
            if (Complete != null)
                Complete(step);
        }

        private IWorkStep _step;
        private UIElement _buttonContainer;
        private UIElement _closeButton;

        public event WorkStepProgressEvent ProgressChanged;
        public event WorkStepCompletedEvent Complete;
    }
}
