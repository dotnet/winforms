# Windows Forms
 [![Build status](https://dnceng.visualstudio.com/7ea9116e-9fac-403d-b258-b31fcf1bb293/_apis/build/status/199?branchName=master)](https://dnceng.visualstudio.com/internal/_build/latest?definitionId=199&branch=master)

Windows Forms (WinForms) is a UI framework for building Windows desktop applications. It is a .NET wrapper over Windows user interface libraries, such as User32 and GDI+. It also offers controls and other functionality that is unique to Windows Forms.

WinForms applications typically have good performance because WinForms is a thin layer over Windows for much of the functionality. It also provides one of the most productive ways to create desktop applications based on the visual designer provided in Visual Studio. It enable drag-and-drop of visual controls and other similar functionality that make it easy to build desktop applications.

> Note: The visual designer is not yet available and will be part of a Visual Studio 2019 update.

This repo contains WinForms for .NET Core. It does not contain the .NET Framework variant of WinForms.

[WPF](https://github.com/dotnet/wpf) is another UI framework for building Windows desktop applications that is supported on .NET Core. WPF and WinForms applications only run on Windows. They are part of the `Microsoft.NET.Sdk.WindowsDesktop` SDK.

## Getting started

* [.NET Core 3.0 SDK Preview 1](https://www.microsoft.com/net/download)
* [Getting started instructions](Documentation/getting-started.md)
* [Porting guide](Documentation/porting-guide.md)
* [Contributing guide](Documentation/contributing-guide.md)

## Status

We are in the process of doing four projects with Windows Forms:

* Port Windows Forms to .NET Core.
* Publish source to GitHub.
* Publish (and in some cases write) tests to GitHub and enable automated testing infrastructure.
* Enable the Visual Studio WinForms designer to work with WinForms running on .NET Core.

We first two tasks are well underway. Most of the source has been published to GitHub although we are still bringing the codebase up to functional and performance parity with .NET Framework. 

We have published very few tests and have very limited coverage for PRs at this time as a result. We will be slow in merging PRs as a result. We will add more tests in 2019, however, it will be a progressive process. We welcome test contributions to increase coverage and help us validate PRs more easily. 

The Visual Studio WinForms designer is not yet available. In short, we need to move to an out-of-proc model (relative to Visual Studio) with the designer. This work will be part of Visual Studio 2019.

See [.NET Core roadmap](https://github.com/dotnet/core/blob/master/roadmap.md) for information on ship dates and priorities.

## How to Engage, Contribute and Provide Feedback

Some of the best ways to contribute are to try things out, file bugs, join in design conversations, and fix issues.

* This repo defines [contributing guidelines](Documentation/contributing-guide.md) and also follows the more general[.NET Core contributing guide](https://github.com/dotnet/coreclr/blob/master/Documentation/project-docs/contributing.md).
* If you have a question or have found a bug, [file an issue](https://github.com/dotnet/winforms/issues/new).
* Use [daily builds](Documentation/getting-started.md#installation) if you want to contribute and stay up to date with the team.

Security issues and bugs should be reported privately via email to the Microsoft Security Response Center (MSRC) <secure@microsoft.com>. You should receive a response within 24 hours. If for some reason you do not, please follow up via email to ensure we received your original message. Further information, including the MSRC PGP key, can be found in the [Security TechCenter](https://www.microsoft.com/msrc/faqs-report-an-issue). Also see info about related [Microsoft .NET Core and ASP.NET Core Bug Bounty Program](https://www.microsoft.com/msrc/bounty-dot-net-core).

Issues with .NET Framework, including WinForms, should be filed on [VS developer community](https://developercommunity.visualstudio.com/spaces/61/index.html), or [Product Support](https://support.microsoft.com/en-us/contactus?ws=support). They should not be filed on this repo.

## Relationship to .NET Framework

This code base is a fork of the Windows Forms code in the .NET Framework. We intend to release .NET Core 3.0 with Windows Forms having parity with the .NET Framework version. Over time, the two implementations may diverge.

The [Update on .NET Core 3.0 and .NET Framework 4.8](https://blogs.msdn.microsoft.com/dotnet/2018/10/04/update-on-net-core-3-0-and-net-framework-4-8/) provides a good description of the forward-looking differences between .NET Core and .NET Framework.

## Code of Conduct

This project uses the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct) to define expected conduct in our community. Instances of abusive, harassing, or otherwise unacceptable behavior may be reported by contacting a project maintainer at conduct@dotnetfoundation.org.

## License

.NET Core (including WinForms repo) is licensed under the [MIT license](LICENSE.TXT).

## .NET Foundation

.NET Core WinForms is a [.NET Foundation](https://www.dotnetfoundation.org/projects) project.

There are many .NET related projects on GitHub.

- [.NET home repo](https://github.com/Microsoft/dotnet) - links to 100s of .NET
  projects, from Microsoft and the community.
