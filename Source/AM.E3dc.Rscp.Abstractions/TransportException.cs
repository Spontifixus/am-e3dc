using System;

namespace AM.E3dc.Rscp.Abstractions
{
    /// <summary>
    /// This exception gets thrown if the connection used by the transport provider failed.
    /// </summary>
    public class TransportException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransportException"/> message.
        /// </summary>
        /// <param name="message">The message of the exception.</param>
        public TransportException(string message)
            : base(message)
        {
        }
    }
}
