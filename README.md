# chino-dotnet

A C# SDK for using [Chino.io API](https://chino.io) with the .NET framework.

*Read the full docs at https://docs.chino.io*

**Note:** The SDK is still in a beta phase. If something is wrong, please 
[open an issue](https://github.com/chinoio/chino-dotnet/issues/new) on Github.

[See changelog](./CHANGELOG.md)

## Contacts

Issues / help / feedback: <tech-support@chino.io>

Maintainer (from v0.1): [@aarighi](https://github.com/aarighi) <andrea@chino.io>

Developer: [@prempaolo](https://github.com/prempaolo) <prempaolo@gmail.com>

## Requirements

Install requirements using NuGet. To install `Chino.dll`, run the following command in the Package Manager Console:

```PM> Install-Package Chino.dll```

**NOTE:** The SDK can not be used in a `Universal Windows Application`.

## Test

Tests can be run using `ChinoTest\UnitTest1.cs`.

Only setup required is setting the values of `customer_id`, `customer_key` and `host` in your environment variables.
If you don't have those credentials, register at [https://console.test.chino.io/](https://console.test.chino.io/) .

**NOTE:** If you are using an IDE other than Visual Studio (e.g. IntelliJ Rider),
you may need to install also VisualStudio for running Unit Tests.