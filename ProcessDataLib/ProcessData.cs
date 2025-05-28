/**************************************************************************
 *                                                                        *
 *  File:        ProcessData.cs                                           *
 *  Copyright:   (c) 2025, Bostan Sorina-Gabriela, Brinza Denis-Stefan,   *
 *  Colibaba Rares-Andrei, Dodita Alexandru-Tomi                          *
 *  Authors:     Bostan Sorina-Gabriela, Brinza Denis-Stefan,             *
 *               Colibaba Rares-Andrei, Dodita Alexandru-Tomi             *
 *  University:  Universitatea Tehnica "Gheorghe Asachi" din Iasi         *
 *  Group:       1312A                                                    *
 *                                                                        *
 *  Description: Defineste clasa ProcessData, care actioneaza ca un       *
 *               model de date pentru a stoca informatii despre un        *
 *               proces individual monitorizat. Aceasta include numele    *
 *               procesului, PID-ul, categoria, timpul de utilizare       *
 *               pentru ziua curenta si o lista de date istorice.         *
 *               Clasa ofera si metode simple pentru a obtine timpul      *
 *               total, activ si inactiv estimat.                         *
 *                                                                        *
 *  This code is part of the TimeThread application, a process time       *
 *  tracking utility.                                                     *
 *                                                                        *
 **************************************************************************/
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