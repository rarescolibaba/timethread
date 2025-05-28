// AUTORI: Bostan Sorina-Gabirela, Brinza Denis-Stefan, Colibaba Rares-Andrei, Dodita Alexandru-Tomi
// UNIVERSITATEA TEHNICA GHEORGHE ASACHI, GRUPA 1312A
// Functionalitate:
// Clasa ce reprezinta un proces si datele sale de utilizare pentru aplicatia Process Time Tracker.
// ---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessDataLib
{
    /// <summary>
    /// Reprezinta un proces cu datele sale de utilizare
    /// </summary>
    public class ProcessData
    {
        /// <summary>
        /// Numele procesului
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Id-ul procesului
        /// </summary>
        public int PID { get; set; }

        /// <summary>
        /// Categoria procesului (Games, Learning, etc.)
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Timpul petrecut azi pe proces
        /// </summary>
        public TimeSpan TimeToday { get; set; }

        /// <summary>
        /// Date istorice pentru proces (ore pe zi)
        /// </summary>
        public List<KeyValuePair<DateTime, double>> HistoricalData { get; set; }

        /// <summary>
        /// Constructor pentru ProcessData
        /// </summary>
        public ProcessData()
        {
            HistoricalData = new List<KeyValuePair<DateTime, double>>();
        }

        /// <summary>
        /// Returneaza timpul total petrecut pe proces
        /// </summary>
        /// <returns>Timp total ca TimeSpan</returns>
        public TimeSpan GetTotalTime()
        {
            // In a real implementation, this would calculate the total time
            return TimeToday;
        }

        /// <summary>
        /// Returneaza timpul activ petrecut pe proces
        /// </summary>
        /// <returns>Timp activ ca TimeSpan</returns>
        public TimeSpan GetActiveTime()
        {
            // In a real implementation, this would calculate the active time
            // For now, we'll assume 70% of total time is active
            return TimeSpan.FromTicks((long)(TimeToday.Ticks * 0.7));
        }

        /// <summary>
        /// Returneaza timpul inactiv petrecut pe proces
        /// </summary>
        /// <returns>Timp inactiv ca TimeSpan</returns>
        public TimeSpan GetIdleTime()
        {
            // In a real implementation, this would calculate the idle time
            // For now, we'll assume 30% of total time is idle
            return TimeSpan.FromTicks((long)(TimeToday.Ticks * 0.3));
        }
    }
}