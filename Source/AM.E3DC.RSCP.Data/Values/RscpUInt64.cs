using System;
using System.Runtime.InteropServices;

namespace AM.E3DC.RSCP.Data.Values
{
    /// <summary>
    /// Value object used to transport messages an unsigned 64-bit integer (<see cref="ulong"/>) payload.
    /// </summary>
    public sealed class RscpUInt64 : RscpReferenceType<ulong>
    {
        private const ushort DataLength = 8;

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpUInt64"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of the object.</param>
        public RscpUInt64(RscpTag tag, ulong value)
        : base(tag, RscpDataType.UInt64, DataLength, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpUInt64"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpUInt64(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, RscpDataType.UInt64, DataLength)
        {
            this.Value = MemoryMarshal.Read<ulong>(data);
        }
    }
}
