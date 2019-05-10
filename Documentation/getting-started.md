# Getting started with WinForms for .NET Core

This document describes the experience of using WinForms on .NET Core. The [Developer Guide](developer-guide.md) describes how to develop features and fixes for Windows Forms.

## Installation

Choose one of these options:

1. [.NET Core 3.0 SDK Preview 1 (recommended)][.net-core-3.0-sdk-preview]

1. [.NET Core 3.0 daily build (latest changes, but less stable)][.net-core-3.0-daily]

## Creating new applications

You can create a new WinForms application with `dotnet new` command, using the following commands:

```cmd
dotnet new winforms -o MyWinFormsApp
cd MyWinFormsApp
dotnet run
```

## Designing Forms

WinForms Core does not yet have a dedicated Designer tool. For the time being, you can use [this workaround](winforms-designer.md).

## Samples

Check out the [.NET Core 3.0 WinForms samples][.net-core-3.0-samples] for both basic and advanced scenarios. Additionally, there is a collection of[ WinForms sample applications on MSDN][MSDN-winforms-samples].

## Porting existing applications

To port your existing WinForms application from .NET Framework to .NET Core 3.0, refer to our [porting guidelines](porting-guidelines.md).

[comment]: <> (URI Links)

[.net-core-3.0-sdk-preview]: https://dotnet.microsoft.com/download/dotnet-core/3.0
[.net-core-3.0-daily]: https://github.com/dotnet/core/blob/master/daily-builds.md
[.net-core-3.0-samples]: https://github.com/dotnet/samples/tree/master/windowsforms
[MSDN-winforms-samples]: https://code.msdn.microsoft.com/site/search?f%5B0%5D.Type=Platform&f%5B0%5D.Value=Desktop&f%5B0%5D.Text=Desktop&f%5B1%5D.Type=Contributors&f%5B1%5D.Value=Microsoft&f%5B1%5D.Text=Microsoft&f%5B2%5D.Type=Technology&f%5B2%5D.Value=Windows%20Forms
