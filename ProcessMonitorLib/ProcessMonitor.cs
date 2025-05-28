/**************************************************************************
 *                                                                        *
 *  File:        ProcessMonitor.cs                                        *
 *  Copyright:   (c) 2025, Bostan Sorina-Gabriela, Brinza Denis-Stefan,   *
 *  Colibaba Rares-Andrei, Dodita Alexandru-Tomi                          *
 *  Authors:     Bostan Sorina-Gabriela, Brinza Denis-Stefan,             *
 *               Colibaba Rares-Andrei, Dodita Alexandru-Tomi             *
 *  University:  Universitatea Tehnica "Gheorghe Asachi" din Iasi         *
 *  Group:       1312A                                                    *
 *                                                                        *
 *  Description: Implementarea clasei ProcessMonitor responsabila cu      *
 *               monitorizarea proceselor active ale sistemului,          *
 *               colectarea datelor de utilizare (timp, categorie),       *
 *               si orchestrarea salvarii periodice a acestor date.       *
 *               Aceasta clasa utilizeaza timere pentru scanarea          *
 *               periodica a proceselor si pentru salvarea datelor,       *
 *               interactionand cu API-urile sistemului de operare        *
 *               si cu UsageDataService pentru persistenta datelor.       *
 *                                                                        *
 *  This code is part of the TimeThread application, a process time       *
 *  tracking utility.                                                     *
 *                                                                        *
 **************************************************************************/
using ProcessDataLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;
using UsageDataServiceLib;

namespace ProcessMonitorLib
{
    /// <summary>
    /// Clasa pentru monitorizarea proceselor si urmarirea utilizarii
    /// </summary>
    public class ProcessMonitor
    {
        private Dictionary<int, ProcessData> _processes;
        private Dictionary<int, DateTime> _processStartTimes;
        private Dictionary<string, string> _processCategories;
        private Timer _monitorTimer;
        private Timer _saveDataTimer;
        private const int MonitorInterval = 5000; // Cinci secunde
        private const int SaveDataInterval = 60000; // Un minut
        private UsageDataService _dataService;

        private bool _processDataChangedSinceLastSave = false;

        // Lista de observatori pentru procese
        private readonly ConcurrentBag<WeakReference<IProcessObserver>> _observers = new ConcurrentBag<WeakReference<IProcessObserver>>();
        private readonly object _lock = new object(); // Pentru sincronizarea accesului la datele partajate

        /// <summary>
        /// Constructor pentru ProcessMonitor
        /// </summary>
        public ProcessMonitor()
        {
            _processes = new Dictionary<int, ProcessData>();
            _processStartTimes = new Dictionary<int, DateTime>();
            _processCategories = InitializeDefaultCategories();
            _dataService = new UsageDataService();

            // Porneste monitorizarea proceselor
            _monitorTimer = new Timer(MonitorProcesses, null, 0, MonitorInterval);

            // Porneste salvarea datelor in CSV
            _saveDataTimer = new Timer(SaveDataToCSV, null, SaveDataInterval, SaveDataInterval);
        }

        /// <summary>
        /// Initializeaza categoriile implicite pentru aplicatii comune
        /// </summary>
        private Dictionary<string, string> InitializeDefaultCategories()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // Browsers
                { "chrome", "Entertainment" },
                { "firefox", "Entertainment" },
                { "edge", "Entertainment" },
                { "iexplore", "Entertainment" },
                { "opera", "Entertainment" },
                
                // Development tools
                { "devenv", "Coding" },  // Visual Studio
                { "code", "Coding" },    // VS Code
                { "rider", "Coding" },
                { "idea64", "Coding" },     //IntelliJ
                { "notepad++", "Coding" },
                { "sublime_text", "Coding" },
                { "atom", "Coding" },
                
                // Games and gaming platforms
                { "steam", "Games" },
                { "epicgameslauncher", "Games" },
                { "minecraft", "Games" },
                { "javaw", "Games" },    // Often used for Java games
                { "leagueclient", "Games" },
                { "origin", "Games" },
                { "battlenet", "Games" },
                
                // Productivity
                { "excel", "Learning" },
                { "word", "Learning" },
                { "powerpnt", "Learning" },
                { "onenote", "Learning" },
                { "outlook", "Learning" },
                { "teams", "Learning" },
                { "slack", "Learning" },
                { "zoom", "Learning" },
                
