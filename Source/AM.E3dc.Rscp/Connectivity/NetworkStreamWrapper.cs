using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace AM.E3dc.Rscp.Connectivity
{
    /// <summary>
    /// This class wraps the standard <see cref="NetworkStream"/> so it can be mocked
    /// for testing purposes.
    /// </summary>
    internal class NetworkStreamWrapper : INetworkStream
    {
        private readonly NetworkStream networkStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkStreamWrapper"/> class.
        /// </summary>
        /// <param name="networkStream">The <see cref="NetworkStream"/> being wrapped by this class.</param>
        internal NetworkStreamWrapper(NetworkStream networkStream)
        {
            this.networkStream = networkStream;
        }

        /// <inheritdoc cref="NetworkStream.DataAvailable" />
        public bool DataAvailable => this.networkStream.DataAvailable;

        /// <inheritdoc cref="NetworkStream.ReadAsync(Memory{byte}, CancellationToken)" />
        public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken) => this.networkStream.ReadAsync(buffer, cancellationToken);

        /// <inheritdoc cref="NetworkStream.WriteAsync(ReadOnlyMemory{byte}, CancellationToken)" />
        public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken) => this.networkStream.WriteAsync(buffer, cancellationToken);
    }
}
