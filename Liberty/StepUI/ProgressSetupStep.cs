using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Liberty.StepUI
{
    class ProgressSetupStep : IStep
    {
        public ProgressSetupStep(IStep step, IStepProgressBar progressBar)
        {
            _step = step;
            _progressBar = progressBar;
        }

        public void AddGroup(string name, ProgressBarGroup group)
        {
            _groups[name.ToUpper()] = group;
        }

        public void Show()
        {
            _step.Show();

            _progressBar.Clear();
            foreach (KeyValuePair<string, ProgressBarGroup> group in _groups)
                _progressBar.AddGroup(group.Key, group.Value);
        }

        public void Hide()
        {
            _step.Hide();
        }

        public void Load()
        {
            _step.Load();
        }

        public bool Save()
        {
            return _step.Save();
        }

        private IStep _step;
        private IStepProgressBar _progressBar;
        private Dictionary<string, ProgressBarGroup> _groups = new Dictionary<string, ProgressBarGroup>();
    }
}
