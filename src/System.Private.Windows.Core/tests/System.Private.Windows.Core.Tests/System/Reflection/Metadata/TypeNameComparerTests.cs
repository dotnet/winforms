// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Reflection.Metadata.Tests;

public class TypeNameComparerTests
{
    private class TestType { }

    public static TheoryData<TypeName, Type> TypeNameComparer_FullyQualified_Success() => new()
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
    [MemberData(nameof(TypeNameComparer_FullyQualified_Success))]
    public void DictionaryLookup_FullyQualified_Succeeds(TypeName name, Type expected)
    {
        Dictionary<TypeName, Type> types = new(TypeNameComparer.FullyQualifiedMatch)
        {
            { TypeName.Parse(typeof(int).AssemblyQualifiedName), typeof(int) },
            { TypeName.Parse(typeof(int[]).AssemblyQualifiedName), typeof(int[]) },
            { TypeName.Parse(typeof(List<int>).AssemblyQualifiedName), typeof(List<int>) },
            { TypeName.Parse(typeof(TestType).AssemblyQualifiedName), typeof(TestType) },
        };

        types.TryGetValue(name, out Type? resolvedType).Should().BeTrue();
        resolvedType.Should().Be(expected);
    }

    public static TheoryData<TypeName> TypeNameComparer_FullyQualified_Fail() =>
    [
        TypeName.Parse("System.Int32[], System.Private.CoreLib, Version=9.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"),
        TypeName.Parse("System.Int32, System.Private.CoreLib, Version=9.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"),
        TypeName.Parse("System.Int32, System.Private.CoreLib, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"),
        TypeName.Parse($"System.Collections.Generic.List`1[[System.Int32, System.Private.CoreLib, Version=9.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]], {typeof(List<int>).Assembly.FullName}"),
        TypeName.Parse($"{typeof(TestType).FullName}, {typeof(int).Assembly.FullName}")
    ];

    [Theory]
    [MemberData(nameof(TypeNameComparer_FullyQualified_Fail))]
    [MemberData(nameof(TypeNameComparer_FullNameMatch_Fail))]
    [MemberData(nameof(TypeNameComparer_FullNameAndAssemblyNameMatch_Fail))]
    public void DictionaryLookup_FullyQualified_VersionMismatch(TypeName name)
    {
        Dictionary<TypeName, Type> types = new(TypeNameComparer.FullyQualifiedMatch)
        {
            { TypeName.Parse(typeof(int).AssemblyQualifiedName), typeof(int) },
            { TypeName.Parse(typeof(int[]).AssemblyQualifiedName), typeof(int[]) },
            { TypeName.Parse(typeof(List<int>).AssemblyQualifiedName), typeof(List<int>) },
            { TypeName.Parse(typeof(TestType).AssemblyQualifiedName), typeof(TestType) },
        };

        types.TryGetValue(name, out Type? _).Should().BeFalse();
    }

    [Fact]
    public void DictionaryLookup_FullyQualified_Fails()
    {
        Dictionary<TypeName, Type> types = new(TypeNameComparer.FullyQualifiedMatch)
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
        var comparer = TypeNameComparer.FullyQualifiedMatch;

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
        var comparer = TypeNameComparer.FullyQualifiedMatch;

        int hash = comparer.GetHashCode(TypeName.Parse(typeof(int).AssemblyQualifiedName));
        comparer.GetHashCode(TypeName.Parse(typeof(int).AssemblyQualifiedName)).Should().Be(hash);
        comparer.GetHashCode(TypeName.Parse($"{typeof(int).FullName}, {typeof(int).Assembly.FullName}")).Should().Be(hash);
    }

    public static TheoryData<TypeName, Type> TypeNameComparer_FullNameMatch_Success() => new()
    {
        { TypeName.Parse("System.Int32"), typeof(int) },
        { TypeName.Parse("System.Int32[]"), typeof(int[]) },
        { TypeName.Parse("System.Collections.Generic.List`1[[System.Int32]]"), typeof(List<int>) },
        { TypeName.Parse(typeof(TestType).FullName), typeof(TestType) },
    };

    [Theory]
    [MemberData(nameof(TypeNameComparer_FullNameMatch_Success))]
    [MemberData(nameof(TypeNameComparer_FullNameAndAssemblyNameMatch_Success))]
    [MemberData(nameof(TypeNameComparer_FullyQualified_Success))]
    public void DictionaryLookup_FullNameMatch_Succeeds(TypeName name, Type expected)
    {
        var types = new Dictionary<TypeName, Type>(TypeNameComparer.FullNameMatch)
        {
            { TypeName.Parse(typeof(int).FullName), typeof(int) },
            { TypeName.Parse(typeof(int[]).FullName), typeof(int[]) },
            { TypeName.Parse(typeof(List<int>).FullName), typeof(List<int>) },
            { TypeName.Parse(typeof(TestType).FullName), typeof(TestType) },
        };

        types.TryGetValue(name, out var resolvedType).Should().BeTrue();
        resolvedType.Should().Be(expected);
    }

