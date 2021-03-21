using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace AM.E3dc.Rscp
{
    /// <summary>
    /// Provides an interface for the standard .NET <see cref="TcpClient"/>
    /// so that can be mocked for testing purposes.
    /// </summary>
    internal interface ITcpClient
    {
        /// <inheritdoc cref="TcpClient.Connected"/>
        bool Connected { get; }

        /// <inheritdoc cref="TcpClient.ConnectAsync(IPAddress, int)"/>
        Task ConnectAsync(IPAddress address, int port);

        /// <inheritdoc cref="TcpClient.Close()"/>
        void Close();

        /// <inheritdoc cref="TcpClient.Dispose()"/>
        void Dispose();

        /// <inheritdoc cref="TcpClient.GetStream()"/>
        NetworkStream GetStream();
    }
}
