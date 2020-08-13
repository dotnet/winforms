# Getting started with WinForms for .NET Core

This document describes the experience of using WinForms on .NET Core. The [Developer Guide](developer-guide.md) describes how to develop features and fixes for Windows Forms.

## Installation

Choose one of these options:

1. [.NET Core 3.1 SDK (recommended)][.net-core-3.1-sdk]

1. [.NET 5.0 daily build (latest changes, could be less stable)][.net-core-daily]

## Creating new applications

You can create a new WinForms application with `dotnet new` command, using the following commands:

```cmd
dotnet new winforms -o MyWinFormsApp
cd MyWinFormsApp
dotnet run
```

## Designing Forms

You can try the Windows Forms Core Designer Visual Studio extension, see [Windows Forms Designer documentation](designer-releases/readme.md). As an alternative, you can use [this workaround](winforms-designer.md).

## Samples

Check out the [.NET Core WinForms samples][.net-core-samples] for both basic and advanced scenarios. Additionally, there is a collection of[ WinForms sample applications on MSDN][MSDN-winforms-samples].

## Porting existing applications

To port your existing WinForms application from .NET Framework to .NET Core 3.1, refer to our [porting guidelines](porting-guidelines.md).

[comment]: <> (URI Links)

[.net-core-3.1-sdk]: https://dotnet.microsoft.com/download/dotnet-core/3.1
[.net-core-daily]: https://github.com/dotnet/core-sdk/blob/master/README.md#installers-and-binaries
[.net-core-samples]: https://github.com/dotnet/samples/tree/master/windowsforms
[MSDN-winforms-samples]: https://code.msdn.microsoft.com/site/search?f%5B0%5D.Type=Platform&f%5B0%5D.Value=Desktop&f%5B0%5D.Text=Desktop&f%5B1%5D.Type=Contributors&f%5B1%5D.Value=Microsoft&f%5B1%5D.Text=Microsoft&f%5B2%5D.Type=Technology&f%5B2%5D.Value=Windows%20Forms
