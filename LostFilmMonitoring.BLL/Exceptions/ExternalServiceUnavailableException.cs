namespace LostFilmMonitoring.BLL.Exceptions;

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
