using System;
using AM.E3DC.RSCP.Data.Values;
using FluentAssertions;

namespace AM.E3DC.RSCP.Data.Tests
{
    public static class RscpValueExtensions
    {
        public static RscpValue SerializeAndDeserialize(this RscpValue rscpValue)
        {
            const ushort headerLength = 7;
            var bytes = new byte[headerLength + rscpValue.Length];
            var destination = new Span<byte>(bytes);

            rscpValue.WriteTo(destination);

            return RscpValue.FromBytes(destination);
        }

        public static void AssertHeader<TValue>(this RscpValue value, RscpTag expectedTag, RscpDataType expectedDataType, ushort expectedLength)
            where TValue : RscpValue
        {
            value.Should().BeOfType<TValue>();
            value.Tag.Should().Be(expectedTag);
            value.DataType.Should().Be(expectedDataType);
            value.Length.Should().Be(expectedLength);
        }
    }
}
