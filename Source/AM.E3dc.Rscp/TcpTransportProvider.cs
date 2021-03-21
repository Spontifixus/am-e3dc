//using System;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Threading;
//using System.Threading.Tasks;
//using AM.E3dc.Rscp.Abstractions;
//using Microsoft.Extensions.Logging;

//namespace AM.E3dc.Rscp.Tcp
//{
//    /// <summary>
//    /// This class provides a tcp connection to connect to the E3/DC power station.
//    /// </summary>
//    public sealed class TcpClientWrapper2 : ITcpClient
//    {
//        private readonly ILogger<TcpTransportProvider> logger;
//        private readonly object syncRoot = new object();

//        private bool isConnecting;
//        private TcpClient tcpClient;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="TcpTransportProvider"/> class.
//        /// </summary>
//        /// <param name="logger">An instance of the logger.</param>
//        public TcpTransportProvider(ILogger<TcpTransportProvider> logger = null)
//        {
//            this.logger = logger;
//            this.logger?.LogTrace(".ctor()");
//        }

//        /// <inheritdoc cref="ITransportProvider.IsConnected"/>
//        public bool IsConnected => this.tcpClient != null && this.tcpClient.Connected;

//        /// <inheritdoc cref="ITransportProvider.ConnectAsync(IPEndPoint)"/>
//        public async Task ConnectAsync(IPEndPoint endpoint)
//        {
//            this.logger?.LogTrace(".ConnectAsync({endpoint})", endpoint);
//            using var scope = this.logger?.BeginScope("Connect");

//            lock (this.syncRoot)
//            {
//                if (this.IsConnected || this.isConnecting)
//                {
//                    const string alreadyConnectedMessage = "Cannot create a new connection. Transport provider is already connecting or connected.";
//                    this.logger?.LogWarning(alreadyConnectedMessage);
//                    throw new TransportException(alreadyConnectedMessage);
//                }

//                this.isConnecting = true;
//            }

//            if (this.tcpClient != null)
//            {
//                this.logger?.LogDebug("TcpClient in invalid state. Cleaning up before re-connecting...");
//                this.CleanupConnection();
//            }

//            this.logger?.LogDebug("Creating TcpClient...");
//            this.tcpClient = new TcpClient { NoDelay = true };

//            try
//            {
//                this.logger?.LogDebug("Connecting to {endpoint}...", endpoint);
//                await this.tcpClient.ConnectAsync(endpoint.Address, endpoint.Port);
//                this.logger?.LogInformation("Connected to {endpoint}.", endpoint);
//            }
//            catch (Exception exception)
//            {
//                var exceptionMessage = $"Connecting to the E3/DC unit failed. Reason: {exception.Message}";
//                this.logger?.LogError(exception, exceptionMessage);

//                this.Disconnect();

//                throw new TransportException(exceptionMessage);
//            }

//            this.isConnecting = false;
//        }

//        /// <inheritdoc cref="ITransportProvider.SendAsync"/>
//        public async Task<byte[]> SendAsync(byte[] data, CancellationToken cancellationToken)
//        {
//            this.logger?.LogTrace(".SendAsync(frame)");
//            using var scope = this.logger?.BeginScope("Send");

//            lock (this.syncRoot)
//            {
//                if (!this.IsConnected)
//                {
//                    const string notConnectedMessage = "Cannot send the frame. Transport provider is disconnected.";
//                    this.logger?.LogWarning(notConnectedMessage);
//                    throw new TransportException(notConnectedMessage);
//                }
//            }

//            try
//            {
//                this.logger?.LogDebug("Writing {byteCount} bytes to the stream...", data.Length);
//                var stream = this.tcpClient.GetStream();
//                await stream.WriteAsync(data, cancellationToken);
//                this.logger?.LogDebug("Successfully wrote {byteCount} bytes to the stream.", data.Length);

//                this.logger?.LogDebug("Waiting for response...");
//                while (!stream.DataAvailable)
//                {
//                    await Task.Delay(100, cancellationToken);
//                }

