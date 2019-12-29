# Port existing applications to .NET Core 3.1

>We suggest doing migration in a separate branch or, if you're not using version
>control, creating a copy of your project so you have a clean state to go back
>to if necessary.

The migration process includes two steps: **preparing your project for porting** to
.NET Core and **porting** itself.

For additional information and assistance, we recommend checking out [this article on the dotnet blog][dotnet-blog-port-guide] as well as the [accompanying video tutorial][dotnet-blog-port-video].


## Breaking changes

Review the following resources that describe breaking changes between Windows Forms on .NET Framework and .NET Core:

* The [Breaking changes in Windows Forms (.NET Framework to .NET Core)](https://docs.microsoft.com/dotnet/core/porting/winforms-breaking-changes) describes breaking changes you need to be aware of when migrating your applications from .NET Framework
* The [Breaking changes in Windows Forms (.NET Core to .NET Core)](https://docs.microsoft.com/dotnet/core/compatibility/winforms) describes breaking changes you need to be aware of when migrating your applications from one version of .NET Core to another.

## Prepare your project for porting

1. **Run [.NET Portability Analyzer][api-port]** first to determine if there are
   any APIs your application depends on that are missing in .NET Core. If there
   are, you have a few options.
    * Remove not supported APIs or replace them with those, that are included in
      .NET Core
    * Separate your code into different projects: the one that contains only
      .NET Core supported APIs and another with APIs not supported in .NET Core.
      Migrate only the first project.

1. **Start from a working solution**. Ensure the solution opens, builds, and runs without any issues.

1. **Replace `packages.config` with `PackageReference`**. If your project uses
   NuGet packages, you will need to add the same NuGet packages to the new .NET
   Core project. .NET Core projects support only `PackageReference` for adding
   NuGet packages. To move your NuGet references from `packages.config` to your
   project file, right-click on `packages.config` -> **Migrate packages.config
   to PackageReference...**.

   You can learn more about this migration in our [docs][pkg-config].

1. **Migrate to the SDK-style .csproj file**. The new SDK-style `.csproj` format is leaner and easier to read. To be able to simply copy-paste your references from the old project to the new one, you first need to migrate your old project file to SDK-style so both project are in the same format. You can either do it by hand or use a third-party tool [CsprojToVs2017][sdk-tool].

   After using the tool you still might need to delete some reference by hand; for example:

    ```xml
    <Reference Include="System.Data.DataSetExtensions" />
    <Reference Include="Microsoft.CSharp" />
    <Reference Include="System.Net.Http" />
    ```

   After you've migrated to the new SDK-style format, ensure your project builds and runs successfully.

1. **Configure assembly file generation**. Most existing projects include an `AssemblyInfo.cs` file in the Properties folder. The new project style uses a different approach and generates the same assembly attributes as part of the build process. As a result, you might end up with two `AssemblyInfo.cs` files and your build will fail. There are two ways to resolve this problem. You can either:
    * Disable `AssemblyInfo.cs` generation on build by setting the property:

        ```xml
        <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
        ```
    * Move the static values from `AssemblyInfo.cs` to properties in the new `.csproj` file.

    Build and run to make sure you did not introduce any issues while preparing your project. Now it is time to port it.



## Port your project

### Automated porting

1. Try porting your project using [`try-convert` tool](https://github.com/dotnet/try-convert), which we built to help migrations. <br />
NB: The tool may not for all possible projects, in that case please read on.

### Manual porting

1. **Add .NET Core Windows Forms project**. Add a new .NET Core 3.1 Windows Forms project to the solution.

1. **Add `<ProjectReference>`**. Copy the `<ProjectReference>` elements from the `.csproj` file of the original project to the new project's `.csproj` file.<br />
Note: The new project format does not use the `Name` and `ProjectGuid` elements, so you can safely delete those.

1. **Restore/Build**. At this point, it's a good idea to restore/build to make sure all dependencies are properly configured.

1. **Link files**. Link all files from your existing .NET Framework WinForms project to the .NET Core 3.1 WinForms project by adding following to the `.csproj` file.

    ```xml
    <ItemGroup>
        <Compile Include="..\<Your .NET Framework Project Name>\**\*.cs" />
        <EmbeddedResource Include="..\<Your .NET Framework Project Name>\**\*.resx" />
    </ItemGroup>
    ```

1. **Align default namespace and assembly name**. Since you're linking to designer generated files (for example, `Resources.Designer.cs`) you generally want to make sure that the .NET Core version of your application uses the same namespace and the same assembly name. Copy the following settings from your .NET Framework project:

    ```xml
    <PropertyGroup>
        <RootNamespace><!-- (Your default namespace) --></RootNamespace>
        <AssemblyName><!-- (Your assembly name) --></AssemblyName>
    </PropertyGroup>
    ```

1. **Run new project**. Set your new .NET Core project as StartUp Project and run it. Make sure everything works.

1. **Copy or leave linked**. Now instead of linking the files, you can actually copy them from the old .NET Framework WinForms project to the new .NET Core 3.1 WinForms project. After that you can get rid of the old project. However, if you would like to use the Windows Forms' designer, it is not available in Visual Studio just yet. So you can stop at the step 6 and perform step 7 once designer support is available.

## Migration tips

### Include the System.Windows.Forms.Datavisualization Pack

If you wish to use types previously associated with the [Charting control in the .NET Framework][framework-charting], you should add a package reference to the [NuGet package of Data Visualization][nuget-dataviz] ported to .NET Core. For more information about Data Visualization and the Charting control in .NET Core, including a sample application demonstrating its use, see the [winforms-datavisualization repository][dataviz]

### Include the Windows.Compatibility Pack

Windows applications like Windows Forms and WPF often use APIs that aren't referenced by default in .NET Core. The reason is that .NET Core tries to reduce the risk that new applications accidentally depend on legacy technologies or on APIs that are Windows-only. However, when porting existing Windows applications, neither of these two aspects is a concern. To simplify your porting efforts, you can simply reference the [Windows Compatibility Pack][compat-pack], which will give
you access to many more APIs.

```cmd
dotnet add package Microsoft.Windows.Compatibility
```

### Migrating WCF clients

.NET Core has its own implementation of `System.ServiceModel` with some
differences:

* It is available as a set of NuGet packages (also included in the [Windows
  Compatibility Pack][compat-pack]).
* There are [unsupported features][wcf-supported] that you should review.
* The binding and endpoint address must be specified in the service client constructor. Otherwise, if you reuse the `ServiceReference` created by Visual Studio, you may get the following error:

```cs
System.PlatformNotSupportedException: 'Configuration files are not supported.'
```

### Additional Types and Namespaces

You can search for additional types which you may need in porting your apps to .NET Core on [APIs of DotNet][apisofnet]. For example, when you search for the type `System.AppDomain`, you will see that the type has been moved to `System.Runtime.Extensions` namespace starting in .NET Core 2.0.

[comment]: <> (URI Links)

[dotnet-blog-port-guide]: https://devblogs.microsoft.com/dotnet/how-to-port-desktop-applications-to-net-core-3-0/
[dotnet-blog-port-video]: https://www.youtube.com/watch?v=upVQEUc_KwU
[api-port]: https://blogs.msdn.microsoft.com/dotnet/2018/08/08/are-your-windows-forms-and-wpf-applications-ready-for-net-core-3-0/
[pkg-config]: https://docs.microsoft.com/en-us/nuget/reference/migrate-packages-config-to-package-reference
[sdk-tool]:https://github.com/hvanbakel/CsprojToVs2017
[framework-charting]: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.datavisualization.charting
[nuget-dataviz]: https://www.nuget.org/packages/System.windows.forms.datavisualization
[dataviz]: https://github.com/dotnet/winforms-datavisualization
[compat-pack]: https://docs.microsoft.com/en-us/dotnet/core/porting/windows-compat-pack
[wcf-supported]: https://github.com/dotnet/wcf/blob/master/release-notes/SupportedFeatures-v2.1.0.md
[apisofnet]: https://apisof.net/
