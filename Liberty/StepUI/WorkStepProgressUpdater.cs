using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Liberty.StepUI
{
    public class WorkStepProgressUpdater : IStep
    {
        public WorkStepProgressUpdater(IWorkStep step, ProgressBarGroup group, StepViewer viewer)
        {
            _step = step;
            _group = group;
            _viewer = viewer;

            step.ProgressChanged += step_OnProgressChanged;
            step.Complete += step_OnComplete;
        }

        public void Show()
        {
            _step.Show();
        }

        public void Hide()
        {
            _step.Hide();
        }

        public void Load(Util.SaveManager saveManager)
        {
            _saveManager = saveManager;
            _step.Load(saveManager);
        }

        public bool Save(Util.SaveManager saveManager)
        {
            return _step.Save(saveManager);
        }

        private void step_OnProgressChanged(IWorkStep step, double progress)
        {
            _group.UpdateStepProgress(progress);
        }

        private void step_OnComplete(IWorkStep step)
        {
            _viewer.Forward(_saveManager);
        }

        private IWorkStep _step;
        private ProgressBarGroup _group;
        private StepViewer _viewer;
        private Util.SaveManager _saveManager;
    }
}
