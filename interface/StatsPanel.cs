// AUTORI: Bostan Sorina-Gabriela, Brinza Denis-Stefan, Colibaba Rares-Andrei, Dodita Alexandru-Tomi
// UNIVERSITATEA TEHNICA GHEORGHE ASACHI, GRUPA 1312A
// Functionalitate:
// Control pentru afisarea statisticilor de utilizare pentru procese, categorii si sistem in aplicatia Process Time Tracker.
// ---------------------------------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Windows.Forms;
using ProcessDataLib;
using @interface; // for Utils
using Common; // for Logger and custom exceptions

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
        /// Initializeaza etichetele pentru panoul de statistici
        /// </summary>
        private void InitializeLabels()
        {
            // Eticheta titlu
            _titleLabel = new Label
            {
                Text = "Statistics",
                Font = new Font("Arial", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 10)
            };

            // Eticheta timpul total
            _totalTimeLabel = new Label
            {
                Text = "Total today",
                AutoSize = true,
                Location = new Point(10, 40)
            };

            // Eticheta timp inactiv
            _idleTimeLabel = new Label
            {
                Text = "Idle",
                AutoSize = true,
                Location = new Point(10, 70)
            };

            // Eticheta timp activ
            _activeTimeLabel = new Label
            {
                Text = "Active",
                AutoSize = true,
                Location = new Point(10, 100)
            };

            // Valoare timp total
            _totalTimeValueLabel = new Label
            {
                Text = "0h 0m",
                Font = new Font("Arial", 9, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(100, 40),
                TextAlign = ContentAlignment.MiddleRight
            };

            // Valoare timp inactiv
            _idleTimeValueLabel = new Label
            {
                Text = "0h 0m",
                Font = new Font("Arial", 9, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(100, 70),
                TextAlign = ContentAlignment.MiddleRight
            };

            // Valoare timp activ
            _activeTimeValueLabel = new Label
            {
                Text = "0h 0m",
                Font = new Font("Arial", 9, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(100, 100),
                TextAlign = ContentAlignment.MiddleRight
            };

            // Eticheta uptime sistem
            _systemUptimeLabel = new Label
            {
                Text = "System Uptime",
                AutoSize = true,
                Location = new Point(10, 130)
            };

            // Valoare uptime sistem
            _systemUptimeValueLabel = new Label
            {
                Text = "0d 0h 0m",
                Font = new Font("Arial", 9, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(100, 130),
                TextAlign = ContentAlignment.MiddleRight
            };

            // Eticheta timp categorie (initial invizibila)
            _categoryTimeLabel = new Label
            {
                Text = "Category Time",
                AutoSize = true,
                Location = new Point(10, 160),
                Visible = false
            };

            // Valoare timp categorie (initial invizibila)
            _categoryTimeValueLabel = new Label
            {
                Text = "0h 0m",
                Font = new Font("Arial", 9, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(100, 160),
                TextAlign = ContentAlignment.MiddleRight,
                Visible = false
            };

            // Adauga etichetele in panou
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
        /// Actualizeaza statisticile cu datele unui proces
        /// </summary>
        /// <param name="processData">Datele procesului pentru afisare</param>
        public void UpdateStats(ProcessData processData)
        {
            try
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

                _totalTimeValueLabel.Text = Utils.FormatTimeSpan(totalTime);
                _idleTimeValueLabel.Text = Utils.FormatTimeSpan(idleTime);
                _activeTimeValueLabel.Text = Utils.FormatTimeSpan(activeTime);

                SetLabelVisibility(isProcessView: true);
            }
            catch (Exception ex)
            {
                Logger.Log(new UIException("Error updating stats panel.", ex), "StatsPanel.UpdateStats");
            }
        }

        /// <summary>
        /// Actualizeaza panoul pentru a afisa timpul activ total pentru o categorie astazi
        /// </summary>
        /// <param name="categoryName">Numele categoriei</param>
        /// <param name="totalActiveMinutesToday">Timpul activ total astazi (minute)</param>
        public void UpdateCategoryStats(string categoryName, double totalActiveMinutesToday)
        {
            try
            {
                _titleLabel.Text = $"{categoryName} Stats";
                _categoryTimeLabel.Text = "Today's Active";
                _categoryTimeValueLabel.Text = Utils.FormatTimeSpan(TimeSpan.FromMinutes(totalActiveMinutesToday));

                SetLabelVisibility(isCategoryView: true);
            }
            catch (Exception ex)
            {
                Logger.Log(new UIException("Error updating category stats.", ex), "StatsPanel.UpdateCategoryStats");
            }
        }

        /// <summary>
        /// Actualizeaza afisarea timpului de functionare al sistemului
        /// </summary>
        /// <param name="uptime">Timpul curent de functionare al sistemului</param>
        public void UpdateSystemUptime(TimeSpan uptime)
        {
            try
            {
                if (_titleLabel.Text == "Statistics" || string.IsNullOrEmpty(_titleLabel.Text))
                {
                    _titleLabel.Text = "System Stats";
                }
                _systemUptimeValueLabel.Text = $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m";
                SetLabelVisibility(isSystemView: true);
            }
            catch (Exception ex)
            {
                Logger.Log(new UIException("Error updating system uptime.", ex), "StatsPanel.UpdateSystemUptime");
            }
        }

        /// <summary>
        /// Reseteaza toate statisticile afisate
        /// </summary>
        public void ClearStats()
        {
            try
            {
                _titleLabel.Text = "Statistics";
                _totalTimeValueLabel.Text = "0h 0m";
                _idleTimeValueLabel.Text = "0h 0m";
                _activeTimeValueLabel.Text = "0h 0m";
                _systemUptimeValueLabel.Text = "0d 0h 0m";
                _categoryTimeValueLabel.Text = "0h 0m";

                SetLabelVisibility(isProcessView: false, isCategoryView: false, isSystemView: false);
            }
            catch (Exception ex)
            {
                Logger.Log(new UIException("Error clearing stats panel.", ex), "StatsPanel.ClearStats");
            }
        }

        /// <summary>
        /// Seteaza vizibilitatea etichetelor in functie de tipul statisticilor afisate
        /// </summary>
        /// <param name="isProcessView">Adevarat daca se afiseaza statistici pentru procese</param>
        /// <param name="isCategoryView">Adevarat daca se afiseaza statistici pentru categorii</param>
        /// <param name="isSystemView">Adevarat daca se afiseaza statistici pentru sistem</param>
        private void SetLabelVisibility(bool isProcessView = false, bool isCategoryView = false, bool isSystemView = false)
        {
            _totalTimeLabel.Visible = isProcessView;
            _totalTimeValueLabel.Visible = isProcessView;
            _idleTimeLabel.Visible = isProcessView;
            _idleTimeValueLabel.Visible = isProcessView;
            _activeTimeLabel.Visible = isProcessView;
            _activeTimeValueLabel.Visible = isProcessView;

            _categoryTimeLabel.Visible = isCategoryView;
            _categoryTimeValueLabel.Visible = isCategoryView;

            _systemUptimeLabel.Visible = isSystemView;
            _systemUptimeValueLabel.Visible = isSystemView;
        }
    }
}
