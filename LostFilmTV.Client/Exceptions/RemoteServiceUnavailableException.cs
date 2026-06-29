namespace LostFilmTV.Client.Exceptions;

using System;
using System.Runtime.Serialization;

/// <summary>
/// RemoteServiceUnavailableException.
/// </summary>
[Serializable]
public class RemoteServiceUnavailableException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RemoteServiceUnavailableException"/> class.
    /// </summary>
    /// <param name="innerException">Instance of <see cref="Exception"/>.</param>
    public RemoteServiceUnavailableException(Exception innerException)
        : base("Remote Service Unavailable", innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoteServiceUnavailableException"/> class.
    /// </summary>
    /// <param name="message">Message that describes what happened.</param>
    public RemoteServiceUnavailableException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoteServiceUnavailableException"/> class.
    /// </summary>
    /// <param name="message">Message that describes what happened.</param>
    /// <param name="innerException">Actual exception occurred.</param>
    public RemoteServiceUnavailableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoteServiceUnavailableException"/> class.
    /// </summary>
    /// <param name="info">The SerializationInfo object.</param>
    /// <param name="context">The StreamingContext object.</param>
    protected RemoteServiceUnavailableException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
