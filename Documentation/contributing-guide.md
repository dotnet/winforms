# Contributing Guide

Primary focus of .NET Core 3.0 release is to achieve parity with WinForms for .NET Framework.
Since we're currently still porting parts of WinForms codebase (incl. tests) to GitHub, we're not ready to handle non-trivial or larger contributions beyond parity fixes yet.

We plan to accept these kind of contributions during 3.0 release:
* Low-risk changes, which are easy to review (e.g. typos, comment changes, documentation improvements, etc.).
* Test fixes, test improvements and new tests increasing code coverage.
* Infrastructure fixes and improvements, which are aligned with achieving our goal to ship high quality .NET Core 3.0 release.
* Bug fixes for differences between WinForms for .NET Core and .NET Framework.

If you have a **larger change** falling into any of these categories, we recommend to **check with our team members** prior to creating a PR.
We recommend to first create a [new issue](https://github.com/dotnet/winforms/issues), where you can describe your intent and help us understand the change you plan to contribute.

**WARNING:** Expect that we may reject or postpone PRs which do not align with our primary focus (parity with WinForms for .NET Framework), 
or which could introduce unnecessary risk (e.g. in code which is historically sensitive, or is not well covered by tests).
Such PRs may be closed and reconsidered later after we ship .NET Core 3.0.



## Developer Guide

Before you start, please review [WinForms contributing doc](TODO) and **[.NET Core contributing doc](https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/contributing.md)** for coding style and PR gotchas.

* [git commands and workflow](https://github.com/dotnet/corefx/wiki/git-reference) - for newbies on GitHub
* Pick issue: [up-for-grabs](https://github.com/dotnet/winforms/issues?q=is%3Aopen+is%3Aissue+label%3Aup-for-grabs) or [easy](https://github.com/dotnet/winforms/issues?utf8=%E2%9C%93&q=is%3Aopen+is%3Aissue+label%3Aeasy)
* [Coding guidelines](https://github.com/dotnet/corefx/tree/master/Documentation#coding-guidelines)

To work in this repository, begin by observing the following:
1. Clone this repository
    * Fork your own copy of the [WinForms repository]( https://github.com/dotnet/winforms) with the _Fork_ button on the repository’s web page to your account. 
    * Clone locally with the _Clone_ button or using **`git clone https://github.com/[YourGitHubAccount]/winforms`**.
2. [Machine Setup](https://github.com/dotnet/winforms/blob/master/Documentation/machine-setup.md) instructions
3. [Building](https://github.com/dotnet/winforms/blob/master/Documentation/building.md) instructions to build from source
4. [Debugging](https://github.com/dotnet/winforms/blob/master/Documentation/debugging.md) recomendations and techniques
5. [Testing](https://github.com/dotnet/winforms/blob/master/Documentation/testing.md) instructions, guidance, and rules
