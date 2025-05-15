using System;
using System.Drawing;
using System.Windows.Forms;

namespace @interface
{
    /// <summary>
    /// Control for displaying usage statistics
    /// </summary>
    public class StatsPanel : Panel
    {
        private Label _titleLabel;
        private Label _totalTimeLabel;
        private Label _idleTimeLabel;
        private Label _activeTimeLabel;
        private Label _totalTimeValueLabel;
        private Label _idleTimeValueLabel;
        private Label _activeTimeValueLabel;

        /// <summary>
        /// Constructor for StatsPanel
        /// </summary>
        public StatsPanel()
        {
            BorderStyle = BorderStyle.FixedSingle;
            Padding = new Padding(5);
            
            InitializeLabels();
        }

        /// <summary>
        /// Initializes the labels for the stats panel
        /// </summary>
        private void InitializeLabels()
        {
            // Title label
            _titleLabel = new Label
            {
                Text = "All time",
                Font = new Font("Arial", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            
            // Total time label
            _totalTimeLabel = new Label
            {
                Text = "Total today",
                AutoSize = true,
                Location = new Point(10, 40)
            };
            
            // Idle time label
            _idleTimeLabel = new Label
            {
                Text = "Idle",
                AutoSize = true,
                Location = new Point(10, 70)
            };
            
            // Active time label
            _activeTimeLabel = new Label
            {
                Text = "Active",
                AutoSize = true,
                Location = new Point(10, 100)
            };
            
            // Total time value label
            _totalTimeValueLabel = new Label
            {
                Text = "0h 0m",
                Font = new Font("Arial", 9, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(100, 40),
                TextAlign = ContentAlignment.MiddleRight
            };
            
            // Idle time value label
            _idleTimeValueLabel = new Label
            {
                Text = "0h 0m",
                Font = new Font("Arial", 9, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(100, 70),
                TextAlign = ContentAlignment.MiddleRight
            };
            
            // Active time value label
            _activeTimeValueLabel = new Label
            {
                Text = "0h 0m",
                Font = new Font("Arial", 9, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(100, 100),
                TextAlign = ContentAlignment.MiddleRight
            };
            
            // Add labels to panel
            Controls.Add(_titleLabel);
            Controls.Add(_totalTimeLabel);
            Controls.Add(_idleTimeLabel);
            Controls.Add(_activeTimeLabel);
            Controls.Add(_totalTimeValueLabel);
            Controls.Add(_idleTimeValueLabel);
            Controls.Add(_activeTimeValueLabel);
        }

        /// <summary>
        /// Updates the statistics with process data
        /// </summary>
        /// <param name="processData">Process data to display</param>
        public void UpdateStats(ProcessData processData)
        {
            if (processData == null)
            {
                _titleLabel.Text = "All time";
                _totalTimeValueLabel.Text = "0h 0m";
                _idleTimeValueLabel.Text = "0h 0m";
                _activeTimeValueLabel.Text = "0h 0m";
                return;
            }
            
            _titleLabel.Text = processData.Name;
            
            TimeSpan totalTime = processData.GetTotalTime();
            TimeSpan idleTime = processData.GetIdleTime();
            TimeSpan activeTime = processData.GetActiveTime();
            
            _totalTimeValueLabel.Text = FormatTimeSpan(totalTime);
            _idleTimeValueLabel.Text = FormatTimeSpan(idleTime);
            _activeTimeValueLabel.Text = FormatTimeSpan(activeTime);
        }

        /// <summary>
        /// Formats a TimeSpan as hours and minutes
        /// </summary>
        /// <param name="time">TimeSpan to format</param>
        /// <returns>Formatted time string</returns>
        private string FormatTimeSpan(TimeSpan time)
        {
            return $"{time.Hours}h {time.Minutes}m";
        }
    }
} 