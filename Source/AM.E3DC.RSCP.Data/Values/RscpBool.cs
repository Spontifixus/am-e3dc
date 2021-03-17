using System;
using System.Runtime.InteropServices;

namespace AM.E3dc.Rscp.Data.Values
{
    /// <summary>
    /// Value object used to transport messages with a boolean payload.
    /// </summary>
    public sealed class RscpBool : RscpValue<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RscpBool"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of the object.</param>
        public RscpBool(RscpTag tag, bool value)
        : base(tag, 1, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpBool"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpBool(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, 1, MemoryMarshal.Read<bool>(data))
        {
        }

        /// <inheritdoc />
        public override RscpDataType DataType => RscpDataType.Bool;

        private protected override void OnWrite(Span<byte> destination)
        {
            destination[0] = (byte)(this.Value ? 0x01 : 0x00);
        }
    }
}
