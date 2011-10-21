using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.StepUI
{
    public interface IStepProgressBar
    {
        /// <summary>
        /// Adds an item group to the progress bar.
        /// </summary>
        /// <param name="name">The group's name to display above the progress bar.</param>
        /// <param name="group">The IProgressBarGroup to add.</param>
        void AddGroup(string name, ProgressBarGroup group);

        /// <summary>
        /// Removes all groups from the progress bar.
        /// </summary>
        void Clear();
    }
}
