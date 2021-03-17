using System;
using System.Runtime.InteropServices;

namespace AM.E3dc.Rscp.Data.Values
{
    /// <summary>
    /// This is the base class for value objects that can be added to an RscpMessage.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public abstract class RscpReferenceType<TValue> : RscpValue<TValue>
        where TValue : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RscpReferenceType{TValue}"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="value">The value of this object.</param>
        protected RscpReferenceType(RscpTag tag, TValue value)
            : base(tag, (ushort)Marshal.SizeOf<TValue>(), value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpValue{TValue}"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The raw data of this object.</param>
        protected RscpReferenceType(RscpTag tag, ReadOnlySpan<byte> data)
            : this(tag, MemoryMarshal.Read<TValue>(data))
        {
        }

        private protected override void OnWrite(Span<byte> destination)
        {
            var value = this.Value;
            MemoryMarshal.Write(destination, ref value);
        }
    }
}
