# Working around accessibility issues in the Windows Forms Out of Process Designer

The introduction of Windows Forms (WinForms) to .NET Core 3.1 and beyond is enabling developers to migrate their  applications from .NET Framework to the more modern .NET versions. In order to support the designing of these applications within Visual Studio the team was required to create an entirely new Out of Process (OOP) Designer, which is currently still in a preview state. For more information on the technical reasons for this change, and the current status of the designer, see [our recent blog post](https://devblogs.microsoft.com/dotnet/state-of-the-windows-forms-designer-for-net-applications/).

There are several features that still need to be implemented in the OOP designer, foremost of which is supporting the use of Assistive Technologies such as NVDA, Narrator, and Jaws while using the OOP designer. As much as the team wanted to fully support accessibility right out of the gate with this new designer, technical reasons prevented getting an early start on this feature, and resulted in unfortunate user experiences and deadlocks under assistive technology tooling in certain scenarios.

While the team is rebuilding the experience under the new model, it is straightforward to work around the current accessibility issues by using the classic .NET Framework in-process designer and building output assemblies in both .NET and .NET Framework 4.7.2 (or 4.8). In order to multi-target and use the .NET Framework designer use the following steps:

1.  Create your .NET WinForms 3.1 (C\# only), 5 or 6 app (both VB and C\#).

2.  Before the designer by opening a Form or a UserControl, edit the *.csproj*
    file.

3.  In that file you will find the following entry, telling the compiler what
    .NET version to build against:

```<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net6.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <UseWindowsForms>true</UseWindowsForms>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```
4.  Change the `<TargetFramework>net6.0-windows</TargetFramework>` to the following: `<TargetFrameworks>net472;net6.0-windows</TargetFrameworks>`

**Note:** it is important to use **net472** (or net48 if you want to target against .NET Framework 4.8) **first** in the  list of targeted frameworks. This tells Visual Studio to load the in-process .NET Framework rather than the .NET OOP WinForms designer.

Once you’ve completed the above steps, you can build your application and interact with the forms directly as you have always done. When you build binaries, they will be built targeting both .NET 6 and .NET Framework 4.7.2. 

:warning: There are limitations around only using the features and types available in both .NET Framework 4.7.2 and .NET 6. If you want to use .NET 6 features, you will need to use C\# preprocessor directives.

We will be communicating out as soon as full accessibility support is available in the WinForms .NET OOP designer so that you can remove the need to multi-target. In the meantime, this workaround should allow you to build your .NET Windows Forms  applications with the respective accessibility features supported at design time.

Happy Coding!
