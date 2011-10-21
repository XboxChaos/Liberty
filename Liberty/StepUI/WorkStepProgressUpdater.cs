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

        public void Load(Util.SaveEditor saveEditor)
        {
            _saveEditor = saveEditor;
            _step.Load(saveEditor);
        }

        public bool Save(Util.SaveEditor saveEditor)
        {
            return _step.Save(saveEditor);
        }

        private void step_OnProgressChanged(IWorkStep step, double progress)
        {
            _group.UpdateStepProgress(progress);
        }

        private void step_OnComplete(IWorkStep step)
        {
            _viewer.Forward(_saveEditor);
        }

        private IWorkStep _step;
        private ProgressBarGroup _group;
        private StepViewer _viewer;
        private Util.SaveEditor _saveEditor;
    }
}
