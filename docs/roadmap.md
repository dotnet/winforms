# Windows Forms Roadmap

This roadmap communicates priorities for evolving and extending the scope of Windows Forms. For general information regarding .NET plans, see [.NET roadmap](https://github.com/dotnet/core/blob/master/roadmap.md).

* [Future](#future)
* [Present](#present)

This repository is a community effort and we welcome community feedback on our plans. The best way to give your feedback is to open an issue in this repo.
We also invite contributions. The [help wanted issues](https://github.com/dotnet/winforms/issues?q=is%3Aopen+is%3Aissue+label%3A"help%20wanted") on GitHub are a good place to start.

## Future

Here is the list of improvements we are planning to work on in the future.

### Accessibility improvements

Including support for standard [WCAG2.1]( https://www.w3.org/TR/WCAG21/), such as enabling tooltips for controls on `Tab` ([#2726](https://github.com/dotnet/winforms/issues/2726)), etc.

### High DPI improvements

* Fix existing scaling bugs in Per Monitor DPI aware applications
* Enable all controls to support Per Monitor V2 mode
* Add a new "clean" way of calculating location/size information in PMA mode

### Performance improvements

Establish a base line and improve runtime and designer performance.

### Testing infrastructure

Add testing infrastructure and improve test coverage.

## Some potential features

We also are gathering feedback regarding what other features you'd like to see in WinForms. Please let us know if any of those or something else would be useful for your applications. You an create a feature request in this repo or vote for an existing one.

### Add and improve wrappers for missing Win32 controls

Such as Ribbon Control, Balloons, SearchBox, improvements around ListView, etc.

### Add support for OS themes in WinForms applications

Enable dark theme, etc.

## Present

### Windows Forms runtime

**Under considerations**.<br/>A number of aspects are being considered including (but not limited to) layout engine and high DPI support, theming, new controls and components.

### Windows Forms Designer

We continue the work on the designer to to achieve the parity with the .NET Framework designer.
This involves adding the remaining controls and features and improving stability and performance for the designer.
