using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using AM.E3dc.Rscp.Connectivity;
using AM.E3dc.Rscp.Crypto;
using AM.E3dc.Rscp.Data;
using Microsoft.Extensions.Logging;

namespace AM.E3dc.Rscp
{
    /// <summary>
    /// Use an instance of this class to interact with your E3/DC solar power station.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentlySynchronizedField", Justification = "For the logger, synchronization does not matter, for the connection state everything is fine as it is.")]
    public sealed class E3dcConnection : IDisposable
    {
        private readonly object syncRoot = new object();
        private readonly ILogger<E3dcConnection> logger;
        private readonly ICryptoProvider cryptoProvider;
        private readonly ITcpClient tcpClient;
        private readonly SemaphoreSlim sendQueueSemaphore = new SemaphoreSlim(1);
        private ConnectionState connectionState = ConnectionState.New;

        /// <summary>
        /// Initializes a new instance of the <see cref="E3dcConnection"/> class.
        /// </summary>
        /// <param name="logger">An instance of the logger.</param>
        public E3dcConnection(ILogger<E3dcConnection> logger = null)
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

            this.tcpClient = tcpClient;
            this.cryptoProvider = cryptoProvider;
        }

        /// <summary>
        /// This enum contains the possible connection states for this enum.
        /// </summary>
        private enum ConnectionState
        {
            New,
            Connecting,
            Connected,
            Disconnecting,
            Disconnected
        }

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

                if (this.connectionState != ConnectionState.New)
                {
                    this.logger?.LogWarning("Cannot connect to the E3/DC power station (State: {connectionState}).", this.connectionState);
                    throw new InvalidOperationException($"The instance is or was already connected to an E3/DC power station (State: {this.connectionState}).");
                }

                this.connectionState = ConnectionState.Connecting;
            }

            this.logger?.LogDebug("Connecting to {endpoint}...", endpoint);
            await this.tcpClient.ConnectAsync(endpoint.Address, endpoint.Port).ConfigureAwait(false);
            this.logger?.LogInformation("Connected to {endpoint}.", endpoint);

            this.logger?.LogDebug("Configuring encryption...");
            this.cryptoProvider.SetPassword(rscpPassword);

            this.logger?.LogInformation("Connected to E3/DC power station at {endpoint}.", endpoint);
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
            lock (this.syncRoot)
            {
                this.logger?.LogTrace(".SendAsync(RscpFrame)");

                if (this.connectionState != ConnectionState.Connected)
                {
                    this.logger?.LogWarning("Cannot communicate with E3/DC power station (State: {connectionState}).", this.connectionState);
                    throw new InvalidOperationException($"The instance is not connected to an E3/DC power station (State: {this.connectionState}).");
                }
            }

            await this.sendQueueSemaphore.WaitAsync(cancellationToken);
            try
            {
                this.logger?.LogTrace(".SendFrameAsync(RscpFrame)");

                this.logger?.LogDebug("Serializing frame...");
                var frameData = frame.GetBytes();

                this.logger?.LogDebug("Encrypting frame...");
                var encryptedFrame = this.cryptoProvider.Encrypt(frameData);

                this.logger?.LogDebug("Writing {byteCount} bytes to the stream...", encryptedFrame.Length);
                var stream = this.tcpClient.GetStream();
                await stream.WriteAsync(encryptedFrame, cancellationToken).ConfigureAwait(false);
                this.logger?.LogDebug("Successfully wrote {byteCount} bytes to the stream.", encryptedFrame.Length);

                this.logger?.LogDebug("Waiting for response...");
                while (!stream.DataAvailable)
                {
                    await Task.Delay(100, cancellationToken).ConfigureAwait(false);
                }

                this.logger?.LogDebug("Reading response from stream...");
                byte[] receivedData;
                int receivedDataLength = 0;
                await using (var memoryStream = new MemoryStream())
                {
                    do
                    {
                        var buffer = new byte[1024];
                        var bytesRead = await stream.ReadAsync(buffer.AsMemory(), cancellationToken).ConfigureAwait(false);
                        receivedDataLength += bytesRead;
                        await memoryStream.WriteAsync(buffer, cancellationToken);
                    }
                    while (stream.DataAvailable);

                    receivedData = memoryStream.ToArray();
                }

                this.logger?.LogDebug("Received {byteCount} bytes.");

                var responseData = receivedData[..receivedDataLength];

                this.logger?.LogDebug("Decrypting response...");
                var decryptedFrame = this.cryptoProvider.Decrypt(responseData);

                this.logger?.LogDebug("Deserializing frame...");
                var responseFrame = new RscpFrame(decryptedFrame);

                return responseFrame;
            }
            finally
            {
                this.sendQueueSemaphore.Release();
            }
        }

        /// <summary>
        /// Disconnects from the E3/DC power station.
        /// </summary>
        /// <param name="cancellationToken">A token to provide a timeout for this class.</param>
        /// <exception cref="InvalidOperationException">Thrown if this instance is not connected to an E3/DC unit.</exception>
        /// <returns>A <see cref="Task"/> that completes once the disconnection was successful.</returns>
        public async Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            lock (this.syncRoot)
            {
                this.logger?.LogTrace(".Disconnect()");

                if (this.connectionState != ConnectionState.Connecting && this.connectionState != ConnectionState.Connected)
                {
                    this.logger?.LogWarning("Cannot disconnect from the E3/DC power station (State: {connectionState}).", this.connectionState);
                    throw new InvalidOperationException("The instance is not connected to an E3/DC power station.");
                }

                this.connectionState = ConnectionState.Disconnecting;
            }

            this.logger?.LogDebug("Waiting for ongoing requests to complete...");
            await this.sendQueueSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            this.logger?.LogDebug("No more requests waiting. Disconnecting...");
            try
            {
                this.logger?.LogDebug("Closing TCP connection...");
                this.tcpClient.Close();
                this.tcpClient.Dispose();
                this.logger?.LogDebug("TCP connection closed.");

                this.logger?.LogInformation("Connection to E3/DC power station closed.");
            }
            finally
            {
                this.sendQueueSemaphore.Release();
            }

            this.connectionState = ConnectionState.Disconnected;
        }

        /// <inheritdoc cref="IDisposable.Dispose()"/>
        public async void Dispose()
        {
            this.logger?.LogTrace(".Dispose()");
            await this.DisconnectAsync().ConfigureAwait(false);
        }
    }
}
