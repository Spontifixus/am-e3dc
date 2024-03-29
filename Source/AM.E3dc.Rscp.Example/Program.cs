﻿using System;
using System.Net;
using System.Threading.Tasks;
using AM.E3dc.Rscp.Data;
using AM.E3dc.Rscp.Data.Values;
using Microsoft.Extensions.Logging;

namespace AM.E3dc.Rscp.Example
{
    /// <summary>
    /// This class provides the logic for the example application.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Entry point for the example application.
        /// </summary>
        /// <returns>A <see cref="Task"/> that can be awaited.</returns>
        public static async Task Main()
        {
            const string e3dcIpAddress = "192.168.0.2";
            const string e3dcUserName = "your-e3dc-portal-username@example.com";
            const string e3dcPassword = "your-e3dc-portal-password";
            const string rscpPassword = "your-rscp-password";

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSimpleConsole(c => c.SingleLine = true);
                builder.SetMinimumLevel(LogLevel.Trace);
            });

            var e3dcLogger = loggerFactory.CreateLogger<E3dcConnection>();
            var logger = loggerFactory.CreateLogger("Program");
            logger.LogInformation("Logging configured.");

            logger.LogInformation("Setting up connection to E3/DC power station...");
            var e3dcConnection = new E3dcConnection(e3dcLogger);
            var endpoint = new IPEndPoint(IPAddress.Parse(e3dcIpAddress), 5033);
            await e3dcConnection.ConnectAsync(endpoint, rscpPassword);

            logger.LogInformation("Starting authorization for user '{userName}'...", e3dcUserName);
            var authFrame = new RscpFrame
            {
                new RscpContainer(RscpTag.RSCP_REQ_AUTHENTICATION)
                {
                    new RscpString(RscpTag.RSCP_AUTHENTICATION_USER, e3dcUserName),
                    new RscpString(RscpTag.RSCP_AUTHENTICATION_PASSWORD, e3dcPassword)
                }
            };

            var response = await e3dcConnection.SendAsync(authFrame);
            if (response.Values[0] is RscpUInt8 userLevelValue)
            {
                var userLevel = (RscpUserLevel)userLevelValue.Value;
                logger.LogInformation("Authorization of '{userName}' successful (UserLevel: {userLevel}).", e3dcUserName, userLevel);
            }
            else
            {
                logger.LogWarning("Authorization of '{userName}' failed.", e3dcUserName);
            }

            logger.LogInformation("Loading battery charge cycles...");
            var requestFrame = new RscpFrame
            {
                new RscpContainer(RscpTag.BAT_REQ_DATA)
                {
                    new RscpInt32(RscpTag.BAT_INDEX, 0),
                    new RscpVoid(RscpTag.BAT_REQ_CHARGE_CYCLES)
                }
            };
            var responseFrame = await e3dcConnection.SendAsync(requestFrame);

            var chargeCycles = responseFrame.Get<RscpUInt32>(RscpTag.BAT_CHARGE_CYCLES).Value;
            logger.LogInformation($"Battery has undergone {chargeCycles} charge cycles.");

            Console.ReadLine();
        }
    }
}