                // Entertainment
                { "spotify", "Entertainment" },
                { "netflix", "Entertainment" },
                { "vlc", "Entertainment" },
                { "wmplayer", "Entertainment" },  // Windows Media Player
                { "discord", "Entertainment" }
            };
        }

        /// <summary>
        /// Returneaza lista curenta de date despre procese
        /// </summary>
        public List<ProcessData> ProcessData
        {
            get
            {
                lock (_lock)
                {
                    return _processes.Values.ToList();
                }
            }
        }

        /// <summary>
        /// Returneaza un snapshot al tuturor proceselor curente (pentru observatori noi)
        /// </summary>
        public List<ProcessData> GetProcessSnapshot()
        {
            lock (_lock)
            {
                return _processes.Values.Select(pd => new ProcessData
                {
                    Name = pd.Name,
                    PID = pd.PID,
                    Department = pd.Department,
                    TimeToday = pd.TimeToday,
                    HistoricalData = new List<KeyValuePair<DateTime, double>>(pd.HistoricalData)
                }).ToList();
            }
        }

        /// <summary>
        /// Actualizeaza datele despre procese prin monitorizarea proceselor rulate
        /// </summary>
        private void MonitorProcesses(object state)
        {
            try
            {
                Process[] runningProcesses = Process.GetProcesses();
                HashSet<int> currentPids = new HashSet<int>();

                foreach (Process process in runningProcesses)
                {
                    try
                    {
                        // TODO: Sa nu mai sara procesele de fundal

                        if (string.IsNullOrEmpty(process.MainWindowTitle) &&
                            process.ProcessName != "explorer") // Daca procesul nu are fereastra si nu e explorer, il sarim
                        {
                            continue;
                        }
                        // Skip system processes
                        if (process.SessionId == 0 || process.ProcessName.Equals("System", StringComparison.OrdinalIgnoreCase))
                            continue;

                        currentPids.Add(process.Id);

                        bool addedOrUpdated = false;
                        lock (_lock)
                        {
                            // Daca este un proces nou, il adaugam la urmarire
                            if (!_processes.ContainsKey(process.Id))
                            {
                                AddNewProcess(process);
                                addedOrUpdated = true;
                            }
                            else
                            {
                                if (UpdateProcessData(process))
                                    addedOrUpdated = true;
                            }
                        }
                        if (addedOrUpdated)
                            _processDataChangedSinceLastSave = true;
                    }
                    catch (Exception ex)
                    {
                        // Procesul poate sa fi iesit, il sarim
                    }
                }

                // Eliminam procesele care nu mai ruleaza
                List<int> processesToRemove;
                lock (_lock)
                {
                    processesToRemove = _processes.Keys.Where(pid => !currentPids.Contains(pid)).ToList();
                    foreach (int pid in processesToRemove)
                    {
                        _processes.Remove(pid);
                        _processStartTimes.Remove(pid);
                    }
                }
                if (processesToRemove.Count > 0)
                    _processDataChangedSinceLastSave = true;
                foreach (int pid in processesToRemove)
                {
                    SafeNotifyProcessRemoved(pid); // Notifica observatorii
                }
            }
            catch (Exception ex)
            {
                LogException("MonitorProcesses", ex);
            }
        }

        /// <summary>
        /// Adauga un proces nou la urmarire
        /// </summary>
        private void AddNewProcess(Process process)
        {
            try
            {
                string category = GetProcessCategory(process.ProcessName);
                DateTime startTime = GetProcessStartTime(process);

                ProcessData data = new ProcessData
                {
                    Name = string.IsNullOrEmpty(process.MainWindowTitle) ? process.ProcessName : process.MainWindowTitle,
                    PID = process.Id,
                    Department = category,
                    TimeToday = DateTime.Now - startTime
                };

                // Adaugam date istorice - pentru moment doar pentru azi
                data.HistoricalData.Add(new KeyValuePair<DateTime, double>(
                    DateTime.Today,
                    (DateTime.Now - startTime).TotalHours));

                lock (_lock)
                {
                    _processes[process.Id] = data;
                    _processStartTimes[process.Id] = startTime;
                }
                _processDataChangedSinceLastSave = true;
                SafeNotifyProcessAdded(data);
            }
            catch (Exception ex)
            {
                LogException("AddNewProcess", ex);
            }
        }

        /// <summary>
        /// Actualizeaza un proces existent cu datele curente
        /// </summary>
        /// <returns>true daca s-a facut o actualizare</returns>
        private bool UpdateProcessData(Process process)
        {
            try
            {
                ProcessData data;
                DateTime startTime;
                lock (_lock)
                {
                    if (!_processStartTimes.TryGetValue(process.Id, out startTime))
                        return false;
                    data = _processes[process.Id];
                }

                bool updated = false;

                // Actualizam titlul procesului daca s-a schimbat
                if (string.IsNullOrEmpty(data.Name) && !string.IsNullOrEmpty(process.MainWindowTitle))
                {
                    data.Name = process.MainWindowTitle;
                    updated = true;
                }

                // Actualizam timpul de azi
                var newTimeToday = DateTime.Now - startTime;
                if (data.TimeToday != newTimeToday)
                {
                    data.TimeToday = newTimeToday;
                    updated = true;
                }

                // Actualizam datele istorice pentru azi
                var todayData = data.HistoricalData.FirstOrDefault(kvp => kvp.Key.Date == DateTime.Today.Date);
                int index = data.HistoricalData.IndexOf(todayData);

                if (index >= 0)
                {
                    var newHours = (DateTime.Now - startTime).TotalHours;
                    if (Math.Abs(data.HistoricalData[index].Value - newHours) > 0.001)
                    {
                        data.HistoricalData[index] = new KeyValuePair<DateTime, double>(
                            DateTime.Today,
                            newHours);
                        updated = true;
                    }
                }

                if (updated)
                {
                    _processDataChangedSinceLastSave = true;
                    SafeNotifyProcessUpdated(data);
                }
                return updated;
            }
            catch (Exception ex)
            {
                LogException("UpdateProcessData", ex);
                return false;
            }
        }

        /// <summary>
        /// Returneaza categoria pentru un proces pe baza numelui
        /// </summary>
        private string GetProcessCategory(string processName)
        {
            lock (_lock)
            {
                if (_processCategories.TryGetValue(processName, out string category))
                {
                    return category;
                }
            }
            return "Other";
        }

        /// <summary>
        /// Returneaza timpul de start al unui proces
        /// </summary>
        private DateTime GetProcessStartTime(Process process)
        {
            try
            {
                return process.StartTime;
            }
            catch (Exception)
            {
                // Unele procese de sistem nu permit accesul la timpul de start
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Seteaza o categorie personalizata pentru un proces
        /// </summary>
        public void SetProcessCategory(string processName, string category)
        {
            List<ProcessData> updatedProcesses = new List<ProcessData>();
            lock (_lock)
            {
                _processCategories[processName] = category;

                // Actualizam categoriile pentru procesele existente
                foreach (var process in _processes.Values.Where(p => p.Name.Contains(processName)))
                {
                    process.Department = category;
                    updatedProcesses.Add(process);
                }
            }
            if (updatedProcesses.Count > 0)
            {
                _processDataChangedSinceLastSave = true;
                // Batch notification: notify once for all updated processes
                SafeNotifyProcessesUpdated(updatedProcesses);
            }
        }

        /// <summary>
        /// Notifica observatorii despre actualizarea mai multor procese (batch)
        /// </summary>
        private void SafeNotifyProcessesUpdated(IEnumerable<ProcessData> processes)
        {
            var observers = GetLiveObservers();
            foreach (var observer in observers)
            {
                try
                {
                    foreach (var process in processes)
                        observer.OnProcessUpdated(process);
                }
                catch (Exception ex)
                {
                    LogException("SafeNotifyProcessesUpdated", ex);
                }
            }
        }

        /// <summary>
        /// Opreste monitorizarea proceselor
        /// </summary>
        public void StopMonitoring()
        {
            _monitorTimer?.Dispose();
            _saveDataTimer?.Dispose();

            // Salvam datele inainte de oprire
            SaveDataToCSV(null);
        }

        /// <summary>
        /// Salveaza datele despre procese in fisierul CSV si actualizeaza timpul total de pornire al sistemului
        /// </summary>
        private void SaveDataToCSV(object state)
        {
            try
            {
                if (!_processDataChangedSinceLastSave)
                    return; // Nicio schimbare, skip la scriere

                // Salvam datele despre procese
                List<ProcessData> processesToSave;
                lock (_lock)
                {
                    processesToSave = _processes.Values.ToList();
                }
                _dataService.SaveProcessData(processesToSave);

                // Calculam si salvam timpul total de functionare al sistemului pentru azi
                DateTime lastBoot = GetLastBootUpTime();
                if (lastBoot != DateTime.MinValue)
                {
                    // Metoda returneaza o lista, pentru azi va fi un singur element
                    var todayOnTimeData = _dataService.GetDailySystemOnTime(1, lastBoot);
                    if (todayOnTimeData.Any())
                    {
                        // Metoda calculeaza pana la momentul curent pentru azi
                        double currentTodayOnTimeHours = todayOnTimeData.First().Value;
                        _dataService.SaveDailySystemOnTime(DateTime.Today, currentTodayOnTimeHours);
                        Console.WriteLine($"Saved today's system on-time: {currentTodayOnTimeHours:F2} hours.");
                    }
                }
                _processDataChangedSinceLastSave = false;
            }
            catch (Exception ex) // Exceptie generala pentru metoda
            {
                Console.WriteLine($"Error in SaveDataToCSV (ProcessMonitor): {ex.Message}");
            }
        }

        /// <summary>
        /// Returneaza timpul ultimei porniri a sistemului
        /// </summary>
        /// <returns>DateTime representing the last boot up time, or DateTime.MinValue if an error occurs.</returns>
        public static DateTime GetLastBootUpTime()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT LastBootUpTime FROM Win32_OperatingSystem"))
                {
                    foreach (ManagementObject os in searcher.Get())
                    {
                        return ManagementDateTimeConverter.ToDateTime(os["LastBootUpTime"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting last boot up time: {ex.Message}");
                // In unele cazuri, poti loga sau trata mai detaliat
            }
            return DateTime.MinValue; // Nu ar trebui sa se intample in mod normal
        }

        /// <summary>
        /// Returneaza timpul de functionare al sistemului
        /// </summary>
        /// <param name="lastBootTime">The system's last boot time.</param>
        /// <returns>TimeSpan representing the system uptime.</returns>
        public static TimeSpan GetSystemUptime(DateTime lastBootTime)
        {
            if (lastBootTime == DateTime.MinValue)
            {
                return TimeSpan.Zero;
            }
            return DateTime.Now - lastBootTime;
        }

        // Metode pentru patternul Observer
        public void Subscribe(IProcessObserver observer)
        {
            if (observer == null) return;
            _observers.Add(new WeakReference<IProcessObserver>(observer));
            // Immediately sync state for new observer
            var snapshot = GetProcessSnapshot();
            foreach (var process in snapshot)
            {
                try { observer.OnProcessAdded(process); }
                catch (Exception ex) { LogException("Subscribe.OnProcessAdded", ex); }
            }
        }

        public void Unsubscribe(IProcessObserver observer)
        {
            // No direct removal in ConcurrentBag; rely on WeakReference GC
        }

        // Helper to get live observers
        private List<IProcessObserver> GetLiveObservers()
        {
            var live = new List<IProcessObserver>();
            foreach (var weak in _observers)
            {
                if (weak.TryGetTarget(out var obs) && obs != null)
                    live.Add(obs);
            }
            return live;
        }

        // Metode pentru notificarea observatorilor
        private void SafeNotifyProcessAdded(ProcessData process)
        {
            foreach (var observer in GetLiveObservers())
            {
                try { observer.OnProcessAdded(process); }
                catch (Exception ex) { LogException("SafeNotifyProcessAdded", ex); }
            }
        }

        private void SafeNotifyProcessRemoved(int pid)
        {
            foreach (var observer in GetLiveObservers())
            {
                try { observer.OnProcessRemoved(pid); }
                catch (Exception ex) { LogException("SafeNotifyProcessRemoved", ex); }
            }
        }

        private void SafeNotifyProcessUpdated(ProcessData process)
        {
            foreach (var observer in GetLiveObservers())
            {
                try { observer.OnProcessUpdated(process); }
                catch (Exception ex) { LogException("SafeNotifyProcessUpdated", ex); }
            }
        }

        // Logging helper
        private void LogException(string context, Exception ex)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {context}: {ex}");
                // Optionally, log to file or telemetry here
            }
            catch { }
        }
    }
}