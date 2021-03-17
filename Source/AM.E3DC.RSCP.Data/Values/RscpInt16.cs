using System;

namespace AM.E3dc.Rscp.Data.Values
{
    /// <summary>
    /// Value object used to transport messages with a signed 16-bit integer (<see cref="short"/>) payload.
    /// </summary>
    public sealed class RscpInt16 : RscpReferenceType<short>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RscpInt16"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of the object.</param>
        public RscpInt16(RscpTag tag, short value)
        : base(tag, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpInt16"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpInt16(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, data)
        {
        }

        /// <inheritdoc />
        public override RscpDataType DataType => RscpDataType.Int16;
    }
}
