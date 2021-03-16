using System;
using System.Runtime.InteropServices;

namespace AM.E3DC.RSCP.Data.Values
{
    /// <summary>
    /// Value object used to transport messages with an <see cref="RscpErrorCode"/> payload.
    /// </summary>
    public sealed class RscpError : RscpValue<RscpErrorCode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RscpError"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of the object.</param>
        public RscpError(RscpTag tag, RscpErrorCode value)
        : base(tag, 1, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpError"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpError(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, 1, MemoryMarshal.Read<RscpErrorCode>(data))
        {
        }

        /// <inheritdoc />
        public override RscpDataType DataType => RscpDataType.Error;

        private protected override void OnWrite(Span<byte> destination)
        {
            var value = this.Value;
            MemoryMarshal.Write(destination, ref value);
        }
    }
}
