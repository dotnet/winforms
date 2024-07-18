// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Microsoft.VisualBasic.ApplicationServices.Tests;

public class WindowsFormsApplicationBaseTests
{
    private static string GetAppID(Assembly assembly)
    {
        var testAccessor = typeof(WindowsFormsApplicationBase).TestAccessor();
        return testAccessor.Dynamic.GetApplicationInstanceID(assembly);
    }

    private static Guid GetFilePathAsGuid(string filePath)
    {
        var testAccessor = typeof(WindowsFormsApplicationBase).TestAccessor();
        return testAccessor.Dynamic.GetFilePathAsGuid(filePath);
    }

    [Fact]
    public void GetApplicationInstanceID()
    {
        var assembly = typeof(WindowsFormsApplicationBaseTests).Assembly;
        string currentUserSID = System.Security.Principal.WindowsIdentity.GetCurrent().User.Value;
        string expectedId = $"{GetFilePathAsGuid(assembly.Location)}-{assembly.ManifestModule.ModuleVersionId}-{currentUserSID}";
        Assert.Equal(expectedId, GetAppID(assembly));
    }

    private static Assembly CreateDynamicAssembly(string guid, Version version)
    {
        CustomAttributeBuilder attributeBuilder = new(
            typeof(GuidAttribute).GetConstructor([typeof(string)]), new[] { guid });
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName(Guid.NewGuid().ToString()) { Version = version },
            AssemblyBuilderAccess.RunAndCollect,
            new[] { attributeBuilder });
        assemblyBuilder.DefineDynamicModule(Guid.NewGuid().ToString());
        return assemblyBuilder;
    }

    [Fact]
    public void GetApplicationInstanceID_GuidAttribute()
    {
        string guid = Guid.NewGuid().ToString();
        string currentUserSID = System.Security.Principal.WindowsIdentity.GetCurrent().User.Value;
        Version version = new Version(1, 2, 3, 4);
        var assembly = CreateDynamicAssembly(guid, version);
        string location = assembly.Location;
        Guid expectedPathGuid = GetFilePathAsGuid(location);
        Assert.Equal($"{expectedPathGuid}-{guid}{version.Major}.{version.Minor}-{currentUserSID}", GetAppID(assembly));
    }

    [Fact]
    public void GetApplicationInstanceID_GuidAttributeNewVersion()
    {
        string guid = Guid.NewGuid().ToString();
        string currentUserSID = System.Security.Principal.WindowsIdentity.GetCurrent().User.Value;
        Version version = new Version();
        var assembly = CreateDynamicAssembly(guid, version);
        string location = assembly.Location;
        Guid expectedPathGuid = GetFilePathAsGuid(location);
        Assert.Equal($"{expectedPathGuid}-{guid}0.0-{currentUserSID}", GetAppID(assembly));
    }

    [Fact]
    public void GetApplicationInstanceID_GuidAttributeNullVersion()
    {
        string guid = Guid.NewGuid().ToString();
        string currentUserSID = System.Security.Principal.WindowsIdentity.GetCurrent().User.Value;
        var assembly = CreateDynamicAssembly(guid, null);
        string location = assembly.Location;
        Guid expectedPathGuid = GetFilePathAsGuid(location);
        Assert.Equal($"{expectedPathGuid}-{guid}0.0-{currentUserSID}", GetAppID(assembly));
    }
}
