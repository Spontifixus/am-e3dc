using System;

namespace AM.E3DC.RSCP.Data.Values
{
    /// <summary>
    /// Value object used to transport messages with a <see cref="T:byte[]"/> payload.
    /// </summary>
    public class RscpByteArray : RscpValue<byte[]>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RscpByteArray"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of the object.</param>
        public RscpByteArray(RscpTag tag, byte[] value)
        : base(tag, (ushort)value.Length, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpByteArray"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpByteArray(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, (ushort)data.Length)
        {
            this.Value = data.ToArray();
        }

        /// <inheritdoc />
        public override RscpDataType DataType => RscpDataType.ByteArray;

        private protected override void OnWrite(Span<byte> destination)
        {
            var bytes = this.Value;
            bytes.CopyTo(destination);
        }
    }
}
