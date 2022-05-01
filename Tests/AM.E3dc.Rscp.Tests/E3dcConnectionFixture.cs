using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AM.E3dc.Rscp.Connectivity;
using AM.E3dc.Rscp.Crypto;
using AM.E3dc.Rscp.Data;
using AM.E3dc.Rscp.Data.Values;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace AM.E3dc.Rscp.Tests
{
    public class E3dcConnectionFixture
    {
        private const string RscpPassword = "abc123";
        private const int E3dcPort = 5033;
        private static readonly IPAddress E3dcAddress = IPAddress.Parse("192.168.0.2");

        private readonly ITcpClient tcpClient = Substitute.For<ITcpClient>();
        private readonly ICryptoProvider cryptoProvider = Substitute.For<ICryptoProvider>();
        private readonly INetworkStream networkSteam = Substitute.For<INetworkStream>();
        private readonly E3dcConnection subject;

        public E3dcConnectionFixture()
        {
            this.subject = new E3dcConnection(null, this.tcpClient, this.cryptoProvider);

            this.cryptoProvider.Encrypt(Arg.Any<byte[]>()).Returns(args => args[0]);
            this.cryptoProvider.Decrypt(Arg.Any<byte[]>()).Returns(args => args[0]);

            this.tcpClient.GetStream().Returns(this.networkSteam);

            this.networkSteam.WriteAsync(Arg.Any<ReadOnlyMemory<byte>>(), Arg.Any<CancellationToken>()).Returns(ValueTask.CompletedTask);
        }

        [Fact]
        public async Task CanSendSingleFrame()
        {
            var value = new RscpVoid(RscpTag.BAT_DATA);
            var frame = new RscpFrame();
            frame.Add(value);
            var frameBytes = frame.GetBytes();

            this.networkSteam.DataAvailable.Returns(true, false);
            this.networkSteam.ReadAsync(Arg.Any<Memory<byte>>(), Arg.Any<CancellationToken>())
                .Returns(
                    args =>
                    {
                        var output = (Memory<byte>)args[0];
                        frameBytes.CopyTo(output);
                        return frameBytes.Length;
                    });

            this.tcpClient.ConnectAsync(Arg.Is(E3dcAddress), Arg.Is(E3dcPort)).Returns(Task.CompletedTask);

            await this.subject.ConnectAsync(new IPEndPoint(E3dcAddress, E3dcPort), RscpPassword);
            await this.subject.SendAsync(frame);
            await this.subject.DisconnectAsync();

            _ = this.networkSteam.Received(2).DataAvailable;
            Received.InOrder(
                async () =>
                {
                    await this.tcpClient.Received(1).ConnectAsync(Arg.Is(E3dcAddress), Arg.Is(E3dcPort));
                    this.cryptoProvider.Received(1).SetPassword(Arg.Is(RscpPassword));

                    this.cryptoProvider.Received(1).Encrypt(Arg.Is<byte[]>(a => a.SequenceEqual(frameBytes)));

                    this.tcpClient.Received(1).GetStream();
                    await this.networkSteam.Received(1).WriteAsync(Arg.Is<ReadOnlyMemory<byte>>(a => a.ToArray().SequenceEqual(frameBytes)), Arg.Any<CancellationToken>());

                    await this.networkSteam.Received(1).ReadAsync(Arg.Any<Memory<byte>>(), Arg.Any<CancellationToken>());
                    this.cryptoProvider.Received(1).Decrypt(Arg.Is<byte[]>(a => a.SequenceEqual(frameBytes)));

                    this.tcpClient.Received(1).Close();
                });
        }

        [Fact]
        public async Task CanSendMultipleFramesInRapidSequence()
        {
            var value = new RscpVoid(RscpTag.BAT_DATA);
            var frame = new RscpFrame();
            frame.Add(value);
            var frameBytes = frame.GetBytes();

            this.networkSteam.DataAvailable.Returns(true, false, true, false, true, false);
            this.networkSteam.ReadAsync(Arg.Any<Memory<byte>>(), Arg.Any<CancellationToken>())
                .Returns(
                    args =>
                    {
                        var output = (Memory<byte>)args[0];
                        frameBytes.CopyTo(output);
                        return frameBytes.Length;
                    });

            this.tcpClient.ConnectAsync(Arg.Is(E3dcAddress), Arg.Is(E3dcPort)).Returns(Task.CompletedTask);

            await this.subject.ConnectAsync(new IPEndPoint(E3dcAddress, E3dcPort), RscpPassword);
#pragma warning disable 4014

            // There's no await needed here. We want to test that sending two messages in parallel works fine.
            this.subject.SendAsync(frame);
            this.subject.SendAsync(frame);

#pragma warning restore 4014
            await this.subject.SendAsync(frame);
            await this.subject.DisconnectAsync();

            _ = this.networkSteam.Received(6).DataAvailable;
            Received.InOrder(
                async () =>
                {
                    await this.tcpClient.Received(1).ConnectAsync(Arg.Is(E3dcAddress), Arg.Is(E3dcPort));
                    this.cryptoProvider.Received(1).SetPassword(Arg.Is(RscpPassword));
                    for (int i = 0; i < 3; i++)
                    {
                        this.cryptoProvider.Received(1).Encrypt(Arg.Is<byte[]>(a => a.SequenceEqual(frameBytes)));

                        this.tcpClient.Received(1).GetStream();
                        await this.networkSteam.Received(1).WriteAsync(Arg.Is<ReadOnlyMemory<byte>>(a => a.ToArray().SequenceEqual(frameBytes)), Arg.Any<CancellationToken>());

                        await this.networkSteam.Received(1).ReadAsync(Arg.Any<Memory<byte>>(), Arg.Any<CancellationToken>());
                        this.cryptoProvider.Received(1).Decrypt(Arg.Is<byte[]>(a => a.SequenceEqual(frameBytes)));
                    }

                    this.tcpClient.Received(1).Close();
                });
        }

        [Fact]
        public async Task ConnectThrowsExceptionIfAlreadyConnected()
        {
            this.tcpClient.ConnectAsync(Arg.Is(E3dcAddress), Arg.Is(E3dcPort)).Returns(Task.CompletedTask);

#pragma warning disable 4014

            // There's no await needed here. We want to test that connecting twice produces the expected exception.
            this.subject.ConnectAsync(new IPEndPoint(E3dcAddress, E3dcPort), RscpPassword);
            var action = new Func<Task>(async () => await this.subject.ConnectAsync(new IPEndPoint(E3dcAddress, E3dcPort), RscpPassword));

#pragma warning restore 4014

            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task DisconnectThrowsExceptionIfNotConnected()
        {
            var action = new Func<Task>(async () => await this.subject.DisconnectAsync());
            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task EnqueuedFramesAreSentBeforeDisconnection()
        {
            var value = new RscpVoid(RscpTag.BAT_DATA);
            var frame = new RscpFrame();
            frame.Add(value);
            var frameBytes = frame.GetBytes();

            this.networkSteam.DataAvailable.Returns(true, false, true, false, true, false);
            this.networkSteam.ReadAsync(Arg.Any<Memory<byte>>(), Arg.Any<CancellationToken>())
                .Returns(
                    args =>
                    {
                        var output = (Memory<byte>)args[0];
                        frameBytes.CopyTo(output);
                        return frameBytes.Length;
                    });

            this.tcpClient.ConnectAsync(Arg.Is(E3dcAddress), Arg.Is(E3dcPort)).Returns(Task.CompletedTask);

            await this.subject.ConnectAsync(new IPEndPoint(E3dcAddress, E3dcPort), RscpPassword);
#pragma warning disable 4014

            // There's no await needed here. We want to test that sending two messages in parallel works fine.
            this.subject.SendAsync(frame);
            this.subject.SendAsync(frame);
            this.subject.SendAsync(frame);

#pragma warning restore 4014
            await this.subject.DisconnectAsync();

            _ = this.networkSteam.Received(6).DataAvailable;
            Received.InOrder(
                async () =>
                {
                    await this.tcpClient.Received(1).ConnectAsync(Arg.Is(E3dcAddress), Arg.Is(E3dcPort));
                    this.cryptoProvider.Received(1).SetPassword(Arg.Is(RscpPassword));
                    for (int i = 0; i < 3; i++)
                    {
                        this.cryptoProvider.Received(1).Encrypt(Arg.Is<byte[]>(a => a.SequenceEqual(frameBytes)));

                        this.tcpClient.Received(1).GetStream();
                        await this.networkSteam.Received(1).WriteAsync(Arg.Is<ReadOnlyMemory<byte>>(a => a.ToArray().SequenceEqual(frameBytes)), Arg.Any<CancellationToken>());

                        await this.networkSteam.Received(1).ReadAsync(Arg.Any<Memory<byte>>(), Arg.Any<CancellationToken>());
                        this.cryptoProvider.Received(1).Decrypt(Arg.Is<byte[]>(a => a.SequenceEqual(frameBytes)));
                    }

                    this.tcpClient.Received(1).Close();
                });
        }
    }
}
