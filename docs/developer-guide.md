# Developer Guide

The following document describes the setup and workflow that is recommended for working on the Windows Forms project. It assumes that you have read the [Contributing Guide](../CONTRIBUTING.md).

The [Issue Guide](issue-guide.md) describes our approach to using GitHub issues.

## Machine Setup

Windows Forms requires the following workloads and components be selected when installing Visual Studio 2022 (17.0.0):

* Required Workloads:
  * .NET Desktop Development
  * Desktop development with C++
* [Required Individual Components][required-individual-components]:
  * Windows 10 SDK
  * C++/CLI support


  :warning: CMake 3.21.0 or later is required. Install CMake from the [official website][cmake-download] or via [Chocolatey][chocolatey]:
  ```
  choco install cmake --installargs 'ADD_CMAKE_TO_PATH=System'
  ```
  

## Workflow

We use the following workflow for building as well as testing features and fixes.

You first need to [fork][fork] then [clone][clone] this Windows Forms repository. This is a one-time task.

1. [Build](building.md) the repository.
2. [Debug](debugging.md) the change, as needed.
3. [Test](testing.md) the change, to validate quality.

## More Information

* [.NET Docs and Guidelines][net-runtime-instructions]
* ["up-for-grabs" issues][up-for-grabs]

[comment]: <> (URI Links)

[net-runtime-instructions]: https://github.com/dotnet/runtime/tree/master/docs
[fork]: https://guides.github.com/activities/forking/
[clone]: https://www.git-scm.com/docs/git-clone
[up-for-grabs]: https://github.com/dotnet/winforms/issues?q=is%3Aopen+is%3Aissue+label%3Aup-for-grabs
[chocolatey]: https://chocolatey.org/
[cmake-download]: https://cmake.org/download/
[required-individual-components]: ../WinForms.vsconfig
