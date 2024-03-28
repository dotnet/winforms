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

    [Fact]
    public void GetApplicationInstanceID()
    {
        var assembly = typeof(WindowsFormsApplicationBaseTests).Assembly;
        string expectedId = assembly.ManifestModule.ModuleVersionId.ToString();
        Assert.Equal(expectedId, GetAppID(assembly));
    }

    private static string GetUniqueIDFromAssembly(string guid, Version version)
    {
        CustomAttributeBuilder attributeBuilder = new(
            typeof(GuidAttribute).GetConstructor([typeof(string)]), new[] { guid });
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName(Guid.NewGuid().ToString()) { Version = version },
            AssemblyBuilderAccess.RunAndCollect,
            new[] { attributeBuilder });
        assemblyBuilder.DefineDynamicModule(Guid.NewGuid().ToString());
        return GetAppID(assemblyBuilder);
    }

    [Fact]
    public void GetApplicationInstanceID_GuidAttribute()
    {
        string guid = Guid.NewGuid().ToString();
        Assert.Equal($"{guid}1.2", GetUniqueIDFromAssembly(guid, new Version(1, 2, 3, 4)));
    }

    [Fact]
    public void GetApplicationInstanceID_GuidAttributeNewVersion()
    {
        string guid = Guid.NewGuid().ToString();
        Assert.Equal($"{guid}0.0", GetUniqueIDFromAssembly(guid, new Version()));
    }

    [Fact]
    public void GetApplicationInstanceID_GuidAttributeNullVersion()
    {
        string guid = Guid.NewGuid().ToString();
        Assert.Equal($"{guid}0.0", GetUniqueIDFromAssembly(guid, version: null));
    }
}
