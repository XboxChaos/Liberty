using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.StepUI
{
    public class StepGraphBuilder
    {
        public StepGraphBuilder(IStepProgressBar progressBar)
        {
            _progressBar = progressBar;
        }

        public IStepNode AddStep(IStep step)
        {
            if (_rootNode == null || _lastSimpleNode != null)
            {
                SimpleStepNode node = new SimpleStepNode(DecorateStep(step, null));
                AddNode(node, node);
                return node;
            }
            return null;
        }

        public IStepNode AddStep(IStep step, string progressGroupName)
        {
            if (_rootNode == null || _lastSimpleNode != null)
            {
                // Just decorate the step and wrap it in a SimpleStepNode
                SimpleStepNode node = new SimpleStepNode(DecorateStep(step, FindGroup(progressGroupName)));
                AddNode(node, node);
                return node;
            }
            return null;
        }

        public void AddPlaceholder(string progressGroupName)
        {
            ProgressBarGroup group;
            if (_groups.TryGetValue(progressGroupName, out group))
                group.AddPlaceholder();
        }

        public BranchStepNode<T> AddBranchStep<T>(IBranchStep<T> branchData, string progressGroupName)
        {
            BranchStepNode<T> node = new BranchStepNode<T>(branchData, DecorateStep(branchData, FindGroup(progressGroupName)));
            AddNode(node, null);
            return node;
        }

        public StepGraphBuilder StartBranch<T>(T branch, bool copyGroups)
        {
            BranchStepNode<T> branchNode = (BranchStepNode<T>)_lastNode;
            BranchStepGraphBuilder<T> builder = new BranchStepGraphBuilder<T>(_progressBar, branchNode, branch);

            if (copyGroups)
            {
                foreach (KeyValuePair<string, ProgressBarGroup> group in _groups)
                    builder._groups.Add(group.Key, (ProgressBarGroup)group.Value.Clone());
            }

            _branches.Add(builder);
            return builder;
        }

        public void AddGroup(string name)
        {
            AddGroup(name, new ProgressBarGroup());
        }

        public void AddGroup(string name, ProgressBarGroup group)
        {
            _groups[name] = group;
            if (_progressSetup != null)
                _progressSetup.AddGroup(name, group);
        }

        public virtual IStepNode BuildGraph()
        {
            foreach (StepGraphBuilder branch in _branches)
                branch.BuildGraph();

            return _rootNode;
        }

        private IStep DecorateStep(IStep step, ProgressBarGroup progressBarGroup)
        {
            IStep result = step;
            if (_rootNode == null)
            {
                // Wrap it in a ProgressSetupStep and add any existing groups to it
                // The ProgressSetupStep decorator creates the effect where the group names appear on the progress bar
                _progressSetup = new ProgressSetupStep(result, _progressBar);
                foreach (KeyValuePair<string, ProgressBarGroup> group in _groups)
                    _progressSetup.AddGroup(group.Key, group.Value);
                result = _progressSetup;
            }

            if (progressBarGroup != null)
            {
                // Wrap it in a ProgressLinkedStep and add it to the group
                // The ProgressLinkedStep decorator updates the progress bar whenever the node is activated
                result = new ProgressLinkedStep(result, progressBarGroup);
            }
            return result;
        }

        private void AddNode(IStepNode node, SimpleStepNode simpleNode)
        {
            if (_rootNode == null)
            {
                _rootNode = node;
            }
            else if (_lastSimpleNode != null)
            {
                _lastSimpleNode.AttachNode(node);

                // HAX: attach the node to the top-level decorator so that Previous works correctly
                node.AttachTo(_lastNode);
            }

            _lastNode = node;
            _lastSimpleNode = simpleNode;
        }

        private ProgressBarGroup FindGroup(string groupName)
        {
            ProgressBarGroup group;
            if (!_groups.TryGetValue(groupName, out group))
            {
                // The group doesn't exist - create a new one and add it to the ProgressSetupStep node
                group = new ProgressBarGroup();
                _groups[groupName] = group;
                if (_progressSetup != null)
                    _progressSetup.AddGroup(groupName, group);
            }
            return group;
        }

        private IStepNode _rootNode = null;
        private IStepNode _lastNode = null;
        private SimpleStepNode _lastSimpleNode = null;
        private ProgressSetupStep _progressSetup = null;
        private IStepProgressBar _progressBar = null;
        private List<StepGraphBuilder> _branches = new List<StepGraphBuilder>();
        private Dictionary<string, ProgressBarGroup> _groups = new Dictionary<string, ProgressBarGroup>();
    }
}
