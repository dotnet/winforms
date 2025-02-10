// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace System.Tests;

public class TypeExtensionsTests
{
    public static TheoryData<Type, TypeName, bool> MatchesNameAndAssemblyLessVersionData() => new()
    {
        // int type is forwarded to mscorlib, type name with CoreLib will not match.
        { typeof(int), TypeName.Parse(typeof(int).AssemblyQualifiedName), false },
        { typeof(int), TypeName.Parse($"System.Int32, {Assemblies.Mscorlib}"), true },
        { typeof(int?), TypeName.Parse($"System.Int32, {Assemblies.Mscorlib}"), true },
        { typeof(int?[]), TypeName.Parse($"System.Nullable`1[[System.Int32, {Assemblies.Mscorlib}]][], {Assemblies.Mscorlib}"), true},
        { typeof(DayOfWeek), TypeName.Parse($"System.Nullable`1[[System.DayOfWeek, {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}"), false },
        // Assembly version is incorrect.
        { typeof(int), TypeName.Parse("System.Int32, System.Private.CoreLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"), false },
        // Public key token is incorrect.
        { typeof(int), TypeName.Parse(typeof(int).AssemblyQualifiedName!.Replace("7cec85d7bea7798e", "7cec00000ea7798e")), false },
        // Culture is incorrect.
        { typeof(int), TypeName.Parse(typeof(int).AssemblyQualifiedName!.Replace("neutral", "en-US")), false },
        // Namespace name is incorrect.
        { typeof(int), TypeName.Parse(typeof(int).AssemblyQualifiedName!.Replace("System.Int32", "Int32")), false },
        { typeof(NonForwardedType), TypeName.Parse("System.Tests.TypeExtensionsTests.NonForwardedType"), false },
        // Namespace name is cased differently.
        { typeof(NonForwardedType), TypeName.Parse("System.Windows.Forms.Tests.TypeExtensionstests+NonForwardedType"), false },
        // Assembly information is missing.
        { typeof(NonForwardedType), TypeName.Parse("System.Tests.TypeExtensionsTests+NonForwardedType"), false },
        { typeof(NonForwardedType), TypeName.Parse("System.Tests.TypeExtensionsTests+NonForwardedType, System.Private.Windows.Core.Tests"), false },
        // Assembly name is cased differently.
        { typeof(NonForwardedType), TypeName.Parse("System.Tests.TypeExtensionsTests+NonForwardedType, System.private.Windows.Core.Tests, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"), true },
        // Namespace name is incorrect.
        { typeof(NonForwardedType), TypeName.Parse("System.Windows.Forms.Tests.typeExtensionsTests+NonForwardedType, System.Windows.Forms.Tests"), false },
        { typeof(ForwardedType), TypeName.Parse($"{typeof(ForwardedType).FullName}, Abc"), true },
    };

    [Theory]
    [MemberData(nameof(MatchesNameAndAssemblyLessVersionData))]
    public void MatchesLessAssemblyVersion(Type type, TypeName typeName, bool matches) =>
        // Match full type names and assembly names without version.
        type.MatchExceptAssemblyVersion(typeName).Should().Be(matches);

    public static TheoryData<TypeName, TypeName, bool> MatchesTypeNameData() => new()
    {
        { TypeName.Parse($"System.Int32, {Assemblies.Mscorlib}"), TypeName.Parse($"System.Int32,  {Assemblies.Mscorlib}"), true },
        { TypeName.Parse($"System.Int32, {Assemblies.Mscorlib}"), TypeName.Parse($"System.String, {Assemblies.Mscorlib}"), false },
        { TypeName.Parse($"System.Collections.Generic.Dictionary`2[[System.String, {Assemblies.Mscorlib}],[System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], {Assemblies.Mscorlib}"), TypeName.Parse($"System.Collections.Generic.Dictionary`2[[System.String, {Assemblies.Mscorlib}],[System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], {Assemblies.Mscorlib}"), true },
        { TypeName.Parse($"System.Collections.Generic.Dictionary`2[[System.String, {Assemblies.Mscorlib}],[System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], {Assemblies.Mscorlib}"), TypeName.Parse($"System.Collections.Generic.Dictionary`2[[System.String, {Assemblies.Mscorlib}],[System.Drawing.Bitmap, System.Drawing, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], {Assemblies.Mscorlib}"), false },
        { TypeName.Parse($"System.Collections.Generic.Dictionary`2[[System.String, {Assemblies.Mscorlib}],[System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], {Assemblies.Mscorlib}"), TypeName.Parse($"System.Collections.Generic.Dictionary`2[[System.String, {Assemblies.Mscorlib}],[System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], mscorlib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"), false },
        { TypeName.Parse($"System.Drawing.Bitmap[], System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), TypeName.Parse($"System.Drawing.Bitmap[], System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), true },
        { TypeName.Parse($"System.Drawing.Bitmap[], System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), TypeName.Parse($"System.Drawing.Bitmap[], System.Drawing, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), false },
        { TypeName.Parse($"System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), TypeName.Parse($"System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), true },
        { TypeName.Parse($"System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), TypeName.Parse($"System.Drawing.Bitmap, System.Drawing, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), false },
    };

