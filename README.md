# Windows Forms

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/dotnet/winforms/blob/master/LICENSE.TXT)

Windows Forms (WinForms) is a UI framework for building Windows desktop applications. It is a .NET wrapper over Windows user interface libraries, such as User32 and GDI+. It also offers controls and other functionality that is unique to Windows Forms.

Windows Forms also provides one of the most productive ways to create desktop applications based on the visual designer provided in Visual Studio. It enables drag-and-drop of visual controls and other similar functionality that make it easy to build desktop applications.

> Note: The Windows Forms visual designer is not yet available and will be part of a Visual Studio 2019 update. [See here for a workaround invoking the Classic Framework Designer](Documentation/winforms-designer.md).

To learn about project priorities as well as status and ship dates see the [Windows Forms Roadmap](roadmap.md).

This repository contains WinForms for .NET Core. It does not contain the .NET Framework variant of WinForms.

[Windows Presentation Foundation][wpf] (WPF) is another UI framework used to build Windows desktop applications which is supported on .NET Core. WPF and Windows Forms applications  run only on Windows operating systems. They are part of the `Microsoft.NET.Sdk.WindowsDesktop` SDK. You are recommended to use Visual Studio 2019 Preview 1 to use WPF and Windows Forms with .NET Core.

## Getting started

* [.NET Core 3.0 SDK Preview][.net-core-3.0-sdk-preview]
* [Getting started instructions][getting-started]
* [Contributing guide][contributing]
* [Porting guide][porting-guidelines]

## Build Status & Dependency Flow

|               | Public CI                                  :arrow_right:  | Internal CI                                    :arrow_right:  | Core Setup CI                                     :arrow_right:  | Core SDK CI                                                   |
|-------------  |---------------------------------------------------------  |-------------------------------------------------------------  |----------------------------------------------------------------  |-------------------------------------------------------------  |
| master        | [![Build Status][master-public-build]][public-build]      | [![Build Status][master-internal-build]][internal-build]      | [![Build Status][master-core-setup-build]][core-setup-build]     | [![Build Status][master-core-sdk-build]][core-sdk-build]      |
| release/3.0   | [![Build Status][release3-public-build]][public-build]    | [![Build Status][release3-internal-build]][internal-build]    | [![Build Status][release3-core-setup-build]][core-setup-build]   | [![Build Status][release3-core-sdk-build]][core-sdk-build]    |

### Code Coverage

|               | Production Code                                   | Test Code                                         | Combined                                          |
|-------------  |-------------------------------------------------  |-------------------------------------------------  |-------------------------------------------------  |
| master        | [![codecov][master-coverage-prod]][coverage]      | [![codecov][master-coverage-test]][coverage]      | [![codecov][master-coverage-all]][coverage]       |
| release/3.0   | [![codecov][release3-coverage-prod]][coverage]    | [![codecov][release3-coverage-test]][coverage]    | [![codecov][release3-coverage-all]][coverage]     |

## Status

We are in the process of doing four projects with Windows Forms:

1. Port Windows Forms to .NET Core.

1. Publish source to GitHub.

1. Publish (and in some cases write) tests to GitHub and enable automated testing infrastructure.

1. Enable the Visual Studio WinForms designer to work with WinForms running on .NET Core.

The first two tasks are well underway. Most of the source has been published to GitHub although we are still bringing the codebase up to functional and performance parity with .NET Framework.

We have published very few tests and have very limited coverage for PRs at this time as a result. We will be slow in merging PRs as a result. We will add more tests in 2019, however, it will be an incremental process. We welcome test contributions to increase coverage and help us validate PRs more easily.

The Visual Studio WinForms designer is not yet available and will be part of a Visual Studio 2019 update. In short, we need to move to an out-of-proc model (relative to Visual Studio) for the designer.

## How to Engage, Contribute, and Provide Feedback

Some of the best ways to contribute are to try things out, file bugs, join in design conversations, and fix issues.

* The [contributing guidelines][contributing] and the more general [.NET Core contributing guide][corefx-contributing] define contributing rules.
* The [Developer Guide][developing] defines the setup and workflow for working on this repository.
* If you have a question or have found a bug, [file an issue][issue-new].
* Use [daily builds][getting-started] if you want to contribute and stay up to date with the team.

### .NET Framework issues

Issues with .NET Framework, including WinForms, should be filed on [VS developer community][developer-community], or [Product Support][product-support]. They should not be filed on this repository.

### Reporting security issues

