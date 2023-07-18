# Windows Forms Out-Of-Process Designer

The Windows Forms out-of-process designer is currently available as a preview Visual Studio feature in [Visual Studio 2019 or later](https://visualstudio.microsoft.com/vs/preview/). It can be enabled in **Tools -> Options -> Environment -> Preview Features** dialog by checking the **Use the preview Windows Forms out-of-process designer for .NET apps** checkbox.
 Releases of .NET Windows Forms Designer are tied to Visual Studio Preview releases, so update to the latest Visual Studio Preview version to get the newest designer features.

:heavy_exclamation_mark: The latest designer feature shipped in `Visual Studio 2022 v17.5 Preview 3` is [Modernization of code-behind generation](./modernization-of-code-behind-in-OOP-designer/modernization-of-code-behind-in-oop-designer.md).

For the latest news on the designer please refer to [the series of blogs](https://devblogs.microsoft.com/search?query=winforms&blog=%2Fdotnet%2F):

* [Custom Controls for WinForms' Out-Of-Process Designer](https://devblogs.microsoft.com/dotnet/custom-controls-for-winforms-out-of-process-designer/)
* [Visual Basic WinForms Apps in .NET 5 and Visual Studio 16.8](https://devblogs.microsoft.com/dotnet/visual-basic-winforms-apps-in-net-5-and-visual-studio-16-8/)
* [Databinding with the OOP Windows Forms Designer](https://devblogs.microsoft.com/dotnet/databinding-with-the-oop-windows-forms-designer/)
* [State of the Windows Forms Designer for .NET Applications as of 01/13/2022](https://devblogs.microsoft.com/dotnet/state-of-the-windows-forms-designer-for-net-applications/)

### WinForms out of process designer SDK

For more information about creating custom control libraries, including sample code and templates refer to the [WinForms Designer Extensibility Repo](https://github.com/microsoft/winforms-designer-extensibility/). Details about how to use the WinForms Designer Extensibility SDK and structure a control library nuget package can be found there as well. Download the Extensibility SDK from [NuGet](https://www.nuget.org/packages/Microsoft.WinForms.Designer.SDK).

## Table of Content

* [How to use .NET Framework designer for .NET applications](net-inproc-designer.md)
* [How to troubleshoot the out-of-process designer](troubleshooting.md)
