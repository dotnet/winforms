# Windows Forms

Windows Forms (WinForms) is a framework for building rich Windows desktop applications using .NET. Since it leverages a what-you-see-is-what-you-get (WYSIWYG) graphical editor with drag & drop, it's also one of the easiest and most productive ways to create desktop applications.

This repo contains the open-source components of WinForms that run on top of .NET Core. It is based on, but separate from, the version of WinForms that is part of the .NET Framework.

We haven't finished porting WinForms  to .NET Core yet, which means not all source code is on GitHub yet. You can expect us to complete this over the next months. The reason it takes some time is that we need to support & build all the pieces in an open source way, which requires decoupling the code base from our internal engineering system. At the same time, we don't want to block open sourcing until the port is complete either. This is similar to how other repos in .NET Core with existing code have been brought up, such as [CoreFx](https://github.com/dotnet/corefx).

Even though .NET Core is a cross-platform technology, WinForms only runs on Windows.

## What are the benefits of building WinForm apps with .NET Core?

There are three primary reasons for considering .NET Core over the .NET Framework for building desktop apps with WinForms:

1. **More deployment options**. You can deploy .NET Core  side-by-side or even produce self-contained apps that you can just XCOPY deploy.

2. **Contains features that we can't provide in .NET Framework**. [Due to the fact][post] that .NET Framework is deployed as an in-place update, the compatibility is extremely high, which prevents us from making fundamental changes, such as performance improvements or making the UI more high-DPI aware by default.

3. **.NET Core goodness**. As an open source platform, .NET Core is receiving a lot of contributions from the open source community. As an application writer, you also benefit from those when building WinForms apps with .NET Core.

To learn more read the blog post [Update on .NET Core 3.0 and .NET Framework 4.8](https://blogs.msdn.microsoft.com/dotnet/2018/10/04/update-on-net-core-3-0-and-net-framework-4-8/)

## Installation

Install [.NET Core 3.0 SDK Preview 1](https://www.microsoft.com/net/download).

[Daily builds](https://aka.ms/netcore3sdk) are also available in the [dotnet/code-sdk repo](https://github.com/dotnet/core-sdk).

To use **WinForms Designer**, you'll need Visual Studio 2017 Update 15.8 or higher. You can install it from [https://visualstudio.microsoft.com/downloads/](https://visualstudio.microsoft.com/downloads/), selecting the **.NET desktop development** workload with the options: **.NET Framework 4.7.2 development tools** and **.NET Core 3.0 development tools**. 

## Creating new application

You can create a new WinForms application with `dotnet new` command, using the new templates for WinForms.

In your favorite console run:
```cmd
dotnet new winforms -o MyWinFormsApp
cd MyWinFormsApp
dotnet run
```

## Porting existing application

To port your existing WinForms application from .NET Framework to .NET Core 3.0 use our [porting guidelines](https://github.com/OliaG/winforms/blob/master/Documentation/porting-guidelines.md).

## Contributing

Since we're currently still porting parts of the code to GitHub, we're not equipped to handle larger contributions yet. Our goal is to accept contributions on day 1, but we'll only be able to accept minor modifications (typos, build fixes, test breaks etc). We'll be happy to hear your feedback!

* For WinForms on .NET Core you can simply file bugs and feature requests in [this repo](https://github.com/dotnet/winforms/issues/new).
* For WinForms on .NET Framework, please us the existing [Visual Studio developer community](https://developercommunity.visualstudio.com/spaces/61/index.html).
* For .NET Core 3 in general, use the [.NET Core](https://github.com/dotnet/core/issues/) repo.

[.NET Core Contribution Guidelines](https://github.com/dotnet/coreclr/blob/master/Documentation/project-docs/contributing.md).

## Community

This project has adopted the code of conduct defined by the [Contributor Covenant](https://contributor-covenant.org/) to clarify expected behavior in our community.
For more information, see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

## Samples

Check out the WinForms [samples](https://github.com/dotnet/samples/tree/master/windowsforms) for Hello World examples and more advanced scenarios.

## License

Code in this repo is licensed under the [MIT license](LICENSE).

## .NET Foundation

.NET Core WinForms is a [.NET Foundation](https://www.dotnetfoundation.org/projects) project.

There are many .NET related projects on GitHub.

- [.NET home repo](https://github.com/Microsoft/dotnet) - links to 100s of .NET projects, from Microsoft and the community.