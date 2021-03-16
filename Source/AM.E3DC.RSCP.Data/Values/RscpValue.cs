using System;
using System.Runtime.InteropServices;

namespace AM.E3DC.RSCP.Data.Values
{
    /// <summary>
    /// This is the base class for value objects that can be added to an RscpMessage.
    /// </summary>
    public abstract class RscpValue
    {
        private const ushort HeaderLength = 7;

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpValue"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="dataType">The data type of the value object.</param>
        /// <param name="length">The length of the value object.</param>
        protected RscpValue(RscpTag tag, RscpDataType dataType, ushort length)
        {
            this.Tag = tag;
            this.DataType = dataType;
            this.Length = length;
        }

        /// <summary>
        /// Gets the tag of the value object.
        /// </summary>
        public RscpTag Tag { get; }

        /// <summary>
        /// Gets the data type of the value object.
        /// </summary>
        public RscpDataType DataType { get; }

        /// <summary>
        /// Gets the length of the value object.
        /// </summary>
        public ushort Length { get; }

        /// <summary>
        /// Reads the value from the source.
        /// </summary>
        /// <param name="source">The source span where the data can be read from.</param>
        internal static RscpValue FromBytes(ReadOnlySpan<byte> source)
        {
            var tag = MemoryMarshal.Read<RscpTag>(source.Slice(0, 4));
            var dataType = MemoryMarshal.Read<RscpDataType>(source.Slice(4, 1));
            var length = MemoryMarshal.Read<ushort>(source.Slice(5, 2));

            var data = source.Slice(HeaderLength, length);
            return dataType switch
            {
                RscpDataType.Void => new RscpVoid(tag),
                RscpDataType.Bool => new RscpBool(tag, data),
                RscpDataType.Int8 => new RscpInt8(tag, data),
                RscpDataType.UInt8 => new RscpUInt8(tag, data),
                RscpDataType.Int16 => new RscpInt16(tag, data),
                RscpDataType.UInt16 => new RscpUInt16(tag, data),
                RscpDataType.Int32 => new RscpInt32(tag, data),
                RscpDataType.UInt32 => new RscpUInt32(tag, data),
                RscpDataType.Int64 => new RscpInt64(tag, data),
                RscpDataType.UInt64 => new RscpUInt64(tag, data),
                RscpDataType.Float => new RscpFloat(tag, data),
                RscpDataType.Double => new RscpDouble(tag, data),
                RscpDataType.String => new RscpString(tag, data),
                _ => throw new InvalidOperationException($"The data type 0x{(byte)dataType:X2} is unknown!")
            };
        }

        /// <summary>
        /// Writes the value to a span.
        /// </summary>
        /// <param name="destination">The span where the value is to be written to.</param>
        internal void WriteTo(Span<byte> destination)
        {
            var tag = this.Tag;
            MemoryMarshal.Write(destination.Slice(0, 4), ref tag);

            var dataType = this.DataType;
            MemoryMarshal.Write(destination.Slice(4, 1), ref dataType);

            var length = this.Length;
            MemoryMarshal.Write(destination.Slice(5, 2), ref length);

            this.OnWrite(destination.Slice(HeaderLength));
        }

        /// <summary>
        /// Writes the contents of this value to a span.
        /// </summary>
        /// <param name="destination">The span where the value is to be written to.</param>
        /// <remarks>Override this method in a derived class to write the value of the object to the destination.</remarks>
        private protected virtual void OnWrite(Span<byte> destination)
        {
        }
    }
}
