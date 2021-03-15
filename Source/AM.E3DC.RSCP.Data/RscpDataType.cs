// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global

namespace AM.E3DC.RSCP.Data
{
    /// <summary>
    /// This enum contains all data types the RSCP values can have.
    /// </summary>
    public enum RscpDataType : byte
    {
        /// <summary>
        /// Value does not contain data.
        /// </summary>
        Void = 0x00,

        /// <summary>
        /// Value is a boolean.
        /// </summary>
        Bool = 0x01,

        /// <summary>
        ///  Value is an 8-bit signed integer (<see cref="sbyte"/>).
        /// </summary>
        Int8 = 0x02,

        /// <summary>
        ///  Value is an 8-bit unsigned integer (<see cref="byte"/>).
        /// </summary>
        UInt8 = 0x03,

        /// <summary>
        ///  Value is an 16-bit signed integer (<see cref="short"/>).
        /// </summary>
        Int16 = 0x04,

        /// <summary>
        ///  Value is an 16-bit unsigned integer (<see cref="ushort"/>).
        /// </summary>
        UInt16 = 0x05,

        /// <summary>
        ///  Value is an 32-bit signed integer (<see cref="int"/>).
        /// </summary>
        Int32 = 0x06,

        /// <summary>
        ///  Value is an 32-bit unsigned integer (<see cref="uint"/>).
        /// </summary>
        UInt32 = 0x07,

        /// <summary>
        ///  Value is an 64-bit signed integer (<see cref="long"/>).
        /// </summary>
        Int64 = 0x08,

        /// <summary>
        ///  Value is an 64-bit unsigned integer (<see cref="ulong"/>).
        /// </summary>
        UInt64 = 0x09,

        /// <summary>
        ///  Value is a single precision (32-bit) float value (<see cref="float"/>).
        /// </summary>
        Float = 0x0A,

        /// <summary>
        ///  Value is a double precision (64-bit) float value (<see cref="double"/>).
        /// </summary>
        Double = 0x0B,

        /// <summary>
        ///  Value is a bitfield (here represented at <see cref="T:byte[]"/>).
        /// </summary>
        Bitfield = 0x0C,

        /// <summary>
        ///  Value is a string (<see cref="string"/>).
        /// </summary>
        String = 0x0D,

        /// <summary>
        /// Value is a container for other values.
        /// </summary>
        Container = 0x0E,

        /// <summary>
        /// Value is a timestamp.
        /// </summary>
        Timestamp = 0x0F,

        /// <summary>
        /// Value is binary data (<see cref="T:byte[]"/>)
        /// </summary>
        ByteArray = 0x10,

        /// <summary>
        /// Value is an error.
        /// </summary>
        Error = 0xFF
    }
}
