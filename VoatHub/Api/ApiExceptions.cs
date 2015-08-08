using System;
using Windows.Web.Http;

namespace VoatHub.Api
{
    /// <summary>
    /// Thrown when attempting to access authenticated API when unauthenticated. 
    /// </summary>
    public class UnauthenticatedException : Exception
    {
        public UnauthenticatedException() : base() { }
        public UnauthenticatedException(string message) : base(message) { }
        public UnauthenticatedException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Uniformed exception encapsulating all serialization errors.
    /// <para>Wraps exceptions like <see cref="JsonException"/> and maybe future xml exceptions.</para>
    /// </summary>
    public class SerializationException : Exception
    {
        public SerializationException() : base() { }
        public SerializationException(string message) : base(message) { }
        public SerializationException(string message, Exception inner) : base(message, inner) { }
    }
}