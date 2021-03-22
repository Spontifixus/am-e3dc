using System;
using System.Runtime.InteropServices;

namespace AM.E3dc.Rscp.Data.Values
{
    /// <summary>
    /// Value object used to transport messages with an <see cref="RscpTimestamp"/> payload.
    /// </summary>
    public sealed class RscpTime : RscpValue<DateTime>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RscpTime"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of the object.</param>
        public RscpTime(RscpTag tag, DateTime value)
        : base(tag, 12, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpTime"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpTime(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, 12, MemoryMarshal.Read<RscpTimestamp>(data).ToDateTime())
        {
        }

        /// <inheritdoc />
        public override RscpDataType DataType => RscpDataType.Timestamp;

        private protected override void OnWrite(Span<byte> destination)
        {
            var value = new RscpTimestamp(this.Value);
            MemoryMarshal.Write(destination, ref value);
        }
    }
}
