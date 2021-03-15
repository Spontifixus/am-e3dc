using System;
using System.Runtime.InteropServices;

namespace AM.E3DC.RSCP.Data.Values
{
    /// <summary>
    /// Value object used to transport messages with an unsigned 16-bit integer (<see cref="ushort"/>) payload.
    /// </summary>
    public sealed class RscpUInt16 : RscpStruct<ushort>
    {
        private const ushort DataLength = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpUInt16"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of the object.</param>
        public RscpUInt16(RscpTag tag, ushort value)
        : base(tag, RscpDataType.UInt16, DataLength, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpUInt16"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpUInt16(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, RscpDataType.UInt16, DataLength)
        {
            this.Value = MemoryMarshal.Read<ushort>(data);
        }
    }
}
