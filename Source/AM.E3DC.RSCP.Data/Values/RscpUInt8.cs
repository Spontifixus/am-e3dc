using System;

namespace AM.E3DC.RSCP.Data.Values
{
    /// <summary>
    /// Value object used to transport messages with an unsigned 8-bit integer (<see cref="byte"/>) payload.
    /// </summary>
    public sealed class RscpUInt8 : RscpStruct<byte>
    {
        private const ushort DataLength = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpUInt8"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of the object.</param>
        public RscpUInt8(RscpTag tag, byte value)
        : base(tag, RscpDataType.UInt8, DataLength, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpUInt8"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpUInt8(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, RscpDataType.UInt8, DataLength)
        {
            this.Value = data[0];
        }
    }
}
