// <copyright file="LoggerConfiguration.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2021 Alexander Panfilenok
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the 'Software'), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>

namespace LostFilmMonitoring.Common
{
    /// <summary>
    /// Logger configuration helper.
    /// </summary>
    public static class LoggerConfiguration
    {
        /// <summary>
        /// Configure logger.
        /// </summary>
        /// <param name="minLogLevel">Min log level. Can be:Trace|Debug|Info|Warn|Error|Fatal|Off.</param>
        /// <param name="maxLogLevel">Max log level. Can be:Trace|Debug|Info|Warn|Error|Fatal|Off.</param>
        public static void ConfigureLogger(string minLogLevel, string maxLogLevel)
        {
            var config = new NLog.Config.LoggingConfiguration();
            var target = new NLog.Targets.ConsoleTarget()
            {
                Name = "ConsoleTarget",
                Encoding = System.Text.Encoding.Unicode,
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
