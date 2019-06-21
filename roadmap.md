# WinForms on .NET Core Roadmap

This roadmap communicates priorities for evolving and extending the scope of WinForms for .NET Core.

At present, our primary focus is enabling the following for .NET Core 3.0:

* Achieve WinForms functional and performance parity compared to .NET Framework
* Publish remaining WinForms components to the repository
* Publish (and write) more WinForms tests to the repository

> Note: There are some specific .NET Framework features will not be supported, such as hosting WinForms controls in Internet Explorer.

As we complete those goals, we'll update our roadmap to include additional feature/capability areas we will focus on next.

For general information regarding .NET Core plans, see [.NET Core
roadmap][core-roadmap].

## Timelines

| Milestone                                         | Date              |
|---                                                |---                |
|Initial launch of WinForms on .NET Core repository |Dec 4, 2018        |
|Functional parity with .NET Framework WinForms     |Q1 2019            |
|First version of WinForms on .NET Core             |.NET Core 3.0 GA   |
|Designer support in Visual Studio|Update to VS 2019|                   |

If you'd like to contribute to WinForms, please take a look at our [Contributing
Guide](Documentation/contributing.md).

## Feature Backlog

* Add WinForms Designer support for .NET Core 3 projects in a Visual Studio 2019 update
* Fix existing scaling bugs in Per Monitor DPI aware applications
* Add a new “clean" way of calculating location/size information in PMA mode.
* Make new projects be per monitor aware
* Add Chromium-based Edge browser control
* Improve accessibility support for some missing UIA interfaces
* Improve performance of WinForms runtime

[comment]: <> (URI Links)

[core-roadmap]: https://github.com/dotnet/core/blob/master/roadmap.md
