using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Liberty.StepUI
{
    public class SimpleStepNode : IStepNode
    {
        public SimpleStepNode(IStep step)
        {
            _step = step;
        }

        public void AttachNode(IStepNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node cannot be null");

            // Place the new node in-between this node and the current next node
            IStepNode oldNext = _next;
            node.AttachTo(this);
            if (oldNext != null)
                oldNext.AttachTo(node);

            // Link the node to us
            _next = node;
        }

        public SimpleStepNode AttachStep(IStep step)
        {
            SimpleStepNode newNode = new SimpleStepNode(step);
            AttachNode(newNode);
            return newNode;
        }

        public void AttachTo(IStepNode node)
        {
            _previous = node;
        }

        public void Show()
        {
            _step.Show();
        }

        public void Hide()
        {
            _step.Hide();
        }

        public void Load()
        {
            _step.Load();
        }

        public bool Save()
        {
            return _step.Save();
        }

        public IStepNode Next
        {
            get { return _next; }
        }

        public IStepNode Previous
        {
            get { return _previous; }
        }

        private IStep _step;
        private IStepNode _next = null;
        private IStepNode _previous = null;
    }
}
