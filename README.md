# Windows Forms

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/dotnet/winforms/blob/master/LICENSE.TXT)

Windows Forms (WinForms) is a UI framework for building Windows desktop applications. It is a .NET wrapper over Windows user interface libraries, such as User32 and GDI+. It also offers controls and other functionality that is unique to Windows Forms.

Windows Forms also provides one of the most productive ways to create desktop applications based on the visual designer provided in Visual Studio. It enables drag-and-drop of visual controls and other similar functionality that make it easy to build desktop applications.

## Windows Forms Designer
For more information about the designer, please see the [Windows Forms Designer Documentation](Documentation/winforms-designer.md).

To learn about project priorities as well as status and ship dates see the [Windows Forms Roadmap](roadmap.md).

:warning: This repository contains WinForms for .NET Core. It does not contain the .NET Framework variant of WinForms.

[Windows Presentation Foundation](https://github.com/dotnet/wpf) (WPF) is another UI framework used to build Windows desktop applications which is supported on .NET Core. WPF and Windows Forms applications  run only on Windows operating systems. They are part of the `Microsoft.NET.Sdk.WindowsDesktop` SDK. You are recommended to use Visual Studio 2019 Preview 1 to use WPF and Windows Forms with .NET Core.

## Getting started

* [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1)
* [Getting started instructions][getting-started]
* [Contributing guide][contributing]
* [Porting guide][porting-guidelines]

## Build Status & Dependency Flow

|               | Public CI                                  :arrow_right:  | Internal CI                                    :arrow_right:  | Core Setup CI                                     :arrow_right:  | Core SDK CI                                                   |
|-------------  |---------------------------------------------------------  |-------------------------------------------------------------  |----------------------------------------------------------------  |-------------------------------------------------------------  |
| master        | [![Build Status][master-public-build]][public-build]      | [![Build Status][master-internal-build]][internal-build]      | [![Build Status][master-core-setup-build]][core-setup-build]     | [![Build Status][master-core-sdk-build]][core-sdk-build]      |
| release/3.1   | [![Build Status][release31-public-build]][public-build]   | [![Build Status][release31-internal-build]][internal-build]   | [![Build Status][release31-core-setup-build]][core-setup-build]  | [![Build Status][release31-core-sdk-build]][core-sdk-build]   |
| release/3.0   | [![Build Status][release3-public-build]][public-build]    | [![Build Status][release3-internal-build]][internal-build]    | [![Build Status][release3-core-setup-build]][core-setup-build]   | [![Build Status][release3-core-sdk-build]][core-sdk-build]    |

### Code Coverage

|               | Production Code                                   | 
|-------------  |-------------------------------------------------  |
| master        | [![codecov][master-coverage-prod]][coverage]      |
| release/3.1   | [![codecov][release31-coverage-prod]][coverage]   |
| release/3.0   | [![codecov][release3-coverage-prod]][coverage]    |


## How to Engage, Contribute, and Provide Feedback

Some of the best ways to contribute are to try things out, file bugs, join in design conversations, and fix issues.

* The [contributing guidelines][contributing] and the more general [.NET Core contributing guide](https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/contributing.md) define contributing rules.
* The [Developer Guide](Documentation/developer-guide.md) defines the setup and workflow for working on this repository.
* If you have a question or have found a bug, [file an issue](https://github.com/dotnet/winforms/issues/new?template=bug_report.md).
* Use [daily builds][getting-started] if you want to contribute and stay up to date with the team.

### .NET Framework issues

Issues with .NET Framework, including Windows Forms, should be filed on the [Developer Community](https://developercommunity.visualstudio.com/spaces/61/index.html) or [Product Support](https://support.microsoft.com/en-us/contactus?ws=support) websites. They should not be filed on this repository.

### Reporting security issues

Security issues and bugs should be reported privately via email to the Microsoft Security Response Center (MSRC) <secure@microsoft.com>. You should receive a response within 24 hours. If for some reason you do not, please follow up via email to ensure we received your original message. Further information, including the MSRC PGP key, can be found in the [Security TechCenter](https://www.microsoft.com/msrc/faqs-report-an-issue). Also see info about related [Microsoft .NET Core and ASP.NET Core Bug Bounty Program](https://www.microsoft.com/msrc/bounty-dot-net-core).

## Relationship to .NET Framework

This codebase is a fork of the Windows Forms code in the .NET Framework 4.8. In Windows Forms .NET Core 3.0, we've strived to bring the two runtimes to a parity. However, since then, we've done a number of changes, including [breaking changes](https://docs.microsoft.com/dotnet/core/compatibility/winforms), which diverged the two.

For more information about breaking changes, see the [Porting guide][porting-guidelines].

## Code of Conduct

This project uses the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct) to define expected conduct in our community. Instances of abusive, harassing, or otherwise unacceptable behavior may be reported by contacting a project maintainer at conduct@dotnetfoundation.org.

## License

.NET Core (including the Windows Forms repository) is licensed under the [MIT license](LICENSE.TXT).

## .NET Foundation

.NET Core WinForms is a [.NET Foundation](https://www.dotnetfoundation.org/projects) project.<br />
See the [.NET home repository](https://github.com/Microsoft/dotnet) to find other .NET-related projects.

[getting-started]: Documentation/getting-started.md
[contributing]: Documentation/contributing.md
[porting-guidelines]: Documentation/porting-guidelines.md

[master-public-build]: https://dev.azure.com/dnceng/public/_apis/build/status/267?branchName=master
[release3-public-build]: https://dev.azure.com/dnceng/public/_apis/build/status/267?branchName=release%2f3.0
[release31-public-build]: https://dev.azure.com/dnceng/public/_apis/build/status/267?branchName=release%2f3.1
[public-build]: https://dnceng.visualstudio.com/public/_build?definitionId=267

[master-internal-build]: https://dev.azure.com/dnceng/internal/_apis/build/status/164?branchName=master
[release3-internal-build]: https://dev.azure.com/dnceng/internal/_apis/build/status/164?branchName=release%2f3.0
[release31-internal-build]: https://dev.azure.com/dnceng/internal/_apis/build/status/164?branchName=release%2f3.1
[internal-build]: https://dnceng.visualstudio.com/internal/_build?definitionId=164

[master-core-setup-build]: https://dev.azure.com/dnceng/internal/_apis/build/status/288?branchName=master
[release3-core-setup-build]: https://dev.azure.com/dnceng/internal/_apis/build/status/288?branchName=release%2f3.0
[release31-core-setup-build]: https://dev.azure.com/dnceng/internal/_apis/build/status/288?branchName=release%2f3.1
[core-setup-build]: https://dev.azure.com/dnceng/internal/_build?definitionId=288

[master-core-sdk-build]: https://dev.azure.com/dnceng/internal/_apis/build/status/286?branchName=master
[release3-core-sdk-build]: https://dev.azure.com/dnceng/internal/_apis/build/status/286?branchName=release%2f3.0.1xx
[release31-core-sdk-build]: https://dev.azure.com/dnceng/internal/_apis/build/status/286?branchName=release%2f3.1.1xx
[core-sdk-build]: https://dev.azure.com/dnceng/internal/_build?definitionId=286

[master-coverage-prod]: https://codecov.io/gh/dotnet/winforms/branch/master/graph/badge.svg?flag=production
[release3-coverage-prod]: https://codecov.io/gh/dotnet/winforms/branch/release%2F3.0/graph/badge.svg?flag=production
[release31-coverage-prod]: https://codecov.io/gh/dotnet/winforms/branch/release%2F3.1/graph/badge.svg?flag=production
[coverage]: https://codecov.io/gh/dotnet/winforms
