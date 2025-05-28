using System;
using System.IO;

namespace Common
{
    public static class Logger
    {
        private static readonly string LogFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "app_error.log");

        public static void Log(Exception ex, string context)
        {
            try
            {
                string msg = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {context}: {ex}\r\n";
                File.AppendAllText(LogFilePath, msg);
                System.Diagnostics.Debug.WriteLine(msg);
            }
            catch
            {
                // nu facem nimic...
            }
        }
    }
}