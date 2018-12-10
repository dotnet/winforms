# Getting started with WinForms for .NET Core

This document describes the experience of using WinForms on .NET Core. The [Developer Guide](developer-guide.md) describes how to develop features and fixes for Windows Forms.

## Installation

Choose one of these options:

1. [.NET Core 3.0 SDK Preview 1 (recommended)](https://www.microsoft.com/net/download)
2. [.NET Core 3.0 daily build (latest changes, but less stable)](https://github.com/dotnet/core/blob/master/daily-builds.md)

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

Check out the [.NET Core 3.0 WinForms samples](https://github.com/dotnet/samples/tree/master/windowsforms) for both basic and advanced scenarios.

## Porting existing applications

To port your existing WinForms application from .NET Framework to .NET Core 3.0, refer to our [porting guidelines](porting-guidelines.md).
