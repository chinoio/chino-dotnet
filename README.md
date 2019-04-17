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

## Usage

### new Search API

The new Search API will replace the old Search in a future release. 
See how they work on the [Chino.io docs](https://docs.chino.io/#search-api).

To perform a Search with this SDK:

1 - Create a Search query
```c#
// search Documents with 123 < integer_field < 200
DocumentsSearch query = chino.search.documents("<schema_id>")                                   // specify Schema ID
    .setResultType(ResultTypeEnum.OnlyId)                                                       // specify result type
    .addSortRule("<field_name>", OrderEnum.Asc)                                                 // add sort rules
    .with("<integer_field>", FilterOperator.filter(FilterOperatorEnum.GreaterThan), 123)
    .and("<integer_field>", FilterOperator.filter(FilterOperatorEnum.LowerEqual), 200)
    .buildSearch()
```
    
2 - Execute query:
```c#
GetDocumentsResponse results = query.execute();
```

3 - Read results:
```c#
Console.WriteLine($"Read { result.count } documents of { result.total_count } total results.");
List<Documents> = results.documents;
```


## Test
A simple test suite is provided, which can be found in `ChinoTest\UnitTest1.cs`.

***WARNING: running Unit Tests will delete everything on the Chino.io account!*** 
If you still want to run the test, add `automated_test` to your environment variables:

- `automated_tests=allow`: enable all tests
- `automated_tests=dotnet`: enable only tests of this SDK

You will also need to set the values of `customer_id` and `customer_key` in your environment variables.
If you don't have those credentials, register a free account at 
[https://console.test.chino.io/](https://console.test.chino.io/).

You may optionaly set the value of `host`, otherwise it will default to `https://api.test.chino.io/v1` 
(mind the */v1* at the end).

**NOTE:** If you are using an IDE other than Visual Studio (e.g. IntelliJ Rider),
you may need to install also VisualStudio binaries for running Unit Tests.