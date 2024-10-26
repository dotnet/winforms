// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;

namespace Microsoft.VisualBasic.ApplicationServices.Tests;

public class AssemblyInfoTests
{
    [Fact]
    public void Constructor_ArgumentNullException()
    {
        Action action = () => new AssemblyInfo(null);
        action.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [MemberData(nameof(AssemblyProperties_TestData))]
    public void AssemblyProperties(Assembly assembly)
    {
        AssemblyInfo assemblyInfo = new(assembly);
        var assemblyName = assembly.GetName();
        assemblyInfo.AssemblyName.Should().Be(assemblyName.Name);
        assemblyInfo.DirectoryPath.Should().Be(Path.GetDirectoryName(assembly.Location));
        assemblyInfo.CompanyName.Should().Be(GetAttributeValue<AssemblyCompanyAttribute>(assembly, attr => attr.Company));
        assemblyInfo.Copyright.Should().Be(GetAttributeValue<AssemblyCopyrightAttribute>(assembly, attr => attr.Copyright));
        assemblyInfo.Description.Should().Be(GetAttributeValue<AssemblyDescriptionAttribute>(assembly, attr => attr.Description));
        assemblyInfo.ProductName.Should().Be(GetAttributeValue<AssemblyProductAttribute>(assembly, attr => attr.Product));
        assemblyInfo.Title.Should().Be(GetAttributeValue<AssemblyTitleAttribute>(assembly, attr => attr.Title));
        assemblyInfo.Trademark.Should().Be(GetAttributeValue<AssemblyTrademarkAttribute>(assembly, attr => attr.Trademark));
        assemblyInfo.Version.Should().Be(assemblyName.Version);
    }

    public static IEnumerable<object[]> AssemblyProperties_TestData()
    {
        yield return new object[] { typeof(object).Assembly };
        yield return new object[] { Assembly.GetExecutingAssembly() };
    }

    [Fact]
    public void LoadedAssemblies()
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        AssemblyInfo assemblyInfo = new(executingAssembly);
        var loadedAssemblies = assemblyInfo.LoadedAssemblies;
        loadedAssemblies.Should().Contain(executingAssembly);
    }

    [Fact]
    public void StackTrace()
    {
        // Property is independent of the actual assembly.
        AssemblyInfo assemblyInfo = new(Assembly.GetExecutingAssembly());
        string stackTrace = assemblyInfo.StackTrace;
        stackTrace.Should().Contain(nameof(AssemblyInfoTests));
    }

    [Fact]
    public void WorkingSet()
    {
        // Property is independent of the actual assembly.
        AssemblyInfo assemblyInfo = new(Assembly.GetExecutingAssembly());
        long workingSet = assemblyInfo.WorkingSet;
        workingSet.Should().BeGreaterThan(0);
    }

    private static string GetAttributeValue<TAttribute>(Assembly assembly, Func<TAttribute, string> getAttributeValue)
        where TAttribute : Attribute
    {
        var attribute = (TAttribute)assembly.GetCustomAttribute(typeof(TAttribute));
        return (attribute is null) ? "" : getAttributeValue(attribute);
    }
}
