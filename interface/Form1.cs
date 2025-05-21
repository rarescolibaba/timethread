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
        private UsageDataService _dataService;
        private Timer _updateTimer;
        private const int DisplayDays = 30; // Number of days to display in the graph
        private DateTime _lastBootTime;
        private string _currentGraphView = "TOTAL_PC_ON_TIME"; // Possible values: TOTAL_PC_ON_TIME, CATEGORY, PROCESS
        private string _selectedCategoryForGraph = null;

        /// <summary>
        /// Constructor for the main form
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            Text = "Process Time Tracker";
            Size = new Size(800, 600);
            
            try
            {
                _lastBootTime = ProcessMonitor.GetLastBootUpTime();
                _dataService = new UsageDataService();
                InitializeControls();
                _processMonitor = new ProcessMonitor();
                
                _updateTimer = new Timer();
                _updateTimer.Interval = 10000; // Update every 10 seconds for process list and stats
                _updateTimer.Tick += UpdateTimer_Tick;
                _updateTimer.Start();
                
                LoadRealProcessData();
                UpdateGraphView(); // Initial graph view
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing application: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Updates the UI with current process data and stats panel
        /// </summary>
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            LoadRealProcessData();
            // Update stats panel with current uptime if no process is selected
            if (_selectedProcess == null)
            {
                TimeSpan uptime = ProcessMonitor.GetSystemUptime(_lastBootTime);
                _statsPanel.UpdateSystemUptime(uptime);
            } 
            // else the stats panel is updated by ProcessListView_SelectedIndexChanged or CategorySelector_CategorySelected
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
            
            // If a process was previously selected, try to find and select it again
            if (_selectedProcess != null)
            {
                foreach (ListViewItem item in _processListView.Items)
                {
                    ProcessData process = item.Tag as ProcessData;
                    if (process != null && process.PID == _selectedProcess.PID)
                    {
                        item.Selected = true;
                        break;
                    }
                }
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
        /// Updates the graph based on the current view state (_currentGraphView, _selectedProcess, _selectedCategoryForGraph)
        /// </summary>
        private void UpdateGraphView()
        {
            List<KeyValuePair<DateTime, double>> graphData = new List<KeyValuePair<DateTime, double>>();
            string graphTitle = "PC Usage Data";

            switch (_currentGraphView)
            {
                case "PROCESS":
                    if (_selectedProcess != null)
                    {
                        graphTitle = $"{_selectedProcess.Name} Usage (Active Time)";
                        graphData = _dataService.GetHistoricalDataForProcess(_selectedProcess.Name, DisplayDays);
                    }
                    else
                    {
                        _currentGraphView = "TOTAL_PC_ON_TIME";
                        UpdateGraphView(); 
                        return;
                    }
                    break;

                case "CATEGORY":
                    if (!string.IsNullOrEmpty(_selectedCategoryForGraph) && _selectedCategoryForGraph != "All")
                    {
                        graphTitle = $"{_selectedCategoryForGraph} Category (Total Active Time)";
                        graphData = _dataService.GetTotalActiveTimeForCategory(_selectedCategoryForGraph, DisplayDays);
                    }
                    else
                    {
                        _currentGraphView = "TOTAL_PC_ON_TIME";
                         _selectedCategoryForGraph = null; 
                        UpdateGraphView(); 
                        return;
                    }
                    break;

                case "TOTAL_PC_ON_TIME":
                default:
                    graphTitle = "Total PC On Time (Persisted)";
                    // Use the persisted data for the graph
                    graphData = _dataService.GetPersistedDailySystemOnTime(DisplayDays);
                    // For today's entry in the graph, we might want to update it with the live calculated value if it's more current
                    // than the last saved value, or rely on ProcessMonitor to save it frequently.
                    // For simplicity now, we use what's persisted. ProcessMonitor updates it every minute.
                    _currentGraphView = "TOTAL_PC_ON_TIME"; 
                    break;
            }

            _usageGraph.Title = graphTitle;
            if (graphData.Any() && graphData.Any(d => d.Value > 0.001)) // Check if there's any significant data
            {
                 _usageGraph.TimeData = graphData;
            }
            else
            {
                List<KeyValuePair<DateTime, double>> emptyTimeline = new List<KeyValuePair<DateTime, double>>();
                for (int i = 0; i < DisplayDays; i++)
                {
                    emptyTimeline.Add(new KeyValuePair<DateTime, double>(DateTime.Today.AddDays(-(DisplayDays -1) + i), 0));
                }
                _usageGraph.TimeData = emptyTimeline;
            }
            Console.WriteLine($"Graph updated: {graphTitle} with {graphData.Count} points. View: {_currentGraphView}");
        }

        /// <summary>
        /// Handles selection changes in the process list view
        /// </summary>
        private void ProcessListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_processListView.SelectedItems.Count > 0)
            {
                ListViewItem item = _processListView.SelectedItems[0];
                _selectedProcess = item.Tag as ProcessData;
                _selectedCategoryForGraph = null; // Clear category selection
                _currentGraphView = "PROCESS";
                UpdateGraphView();
                _statsPanel.UpdateStats(_selectedProcess); // Show process specific stats
            }
            else
            {
                _selectedProcess = null;
                 // If no category is actively selected for graph, revert to total PC on time
                if (string.IsNullOrEmpty(_selectedCategoryForGraph) || _selectedCategoryForGraph == "All")
                {
                    _currentGraphView = "TOTAL_PC_ON_TIME";
                }
                // Otherwise, keep the CATEGORY view (e.g. user cleared process selection but wants to see category graph)
                UpdateGraphView();
                _statsPanel.ClearStats();
                TimeSpan uptime = ProcessMonitor.GetSystemUptime(_lastBootTime);
                _statsPanel.UpdateSystemUptime(uptime); // Show system uptime
            }
        }

        /// <summary>
        /// Handles category selection changes from the CategorySelector panel
        /// </summary>
        private void CategorySelector_CategorySelected(object sender, string category)
        {
            UpdateProcessList(); // Filter the list view
            _selectedProcess = null; // Clear any selected process
            _processListView.SelectedItems.Clear(); // Visually clear selection in list

            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                _selectedCategoryForGraph = category;
                _currentGraphView = "CATEGORY";
                _statsPanel.UpdateCategoryStats(category, _dataService.GetTotalActiveTimeForCategory(category, 1).FirstOrDefault().Value * 60); // Show total time for category today
            }
            else
            {
                _selectedCategoryForGraph = null;
                _currentGraphView = "TOTAL_PC_ON_TIME";
                _statsPanel.ClearStats();
                 TimeSpan uptime = ProcessMonitor.GetSystemUptime(_lastBootTime);
                _statsPanel.UpdateSystemUptime(uptime);
            }
            UpdateGraphView();
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
