using System;

namespace AM.E3dc.Rscp.Data.Values
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
        /// <exception cref="InvalidOperationException">Thrown if value is too long.</exception>
        public RscpByteArray(RscpTag tag, byte[] value)
            : base(tag, (ushort)value.Length, value)
        {
            // If the byte array is too long, calculation of TotalLength
            // will overflow and thus be smaller than length.
            // If that is the case, the input array is too long.
            if (this.TotalLength < value.Length)
            {
                throw new InvalidOperationException("The byte array is too long for a single message.");
            }
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
