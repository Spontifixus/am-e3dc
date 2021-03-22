# alphamarkus E3DC RSC<span>P.N</span>ET

This library enables you to access your [E3/DC S10 power station](https://www.e3dc.com/produkte) via the RSCP protocol.

It targets .NET Standard 2.1 and can thus be used by all current .NET versions.

## Installation

You can add the library to your project's references using either the nuget package manager of visual studio or run the following command:

    dotnet add package AM.E3dc.Rscp

## Usage

Using this library is simple - it provides a set of asynchronous methods to communicate with your power station.

To connect to the E3/DC power station create an instance of the `E3dcConnector`
 class. Then invoke the `ConnectAsync` method to establish a connection. Once the connection was created you can send requests to your power station using the `SendAsync` method:

The following code snippet shows how to do this:

```csharp
const string e3dcIpAddress = "192.168.0.2";
const string e3dcUserName = "your-e3dc-portal-username@example.com";
const string e3dcPassword = "your-e3dc-portal-password";
const string rscpPassword = "your-rscp-password";

// Create the connector
var e3dcConnection = new E3dcConnection();

// Connect to the E3/DC power station
var endpoint = new IPEndPoint(IPAddress.Parse(e3dcIpAddress), 5033);
await e3dcConnection.ConnectAsync(endpoint, rscpPassword);

// Build the authorization frame
var authFrame = new RscpFrame();
var authContainer = new RscpContainer(RscpTag.TAG_RSCP_REQ_AUTHENTICATION);
authContainer.Add(new RscpString(RscpTag.TAG_RSCP_AUTHENTICATION_USER, e3dcUserName));
authContainer.Add(new RscpString(RscpTag.TAG_RSCP_AUTHENTICATION_PASSWORD, e3dcPassword));
authFrame.Add(authContainer);

// Send the frame to the power station and await the response
var response = await e3dcConnection.SendAsync(authFrame);
response.TryGetValue<RscpUInt8>(RscpTag.TAG_RSCP_AUTHENTICATION, out var authResponse);

var userLevel = (RscpUserLevel)authResponse.Value;
```

### Logging

You can pass an instance of an `ILogger<E3dcConnector>` to the `E3dcConnector` constructor to generate logging outputs. To create a logger reference one of the [Microsoft.Extensions.Logging](https://www.nuget.org/packages?q=Microsoft.Extensions.Logging) packages first.

```csharp
using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddSimpleConsole(c => c.SingleLine = true);
    builder.SetMinimumLevel(LogLevel.Trace);
});

var e3dcLogger = loggerFactory.CreateLogger<E3dcConnection>();
var e3dcConnection = new E3dcConnection(e3dcLogger);
```

### Creating values

The library provides value classes for all data types that are provided by the protocol. Additionally enums for data types, tags, errors and user level are provided.

To create a value create an instance of the respective type, passing the tag and the value to its constructor:

```csharp
var stringValue = new RscpString(RscpTags.TAG_RSCP_SET_ENCRYPTION_PASSPHRASE, "Hello World");
var intValue = new RscpInt32(RscpTags.TAG_EMS_REQ_BAT_SOC, 42);
```

E3DC provides value containers that can bundle multiple values. You can see an example for use of an `RscpContainer` class in the example code in the last chapter.

To add a value to a frame use the `Add` method of the `RscpFrame` class. Containers and frames can both handle multiple values.

### Reading values

To access a value from the power station's response you can either address the value by both its tag and its type:

```csharp
var result = responseFrame.GetValue<RscpInt32>(RscpTag.TAG_BAT_CHARGE_CYCLES);
```

> **Note:** Sometimes the RSCP protocol acts weird. While the response to a successful authorization attempt is an `RscpUInt8` value, it returns an `RscpUInt32` if the authorization failed...

In other cases, for example if you want to retrieve a list of error messages, you'd rather fetch all values:

```csharp
var responseValues = responseFrame.GetValues();
```

You can access the value's value by means of the `Value` property. If the value is an `RscpContainer` that contains other values you need to use its `Children` property.

### Errors

If the power station's response contains an error, the `HasError` property of the frame will be set to `true`. You can then access the errors using the `GetErrors` method:

```csharp
if (responseFrame.HasError)
{
    var errors = responseFrame.GetErrors();
}
```

The resulting values will be of type `RscpError`, and its value will be an `RscpErrorCode` allowing you to find out what went wrong.

### Timestamps

The RSCP protocol handles timestamps in its own way, by storing the seconds and nanoseconds that have elapsed since the Unix-epoch (January 1st, 1970). To handle timestamps use the 'RscpTime' value class, that accepts a 'DateTime':

```csharp
var timeValue = new RscpTime(DateTime.Now);
```

This converts the `DateTime` object to the correct representation internally.

## Example

You can find a small example project in the folder [/Source/Example](/Source/Example).

## License

This library is licensed under the [Apache License Version 2.0](LICENSE). I'd be happy if you give a shout out should you use my library!

## Remarks

Thanks to [@bvotteler](https://github.com/bvotteler/), who created [rscp-e3dc](https://github.com/bvotteler/rscp-e3dc), a Java implementation of the RSCP protocol. His code helped me remove the last bugs from my encryption implementation.

Why "alphamarkus"? I thought this account was named "Spontifixus"? You're right. But alphamarkus.de was what I named my first website - and I still go with this name for software projects.
