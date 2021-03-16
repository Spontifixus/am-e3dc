using System;
using System.Runtime.InteropServices;

namespace AM.E3DC.RSCP.Data.Values
{
    /// <summary>
    /// Value object used to transport messages with a double precision (<see cref="double"/>) value payload.
    /// </summary>
    public sealed class RscpDouble : RscpReferenceType<double>
    {
        private const ushort DataLength = 8;

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpDouble"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of the object.</param>
        public RscpDouble(RscpTag tag, double value)
        : base(tag, RscpDataType.Double, DataLength, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpDouble"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpDouble(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, RscpDataType.Double, DataLength)
        {
            this.Value = MemoryMarshal.Read<double>(data);
        }
    }
}
