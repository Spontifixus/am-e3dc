using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace AM.E3dc.Rscp.Connectivity
{
    /// <summary>
    /// Provides an interface for the standard .NET <see cref="NetworkStream"/>
    /// so that can be mocked for testing purposes.
    /// </summary>
    internal interface INetworkStream
    {
        /// <inheritdoc cref="NetworkStream.DataAvailable" />
        bool DataAvailable { get; }

        /// <inheritdoc cref="NetworkStream.ReadAsync(Memory{byte}, CancellationToken)" />
        ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken);

        /// <inheritdoc cref="NetworkStream.WriteAsync(ReadOnlyMemory{byte}, CancellationToken)" />
        ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken);
    }
}
