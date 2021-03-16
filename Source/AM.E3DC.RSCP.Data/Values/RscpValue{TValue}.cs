namespace AM.E3DC.RSCP.Data.Values
{
    /// <summary>
    /// This is the base class for value objects that can be added to an RscpMessage.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public abstract class RscpValue<TValue> : RscpValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RscpValue{TValue}"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="dataType">The data type of the value object.</param>
        /// <param name="length">The length of the value object.</param>
        protected RscpValue(RscpTag tag, RscpDataType dataType, ushort length)
            : base(tag, dataType, length)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RscpValue{TValue}"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        /// <param name="dataType">The data type of the value object.</param>
        /// <param name="length">The length of the value object.</param>
        /// <param name="value">The value of this object.</param>
        protected RscpValue(RscpTag tag, RscpDataType dataType, ushort length, TValue value)
        : this(tag, dataType, length)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value of this object.
        /// </summary>
        public TValue Value { get; private protected set; }
    }
}
