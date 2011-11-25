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
        void Load();

        /// <summary>
        /// Finishes editing and saves any data changed by the user.
        /// </summary>
        /// <returns>Whether or not the save operation succeeded</returns>
        bool Save();
    }
}
