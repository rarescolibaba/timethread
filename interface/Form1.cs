﻿// AUTORI: Bostan Sorina-Gabriela, Brinza Denis-Stefan, Colibaba Rares-Andrei, Dodita Alexandru-Tomi
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
using ProcessDataLib;
using ProcessMonitorLib;
using UsageDataServiceLib;
using @interface; // for Utils
using Common;

namespace @interface
{
    /// <summary>
    /// Fereastra principala pentru aplicatia Process Time Tracker
    /// </summary>
    public partial class Form1 : Form, IProcessObserver
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
                _processMonitor.Subscribe(this);

                _updateTimer = new Timer();
                _updateTimer.Interval = 10000; 
                _updateTimer.Tick += UpdateTimer_Tick;
                _updateTimer.Start();

                LoadRealProcessData();
                UpdateGraphView(); 
            }
            catch (Exception ex)
            {
                Logger.Log(new UIException("Error initializing application.", ex), "Form1.ctor");
                MessageBox.Show($"Error initializing application: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Actualizeaza interfata cu datele curente despre procese si panoul de statistici
        /// </summary>
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Logger.Log(new UIException("Error in UpdateTimer_Tick.", ex), "UpdateTimer_Tick");
            }
        }

        /// <summary>
        /// Initializeaza controalele personalizate
        /// </summary>
        private void InitializeControls()
        {
            try
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
            catch (Exception ex)
            {
                Logger.Log(new UIException("Error initializing controls.", ex), "InitializeControls");
                MessageBox.Show($"Error initializing controls: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Incarca datele reale despre procese din sistem
        /// </summary>
        private void LoadRealProcessData()
        {
            try
            {
                if (_processMonitor != null) 
                {
                    _processData = _processMonitor.ProcessData;
                    UpdateProcessList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(new UIException("Error loading real process data.", ex), "LoadRealProcessData");
            }
        }

        /// <summary>
        /// Actualizeaza lista de procese in functie de categoria selectata
        /// </summary>
        private void UpdateProcessList()
        {
            try
            {
                if (_processListView == null || _processData == null || _categorySelector == null) return; 

                _processListView.BeginUpdate();
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
                    item.SubItems.Add(Utils.FormatTimeSpan(process.TimeToday));
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
                _processListView.EndUpdate();
            }
            catch (Exception ex)
            {
                Logger.Log(new UIException("Error updating process list.", ex), "UpdateProcessList");
            }
        }

        private string FormatTimeSpan(TimeSpan time)
        {
            return Utils.FormatTimeSpan(time);
        }

        private void UpdateGraphView()
        {
            try
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
            catch (Exception ex)
            {
                Logger.Log(new UIException("Error updating graph view.", ex), "UpdateGraphView");
            }
        }

        /// <summary>
        /// Eveniment la selectarea unui proces din lista
        /// </summary>
        private void ProcessListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Logger.Log(new UIException("Error in ProcessListView_SelectedIndexChanged.", ex), "ProcessListView_SelectedIndexChanged");
            }
        }

        /// <summary>
        /// Eveniment la selectarea unei categorii din selector
        /// </summary>
        private void CategorySelector_CategorySelected(object sender, string category)
        {
            try
            {
                // Reaplica filtrul
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
            catch (Exception ex)
            {
                Logger.Log(new UIException("Error in CategorySelector_CategorySelected.", ex), "CategorySelector_CategorySelected");
            }
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
                Logger.Log(new UIException("Error opening help.", ex), "CategorySelector_HelpButtonClicked");
                MessageBox.Show($"A apărut o eroare la deschiderea fișierului de ajutor:\n{ex.Message}",
                                "Eroare Ajutor", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Elibereaza resursele la inchiderea ferestrei
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                base.OnFormClosing(e);
                if (_updateTimer != null) _updateTimer.Stop(); 
                if (_processMonitor != null) _processMonitor.StopMonitoring(); 
            }
            catch (Exception ex)
            {
                Logger.Log(new UIException("Error during form closing.", ex), "OnFormClosing");
            }
        }

        /// <summary>
        /// Elibereaza resursele la inchiderea aplicatiei
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (_processMonitor != null)
                    {
                        _processMonitor.Unsubscribe(this);
                    }
                    if (components != null)
                    {
                        components.Dispose();
                    }
                    // Eliberare resurse suplimentare, daca este cazul
                }
                base.Dispose(disposing);
            }
            catch (Exception ex)
            {
                Logger.Log(new UIException("Error during Dispose.", ex), "Dispose");
            }
        }

        /// <summary>
        /// Notificare la adaugarea unui proces
        /// </summary>
        public void OnProcessAdded(ProcessData process)
        {
            try
            {
                BeginInvoke((Action)(() =>
                {
                    // Only add if it matches the current filter
                    if (IsProcessVisibleInCurrentCategory(process))
                        AddOrUpdateProcessInList(process);
                }));
            }
            catch (Exception ex)
            {
                Logger.Log(new UIException("Error in OnProcessAdded.", ex), "OnProcessAdded");
            }
        }

        /// <summary>
        /// Notificare la eliminarea unui proces
        /// </summary>
        public void OnProcessRemoved(int pid)
        {
            try
            {
                BeginInvoke((Action)(() =>
                {
                    RemoveProcessFromList(pid);
                }));
            }
            catch (Exception ex)
            {
                Logger.Log(new UIException("Error in OnProcessRemoved.", ex), "OnProcessRemoved");
            }
        }

        /// <summary>
        /// Notificare la actualizarea unui proces
        /// </summary>
        public void OnProcessUpdated(ProcessData process)
        {
            try
            {
                BeginInvoke((Action)(() =>
                {
                    // Only update if it matches the current filter
                    if (IsProcessVisibleInCurrentCategory(process))
                        AddOrUpdateProcessInList(process);
                    else
                        RemoveProcessFromList(process.PID);
                }));
            }
            catch (Exception ex)
            {
                Logger.Log(new UIException("Error in OnProcessUpdated.", ex), "OnProcessUpdated");
            }
        }

        /// <summary>
        /// Metoda ajutatoare: determina daca ar fi vizibil procesul in categoria selectata
        /// </summary>
        private bool IsProcessVisibleInCurrentCategory(ProcessData process)
        {
            if (_categorySelector == null) return true;
            var selected = _categorySelector.SelectedCategory;
            return selected == "All" || process.Department == selected;
        }

        private void AddOrUpdateProcessInList(ProcessData process)
        {
            try
            {
                if (_processListView == null) return;
                // Incercam gasirea in lista
                ListViewItem found = null;
                foreach (ListViewItem item in _processListView.Items)
                {
                    if (item.Tag is ProcessData pd && pd.PID == process.PID)
                    {
                        found = item;
                        break;
                    }
                }
                if (found != null)
                {
                    // Update
                    found.Text = process.Name;
                    found.SubItems[1].Text = process.PID.ToString();
                    found.SubItems[2].Text = process.Department;
                    found.SubItems[3].Text = Utils.FormatTimeSpan(process.TimeToday);
                    found.Tag = process;
                }
                else
                {
                    // Add only if it matches the current filter
                    if (IsProcessVisibleInCurrentCategory(process))
                    {
                        ListViewItem newItem = new ListViewItem(process.Name);
                        newItem.SubItems.Add(process.PID.ToString());
                        newItem.SubItems.Add(process.Department);
                        newItem.SubItems.Add(Utils.FormatTimeSpan(process.TimeToday));
                        newItem.Tag = process;
                        _processListView.Items.Add(newItem);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(new UIException("Error in AddOrUpdateProcessInList.", ex), "AddOrUpdateProcessInList");
            }
        }

        private void RemoveProcessFromList(int pid)
        {
            try
            {
                if (_processListView == null) return;
                foreach (ListViewItem item in _processListView.Items)
                {
                    if (item.Tag is ProcessData pd && pd.PID == pid)
                    {
                        _processListView.Items.Remove(item);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(new UIException("Error in RemoveProcessFromList.", ex), "RemoveProcessFromList");
            }
        }
    }
}