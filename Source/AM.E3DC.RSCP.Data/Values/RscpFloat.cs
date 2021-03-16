using System;
using System.Runtime.InteropServices;

namespace AM.E3DC.RSCP.Data.Values
{
    /// <summary>
    /// Value object used to transport messages with a single precision (<see cref="float"/>) value payload.
    /// </summary>
    public sealed class RscpFloat : RscpReferenceType<float>
    {
        private const ushort DataLength = 4;

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpFloat"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of the object.</param>
        public RscpFloat(RscpTag tag, float value)
        : base(tag, RscpDataType.Float, DataLength, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpFloat"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpFloat(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, RscpDataType.Float, DataLength)
        {
            this.Value = MemoryMarshal.Read<float>(data);
        }
    }
}
