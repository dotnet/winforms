// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Reflection.Metadata;

namespace System.Windows.Forms.Tests;

public class TypeNameComparerTests
{
    private class TestType { }

    public static TheoryData<TypeName, Type> TypeNameComparerSuccess() => new()
    {
        { TypeName.Parse(typeof(int).AssemblyQualifiedName), typeof(int) },
        { TypeName.Parse($"{typeof(int).FullName}, {typeof(int).Assembly.FullName}"), typeof(int) },
        { TypeName.Parse(typeof(int[]).AssemblyQualifiedName), typeof(int[]) },
        { TypeName.Parse($"{typeof(int[]).FullName}, {typeof(int[]).Assembly.FullName}"), typeof(int[]) },
        { TypeName.Parse(typeof(List<int>).AssemblyQualifiedName), typeof(List<int>) },
        { TypeName.Parse($"{typeof(List<int>).FullName}, {typeof(List<int>).Assembly.FullName}"), typeof(List<int>) },
        { TypeName.Parse(typeof(TestType).AssemblyQualifiedName), typeof(TestType) },
        { TypeName.Parse($"{typeof(TestType).FullName}, {typeof(TestType).Assembly.FullName}"), typeof(TestType) },
    };

    [Theory]
    [MemberData(nameof(TypeNameComparerSuccess))]
    public void DictionaryLookupSucceeds(TypeName name, Type expected)
    {
        Dictionary<TypeName, Type> types = new(DataObject.Composition.TypeNameComparer.Default)
        {
            { TypeName.Parse(typeof(int).AssemblyQualifiedName), typeof(int) },
            { TypeName.Parse(typeof(int[]).AssemblyQualifiedName), typeof(int[]) },
            { TypeName.Parse(typeof(List<int>).AssemblyQualifiedName), typeof(List<int>) },
            { TypeName.Parse(typeof(TestType).AssemblyQualifiedName), typeof(TestType) },
        };

        types.TryGetValue(name, out Type? resolvedType).Should().BeTrue();
        resolvedType.Should().Be(expected);
    }

    public static TheoryData<TypeName> TypeNameComparerFail() =>
    [
        TypeName.Parse("System.Int32[], System.Private.CoreLib, Version=9.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"),
        TypeName.Parse("System.Int32, System.Private.CoreLib, Version=9.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"),
        TypeName.Parse("System.Int32, System.Private.CoreLib, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"),
        TypeName.Parse($"System.Collections.Generic.List`1[[System.Int32, System.Private.CoreLib, Version=9.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]], {typeof(List<int>).Assembly.FullName}"),
        TypeName.Parse($"{typeof(TestType).FullName}, {typeof(int).Assembly.FullName}")
    ];

    [Theory]
    [MemberData(nameof(TypeNameComparerFail))]
    public void DictionaryLookupVersionMismatch(TypeName name)
    {
        Dictionary<TypeName, Type> types = new(DataObject.Composition.TypeNameComparer.Default)
        {
            { TypeName.Parse(typeof(int).AssemblyQualifiedName), typeof(int) },
            { TypeName.Parse(typeof(int[]).AssemblyQualifiedName), typeof(int[]) },
            { TypeName.Parse(typeof(List<int>).AssemblyQualifiedName), typeof(List<int>) },
            { TypeName.Parse(typeof(TestType).AssemblyQualifiedName), typeof(TestType) },
        };

        types.TryGetValue(name, out Type? _).Should().BeFalse();
    }

    [Fact]
    public void DictionaryLookupFails()
    {
        Dictionary<TypeName, Type> types = new(DataObject.Composition.TypeNameComparer.Default)
        {
            { TypeName.Parse(typeof(int).AssemblyQualifiedName), typeof(int) },
            { TypeName.Parse(typeof(int[]).AssemblyQualifiedName), typeof(int[]) },
            { TypeName.Parse(typeof(List<int>).AssemblyQualifiedName), typeof(List<int>) },
            { TypeName.Parse(typeof(TestType).AssemblyQualifiedName), typeof(TestType) },
        };

        TypeName name = TypeName.Parse(typeof(int).AssemblyQualifiedName);
        AssemblyNameInfo info = name.AssemblyName!;
        string testName = $"{name.FullName}, {info.Name}, Version={info.Version!}, Culture=neutral, PublicKeyToken=null";
        name = TypeName.Parse(testName);
        types.TryGetValue(name, out Type? _).Should().BeFalse();
    }

    [Fact]
    public void TypeNameComparer_Null()
    {
        var comparer = DataObject.Composition.TypeNameComparer.Default;

        comparer.Equals(null, null).Should().BeTrue();
        comparer.Equals(null, TypeName.Parse(typeof(int).AssemblyQualifiedName)).Should().BeFalse();
        comparer.Equals(TypeName.Parse(typeof(int).AssemblyQualifiedName), null).Should().BeFalse();
        comparer.Equals(
            TypeName.Parse(typeof(int).AssemblyQualifiedName),
            TypeName.Parse($"{typeof(int).FullName}, {typeof(int).Assembly.FullName}")).Should().BeTrue();
    }

    [Fact]
    public void TypeNameComparer_GetHashCode()
    {
        var comparer = DataObject.Composition.TypeNameComparer.Default;

        int hash = comparer.GetHashCode(TypeName.Parse(typeof(int).AssemblyQualifiedName));
        comparer.GetHashCode(TypeName.Parse(typeof(int).AssemblyQualifiedName)).Should().Be(hash);
        comparer.GetHashCode(TypeName.Parse($"{typeof(int).FullName}, {typeof(int).Assembly.FullName}")).Should().Be(hash);
    }
}