    public static TheoryData<TypeName> TypeNameComparer_FullNameMatch_Fail() =>
    [
        TypeName.Parse("UnknownType"),
        TypeName.Parse("List`1[[UnknownType]]"),
        TypeName.Parse("Int32"),
        TypeName.Parse("Int32[]"),
        TypeName.Parse("System.Collections.Generic.List`1[[Int32]]"),
        TypeName.Parse(nameof(TestType))
    ];

    [Theory]
    [MemberData(nameof(TypeNameComparer_FullNameMatch_Fail))]
    public void DictionaryLookup_FullNameMatch_Fails(TypeName name)
    {
        var types = new Dictionary<TypeName, Type>(TypeNameComparer.FullNameMatch)
        {
            { TypeName.Parse(typeof(int).FullName), typeof(int) },
            { TypeName.Parse(typeof(int[]).FullName), typeof(int[]) },
            { TypeName.Parse(typeof(List<int>).FullName), typeof(List<int>) },
            { TypeName.Parse(typeof(TestType).FullName), typeof(TestType) },
        };

        types.TryGetValue(name, out _).Should().BeFalse();
    }

    public static TheoryData<TypeName, Type> TypeNameComparer_FullNameAndAssemblyNameMatch_Success() => new()
    {
        { TypeName.Parse($"{typeof(int).FullName}, {typeof(int).Assembly.GetName().Name}"), typeof(int) },
        { TypeName.Parse($"{typeof(int[]).FullName}, {typeof(int).Assembly.GetName().Name}"), typeof(int[]) },
        { TypeName.Parse($"{typeof(List<int>).FullName}, {typeof(List<int>).Assembly.GetName().Name}"), typeof(List<int>) },
        { TypeName.Parse($"{typeof(TestType).FullName}, {typeof(TestType).Assembly.GetName().Name}"), typeof(TestType) },
    };

    [Theory]
    [MemberData(nameof(TypeNameComparer_FullNameAndAssemblyNameMatch_Success))]
    [MemberData(nameof(TypeNameComparer_FullyQualified_Success))]
    public void DictionaryLookup_FullNameAndAssemblyNameMatch_Succeeds(TypeName name, Type expected)
    {
        var types = new Dictionary<TypeName, Type>(TypeNameComparer.FullNameAndAssemblyNameMatch)
        {
            { TypeName.Parse($"{typeof(int).FullName}, {typeof(int).Assembly.GetName().Name}"), typeof(int) },
            { TypeName.Parse($"{typeof(int[]).FullName}, {typeof(int).Assembly.GetName().Name}"), typeof(int[]) },
            { TypeName.Parse($"{typeof(List<int>).FullName}, {typeof(List<int>).Assembly.GetName().Name}"), typeof(List<int>) },
            { TypeName.Parse($"{typeof(TestType).FullName}, {typeof(TestType).Assembly.GetName().Name}"), typeof(TestType) },
        };

        types.TryGetValue(name, out var resolvedType).Should().BeTrue();
        resolvedType.Should().Be(expected);
    }

    public static TheoryData<TypeName> TypeNameComparer_FullNameAndAssemblyNameMatch_Fail() => new()
    {
        // Different assembly name
        TypeName.Parse($"{typeof(int).FullName}, SomeOtherAssembly"),
        // Different full name
        TypeName.Parse($"SomeNamespace.SomeType, {typeof(int).Assembly.GetName().Name}"),
        // Different full name and assembly name
        TypeName.Parse("SomeNamespace.SomeType, SomeOtherAssembly"),
        // Missing assembly name
        TypeName.Parse($"{typeof(int).FullName}"),
        // Different type but same assembly name
        TypeName.Parse($"System.String, {typeof(int).Assembly.GetName().Name}"),
    };

    [Theory]
    [MemberData(nameof(TypeNameComparer_FullNameAndAssemblyNameMatch_Fail))]
    [MemberData(nameof(TypeNameComparer_FullNameMatch_Fail))]
    public void DictionaryLookup_FullNameAndAssemblyNameMatch_Fails(TypeName name)
    {
        var types = new Dictionary<TypeName, Type>(TypeNameComparer.FullNameAndAssemblyNameMatch)
        {
            { TypeName.Parse($"{typeof(int).FullName}, {typeof(int).Assembly.GetName().Name}"), typeof(int) },
            { TypeName.Parse($"{typeof(int[]).FullName}, {typeof(int).Assembly.GetName().Name}"), typeof(int[]) },
            { TypeName.Parse($"{typeof(List<int>).FullName}, {typeof(List<int>).Assembly.GetName().Name}"), typeof(List<int>) },
            { TypeName.Parse($"{typeof(TestType).FullName}, {typeof(TestType).Assembly.GetName().Name}"), typeof(TestType) },
        };

        types.TryGetValue(name, out _).Should().BeFalse();
    }
}
