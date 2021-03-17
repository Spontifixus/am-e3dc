// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global

namespace AM.E3dc.Rscp.Data
{
    /// <summary>
    /// This enum contains all error codes the S10 can produce.
    /// </summary>
    public enum RscpErrorCode : byte
    {
        /// <summary>
        /// Unhandled error.
        /// </summary>
        NotHandled = 0x01,

        /// <summary>
        /// Authorization error.
        /// </summary>
        AccessDenied = 0x02,

        /// <summary>
        /// Invalid format.
        /// </summary>
        Format = 0x03,

        /// <summary>
        /// Again error (whatever that means).
        /// </summary>
        Again = 0x04
    }
}
