using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace AM.E3dc.Rscp.Connectivity
{
    /// <summary>
    /// This class wraps the standard <see cref="TcpClient"/> so it can be mocked
    /// for testing purposes.
    /// </summary>
    internal class TcpClientWrapper : ITcpClient
    {
        private readonly TcpClient tcpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpClientWrapper"/> class.
        /// </summary>
        /// <param name="tcpClient">The <see cref="TcpClient"/> being wrapped by this class.</param>
        internal TcpClientWrapper(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
        }

        /// <inheritdoc cref="TcpClient.ConnectAsync(IPAddress, int)"/>
        public Task ConnectAsync(IPAddress address, int port) => this.tcpClient.ConnectAsync(address, port);

        /// <inheritdoc cref="TcpClient.Close()"/>
        public void Close() => this.tcpClient.Close();

        /// <inheritdoc cref="TcpClient.Dispose()"/>
        public void Dispose() => this.tcpClient.Dispose();

        /// <inheritdoc cref="TcpClient.GetStream()"/>
        public NetworkStream GetStream() => this.tcpClient.GetStream();
    }
}