    [Theory]
    [MemberData(nameof(MatchesTypeNameData))]
    public void Matches_TypeName(TypeName x, TypeName y, bool matches) =>
        // Match TypeName objects including assembly version.
        x.Matches(y).Should().Be(matches);

    [Fact]
    public void TryGetForwardedFromName_ReturnsTrue()
    {
        typeof(int?).TryGetForwardedFromName(out string? name).Should().BeTrue();
        name.Should().Be(Assemblies.Mscorlib);
        typeof(int).TryGetForwardedFromName(out name).Should().BeTrue();
        name.Should().Be(Assemblies.Mscorlib);
        typeof(ForwardedType).TryGetForwardedFromName(out name).Should().BeTrue();
        name.Should().Be("Abc");
        typeof(ForwardedType[]).TryGetForwardedFromName(out name).Should().BeTrue();
        name.Should().Be("Abc");
        typeof(ForwardedType?[]).TryGetForwardedFromName(out name).Should().BeTrue();
        name.Should().Be("Abc");
    }

    [Fact]
    public void TryGetForwardedFromName_ReturnsFalse()
    {
        typeof(NonForwardedType).TryGetForwardedFromName(out string? name).Should().BeFalse();
        name.Should().BeNull();
        typeof(NonForwardedType[]).TryGetForwardedFromName(out name).Should().BeFalse();
        name.Should().BeNull();
        typeof(NonForwardedType?[]).TryGetForwardedFromName(out name).Should().BeFalse();
        name.Should().BeNull();
    }

    [Fact]
    public void ForwardedTypeToTypeName()
    {
        TypeName name = typeof(ForwardedType).ToTypeName();
        name.FullName.Should().Be("System.Tests.TypeExtensionsTests+ForwardedType");
        name.AssemblyName!.FullName.Should().Be("Abc");

        name = typeof(ForwardedType[]).ToTypeName();
        name.FullName.Should().Be("System.Tests.TypeExtensionsTests+ForwardedType[]");
        name.AssemblyName!.FullName.Should().Be("Abc");

        name = typeof(ForwardedType?[]).ToTypeName();
        name.FullName.Should().Be("System.Tests.TypeExtensionsTests+ForwardedType[]");
        name.AssemblyName!.FullName.Should().Be("Abc");

        name = typeof(List<ForwardedType>).ToTypeName();
        name.FullName.Should().Be($"System.Collections.Generic.List`1[[System.Tests.TypeExtensionsTests+ForwardedType, Abc]]");
        name.AssemblyName!.FullName.Should().Be(Assemblies.Mscorlib);

        name = typeof(List<Dictionary<int, string>>).ToTypeName();
        name.FullName.Should().Be($"System.Collections.Generic.List`1[[System.Collections.Generic.Dictionary`2[[System.Int32, {Assemblies.Mscorlib}],[System.String, {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}]]");
        name.AssemblyName!.FullName.Should().Be(Assemblies.Mscorlib);

        name = typeof(List<Dictionary<string, int?>>).ToTypeName();
        name.FullName.Should().Be($"System.Collections.Generic.List`1[[System.Collections.Generic.Dictionary`2[[System.String, {Assemblies.Mscorlib}],[System.Nullable`1[[System.Int32, {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}]]");
        name.AssemblyName!.FullName.Should().Be(Assemblies.Mscorlib);

        name = typeof(List<Dictionary<int, string?>>).ToTypeName();
        name.FullName.Should().Be($"System.Collections.Generic.List`1[[System.Collections.Generic.Dictionary`2[[System.Int32, {Assemblies.Mscorlib}],[System.String, {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}]]");
        name.AssemblyName!.FullName.Should().Be(Assemblies.Mscorlib);
    }

    [TypeForwardedFrom("Abc")]
    private class ForwardedType { }

    private class NonForwardedType { }
}
