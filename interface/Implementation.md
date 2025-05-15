# Process Time Tracker Implementation Documentation

## 1. Introduction

This document provides detailed information about the implementation of the Process Time Tracker application interface. The application is designed to track and visualize time spent on different processes, categorize them, and provide statistical insights.

## 2. Interface Components

### 2.1 Main Form

The main form (`Form1`) serves as the container for all UI components and manages the application's logic. It is responsible for:

- Initializing all UI components
- Loading and managing process data
- Handling user interactions
- Updating the UI based on user actions

### 2.2 Usage Graph

The `UsageGraph` custom control visualizes process usage over time. It features:

- A line graph showing hours per day
- Configurable time range (day, week, month)
- Axis labels and scaling
- Title showing the selected process name

### 2.3 Process List

The process list is implemented using a standard `ListView` control with the following columns:

- Name: The name of the process
- PID: Process ID
- Dept: Category/department of the process
- Today: Time spent on the process today

The list supports selection, and when a process is selected, the graph and statistics panel are updated accordingly.

### 2.4 Category Selector

The `CategorySelector` custom control allows users to filter processes by category. It features:

- Buttons for each available category
- Visual indication of the selected category
- "All" option to show all processes
- Event handling for category selection

### 2.5 Statistics Panel

The `StatsPanel` custom control displays statistics about the selected process. It shows:

- Total time spent today
- Idle time
- Active time

## 3. Data Model

### 3.1 ProcessData

The `ProcessData` class represents a single process with its usage data. It contains:

- Basic information (name, PID, department)
- Time data (time spent today, historical data)
- Methods for calculating various time metrics

## 4. User Interaction Flow

1. **Application Start**
   - The application loads with sample process data
   - The process list displays all processes
   - The graph is empty until a process is selected
   - The stats panel shows zero values

2. **Process Selection**
   - User selects a process from the list
   - The graph updates to show the selected process's usage over time
   - The stats panel updates to show the selected process's statistics

3. **Category Filtering**
   - User selects a category from the category selector
   - The process list filters to show only processes in the selected category
   - If no process is selected in the filtered list, the graph and stats panel are reset

## 5. User Interface Design

The user interface is designed to be intuitive and informative, with the following layout:

- Top Left: Usage graph showing historical data
- Top Right: Category selector for filtering processes
- Bottom Left: Process list showing all or filtered processes
- Bottom Right: Statistics panel showing usage statistics

## 6. Implementation Details

### 6.1 Graphics Rendering

The `UsageGraph` control uses GDI+ for rendering the graph. It calculates the appropriate scaling based on the data and draws:

- Axes with labels
- Data points
- Lines connecting data points
- Border and title

### 6.2 Event Handling

The application uses events to handle user interactions:

- `ProcessListView_SelectedIndexChanged`: Triggered when a process is selected in the list
- `CategorySelector_CategorySelected`: Triggered when a category is selected

### 6.3 Data Generation

For demonstration purposes, the application generates sample data with:

- A list of processes with different categories
- Random historical data for each process
- Simulated active and idle times 