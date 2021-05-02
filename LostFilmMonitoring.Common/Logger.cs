// <copyright file="Logger.cs" company="Alexander Panfilenok">
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
    using System;

    /// <summary>
    /// Logger implementation.
    /// </summary>
    public class Logger : ILogger
    {
        private readonly string scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="name">Scope name.</param>
        public Logger(string name)
        {
            this.scope = name;
        }

        /// <inheritdoc/>
        public ILogger CreateScope(string name)
        {
            return new Logger(name);
        }

        /// <inheritdoc/>
        public void Debug(string message)
        {
            Console.WriteLine($"DEBUG|{this.scope}|{message}");
        }

        /// <inheritdoc/>
        public void Error(string message)
        {
            Console.Error.WriteLine($"ERROR|{this.scope}|{message}");
        }

        /// <inheritdoc/>
        public void Fatal(string message)
        {
            Console.Error.WriteLine($"FATAL|{this.scope}|{message}");
        }

        /// <inheritdoc/>
        public void Info(string message)
        {
            Console.WriteLine($"INFO|{this.scope}|{message}");
        }

        /// <inheritdoc/>
        public void Log(Exception ex)
        {
            this.Fatal(ex.GetType() + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
        }

        /// <inheritdoc/>
        public void Warning(string message)
        {
            Console.WriteLine($"WARNING|{this.scope}|{message}");
        }
    }
}
