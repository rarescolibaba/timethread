// AUTORI: Bostan Sorina-Gabirela, Brinza Denis-Stefan, Colibaba Rares-Andrei, Dodita Alexandru-Tomi
// UNIVERSITATEA TEHNICA GHEORGHE ASACHI, GRUPA 1312A
// Functionalitate:
// Fereastra principala a aplicatiei Process Time Tracker.
// Gestioneaza interfata, interactiunea cu utilizatorul si actualizarea datelor despre procese si statistici.
// ---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO; 
using System.Reflection; 

namespace @interface
{
    /// <summary>
    /// Fereastra principala pentru aplicatia Process Time Tracker
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
        private const int DisplayDays = 30; 
        private DateTime _lastBootTime;
        private string _currentGraphView = "TOTAL_PC_ON_TIME"; 
        private string _selectedCategoryForGraph = null;
        private const string HelpFileName = "HelpTimeTracker.chm"; 

        /// <summary>
        /// Constructor pentru fereastra principala
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
                _updateTimer.Interval = 10000; 
                _updateTimer.Tick += UpdateTimer_Tick;
                _updateTimer.Start();

                LoadRealProcessData();
                UpdateGraphView(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing application: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Actualizeaza interfata cu datele curente despre procese si panoul de statistici
        /// </summary>
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            LoadRealProcessData();
            // Update stats panel with current uptime if no process is selected
            if (_selectedProcess == null && (_selectedCategoryForGraph == null || _selectedCategoryForGraph == "All"))
            {
                TimeSpan uptime = ProcessMonitor.GetSystemUptime(_lastBootTime);
                _statsPanel.UpdateSystemUptime(uptime);
            }
            // else the stats panel is updated by ProcessListView_SelectedIndexChanged or CategorySelector_CategorySelected
        }

        /// <summary>
        /// Initializeaza controalele personalizate
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

            _processListView.Columns.Add("Name", 200);
            _processListView.Columns.Add("PID", 70);
            _processListView.Columns.Add("Dept", 100);
            _processListView.Columns.Add("Today", 80, HorizontalAlignment.Right);
            _processListView.SelectedIndexChanged += ProcessListView_SelectedIndexChanged;

            // Create the category selector
            _categorySelector = new CategorySelector
            {
                Location = new Point(570, 10),
                Size = new Size(200, 280), 
                Categories = new List<string> { "Games", "Learning", "Coding", "Entertainment", "Other" }
            };
            _categorySelector.CategorySelected += CategorySelector_CategorySelected;
            _categorySelector.HelpButtonClicked += CategorySelector_HelpButtonClicked; 
            // Create the stats panel
            _statsPanel = new StatsPanel
            {
                Location = new Point(570, 300), 
                Size = new Size(200, 250) 
            };

