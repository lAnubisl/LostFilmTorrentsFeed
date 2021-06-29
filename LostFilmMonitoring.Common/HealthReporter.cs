// <copyright file="HealthReporter.cs" company="Alexander Panfilenok">
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
    /// Responsible for storing Unhealthy Message.
    /// </summary>
    public class HealthReporter
    {
        private static Exception unhealthyException;
        private static string unhealthyMessage;
        private static DateTime unhealthyMessageDate;

        /// <summary>
        /// Store new message.
        /// </summary>
        /// <param name="message">Unhealthy message.</param>
        /// <param name="exception">Exception.</param>
        public void ReportUnhealthy(string message, Exception exception)
        {
            unhealthyMessageDate = DateTime.UtcNow;
            unhealthyMessage = message;
            unhealthyException = exception;
        }

        /// <summary>
        /// Get last unhealthy message.
        /// </summary>
        /// <returns>Unhealthy message.</returns>
        public (string message, Exception exception) GetLastUnhealthyDetails()
        {
            return unhealthyMessageDate.AddMinutes(1) > DateTime.UtcNow ? (unhealthyMessage, unhealthyException) : (null, null);
        }
    }
}
