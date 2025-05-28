/**************************************************************************
 *                                                                        *
 *  File:        UsageDataService.cs (sau numele real al fisierului)      *
 *  Copyright:   (c) 2025, Bostan Sorina-Gabriela, Brinza Denis-Stefan,   *
 *  Colibaba Rares-Andrei, Dodita Alexandru-Tomi                          *
 *  Authors:     Bostan Sorina-Gabriela, Brinza Denis-Stefan,             *
 *               Colibaba Rares-Andrei, Dodita Alexandru-Tomi             *
 *  University:  Universitatea Tehnica "Gheorghe Asachi" din Iasi         *
 *  Group:       1312A                                                    *
 *                                                                        *
 *  Description: Implementarea clasei UsageDataService, responsabila      *
 *               cu gestionarea persistentei datelor aplicatiei           *
 *               TimeThread. Aceasta include crearea si mentenanta        *
 *               fisierelor CSV pentru datele de utilizare ale            *
 *               proceselor si pentru timpul total de functionare zilnic  *
 *               al sistemului. Clasa ofera metode pentru salvarea        *
 *               datelor noi sau actualizate.                             *
 *                                                                        *
 *  This code is part of the TimeThread application, a process time       *
 *  tracking utility.                                                     *
 *                                                                        *
 **************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProcessDataLib;
using Common;

namespace UsageDataServiceLib
{  
    /// <summary>
    /// Clasa de servicii pentru gestionarea persistentei si preluarii datelor despre utilizarea proceselor
    /// </summary>
    public class UsageDataService
    {
        private string _dataFilePath;
        private string _dailySystemOnTimeCsvPath;
        private const string DATA_FOLDER = "data";
        private const string CSV_FILENAME = "usage_data.csv";
        private const string DAILY_ON_TIME_CSV_FILENAME = "daily_system_on_time.csv";

        // Cache pentru a nu scrie de mai multe ori aceleasi date
        private List<ProcessDataLib.ProcessData> _lastSavedProcessData = null;
        private DateTime _lastSavedSystemOnTimeDate = DateTime.MinValue;
        private double _lastSavedSystemOnTimeValue = -1;
        
        /// <summary>
        /// Constructor pentru UsageDataService
        /// </summary>
        public UsageDataService()
        {
            try
            {
                // Get the current directory path
                string basePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string dataFolderPath = Path.Combine(basePath, DATA_FOLDER);
                
                // Ensure data directory exists
                if (!Directory.Exists(dataFolderPath))
                {
                    Directory.CreateDirectory(dataFolderPath);
                }
                
                _dataFilePath = Path.Combine(dataFolderPath, CSV_FILENAME);
                _dailySystemOnTimeCsvPath = Path.Combine(dataFolderPath, DAILY_ON_TIME_CSV_FILENAME);
                
                // Create file with header if it doesn't exist
                if (!File.Exists(_dataFilePath))
                {
                    CreateEmptyDataFile(_dataFilePath, "Date,ProcessName,ProcessId,Category,ActiveTimeMinutes,IdleTimeMinutes");
                }
                
                EnsureDailySystemOnTimeCsvExists();
                
                // Debug output to console to help track file location
                Console.WriteLine($"Process CSV file path: {_dataFilePath}");
                Console.WriteLine($"Daily On Time CSV file path: {_dailySystemOnTimeCsvPath}");
            }
            catch (Exception ex)
            {
                Logger.Log(new UsageDataServiceException("Error initializing UsageDataService.", ex), "UsageDataService.ctor");
                // Fallback to a more accessible location if there's an error
                _dataFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), CSV_FILENAME);
                _dailySystemOnTimeCsvPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), DAILY_ON_TIME_CSV_FILENAME);
            }
        }
        
        /// <summary>
        /// Creeaza un fisier de date gol cu antet
        /// </summary>
        private void CreateEmptyDataFile(string filePath, string header)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    writer.WriteLine(header);
                }
                
                Console.WriteLine($"Empty data file created: {filePath}");
            }
            catch (Exception ex)
            {
                Logger.Log(new UsageDataServiceException($"Error creating empty data file {filePath}.", ex), "CreateEmptyDataFile");
            }
        }
        
        /// <summary>
        /// Asigura existenta fisierului daily_system_on_time.csv cu antetul sau
        /// </summary>
        private void EnsureDailySystemOnTimeCsvExists()
        {
            if (!File.Exists(_dailySystemOnTimeCsvPath))
            {
                CreateEmptyDataFile(_dailySystemOnTimeCsvPath, "Date,TotalOnTimeHours");
            }
        }
        
        /// <summary>
        /// Salveaza datele despre utilizarea proceselor in fisierul CSV
        /// </summary>
        /// <param name="processes">List of processes to save</param>
        public void SaveProcessData(List<ProcessData> processes)
        {
            try
            {
                // Salvam in fisier doar daca exista schimbari
                if (_lastSavedProcessData != null && _lastSavedProcessData.Count == processes.Count)
                {
                    bool same = true;
                    for (int i = 0; i < processes.Count; i++)
                    {
                        if (processes[i].PID != _lastSavedProcessData[i].PID ||
                            processes[i].TimeToday != _lastSavedProcessData[i].TimeToday)
                        {
                            same = false;
                            break;
                        }
                    }
                    if (same) return;
                }

                // Load all existing data
                Dictionary<string, List<string>> existingDataByDate = new Dictionary<string, List<string>>();
                List<string> headerLines = new List<string>();
                bool headerProcessed = false;
                
                if (File.Exists(_dataFilePath))
                {
                    using (StreamReader reader = new StreamReader(_dataFilePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (!headerProcessed)
                            {
                                // Save header
                                headerLines.Add(line);
                                headerProcessed = true;
                                continue;
                            }
                            
                            if (string.IsNullOrWhiteSpace(line))
                                continue;
                                
                            string[] parts = line.Split(',');
                            if (parts.Length >= 6)
                            {
                                string date = parts[0];
                                string processNameWithQuotes = parts[1];
                                
                                // Create a key to identify this record
                                string recordKey = $"{date}_{processNameWithQuotes}";
                                
                                if (!existingDataByDate.ContainsKey(date))
                                {
                                    existingDataByDate[date] = new List<string>();
                                }
                                
                                existingDataByDate[date].Add(line);
                            }
                        }
                    }
                }
                
                // If no header was found, create it
                if (headerLines.Count == 0)
                {
                    headerLines.Add("Date,ProcessName,ProcessId,Category,ActiveTimeMinutes,IdleTimeMinutes");
                }
                
                // Create updated data for today
                string todayStr = DateTime.Today.ToString("yyyy-MM-dd");
                HashSet<string> updatedProcessKeys = new HashSet<string>();
                
                // Add updated data for each process
                foreach (ProcessData process in processes)
                {
                    if (process.TimeToday.TotalMinutes < 1) continue;

                    double activeMinutes = process.GetActiveTime().TotalMinutes;
                    double idleMinutes = process.GetIdleTime().TotalMinutes;
                    string processNameWithQuotes = $"\"{process.Name.Replace("\"", "\"\"")}\"";

                    // Use CultureInfo.InvariantCulture for double to string conversion
                    string line = $"{todayStr}," +
                                  $"{processNameWithQuotes}," +
                                  $"{process.PID}," +
                                  $"{process.Department}," +
                                  $"{activeMinutes.ToString("F2", CultureInfo.InvariantCulture)}," +
                                  $"{idleMinutes.ToString("F2", CultureInfo.InvariantCulture)}";
                    
                    // Record that we've updated this process
                    string processKey = processNameWithQuotes;
                    updatedProcessKeys.Add(processKey);
                    
                    // Update or add to the existing data
                    if (existingDataByDate.ContainsKey(todayStr))
                    {
                        // Check if this process already exists for today
                        bool processFound = false;
                        List<string> updatedDayData = new List<string>();
                        
                        foreach (string existingLine in existingDataByDate[todayStr])
                        {
                            string[] parts = existingLine.Split(',');
                            if (parts.Length >= 6)
                            {
                                string existingProcessName = parts[1];
                                
                                // If this is the same process, replace with updated data
                                if (existingProcessName.Equals(processNameWithQuotes, StringComparison.OrdinalIgnoreCase))
                                {
                                    updatedDayData.Add(line);
                                    processFound = true;
                                }
                                else
                                {
                                    // Keep other processes for today
                                    updatedDayData.Add(existingLine);
                                }
                            }
                        }
                        
                        // If process wasn't found, add it
                        if (!processFound)
                        {
                            updatedDayData.Add(line);
                        }
                        
                        // Replace today's data with updated list
                        existingDataByDate[todayStr] = updatedDayData;
                    }
                    else
                    {
                        // First entry for today
                        existingDataByDate[todayStr] = new List<string> { line };
                    }
                }
                
                // Combine all lines and write back to file
                List<string> outputLines = new List<string>(headerLines);
                
                foreach (var date in existingDataByDate.Keys.OrderBy(d => d))
                {
                    outputLines.AddRange(existingDataByDate[date]);
                }
                
                // Write all lines back to the file
                File.WriteAllLines(_dataFilePath, outputLines);
                
                Console.WriteLine($"Saved data to CSV with {outputLines.Count - 1} records");

                // Actualizare cache
                _lastSavedProcessData = processes.Select(pd => new ProcessData
                {
                    Name = pd.Name,
                    PID = pd.PID,
                    Department = pd.Department,
                    TimeToday = pd.TimeToday,
                    HistoricalData = new List<KeyValuePair<DateTime, double>>(pd.HistoricalData)
                }).ToList();
            }
            catch (Exception ex)
            {
                Logger.Log(new UsageDataServiceException("Error saving process data.", ex), "SaveProcessData");
            }
        }
        
        /// <summary>
        /// Returneaza datele procesului pentru o anumita data
        /// </summary>
        /// <param name="date">The date to get data for</param>
        /// <returns>Dictionary of process data keyed by ProcessName_PID</returns>
        private Dictionary<string, ProcessUsageRecord> GetProcessDataForDate(DateTime date)
        {
            try
            {
                Dictionary<string, ProcessUsageRecord> result = new Dictionary<string, ProcessUsageRecord>();
                if (!File.Exists(_dataFilePath)) return result;
                    
                string dateStr = date.ToString("yyyy-MM-dd");
                using (StreamReader reader = new StreamReader(_dataFilePath))
                {
                    reader.ReadLine(); // Skip header
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        string[] parts = line.Split(',');
                        
                        if (parts.Length >= 6 && parts[0] == dateStr)
                        {
                            string processName = parts[1].Trim('"');
                            int processId = 0;
                            double activeMinutes = 0;
                            double idleMinutes = 0;

                            // Use CultureInfo.InvariantCulture for parsing
                            if (int.TryParse(parts[2], out processId) &&
                                double.TryParse(parts[4], NumberStyles.Any, CultureInfo.InvariantCulture, out activeMinutes) &&
                                double.TryParse(parts[5], NumberStyles.Any, CultureInfo.InvariantCulture, out idleMinutes))
                            {
                                string key = $"{processName}_{processId}";
                                result[key] = new ProcessUsageRecord
                                {
                                    Date = date,
                                    ProcessName = processName,
                                    ProcessId = processId,
                                    Category = parts[3],
                                    ActiveMinutes = activeMinutes,
                                    IdleMinutes = idleMinutes
                                };
                            }
                            else
                            {
                                Console.WriteLine($"GetProcessDataForDate: Failed to parse data for date {dateStr}. Line: {string.Join(",",parts)}");
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log(new UsageDataServiceException("Error reading process data for date.", ex), "GetProcessDataForDate");
                return new Dictionary<string, ProcessUsageRecord>();
            }
        }
        
        /// <summary>
        /// Returneaza datele istorice pentru un proces pentru ultimele n zile din CSV
        /// </summary>
        /// <param name="processName">Process name to get data for</param>
        /// <param name="days">Number of days to get data for</param>
        /// <returns>List of key value pairs with date and hours</returns>
        public List<KeyValuePair<DateTime, double>> GetHistoricalDataForProcess(string processName, int days)
        {
            try
            {
                List<KeyValuePair<DateTime, double>> result = new List<KeyValuePair<DateTime, double>>();
                
                if (!File.Exists(_dataFilePath))
                    return result;
                    
                // Calculate start date
                DateTime startDate = DateTime.Today.AddDays(-(days - 1));
                
                // Initialize result with all days (including zero values)
                for (int i = 0; i < days; i++)
                {
                    result.Add(new KeyValuePair<DateTime, double>(startDate.AddDays(i), 0));
                }
                
                using (StreamReader reader = new StreamReader(_dataFilePath))
                {
                    reader.ReadLine(); // Skip header
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        string[] parts = line.Split(',');

                        if (parts.Length >= 6)
                        {
                            DateTime date;
                            if (DateTime.TryParseExact(parts[0], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date) && date >= startDate)
                            {
                                string currentProcessName = parts[1].Trim('"');
                                if (currentProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase))
                                {
                                    // Use CultureInfo.InvariantCulture for string to double parsing
                                    double activeMinutes = 0;
                                    double idleMinutes = 0;
                                    if (double.TryParse(parts[4], NumberStyles.Any, CultureInfo.InvariantCulture, out activeMinutes) &&
                                        double.TryParse(parts[5], NumberStyles.Any, CultureInfo.InvariantCulture, out idleMinutes))
                                    {
                                        double totalHours = (activeMinutes + idleMinutes) / 60.0;
                                        int index = result.FindIndex(kvp => kvp.Key.Date == date.Date);
                                        if (index >= 0)
                                        {
                                            double currentValue = result[index].Value;
                                            result[index] = new KeyValuePair<DateTime, double>(date.Date, currentValue + totalHours);
                                            // Log successful parse and add
                                            Console.WriteLine($"GetHistoricalData: Added {totalHours:F2}h for {processName} on {date.ToShortDateString()}");
                                        }
                                    }
                                    else
                                    {
                                         Console.WriteLine($"GetHistoricalData: Failed to parse minutes for {processName} on {date.ToShortDateString()}. Line: {string.Join(",", parts)}");
                                    }
                                }
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log(new UsageDataServiceException("Error reading historical data for process.", ex), "GetHistoricalDataForProcess");
                return new List<KeyValuePair<DateTime, double>>();
            }
        }
        
        /// <summary>
        /// Returneaza datele totale de utilizare PC pentru ultimele n zile
        /// </summary>
        /// <param name="days">Number of days to get data for</param>
        /// <returns>List of key value pairs with date and hours</returns>
        public List<KeyValuePair<DateTime, double>> GetTotalPCUsageData(int days)
        {
            try
            {
                List<KeyValuePair<DateTime, double>> result = new List<KeyValuePair<DateTime, double>>();
                
                if (!File.Exists(_dataFilePath))
                    return result;
                    
                // Calculate start date
                DateTime startDate = DateTime.Today.AddDays(-(days - 1));
                
                // Initialize result with all days (including zero values)
                for (int i = 0; i < days; i++)
                {
                    result.Add(new KeyValuePair<DateTime, double>(startDate.AddDays(i), 0));
                }
                
                // Dictionary to track unique processes per day (to avoid double-counting overlapping times)
                Dictionary<DateTime, HashSet<string>> processesPerDay = new Dictionary<DateTime, HashSet<string>>();
                
                using (StreamReader reader = new StreamReader(_dataFilePath))
                {
                    // Skip header
                    reader.ReadLine();
                    
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        
                        string[] parts = line.Split(',');
                        
                        if (parts.Length >= 6)
                        {
                            DateTime date;
                            if (DateTime.TryParse(parts[0], out date) && date >= startDate)
                            {
                                string processName = parts[1].Trim('"');
                                int processId = int.Parse(parts[2]);
                                double activeMinutes = double.Parse(parts[4]);
                                double idleMinutes = double.Parse(parts[5]);
                                double totalHours = (activeMinutes + idleMinutes) / 60.0;
                                
                                // Find and update the entry for this date
                                int index = result.FindIndex(kvp => kvp.Key.Date == date.Date);
                                if (index >= 0)
                                {
                                    // Add to existing value
                                    double currentValue = result[index].Value;
                                    result[index] = new KeyValuePair<DateTime, double>(date, currentValue + totalHours);
                                }
                                
                                // Track this process for the day
                                if (!processesPerDay.ContainsKey(date.Date))
                                {
                                    processesPerDay[date.Date] = new HashSet<string>();
                                }
                                processesPerDay[date.Date].Add($"{processName}_{processId}");
                            }
                        }
                    }
                }
                
                // Normalize the daily totals (to avoid excessive values due to parallel processes)
                for (int i = 0; i < result.Count; i++)
                {
                    DateTime day = result[i].Key.Date;
                    double value = result[i].Value;
                    
                    // Cap at 24 hours
                    if (value > 24)
                    {
                        value = 24;
                    }
                    
                    result[i] = new KeyValuePair<DateTime, double>(day, value);
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log(new UsageDataServiceException("Error reading total PC usage data.", ex), "GetTotalPCUsageData");
                return new List<KeyValuePair<DateTime, double>>();
            }
        }

        /// <summary>
        /// Returneaza timpul calculat de functionare zilnica a sistemului pentru ultimele n zile pe baza ultimei porniri
        /// </summary>
        /// <param name="days">Number of days to get data for.</param>
        /// <param name="lastBootTime">The system's last boot up time.</param>
        /// <returns>List of KeyValuePair with date and total hours the system was on.</returns>
        public List<KeyValuePair<DateTime, double>> GetDailySystemOnTime(int days, DateTime lastBootTime)
        {
            try
            {
                List<KeyValuePair<DateTime, double>> result = new List<KeyValuePair<DateTime, double>>();
                DateTime today = DateTime.Today;

                if (lastBootTime == DateTime.MinValue) // If boot time couldn't be determined
                {
                    for (int i = 0; i < days; i++)
                    {
                        result.Add(new KeyValuePair<DateTime, double>(today.AddDays(-(days - 1) + i), 0));
                    }
                    return result;
                }

                for (int i = 0; i < days; i++)
                {
                    DateTime currentDate = today.AddDays(-(days - 1) + i);
                    double hoursOn = 0;

                    if (currentDate < lastBootTime.Date)
                    {
                        hoursOn = 0; // System was not on yet
                    }
                    else if (currentDate == lastBootTime.Date)
                    {
                        // If it's the boot day
                        if (currentDate == today) // Booted today
                        {
                            hoursOn = (DateTime.Now - lastBootTime).TotalHours;
                        }
                        else // Booted on a previous day (this is the boot day)
                        {
                            hoursOn = (currentDate.AddDays(1) - lastBootTime).TotalHours;
                        }
                    }
                    else if (currentDate > lastBootTime.Date && currentDate < today)
                    {
                        // Full day between boot day and today
                        hoursOn = 24;
                    }
                    else if (currentDate == today)
                    {
                        // Today, and boot time was before today
                        hoursOn = DateTime.Now.TimeOfDay.TotalHours;
                    }
                    
                    // Ensure hoursOn does not exceed 24 for a single day (except potentially the current day if uptime spans >24h)
                    if (currentDate != today || (currentDate == today && lastBootTime.Date == today)){
                         hoursOn = Math.Min(hoursOn, 24.0);
                    }
                    hoursOn = Math.Max(0, hoursOn); // Ensure it's not negative

                    result.Add(new KeyValuePair<DateTime, double>(currentDate, hoursOn));
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Log(new UsageDataServiceException("Error calculating daily system on time.", ex), "GetDailySystemOnTime");
                return new List<KeyValuePair<DateTime, double>>();
            }
        }

        /// <summary>
        /// Returneaza timpul total activ pentru o categorie pentru ultimele n zile din CSV
        /// </summary>
        /// <param name="category">Category to filter by.</param>
        /// <param name="days">Number of days to get data for.</param>
        /// <returns>List of KeyValuePair with date and total active hours for the category.</returns>
        public List<KeyValuePair<DateTime, double>> GetTotalActiveTimeForCategory(string category, int days)
        {
            try
            {
                List<KeyValuePair<DateTime, double>> result = new List<KeyValuePair<DateTime, double>>();
                DateTime today = DateTime.Today;
                DateTime startDate = today.AddDays(-(days - 1));

                // Initialize result with all days (including zero values)
                for (int i = 0; i < days; i++)
                {
                    result.Add(new KeyValuePair<DateTime, double>(startDate.AddDays(i), 0));
                }

                if (!File.Exists(_dataFilePath))
                {
                    Console.WriteLine("CSV file not found for category aggregation.");
                    return result; // Return empty initialized list
                }

                using (StreamReader reader = new StreamReader(_dataFilePath))
                {
                    reader.ReadLine(); // Skip header
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        string[] parts = line.Split(',');

                        if (parts.Length >= 6)
                        {
                            // Use CultureInfo.InvariantCulture for parsing date and double
                            if (DateTime.TryParseExact(parts[0], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date) && 
                                date >= startDate && 
                                parts[3].Equals(category, StringComparison.OrdinalIgnoreCase))
                            {
                                if (double.TryParse(parts[4], NumberStyles.Any, CultureInfo.InvariantCulture, out double activeMinutes))
                                {
                                    int index = result.FindIndex(kvp => kvp.Key.Date == date.Date);
                                    if (index != -1)
                                    {
                                        double currentHours = result[index].Value;
                                        result[index] = new KeyValuePair<DateTime, double>(date.Date, currentHours + (activeMinutes / 60.0));
                                        Console.WriteLine($"GetCategoryTime: Added {(activeMinutes / 60.0):F2}h for category {category} on {date.ToShortDateString()}");
                                    }
                                }
                                else
                                {
                                     Console.WriteLine($"GetCategoryTime: Failed to parse activeMinutes for category {category} on {date.ToShortDateString()}. Line: {string.Join(",", parts)}");
                                }
                            }
                        }
                    }
                }
                
                // Cap hours at 24 per day
                for (int i = 0; i < result.Count; i++)
                {
                    if (result[i].Value > 24.0)
                    {
                        result[i] = new KeyValuePair<DateTime, double>(result[i].Key, 24.0);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Log(new UsageDataServiceException($"Error reading CSV for category {category} aggregation.", ex), "GetTotalActiveTimeForCategory");
                return new List<KeyValuePair<DateTime, double>>();
            }
        }

        /// <summary>
        /// Salveaza sau actualizeaza timpul total de functionare al sistemului pentru o anumita zi in daily_system_on_time.csv
        /// </summary>
        public void SaveDailySystemOnTime(DateTime dateToSave, double totalHours)
        {
            try
            {
                if (_lastSavedSystemOnTimeDate == dateToSave && Math.Abs(_lastSavedSystemOnTimeValue - totalHours) < 0.001)
                    return;

                EnsureDailySystemOnTimeCsvExists();
                List<string> lines = new List<string>();
                string dateToSaveStr = dateToSave.ToString("yyyy-MM-dd");
                bool dateFound = false;

                if (File.Exists(_dailySystemOnTimeCsvPath))
                {
                    lines.AddRange(File.ReadAllLines(_dailySystemOnTimeCsvPath));
                }

                // Find and update if exists, or add new line
                for (int i = 0; i < lines.Count; i++)
                {
                    string[] parts = lines[i].Split(',');
                    if (i == 0 && lines[i].Equals("Date,TotalOnTimeHours", StringComparison.OrdinalIgnoreCase)) continue; // Skip header if it's the first line
                    if (parts.Length > 0 && parts[0] == dateToSaveStr)
                    {
                        lines[i] = $"{dateToSaveStr},{totalHours.ToString("F2", CultureInfo.InvariantCulture)}";
                        dateFound = true;
                        break;
                    }
                }

                if (!dateFound)
                {
                     // Add header if file was empty or header was missing
                    if (!lines.Any() || !lines[0].Equals("Date,TotalOnTimeHours", StringComparison.OrdinalIgnoreCase))
                    {
                        lines.Insert(0, "Date,TotalOnTimeHours");
                    }
                    lines.Add($"{dateToSaveStr},{totalHours.ToString("F2", CultureInfo.InvariantCulture)}");
                }
                
                // Sort lines by date, keeping header at the top
                if (lines.Count > 1) {
                    var header = lines[0];
                    var dataLines = lines.Skip(1)
                        .Select(line => {
                            var parts = line.Split(',');
                            DateTime.TryParseExact(parts[0], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);
                            return new { Date = date, Line = line };
                        })
                        .OrderBy(x => x.Date)
                        .Select(x => x.Line)
                        .ToList();
                    lines = new List<string> { header };
                    lines.AddRange(dataLines);
                }

                File.WriteAllLines(_dailySystemOnTimeCsvPath, lines);
                _lastSavedSystemOnTimeDate = dateToSave;
                _lastSavedSystemOnTimeValue = totalHours;
            }
            catch (Exception ex)
            {
                Logger.Log(new UsageDataServiceException("Error saving daily system on time.", ex), "SaveDailySystemOnTime");
            }
        }

        /// <summary>
        /// Returneaza timpul de functionare zilnic persistat pentru ultimele n zile din daily_system_on_time.csv
        /// </summary>
        public List<KeyValuePair<DateTime, double>> GetPersistedDailySystemOnTime(int days)
        {
            try
            {
                EnsureDailySystemOnTimeCsvExists();
                List<KeyValuePair<DateTime, double>> result = new List<KeyValuePair<DateTime, double>>();
                Dictionary<DateTime, double> dataMap = new Dictionary<DateTime, double>();
                DateTime today = DateTime.Today;

                if (File.Exists(_dailySystemOnTimeCsvPath))
                {
                    string[] lines = File.ReadAllLines(_dailySystemOnTimeCsvPath);
                    foreach (string line in lines.Skip(1)) // Skip header
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        string[] parts = line.Split(',');
                        if (parts.Length == 2 && DateTime.TryParseExact(parts[0], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                        {
                            if (double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double hours))
                            {
                                dataMap[date.Date] = hours;
                            }
                        }
                    }
                }

                // Populate result for the last 'days', using stored data or 0 if not found
                for (int i = 0; i < days; i++)
                {
                    DateTime currentDate = today.AddDays(-(days - 1) + i);
                    double hours = dataMap.ContainsKey(currentDate.Date) ? dataMap[currentDate.Date] : 0;
                    result.Add(new KeyValuePair<DateTime, double>(currentDate, hours));
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log(new UsageDataServiceException("Error reading persisted daily system on time.", ex), "GetPersistedDailySystemOnTime");
                return new List<KeyValuePair<DateTime, double>>();
            }
        }
    }
    
    /// <summary>
    /// Record class for process usage data
    /// </summary>
    public class ProcessUsageRecord
    {
        public DateTime Date { get; set; }
        public string ProcessName { get; set; }
        public int ProcessId { get; set; }
        public string Category { get; set; }
        public double ActiveMinutes { get; set; }
        public double IdleMinutes { get; set; }
    }
} 