using System;
using System.Runtime.InteropServices;

namespace AM.E3DC.RSCP.Data.Values
{
    /// <summary>
    /// Value object used to transport messages with an unsigned 32-bit integer (<see cref="uint"/>) payload.
    /// </summary>
    public sealed class RscpUInt32 : RscpStruct<uint>
    {
        private const ushort DataLength = 4;

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpUInt32"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of the object.</param>
        public RscpUInt32(RscpTag tag, uint value)
        : base(tag, RscpDataType.UInt32, DataLength, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpUInt32"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpUInt32(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, RscpDataType.UInt32, DataLength)
        {
            this.Value = MemoryMarshal.Read<uint>(data);
        }
    }
}
