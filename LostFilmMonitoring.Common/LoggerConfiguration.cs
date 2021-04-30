namespace LostFilmMonitoring.Common
{
    public static class LoggerConfiguration
    {
        public static void ConfigureLogger(string minLogLevel, string maxLogLevel)
        {
            var config = new NLog.Config.LoggingConfiguration();
            var target = new NLog.Targets.ConsoleTarget()
            {
                Name = "ConsoleTarget",
                Encoding = System.Text.Encoding.Unicode
            };
            config.AddTarget("ConsoleTarget", target);
            config.AddRule(Map(minLogLevel), Map(maxLogLevel), target);
            NLog.LogManager.Configuration = config;
        }

        private static NLog.LogLevel Map(string level)
        {
            return level switch
            {
                "Trace" => NLog.LogLevel.Trace,
                "Debug" => NLog.LogLevel.Debug,
                "Info" => NLog.LogLevel.Info,
                "Warn" => NLog.LogLevel.Warn,
                "Error" => NLog.LogLevel.Error,
                "Fatal" => NLog.LogLevel.Fatal,
                _ => NLog.LogLevel.Off,
            };
        }
    }
}
