using System;

namespace AM.E3DC.RSCP.Data.Values
{
    /// <summary>
    /// Value object used to transport messages with a boolean payload.
    /// </summary>
    public sealed class RscpBool : RscpReferenceType<bool>
    {
        private const ushort DataLength = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpBool"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of the object.</param>
        public RscpBool(RscpTag tag, bool value)
        : base(tag, RscpDataType.Bool, DataLength, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpBool"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpBool(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, RscpDataType.Bool, DataLength)
        {
            this.Value = data[0] == 0x01;
        }
    }
}
