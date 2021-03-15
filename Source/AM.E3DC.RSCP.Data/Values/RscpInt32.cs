using System;
using System.Runtime.InteropServices;

namespace AM.E3DC.RSCP.Data.Values
{
    /// <summary>
    /// Value object used to transport messages with a signed 32-bit integer (<see cref="int"/>) payload.
    /// </summary>
    public sealed class RscpInt32 : RscpStruct<int>
    {
        private const ushort DataLength = 4;

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpInt32"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of the object.</param>
        public RscpInt32(RscpTag tag, int value)
        : base(tag, RscpDataType.Int32, DataLength, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpInt32"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpInt32(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, RscpDataType.Int32, DataLength)
        {
            this.Value = MemoryMarshal.Read<int>(data);
        }
    }
}
