using System;
using System.Runtime.InteropServices;

namespace AM.E3DC.RSCP.Data.Values
{
    /// <summary>
    /// Value object used to transport messages with a signed 64-bit integer (<see cref="long"/>) payload.
    /// </summary>
    public sealed class RscpInt64 : RscpReferenceType<long>
    {
        private const ushort DataLength = 8;

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpInt64"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of the object.</param>
        public RscpInt64(RscpTag tag, long value)
        : base(tag, RscpDataType.Int64, DataLength, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpInt64"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpInt64(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, RscpDataType.Int64, DataLength)
        {
            this.Value = MemoryMarshal.Read<long>(data);
        }
    }
}
