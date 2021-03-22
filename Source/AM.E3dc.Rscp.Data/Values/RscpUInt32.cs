using System;

namespace AM.E3dc.Rscp.Data.Values
{
    /// <summary>
    /// Value object used to transport messages with an unsigned 32-bit integer (<see cref="uint"/>) payload.
    /// </summary>
    public sealed class RscpUInt32 : RscpReferenceType<uint>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RscpUInt32"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of the object.</param>
        public RscpUInt32(RscpTag tag, uint value)
        : base(tag, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpUInt32"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpUInt32(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, data)
        {
        }

        /// <inheritdoc />
        public override RscpDataType DataType => RscpDataType.UInt32;
    }
}
