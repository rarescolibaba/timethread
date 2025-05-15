using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace @interface
{
    public partial class Form1 : Form
    {
        private ProcessManager processManager;
        private ProcessInfo selectedProcess;
        private Timer refreshTimer;
        
        // Custom colors
        private readonly Color appBackgroundColor = Color.FromArgb(240, 240, 245);
        private readonly Color panelBackgroundColor = Color.White;
        private readonly Color chartLineColor = Color.FromArgb(77, 166, 255);
        private readonly Color chartAreaColor = Color.FromArgb(249, 249, 255);
        private readonly Color headerColor = Color.FromArgb(51, 51, 76);
        private readonly Color accentColor = Color.FromArgb(77, 166, 255);
        private readonly Color textColor = Color.FromArgb(60, 60, 60);

        public Form1()
        {
            InitializeComponent();
            
            // Apply custom styling
            ApplyCustomStyling();
            
            // Initialize the process manager
            processManager = new ProcessManager();
            
            // Configure the DataGridView
            SetupDataGridView();
            
            // Configure the chart
            SetupChart();
            
            // Set up the refresh timer
            refreshTimer = new Timer();
            refreshTimer.Interval = 5000; // Refresh every 5 seconds
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
            
            // Set up event handlers
            processDataGridView.SelectionChanged += ProcessDataGridView_SelectionChanged;
            allRadioButton.CheckedChanged += CategoryRadioButton_CheckedChanged;
            gamesRadioButton.CheckedChanged += CategoryRadioButton_CheckedChanged;
            learningRadioButton.CheckedChanged += CategoryRadioButton_CheckedChanged;
            codingRadioButton.CheckedChanged += CategoryRadioButton_CheckedChanged;
            entertainmentRadioButton.CheckedChanged += CategoryRadioButton_CheckedChanged;
            
            // Initial UI update
            allRadioButton.Checked = true;
            UpdateUI();
        }

        private void ApplyCustomStyling()
        {
            // Form styling
            this.BackColor = appBackgroundColor;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.Text = "TimeThread - Process Tracker";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Icon = SystemIcons.Application;
            
            // Chart panel styling
            Panel chartPanel = new Panel();
            chartPanel.Location = new Point(20, 20);
            chartPanel.Size = new Size(410, 210);
            chartPanel.BackColor = panelBackgroundColor;
            chartPanel.BorderStyle = BorderStyle.None;
            chartPanel.Padding = new Padding(10);
            ApplyShadowEffect(chartPanel);
            
            // Adjust chart position inside the panel
            usageChart.Parent = chartPanel;
            usageChart.Location = new Point(10, 10);
            usageChart.Size = new Size(390, 190);
            usageChart.BackColor = panelBackgroundColor;
            usageChart.BorderlineColor = Color.Transparent;
            
            // DataGridView panel styling
            Panel dataGridPanel = new Panel();
            dataGridPanel.Location = new Point(20, 240);
            dataGridPanel.Size = new Size(410, 190);
            dataGridPanel.BackColor = panelBackgroundColor;
            dataGridPanel.BorderStyle = BorderStyle.None;
            dataGridPanel.Padding = new Padding(10);
            ApplyShadowEffect(dataGridPanel);
            
            // Adjust DataGridView position inside the panel
            processDataGridView.Parent = dataGridPanel;
            processDataGridView.Location = new Point(10, 10);
            processDataGridView.Size = new Size(390, 170);
            processDataGridView.BorderStyle = BorderStyle.None;
            processDataGridView.BackgroundColor = panelBackgroundColor;
            processDataGridView.GridColor = Color.FromArgb(230, 230, 230);
            processDataGridView.RowHeadersVisible = false;
            processDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            processDataGridView.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            processDataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            processDataGridView.EnableHeadersVisualStyles = false;
            processDataGridView.ColumnHeadersDefaultCellStyle.BackColor = headerColor;
            processDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            processDataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9F);
            processDataGridView.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            processDataGridView.ColumnHeadersHeight = 35;
            processDataGridView.RowTemplate.Height = 30;
            processDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(246, 246, 246);
            processDataGridView.DefaultCellStyle.SelectionBackColor = accentColor;
            processDataGridView.DefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            
            // Category panel styling
            categoryGroupBox.BackColor = panelBackgroundColor;
            categoryGroupBox.ForeColor = textColor;
            categoryGroupBox.Font = new Font("Segoe UI Semibold", 9F);
            categoryGroupBox.Location = new Point(450, 20);
            categoryGroupBox.Size = new Size(320, 135);
            categoryGroupBox.Padding = new Padding(15);
            ApplyShadowEffect(categoryGroupBox);
            
            // Style radio buttons
            StyleRadioButton(allRadioButton, "All Categories");
            StyleRadioButton(gamesRadioButton, "Games");
            StyleRadioButton(learningRadioButton, "Learning");
            StyleRadioButton(codingRadioButton, "Coding");
            StyleRadioButton(entertainmentRadioButton, "Entertainment");
            
            // Stats panel styling
            statsPanel.BackColor = panelBackgroundColor;
            statsPanel.Location = new Point(450, 165);
            statsPanel.Size = new Size(320, 265);
            statsPanel.Padding = new Padding(15);
            ApplyShadowEffect(statsPanel);
            
            // Add header to stats panel
            Label statsHeader = new Label();
            statsHeader.Text = "Process Statistics";
            statsHeader.Font = new Font("Segoe UI Semibold", 11F);
            statsHeader.ForeColor = headerColor;
            statsHeader.Location = new Point(15, 15);
            statsHeader.AutoSize = true;
            statsPanel.Controls.Add(statsHeader);
            
            // Style stats labels
            StyleStatsLabel(allTimeLabel, "All time", 60);
            StyleStatsLabel(totalTodayLabel, "Total today", 95);
            StyleStatsLabel(idleLabel, "Idle time", 130);
            StyleStatsLabel(activeLabel, "Active time", 165);
            
            // Add controls to form
            this.Controls.Add(chartPanel);
            this.Controls.Add(dataGridPanel);
        }

        private void ApplyShadowEffect(Control control)
        {
            // This simulates a shadow by adding a darkened panel behind the control
            Panel shadowPanel = new Panel();
            shadowPanel.BackColor = Color.FromArgb(220, 220, 220);
            shadowPanel.Location = new Point(control.Location.X + 3, control.Location.Y + 3);
            shadowPanel.Size = control.Size;
            shadowPanel.BorderStyle = BorderStyle.None;
            this.Controls.Add(shadowPanel);
            this.Controls.SetChildIndex(shadowPanel, this.Controls.GetChildIndex(control) + 1);
            
            // Make sure control is on top
            control.BringToFront();
        }

        private void StyleRadioButton(RadioButton radioButton, string text)
        {
            radioButton.Text = text;
            radioButton.Font = new Font("Segoe UI", 9F);
            radioButton.ForeColor = textColor;
            radioButton.BackColor = panelBackgroundColor;
            radioButton.Padding = new Padding(5);
        }

        private void StyleStatsLabel(Label label, string prefix, int yPosition)
        {
            label.AutoSize = false;
            label.Width = 290;
            label.Height = 25;
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Location = new Point(15, yPosition);
            label.Font = new Font("Segoe UI", 10F);
            label.ForeColor = textColor;
            label.Text = $"{prefix}: 0";
        }

        private void SetupDataGridView()
        {
            // Configure columns
            processDataGridView.AutoGenerateColumns = false;
            processDataGridView.ColumnCount = 4;
            
            processDataGridView.Columns[0].Name = "Name";
            processDataGridView.Columns[0].HeaderText = "Application";
            processDataGridView.Columns[0].DataPropertyName = "Name";
            processDataGridView.Columns[0].Width = 140;
            
            processDataGridView.Columns[1].Name = "PID";
            processDataGridView.Columns[1].HeaderText = "PID";
            processDataGridView.Columns[1].DataPropertyName = "PID";
            processDataGridView.Columns[1].Width = 60;
            
            processDataGridView.Columns[2].Name = "Category";
            processDataGridView.Columns[2].HeaderText = "Category";
            processDataGridView.Columns[2].DataPropertyName = "Category";
            processDataGridView.Columns[2].Width = 90;
            
            processDataGridView.Columns[3].Name = "TimeToday";
            processDataGridView.Columns[3].HeaderText = "Today";
            processDataGridView.Columns[3].DataPropertyName = "TimeToday";
            processDataGridView.Columns[3].Width = 80;
            processDataGridView.Columns[3].DefaultCellStyle.Format = "0 min";
            
            // Allow sorting
            processDataGridView.Sort(processDataGridView.Columns["TimeToday"], ListSortDirection.Descending);
        }

        private void SetupChart()
        {
            usageChart.Series.Clear();
            usageChart.ChartAreas.Clear();
            usageChart.Titles.Clear();
            usageChart.Legends.Clear();
            
            // Create chart area with styling
            ChartArea chartArea = new ChartArea("UsageArea");
            chartArea.BackColor = chartAreaColor;
            
            // Configure axes
            chartArea.AxisX.Title = "Date";
            chartArea.AxisY.Title = "Hours";
            chartArea.AxisX.TitleFont = new Font("Segoe UI", 8F);
            chartArea.AxisY.TitleFont = new Font("Segoe UI", 8F);
            chartArea.AxisX.LabelStyle.Font = new Font("Segoe UI", 7F);
            chartArea.AxisY.LabelStyle.Font = new Font("Segoe UI", 7F);
            chartArea.AxisX.LabelStyle.Format = "d";
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(230, 230, 230);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(230, 230, 230);
            chartArea.AxisX.LineColor = Color.FromArgb(200, 200, 200);
            chartArea.AxisY.LineColor = Color.FromArgb(200, 200, 200);
            
            // Add the area to chart
            usageChart.ChartAreas.Add(chartArea);
            
            // Create series with styling
            Series series = new Series("Usage");
            series.ChartType = SeriesChartType.Line;
            series.ChartArea = "UsageArea";
            series.Color = chartLineColor;
            series.BorderWidth = 3;
            series.MarkerStyle = MarkerStyle.Circle;
            series.MarkerSize = 8;
            series.MarkerColor = chartLineColor;
            
            // Add the series to chart
            usageChart.Series.Add(series);
            
            // Set chart title with styling
            Title chartTitle = new Title("Application Usage (Last 30 Days)");
            chartTitle.Font = new Font("Segoe UI Semibold", 10F);
            chartTitle.ForeColor = headerColor;
            chartTitle.Alignment = ContentAlignment.TopCenter;
            usageChart.Titles.Add(chartTitle);
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void ProcessDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (processDataGridView.SelectedRows.Count > 0)
            {
                int index = processDataGridView.SelectedRows[0].Index;
                if (index >= 0 && index < processManager.Processes.Count)
                {
                    selectedProcess = processManager.Processes[index];
                    UpdateProcessDetails();
                }
            }
        }

        private void CategoryRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            string category = GetSelectedCategory();
            processManager.GetProcesses();
            var filteredProcesses = processManager.FilterByCategory(category);
            filteredProcesses = processManager.SortByTime(filteredProcesses, "TimeToday");
            
            processDataGridView.DataSource = null;
            processDataGridView.DataSource = filteredProcesses;
            
            if (selectedProcess != null)
            {
                // Check if selected process still exists in the filtered list
                var existingProcess = filteredProcesses.FirstOrDefault(p => p.PID == selectedProcess.PID);
                if (existingProcess != null)
                {
                    selectedProcess = existingProcess;
                    UpdateProcessDetails();
                }
                else
                {
                    selectedProcess = null;
                    ClearProcessDetails();
                }
            }
        }

        private string GetSelectedCategory()
        {
            if (gamesRadioButton.Checked) return "Games";
            if (learningRadioButton.Checked) return "Learning";
            if (codingRadioButton.Checked) return "Coding";
            if (entertainmentRadioButton.Checked) return "Entertainment";
            return "All"; // Default if all is selected
        }

        private void UpdateProcessDetails()
        {
            if (selectedProcess == null) return;
            
            // Update chart with process history
            UpdateChart();
            
            // Update chart title with process name
            if (usageChart.Titles.Count > 0)
            {
                usageChart.Titles[0].Text = $"{selectedProcess.Name} - Usage (Last 30 Days)";
            }
            
            // Update stats panel
            allTimeLabel.Text = $"All time: {selectedProcess.TotalTimeSpent:0.0} hours";
            totalTodayLabel.Text = $"Total today: {selectedProcess.TimeToday} minutes";
            idleLabel.Text = $"Idle: {selectedProcess.IdleTime} minutes";
            activeLabel.Text = $"Active: {selectedProcess.ActiveTime} minutes";
        }

        private void ClearProcessDetails()
        {
            // Clear chart
            foreach (var series in usageChart.Series)
            {
                series.Points.Clear();
            }
            
            // Reset chart title
            if (usageChart.Titles.Count > 0)
            {
                usageChart.Titles[0].Text = "Application Usage (Last 30 Days)";
            }
            
            // Clear stats
            allTimeLabel.Text = "All time: 0 hours";
            totalTodayLabel.Text = "Total today: 0 minutes";
            idleLabel.Text = "Idle: 0 minutes";
            activeLabel.Text = "Active: 0 minutes";
        }

        private void UpdateChart()
        {
            if (selectedProcess == null) return;
            
            var series = usageChart.Series[0];
            series.Points.Clear();
            
            // Add data points from process history
            for (int i = 0; i < selectedProcess.History.Length; i++)
            {
                series.Points.AddXY(DateTime.Now.AddDays(-29 + i).ToShortDateString(), selectedProcess.History[i]);
            }
            
            // Set Y-axis range to better fit the data
            double maxValue = selectedProcess.History.Max() * 1.2; // Add 20% padding
            usageChart.ChartAreas[0].AxisY.Maximum = Math.Max(1, maxValue); // At least 1 hour for visibility
        }
    }

    public class ProcessManager
    {
        public List<ProcessInfo> Processes { get; private set; }
        
        public ProcessManager()
        {
            Processes = new List<ProcessInfo>();
        }
        
        public void GetProcesses()
        {
            // In a real implementation, this would retrieve actual running processes
            // For this prototype, use dummy data if the list is empty
            if (Processes.Count == 0)
            {
                GenerateDummyData();
            }
            
            // Update process info with current stats
            foreach (var process in Processes)
            {
                process.UpdateUsageData();
            }
        }
        
        public List<ProcessInfo> FilterByCategory(string category)
        {
            if (category == "All")
            {
                return Processes.ToList();
            }
            
            return Processes.Where(p => p.Category == category).ToList();
        }
        
        public List<ProcessInfo> SortByTime(List<ProcessInfo> processes, string sortColumn)
        {
            switch (sortColumn)
            {
                case "Name":
                    return processes.OrderBy(p => p.Name).ToList();
                case "PID":
                    return processes.OrderBy(p => p.PID).ToList();
                case "Category":
                    return processes.OrderBy(p => p.Category).ToList();
                case "TimeToday":
                    return processes.OrderByDescending(p => p.TimeToday).ToList();
                default:
                    return processes;
            }
        }
        
        private void GenerateDummyData()
        {
            // Create sample processes for testing
            Processes.Add(new ProcessInfo("Chrome", 1234, "Entertainment"));
            Processes.Add(new ProcessInfo("Visual Studio", 2345, "Coding"));
            Processes.Add(new ProcessInfo("Steam", 3456, "Games"));
            Processes.Add(new ProcessInfo("Spotify", 4567, "Entertainment"));
            Processes.Add(new ProcessInfo("Word", 5678, "Learning"));
            Processes.Add(new ProcessInfo("Excel", 6789, "Learning"));
            Processes.Add(new ProcessInfo("Photoshop", 7890, "Entertainment"));
            Processes.Add(new ProcessInfo("Unity", 8901, "Coding"));
            Processes.Add(new ProcessInfo("Fortnite", 9012, "Games"));
            Processes.Add(new ProcessInfo("Discord", 1122, "Entertainment"));
        }
    }

    public class ProcessInfo
    {
        public string Name { get; set; }
        public int PID { get; set; }
        public string Category { get; set; }
        public double TotalTimeSpent { get; set; } // In hours
        public int TimeToday { get; set; } // In minutes
        public int IdleTime { get; set; } // In minutes
        public int ActiveTime { get; set; } // In minutes
        public double[] History { get; set; } // 30 days of history in hours
        
        private Random random;
        
        public ProcessInfo(string name, int pid, string category)
        {
            Name = name;
            PID = pid;
            Category = category;
            random = new Random(pid); // Use PID as seed for consistency
            
            // Initialize with random data for demonstration
            TotalTimeSpent = Math.Round(random.NextDouble() * 100, 1); // Up to 100 hours total
            TimeToday = random.Next(0, 120); // Up to 2 hours today
            IdleTime = random.Next(0, TimeToday);
            ActiveTime = TimeToday - IdleTime;
            
            // Generate 30 days of random history
            History = new double[30];
            for (int i = 0; i < 30; i++)
            {
                History[i] = Math.Round(random.NextDouble() * 4, 1); // Up to 4 hours per day
            }
        }
        
        public void UpdateUsageData()
        {
            // Simulate usage change (in a real app this would track actual usage)
            TimeToday += random.Next(-5, 10); // Randomly adjust time (+/- minutes)
            if (TimeToday < 0) TimeToday = 0;
            
            IdleTime = random.Next(0, TimeToday);
            ActiveTime = TimeToday - IdleTime;
            
            // Update total time
            TotalTimeSpent = Math.Round(TotalTimeSpent + (TimeToday - History[29]) / 60.0, 1);
            
            // Shift history and add today's usage
            for (int i = 0; i < 29; i++)
            {
                History[i] = History[i + 1];
            }
            History[29] = Math.Round(TimeToday / 60.0, 1); // Convert minutes to hours
        }
    }
}
