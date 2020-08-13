# Debugging

There are two recommended ways to debug Windows Forms binaries. Both require that you first [build from source][corefx-building].

Then, you are free make your changes locally.

Once you are ready to debug your changes on an existing Windows Forms application, please follow one of the two following recommended techniques for relating your changes back to your project. You should then be able to set breakpoints and debug as expected.

If you do not want to modify your local SDK, you may with to perform technique 2, while if you do not want to add an additional reference to your project, technique 1 may be better for you.

## 1. Copy your changes into the SDK

copy the resulting assembly(-ies) from the base of the repository  

`[path-to-repo]\winforms\artifacts\bin\System.Windows.Forms\Debug\netcoreapp5.0_`

to your dotnet folder at:  

`[Drive]:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\[Version]`

where **[Drive]** is your OS drive (for example, C:)  and **[path-to-repo]** is the additional path to our repository from the base drive. **[Version]** is your DesktopUI version directory (for example, 3.0.0-alpha-27017-4). Note if you have Microsoft.DesktopUI.App instead of Microsoft.WindowsDesktop.App, this is the outdated version.

**NOTE** that this will modify your SDK; to revert back, you will have to repair the install or reinstall. See the [dotnet Core repository][dotnet-core-repos] for more information.

## 2. Point your project to your experimental binary(-ies)

Add references to the binary(-ies) to your project ported to Core. For example, for System.Windows.Forms, you should add the following reference:

```xml
<ItemGroup>
    <Reference Include="[Drive]:[path-to-repo]\winforms\artifacts\bin\System.Windows.Forms\Debug\netcoreapp5.0\System.Windows.Forms.dll" />
</ItemGroup>
```

where **[Drive]** is the drive you have our repository in and **[path-to-repo]** is the additional path to our repository from the base drive (this may be nothing). Note netcoreapp5.0 may change.

[comment]: <> (URI Links)

[corefx-building]: https://github.com/dotnet/corefx/blob/master/Documentation/building.md
[dotnet-core-repos]: https://github.com/dotnet/core