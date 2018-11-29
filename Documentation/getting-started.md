# Getting started with WinForms for .NET Core

## Installation

Choose one of these options:

1. Official public preview [.NET Core 3.0 SDK Preview 1](https://www.microsoft.com/net/download), or
2. [Daily build](https://aka.ms/netcore3sdk) (for more installer options see [dotnet/code-sdk repo](https://github.com/dotnet/core-sdk)).

To use the **WinForms designer**, you'll need Visual Studio. We recommend [Visual Studio 2019 Preview 1](https://visualstudio.microsoft.com/downloads/).
Select the **.NET desktop development** workload with the options: **.NET Framework 4.7.2 development tools** and **.NET Core 3.0 development tools**.

## Creating new applications

You can create a new WinForms application with `dotnet new` command, using the new templates for WinForms.

In your favorite console run:
```cmd
dotnet new winforms -o MyWinFormsApp
cd MyWinFormsApp
dotnet run
```

## Porting existing applications

To port your existing WinForms application from .NET Framework to .NET Core 3.0, refer to our [porting guidelines](porting-guidelines.md).

## Samples

Check out the .NET Core 3.0 WinForms [samples](https://github.com/dotnet/samples/tree/master/windowsforms) 
for HelloWorld examples and more advanced scenarios.

## How to build
See [building guidelines](building-guidelines.md)

## How to test
See [testing guidelines](testing-guidelines.md).

