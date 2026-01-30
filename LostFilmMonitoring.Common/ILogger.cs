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
