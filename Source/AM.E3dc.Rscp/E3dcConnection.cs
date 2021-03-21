using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using AM.E3dc.Rscp.Crypto;
using AM.E3dc.Rscp.Data;
using Microsoft.Extensions.Logging;
using Stateless;

namespace AM.E3dc.Rscp
{
    /// <summary>
    /// Use an instance of this class to interact with your E3/DC solar power station.
    /// </summary>
    public sealed class E3dcConnection : IDisposable
    {
        private readonly object syncRoot = new object();
        private readonly ILogger<E3dcConnection> logger;
        private readonly ITcpClient tcpClient;
        private readonly ICryptoProvider cryptoProvider;

        private ConnectionState connectionState = ConnectionState.Disconnected;

        /// <summary>
        /// Initializes a new instance of the <see cref="E3dcConnection"/> class.
        /// </summary>
        /// <param name="logger">An instance of the logger.</param>
        public E3dcConnection(ILogger<E3dcConnection> logger)
            : this(logger, new TcpClientWrapper(new TcpClient { NoDelay = true }), new E3dcAes256CryptoProvider())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="E3dcConnection"/> class.
        /// </summary>
        /// <param name="logger">An instance of the logger.</param>
        /// <param name="tcpClient">An instance of a TCP client for communication.</param>
        /// <param name="cryptoProvider">An instance of the crypto provider for RSCP encryption.</param>
        /// <remarks>
        /// This constructor is to be used by unit tests, so the default implementation
        /// of the .NET <see cref="TcpClient"/> can be replaced by a mock.
        /// </remarks>
        internal E3dcConnection(ILogger<E3dcConnection> logger, ITcpClient tcpClient, ICryptoProvider cryptoProvider)
        {
            this.logger = logger;
            this.logger?.LogTrace(".ctor()");

            using var scope = this.logger?.BeginScope("Initialize");

            this.tcpClient = tcpClient;
            this.cryptoProvider = cryptoProvider;
        }

        /// <summary>
        /// This enum contains the possible connection states for this enum.
        /// </summary>
        private enum ConnectionState
        {
            Disconnected,
            Connecting,
            Connected
        }

        /// <summary>
        /// Gets a flag indicating whether this instance is currently connected to an E3/DC unit.
        /// </summary>
        /// <value>If set to <c>true</c> the instance is connected.</value>
        public bool IsConnected => this.connectionState == ConnectionState.Connected;

        /// <summary>
        /// Connects to the E3DC power station, but does not authorize.
        /// </summary>
        /// <param name="endpoint">The IP-endpoint of the E3/DC power station.</param>
        /// <param name="rscpPassword">The RSCP password as configured in the E3/DC unit.</param>
        /// <returns>A task that completes, once the connection was established.</returns>
        /// <remarks>
        /// The endpoint consists of the IP-address of the E3/DC power station as well as the RSCP-port. The latter is 5033 for a single power station and 5034 for a power station cluster.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if this instance is already connected to an E3/DC unit.</exception>
        public async Task ConnectAsync(IPEndPoint endpoint, string rscpPassword)
        {
            lock (this.syncRoot)
            {
                this.logger?.LogTrace(".ConnectAsync({endpoint}, {rscpPassword})", endpoint, "****"); // No, I'm not gonna log a password.

                if (this.connectionState != ConnectionState.Disconnected)
                {
                    this.logger?.LogWarning("Cannot connect to the E3/DC power station right now (State: {connectionState}).", this.connectionState);
                    throw new InvalidOperationException("The instance is already connected to an E3/DC power station or currently initializing a connection.");
                }

                this.connectionState = ConnectionState.Connecting;
            }

            this.logger?.LogDebug("Connecting to {endpoint}...", endpoint);
            await this.tcpClient.ConnectAsync(endpoint.Address, endpoint.Port);
            this.logger?.LogInformation("Connected to {endpoint}.", endpoint);

            this.cryptoProvider.SetPassword(rscpPassword);

            this.connectionState = ConnectionState.Connected;
        }

        /// <summary>
        /// Sends an <see cref="RscpFrame"/> to the E3/DC power station and waits for a response.
        /// </summary>
        /// <param name="frame">The frame to be sent to the power station.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to stop the ongoing operation.</param>
        /// <returns>A task that completes once the frame was answered and that contains the answer of the power station.</returns>
        /// <exception cref="InvalidOperationException">Thrown if this instance is not connected to an E3/DC unit.</exception>
        public async Task<RscpFrame> SendAsync(RscpFrame frame, CancellationToken cancellationToken = default)
        {
            this.logger?.LogTrace(".SendAsync(RscpFrame)");

            this.logger.LogDebug("Serializing frame...");
            var frameData = frame.GetBytes();

            this.logger.LogDebug("Encrypting frame...");
            var encryptedFrame = this.cryptoProvider.Encrypt(frameData);

            this.logger?.LogDebug("Writing {byteCount} bytes to the stream...", encryptedFrame.Length);
            var stream = this.tcpClient.GetStream();
            await stream.WriteAsync(encryptedFrame, cancellationToken);
            this.logger?.LogDebug("Successfully wrote {byteCount} bytes to the stream.", encryptedFrame.Length);

            this.logger?.LogDebug("Waiting for response...");
            while (!stream.DataAvailable)
            {
                await Task.Delay(100, cancellationToken);
            }

            this.logger?.LogDebug("Reading response from stream...");
            var buffer = new byte[4096];
            var offset = 0;
            do
            {
                var bytesRead = await stream.ReadAsync(buffer.AsMemory(offset, buffer.Length - offset), cancellationToken);
                offset += bytesRead;
            }
            while (stream.DataAvailable);
            this.logger?.LogDebug("Received {byteCount} bytes.");

            var responeData = buffer.Take(offset).ToArray();

            this.logger?.LogDebug("Decrypting response...");
            var decryptedFrame = this.cryptoProvider.Decrypt(responeData);

            this.logger?.LogDebug("Deserializing frame...");
            var responseFrame = new RscpFrame(decryptedFrame);

            return responseFrame;
        }

        /// <summary>
        /// Disconnects from the E3/DC power station.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if this instance is not connected to an E3/DC unit.</exception>
        public void Disconnect()
        {
        }

        /// <inheritdoc cref="IDisposable.Dispose()"/>
        public void Dispose()
        {
            this.logger?.LogTrace(".Dispose()");
            this.tcpClient?.Dispose();
        }
    }
}
