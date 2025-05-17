using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace @interface
{
    /// <summary>
    /// Class to monitor system processes and track their usage
    /// </summary>
    public class ProcessMonitor
    {
        private Dictionary<int, ProcessData> _processes;
        private Dictionary<int, DateTime> _processStartTimes;
        private Dictionary<string, string> _processCategories;
        private Timer _monitorTimer;
        private const int MonitorInterval = 5000; // 5 seconds

        /// <summary>
        /// Constructor for ProcessMonitor
        /// </summary>
        public ProcessMonitor()
        {
            _processes = new Dictionary<int, ProcessData>();
            _processStartTimes = new Dictionary<int, DateTime>();
            _processCategories = InitializeDefaultCategories();
            
            // Start monitoring processes
            _monitorTimer = new Timer(MonitorProcesses, null, 0, MonitorInterval);
        }

        /// <summary>
        /// Initialize default categories for common applications
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
        /// Gets the current list of process data
        /// </summary>
        public List<ProcessData> GetProcessData()
        {
            return _processes.Values.ToList();
        }

        /// <summary>
        /// Update process data by monitoring running processes
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
                        //TO DO: Make it not skip background processes

                        if (string.IsNullOrEmpty(process.MainWindowTitle) && 
                            process.ProcessName != "explorer") // Skip background processes except explorer
                        {
                            continue;
                        }
                        
                        currentPids.Add(process.Id);
                        
                        // If this is a new process, add it to our tracking
                        if (!_processes.ContainsKey(process.Id))
                        {
                            AddNewProcess(process);
                        }
                        else
                        {
                            // Update existing process data
                            UpdateProcessData(process);
                        }
                    }
                    catch (Exception)
                    {
                        // Process may have exited, skip it
                    }
                }
                
                // Remove processes that are no longer running
                List<int> processesToRemove = _processes.Keys.Where(pid => !currentPids.Contains(pid)).ToList();
                foreach (int pid in processesToRemove)
                {
                    _processes.Remove(pid);
                    _processStartTimes.Remove(pid);
                }
            }
            catch (Exception)
            {
                // Log or handle the exception
            }
        }

        /// <summary>
        /// Add a new process to tracking
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
                
                // Add historical data - for now just today's data
                data.HistoricalData.Add(new KeyValuePair<DateTime, double>(
                    DateTime.Today, 
                    (DateTime.Now - startTime).TotalHours));
                
                _processes[process.Id] = data;
                _processStartTimes[process.Id] = startTime;
            }
            catch (Exception)
            {
                // Process may have exited, skip it
            }
        }

        /// <summary>
        /// Update an existing process with current data
        /// </summary>
        private void UpdateProcessData(Process process)
        {
            try
            {
                if (_processStartTimes.TryGetValue(process.Id, out DateTime startTime))
                {
                    ProcessData data = _processes[process.Id];
                    
                    // Update process title if it has changed
                    if (string.IsNullOrEmpty(data.Name) && !string.IsNullOrEmpty(process.MainWindowTitle))
                    {
                        data.Name = process.MainWindowTitle;
                    }
                    
                    // Update time today
                    data.TimeToday = DateTime.Now - startTime;
                    
                    // Update historical data for today
                    var todayData = data.HistoricalData.FirstOrDefault(kvp => kvp.Key.Date == DateTime.Today.Date);
                    int index = data.HistoricalData.IndexOf(todayData);
                    
                    if (index >= 0)
                    {
                        data.HistoricalData[index] = new KeyValuePair<DateTime, double>(
                            DateTime.Today,
                            (DateTime.Now - startTime).TotalHours);
                    }
                }
            }
            catch (Exception)
            {
                // Process may have exited, skip it
            }
        }

        /// <summary>
        /// Get the category for a process based on its name
        /// </summary>
        private string GetProcessCategory(string processName)
        {
            if (_processCategories.TryGetValue(processName, out string category))
            {
                return category;
            }
            
            return "Other";
        }

        /// <summary>
        /// Get the start time of a process
        /// </summary>
        private DateTime GetProcessStartTime(Process process)
        {
            try
            {
                return process.StartTime;
            }
            catch (Exception)
            {
                // Some system processes don't allow access to the start time
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Set a custom category for a process
        /// </summary>
        public void SetProcessCategory(string processName, string category)
        {
            _processCategories[processName] = category;
            
            // Update categories for existing processes
            foreach (var process in _processes.Values.Where(p => p.Name.Contains(processName)))
            {
                process.Department = category;
            }
        }

        /// <summary>
        /// Stop monitoring processes
        /// </summary>
        public void StopMonitoring()
        {
            _monitorTimer?.Dispose();
        }
    }
} 