namespace AM.E3DC.RSCP.Data.Values
{
    /// <summary>
    /// Value object used to transport messages without payload.
    /// </summary>
    public sealed class RscpVoid : RscpValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RscpVoid"/> class.
        /// </summary>
        /// <param name="tag">The tag of the value object.</param>
        public RscpVoid(RscpTag tag)
            : base(tag, 0)
        {
        }

        /// <inheritdoc />
        public override RscpDataType DataType => RscpDataType.Void;
    }
}
