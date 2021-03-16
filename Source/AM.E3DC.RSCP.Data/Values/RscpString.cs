using System;
using System.Text;

namespace AM.E3DC.RSCP.Data.Values
{
    /// <summary>
    /// Value object used to transport messages with a boolean payload.
    /// </summary>
    public sealed class RscpString : RscpValue<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RscpString"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of the object.</param>
        public RscpString(RscpTag tag, string value)
        : base(tag, RscpDataType.String, (ushort)value.Length, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpString"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpString(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, RscpDataType.String, (ushort)data.Length)
        {
            this.Value = Encoding.UTF8.GetString(data);
        }

        private protected override void OnWrite(Span<byte> destination)
        {
            var bytes = Encoding.UTF8.GetBytes(this.Value);
            bytes.CopyTo(destination);
        }
    }
}
