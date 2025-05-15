# Process Time Tracker Application

## Overview
This Windows Forms application implements a process time tracking interface that displays application usage statistics. Users can view time spent in various applications, filter by categories, and see historical usage data through graphs.

## UML Class Diagram

```
┌───────────────────────┐           ┌─────────────────────┐
│      MainForm         │           │   ProcessData       │
├───────────────────────┤           ├─────────────────────┤
│ - processListView     │           │ - Name: string      │
│ - usageGraph          │           │ - PID: int          │
│ - categorySelector    │           │ - Department: string│
│ - statsPanel          │           │ - TimeToday: TimeSpan│
├───────────────────────┤           ├─────────────────────┤
│ + InitializeComponent()│           │ + GetTotalTime()   │
│ + LoadProcessData()   │◄────────┐ │ + GetActiveTime()  │
│ + UpdateGraph()       │         │ │ + GetIdleTime()    │
│ + FilterByCategory()  │         │ └─────────────────────┘
└───────────────────────┘         │
                                  │ ┌─────────────────────┐
                                  └─│   UsageGraph        │
                                    ├─────────────────────┤
                                    │ - timeData: List    │
                                    │ - timeRange: string │
                                    ├─────────────────────┤
                                    │ + DrawGraph()       │
                                    │ + UpdateTimeRange() │
                                    └─────────────────────┘
```

## Components

### 1. MainForm
The primary interface that contains all UI elements and manages user interactions.

**Properties:**
- `processListView`: ListView displaying process information
- `usageGraph`: Custom control for rendering the usage graph
- `categorySelector`: Panel with category filter buttons
- `statsPanel`: Panel displaying usage statistics

**Methods:**
- `InitializeComponent()`: Sets up the UI components
- `LoadProcessData()`: Loads process data from the system
- `UpdateGraph()`: Updates the graph based on selected process
- `FilterByCategory()`: Filters the process list by selected category

### 2. ProcessData
Represents a single process with its usage data.

**Properties:**
- `Name`: Process name
- `PID`: Process ID
- `Department`: Category of the process (Games, Learning, etc.)
- `TimeToday`: Time spent on the process today

**Methods:**
- `GetTotalTime()`: Returns the total time spent on the process
- `GetActiveTime()`: Returns the active time spent on the process
- `GetIdleTime()`: Returns the idle time spent on the process

### 3. UsageGraph
Custom control for rendering the usage graph.

**Properties:**
- `timeData`: Collection of time data points
- `timeRange`: Selected time range (day, week, month)

**Methods:**
- `DrawGraph()`: Renders the graph based on current data
- `UpdateTimeRange()`: Changes the time range of displayed data

## Implementation Details

### Main Interface
The main interface is divided into three main sections:
1. **Top Section**: Contains the usage graph showing hours per day over the selected time period
2. **Bottom Left Section**: Contains the process list with columns for Name, PID, Department, and Today's usage
3. **Bottom Right Section**: Contains usage statistics and category filters

### Process List
The process list is implemented as a ListView control with columns for:
- Name: The name of the process
- PID: Process ID
- Dept: Category/department of the process
- Today: Time spent on the process today

### Usage Graph
The usage graph is implemented as a custom control that:
- Displays time usage over a specified period (day, week, month)
- Updates when a process is selected from the list
- Shows hours per day as a line graph

### Category Filter
The category filter allows users to filter processes by categories:
- Games
- Learning
- Coding
- Entertainment

### Statistics Panel
The statistics panel shows:
- Total time spent today
- Idle time
- Active time

## User Interaction Flow
1. User opens the application
2. The application loads and displays all processes with their usage data
3. User can filter processes by category using the category buttons
4. User can select a process from the list to view its detailed usage graph
5. The graph updates to show the selected process's usage over time
6. Statistics panel updates to show the selected process's usage statistics 