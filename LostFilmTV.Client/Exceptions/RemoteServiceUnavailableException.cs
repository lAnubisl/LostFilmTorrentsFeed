namespace LostFilmTV.Client.Exceptions;

using System;
using System.Runtime.Serialization;

/// <summary>
/// RemoteServiceUnavailableException.
/// </summary>
[Serializable]
public class RemoteServiceUnavailableException : Exception, ISerializable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RemoteServiceUnavailableException"/> class.
    /// </summary>
    /// <param name="innerException">Instance of <see cref="Exception"/>.</param>
    public RemoteServiceUnavailableException(Exception innerException)
        : base("Remote Service Unavailable", innerException)
    {
    }
}
