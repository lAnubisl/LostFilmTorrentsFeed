// <copyright file="ValidationResult.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.BLL.Models
{
    /// <summary>
    /// Describes validation result.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        internal ValidationResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="property">Property that has an error.</param>
        /// <param name="message">Error message.</param>
        internal ValidationResult(string property, string message)
        {
            this.Errors[property] = message;
        }

        /// <summary>
        /// Gets list of model properties and error messages.
        /// </summary>
        public Dictionary<string, string> Errors { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets a value indicating whether is the validation was successful or not.
        /// </summary>
        public bool IsValid => this.Errors == null || this.Errors.Count == 0;

        /// <summary>
        /// Gets <see cref="ValidationResult"/> without any errors.
        /// </summary>
        internal static ValidationResult Ok => new();

        /// <summary>
        /// Gets an instance of <see cref="ValidationResult"/> with predefined error message.
        /// </summary>
        /// <param name="property">Property that has an error.</param>
        /// <param name="message">Error message.</param>
        /// <param name="objects">Parameters for string.format.</param>
        /// <returns>Instance of <see cref="ValidationResult"/>.</returns>
        internal static ValidationResult Fail(string property, string message, params string[] objects)
            => new(
                property,
                string.Format(message, new string[] { property }
                .Union(objects ?? Array.Empty<string>()).ToArray()));

        /// <summary>
        /// Gets an instance of <see cref="ValidationResult"/> with predefined error message.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Instance of <see cref="ValidationResult"/>.</returns>
        internal static ValidationResult Fail(string message) => new("model", message);

        /// <summary>
        /// Sets validation error to <see cref="ValidationResult"/>.
        /// </summary>
        /// <param name="property">Model property name.</param>
        /// <param name="message">Model property validation message.</param>
        internal void SetError(string property, string message)
        {
            this.Errors[property] = message;
        }
    }
}
