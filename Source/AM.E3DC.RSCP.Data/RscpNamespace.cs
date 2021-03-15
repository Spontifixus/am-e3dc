// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global

namespace AM.E3DC.RSCP.Data
{
    /// <summary>
    /// This enum provides all namespaces for the messages.
    /// </summary>
    public enum RscpNamespace : byte
    {
        /// <summary>
        /// Namespace for RSCP protocol messages.
        /// </summary>
        RSCP = 0x00,

        /// <summary>
        /// Namespace for Energy Management tags
        /// </summary>
        EMS = 0x01,

        /// <summary>
        /// Namespace for Photovoltaic Inverter messages.
        /// </summary>
        PVI = 0x02,

        /// <summary>
        /// Namespace for Battery messages.
        /// </summary>
        BAT = 0x03,

        /// <summary>
        /// Namespace for Battery DCDC-system messages.
        /// </summary>
        DCDC = 0x04,

        /// <summary>
        /// Namespace for Power Meter messages.
        /// </summary>
        PM = 0x05,

        /// <summary>
        /// Namespace for history DataBase messages.
        /// </summary>
        DB = 0x06,

        /// <summary>
        /// Namespace for FMS (Facility Management?) messages.
        /// </summary>
        FMS = 0x07,

        /// <summary>
        /// Namespace for Service messages.
        /// </summary>
        SRV = 0x08,

        /// <summary>
        /// Namespace for Home Automation messages.
        /// </summary>
        HA = 0x09,

        /// <summary>
        /// Namespace for Information messages.
        /// </summary>
        INFO = 0x0A,

        /// <summary>
        /// Namespace for Emergency Power messages.
        /// </summary>
        EP = 0x0B,

        /// <summary>
        /// Namespace for System messages.
        /// </summary>
        SYS = 0x0C,

        /// <summary>
        /// Namespace for Update Management messages.
        /// </summary>
        UM = 0x0D,

        /// <summary>
        /// Namespace for WallBox messages.
        /// </summary>
        WB = 0x0E
    }
}
