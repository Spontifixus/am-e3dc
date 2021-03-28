using System;
using System.Runtime.InteropServices;

namespace AM.E3dc.Rscp.Data
{
    /// <summary>
    /// This class represents a time value.
    /// </summary>
    /// <remarks>
    /// Times in the Rscp-Protocol are represented as seconds and nanoseconds after
    /// the unix epoch (01.01.1970).
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public readonly struct RscpTimestamp
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RscpTimestamp"/> class.
        /// </summary>
        /// <param name="timespan">The time that has passed since the unix-epoch (1.1.1970).</param>
        public RscpTimestamp(TimeSpan timespan)
        {
            var ticks = timespan.Ticks;
            this.Seconds = ticks / TimeSpan.TicksPerSecond;
            this.Nanoseconds = (int)(ticks % TimeSpan.TicksPerSecond) * 100;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpTimestamp"/> struct.
        /// </summary>
        /// <param name="timestamp">The timestamp that is to be represented by this instance.</param>
        public RscpTimestamp(DateTime timestamp)
            : this(timestamp.Subtract(DateTime.UnixEpoch))
        {
        }

        /// <summary>
        /// Gets the seconds-part of the timestamp.
        /// </summary>
        /// <value>The seconds that have elapsed since January 1st, 1970.</value>
        //[field: FieldOffset(0)]
        public long Seconds { get; }

        /// <summary>
        /// Gets the nanoseconds-part of the timestamp.
        /// </summary>
        /// <value>
        /// The nanoseconds that have elapsed since the time identified by the <see cref="Seconds"/> property.
        /// </value>
        //[field: FieldOffset(8)]
        public int Nanoseconds { get; }

        /// <summary>
        /// Creates a new instance of <see cref="DateTime"/> from this instance.
        /// </summary>
        /// <returns>A <see cref="DateTime"/> instance representing this timestamp.</returns>
        public DateTime ToDateTime()
        {
            return DateTime.UnixEpoch.Add(this.ToTimeSpan());
        }

        /// <summary>
        /// Creates a new instance of <see cref="TimeSpan"/> from this instance.
        /// </summary>
        /// <returns>A <see cref="TimeSpan"/> representing the time that has passed since the unix epoch (1.1.1970).</returns>
        public TimeSpan ToTimeSpan()
        {
            var ticks = (this.Seconds * TimeSpan.TicksPerSecond) + (this.Nanoseconds / 100);
            return TimeSpan.FromTicks(ticks);
        }
    }
}
