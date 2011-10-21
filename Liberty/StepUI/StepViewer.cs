﻿using System;
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

        public void ViewNode(IStepNode node, Util.SaveManager saveManager)
        {
            if (_currentNode != null)
                _currentNode.Hide();
            _currentNode = node;
            node.Load(saveManager);
            node.Show();
        }

        public bool Forward(Util.SaveManager saveManager)
        {
            if (_currentNode == null)
                return false;
            if (!_currentNode.Save(saveManager))
                return true;

            IStepNode nextNode = _currentNode.Next;
            if (nextNode == null)
                return false;
            nextNode.Load(saveManager);
            _currentNode.Hide();
            _currentNode = nextNode;
            nextNode.Show();
            return true;
        }

        public bool Back(Util.SaveManager saveManager)
        {
            if (_currentNode == null)
                return false;

            IStepNode previousNode = _currentNode.Previous;
            if (previousNode == null)
                return false;
            _currentNode.Hide();
            _currentNode = previousNode;
            _currentNode.Load(saveManager);
            _currentNode.Show();
            return true;
        }

        private Grid _stepGrid;
        private IStepNode _currentNode = null;
    }
}
