using System;
using System.Collections.Generic;
using System.Linq;

namespace AM.E3dc.Rscp.Data.Values
{
    /// <summary>
    /// Value object used to transport messages with a <see cref="string"/> payload.
    /// </summary>
    public sealed class RscpContainer : RscpValue
    {
        private readonly List<RscpValue> children = new List<RscpValue>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpContainer"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        public RscpContainer(RscpTag tag)
        : base(tag, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpString"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="data">The span containing the value of this object.</param>
        internal RscpContainer(RscpTag tag, ReadOnlySpan<byte> data)
            : base(tag, 0)
        {
            this.InitializeFromBytes(data);
        }

        /// <inheritdoc />
        public override RscpDataType DataType => RscpDataType.Container;

        /// <summary>
        /// Gets the children of this container.
        /// </summary>
        public IReadOnlyList<RscpValue> Children => this.children;

        /// <summary>
        /// Adds a value to the container.
        /// </summary>
        /// <param name="value">The value to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if no value was passed.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the container is full already or adding the child would cause a circular reference.</exception>
        public void Add(RscpValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (this.Length + value.TotalLength > ushort.MaxValue)
            {
                throw new InvalidOperationException("Can't put the value into this container because then the lid won't close.");
            }

            if (this.CausesCircularReference(value))
            {
                throw new InvalidOperationException("The value cannot be added, because it would cause a circular reference.");
            }

            this.children.Add(value);
            this.Length += value.TotalLength;
        }

        private protected override void OnWrite(Span<byte> destination)
        {
            var offset = 0;
            foreach (var rscpValue in this.Children)
            {
                rscpValue.WriteTo(destination.Slice(offset, rscpValue.TotalLength));
                offset += rscpValue.TotalLength;
            }
        }

        private bool CausesCircularReference(RscpValue value)
        {
            return value is RscpContainer rscpContainer && (rscpContainer == this || rscpContainer.Children.Any(this.CausesCircularReference));
        }

        private void InitializeFromBytes(ReadOnlySpan<byte> data)
        {
            var offset = 0;
            do
            {
                var rscpValue = FromBytes(data.Slice(offset));
                this.Add(rscpValue);
                offset += rscpValue.TotalLength;
            }
            while (offset < data.Length);
        }
    }
}
