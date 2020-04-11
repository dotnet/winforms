# WinForms Roadmap

This roadmap communicates priorities for evolving and extending the scope of WinForms for .NET Core and .NET 5 projects.

This repository is a community effort and we welcome community feedback on our plans. The best way to give your feedback is to open an issue in this repo.

We also invite contributions. The [up-for-grabs issues](https://github.com/dotnet/winforms/issues?q=is%3Aopen+is%3Aissue+label%3Aup-for-grabs) on GitHub are a good place to start.

## Present

### Designer for .NET Core project

Our main effort right now is focused around enabling full experience for WinForms designer for .NET Core projects. This involves adding the remaining controls and features and improving stability and performance for the designer.

## Future

Besides the designer work, here is the list of improvements we are planning to work on in the future.

### Add WebView2 control

New Chromium-based Edge browser control.

### Accessibility improvements

Including support for standard WCAG2.1, enabling tooltips for controls on `Tab`, etc.

### Support for ARM64

Enabling WinForms applications to run efficiently with ARM processors.

### High DPI improvements

* Fix existing scaling bugs in Per Monitor DPI aware applications
* Enable all controls to support Per Monitor V2 mode
* Add a new "clean" way of calculating location/size information in PMA mode

### Performance improvements

Establish a base line and improve runtime and designer performance.

### Testing infrastructure

Add testing infrastructure and improve test coverage.

## Some potential features

We also are gathering feedback regarding what other features you'd like to see in WinForms. Please let us know if any of those or something else would be useful for your WinForms applications. You an create a feature request in this repo or vote for an existing one.

### Add wrappers for missing Win32 controls

such as Task Dialog, Ribbon Control, Balloons, SearchBox, ...

### Add support for OS themes in WinForms applications

Enable dark theme, etc., in WinForms apps.

For general information regarding .NET Core plans, see [.NET Core roadmap](https://github.com/dotnet/core/blob/master/roadmap.md).
