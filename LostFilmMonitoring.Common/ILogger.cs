using System;

namespace LostFilmMonitoring.Common
{
    public interface ILogger
    {
        ILogger CreateScope(string name);
        void Debug(string message);
        void Info(string message);
        void Warning(string message);
        void Error(string message);
        void Fatal(string message);
        void Log(Exception ex);
    }
}
