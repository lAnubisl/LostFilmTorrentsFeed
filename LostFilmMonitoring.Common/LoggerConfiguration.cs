namespace LostFilmMonitoring.Common
{
    public static class LoggerConfiguration
    {
        public static void ConfigureLogger(string application, string connectionString, string minLogLevel, string maxLogLevel)
        {
            var config = new NLog.Config.LoggingConfiguration();
            var target = new NLog.Targets.DatabaseTarget()
            {
                Name = "DatabaseTarget",
                KeepConnection = true,
                DBProvider = "MySql.Data.MySqlClient.MySqlConnection, MySqlConnector",
                ConnectionString = connectionString,
                CommandText = @"insert into log (Application, Logged, Level, Message, Logger, Exception, ActivityId) 
                values (@Application, @Logged, @Level, @Message, @Logger, @Exception, @ActivityId);",
                CommandType = System.Data.CommandType.Text,
            };
            target.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Application", application));
            target.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Logged", "${date}"));
            target.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Level", "${level}"));
            target.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Logged", "${date}"));
            target.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Message", "${message}"));
            target.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Logger", "${logger}"));
            target.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Exception", "${exception:tostring}"));
            target.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@ActivityId", "${activityid}"));
            config.AddTarget("DatabaseTarget", target);
            config.AddRule(Map(minLogLevel), Map(maxLogLevel), target);
            NLog.LogManager.Configuration = config;
        }

        private static NLog.LogLevel Map(string level)
        {
            switch (level)
            {
                case "Trace": return NLog.LogLevel.Trace;
                case "Debug": return NLog.LogLevel.Debug;
                case "Info": return NLog.LogLevel.Info;
                case "Warn": return NLog.LogLevel.Warn;
                case "Error": return NLog.LogLevel.Error;
                case "Fatal": return NLog.LogLevel.Fatal;
                default: return NLog.LogLevel.Off;
            }
        }
    }
}