Security issues and bugs should be reported privately via email to the Microsoft Security Response Center (MSRC) <secure@microsoft.com>. You should receive a response within 24 hours. If for some reason you do not, please follow up via email to ensure we received your original message. Further information, including the MSRC PGP key, can be found in the [Security TechCenter][faqs-report-an-issue]. Also see info about related [Microsoft .NET Core and ASP.NET Core Bug Bounty Program][bounty-dot-net-core].

## Relationship to .NET Framework

This code base is a fork of the Windows Forms code in the .NET Framework. We intend to release .NET Core 3.0 with Windows Forms having parity with the .NET Framework version. Over time, the two implementations may diverge.

The [Update on .NET Core 3.0 and .NET Framework 4.8][update-on-net-core-3-0-and-net-framework-4-8] provides a good description of the forward-looking differences between .NET Core and .NET Framework.

## Code of Conduct

This project uses the [.NET Foundation Code of Conduct][dotnet-code-of-conduct] to define expected conduct in our community. Instances of abusive, harassing, or otherwise unacceptable behavior may be reported by contacting a project maintainer at conduct@dotnetfoundation.org.

## License

.NET Core (including the Windows Forms repository) is licensed under the [MIT license](LICENSE.TXT).

## .NET Foundation

.NET Core WinForms is a [.NET Foundation][.net-foundation] project.

See the [.NET home repository][dotnet-home] to find other .NET-related projects.

[getting-started]: Documentation/getting-started.md
[contributing]: Documentation/contributing.md
[porting-guidelines]: Documentation/porting-guidelines.md
[developing]: Documentation/developer-guide.md

[wpf]: https://github.com/dotnet/wpf
[.net-core-3.0-sdk-preview]: https://dotnet.microsoft.com/download/dotnet-core/3.0
[corefx-contributing]: https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/contributing.md
[issue-new]: https://github.com/dotnet/winforms/issues/new
[developer-community]: https://developercommunity.visualstudio.com/spaces/61/index.html
[product-support]: https://support.microsoft.com/en-us/contactus?ws=support
[faqs-report-an-issue]: https://www.microsoft.com/msrc/faqs-report-an-issue
[bounty-dot-net-core]: https://www.microsoft.com/msrc/bounty-dot-net-core
[update-on-net-core-3-0-and-net-framework-4-8]: https://blogs.msdn.microsoft.com/dotnet/2018/10/04/update-on-net-core-3-0-and-net-framework-4-8/
[dotnet-code-of-conduct]: https://dotnetfoundation.org/code-of-conduct
[.net-foundation]: https://www.dotnetfoundation.org/projects
[dotnet-home]: https://github.com/Microsoft/dotnet

[master-public-build]: https://dev.azure.com/dnceng/public/_apis/build/status/267?branchName=master
[release3-public-build]: https://dev.azure.com/dnceng/public/_apis/build/status/267?branchName=release%2f3.0
[public-build]: https://dnceng.visualstudio.com/public/_build?definitionId=267

[master-internal-build]: https://dev.azure.com/dnceng/internal/_apis/build/status/164?branchName=master
[release3-internal-build]: https://dev.azure.com/dnceng/internal/_apis/build/status/164?branchName=release%2f3.0
[internal-build]: https://dnceng.visualstudio.com/internal/_build?definitionId=164

[master-core-setup-build]: https://dev.azure.com/dnceng/internal/_apis/build/status/288
[release3-core-setup-build]: https://dev.azure.com/dnceng/internal/_apis/build/status/288?branchName=release%2f3.0
[core-setup-build]: https://dev.azure.com/dnceng/internal/_build?definitionId=288

[master-core-sdk-build]: https://dev.azure.com/dnceng/internal/_apis/build/status/286
[release3-core-sdk-build]: https://dev.azure.com/dnceng/internal/_apis/build/status/286?branchName=release%2f3.0.1xx
[core-sdk-build]: https://dev.azure.com/dnceng/internal/_build?definitionId=286

[master-coverage-prod]: https://codecov.io/gh/dotnet/winforms/branch/master/graph/badge.svg?flag=production
[release3-coverage-prod]: https://codecov.io/gh/dotnet/winforms/branch/release%2F3.0/graph/badge.svg?flag=production

[master-coverage-test]: https://codecov.io/gh/dotnet/winforms/branch/master/graph/badge.svg?flag=test
[release3-coverage-test]: https://codecov.io/gh/dotnet/winforms/branch/release%2F3.0/graph/badge.svg?flag=test

[master-coverage-all]: https://codecov.io/gh/dotnet/winforms/branch/master/graph/badge.svg?
[release3-coverage-all]: https://codecov.io/gh/dotnet/winforms/branch/release%2F3.0/graph/badge.svg?
[coverage]: https://codecov.io/gh/dotnet/winforms
