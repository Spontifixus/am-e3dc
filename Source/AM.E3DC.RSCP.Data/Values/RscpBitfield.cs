using System;

namespace AM.E3DC.RSCP.Data.Values
{
    /// <summary>
    /// Value object used to transport messages with a bitfield payload.
    /// </summary>
    public sealed class RscpBitfield : RscpByteArray
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RscpBitfield"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of the object.</param>
        public RscpBitfield(RscpTag tag, byte[] value)
        : base(tag, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpBitfield"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpBitfield(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, data)
        {
        }

        /// <inheritdoc />
        public override RscpDataType DataType => RscpDataType.Bitfield;
    }
}
