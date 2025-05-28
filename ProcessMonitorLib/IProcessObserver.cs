using ProcessDataLib;

namespace ProcessMonitorLib
{
    /// <summary>
    /// Interfata Observer pentru a primi evenimentele proceselor de la ProcessMonitor
    /// </summary>
    public interface IProcessObserver
    {
        void OnProcessAdded(ProcessData process);
        void OnProcessRemoved(int pid);
        void OnProcessUpdated(ProcessData process);
    }
}