// <copyright file="HealthCheck.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.Web
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using LostFilmMonitoring.Common;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    /// <summary>
    /// Responsible for Health Checking.
    /// </summary>
    public class LastExceptionHealthCheck : IHealthCheck
    {
        private readonly HealthReporter healthReporter;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastExceptionHealthCheck"/> class.
        /// </summary>
        /// <param name="healthReporter">HealthReporter.</param>
        public LastExceptionHealthCheck(HealthReporter healthReporter)
        {
            this.healthReporter = healthReporter;
        }

        /// <inheritdoc/>
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var (message, exception) = this.healthReporter.GetLastUnhealthyDetails();
            var response = message == null
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Degraded(
                    description: message,
                    exception: exception,
                    data: exception != null ? new Dictionary<string, object>() { { "StackTrace", exception?.StackTrace } } : null);
            return Task.FromResult(response);
        }
    }
}
