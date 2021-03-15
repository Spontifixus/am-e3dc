using System;
using System.Buffers;
using System.Runtime.InteropServices;
using Force.Crc32;

namespace AM.E3DC.RSCP.Data
{
    /// <summary>
    /// This class represents an RSCP control frame that can be used to communicate with an E3DC power station.
    /// </summary>
    public class RscpFrame
    {
        private static readonly byte[] MagicBytes = { 0xE3, 0xDC };

        private readonly IMemoryOwner<byte> bufferOwner = MemoryPool<byte>.Shared.Rent();

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpFrame"/> class.
        /// </summary>
        public RscpFrame()
        {
            this.InitializeMagicBytes();
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
            get
            {
                var seconds = MemoryMarshal.Read<long>(this.Memory.Span.Slice(4, 8));
                var nanoSeconds = MemoryMarshal.Read<int>(this.Memory.Span.Slice(12, 4));

                var ticks = DateTime.UnixEpoch.Ticks + (seconds * TimeSpan.TicksPerSecond) + (nanoSeconds / 100);
                return new DateTime(ticks);
            }

            internal set
            {
                var unixTimestamp = value.Ticks - DateTime.UnixEpoch.Ticks;
                var seconds = unixTimestamp / TimeSpan.TicksPerSecond;
                var nanoseconds = (int)(unixTimestamp % TimeSpan.TicksPerSecond) * 100;

                MemoryMarshal.Write(this.Memory.Span.Slice(4, 8), ref seconds);
                MemoryMarshal.Write(this.Memory.Span.Slice(12, 4), ref nanoseconds);
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
            get
            {
                return (byte)(this.Memory.Span[3] & 0x0F);
            }

            internal set
            {
                if (value == 0x00 || value > 0x0F)
                {
                    throw new InvalidOperationException("Invalid protocol version! The protocol version must be between 1 and 15.");
                }

                // Remove old version and set new one.
                this.Memory.Span[3] = (byte)(this.Memory.Span[3] & 0xF0);
                this.Memory.Span[3] = (byte)(this.Memory.Span[3] | value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this frame should contain a CRC32 checksum.
        /// </summary>
        /// <value>If set to <c>true</c>, a checksum will be included at the end of the frame.</value>
        public bool HasChecksum
        {
            get
            {
                return (byte)(this.Memory.Span[3] & 0x10) == 0x10;
            }

            set
            {
                if (value)
                {
                    this.Memory.Span[3] = (byte)(this.Memory.Span[3] | 0x10);
                }
                else
                {
                    this.Memory.Span[3] = (byte)(this.Memory.Span[3] & 0x0F);
                }
            }
        }

        /// <summary>
        /// Gets the total length of this frame.
        /// </summary>
        /// <remarks>The length is calculated without the CRC32 checksum.</remarks>
        public ushort Length => 18;

        private Memory<byte> Memory => this.bufferOwner.Memory;

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
            // Writing the length here, so we don't need to update this
            // anytime we're modifying the frame.
            var length = this.Length;
            MemoryMarshal.Write(this.Memory.Span.Slice(16, 2), ref length);

            var fullLength = this.HasChecksum ? this.Length + 4 : this.Length;
            var bytes = this.Memory.Span.Slice(0, fullLength).ToArray();
            if (this.HasChecksum)
            {
                Crc32Algorithm.ComputeAndWriteToEnd(bytes);
            }

            return bytes;
        }

        private void InitializeMagicBytes()
        {
            var span = this.Memory.Slice(0, 2).Span;
            MagicBytes.AsSpan().CopyTo(span);
        }
    }
}
