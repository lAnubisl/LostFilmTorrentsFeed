// <copyright file="Logger.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2023 Alexander Panfilenok
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

namespace LostFilmMonitoring.BLL
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// This logger implements <see cref="Common.ILogger"/> using <see cref="ILoggerFactory"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2254:Template should be a static expression", Justification = "Custom properties are not ready.")]
    public class Logger : Common.ILogger
    {
        private readonly ILoggerFactory loggerFactory;
        private ILogger logger;
        private string scopeName;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="loggerFactory">An instance of ILoggerFactory that will be used to generate internal logger.</param>
        public Logger(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.scopeName = "default";
            this.logger = loggerFactory.CreateLogger(this.scopeName);
        }

        /// <inheritdoc/>
        public Common.ILogger CreateScope(string name)
        {
            var scope = new Logger(this.loggerFactory);
            scope.SetScope(name);
            return scope;
        }

        /// <inheritdoc/>
        public void Debug(string message) => this.logger.LogDebug(this.WrapMessage(message));

        /// <inheritdoc/>
        public void Error(string message) => this.logger.LogError(this.WrapMessage(message));

        /// <inheritdoc/>
        public void Fatal(string message) => this.logger.LogCritical(this.WrapMessage(message));

        /// <inheritdoc/>
        public void Info(string message) => this.logger.LogInformation(this.WrapMessage(message));

        /// <inheritdoc/>
        public void Log(Exception ex) => this.logger.LogCritical(ex, this.WrapMessage("Exception occurred."));

        /// <inheritdoc/>
        public void Log(string message, Exception ex) => this.logger.LogCritical(ex, this.WrapMessage(message));

        /// <inheritdoc/>
        public void Warning(string message) => this.logger.LogWarning(this.WrapMessage(message));

        private void SetScope(string name)
        {
            this.logger = this.loggerFactory.CreateLogger(name);
            this.scopeName = name;
        }

        private string WrapMessage(string message) => $"{this.scopeName}: {message}";
    }
}
