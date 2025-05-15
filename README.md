# TimeThread – Process Time Tracker

## 1. Introduction

**Purpose:**  
TimeThread is a Windows Forms application that tracks and visualizes the time spent on various processes (apps) on your computer. It allows users to sort, filter, and analyze their app usage, helping them understand and optimize their time.

**Scope:**  
- Track running processes and time spent in each.
- Visualize usage history (hours/day for the last month).
- Filter by app categories (Games, Learning, Coding, Entertainment).
- View detailed stats for each process.

---

## 2. UML Diagram

```plaintext
+-------------------+         +-------------------+         +-------------------+
|   MainForm        |<>------>|   ProcessManager  |<>------>|   ProcessInfo     |
+-------------------+         +-------------------+         +-------------------+
| - processList     |         | - processes       |         | - Name            |
| - categoryFilter  |         |                   |         | - PID             |
| - chart           |         | + GetProcesses()  |         | - Category        |
| - statsPanel      |         | + SortByTime()    |         | - TimeSpent       |
|                   |         | + FilterByCat()   |         | - History[]       |
| + UpdateUI()      |         |                   |         |                   |
| + OnSelectProc()  |         +-------------------+         +-------------------+
| + OnSort()        |
| + OnFilter()      |
+-------------------+
```

---

## 3. System Features

### 3.1 Process List Panel
- **Description:** Displays all tracked processes with columns: Name, PID, Time Difference, Time Today.
- **Sorting:** Clickable column headers to sort by time spent.
- **Selection:** Clicking a process updates the graph and stats.

### 3.2 Usage Graph
- **Description:** Line chart showing hours/day for the selected process over the last month.
- **Interaction:** Updates when a process is selected.

### 3.3 Category Filter
- **Description:** Panel to select app categories (Games, Learning, Coding, Entertainment, All).
- **Function:** Filters the process list and graph.

### 3.4 Stats Panel
- **Description:** Shows all-time, today's total, idle, and active time for the selected process.

---

## 4. Classes and Methods

### 4.1 MainForm
- **Role:** Main UI controller.
- **Key Methods:**
  - `UpdateUI()`: Refreshes all UI elements.
  - `OnProcessSelected(ProcessInfo)`: Updates graph and stats for selected process.
  - `OnSort(column)`: Sorts process list.
  - `OnCategoryFilter(category)`: Filters process list and updates UI.

### 4.2 ProcessManager
- **Role:** Handles process tracking and data management.
- **Key Methods:**
  - `GetProcesses()`: Returns a list of current processes.
  - `SortByTime(List<ProcessInfo>, sortColumn)`: Sorts processes by specified column.
  - `FilterByCategory(List<ProcessInfo>, category)`: Filters processes by category.

### 4.3 ProcessInfo
- **Role:** Represents a tracked process.
- **Attributes:**
  - `Name`: Process name.
  - `PID`: Process ID.
  - `Category`: App category.
  - `TimeSpent`: Total time spent.
  - `History[]`: Array of daily usage for the last month.

---

## 5. SRS IEEE-Style Documentation

### 5.1 Functional Requirements
- The system shall display a list of running processes with time tracking.
- The system shall allow sorting of processes by time spent.
- The system shall allow filtering by app category.
- The system shall display a usage graph for the selected process.
- The system shall show detailed stats for the selected process.

### 5.2 Non-Functional Requirements
- The UI shall be responsive and update in real-time.
- The system shall persist usage data between sessions.
- The system shall be compatible with Windows 10+.

### 5.3 User Interface Requirements
- The main window shall contain a process list, usage graph, category filter, and stats panel.
- The process list shall support sorting and selection.
- The category filter shall support multiple categories.

---

## 6. How to Run

1. Open the solution in Visual Studio.
2. Build the project.
3. Run the application.

---

## 7. Future Work

- Add notifications for excessive usage.
- Export usage data to CSV.
- Add more categories and custom tags. 