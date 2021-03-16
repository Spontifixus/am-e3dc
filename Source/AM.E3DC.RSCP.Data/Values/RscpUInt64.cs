using System;

namespace AM.E3DC.RSCP.Data.Values
{
    /// <summary>
    /// Value object used to transport messages an unsigned 64-bit integer (<see cref="ulong"/>) payload.
    /// </summary>
    public sealed class RscpUInt64 : RscpReferenceType<ulong>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RscpUInt64"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of the object.</param>
        public RscpUInt64(RscpTag tag, ulong value)
        : base(tag, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpUInt64"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpUInt64(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, data)
        {
        }

        /// <inheritdoc />
        public override RscpDataType DataType => RscpDataType.UInt64;
    }
}
