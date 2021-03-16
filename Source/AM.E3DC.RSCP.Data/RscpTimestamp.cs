using System;
using System.Runtime.InteropServices;

namespace AM.E3DC.RSCP.Data
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
        /// Initializes a new instance of the <see cref="RscpTimestamp"/> struct.
        /// </summary>
        /// <param name="timestamp">The timestamp that is to be represented by this instance.</param>
        public RscpTimestamp(DateTime timestamp)
        {
            var unixTimestamp = timestamp.Ticks - new DateTime(1970, 1, 1).Ticks;
            this.Seconds = unixTimestamp / TimeSpan.TicksPerSecond;
            this.Nanoseconds = (int)(unixTimestamp % TimeSpan.TicksPerSecond) * 100;
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
        /// Creates a nes instance of <see cref="DateTime"/> from this instance.
        /// </summary>
        /// <returns>A <see cref="DateTime"/> instance representing this timestamp.</returns>
        public DateTime ToDateTime()
        {
            var ticks = DateTime.UnixEpoch.Ticks + (this.Seconds * TimeSpan.TicksPerSecond) + (this.Nanoseconds / 100);
            return new DateTime(ticks);
        }
    }
}
