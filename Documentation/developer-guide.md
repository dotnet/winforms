# Developer Guide

The following document describes the setup and workflow that is recommended for working on the Windows Forms project. It assumes that you have read the [Contributing Document](contributing.md).

The [Issue Guide](issue-guide.md) describes our approach to using GitHub issues.

## Machine Setup

Follow the [.NET Runtime][net-runtime-instructions] instructions.

Windows Forms requires the following workloads and components be selected when installing Visual Studio:

* Required Workloads:
  * .NET Desktop Development
  * Desktop development with C++
* Required Individual Components:
  * C++/CLI support
  * Windows 10 SDK

## Workflow

We use the following workflow for building as well as testing features and fixes.

You first need to [fork][fork] and [clone][clone] this Windows Forms repository. This is a one-time task.

1. [Build](building.md) the repository.
2. [Debug](debugging.md) the change, as needed.
3. [Test](testing.md) the change, to validate quality.

## More Information

* [Git commands and workflow][git-commands]
* [Coding guidelines][corefx-coding-guidelines]
* [up-for-grabs Windows Forms issues][up-for-grabs]
* [easy WinForms issues][easy]

[comment]: <> (URI Links)

[net-runtime-instructions]: https://github.com/dotnet/runtime/tree/master/docs
[fork]: https://github.com/dotnet/corefx/wiki/Checking-out-the-code-repository#fork-the-repository
[clone]: https://github.com/dotnet/corefx/wiki/Checking-out-the-code-repository#clone-the-repository
[git-commands]: https://github.com/dotnet/corefx/wiki/git-reference
[corefx-coding-guidelines]: https://github.com/dotnet/runtime/tree/master/docs/coding-guidelines
[up-for-grabs]: https://github.com/dotnet/winforms/issues?q=is%3Aopen+is%3Aissue+label%3Aup-for-grabs
[easy]: https://github.com/dotnet/winforms/issues?utf8=%E2%9C%93&q=is%3Aopen+is%3Aissue+label%3Aeasy
