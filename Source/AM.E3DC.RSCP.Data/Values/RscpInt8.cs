using System;

namespace AM.E3DC.RSCP.Data.Values
{
    /// <summary>
    /// Value object used to transport messages with a signed 8-bit integer (<see cref="sbyte"/>) payload.
    /// </summary>
    public sealed class RscpInt8 : RscpReferenceType<sbyte>
    {
        private const ushort DataLength = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpInt8"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of the object.</param>
        public RscpInt8(RscpTag tag, sbyte value)
        : base(tag, RscpDataType.Int8, DataLength, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpInt8"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpInt8(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, RscpDataType.Int8, DataLength)
        {
            this.Value = (sbyte)data[0];
        }
    }
}
