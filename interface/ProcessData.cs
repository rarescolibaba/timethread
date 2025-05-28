using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace @interface
{
    /// <summary>
    /// Represents a single process with its usage data
    /// </summary>
    public class ProcessData
    {
        /// <summary>
        /// Name of the process
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Process ID
        /// </summary>
        public int PID { get; set; }

        /// <summary>
        /// Category of the process (Games, Learning, etc.)
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Time spent on the process today
        /// </summary>
        public TimeSpan TimeToday { get; set; }

        /// <summary>
        /// Historical time data for the process (hours per day)
        /// </summary>
        public List<KeyValuePair<DateTime, double>> HistoricalData { get; set; }

        /// <summary>
        /// Constructor for ProcessData
        /// </summary>
        public ProcessData()
        {
            HistoricalData = new List<KeyValuePair<DateTime, double>>();
        }

        /// <summary>
        /// Returns the total time spent on the process
        /// </summary>
        /// <returns>Total time as TimeSpan</returns>
        public TimeSpan GetTotalTime()
        {
            // In a real implementation, this would calculate the total time
            return TimeToday;
        }

        /// <summary>
        /// Returns the active time spent on the process
        /// </summary>
        /// <returns>Active time as TimeSpan</returns>
        public TimeSpan GetActiveTime()
        {
            // In a real implementation, this would calculate the active time
            // For now, we'll assume 70% of total time is active
            return TimeSpan.FromTicks((long)(TimeToday.Ticks * 0.7));
        }

        /// <summary>
        /// Returns the idle time spent on the process
        /// </summary>
        /// <returns>Idle time as TimeSpan</returns>
        public TimeSpan GetIdleTime()
        {
            // In a real implementation, this would calculate the idle time
            // For now, we'll assume 30% of total time is idle
            return TimeSpan.FromTicks((long)(TimeToday.Ticks * 0.3));
        }
    }
}