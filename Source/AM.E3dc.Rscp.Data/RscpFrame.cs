using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using AM.E3dc.Rscp.Data.Values;
using Force.Crc32;

namespace AM.E3dc.Rscp.Data
{
    /// <summary>
    /// This class represents an RSCP control frame that can be used to communicate with an E3DC power station.
    /// </summary>
    public sealed class RscpFrame
    {
        private const ushort HeaderLength = 18;
        private const ushort ChecksumLength = 4;

        private static readonly byte[] MagicBytes = { 0xE3, 0xDC };

        private readonly Dictionary<RscpTag, RscpValue> values = new Dictionary<RscpTag, RscpValue>();

        private byte protocolVersion;
        private RscpTimestamp timestamp;

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpFrame"/> class.
        /// </summary>
        public RscpFrame()
        {
            this.HasChecksum = true;
            this.ProtocolVersion = 1;
            this.Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpFrame"/> class.
        /// </summary>
        /// <param name="data">The raw bytes to construct the frame from.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value has been passed to this method.</exception>
        /// <exception cref="ArgumentException">Thrown if no bytes have been passed to the method, the bytes do not contain an RscpFrame, or the checksum is invalid.</exception>
        internal RscpFrame(ReadOnlySpan<byte> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.IsEmpty)
            {
                throw new ArgumentException("No bytes have been passed.");
            }

            if (!data.Slice(0, 2).SequenceEqual(MagicBytes))
            {
                throw new ArgumentException("Data does not contain an RscpFrame.");
            }

            // According to the E3/DC protocol documentation this should
            // be rawData[2], but the documentation got that wrong...
            this.HasChecksum = (data[3] & 0x10) == 0x10;
            this.ProtocolVersion = (byte)(data[3] & 0x0F);

            this.timestamp = MemoryMarshal.Read<RscpTimestamp>(data.Slice(4, 12));
            this.Length = MemoryMarshal.Read<ushort>(data.Slice(16, 2));

            if (this.HasChecksum && !Crc32Algorithm.IsValidWithCrcAtEnd(data.Slice(0, (ushort)(HeaderLength + this.Length + 4)).ToArray()))
            {
                throw new ArgumentException("Data checksum is invalid.");
            }

            if (this.Length > 0)
            {
                var offset = HeaderLength;
                var expectedLength = HeaderLength + this.Length;
                while (offset < expectedLength)
                {
                    var value = RscpValue.FromBytes(data[offset..]);
                    this.values.Add(value.Tag, value);
                    offset += value.TotalLength;
                }
            }
        }

        /// <summary>
        /// Gets the timestamp of the frame.
        /// </summary>
        /// <value>The point in time where the frame was created.</value>
        public DateTime Timestamp
        {
            get => this.timestamp.ToDateTime();
            internal set
            {
                this.timestamp = new RscpTimestamp(value);
            }
        }

        /// <summary>
        /// Gets the protocol version of this frame.
        /// </summary>
        /// <value>A <see cref="byte"/> indicating the protocol version.</value>
        /// <remarks>For now only protocol version 1 is supported.</remarks>
        /// <exception cref="InvalidOperationException">Thrown if a version lower than 1 or higher than 15 is being set.</exception>
        public byte ProtocolVersion
        {
            get => this.protocolVersion;

            internal set
            {
                if (value == 0x00 || value > 0x0F)
                {
                    throw new InvalidOperationException("Invalid protocol version! The protocol version must be between 1 and 15.");
                }

                this.protocolVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this frame should contain a CRC32 checksum.
        /// </summary>
        /// <value>If set to <c>true</c>, a checksum will be included at the end of the frame.</value>
        public bool HasChecksum { get; set; }

        /// <summary>
        /// Gets the total length of this frame.
        /// </summary>
        /// <remarks>The length is calculated without the CRC32 checksum.</remarks>
        public ushort Length { get; private set; }

        /// <summary>
        /// Gets a value indicating whether an error was returned from the E3/DC unit.
        /// </summary>
        /// <value><c>true</c> if an error was returned; <c>false</c> otherwise.</value>
        public bool HasError => this.values.Values.Any(rscpValue => rscpValue.DataType == RscpDataType.Error);

        /// <summary>
        /// Gets the values that are contained in this frame.
        /// </summary>
        /// <value>A readonly collection of <see cref="RscpValue"/>.</value>
        public IReadOnlyList<RscpValue> Values => new ReadOnlyCollection<RscpValue>(this.values.Values.ToArray());

        /// <summary>
        /// Tries to receive the value with the specified tag and the specified value type from the frame.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="tag">The tag to be returned.</param>
        /// <param name="value">The value that was found.</param>
        /// <returns><c>true</c> if the value was found; <c>false</c> otherwise.</returns>
        public bool TryGetValue<TValue>(RscpTag tag, out TValue value)
        where TValue : RscpValue
        {
            if (this.values.ContainsKey(tag) && this.values[tag] is TValue typedValue)
            {
                value = typedValue;
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Gets the errors that are contained in this instance's values.
        /// </summary>
        /// <returns>An enumeration of RscpErrors.</returns>
        public IReadOnlyList<RscpError> GetErrors()
        {
            return new ReadOnlyCollection<RscpError>(this.values.Values.OfType<RscpError>().ToArray());
        }

        /// <summary>
        /// Adds a value to the frame.
        /// </summary>
        /// <param name="value">The value to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if no value was passed.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the frame is full already or a value with the same tag was added previously.</exception>
        public void Add(RscpValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (this.Length + value.TotalLength > ushort.MaxValue)
            {
                throw new InvalidOperationException("Can't put the value into this frame because then it would be too long.");
            }

            if (this.values.ContainsKey(value.Tag))
            {
                throw new InvalidOperationException("A value with this tag was added to the frame already.");
            }

            this.values.Add(value.Tag, value);
            this.Length += value.TotalLength;
        }

        /// <summary>
        /// Gets the raw bytes representing this frame.
        /// </summary>
        /// <returns>A <see cref="T:byte[]"/> containing all data of this frame.</returns>
        /// <remarks>
        /// Use this method to get the raw byte representation of this frame, that can then
        /// be encrypted and sent to the E3DC unit.
        /// </remarks>
        public byte[] GetBytes()
        {
            var totalLength = HeaderLength + this.Length;
            if (this.HasChecksum)
            {
                totalLength += ChecksumLength;
            }

            var rawDataBytes = new byte[totalLength];
            rawDataBytes.Initialize();

            var rawData = new Span<byte>(rawDataBytes);

            MagicBytes.CopyTo(rawData);

            // According to the E3/DC protocol documentation this should
            // be rawData[2], but the documentation got that wrong...
            rawData[3] = (byte)(rawData[3] | this.ProtocolVersion);
            if (this.HasChecksum)
            {
                rawData[3] = (byte)(rawData[3] | 0x10);
            }

            MemoryMarshal.Write(rawData.Slice(4, 12), ref this.timestamp);

            var length = this.Length;
            MemoryMarshal.Write(rawData.Slice(16, 2), ref length);

            var offset = HeaderLength;
            foreach (var rscpValue in this.values.Values)
            {
                rscpValue.WriteTo(rawData.Slice(offset, rscpValue.TotalLength));
                offset += rscpValue.TotalLength;
            }

            if (this.HasChecksum)
            {
                Crc32Algorithm.ComputeAndWriteToEnd(rawDataBytes);
            }

            return rawDataBytes;
        }
    }
}
