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
        void Load(Util.SaveManager saveManager);
        bool Save(Util.SaveManager saveManager);

        IStepNode Next { get; }
        IStepNode Previous { get; }
    }
}
