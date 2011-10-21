using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.StepUI
{
    public class BranchStepGraphBuilder<T> : StepGraphBuilder
    {
        public BranchStepGraphBuilder(IStepProgressBar progressBar, BranchStepNode<T> node, T branch)
            : base(progressBar)
        {
            _node = node;
            _branch = branch;
        }

        public override IStepNode BuildGraph()
        {
            IStepNode root = base.BuildGraph();
            _node.ConnectBranch(_branch, root);
            return root;
        }

        private BranchStepNode<T> _node;
        private T _branch;
    }
}
