namespace LostFilmMonitoring.BLL.Exceptions;

using System.Runtime.Serialization;

/// <summary>
/// Generic exception that covers all communication issues to external services.
/// </summary>
[Serializable]
public sealed class ExternalServiceUnavailableException : Exception
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

    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalServiceUnavailableException"/> class.
    /// </summary>
    /// <param name="message">Message that describes what happened.</param>
    public ExternalServiceUnavailableException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalServiceUnavailableException"/> class.
    /// </summary>
    private ExternalServiceUnavailableException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalServiceUnavailableException"/> class.
    /// </summary>
    /// <param name="info">The SerializationInfo object.</param>
    /// <param name="context">The StreamingContext object.</param>
    private ExternalServiceUnavailableException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
