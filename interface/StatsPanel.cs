// AUTORI: Bostan Sorina-Gabirela, Brinza Denis-Stefan, Colibaba Rares-Andrei, Dodita Alexandru-Tomi
// UNIVERSITATEA TEHNICA GHEORGHE ASACHI, GRUPA 1312A
// Functionalitate:
//Control pentru afisarea statisticilor de utilizare pentru procese, categorii si sistem in aplicatia Process Time Tracker.
// ---------------------------------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Windows.Forms;
using ProcessDataLib;

namespace @interface
{
    /// <summary>
    /// Control pentru afisarea statisticilor de utilizare
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
        private Label _systemUptimeLabel;
        private Label _systemUptimeValueLabel;
        private Label _categoryTimeLabel;
        private Label _categoryTimeValueLabel;

        /// <summary>
        /// Constructor pentru StatsPanel
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
                Text = "Statistics",
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
            
            // System Uptime Label
            _systemUptimeLabel = new Label
            {
                Text = "System Uptime",
                AutoSize = true,
                Location = new Point(10, 130)
            };

            _systemUptimeValueLabel = new Label
            {
                Text = "0d 0h 0m",
                Font = new Font("Arial", 9, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(100, 130),
                TextAlign = ContentAlignment.MiddleRight
            };
            
            // Category Time Label (initially hidden or generic)
            _categoryTimeLabel = new Label
            {
                Text = "Category Time",
                AutoSize = true,
                Location = new Point(10, 160),
                Visible = false
            };

            _categoryTimeValueLabel = new Label
            {
                Text = "0h 0m",
                Font = new Font("Arial", 9, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(100, 160),
                TextAlign = ContentAlignment.MiddleRight,
                Visible = false
            };
            
            // Add labels to panel
            Controls.Add(_titleLabel);
            Controls.Add(_totalTimeLabel);
            Controls.Add(_idleTimeLabel);
            Controls.Add(_activeTimeLabel);
            Controls.Add(_totalTimeValueLabel);
            Controls.Add(_idleTimeValueLabel);
            Controls.Add(_activeTimeValueLabel);
            Controls.Add(_systemUptimeLabel);
            Controls.Add(_systemUptimeValueLabel);
            Controls.Add(_categoryTimeLabel);
            Controls.Add(_categoryTimeValueLabel);
        }

        /// <summary>
        /// Actualizeaza statisticile cu datele procesului
        /// </summary>
        /// <param name="processData">Process data to display</param>
        public void UpdateStats(ProcessData processData)
        {
            if (processData == null)
            {
                ClearStats();
                return;
            }
            
            _titleLabel.Text = processData.Name;
            
            TimeSpan totalTime = processData.GetTotalTime();
            TimeSpan idleTime = processData.GetIdleTime();
            TimeSpan activeTime = processData.GetActiveTime();
            
            _totalTimeValueLabel.Text = FormatTimeSpan(totalTime);
            _idleTimeValueLabel.Text = FormatTimeSpan(idleTime);
            _activeTimeValueLabel.Text = FormatTimeSpan(activeTime);

            SetLabelVisibility(isProcessView: true);
        }

        /// <summary>
        /// Actualizeaza panoul pentru a arata timpul activ total pentru o categorie astazi
        /// </summary>
        /// <param name="categoryName">The name of the category.</param>
        /// <param name="totalActiveMinutesToday">Total active minutes for the category today.</param>
        public void UpdateCategoryStats(string categoryName, double totalActiveMinutesToday)
        {
            _titleLabel.Text = $"{categoryName} Stats";
            _categoryTimeLabel.Text = "Today's Active";
            _categoryTimeValueLabel.Text = FormatTimeSpan(TimeSpan.FromMinutes(totalActiveMinutesToday));
            
            SetLabelVisibility(isCategoryView: true);
        }

        /// <summary>
        /// Actualizeaza afisarea timpului de functionare al sistemului
        /// </summary>
        /// <param name="uptime">Current system uptime.</param>
        public void UpdateSystemUptime(TimeSpan uptime)
        {
            if (_titleLabel.Text == "Statistics" || string.IsNullOrEmpty(_titleLabel.Text))
            {
                 _titleLabel.Text = "System Stats";
            }
            _systemUptimeValueLabel.Text = $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m";
            SetLabelVisibility(isSystemView: true); 
        }

        /// <summary>
        /// Reseteaza toate statisticile si titlul
        /// </summary>
        public void ClearStats()
        {
            _titleLabel.Text = "Statistics";
            _totalTimeValueLabel.Text = "0h 0m";
            _activeTimeValueLabel.Text = "0h 0m";
            _idleTimeValueLabel.Text = "0h 0m";
            _systemUptimeValueLabel.Text = "0d 0h 0m";
            _categoryTimeValueLabel.Text = "0h 0m";
            SetLabelVisibility();
        }

        /// <summary>
        /// Metoda ajutatoare pentru gestionarea vizibilitatii etichetelor
        /// </summary>
        private void SetLabelVisibility(bool isProcessView = false, bool isCategoryView = false, bool isSystemView = false)
        {
            _totalTimeLabel.Visible = isProcessView;
            _totalTimeValueLabel.Visible = isProcessView;
            _activeTimeLabel.Visible = isProcessView;
            _activeTimeValueLabel.Visible = isProcessView;
            _idleTimeLabel.Visible = isProcessView;
            _idleTimeValueLabel.Visible = isProcessView;

            _categoryTimeLabel.Visible = isCategoryView;
            _categoryTimeValueLabel.Visible = isCategoryView;

            _systemUptimeLabel.Visible = isSystemView || (!isProcessView && !isCategoryView);
            _systemUptimeValueLabel.Visible = isSystemView || (!isProcessView && !isCategoryView);
            
            if (!isProcessView && !isCategoryView && !isSystemView) {
                 _systemUptimeLabel.Visible = true;
                 _systemUptimeValueLabel.Visible = true;
            }
        }

        /// <summary>
        /// Formateaza un TimeSpan ca ore si minute
        /// </summary>
        /// <param name="time">TimeSpan to format</param>
        /// <returns>Formatted time string</returns>
        private string FormatTimeSpan(TimeSpan time)
        {
            return $"{time.Hours}h {time.Minutes}m";
        }
    }
} 