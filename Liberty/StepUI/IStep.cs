using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Liberty.StepUI
{
    public interface IStep
    {
        /// <summary>
        /// Shows the step UI.
        /// </summary>
        void Show();

        /// <summary>
        /// Hides the step UI.
        /// </summary>
        void Hide();

        /// <summary>
        /// Loads any data needed by the step.
        /// </summary>
        /// <param name="saveEditor">The SaveEditor object that should be manipulated.</param>
        void Load(Util.SaveEditor saveEditor);

        /// <summary>
        /// Finishes editing and saves any data changed by the user.
        /// </summary>
        /// <param name="saveEditor">The SaveEditor object that should be manipulated.</param>
        bool Save(Util.SaveEditor saveEditor);
    }
}
