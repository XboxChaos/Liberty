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

namespace Liberty.Controls
{
	/// <summary>
	/// Interaction logic for progressStepBar.xaml
	/// </summary>
	public partial class progressStepBar : UserControl, StepUI.IStepProgressBar
	{
		public progressStepBar()
		{
			this.InitializeComponent();
            progressBar.Visibility = Visibility.Hidden;

            // Set up brushes
            BrushConverter bc = new BrushConverter();
            _selectedBrush = (Brush)bc.ConvertFrom("#FF868686");
            _unselectedBrush = (Brush)bc.ConvertFrom("#FFD4D2D2");
		}

        public void AddGroup(string name, StepUI.ProgressBarGroup group)
        {
            if (group != null)
                group.ProgressChanged += GroupProgressChanged;
            _groups.Add(group);

            // Set up a new TextBlock for the label
            TextBlock groupLabel = new TextBlock();
            groupLabel.Text = name.ToUpper();
            groupLabel.FontFamily = new FontFamily("Segoe UI");
            const double PixelsPerPoint = 96.0 / 72.0;
            groupLabel.FontSize = 6.75 * PixelsPerPoint;
            groupLabel.FontWeight = FontWeights.SemiBold;

            // Set the TextBlock's color based on whether or not it is the first label
            // Also set the current group
            if (gridItems.Children.Count == 0)
            {
                groupLabel.Foreground = _selectedBrush;
                _currentGroup = 0;
            }
            else
            {
                groupLabel.Foreground = _unselectedBrush;
            }

            // Fit the label to its contents
            classInfo.applicationExtra.fitTextBlock(groupLabel);
            _totalLabelWidth += groupLabel.Width;

            // Add the label, realign everything, and update the progress bar
            gridItems.Children.Add(groupLabel);
            AlignLabels();
            UpdateBar(0);
        }

        public void Clear()
        {
            _groups.Clear();
            _labelMidpoints.Clear();
            gridItems.Children.Clear();
            _totalLabelWidth = 0;
            _currentGroup = -1;
        }

        private void AlignLabels()
        {
            // Is there some sort of control that can do this?
            // Maybe I could have made some sort of UserControl from just this, but w/e

            // Calculate spacing so that the labels are evenly spaced
            double spacing = (gridItems.Width - _totalLabelWidth) / (double)(gridItems.Children.Count - 1);

            double left = 0;
            _labelMidpoints.Clear();
            for (int i = 0; i < gridItems.Children.Count; i++)
            {
                FrameworkElement label = (FrameworkElement)gridItems.Children[i];
                _labelMidpoints.Add(left + label.Width / 2);
                if (i == gridItems.Children.Count - 1 && gridItems.Children.Count > 1)
                {
                    // Last label - just right-align it
                    label.HorizontalAlignment = HorizontalAlignment.Right;
                    label.Margin = new Thickness(0, 5, 0, 0);
                }
                else
                {
                    label.HorizontalAlignment = HorizontalAlignment.Left;
                    label.Margin = new Thickness(left, 5, 0, 0);
                    left += label.Width + spacing;
                }
            }
        }

        private void UpdateBar(double groupProgress)
        {
            if (_currentGroup != -1)
            {
                progressBar.Visibility = Visibility.Visible;
                if (_currentGroup < _groups.Count - 1)
                {
                    double currentMidpoint = _labelMidpoints[_currentGroup];
                    double nextMidpoint = _labelMidpoints[_currentGroup + 1];
                    progressBar.Width = currentMidpoint + groupProgress * (nextMidpoint - currentMidpoint);
                }
                else if (_groups.Count == 1)
                {
                    double currentMidpoint = _labelMidpoints[_currentGroup];
                    progressBar.Width = currentMidpoint + groupProgress * (progressBarSub.Width - currentMidpoint);
                }
                else
                {
                    progressBar.Width = progressBarSub.Width;
                }

                // Update group label colors
                for (int i = 0; i < gridItems.Children.Count; i++)
                {
                    TextBlock label = gridItems.Children[i] as TextBlock;
                    if (label != null)
                    {
                        if (i == _currentGroup)
                            label.Foreground = _selectedBrush;
                        else
                            label.Foreground = _unselectedBrush;
                    }
                }
            }
            else
            {
                progressBar.Visibility = Visibility.Hidden;
            }
        }

        private void GroupProgressChanged(StepUI.ProgressBarGroup group, double progress)
        {
            _currentGroup = _groups.IndexOf(group);
            UpdateBar(progress);
        }

        private Brush _selectedBrush;
        private Brush _unselectedBrush;

        private double _totalLabelWidth = 0;
        private List<double> _labelMidpoints = new List<double>();
        private List<StepUI.ProgressBarGroup> _groups = new List<StepUI.ProgressBarGroup>();
        private int _currentGroup = -1;
    }
}