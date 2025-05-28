using System;

namespace Common
{
    /// <summary>
    /// Base exception for the application.
    /// </summary>
    public class AppException : Exception
    {
        public AppException(string message, Exception inner = null) : base(message, inner) { }
    }

    public class ProcessMonitorException : AppException
    {
        public ProcessMonitorException(string message, Exception inner = null) : base(message, inner) { }
    }

    public class UsageDataServiceException : AppException
    {
        public UsageDataServiceException(string message, Exception inner = null) : base(message, inner) { }
    }

    public class UIException : AppException
    {
        public UIException(string message, Exception inner = null) : base(message, inner) { }
    }
}