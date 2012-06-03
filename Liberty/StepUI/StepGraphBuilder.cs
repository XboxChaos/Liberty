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
                SimpleStepNode node = new SimpleStepNode(DecorateStep(step, null, _rootNode == null));
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
                SimpleStepNode node = new SimpleStepNode(DecorateStep(step, FindGroup(progressGroupName), _rootNode == null));
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
            BranchStepNode<T> node = new BranchStepNode<T>(branchData, DecorateStep(branchData, FindGroup(progressGroupName), true));
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
            foreach (ProgressSetupStep step in _progressSetup)
                step.AddGroup(name, group);
        }

        public virtual IStepNode BuildGraph()
        {
            foreach (StepGraphBuilder branch in _branches)
                branch.BuildGraph();

            return _rootNode;
        }

        private ProgressSetupStep DecorateWithProgressSetup(IStep step)
        {
            ProgressSetupStep result = new ProgressSetupStep(step, _progressBar);
            foreach (KeyValuePair<string, ProgressBarGroup> group in _groups)
                result.AddGroup(group.Key, group.Value);
            _progressSetup.Add(result);
            return result;
        }

        private IStep DecorateStep(IStep step, ProgressBarGroup progressBarGroup, bool setupProgressBar)
        {
            IStep result = step;
            if (setupProgressBar)
                result = DecorateWithProgressSetup(step);
            if (progressBarGroup != null)
                result = new ProgressLinkedStep(result, progressBarGroup);
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
                foreach (ProgressSetupStep step in _progressSetup)
                    step.AddGroup(groupName, group);
            }
            return group;
        }

        private IStepNode _rootNode = null;
        private IStepNode _lastNode = null;
        private SimpleStepNode _lastSimpleNode = null;
        private List<ProgressSetupStep> _progressSetup = new List<ProgressSetupStep>();
        private IStepProgressBar _progressBar = null;
        private List<StepGraphBuilder> _branches = new List<StepGraphBuilder>();
        private Dictionary<string, ProgressBarGroup> _groups = new Dictionary<string, ProgressBarGroup>();
    }
}
