using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.StepUI
{
    public interface IBranchStep<T> : IStep
    {
        T SelectedBranch { get; }
    }
}
