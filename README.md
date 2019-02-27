# chino-dotnet

A C# SDK for using [Chino.io API](https://chino.io) with the .NET framework.

*Read the full docs at https://docs.chino.io*

[See changelog](./CHANGELOG.md)

## Contacts

Issues / help / feedback: <tech-support@chino.io>

Maintainer: [@aarighi](https://github.com/aarighi) <andrea@chino.io>

Developer: [@prempaolo](https://github.com/prempaolo) <prempaolo@gmail.com>

## Requirements

Install requirements using NuGet. To install `Chino.dll`, run the following command in the Package Manager Console:

```PM> Install-Package Chino.dll```

### NOTE

The SDK can not be used in a `Universal Windows Application`.

## Test

Tests can be run using `ChinoTest\UnitTest1.cs`. Only setup required is setting the values of `customerId`, 
`customerKey` and `hostUrl`.