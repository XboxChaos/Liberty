using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.StepUI
{
    public delegate void ProgressEventHandler(ProgressBarGroup group, double progress);

    public class ProgressBarGroup : ICloneable
    {
        /// <summary>
        /// Adds a ProgressLinkedStep to this group.
        /// </summary>
        /// <param name="step">The ProgressLinkedStep to add.</param>
        public void AddStep(ProgressLinkedStep step)
        {
            int position = _stepPositions.Count;
            _stepPositions[step] = position;
            _numPositions++;
        }

        /// <summary>
        /// Adds a placeholder step at the end of the group that doesn't copy over when this is cloned
        /// So much hax....
        /// </summary>
        public void AddPlaceholder()
        {
            _numPositions++;
        }

        /// <summary>
        /// Removes all ProgressLinkedSteps from this group.
        /// </summary>
        public void Clear()
        {
            _stepPositions.Clear();
            OnProgressChanged(0);
            _progressRangeStart = 0;
            _progressRangeEnd = 1;
        }

        /// <summary>
        /// Activates a ProgressLinkedStep, updating any attached progress bars.
        /// </summary>
        public void ActivateStep(ProgressLinkedStep step)
        {
            int position;
            if (_stepPositions.TryGetValue(step, out position))
            {
                _progressRangeStart = (double)position / _numPositions;
                _progressRangeEnd = (double)(position + 1) / _numPositions;
                OnProgressChanged(_progressRangeStart);
            }
            else
            {
                throw new InvalidOperationException("Cannot activate a node that is not part of this group");
            }
        }

        public void UpdateStepProgress(double progress)
        {
            OnProgressChanged(_progressRangeStart + progress * (_progressRangeEnd - _progressRangeStart));
        }

        public object Clone()
        {
            ProgressBarGroup newGroup = new ProgressBarGroup();
            newGroup._progressRangeStart = _progressRangeStart;
            newGroup._progressRangeEnd = _progressRangeEnd;
            foreach (KeyValuePair<ProgressLinkedStep, int> position in _stepPositions)
                newGroup._stepPositions.Add(position.Key, position.Value);
            if (ProgressChanged != null)
                newGroup.ProgressChanged = (ProgressEventHandler)ProgressChanged.Clone();
            newGroup._numPositions = newGroup._stepPositions.Count;
            return newGroup;
        }

        public event ProgressEventHandler ProgressChanged;

        protected void OnProgressChanged(double newProgress)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, newProgress);
        }

        private double _progressRangeStart = 0;
        private double _progressRangeEnd = 1;
        private double _numPositions = 0;
        private Dictionary<ProgressLinkedStep, int> _stepPositions = new Dictionary<ProgressLinkedStep, int>();
    }
}
