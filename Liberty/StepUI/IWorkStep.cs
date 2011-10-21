using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.StepUI
{
    public delegate void WorkStepProgressEvent(IWorkStep step, double progress);
    public delegate void WorkStepCompletedEvent(IWorkStep step);

    public interface IWorkStep : IStep
    {
        event WorkStepProgressEvent ProgressChanged;
        event WorkStepCompletedEvent Complete;
    }
}
