using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace @interface
{
    /// <summary>
    /// Main form for the Process Time Tracker application
    /// </summary>
    public partial class Form1 : Form
    {
        private UsageGraph _usageGraph;
        private ListView _processListView;
        private CategorySelector _categorySelector;
        private StatsPanel _statsPanel;
        private List<ProcessData> _processData;
        private ProcessData _selectedProcess;
        private ProcessMonitor _processMonitor;
        private Timer _updateTimer;

        /// <summary>
        /// Constructor for the main form
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            Text = "Process Time Tracker";
            Size = new Size(800, 600);
            
            InitializeControls();
            
            // Initialize process monitor and timer for updating UI
            _processMonitor = new ProcessMonitor();
            _updateTimer = new Timer();
            _updateTimer.Interval = 10000; // Update every 10 seconds
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
            
            // Load initial data
            LoadRealProcessData();
        }

        /// <summary>
        /// Initializes the custom controls
        /// </summary>
        private void InitializeControls()
        {
            // Create the usage graph
            _usageGraph = new UsageGraph
            {
                Location = new Point(10, 10),
                Size = new Size(550, 250),
                Title = "Usage Over Time",
                TimeRange = "1 month"
            };
            
            // Create the process list view
            _processListView = new ListView
            {
                Location = new Point(10, 270),
                Size = new Size(550, 280),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            
            // Add columns to the list view
            _processListView.Columns.Add("Name", 200);
            _processListView.Columns.Add("PID", 70);
            _processListView.Columns.Add("Dept", 100);
            _processListView.Columns.Add("Today", 80, HorizontalAlignment.Right);
            
            // Handle list view selection
            _processListView.SelectedIndexChanged += ProcessListView_SelectedIndexChanged;
            
            // Create the category selector
            _categorySelector = new CategorySelector
            {
                Location = new Point(570, 10),
                Size = new Size(200, 250),
                Categories = new List<string> { "Games", "Learning", "Coding", "Entertainment", "Other" }
            };
            
            // Handle category selection
            _categorySelector.CategorySelected += CategorySelector_CategorySelected;
            
            // Create the stats panel
            _statsPanel = new StatsPanel
            {
                Location = new Point(570, 270),
                Size = new Size(200, 280)
            };
            
            // Add controls to the form
            Controls.Add(_usageGraph);
            Controls.Add(_processListView);
            Controls.Add(_categorySelector);
            Controls.Add(_statsPanel);
        }

        /// <summary>
        /// Updates the UI with current process data
        /// </summary>
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            LoadRealProcessData();
        }

        /// <summary>
        /// Loads real process data from the system
        /// </summary>
        private void LoadRealProcessData()
        {
            _processData = _processMonitor.GetProcessData();
            UpdateProcessList();
        }

        /// <summary>
        /// Updates the process list based on the selected category
        /// </summary>
        private void UpdateProcessList()
        {
            _processListView.Items.Clear();
            
            // Filter processes by category
            List<ProcessData> filteredProcesses = _processData;
            
            if (_categorySelector.SelectedCategory != "All")
            {
                filteredProcesses = _processData.Where(p => p.Department == _categorySelector.SelectedCategory).ToList();
            }
            
            // Add filtered processes to the list view
            foreach (ProcessData process in filteredProcesses)
            {
                ListViewItem item = new ListViewItem(process.Name);
                item.SubItems.Add(process.PID.ToString());
                item.SubItems.Add(process.Department);
                item.SubItems.Add(FormatTimeSpan(process.TimeToday));
                item.Tag = process;
                
                _processListView.Items.Add(item);
            }
        }

        /// <summary>
        /// Updates the graph with data from the selected process
        /// </summary>
        private void UpdateGraph()
        {
            if (_selectedProcess != null)
            {
                _usageGraph.Title = $"{_selectedProcess.Name} Usage";
                _usageGraph.TimeData = _selectedProcess.HistoricalData;
            }
            else
            {
                _usageGraph.Title = "Usage Over Time";
                _usageGraph.TimeData = new List<KeyValuePair<DateTime, double>>();
            }
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

        /// <summary>
        /// Handles selection changes in the process list view
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProcessListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_processListView.SelectedItems.Count > 0)
            {
                ListViewItem item = _processListView.SelectedItems[0];
                _selectedProcess = item.Tag as ProcessData;
                
                UpdateGraph();
                _statsPanel.UpdateStats(_selectedProcess);
            }
        }

        /// <summary>
        /// Handles category selection changes
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="category">Selected category</param>
        private void CategorySelector_CategorySelected(object sender, string category)
        {
            UpdateProcessList();
        }

        /// <summary>
        /// Clean up resources when form is closing
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _updateTimer.Stop();
            _processMonitor.StopMonitoring();
        }
    }
}
