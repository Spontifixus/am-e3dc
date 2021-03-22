using System;

namespace AM.E3dc.Rscp.Data.Values
{
    /// <summary>
    /// Value object used to transport messages with a signed 8-bit integer (<see cref="sbyte"/>) payload.
    /// </summary>
    public sealed class RscpInt8 : RscpReferenceType<sbyte>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RscpInt8"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of the object.</param>
        public RscpInt8(RscpTag tag, sbyte value)
        : base(tag, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpInt8"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpInt8(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, data)
        {
        }

        /// <inheritdoc />
        public override RscpDataType DataType => RscpDataType.Int8;
    }
}
