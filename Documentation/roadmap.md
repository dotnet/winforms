# WinForms on .NET Core Roadmap

With the introduction of .NET Core 3, WinForms exists as one of [several
layers](https://github.com/dotnet/core/blob/master/Documentation/core-repos.md)
of .NET Core.  Although .NET Core is cross-platform, WinForms relies heavily on
Windows-specific platform pieces and for now will remain Windows only.

At present, our team's primary focus is making additional components of WinForms
available as open source in this repo, ensuring functional parity with the
WinForms as it currently exists .NET Framework, and adding the ability to run
tests publicly so we can accept PRs from the open source community. Here are our
short-term and long-term goals.

## Short-Term

* Port existing functional tests and test infrastructure to this repo
* Add Application property for DPI Awareness setting

## Long-Term

* Add WinForms Designer support for .NET Core 3 projects in Visual Studio
* Fix existing scaling bugs in Per Monitor DPI aware applications
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

If you'd like to contribute to WinForms, please take a look at our [Contributing
Guide](https://github.com/dotnet/winforms/blob/master/Documentation/contributing-guide.md).