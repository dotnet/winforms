# Porting existing applications to .NET Core 3.0

>We suggest doing migration in a separate branch or, if you're not using version
>control, creating a copy of your project so you have a clean state to go back
>to if necessary.

The migration process includes two steps: **preparing your project for porting** to
.NET Core and **porting** itself.

## Prepare your project for porting

1. **Run [.NET Portability Analyzer][api-port]** first to determine if there are
   any APIs your application depends on that are missing in .NET Core. If there
   are, you have a few options.
    * Remove not supported APIs or replace them with those, that are included in
      .NET Core
    * Separate your code into different projects: the one that contains only
      .NET Core supported APIs and another with APIs not supported in .NET Core.
      Migrate only the first project.

2. **Start from a working solution**. Ensure the solution opens, builds, and
   runs without any issues.

3. **Replace `packages.config` with `PackageReference`**. If your project uses
   NuGet packages, you will need to add the same NuGet packages to the new .NET
   Core project. .NET Core projects support only `PackageReference` for adding
   NuGet packages. To move your NuGet references from `packages.config` to your
   project file, right-click on `packages.config` -> **Migrate packages.config
   to PackageReference...**.

   You can learn more about this migration in our [docs][pkg-config].

4. **Migrate to the SDK-style .csproj file**. The new SDK-style `.csproj` format
   is leaner and easier to read. To be able to simply copy-paste your references
   from the old project to the new one, you first need to migrate your old
   project file to SDK-style so both project are in the same format. You can
   either do it by hand or use a third-party tool [CsprojToVs2017][sdk-tool].

   After using the tool you still might need to delete some reference by hand,
   for example:

    ```xml
    <Reference Include="System.Data.DataSetExtensions" />
    <Reference Include="Microsoft.CSharp" />
    <Reference Include="System.Net.Http" />
    ```

   After you've migrated to the new SDK-style format, ensure your project builds
   and runs successfully.

5. **Configure assembly file generation**. Most existing projects include an
   `AssemblyInfo.cs` file in the Properties folder. The new project style uses a
   different approach and generates the same assembly attributes as part of the
   build process. As a result, you might end up with two `AssemblyInfo.cs` files
   and your build will fail. There are two ways to resolve this problem. You can
   either:

    * Disable `AssemblyInfo.cs` generation on build by setting the property:
        ```xml
        <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
        ```
    * Move the static values from `AssemblyInfo.cs` to properties in the new
      `.csproj` file.

    Build and run to make sure you didn't introduce any issues while preparing
    your project. Now it's time to port it.

## Port your project

1. **Add .NET Core Windows Forms project**. Add a new .NET Core 3.0 Windows
   Forms project to the solution.

2. **Add `<ProjectReference>`**. Copy the `<ProjectReference>` elements from the
   `.csproj` file of the original project to the new project's `.csproj` file.
   Note: The new project format does not use the `Name` and `ProjectGuid`
   elements, so you can safely delete those.

3. **Restore/Build**. At this point, it's a good idea to restore/build to make
   sure all dependencies are properly configured.

4. **Link files**. Link all files from your existing .NET Framework WinForms
   project to the .NET Core 3.0 WinForms project by adding following to the
   `.csproj` file.

    ```xml
    <ItemGroup>
        <Compile Include="..\<Your .NET Framework Project Name>\**\*.cs" />
        <EmbeddedResource Include="..\<Your .NET Framework Project Name>\**\*.resx" />
    </ItemGroup>
    ```

5. **Align default namespace and assembly name**. Since you're linking to
   designer generated files (for example, `Resources.Designer.cs`) you generally
   want to make sure that the .NET Core version of your application uses the
   same namespace and the same assembly name. Copy the following settings from
   your .NET Framework project:

    ```xml
    <PropertyGroup>
        <RootNamespace><!-- (Your default namespace) --></RootNamespace>
        <AssemblyName><!-- (Your assembly name) --></AssemblyName>
    </PropertyGroup>
    ```

6. **Run new project**. Set your new .NET Core project as StartUp Project and
   run it. Make sure everything works.

7. **Copy or leave linked**. Now instead of linking the files, you can actually
   copy them from the old .NET Framework WinForms project to the new .NET Core
   3.0 WinForms project. After that you can get rid of the old project. However
   if you'd like to use WinForms designer, it is not available in Visual Studio
   just yet. So you can stop at the step 8 and perform step 9 when the designer
   support is available.

## Migration tips

### Include the Windows.Compatibility Pack

Windows applications like Windows Forms and WPF often use APIs that aren't
referenced by default in .NET Core. The reason is that .NET Core tries to reduce
the risk that new applications accidentally depend on legacy technologies or on
APIs that are Windows-only. However, when porting existing Windows applications,
neither of these two aspects is a concern. To simplify your porting efforts, you
can just reference the [Windows Compatibility Pack][compat-pack] which will give
you access to many more APIs.

```cmd
dotnet add package Microsoft.Windows.Compatibility
```

### Migrating WCF clients

.NET Core has its own implementation of `System.ServiceModel` with some
differences:

* It's available as a set of NuGet packages (also included in the [Windows
  Compatibility Pack][compat-pack]).
* There are [unsupported features][wcf-supported] that you should review.
* The binding and endpoint address must be specified in the service client
  constructor. Otherwise, if you reuse the `ServiceReference` created by Visual
  Studio, you may get the following error:

  ```text
  System.PlatformNotSupportedException: 'Configuration files are not supported.'
  ```

[api-port]: https://blogs.msdn.microsoft.com/dotnet/2018/08/08/are-your-windows-forms-and-wpf-applications-ready-for-net-core-3-0/
[pkg-config]: https://docs.microsoft.com/en-us/nuget/reference/migrate-packages-config-to-package-reference
[sdk-tool]:https://github.com/hvanbakel/CsprojToVs2017
[compat-pack]: https://docs.microsoft.com/en-us/dotnet/core/porting/windows-compat-pack
[wcf-supported]: https://github.com/dotnet/wcf/blob/master/release-notes/SupportedFeatures-v2.1.0.md