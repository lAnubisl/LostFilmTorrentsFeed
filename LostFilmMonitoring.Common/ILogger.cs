// <copyright file="ILogger.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.Common;

/// <summary>
/// Logger interface.
/// </summary>
public interface ILogger
{
    /// <summary>
    /// Creates new logger scope.
    /// </summary>
    /// <param name="name">Scope name.</param>
    /// <returns>New logger with new scope.</returns>
    ILogger CreateScope(string name);

    /// <summary>
    /// Log Debug message.
    /// </summary>
    /// <param name="message">message.</param>
    void Debug(string message);

    /// <summary>
    /// Log Info message.
    /// </summary>
    /// <param name="message">message.</param>
    void Info(string message);

    /// <summary>
    /// Log Warning message.
    /// </summary>
    /// <param name="message">message.</param>
    void Warning(string message);

    /// <summary>
    /// Log Error message.
    /// </summary>
    /// <param name="message">message.</param>
    void Error(string message);

    /// <summary>
    /// Log fatal message.
    /// </summary>
    /// <param name="message">message.</param>
    void Fatal(string message);

    /// <summary>
    /// Log exception.
    /// </summary>
    /// <param name="message">message.</param>
    /// <param name="ex">exception.</param>
    void Log(string message, Exception ex);

    /// <summary>
    /// Log exception.
    /// </summary>
    /// <param name="ex">exception.</param>
    void Log(Exception ex);
}
