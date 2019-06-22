using System;

namespace LostFilmMonitoring.Common
{
    public class Logger : ILogger
    {
        private readonly NLog.Logger _logger;
        public Logger(string name)
        {
            _logger = NLog.LogManager.GetLogger(name);
        }

        public ILogger CreateScope(string name)
        {
            return new Logger(name);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Log(Exception ex)
        {
            _logger.Error(ex.GetType() + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
        }

        public void Warning(string message)
        {
            _logger.Warn(message);
        }
    }
}