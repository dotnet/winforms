# Windows Forms Roadmap

This roadmap communicates priorities for evolving and extending the scope of Windows Forms. For general information regarding .NET plans, see [.NET roadmap](https://github.com/dotnet/core/blob/master/roadmap.md).

* [Past](#past)
* [Present](#present)
* [Future](#future)

This repository is a community effort and we welcome community feedback on our plans. The best way to give your feedback is to open an issue in this repo.
We also invite contributions. The [up-for-grabs issues](https://github.com/dotnet/winforms/issues?q=is%3Aopen+is%3Aissue+label%3Aup-for-grabs) on GitHub are a good place to start.

## Past 

### Windows Forms runtime

* **.NET Core 3.x**<br/>
    The primary goal of .NET Core 3.x release was to achieve parity with .NET Framework.

* **.NET 5.0**
    - We aimed to optimize our implementations, reduce our memory footprints, increase performance, and update implementations to deliver all aspects of modern Windows UI, including missing properties or actions, and new UI controls.
    - Added [Task Dialog](https://docs.microsoft.com/dotnet/api/system.windows.forms.taskdialog) control, and added missing functionality to [ListView](https://docs.microsoft.com/dotnet/api/system.windows.forms.listview) control.
    - We have also further increased our accessibility support, e.g. by adding [Text Pattern support](https://docs.microsoft.com/windows/win32/winauto/uiauto-implementingtextandtextrange).
    - Reinstated Visual Basic support.
    - Enabled ARM64 support.


### Windows Forms Designer

Our main effort was focused on enabling full experience for Windows Forms designer for .NET projects. 

## Present

### Windows Forms runtime

**Under considerations**.<br/>A number of aspects are being considered including (but not limited to) layout engine and high DPI support, theming, new controls and components.

### Windows Forms Designer

We continue the work on the designer to to achieve the parity with the .NET Framework designer.
This involves adding the remaining controls and features and improving stability and performance for the designer.

## Future

Besides the designer work, here is the list of improvements we are planning to work on in the future.

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

