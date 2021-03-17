using System;
using System.Collections.Generic;
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

        private readonly List<RscpValue> values = new List<RscpValue>();

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
        /// Adds a value to the frame.
        /// </summary>
        /// <param name="value">The value to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if no value was passed.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the frame is full already.</exception>
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

            this.values.Add(value);
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
            foreach (var rscpValue in this.values)
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
