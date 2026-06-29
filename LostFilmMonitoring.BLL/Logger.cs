namespace LostFilmMonitoring.BLL;

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
    public void Debug(string message)
    {
        if (this.logger.IsEnabled(LogLevel.Debug))
        {
            this.logger.LogDebug(this.WrapMessage(message));
        }
    }

    /// <inheritdoc/>
    public void Error(string message)
    {
        if (this.logger.IsEnabled(LogLevel.Error))
        {
            this.logger.LogError(this.WrapMessage(message));
        }
    }

    /// <inheritdoc/>
    public void Fatal(string message)
    {
        if (this.logger.IsEnabled(LogLevel.Critical))
        {
            this.logger.LogCritical(this.WrapMessage(message));
        }
    }

    /// <inheritdoc/>
    public void Info(string message)
    {
        if (this.logger.IsEnabled(LogLevel.Information))
        {
            this.logger.LogInformation(this.WrapMessage(message));
        }
    }

    /// <inheritdoc/>
    public void Log(Exception ex)
    {
        if (this.logger.IsEnabled(LogLevel.Critical))
        {
            this.logger.LogCritical(ex, this.WrapMessage("Exception occurred."));
        }
    }

    /// <inheritdoc/>
    public void Log(string message, Exception ex)
    {
        if (this.logger.IsEnabled(LogLevel.Critical))
        {
            this.logger.LogCritical(ex, this.WrapMessage(message));
        }
    }

    /// <inheritdoc/>
    public void Warning(string message)
    {
        if (this.logger.IsEnabled(LogLevel.Warning))
        {
            this.logger.LogWarning(this.WrapMessage(message));
        }
    }

    private void SetScope(string name)
    {
        this.logger = this.loggerFactory.CreateLogger(name);
        this.scopeName = name;
    }

    private string WrapMessage(string message) => $"{this.scopeName}: {message}";
}
