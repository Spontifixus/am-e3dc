using System;
using System.Runtime.InteropServices;

namespace AM.E3dc.Rscp.Data.Values
{
    /// <summary>
    /// Value object used to transport messages with an <see cref="RscpTimestamp"/> payload.
    /// </summary>
    public sealed class RscpTime : RscpValue
    {
        private readonly RscpTimestamp timestamp;

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpTime"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The timespan to be represented by this object.</param>
        public RscpTime(RscpTag tag, TimeSpan value)
            : base(tag, (ushort)Marshal.SizeOf<RscpTimestamp>())
        {
            this.timestamp = new RscpTimestamp(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpTime"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The date to be represented by this object.</param>
        public RscpTime(RscpTag tag, DateTime value)
            : base(tag, (ushort)Marshal.SizeOf<RscpTimestamp>())
        {
            this.timestamp = new RscpTimestamp(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpTime"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpTime(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, (ushort)Marshal.SizeOf<RscpTimestamp>())
        {
            this.timestamp = MemoryMarshal.Read<RscpTimestamp>(data);
        }

        /// <inheritdoc />
        public override RscpDataType DataType => RscpDataType.Timestamp;

        /// <summary>
        /// Gets the value of this instance represented as DateTime object.
        /// </summary>
        public DateTime DateTime => this.timestamp.ToDateTime();

        /// <summary>
        /// Gets the value of this instance represented as TimeSpan (since the unix epoch).
        /// </summary>
        public TimeSpan TimeSpan => this.timestamp.ToTimeSpan();

        private protected override void OnWrite(Span<byte> destination)
        {
            var tempTimestamp = this.timestamp;
            MemoryMarshal.Write(destination, ref tempTimestamp);
        }
    }
}
