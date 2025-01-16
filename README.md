# Windows Forms

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/dotnet/winforms/blob/main/LICENSE.TXT)

Windows Forms (WinForms) is a UI framework for building Windows desktop applications. It is a .NET wrapper over Windows user interface libraries, such as User32 and GDI+. It also offers controls and other functionality that is unique to Windows Forms.

Windows Forms also provides one of the most productive ways to create desktop applications based on the visual designer provided in Visual Studio. It enables drag-and-drop of visual controls and other similar functionality that make it easy to build desktop applications.

## Windows Forms Out-Of-Process Designer

For information about the WinForms Designer supporting the .NET runtime and the changes between the .NET Framework Designer (supporting .NET Framework up to version 4.8.1) vs. the .NET Designer (supporting .NET 6, 7, 8, 9+), please see [Windows Forms Designer Documentation](https://learn.microsoft.com/dotnet/desktop/winforms/controls-design/designer-differences-framework?view=netdesktop-8.0).

**Important:** As a Third Party Control Vendor, when you migrate controls from .NET Framework to .NET, your control libraries _at runtime_ are expected to work as before in the context of the respective new TFM (special modernization or security changes in the TFM kept aside, but those are rare breaking changes). Depending on the richness of your control's design-time support, the migration of control designers from .NET Framework to .NET might need to take a series of areas with breaking changes into account. The provided link points out additional resources which help in that migration process.

## Relationship to .NET Framework

This codebase is a fork of the Windows Forms code in the .NET Framework 4.8. 
We started the migration process by targeting .NET Core 3.0, when we've strived to bring the two runtimes to a parity. Since then, we've done a number of changes, including [breaking changes](https://docs.microsoft.com/dotnet/core/compatibility/winforms), which diverged the two. For more information about breaking changes, see the [Porting guide][porting-guidelines].

## The bar for innovation and new features

WinForms is a technology which was originally introduced as a part of .NET Framework 1.0 on February 13th, 2002. It's primary focus was and is to be a Rapid Application Tool for Windows based Apps, and that principal sentiment has not changed over the years. WinForms at the time addressed developer's requests for

* A framework for stable, monolithic Line of Business Apps, even with extremely complicated and complex domain-specific workflows
* The ability to easily provide rich user interfaces
* A safe and - over the first 3 versions of .NET Framework - increasingly performant way to communicate across process boundaries via various Windows Communication Services, or access on-site databases via ADO.NET providers.
* A very easy to use, visual what-you-see-is-what-you-get designer, which requires little ramp-up time, and was primarily focused to support 96 DPI resolution-based, pixel-coordinated drag & drop design strategies.
* A flexible, .NET reflection-based Designer extensibility model, utilizing the .NET Component Model.
* Visual Controls and Components, which provide their own design-time functionality through Control Designers

Over time, and with a growing need to address working scenarios with multi-monitor, high resolution monitors, significantly more powerful hardware, and much more, WinForms has continued to be modernized.

And then there is the evolution of Windows: When new versions of Windows introduce new or change existing APIs or technologies - WinForms needs to keep up and adjust their APIs accordingly.

And exactly **that** is still the primary motivation for once to modernize and innovate, but also the bar to reach for potential innovation areas we either need or want to consider:

* Areas, where for example for security concerns, the Windows team needed to take an depending area out-of-proc, and we see and extreme performance hit in WinForms Apps running under a new Service Pack or a new Windows Version
* New features to comply with updated industry standards for accessibility.
* HighDPI and per Monitor V2-Scenarios.
* Picking up changed or extended Win32 Control functionality, to keep controls in WinForms working the way the Windows team wants them to be used.
* Addressing Performance and Security issues
* Introducing ways to support asynchronous calls interatively, to enable apps to pick up migration paths via Windows APIs projection/Windows Desktop Bridge, enable scenarios for async WebAPI, SignalR, Azure Function, etc. calls, so WinForms backends can modernized and even migrated to the cloud.

What would not make the bar: 
* New functionality which modern Desktop UIs like WPF or WinUI clearly have already
* Functionality, which would "stretch" a Windows Desktop App to be a mobile, Multi-Media or IoT app.
* Domain-specific custom controls, which are already provided by the vast variety of third party control vendors

**A note about Visual Basic**: Visual Basic .NET developers make up about 20% of WinForms developers. We welcome changes that are specific to VB if they address a bug in a customer-facing scenario. Issues and PRs should describe the customer-facing scenario and, if possible, include images showing the problem before and after the proposed changes. Due to limited bandwidth, we cannot prioritize VB-specific changes that are solely for correctness or code cleanliness. However, VB remains important to us, and we aim to fix any critical issues that arise.

## Please note

:warning: This repository contains only implementations for Windows Forms for [.NET platform](https://github.com/dotnet/core).<br />
It does not contain either:
* The .NET Framework variant of Windows Forms. Issues with .NET Framework, including Windows Forms, should be filed on the [Developer Community](https://developercommunity.visualstudio.com/spaces/61/index.html) or [Product Support](https://support.microsoft.com/en-us/contactus?ws=support) websites. They should not be filed on this repository.
* The Windows Forms Designer implementations. Issues with the Designer can be filed via VS Feedback tool (top right-hand side icon in Visual Studio) or be filed in this repo using the Windows Forms out-of-process designer issue template.

# How can I contribute?

We welcome contributions! Many people all over the world have helped make this project better.

* [Contributing][contributing] explains what kinds of changes we welcome
* [Developer Guide][developer-guide] explains how to build and test
* [Get Up and Running with Windows Forms .NET][getting-started] explains how to get started building Windows Forms applications.


## How to Engage, Contribute, and Provide Feedback

Some of the best ways to contribute are to try things out, file bugs, join in design conversations, and fix issues.

* The [contributing guidelines][contributing] and the more general [.NET contributing guide][net-contributing] define contributing rules.
* The [Developer Guide][developer-guide] defines the setup and workflow for working on this repository.
* If you have a question or have found a bug, [file an issue](https://github.com/dotnet/winforms/issues/new?template=bug_report.md).
* Use [daily builds][developer-guide] if you want to contribute and stay up to date with the team.

## Reporting security issues

Security issues and bugs should be reported privately via email to the Microsoft Security Response Center (MSRC) <secure@microsoft.com>. You should receive a response within 24 hours. If for some reason you do not, please follow up via email to ensure we received your original message. Further information, including the MSRC PGP key, can be found in the [Security TechCenter](https://www.microsoft.com/msrc/faqs-report-an-issue). Also see info about related [Microsoft .NET Core and ASP.NET Core Bug Bounty Program](https://www.microsoft.com/msrc/bounty-dot-net-core).

## Code of Conduct

This project uses the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct) to define expected conduct in our community. Instances of abusive, harassing, or otherwise unacceptable behavior may be reported by contacting a project maintainer at conduct@dotnetfoundation.org.

## License

.NET (including the Windows Forms repository) is licensed under the [MIT license](LICENSE.TXT).

## .NET Foundation

.NET Windows Forms is a [.NET Foundation](https://www.dotnetfoundation.org/projects) project.<br />
See the [.NET home repository](https://github.com/Microsoft/dotnet) to find other .NET-related projects.

[contributing]: CONTRIBUTING.md
[developer-guide]: docs/developer-guide.md
[getting-started]: docs/getting-started.md
[net-contributing]: https://github.com/dotnet/runtime/blob/master/CONTRIBUTING.md
[porting-guidelines]: docs/porting-guidelines.md

