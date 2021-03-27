using System;
using AM.E3dc.Rscp.Data.Values;
using FluentAssertions;
using FluentAssertions.Extensions;
using Force.Crc32;
using Xunit;

namespace AM.E3dc.Rscp.Data.Tests
{
    public class RscpFrameFixture
    {
        private readonly RscpFrame subject;
        private readonly DateTime now;

        public RscpFrameFixture()
        {
            this.now = DateTime.Now;

            this.subject = new RscpFrame { Timestamp = this.now };
        }

        [Fact]
        public void InitializesRscpFrameCorrectly()
        {
            this.subject.HasChecksum.Should().BeTrue();
            this.subject.ProtocolVersion.Should().Be(1);
            this.subject.Length.Should().Be(0);
            this.subject.Timestamp.Should().BeCloseTo(this.now);
            this.subject.HasError.Should().BeFalse();
            this.subject.GetErrors().Should().BeEmpty();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(8)]
        [InlineData(14)]
        [InlineData(15)]
        public void CanSetProtocolVersion(byte protocolVersion)
        {
            this.subject.ProtocolVersion = protocolVersion;

            this.subject.HasChecksum.Should().BeTrue();
            this.subject.ProtocolVersion.Should().Be(protocolVersion);
            this.subject.Length.Should().Be(0);
            this.subject.Timestamp.Should().BeCloseTo(this.now);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(16)]
        [InlineData(255)]
        public void FailsOnInvalidProtocolVersion(byte protocolVersion)
        {
            var action = new Action(() =>
                {
                    this.subject.ProtocolVersion = protocolVersion;
                });

            action.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Invalid protocol version! The protocol version must be between 1 and 15.");
            this.subject.HasChecksum.Should().BeTrue();
            this.subject.ProtocolVersion.Should().Be(1);
            this.subject.Length.Should().Be(0);
            this.subject.Timestamp.Should().BeCloseTo(this.now);
        }

        [Fact]
        public void CanSetHasChecksum()
        {
            this.subject.HasChecksum = false;

            this.subject.HasChecksum.Should().BeFalse();
            this.subject.ProtocolVersion.Should().Be(1);
            this.subject.Length.Should().Be(0);
            this.subject.Timestamp.Should().BeCloseTo(this.now);

            this.subject.HasChecksum = true;

            this.subject.HasChecksum.Should().BeTrue();
            this.subject.ProtocolVersion.Should().Be(1);
            this.subject.Length.Should().Be(0);
            this.subject.Timestamp.Should().BeCloseTo(this.now);
        }

        [Fact]
        public void CanSetTimestamp()
        {
            var timestamp = 13.June(1981)
                .At(12, 30, 15, 500, 250, 125);

            this.subject.Timestamp = timestamp;

            this.subject.HasChecksum.Should().BeTrue();
            this.subject.ProtocolVersion.Should().Be(1);
            this.subject.Length.Should().Be(0);
            this.subject.Timestamp.Should().BeCloseTo(timestamp, 0);
        }

