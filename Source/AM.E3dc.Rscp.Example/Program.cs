using System.Net;
using System.Threading.Tasks;
using AM.E3dc.Rscp.Data;
using AM.E3dc.Rscp.Data.Values;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace AM.E3dc.Rscp.Example
{
    /// <summary>
    /// Ja blah.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Genau.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Main()
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddSimpleConsole(
                        o =>
                        {
                            o.TimestampFormat = "dd HH: mm:ss.fff ";
                            o.ColorBehavior = LoggerColorBehavior.Disabled;
                            o.SingleLine = true;
                        });

                    builder.SetMinimumLevel(LogLevel.Trace);
                });

            var logger = loggerFactory.CreateLogger<E3dcConnection>();

            var e3dcConnection = new E3dcConnection(logger);

            var endpoint = new IPEndPoint(IPAddress.Parse("192.168.0.101"), 5033);
            await e3dcConnection.ConnectAsync(endpoint, "your-rscp-password");

            var authFrame = new RscpFrame();
            var authContainer = new RscpContainer(RscpTag.TAG_RSCP_REQ_AUTHENTICATION);
            authContainer.Add(new RscpString(RscpTag.TAG_RSCP_AUTHENTICATION_USER, "your-e3dc-portal@example.com"));
            authContainer.Add(new RscpString(RscpTag.TAG_RSCP_AUTHENTICATION_PASSWORD, "your-e3-dc-portal-password"));
            authFrame.Add(authContainer);

            var response = await e3dcConnection.SendAsync(authFrame);

            response.TryGetValue<RscpUInt8>(RscpTag.TAG_RSCP_AUTHENTICATION, out var authResponse);

            var userLevel = (RscpUserLevel)authResponse.Value;
        }
    }
}
