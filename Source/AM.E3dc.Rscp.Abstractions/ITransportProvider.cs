using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AM.E3dc.Rscp.Abstractions
{
    /// <summary>
    /// Interface description for a transport provider that contains the
    /// actual communication logic used to talk to the E3/DC power station.
    /// </summary>
    public interface ITransportProvider : IDisposable
    {
        /// <summary>
        /// Gets a flag indicating whether this transport provider is connected.
        /// </summary>
        /// <value><c>true</c> if this instance is connected; <c>false</c> otherwise.</value>
        bool IsConnected { get; }

        /// <summary>
        /// Connects to the power station without authenticating.
        /// </summary>
        /// <param name="endpoint">The ip address and port of the E3/DC power station.</param>
        /// <exception cref="TransportException">Thrown if the transport provider is already connected to an E3/DC power station.</exception>
        /// <returns>A task that completes once the connection was successful.</returns>
        Task ConnectAsync(IPEndPoint endpoint);

        /// <summary>
        /// Disconnects from the power station.
        /// </summary>
        /// <remarks>
        /// If this method is invoked, pending requests will be aborted.
        /// </remarks>
        /// <exception cref="TransportException">Thrown if the transport provider is not connected to the E3/DC power station.</exception>
        void Disconnect();

        /// <summary>
        /// Sets the frame to the E3/DC unit and waits for its answer.
        /// </summary>
        /// <param name="data">The data to be sent to the E3/DC power station.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to abort the task.</param>
        /// <returns>The answer from the E3/DC power station.</returns>
        /// <remarks>The transport provider needs to be connected to send frames to the E3/DC power station.</remarks>
        /// <exception cref="TransportException">Thrown if the transport provider is not connected to the E3/DC power station.</exception>
        Task<byte[]> SendAsync(byte[] data, CancellationToken cancellationToken);
    }
}