        [Fact]
        public void CanGetBytes()
        {
            var expectedData = new byte[] { 0xE3, 0xDC, 0x00, 0x11, 0x3C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            // No need to write a fixed CRC into the result here,
            // as we don't want to test the CrC32 algorithm.
            Crc32Algorithm.ComputeAndWriteToEnd(expectedData);

            // We need to initialize the timestamp explicitly
            // for this test, because otherwise we'll need to
            // calculate the date's bytes here, too, and I'd
            // rather not do that.
            var testTicks = DateTime.UnixEpoch.Ticks + (60 * TimeSpan.TicksPerSecond) + 2;
            this.subject.Timestamp = new DateTime(testTicks);

            var result = this.subject.GetBytes();

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void CanAddAndRetrieveSingleValue()
        {
            var value = new RscpInt8(RscpTag.BAT_DATA, 0x7F);

            this.subject.Add(value);
            this.subject.HasError.Should().BeFalse();
            this.subject.TryGetValue<RscpInt8>(RscpTag.BAT_DATA, out var retrievedValue).Should().BeTrue();
            retrievedValue.Should().BeEquivalentTo(value);

            this.subject.HasError.Should().BeFalse();
            this.subject.GetErrors().Should().BeEmpty();

            var expectedData = new byte[] { 0xE3, 0xDC, 0x00, 0x11, 0x3C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC8, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x84, 0x03, 0x02, 0x01, 0x00, 0x7f, 0x00, 0x00, 0x00, 0x00 };

            // No need to write a fixed CRC into the result here,
            // as we don't want to test the CrC32 algorithm.
            Crc32Algorithm.ComputeAndWriteToEnd(expectedData);

            // We need to initialize the timestamp explicitly
            // for this test, because otherwise we'll need to
            // calculate the date's bytes here, too, and I'd
            // rather not do that.
            var testTicks = DateTime.UnixEpoch.Ticks + (60 * TimeSpan.TicksPerSecond) + 2;
            this.subject.Timestamp = new DateTime(testTicks);

            // No need to write a fixed CRC into the result here,
            // as we don't want to test the CrC32 algorithm.
            Crc32Algorithm.ComputeAndWriteToEnd(expectedData);

            var result = this.subject.GetBytes();

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void CanAddAndRetrieveMultipleValues()
        {
            var value1 = new RscpInt8(RscpTag.BAT_DATA, 0x7F);
            var value2 = new RscpInt8(RscpTag.RSCP_AUTHENTICATION, 0x7F);

            this.subject.Add(value1);
            this.subject.Add(value2);

            this.subject.HasError.Should().BeFalse();

            this.subject.TryGetValue<RscpInt8>(RscpTag.BAT_DATA, out var retrievedValue1).Should().BeTrue();
            retrievedValue1.Should().BeEquivalentTo(value1);

            this.subject.TryGetValue<RscpInt8>(RscpTag.RSCP_AUTHENTICATION, out var retrievedValue2).Should().BeTrue();
            retrievedValue2.Should().BeEquivalentTo(value2);

            this.subject.Values.Should().BeEquivalentTo(value1, value2);

            this.subject.HasError.Should().BeFalse();
            this.subject.GetErrors().Should().BeEmpty();

            var expectedData = new byte[] { 0xE3, 0xDC, 0x00, 0x11, 0x3C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC8, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x84, 0x03, 0x02, 0x01, 0x00, 0x7f, 0x01, 0x00, 0x80, 0x00, 0x02, 0x01, 0x00, 0x7f, 0x00, 0x00, 0x00, 0x00 };

            // No need to write a fixed CRC into the result here,
            // as we don't want to test the CrC32 algorithm.
            Crc32Algorithm.ComputeAndWriteToEnd(expectedData);

            // We need to initialize the timestamp explicitly
            // for this test, because otherwise we'll need to
            // calculate the date's bytes here, too, and I'd
            // rather not do that.
            var testTicks = DateTime.UnixEpoch.Ticks + (60 * TimeSpan.TicksPerSecond) + 2;
            this.subject.Timestamp = new DateTime(testTicks);

            // No need to write a fixed CRC into the result here,
            // as we don't want to test the CrC32 algorithm.
            Crc32Algorithm.ComputeAndWriteToEnd(expectedData);

            var result = this.subject.GetBytes();

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void CanAddAndRetrieveMultipleValuesWithTheSameTag()
        {
            var value1 = new RscpInt8(RscpTag.BAT_DATA, 0x7F);
            var value2 = new RscpInt8(RscpTag.BAT_DATA, 0x7F);

            this.subject.Add(value1);
            this.subject.Add(value2);

            this.subject.HasError.Should().BeFalse();

            this.subject.TryGetValue<RscpInt8>(RscpTag.BAT_DATA, out var retrievedValue1).Should().BeTrue();
            retrievedValue1.Should().BeEquivalentTo(value1, value2);

            this.subject.Values.Should().BeEquivalentTo(value1, value2);

            this.subject.HasError.Should().BeFalse();
            this.subject.GetErrors().Should().BeEmpty();

            var expectedData = new byte[] { 0xE3, 0xDC, 0x00, 0x11, 0x3C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC8, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x84, 0x03, 0x02, 0x01, 0x00, 0x7f, 0x00, 0x00, 0x84, 0x03, 0x02, 0x01, 0x00, 0x7f, 0x00, 0x00, 0x00, 0x00 };

            // No need to write a fixed CRC into the result here,
            // as we don't want to test the CrC32 algorithm.
            Crc32Algorithm.ComputeAndWriteToEnd(expectedData);

            // We need to initialize the timestamp explicitly
            // for this test, because otherwise we'll need to
            // calculate the date's bytes here, too, and I'd
            // rather not do that.
            var testTicks = DateTime.UnixEpoch.Ticks + (60 * TimeSpan.TicksPerSecond) + 2;
            this.subject.Timestamp = new DateTime(testTicks);

            // No need to write a fixed CRC into the result here,
            // as we don't want to test the CrC32 algorithm.
            Crc32Algorithm.ComputeAndWriteToEnd(expectedData);

            var result = this.subject.GetBytes();

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void AddThrowsExceptionIfValueTooLarge()
        {
            var value = new RscpByteArray(RscpTag.BAT_DATA, new byte[short.MaxValue]);
            this.subject.Add(value);

            var action = new Action(() => this.subject.Add(value));

            action.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Can't put the value into this frame because then it would be too long.");
        }

        [Fact]
        public void AddThrowsExceptionIfValueNull()
        {
            var action = new Action(() => this.subject.Add(null));

            action.Should()
                .Throw<ArgumentNullException>()
                .Where(e => e.ParamName == "value");
        }

        [Fact]
        public void TryGetValueDoesNotThrowIfNotFound()
        {
            var value = new RscpBool(RscpTag.BAT_CURRENT, false);
            this.subject.Add(value);

            var action = new Action(() =>
            {
                this.subject.TryGetValue<RscpBool>(RscpTag.BAT_CHARGE_CYCLES, out _);
                this.subject.TryGetValue<RscpBitfield>(RscpTag.BAT_CURRENT, out _);
            });

            action.Should().NotThrow();
        }

        [Fact]
        public void TryGetValueDoesNotThrowIfEmpty()
        {
            var action = new Action(() =>
            {
                this.subject.TryGetValue<RscpBool>(RscpTag.BAT_CHARGE_CYCLES, out _);
            });

            action.Should().NotThrow();
        }

        [Fact]
        public void TryGetValueReturnsFalseIfTypeMismatch()
        {
            var value = new RscpBool(RscpTag.BAT_CURRENT, false);
            this.subject.Add(value);

            this.subject.TryGetValue<RscpUInt16>(RscpTag.BAT_CURRENT, out _).Should().BeFalse();
        }

        [Fact]
        public void TryGetValueReturnsFalseIfTagMismatch()
        {
            var value = new RscpBool(RscpTag.BAT_CURRENT, false);
            this.subject.Add(value);

            this.subject.TryGetValue<RscpBool>(RscpTag.BAT_CHARGE_CYCLES, out _).Should().BeFalse();
        }

        [Fact]
        public void CanDetectAndGetErrorValue()
        {
            var value1 = new RscpBool(RscpTag.BAT_CURRENT, false);
            var value2 = new RscpError(RscpTag.BAT_CHARGE_CYCLES, RscpErrorCode.Again);

            this.subject.Add(value1);
            this.subject.Add(value2);

            this.subject.HasError.Should().BeTrue();
            this.subject.GetErrors().Should().NotBeNullOrEmpty();
            this.subject.GetErrors()[0].Should().BeEquivalentTo(value2);
        }

        [Fact]
        public void CanReadFrameFromBytes()
        {
            var rawData = new byte[] { 0xE3, 0xDC, 0x00, 0x11, 0x3C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC8, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x84, 0x03, 0x02, 0x01, 0x00, 0x7f, 0x01, 0x00, 0x80, 0x00, 0x02, 0x01, 0x00, 0x7f, 0x00, 0x00, 0x00, 0x00 };
            Crc32Algorithm.ComputeAndWriteToEnd(rawData);

            var expectedValue1 = new RscpInt8(RscpTag.BAT_DATA, 0x7F);
            var expectedValue2 = new RscpInt8(RscpTag.RSCP_AUTHENTICATION, 0x7F);

            var frame = new RscpFrame(rawData);

            frame.Length.Should().Be(16);
            frame.HasChecksum.Should().BeTrue();
            frame.Timestamp.Ticks.Should().Be(DateTime.UnixEpoch.Ticks + (60 * TimeSpan.TicksPerSecond) + 2);

            frame.TryGetValue<RscpInt8>(RscpTag.BAT_DATA, out var retrievedValue1).Should().BeTrue();
            retrievedValue1.Should().BeEquivalentTo(expectedValue1);

            frame.TryGetValue<RscpInt8>(RscpTag.RSCP_AUTHENTICATION, out var retrievedValue2).Should().BeTrue();
            retrievedValue2.Should().BeEquivalentTo(expectedValue2);
        }

        [Fact]
        public void ReadFrameThrowsExceptionIfNoDataWasPassed()
        {
            var action = new Action(() => _ = new RscpFrame(null));

            action.Should()
                .Throw<ArgumentNullException>()
                .Where(e => e.ParamName == "data");
        }

        [Fact]
        public void ReadFrameThrowsExceptionIfEmptyDataWasPassed()
        {
            var action = new Action(() => _ = new RscpFrame(Array.Empty<byte>()));

            action.Should()
                .Throw<ArgumentException>()
                .WithMessage("No bytes have been passed.");
        }

        [Fact]
        public void ReadFrameThrowsExceptionIfMagicBytesAreMissing()
        {
            var rawData = new byte[] { 0xAB, 0xCD, 0x00, 0x11, 0x3C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC8, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x84, 0x03, 0x02, 0x01, 0x00, 0x7f, 0x01, 0x00, 0x80, 0x00, 0x02, 0x01, 0x00, 0x7f, 0x00, 0x00, 0x00, 0x00 };
            Crc32Algorithm.ComputeAndWriteToEnd(rawData);

            var action = new Action(() => _ = new RscpFrame(rawData));

            action.Should()
                .Throw<ArgumentException>()
                .WithMessage("Data does not contain an RscpFrame.");
        }

        [Fact]
        public void ReadFrameThrowsExceptionIfChecksumIsIncorrect()
        {
            var rawData = new byte[] { 0xE3, 0xDC, 0x00, 0x11, 0x3C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC8, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x84, 0x03, 0x02, 0x01, 0x00, 0x7f, 0x01, 0x00, 0x80, 0x00, 0x02, 0x01, 0x00, 0x7f, 0x00, 0x00, 0x00, 0x00 };
            var action = new Action(() => _ = new RscpFrame(rawData));

            action.Should()
                .Throw<ArgumentException>()
                .WithMessage("Data checksum is invalid.");
        }
    }
}
