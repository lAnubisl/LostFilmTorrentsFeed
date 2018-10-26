namespace LostFilmMonitoring.Common
{
    public static class LoggerConfiguration
    {
        public static void ConfigureLogger(string application, string connectionString)
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
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, target);
            NLog.LogManager.Configuration = config;
        }
    }
}