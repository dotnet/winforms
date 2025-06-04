# Debugging

There are two recommended ways to debug Windows Forms binaries. Both require that you first [build from source](building.md).

Once you are ready to debug your changes on an existing Windows Forms application, please follow one of the two following recommended techniques for relating your changes back to your project. You should then be able to set breakpoints and debug as expected.

If you do not want to modify your local SDK, you may with to perform technique 2. However if you do not want to add an additional reference to your project, technique 1 may be better for you.

## 1. Copy your changes into the SDK

Copy the resulting assembly(-ies) from your local build.

- They are located at `[Path-to-repo]\winforms\artifacts\bin\System.Windows.Forms\Debug\net7.0`
- And need to be copied to the system dotnet folder `[Drive]:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\[Version]`

:warning: If you have updated any public APIs, you'll need to overwrite the ref assemblies too (since that's what VS uses when resolving types):
- The ref assemblies compile to `[Path-to-repo]\winforms\artifacts\bin\System.Windows.Forms\Debug\net7.0\ref`
- These need to be copied to `[Drive]:\Program Files\dotnet\packs\Microsoft.WindowsDesktop.App.Ref\[Version]`

where:
- **[Drive]** is your OS drive (for example, C:),
- **[Path-to-repo]** is the additional path to our repository from the base drive, and
- **[Version]** is your WindowsDesktop version directory (for example, 5.0.0-rc.2.20464.5).



**NOTE** Make sure to make a backup copy of assemblies you replace, you will modify your SDK. Alternatively you can repair the install or reinstall.

## 2. Point your project to your experimental binary(-ies)

Add references to the binary(-ies) to your project ported to .NET. For example, for System.Windows.Forms, you should add the following reference:

```xml
<ItemGroup>
    <Reference Include="[Drive]:[Path-to-repo]\winforms\artifacts\bin\System.Windows.Forms\Debug\net9.0\System.Drawing.Common.dll" />
    <Reference Include="[Drive]:[Path-to-repo]\winforms\artifacts\bin\System.Windows.Forms\Debug\net9.0\System.Private.Windows.Core.dll" />
    <Reference Include="[Drive]:[Path-to-repo]\winforms\artifacts\bin\System.Windows.Forms\Debug\net9.0\System.Windows.Forms.dll" />
    <Reference Include="[Drive]:[Path-to-repo]\winforms\artifacts\bin\System.Windows.Forms.Primitives\Debug\net9.0\System.Windows.Forms.Primitives.dll" />
    <!-- Optionally you may need designer -->
    <Reference Include="[Drive]:[Path-to-repo]\winforms\artifacts\bin\System.Windows.Forms.Design\Debug\net9.0\System.Windows.Forms.Design.dll" />
</ItemGroup>
```

where:
- **[Drive]** is the drive you have our repository in, and
- **[Path-to-repo]** is the additional path to our repository from the base drive. 
