using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Liberty.StepUI
{
    public class ViewNodeEventArgs : EventArgs
    {
        public ViewNodeEventArgs(IStepNode nextNode, bool canGoBack, bool canGoForward)
        {
            NextNode = nextNode;
            _canGoBack = canGoBack;
            _canGoForward = canGoForward;
        }

        public IStepNode NextNode { get; set; }
        public bool CanGoBack { get { return _canGoBack; } }
        public bool CanGoForward { get { return _canGoForward; } }

        private bool _canGoBack;
        private bool _canGoForward;
    }

    public class StepViewer
    {
        public StepViewer(Grid stepGrid)
        {
            _stepGrid = stepGrid;
        }

        public bool CanGoBack
        {
            get { return (_currentNode != null && _currentNode.Previous != null); }
        }

        public bool CanGoForward
        {
            get { return (_currentNode != null && _currentNode.Next != null); }
        }

        public void ViewNode(IStepNode node)
        {
            if (BeforeViewNode != null)
            {
                ViewNodeEventArgs args = new ViewNodeEventArgs(node, node.Previous != null, node.Next != null);
                BeforeViewNode(this, args);
            }

            IStepNode previousNode = _currentNode;
            _currentNode = node;

            node.Load();
            if (previousNode != null)
                previousNode.Hide();
            node.Show();
        }

        public bool Forward()
        {
            if (_currentNode == null)
                return false;
            if (!_currentNode.Save())
                return true;

            IStepNode nextNode = _currentNode.Next;
            if (nextNode == null)
                return false;

            ViewNode(nextNode);
            return true;
        }

        public bool Back()
        {
            if (_currentNode == null)
                return false;

            IStepNode previousNode = _currentNode.Previous;
            if (previousNode == null)
                return false;

            ViewNode(previousNode);
            return true;
        }

        public event EventHandler<ViewNodeEventArgs> BeforeViewNode;

        private Grid _stepGrid;
        private IStepNode _currentNode = null;
    }
}
