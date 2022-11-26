// <copyright file="ExternalServiceUnavailableException.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.BLL.Exceptions
{
    /// <summary>
    /// Generic exception that covers all communication issues to external services.
    /// </summary>
    [Serializable]
    public sealed class ExternalServiceUnavailableException : Exception, ISerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalServiceUnavailableException"/> class.
        /// </summary>
        /// <param name="message">Message that describes what happened.</param>
        /// <param name="innerException">Actual exception occurred.</param>
        public ExternalServiceUnavailableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private ExternalServiceUnavailableException()
        {
        }
    }
}
