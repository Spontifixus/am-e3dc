using System;
using AM.E3dc.Rscp.Data.Values;
using FluentAssertions;
using Xunit;

namespace AM.E3dc.Rscp.Data.Tests
{
    public class RscpValueFixture
    {
        private const RscpTag Tag = RscpTag.BAT_CHARGE_CYCLES;

        [Fact]
        public void FromBytesThrowsExceptionOnInvalidDataType()
        {
            var value = new byte[] { 0x00, 0x00, 0x00, 0x00, 0xee, 0x00, 0x00 };

            var action = new Action(() => _ = RscpValue.FromBytes(new ReadOnlySpan<byte>(value)));

            action.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("The data type 0xEE is unknown!");
        }

        [Fact]
        public void CanHandleRscpVoid()
        {
            var rscpValue = new RscpVoid(Tag);
            rscpValue.AssertHeader<RscpVoid>(Tag, RscpDataType.Void, 0);

            var deserialized = rscpValue.SerializeAndDeserialize();

            deserialized.AssertHeader<RscpVoid>(Tag, RscpDataType.Void, 0);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanHandleRscpBool(bool value)
        {
            var rscpValue = new RscpBool(Tag, value);
            rscpValue.AssertHeader<RscpBool>(Tag, RscpDataType.Bool, 1);
            rscpValue.Value.Should().Be(value);

            var deserialized = rscpValue.SerializeAndDeserialize();

            deserialized.AssertHeader<RscpBool>(Tag, RscpDataType.Bool, 1);
            ((RscpBool)deserialized).Value.Should().Be(value);
        }

        [Theory]
        [InlineData(sbyte.MinValue)]
        [InlineData(0)]
        [InlineData(sbyte.MaxValue)]
        public void CanHandleRscpInt8(sbyte value)
        {
            var rscpValue = new RscpInt8(Tag, value);
            rscpValue.AssertHeader<RscpInt8>(Tag, RscpDataType.Int8, 1);
            rscpValue.Value.Should().Be(value);

            var deserialized = rscpValue.SerializeAndDeserialize();

            deserialized.AssertHeader<RscpInt8>(Tag, RscpDataType.Int8, 1);
            ((RscpInt8)deserialized).Value.Should().Be(value);
        }

        [Theory]
        [InlineData(byte.MinValue)]
        [InlineData(byte.MaxValue)]
        public void CanHandleRscpUInt8(byte value)
        {
            var rscpValue = new RscpUInt8(Tag, value);
            rscpValue.AssertHeader<RscpUInt8>(Tag, RscpDataType.UInt8, 1);
            rscpValue.Value.Should().Be(value);

            var deserialized = rscpValue.SerializeAndDeserialize();

            deserialized.AssertHeader<RscpUInt8>(Tag, RscpDataType.UInt8, 1);
            ((RscpUInt8)deserialized).Value.Should().Be(value);
        }

        [Theory]
        [InlineData(short.MinValue)]
        [InlineData(0)]
        [InlineData(short.MaxValue)]
        public void CanHandleRscpInt16(short value)
        {
            var rscpValue = new RscpInt16(Tag, value);
            rscpValue.AssertHeader<RscpInt16>(Tag, RscpDataType.Int16, 2);
            rscpValue.Value.Should().Be(value);

            var deserialized = rscpValue.SerializeAndDeserialize();

            deserialized.AssertHeader<RscpInt16>(Tag, RscpDataType.Int16, 2);
            ((RscpInt16)deserialized).Value.Should().Be(value);
        }

        [Theory]
        [InlineData(ushort.MinValue)]
        [InlineData(ushort.MaxValue)]
        public void CanHandleRscpUInt16(ushort value)
        {
            var rscpValue = new RscpUInt16(Tag, value);
            rscpValue.AssertHeader<RscpUInt16>(Tag, RscpDataType.UInt16, 2);
            rscpValue.Value.Should().Be(value);

            var deserialized = rscpValue.SerializeAndDeserialize();

            deserialized.AssertHeader<RscpUInt16>(Tag, RscpDataType.UInt16, 2);
            ((RscpUInt16)deserialized).Value.Should().Be(value);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        public void CanHandleRscpInt32(int value)
        {
            var rscpValue = new RscpInt32(Tag, value);
            rscpValue.AssertHeader<RscpInt32>(Tag, RscpDataType.Int32, 4);
            rscpValue.Value.Should().Be(value);

            var deserialized = rscpValue.SerializeAndDeserialize();

            deserialized.AssertHeader<RscpInt32>(Tag, RscpDataType.Int32, 4);
            ((RscpInt32)deserialized).Value.Should().Be(value);
        }

        [Theory]
        [InlineData(uint.MinValue)]
        [InlineData(uint.MaxValue)]
        public void CanHandleRscpUInt32(uint value)
        {
            var rscpValue = new RscpUInt32(Tag, value);
            rscpValue.AssertHeader<RscpUInt32>(Tag, RscpDataType.UInt32, 4);
            rscpValue.Value.Should().Be(value);

            var deserialized = rscpValue.SerializeAndDeserialize();

            deserialized.AssertHeader<RscpUInt32>(Tag, RscpDataType.UInt32, 4);
            ((RscpUInt32)deserialized).Value.Should().Be(value);
        }

        [Theory]
        [InlineData(long.MinValue)]
        [InlineData(0)]
        [InlineData(long.MaxValue)]
        public void CanHandleRscpInt64(long value)
        {
            var rscpValue = new RscpInt64(Tag, value);
            rscpValue.AssertHeader<RscpInt64>(Tag, RscpDataType.Int64, 8);
            rscpValue.Value.Should().Be(value);

            var deserialized = rscpValue.SerializeAndDeserialize();

            deserialized.AssertHeader<RscpInt64>(Tag, RscpDataType.Int64, 8);
            ((RscpInt64)deserialized).Value.Should().Be(value);
        }

        [Theory]
        [InlineData(ulong.MinValue)]
        [InlineData(ulong.MaxValue)]
        public void CanHandleRscpUInt64(ulong value)
        {
            var rscpValue = new RscpUInt64(Tag, value);
            rscpValue.AssertHeader<RscpUInt64>(Tag, RscpDataType.UInt64, 8);
            rscpValue.Value.Should().Be(value);

            var deserialized = rscpValue.SerializeAndDeserialize();

            deserialized.AssertHeader<RscpUInt64>(Tag, RscpDataType.UInt64, 8);
            ((RscpUInt64)deserialized).Value.Should().Be(value);
        }

        [Theory]
        [InlineData(float.MinValue)]
        [InlineData(0)]
        [InlineData(float.MaxValue)]
        public void CanHandleRscpFloat(float value)
        {
            var rscpValue = new RscpFloat(Tag, value);
            rscpValue.AssertHeader<RscpFloat>(Tag, RscpDataType.Float, 4);
            rscpValue.Value.Should().Be(value);

            var deserialized = rscpValue.SerializeAndDeserialize();

            deserialized.AssertHeader<RscpFloat>(Tag, RscpDataType.Float, 4);
            ((RscpFloat)deserialized).Value.Should().Be(value);
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(0)]
        [InlineData(double.MaxValue)]
        public void CanHandleRscpDouble(double value)
        {
            var rscpValue = new RscpDouble(Tag, value);
            rscpValue.AssertHeader<RscpDouble>(Tag, RscpDataType.Double, 8);
            rscpValue.Value.Should().Be(value);

            var deserialized = rscpValue.SerializeAndDeserialize();

            deserialized.AssertHeader<RscpDouble>(Tag, RscpDataType.Double, 8);
            ((RscpDouble)deserialized).Value.Should().Be(value);
        }

        [Theory]
        [InlineData("Hello World")]
        [InlineData("")]
        [InlineData("Hello\nWorld")]
        public void CanHandleRscpString(string value)
        {
            var rscpValue = new RscpString(Tag, value);
            rscpValue.AssertHeader<RscpString>(Tag, RscpDataType.String, (ushort)value.Length);
            rscpValue.Value.Should().Be(value);

            var deserialized = rscpValue.SerializeAndDeserialize();

            deserialized.AssertHeader<RscpString>(Tag, RscpDataType.String, (ushort)value.Length);
            ((RscpString)deserialized).Value.Should().Be(value);
        }

        [Fact]
        public void RscpStringThrowsExceptionIfTooLong()
        {
            var action = new Action(() => _ = new RscpString(Tag, new string(' ', ushort.MaxValue)));

            action.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("The string is too long for a single message.");
        }

        [Fact]
        public void CanHandleRscpTime()
        {
            var now = DateTime.Now;
            var rscpValue = new RscpTime(Tag, now);
            rscpValue.AssertHeader<RscpTime>(Tag, RscpDataType.Timestamp, 12);
            rscpValue.Value.Should().Be(now);

            var deserialized = rscpValue.SerializeAndDeserialize();

            deserialized.AssertHeader<RscpTime>(Tag, RscpDataType.Timestamp, 12);
            ((RscpTime)deserialized).Value.Should().Be(now);
        }

        [Fact]
        public void CanHandleRscpByteArray()
        {
            var data = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
            var rscpValue = new RscpByteArray(Tag, data);
            rscpValue.AssertHeader<RscpByteArray>(Tag, RscpDataType.ByteArray, (ushort)data.Length);
            rscpValue.Value.Should().BeEquivalentTo(data);

            var deserialized = rscpValue.SerializeAndDeserialize();

            deserialized.AssertHeader<RscpByteArray>(Tag, RscpDataType.ByteArray, (ushort)data.Length);
            ((RscpByteArray)deserialized).Value.Should().BeEquivalentTo(data);
        }

        [Fact]
        public void RscpByteArrayThrowsExceptionIfTooLong()
        {
            var action = new Action(() => _ = new RscpByteArray(Tag, new byte[ushort.MaxValue]));

            action.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("The byte array is too long for a single message.");
        }

        [Fact]
        public void CanHandleRscpBitfield()
        {
            var data = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
            var rscpValue = new RscpBitfield(Tag, data);
            rscpValue.AssertHeader<RscpBitfield>(Tag, RscpDataType.Bitfield, (ushort)data.Length);
            rscpValue.Value.Should().BeEquivalentTo(data);

            var deserialized = rscpValue.SerializeAndDeserialize();

            deserialized.AssertHeader<RscpBitfield>(Tag, RscpDataType.Bitfield, (ushort)data.Length);
            ((RscpBitfield)deserialized).Value.Should().BeEquivalentTo(data);
        }

        [Fact]
        public void RscpBitfieldThrowsExceptionIfTooLong()
        {
            var action = new Action(() => _ = new RscpBitfield(Tag, new byte[ushort.MaxValue]));

            action.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("The byte array is too long for a single message.");
        }

        [Fact]
        public void CanHandleRscpError()
        {
            var rscpValue = new RscpError(Tag, RscpErrorCode.AccessDenied);
            rscpValue.AssertHeader<RscpError>(Tag, RscpDataType.Error, 4);
            rscpValue.Value.Should().BeEquivalentTo(RscpErrorCode.AccessDenied);

            var deserialized = rscpValue.SerializeAndDeserialize();

            deserialized.AssertHeader<RscpError>(Tag, RscpDataType.Error, 4);
            ((RscpError)deserialized).Value.Should().BeEquivalentTo(RscpErrorCode.AccessDenied);
        }

        [Fact]
        public void CanHandleRscpContainer()
        {
            var rscpContainer = new RscpContainer(Tag);
            rscpContainer.AssertHeader<RscpContainer>(Tag, RscpDataType.Container, 0);
            rscpContainer.Children.Should().BeEmpty();

            var value1 = new RscpBool(Tag, true);
            rscpContainer.Add(value1);
            rscpContainer.AssertHeader<RscpContainer>(Tag, RscpDataType.Container, value1.TotalLength);

            var value2 = new RscpUInt64(Tag, ulong.MaxValue);
            var secondContainer = new RscpContainer(Tag);
            secondContainer.Add(value2);
            secondContainer.AssertHeader<RscpContainer>(Tag, RscpDataType.Container, value2.TotalLength);

            rscpContainer.Add(secondContainer);
            rscpContainer.AssertHeader<RscpContainer>(Tag, RscpDataType.Container, (ushort)(value1.TotalLength + secondContainer.TotalLength));

            var deserialized = rscpContainer.SerializeAndDeserialize();

            deserialized.AssertHeader<RscpContainer>(Tag, RscpDataType.Container, (ushort)(value1.TotalLength + secondContainer.TotalLength));
            ((RscpContainer)deserialized).Should().BeEquivalentTo(rscpContainer);
        }

        [Fact]
        public void CanHandleEmptyRscpContainer()
        {
            var rscpContainer = new RscpContainer(Tag);
            rscpContainer.AssertHeader<RscpContainer>(Tag, RscpDataType.Container, 0);
            rscpContainer.Children.Should().BeEmpty();

            var deserialized = rscpContainer.SerializeAndDeserialize();

            deserialized.AssertHeader<RscpContainer>(Tag, RscpDataType.Container, 0);
            ((RscpContainer)deserialized).Should().BeEquivalentTo(rscpContainer);
        }

        [Fact]
        public void RscpContainerThrowsExceptionOnCircularReference()
        {
            var rscpContainer = new RscpContainer(Tag);

            var action = new Action(() =>
                {
                    rscpContainer.Add(rscpContainer);
                });

            action.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("The value cannot be added, because it would cause a circular reference.");

            var secondContainer = new RscpContainer(Tag);
            secondContainer.Add(rscpContainer);

            var action2 = new Action(() =>
            {
                rscpContainer.Add(secondContainer);
            });

            action2.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("The value cannot be added, because it would cause a circular reference.");
        }

        [Fact]
        public void RscpContainerThrowsExceptionIfNullAdded()
        {
            var rscpContainer = new RscpContainer(Tag);

            var action = new Action(() =>
            {
                rscpContainer.Add(null);
            });

            action.Should()
                .Throw<ArgumentNullException>()
                .Where(e => e.ParamName == "value");
        }

        [Fact]
        public void RscpContainerThrowsExceptionWhenTooLong()
        {
            var rscpContainer = new RscpContainer(Tag);

            var action = new Action(() =>
            {
                var data = new byte[short.MaxValue];
                data.Initialize();
                var value1 = new RscpByteArray(Tag, data);
                var value2 = new RscpByteArray(Tag, data);
                rscpContainer.Add(value1);
                rscpContainer.Add(value2);
            });

            action.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Can't put the value into this container because then the lid won't close.");
        }
    }
}
