# Getting started with WinForms for .NET Core

This document describes the experience of using WinForms on .NET Core. The [Developer Guide][developer-guide]describes how to develop features and fixes for Windows Forms.

## Installation

Choose one of these options:

1. [.NET Core 3.0 SDK Preview 1 (recommended)][.net-core-3.0-sdk-preview-1]
2. [.NET Core 3.0 daily build (latest changes, but less stable)][.net-core-3.0-daily]

## Creating new applications

You can create a new WinForms application with `dotnet new` command, using the following commands:

```cmd
dotnet new winforms -o MyWinFormsApp
cd MyWinFormsApp
dotnet run
```

## Designing Forms

WinForms Core does not yet have a dedicated Designer tool. For the time being, you can use [this workaround][winforms-designer].

## Samples

Check out the [.NET Core 3.0 WinForms samples][.net-core-3.0-samples] for both basic and advanced scenarios.

## Porting existing applications

To port your existing WinForms application from .NET Framework to .NET Core 3.0, refer to our [porting guidelines][porting-guidelines].

[comment]: <> (Links)

[developer-guide]: developer-guide.md
[.net-core-3.0-sdk-preview-1]: https://www.microsoft.com/net/download
[.net-core-3.0-daily]: https://github.com/dotnet/core/blob/master/daily-builds.md
[winforms-designer]: winforms-designer.md
[.net-core-3.0-samples]: https://github.com/dotnet/samples/tree/master/windowsforms
[porting-guidelines]: porting-guidelines.md