# WinForms on .NET Core Roadmap

With the introduction of .NET Core 3, WinForms exists as one of [several
layers](https://github.com/dotnet/core/blob/master/Documentation/core-repos.md)
of .NET Core.  Although .NET Core is cross-platform, WinForms relies heavily on
Windows-specific platform pieces and for now will remain Windows only.

At present, our team's primary focus is making additional components of WinForms
available as open source in this repo, ensuring functional parity with the WinForms as it currently exists .NET Framework, and adding the ability to run tests
publicly so we can accept PRs from the open source community. Here are our
short-term and long-term goals.

## Short-Term

* Port existing functional tests and infrastructure to this repo
* Add Application property for DPI Awareness setting

## Long-Term

* Add WinForms Designer support for .NET Core 3 projects in Visual Studio
* Fix existing PMA bugs
* Add a new “clean" way of calculating location/size information in PMA mode.
* Make new projects be per monitor aware
* Add Edge browser control
* Add Data Visualization controls
* Improve accessibility support for some missing UIA interfaces
* Improve performance of WinForms runtime

For general information regarding .NET Core plans, see [.NET Core
roadmap](https://github.com/dotnet/core/blob/master/roadmap.md).  

## Timeline for Open Source

| Milestone | Release Date |
|---|---|
|Initial launch of WinForms on .NET Core repository |Dec 4, 2018|
|Functional parity with .NET Framework WinForms |Q1 2019|
|First version of open source WinForms|.NET Core GA|
|Designer support in Visual Studio|Update to VS 2019|

## Feedback

We welcome your feedback and contributions! The best way to give us feedback is
to [create issues in the
dotnet/winforms](https://github.com/dotnet/winforms/issues/) repo.

* This repo is specifically focused on WinForms on .NET Core, which is separate
  from WinForms that runs on the Desktop Framework.  If you have feedback for
  the latter, please report it on
  [developercommunity.visualstudio.com](https://developercommunity.visualstudio.com/)
  using the "Report a problem" or "Suggest a feature" buttons.
* If you have general feedback about .NET Core 3, please use the
  [dotnet/core](https://github.com/dotnet/core) repo or one of the other [.NET
  Core
  repos](https://github.com/dotnet/core/blob/master/Documentation/core-repos.md)
  suitable for the topic you'd like to discuss.

Some of the feedback we find most valuable pertains to:

* Existing features that are missing some capability or otherwise don't work
  well enough.
* Missing features that should be added to the product.
* Design choices for a feature that is currently in-progress.
* Which WinForms assemblies we should prioritize making available as open source
  as quickly as possible.

If you'd like to contribute to WinForms, please take a look at our [Contributing
Guide](https://github.com/dotnet/winforms/blob/master/Documentation/contributing-guide.md).