using System;
using System.Runtime.InteropServices;

namespace AM.E3DC.RSCP.Data.Values
{
    /// <summary>
    /// Value object used to transport messages with a signed 16-bit integer (<see cref="short"/>) payload.
    /// </summary>
    public sealed class RscpInt16 : RscpStruct<short>
    {
        private const ushort DataLength = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpInt16"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of the object.</param>
        public RscpInt16(RscpTag tag, short value)
        : base(tag, RscpDataType.Int16, DataLength, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpInt16"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpInt16(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, RscpDataType.Int16, DataLength)
        {
            this.Value = MemoryMarshal.Read<short>(data);
        }
    }
}