            Controls.Add(_usageGraph);
            Controls.Add(_processListView);
            Controls.Add(_categorySelector);
            Controls.Add(_statsPanel);
        }

        /// <summary>
        /// Incarca datele reale despre procese din sistem
        /// </summary>
        private void LoadRealProcessData()
        {
            if (_processMonitor != null) 
            {
                _processData = _processMonitor.GetProcessData();
                UpdateProcessList();
            }
        }

        /// <summary>
        /// Actualizeaza lista de procese in functie de categoria selectata
        /// </summary>
        private void UpdateProcessList()
        {
            if (_processListView == null || _processData == null || _categorySelector == null) return; 

            _processListView.Items.Clear();
            List<ProcessData> filteredProcesses = _processData;

            if (_categorySelector.SelectedCategory != "All")
            {
                filteredProcesses = _processData.Where(p => p.Department == _categorySelector.SelectedCategory).ToList();
            }

            foreach (ProcessData process in filteredProcesses)
            {
                ListViewItem item = new ListViewItem(process.Name);
                item.SubItems.Add(process.PID.ToString());
                item.SubItems.Add(process.Department);
                item.SubItems.Add(FormatTimeSpan(process.TimeToday));
                item.Tag = process;
                _processListView.Items.Add(item);
            }

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

        private string FormatTimeSpan(TimeSpan time)
        {
            return $"{time.Hours}h {time.Minutes}m";
        }

        private void UpdateGraphView()
        {
            if (_usageGraph == null || _dataService == null) return; 

            List<KeyValuePair<DateTime, double>> graphData = new List<KeyValuePair<DateTime, double>>();
            string graphTitle = "PC Usage Data";

            switch (_currentGraphView)
            {
                case "PROCESS":
                    if (_selectedProcess != null)
                    {
                        graphTitle = $"{_selectedProcess.Name} Usage (Active+Idle Time)"; 
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
                    graphData = _dataService.GetPersistedDailySystemOnTime(DisplayDays);
                    _currentGraphView = "TOTAL_PC_ON_TIME";
                    break;
            }

            _usageGraph.Title = graphTitle;
            if (graphData.Any() && graphData.Any(d => d.Value > 0.001))
            {
                _usageGraph.TimeData = graphData;
            }
            else
            {
                List<KeyValuePair<DateTime, double>> emptyTimeline = new List<KeyValuePair<DateTime, double>>();
                for (int i = 0; i < DisplayDays; i++)
                {
                    emptyTimeline.Add(new KeyValuePair<DateTime, double>(DateTime.Today.AddDays(-(DisplayDays - 1) + i), 0));
                }
                _usageGraph.TimeData = emptyTimeline;
            }
            Console.WriteLine($"Graph updated: {graphTitle} with {graphData.Count} points. View: {_currentGraphView}");
        }

        /// <summary>
        /// Eveniment la selectarea unui proces din lista
        /// </summary>
        private void ProcessListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_processListView.SelectedItems.Count > 0)
            {
                ListViewItem item = _processListView.SelectedItems[0];
                _selectedProcess = item.Tag as ProcessData;
                _selectedCategoryForGraph = null;
                _currentGraphView = "PROCESS";
                UpdateGraphView();
                if (_statsPanel != null && _selectedProcess != null)
                {
                    _statsPanel.UpdateStats(_selectedProcess);
                }
            }
            else
            {
                _selectedProcess = null;
                if (string.IsNullOrEmpty(_selectedCategoryForGraph) || _selectedCategoryForGraph == "All")
                {
                    _currentGraphView = "TOTAL_PC_ON_TIME";
                }
                UpdateGraphView();
                if (_statsPanel != null) 
                {
                    _statsPanel.ClearStats();
                    TimeSpan uptime = ProcessMonitor.GetSystemUptime(_lastBootTime);
                    _statsPanel.UpdateSystemUptime(uptime);
                }
            }
        }

        /// <summary>
        /// Eveniment la selectarea unei categorii din selector
        /// </summary>
        private void CategorySelector_CategorySelected(object sender, string category)
        {
            UpdateProcessList();
            _selectedProcess = null;
            if (_processListView != null) _processListView.SelectedItems.Clear();

            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                _selectedCategoryForGraph = category;
                _currentGraphView = "CATEGORY";
                if (_statsPanel != null && _dataService != null) 
                {
                    var categoryDataToday = _dataService.GetTotalActiveTimeForCategory(category, 1);
                    double totalMinutesToday = categoryDataToday.Any() ? categoryDataToday.First().Value * 60 : 0;
                    _statsPanel.UpdateCategoryStats(category, totalMinutesToday);
                }
            }
            else
            {
                _selectedCategoryForGraph = null;
                _currentGraphView = "TOTAL_PC_ON_TIME";
                if (_statsPanel != null) 
                {
                    _statsPanel.ClearStats();
                    TimeSpan uptime = ProcessMonitor.GetSystemUptime(_lastBootTime);
                    _statsPanel.UpdateSystemUptime(uptime);
                }
            }
            UpdateGraphView();
        }

        /// <summary>
        /// Eveniment la apasarea butonului de help
        /// </summary>
        private void CategorySelector_HelpButtonClicked(object sender, EventArgs e)
        {
            try
            {
                string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string helpFilePath = Path.Combine(basePath, "Help", HelpFileName);

                if (File.Exists(helpFilePath))
                {
                    Help.ShowHelp(this, helpFilePath);
                }
                else
                {
                    MessageBox.Show($"Fișierul de ajutor '{HelpFileName}' nu a fost găsit în subfolderul 'Help'.\nCalea verificată: {helpFilePath}",
                                    "Eroare Ajutor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"A apărut o eroare la deschiderea fișierului de ajutor:\n{ex.Message}",
                                "Eroare Ajutor", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Elibereaza resursele la inchiderea ferestrei
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (_updateTimer != null) _updateTimer.Stop(); 
            if (_processMonitor != null) _processMonitor.StopMonitoring(); 
        }
    }
}