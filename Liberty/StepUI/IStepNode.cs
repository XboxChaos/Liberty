using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Liberty.StepUI
{
    public interface IStepNode
    {
        void AttachTo(IStepNode node);

        void Show();
        void Hide();
        void Load();
        bool Save();

        IStepNode Next { get; }
        IStepNode Previous { get; }
    }
}
