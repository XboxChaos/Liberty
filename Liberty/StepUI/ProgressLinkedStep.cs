using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Liberty.StepUI
{
    /// <summary>
    /// Wraps an IStepNode object, linking it to a StepProgressBarGroup.
    /// </summary>
    public class ProgressLinkedStep : IStep
    {
        public ProgressLinkedStep(IStep step, ProgressBarGroup group)
        {
            _step = step;
            _group = group;
            _group.AddStep(this);
        }

        public void Show()
        {
            _step.Show();
            _group.ActivateStep(this);
        }

        public void Hide()
        {
            _step.Hide();
        }

        public void Load(Util.SaveEditor saveEditor)
        {
            _step.Load(saveEditor);
        }

        public bool Save(Util.SaveEditor saveEditor)
        {
            return _step.Save(saveEditor);
        }

        private IStep _step;
        private ProgressBarGroup _group;
    }
}
