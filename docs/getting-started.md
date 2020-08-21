# Getting started with Windows Forms for .NET

This document describes the experience of using Windows Forms on .NET. The [Developer Guide](developer-guide.md) describes how to develop features and fixes for Windows Forms.

## Installation

Choose one of these options:

1. [.NET 5.0 SDK (recommended)][.net-5.0-sdk]

1. [.NET 6.0 daily build (latest changes, could be less stable)][.net-daily]

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

Check out the [.NET Core/.NET Windows Forms samples][.net-samples] for both basic and advanced scenarios. Additionally, there is a collection of [Windows Forms sample applications on MSDN][MSDN-winforms-samples].

## Porting existing applications

To port your existing Windows Forms application from .NET Framework to .NET Core 3.1 or .NET 5.0, refer to our [porting guidelines](porting-guidelines.md).

[comment]: <> (URI Links)

[.net-5.0-sdk]: https://dotnet.microsoft.com/download/dotnet/5.0
[.net-daily]: https://github.com/dotnet/core-sdk/blob/master/README.md#installers-and-binaries
[.net-samples]: https://github.com/dotnet/samples/tree/master/windowsforms
[MSDN-winforms-samples]: https://code.msdn.microsoft.com/site/search?f%5B0%5D.Type=Platform&f%5B0%5D.Value=Desktop&f%5B0%5D.Text=Desktop&f%5B1%5D.Type=Contributors&f%5B1%5D.Value=Microsoft&f%5B1%5D.Text=Microsoft&f%5B2%5D.Type=Technology&f%5B2%5D.Value=Windows%20Forms
