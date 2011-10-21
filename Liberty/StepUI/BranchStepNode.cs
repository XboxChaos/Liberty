using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Liberty.StepUI
{
    /// <summary>
    /// Wraps an IBranchStep so that different nodes can be jumped to based upon the step's SelectedBranch.
    /// </summary>
    /// <typeparam name="T">The type of the step's SelectedBranch value.</typeparam>
    public class BranchStepNode<T> : IStepNode
    {
        /// <summary>
        /// Constructs a BranchStepNode from an IBranchStep and an IStep.
        /// </summary>
        /// <param name="branchStep">The IBranchStep to use for node selection logic.</param>
        /// <param name="displayStep">The IStep to display.</param>
        public BranchStepNode(IBranchStep<T> branchStep, IStep displayStep)
        {
            _displayStep = displayStep;
            _branchStep = branchStep;
        }

        /// <summary>
        /// Connects an IStepNode to one of the IBranchStep's branches.
        /// </summary>
        /// <param name="branch">The branch to connect to (see the concrete IBranchStep implementation this wraps).</param>
        /// <param name="node">The IStepNode that should be jumped to if the branch is selected.</param>
        public void ConnectBranch(T branch, IStepNode node)
        {
            node.AttachTo(this);
            _branches[branch] = node;
        }

        public void AttachTo(IStepNode node)
        {
            _previous = node;
        }

        public void Show()
        {
            _displayStep.Show();
        }

        public void Hide()
        {
            _displayStep.Hide();
        }

        public void Load(Util.SaveManager saveManager)
        {
            _displayStep.Load(saveManager);
        }

        public bool Save(Util.SaveManager saveManager)
        {
            return _displayStep.Save(saveManager);
        }

        public IStepNode Next
        {
            get
            {
                IStepNode next;
                if (!_branches.TryGetValue(_branchStep.SelectedBranch, out next))
                    return null;
                return next;
            }
        }

        public IStepNode Previous
        {
            get { return _previous; }
        }

        public Dictionary<T, IStepNode> _branches = new Dictionary<T, IStepNode>();
        public IStep _displayStep;
        public IBranchStep<T> _branchStep;
        public IStepNode _previous = null;
    }
}