//                this.logger?.LogDebug("Reading response from stream...");
//                var buffer = new byte[4096];
//                var offset = 0;
//                do
//                {
//                    var bytesRead = await stream.ReadAsync(buffer.AsMemory(offset, buffer.Length - offset), cancellationToken);
//                    offset += bytesRead;
//                }
//                while (stream.DataAvailable);
//                this.logger?.LogDebug("Received {byteCount} bytes.");

//                return buffer.Take(offset).ToArray();
//            }
//            catch (SocketException socketException)
//            {
//                this.logger?.LogError(socketException, "Could not send the frame.");
//                throw new TransportException($"Could not send the frame. Reason: {socketException.Message}");
//            }
//            catch (Exception exception)
//            {
//                this.logger?.LogError(exception, "An unexpected error occurred.");
//                throw new TransportException($"An unexpected error occurred. Reason: {exception.Message}");
//            }
//        }

//        /// <inheritdoc cref="ITransportProvider.Disconnect"/>
//        public void Disconnect()
//        {
//            this.logger?.LogTrace(".Disconnect()");
//            using var scope = this.logger?.BeginScope("Disconnect");

//            lock (this.syncRoot)
//            {
//                if (this.tcpClient == null)
//                {
//                    const string alreadyDisconnectedMessage = "Cannot disconnect. Transport provider is already disconnected.";
//                    this.logger?.LogWarning(alreadyDisconnectedMessage);
//                    throw new TransportException(alreadyDisconnectedMessage);
//                }

//                this.CleanupConnection();
//            }
//        }

//        /// <inheritdoc cref="IDisposable.Dispose"/>
//        public void Dispose()
//        {
//            this.logger?.LogTrace(".Dispose()");
//            this.CleanupConnection();
//        }

//        private void CleanupConnection()
//        {
//            this.logger?.LogTrace(".CleanupConnection");
//            this.logger?.LogDebug("Cleaning up the remains of the TcpClient...");

//            if (this.tcpClient != null)
//            {
//                this.tcpClient.Close();
//                this.tcpClient.Dispose();
//                this.tcpClient = null;
//            }
//        }

//        //private async Task<RscpUserLevel> Authorize(string userName, string password)
//        //{
//        //    this.logger?.LogTrace(".Authorize({userName}, {password})", userName, "****"); // No. I won't log the password...
//        //    using var scope = this.logger?.BeginScope("Authorize");

//        //    this.logger?.LogInformation("Authorizing user '{userName}'...", userName);
//        //    var authFrame = new RscpFrame();
//        //    var authContainer = new RscpContainer(RscpTag.TAG_RSCP_REQ_AUTHENTICATION);
//        //    authContainer.Add(new RscpString(RscpTag.TAG_RSCP_AUTHENTICATION_USER, userName));
//        //    authContainer.Add(new RscpString(RscpTag.TAG_RSCP_AUTHENTICATION_PASSWORD, password));
//        //    authFrame.Add(authContainer);

//        //    var userLevel = RscpUserLevel.NotAuthorized;
//        //    try
//        //    {
//        //        this.logger?.LogDebug("Sending authentication frame...");
//        //        var authorizationResponse = await this.SendFrameAsync(authFrame);

//        //        this.logger?.LogDebug("Authentication response received.");
//        //        var authValue = (RscpUInt8)authorizationResponse.Values.First();
//        //        userLevel = (RscpUserLevel)authValue.Value;

//        //        if (userLevel != RscpUserLevel.NotAuthorized)
//        //        {
//        //            this.logger?.LogInformation("Authorization of user '{userName}' successful (UserLevel: {userLevel}).", userName, userLevel.ToString("G"));
//        //        }
//        //        else
//        //        {
//        //            this.logger?.LogWarning("Authorization of user '{userName}' failed.", userName);
//        //        }
//        //    }
//        //    catch (RscpException exception)
//        //    {
//        //        this.logger?.LogError(exception, "Authentication failed (Reason: {errorCode}).", exception.ErrorCode.ToString("G"));
//        //    }

//        //    return userLevel;
//        //}
//    }
//}
