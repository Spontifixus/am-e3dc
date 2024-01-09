using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AM.E3dc.Rscp.Data.Values;

namespace AM.E3dc.Rscp.Data
{
    /// <summary>
    /// This class provides extensions for RSCP data classes.
    /// </summary>
    public static class RscpExtensions
    {
        /// <summary>
        /// Recursively gets a value from an <see cref="RscpFrame" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="frame">The frame to be searched.</param>
        /// <param name="tag">The requested tag.</param>
        /// <returns>The requested value or null.</returns>
        public static TValue Get<TValue>(this RscpFrame frame, RscpTag tag)
        {
            return frame.Values.Get<TValue>(tag);
        }

        /// <summary>
        /// Recursively gets a value from a list or <see cref="RscpValue" />s.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="values">The values to be searched.</param>
        /// <param name="tag">The requested tag.</param>
        /// <returns>The requested value or null.</returns>
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "Reviewed.")]
        public static TValue Get<TValue>(this IEnumerable<RscpValue> values, RscpTag tag)
        {
            var result = values
                .Where(v => v.Tag == tag)
                .OfType<TValue>()
                .FirstOrDefault();

            if (result != null)
            {
                return result;
            }

            foreach (var value in values.OfType<RscpContainer>())
            {
                result = value.Get<TValue>(tag);
                if (result != null)
                {
                    return result;
                }
            }

            return default;
        }

        /// <summary>
        /// Recursively gets a value from an <see cref="RscpContainer" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="container">The container to be searched.</param>
        /// <param name="tag">The requested tag.</param>
        /// <returns>The requested value or null.</returns>
        public static TValue Get<TValue>(this RscpContainer container, RscpTag tag)
        {
            return container.Children.Get<TValue>(tag);
        }
    }
}
