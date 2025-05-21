# Process Time Tracker Documentation Guide

The Process Time Tracker application monitors time spent in Windows applications, categorizes usage, and provides visual analytics. It allows users to track productivity, see historical usage patterns, and gain insights into their computer usage habits. The primary graph can display total system on-time (persisted daily), aggregated active time for a selected category, or active time for a specific process.

## Project File Structure

### Form1.cs - Main application window and user interface manager
    Form1 - Primary interface that holds all UI elements and manages user interactions.
        InitializeControls() - Sets up the UI components and their layout.
        LoadRealProcessData() - Gets current process data from the ProcessMonitor.
        UpdateGraphView() - Central method to update the graph based on current selection (Persisted Total PC On-Time, Category, or Process).
        ProcessListView_SelectedIndexChanged() - Handles selection of a specific process for graph display.
        CategorySelector_CategorySelected() - Handles selection of a category for graph display and list filtering.

### ProcessData.cs - Process information data model
    ProcessData - Represents a single process with its usage data.
        GetTotalTime() - Returns the total time spent on the process for the current session.
        GetActiveTime() - Returns the estimated active time for the process.
        GetIdleTime() - Returns the estimated idle time for the process.

### ProcessMonitor.cs - Process monitoring and tracking service
    ProcessMonitor - Monitors system processes and tracks their usage.
        MonitorProcesses() - Updates process data by monitoring running processes.
        SetProcessCategory() - Sets a custom category for a process.
        SaveDataToCSV() - Persists process data to `usage_data.csv` and triggers saving of today's total system on-time to `daily_system_on_time.csv`.
        GetLastBootUpTime() - Retrieves the system's last boot-up time.
        GetSystemUptime() - Calculates the current system uptime based on the last boot time.

### UsageDataService.cs - Data persistence and retrieval service
    UsageDataService - Manages data persistence and retrieval from CSV files.
        SaveProcessData() - Saves process usage data to `usage_data.csv`, preserving historical data.
        GetHistoricalDataForProcess() - Gets historical active time for a specific process from `usage_data.csv`.
        GetTotalActiveTimeForCategory() - Gets total active time for a specified category by summing data from `usage_data.csv`.
        GetDailySystemOnTime() - Calculates daily system on-time based on last boot time (primarily for today's current on-time).
        SaveDailySystemOnTime() - Saves or updates the total system on-time for a specific date in `daily_system_on_time.csv`.
        GetPersistedDailySystemOnTime() - Reads the last N days of on-time data from `daily_system_on_time.csv` for graph display.
        CreateEmptyDataFile() - Generic helper to create a CSV file with a specified header.
        EnsureDailySystemOnTimeCsvExists() - Ensures `daily_system_on_time.csv` exists with its header.
    ProcessUsageRecord - Record class for process usage data (structure within `usage_data.csv`).
        Contains fields for storing usage details including date, process name, ID, category, active and idle minutes.

### UsageGraph.cs - Graphical representation of usage data
    UsageGraph - Custom control for rendering the usage graph.
        DrawGraph() - Renders the graph based on current data with value labels and appropriate X/Y axis.
        UpdateTimeRange() - (Currently static to 30 days, but can be used to change time range).

### StatsPanel.cs - Usage statistics display panel
    StatsPanel - Panel that displays usage statistics.
        UpdateStats() - Updates the panel with data for a selected process.
        UpdateCategoryStats() - Updates the panel with total active time for a selected category today.
        UpdateSystemUptime() - Updates the panel with the current system uptime.
        ClearStats() - Clears all statistics in the panel and resets titles.
        SetLabelVisibility() - Manages which statistic labels are visible based on the current view.

### CategorySelector.cs - Category filter UI component
    CategorySelector - Panel with category filter buttons.
        SetCategories() - Sets the available categories for filtering.
        OnCategoryButtonClick() - Handles category button click events.

## CSV Data Formats

1.  **Process Usage Data (`usage_data.csv`)**: Stores detailed per-process usage.
    -   Location: `data/usage_data.csv` (relative to application executable).
    -   Columns: `Date,ProcessName,ProcessId,Category,ActiveTimeMinutes,IdleTimeMinutes`.
    -   Updated: Every minute for currently running processes.

2.  **Daily System On-Time (`daily_system_on_time.csv`)**: Stores the total time the PC was on for each day.
    -   Location: `data/daily_system_on_time.csv` (relative to application executable).
    -   Columns: `Date,TotalOnTimeHours`.
    -   Updated: Every minute (with today's cumulative on-time) and at shutdown. 