# Contributing Guide

The primary focus of .NET Core 3.0 release for Windows Forms is to achieve parity with .NET Framework. Priority will be given to changes that align with that goal. See the [roadmap](../roadmap.md) to understand project goals.

In .NET 5.0 and beyond we are aiming to optimize our implementations, reduce our memory footprints, increase performance, and update implementations to deliver all aspects of modern Windows UI, including missings properties or actions, and new UI controls.

We need the most help with the following types of changes:

* Test fixes, test improvements, and new tests increasing code coverage.
* Bug fixes that specifically target parity between .NET Core and .NET Framework.

Please [file an issue][issues] for any larger change you would like to propose. Please choose the template most appropriate for the issue type.

See [Developer Guide](developer-guide.md) to learn how to develop changes for this repository.

This project follows the general [.NET Core Contribution Guidelines][runtime-contribution-guidelines]. Please read it before submitting PRs.

The contribution bar from the general contribution guidelines is copied below.

## Contribution "Bar"

Project maintainers will consider changes that improve the product or fix known bugs (please file issues to make bugs "known").

Maintainers will not merge changes that have narrowly-defined benefits due to compatibility risk or complexity added to the product. We may revert changes if they are found to be breaking.

Most .NET Core components are cross-platform and we appreciate contributions that either improve their feature set in a given environment or that add support for a new environment. We will typically not accept contributions that implement support for an OS-specific technology on another operating system. For example, we do not intend to create an implementation of the Windows registry for Linux or an implementation of the macOS keychain for Windows. We also do not intend to accept contributions that provide cross-platform implementations for Windows Forms or WPF.

Contributions must also satisfy the other published guidelines defined in this document.

[comment]: <> (URI Links)

[issues]: https://github.com/dotnet/winforms/issues
[runtime-contribution-guidelines]: https://github.com/dotnet/runtime/blob/master/CONTRIBUTING.md
