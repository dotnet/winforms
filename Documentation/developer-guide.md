# Developer Guide

The following document describes the setup and workflow that is recommended for working on the Windows Forms project. It assumes that you have read the [Contributing Document](contributing.md).

The [Issue Guide](issue-guide.md) describes our approach to using GitHub issues.

## Machine Setup

Follow the [Building CoreFX on Windows][corefx-windows-instructions] instructions. In particular, [Visual Studio 2019 Preview][vs-preview] is required to develop on .NET Core.

Windows Forms requires the following workloads and components be selected when installing Visual Studio:

* Required Workloads:
  * .NET Desktop Development
  * Desktop development with C++
* Required Individual Components:
  * C++/CLI support
  * Windows 10 SDK

## Workflow

We use the following workflow for building as well as testing features and fixes.

You first need to [Fork][fork] and [Clone][clone] this WinForms repository. This is a one-time task.

1. [Build](building.md) the repository.

2. [Debug](debugging.md) the change, as needed.

3. [Test](testing.md) the change, to validate quality.

## More Information

* [Git commands and workflow][git-commands]
* [Coding guidelines][corefx-coding-guidelines]
* [up-for-grabs WinForms issues][up-for-grabs]
* [easy WinForms issues][easy]

[comment]: <> (URI Links)

[corefx-windows-instructions]: https://github.com/dotnet/corefx/blob/master/Documentation/building/windows-instructions.md
[vs-preview]: https://visualstudio.microsoft.com/vs/preview/
[fork]: https://github.com/dotnet/corefx/wiki/Checking-out-the-code-repository#fork-the-repository
[clone]: https://github.com/dotnet/corefx/wiki/Checking-out-the-code-repository#clone-the-repository
[git-commands]: https://github.com/dotnet/corefx/wiki/git-reference
[corefx-coding-guidelines]: https://github.com/dotnet/corefx/tree/master/Documentation#coding-guidelines
[up-for-grabs]: https://github.com/dotnet/winforms/issues?q=is%3Aopen+is%3Aissue+label%3Aup-for-grabs
[easy]: https://github.com/dotnet/winforms/issues?utf8=%E2%9C%93&q=is%3Aopen+is%3Aissue+label%3Aeasy
