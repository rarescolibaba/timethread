/**************************************************************************
 *                                                                        *
 *  File:        Tests.cs                                                 *
 *  Copyright:   (c) 2025, Bostan Sorina-Gabriela, Brinza Denis-Stefan,   *
 *  Colibaba Rares-Andrei, Dodita Alexandru-Tomi                          *
 *                                                                        *
 *  Description: Acest fisier conține o suita de teste unitare            *
 *               pentru validarea funcționalitatilor cheie ale            *
 *               bibliotecilor din proiectul TimeThread, incluzand        *
 *               ProcessDataLib și ProcessMonitorLib. Testele acopera     *
 *               corectitudinea calculelor, gestionarea starii interne    *
 *               si comportamentul metodelor publice și private relevante *
 *                                                                        *
 *                                                                        *
 *  This code is part of the TimeThread project.                          *
 *                                                                        *
 **************************************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ProcessDataLib;
using UsageDataServiceLib;
using ProcessMonitorLib;
using System.Threading;
using System.Diagnostics;

namespace TestareaUnitatilor 
{
    [TestClass]
    public class ProjectTests
    {

        /// <summary>
        /// pentru a putea testa metode private(helper) de la ProcessMonitor
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private T InvokePrivateMethod<T>(object obj, string methodName, params object[] parameters)
        {
            var method = obj.GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (T)method.Invoke(obj, parameters);
        }
        /// <summary>
        /// verifica daca constructorul clasei ProcessData initializeaza corect proprietatea HistoricalData ca o listă goala si non-nula
        /// </summary>
        [TestMethod]
        public void Constructor_ShouldInitializeHistoricalData_AsEmptyList()
        {
            ProcessData pd = new ProcessData();

            Assert.IsNotNull(pd.HistoricalData, "HistoricalData nu ar trebui să fie null după inițializare.");
            Assert.AreEqual(0, pd.HistoricalData.Count, "HistoricalData ar trebui să fie o listă goală inițial.");
        }

        /// <summary>
        /// seteaza o valoare pentru TimeToday, apeleaza GetTotalTime() și verifica daca returneaza aceeasi valoare
        /// </summary>
                [TestMethod]
        public void GetTotalTime_ShouldReturn_TheSameAsTimeToday()
        {
            ProcessData pd = new ProcessData();
            TimeSpan expectedTime = TimeSpan.FromHours(2.5);
            pd.TimeToday = expectedTime;

            TimeSpan actualTime = pd.GetTotalTime();

            Assert.AreEqual(expectedTime, actualTime, "GetTotalTime ar trebui să returneze valoarea TimeToday.");
        }

        /// <summary>
        /// seteaza TimeToday la o valoare usor de calculat calculeaza valoarea așteptata(70 de minute), apeleaza GetActiveTime() si compara rezultatul cu valoarea asteptata
        /// </summary>
        [TestMethod]
        public void GetActiveTime_WhenTimeTodayIsSet_ShouldReturn70PercentOfTimeToday()
        {
            ProcessData pd = new ProcessData();
            TimeSpan timeToday = TimeSpan.FromMinutes(100); 
            pd.TimeToday = timeToday;
            TimeSpan expectedActiveTime = TimeSpan.FromMinutes(70);
            TimeSpan actualActiveTime = pd.GetActiveTime();
            Assert.AreEqual(expectedActiveTime, actualActiveTime, "Calculul timpului activ (70%) este incorect.");
        }

        /// <summary>
        /// verifica daca metoda GetActiveTime returneaza TimeSpan.Zero atunci cand proprietatea TimeToday este TimeSpan.Zero
        /// </summary>
        [TestMethod]
        public void GetActiveTime_WhenTimeTodayIsZero_ShouldReturnZero()
        {
            ProcessData pd = new ProcessData();
            pd.TimeToday = TimeSpan.Zero;

            TimeSpan actualActiveTime = pd.GetActiveTime();

            Assert.AreEqual(TimeSpan.Zero, actualActiveTime, "Timpul activ ar trebui să fie zero când TimeToday este zero.");
        }


        /// <summary>
        /// testeaza metoda GetIdleTime calculeaza și returneaza corect 30% din valoarea proprietatii TimeToday
        /// </summary>
        [TestMethod]
        public void GetIdleTime_WhenTimeTodayIsSet_ShouldReturn30PercentOfTimeToday()
        {
            ProcessData pd = new ProcessData();
            TimeSpan timeToday = TimeSpan.FromMinutes(100); 
            pd.TimeToday = timeToday;

            TimeSpan expectedIdleTime = TimeSpan.FromMinutes(30);

            TimeSpan actualIdleTime = pd.GetIdleTime();

            Assert.AreEqual(expectedIdleTime, actualIdleTime, "Calculul timpului inactiv (30%) este incorect.");
        }


        /// <summary>
        /// verifica daca metoda GetIdleTime returneaza TimeSpan.Zero atunci cand proprietatea TimeToday este TimeSpan.Zero
        /// </summary>
        [TestMethod]
        public void GetIdleTime_WhenTimeTodayIsZero_ShouldReturnZero()
        {
            ProcessData pd = new ProcessData();
            pd.TimeToday = TimeSpan.Zero;

            TimeSpan actualIdleTime = pd.GetIdleTime();
            Assert.AreEqual(TimeSpan.Zero, actualIdleTime, "Timpul inactiv ar trebui să fie zero când TimeToday este zero.");
        }

        /// <summary>
        /// testeaza functionalitatea de baza a proprietatilor publice ale clasei ProcessData
        /// </summary>
        [TestMethod]
        public void Properties_CanBeSetAndGet_Correctly()
        {
            ProcessData pd = new ProcessData();
            string expectedName = "TestProcess";
            int expectedPid = 1234;
            string expectedDepartment = "Testing";
            TimeSpan expectedTimeToday = TimeSpan.FromHours(1);
            var expectedHistoricalData = new List<KeyValuePair<DateTime, double>>
            {
                new KeyValuePair<DateTime, double>(DateTime.Today.AddDays(-1), 2.5)
            };
            pd.Name = expectedName;
            pd.PID = expectedPid;
            pd.Department = expectedDepartment;
            pd.TimeToday = expectedTimeToday;
            pd.HistoricalData = expectedHistoricalData;

            Assert.AreEqual(expectedName, pd.Name, "Proprietatea Name nu a fost setată/obținută corect.");
            Assert.AreEqual(expectedPid, pd.PID, "Proprietatea PID nu a fost setată/obținută corect.");
            Assert.AreEqual(expectedDepartment, pd.Department, "Proprietatea Department nu a fost setată/obținută corect.");
            Assert.AreEqual(expectedTimeToday, pd.TimeToday, "Proprietatea TimeToday nu a fost setată/obținută corect.");
            Assert.AreSame(expectedHistoricalData, pd.HistoricalData, "Proprietatea HistoricalData nu a fost setată/obținută corect (verificare referință).");
            Assert.AreEqual(1, pd.HistoricalData.Count, "Numărul de elemente din HistoricalData este incorect.");
        }

        /// <summary>
        /// testeaza metoda GetSystemUptime cand se ofera o data valida a pornirii
        /// verifica daca timpul de functionare returnat este pozitiv
        /// </summary>
        [TestMethod]
        public void GetSystemUptime_ValidBootTime_ReturnsPositiveTimeSpan()
        {
            DateTime bootTime = DateTime.Now.AddHours(-2);
            var uptime = ProcessMonitor.GetSystemUptime(bootTime);

            Assert.IsTrue(uptime.TotalMinutes > 0);
        }

        /// <summary>
        /// testeaza metoda GetSystemUptime cu o valoare invalida (DateTime.MinValue)
        /// verifica daca este returnat un TimeSpan egal cu zero
        /// </summary>
        [TestMethod]
        public void GetSystemUptime_InvalidBootTime_ReturnsZero()
        {
            var uptime = ProcessMonitor.GetSystemUptime(DateTime.MinValue);

            Assert.AreEqual(TimeSpan.Zero, uptime);
        }

        /// <summary>
        /// testeaza metoda GetProcessCategory pentru un proces cunoscut ("chrome")
        /// se asteapta ca metoda sa returneze categoria corecta ("Entertainment")
        /// </summary>
        [TestMethod]
        public void GetProcessCategory_KnownProcess_ReturnsCorrectCategory()
        {
            var monitor = new ProcessMonitor();
            var category = InvokePrivateMethod<string>(monitor, "GetProcessCategory", "chrome");

            Assert.AreEqual("Entertainment", category);
        }

        /// <summary>
        /// testează metoda GetProcessCategory pentru un proces necunoscut ("unknownapp")
        /// se asteapta ca metoda sa returneze categoria implicita ("Other")
        /// </summary>
        [TestMethod]
        public void GetProcessCategory_UnknownProcess_ReturnsOther()
        {
            var monitor = new ProcessMonitor();
            var category = InvokePrivateMethod<string>(monitor, "GetProcessCategory", "unknownapp");

            Assert.AreEqual("Other", category);
        }

        [TestMethod]
        public void GetLastBootUpTime_ShouldReturnRecentDate()
        {
            DateTime bootTime = ProcessMonitor.GetLastBootUpTime();

            Assert.IsTrue((DateTime.Now - bootTime).TotalHours < 48);
        }

        /// <summary>
        /// verifica daca GetActiveTime calculează corect procentajul
        /// pentru valori TimeToday foarte mici (sub un minut)
        /// </summary>
        [TestMethod]
        public void GetActiveTime_WithSmallTimeToday_CalculatesPercentageCorrectly()
        {
            ProcessData pd = new ProcessData();
            TimeSpan timeToday = TimeSpan.FromSeconds(10); 
            pd.TimeToday = timeToday;
            TimeSpan expectedActiveTime = TimeSpan.FromSeconds(7);
            TimeSpan actualActiveTime = pd.GetActiveTime();
            Assert.AreEqual(expectedActiveTime, actualActiveTime, "Calculul timpului activ pentru valori mici este incorect.");
        }

        /// <summary>
        /// verifica daca GetIdleTime calculează corect procentajul
        /// pentru valori TimeToday foarte mici (sub un minut)
        /// </summary>
        [TestMethod]
        public void GetIdleTime_WithSmallTimeToday_CalculatesPercentageCorrectly()
        {
            ProcessData pd = new ProcessData();
            TimeSpan timeToday = TimeSpan.FromSeconds(10);
            pd.TimeToday = timeToday;
            TimeSpan expectedIdleTime = TimeSpan.FromSeconds(3);
            TimeSpan actualIdleTime = pd.GetIdleTime();

            Assert.AreEqual(expectedIdleTime, actualIdleTime, "Calculul timpului inactiv pentru valori mici este incorect.");
        }

        /// <summary>
        /// testeaza comportamentul la setarea proprietatilor string cu valoarea null
        /// </summary>
        [TestMethod]
        public void StringProperties_CanBeSetToNull()
        {
            ProcessData pd = new ProcessData();
            pd.Name = null;
            pd.Department = null;
            Assert.IsNull(pd.Name, "Proprietatea Name ar trebui să poată fi null.");
            Assert.IsNull(pd.Department, "Proprietatea Department ar trebui să poată fi null.");
        }

        /// <summary>
        /// testeaza ce se intampla daca HistoricalData este setat la null si apoi accesat (desi constructorul previne asta)
        /// </summary>
        [TestMethod]
        public void HistoricalData_WhenSetToNull_DoesNotThrowOnPropertyAccess()
        {
            ProcessData pd = new ProcessData();

            pd.HistoricalData = null; 
            Assert.IsNull(pd.HistoricalData, "HistoricalData ar trebui să fie null după setarea explicită la null.");
        }

        /// <summary>
        /// verifica daca metoda GetProcessData returneaza o lista de procese monitorizate care nu este nula si contine cel putin un element
        /// </summary>
        [TestMethod]
        public void GetProcessData_ShouldReturnTrackedProcesses()
        {
            var monitor = new ProcessMonitor();
            Thread.Sleep(6000); 

            var processes = monitor.ProcessData;

            Assert.IsNotNull(processes);
            Assert.IsTrue(processes.Count > 0);
        }


        /// <summary>
        /// verifica daca metoda SetProcessCategory actualizeaza corect categoria (departmentul) unui proces deja monitorizat
        /// </summary>
        [TestMethod]
        public void SetProcessCategory_ShouldUpdateExistingProcessDepartment()
        {
            var monitor = new ProcessMonitor();
            Thread.Sleep(6000); 

            var processes = monitor.ProcessData;
            var firstProcess = processes.FirstOrDefault();

            if (firstProcess != null)
            {
                string originalCategory = firstProcess.Department;
                monitor.SetProcessCategory(firstProcess.Name, "TestCategory");

                var updatedProcess = monitor.ProcessData.FirstOrDefault(p => p.PID == firstProcess.PID);
                Assert.AreEqual("TestCategory", updatedProcess.Department);
            }
        }

        /// <summary>
        /// verifica daca metoda privata GetProcessStartTime returneaza data curenta (sau apropiata de aceasta) pentru un proces invalid
        /// </summary>

        [TestMethod]
        public void GetProcessStartTime_InvalidProcess_ReturnsNow()
        {
            var monitor = new ProcessMonitor();
            Process nonExistentProcess = new Process();
            nonExistentProcess.StartInfo.FileName = "notepad.exe";

            Process currentValidProcess = Process.GetCurrentProcess();
            DateTime validStartTime = DateTime.MinValue;
            Exception exWhenValid = null;
            try
            {
                validStartTime = InvokePrivateMethod<DateTime>(monitor, "GetProcessStartTime", currentValidProcess);
            }
            catch (Exception ex) { exWhenValid = ex; }

            Assert.IsNull(exWhenValid, "GetProcessStartTime nu ar trebui să arunce excepție pentru un proces valid.");
            Assert.AreNotEqual(DateTime.MinValue, validStartTime, "StartTime pentru un proces valid nu ar trebui să fie MinValue.");
            Assert.IsTrue(validStartTime <= DateTime.Now, "StartTime pentru un proces valid nu poate fi în viitor.");
            Process unstartedProcess = new Process();


            DateTime fallbackTime = DateTime.MinValue;
            DateTime timeBeforeInvoke = DateTime.Now; 
            Exception exWhenUnstarted = null;
            try
            {
                fallbackTime = InvokePrivateMethod<DateTime>(monitor, "GetProcessStartTime", unstartedProcess);
            }
            catch (Exception ex) { exWhenUnstarted = ex; }

            DateTime timeAfterInvoke = DateTime.Now; 

            Assert.IsTrue(fallbackTime >= timeBeforeInvoke && fallbackTime <= timeAfterInvoke,
                          $"Fallback time {fallbackTime} nu este în intervalul așteptat [{timeBeforeInvoke} - {timeAfterInvoke}]. Excepție internă (dacă a fost): {exWhenUnstarted?.Message}");
        }

        /// <summary>
        /// verifica daca metoda MonitorProcesses nu adauga duplicate in lista proceselor monitorizate la apeluri consecutive
        /// </summary>
        [TestMethod]
        public void MonitorProcesses_ShouldNotAddDuplicateProcesses()
        {
            var monitor = new ProcessMonitor();
            Thread.Sleep(6000);

            var processes1 = monitor.ProcessData;
            Thread.Sleep(6000);
            var processes2 = monitor.ProcessData;

            var pids1 = processes1.Select(p => p.PID).ToHashSet();
            var pids2 = processes2.Select(p => p.PID).ToHashSet();

            CollectionAssert.AreEquivalent(pids1.ToList(), pids2.ToList(), "Nu ar trebui să se adauge duplicate la monitorizare.");
        }




    }
}