// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;

namespace System.Resources.Tests;

public class ResXSerializationBinderTests
{
    [Fact]
    public void ResXSerializationBinder_BindToType_FullyQualifiedName()
    {
        TrackGetTypePathTypeResolutionService typeResolutionService = new();
        ResXSerializationBinder binder = new(typeResolutionService);
        binder.BindToType(typeof(Button).Assembly.FullName!, typeof(Button).FullName!).Should().Be(typeof(Button));
        typeResolutionService.FullyQualifiedAssemblyNamePath.Should().BeTrue();
        typeResolutionService.FullyQualifiedAssemblyNameNoVersionPath.Should().BeFalse();
    }

    [Fact]
    public void ResXSerializationBinder_BindToType_FullName()
    {
        TrackGetTypePathTypeResolutionService typeResolutionService = new();
        ResXSerializationBinder binder = new(typeResolutionService);
        binder.BindToType(typeof(MyClass).Assembly.FullName!, typeof(MyClass).FullName!).Should().Be(typeof(MyClass));
        typeResolutionService.FullNamePath.Should().BeTrue();
        typeResolutionService.FullyQualifiedAssemblyNameNoVersionPath.Should().BeFalse();
    }

    [Fact]
    public void ResXSerializationBinder_BindToType_FullyQualifiedNameNoVersion()
    {
        TrackGetTypePathTypeResolutionService typeResolutionService = new();
        ResXSerializationBinder binder = new(typeResolutionService);
        binder.BindToType(typeof(Form).Assembly.FullName!, typeof(Form).FullName!).Should().Be(typeof(Form));
        typeResolutionService.FullyQualifiedAssemblyNameNoVersionPath.Should().BeTrue();
        typeResolutionService.FullNamePath.Should().BeFalse();
    }

    [Fact]
    public void ResXSerializationBinder_BindToName_TypeNameConverter_NoNameChange()
    {
        ResXSerializationBinder binder = new(type => type?.AssemblyQualifiedName ?? string.Empty);
        binder.BindToName(typeof(Button), out string? assemblyName, out string? typeName);
        assemblyName.Should().Be(typeof(Button).Assembly.FullName);
        typeName.Should().BeNull();
    }

    [Fact]
    public void ResXSerializationBinder_BindToName_TypeNameConverter_NameChange()
    {
        string testAssemblyName = "TestAssembly";
        string testTypeName = "TestType";
        ResXSerializationBinder binder = new(type => $"{testTypeName}, {testAssemblyName}");
        binder.BindToName(typeof(Button), out string? assemblyName, out string? typeName);
        assemblyName.Should().Be(testAssemblyName);
        typeName.Should().Be(typeName);
    }

    private class TrackGetTypePathTypeResolutionService : ITypeResolutionService
    {
        public bool FullNamePath { get; private set; }
        public bool FullyQualifiedAssemblyNamePath { get; private set; }
        public bool FullyQualifiedAssemblyNameNoVersionPath { get; private set; }

        public Assembly? GetAssembly(AssemblyName name) => throw new NotImplementedException();
        public Assembly? GetAssembly(AssemblyName name, bool throwOnError) => throw new NotImplementedException();
        public string? GetPathOfAssembly(AssemblyName name) => throw new NotImplementedException();
        public Type? GetType(string typeName)
        {
            if (typeName == $"{typeof(Button).FullName}, {typeof(Button).Assembly.FullName}")
            {
                FullyQualifiedAssemblyNamePath = true;
                return typeof(Button);
            }
            else if (typeName == typeof(MyClass).FullName)
            {
                FullNamePath = true;
                return typeof(MyClass);
            }
            else
            {
                TypeName parsed = TypeName.Parse($"{typeof(Form).FullName}, {typeof(Form).Assembly.FullName}");
                AssemblyNameInfo? assemblyNameInfo = parsed.AssemblyName;
                string formNoVersionFullyQualifiedName = $"{typeof(Form).FullName}, {new AssemblyNameInfo(
                    assemblyNameInfo!.Name,
                    version: null,
                    assemblyNameInfo.CultureName,
                    assemblyNameInfo.Flags,
                    assemblyNameInfo.PublicKeyOrToken).FullName}";
                if (typeName == formNoVersionFullyQualifiedName)
                {
                    FullyQualifiedAssemblyNameNoVersionPath = true;
                    return typeof(Form);
                }
            }

            return null;
        }

        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        public Type? GetType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] string name, bool throwOnError) => throw new NotImplementedException();
        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        public Type? GetType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] string name, bool throwOnError, bool ignoreCase) => throw new NotImplementedException();
        public void ReferenceAssembly(AssemblyName name) => throw new NotImplementedException();
    }

    private class MyClass { }
}
