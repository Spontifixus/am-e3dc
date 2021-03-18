// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global

namespace AM.E3dc.Rscp.Data
{
    /// <summary>
    /// This enum contains the user level an authorized user can have.
    /// </summary>
    public enum RscpUserLevel : byte
    {
        /// <summary>
        /// The current user is not authorized.
        /// </summary>
        NotAuthorized = 0,

        /// <summary>
        /// Normal user level.
        /// </summary>
        User = 10,

        /// <summary>
        /// Installer access.
        /// </summary>
        Installer = 20,

        /// <summary>
        /// Service access.
        /// </summary>
        Service = 30,

        /// <summary>
        /// Administrator access,
        /// </summary>
        Admin = 40,

        /// <summary>
        /// Access for E3/DC.
        /// </summary>
        E3dc = 50,

        /// <summary>
        /// Root access for E3/DC.
        /// </summary>
        E3dcRoot = 60
    }
}
