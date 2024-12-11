// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;
using System.Private.Windows.Core.BinaryFormat;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms.Tests;

public class TypeExtensionsTests
{
    public static TheoryData<Type, TypeName, bool> MatchesNameAndAssemblyLessVersionData() => new()
    {
        // int type is forwarded to mscorlib, type name with CoreLib will not match.
        { typeof(int), TypeName.Parse(typeof(int).AssemblyQualifiedName), false },
        { typeof(int), TypeName.Parse($"System.Int32, {Mscorlib}"), true },
        { typeof(int?), TypeName.Parse($"System.Int32, {Mscorlib}"), true },
        { typeof(int?[]), TypeName.Parse($"System.Nullable`1[[System.Int32, {Mscorlib}]][], {Mscorlib}"), true},
        { typeof(DayOfWeek), TypeName.Parse($"System.Nullable`1[[System.DayOfWeek, {Mscorlib}]], {Mscorlib}"), false },
        { typeof(Bitmap), TypeName.Parse("System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), true },
        // Assembly version is incorrect.
        { typeof(Bitmap), TypeName.Parse("System.Drawing.Bitmap, System.Drawing, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), true },
        // Public key token is incorrect.
        { typeof(Bitmap), TypeName.Parse("System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f1AAAAAAA"), false },
        // Culture is incorrect.
        { typeof(Bitmap), TypeName.Parse("System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=en-US, PublicKeyToken=b03f5f7f11d50a3a"), false },
        // Namespace name is incorrect.
        { typeof(Bitmap), TypeName.Parse("System.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), false },
        { typeof(Bitmap), TypeName.Parse("System.Drawing.MyBitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), false },
        { typeof(Bitmap?[]), TypeName.Parse("System.Drawing.Bitmap[], System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), true },
        { typeof(Dictionary<string, Bitmap>), TypeName.Parse($"System.Collections.Generic.Dictionary`2[[System.String, {Mscorlib}],[System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], {Mscorlib}"), true },
        { typeof(Dictionary<string, Bitmap?>), TypeName.Parse($"System.Collections.Generic.Dictionary`2[[System.String, {Mscorlib}],[System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], {Mscorlib}"), true },
        { typeof(NonForwardedType), TypeName.Parse("System.Windows.Forms.Tests.TypeExtensionsTests.NonForwardedType"), false },
        // Namespace name is cased differently.
        { typeof(NonForwardedType), TypeName.Parse("System.Windows.Forms.Tests.TypeExtensionstests+NonForwardedType"), false },
        // Assembly information is missing.
        { typeof(NonForwardedType), TypeName.Parse("System.Windows.Forms.Tests.TypeExtensionsTests+NonForwardedType"), false },
        { typeof(NonForwardedType), TypeName.Parse("System.Windows.Forms.Tests.TypeExtensionsTests+NonForwardedType, System.Windows.Forms.Tests"), false },
        // Assembly name is cased differently.
        { typeof(NonForwardedType), TypeName.Parse("System.Windows.Forms.Tests.TypeExtensionsTests+NonForwardedType, System.windows.Forms.Tests, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"), true },
        // Namespace name is incorrect.
        { typeof(NonForwardedType), TypeName.Parse("System.Windows.Forms.Tests.typeExtensionsTests+NonForwardedType, System.Windows.Forms.Tests"), false },
        { typeof(ForwardedType), TypeName.Parse("System.Windows.Forms.Tests.TypeExtensionsTests+ForwardedType, Abc"), true },
    };

    [Theory]
    [MemberData(nameof(MatchesNameAndAssemblyLessVersionData))]
    public void MatchesLessAssemblyVersion(Type type, TypeName typeName, bool matches) =>
        // Match full type names and assembly names without version.
        type.MatchExceptAssemblyVersion(typeName).Should().Be(matches);

    public static TheoryData<TypeName, TypeName, bool> MatchesTypeNameData() => new()
    {
        { TypeName.Parse($"System.Int32, {Mscorlib}"), TypeName.Parse($"System.Int32,  {Mscorlib}"), true },
        { TypeName.Parse($"System.Int32, {Mscorlib}"), TypeName.Parse($"System.String, {Mscorlib}"), false },
        { TypeName.Parse($"System.Collections.Generic.Dictionary`2[[System.String, {Mscorlib}],[System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], {Mscorlib}"), TypeName.Parse($"System.Collections.Generic.Dictionary`2[[System.String, {Mscorlib}],[System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], {Mscorlib}"), true },
        { TypeName.Parse($"System.Collections.Generic.Dictionary`2[[System.String, {Mscorlib}],[System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], {Mscorlib}"), TypeName.Parse($"System.Collections.Generic.Dictionary`2[[System.String, {Mscorlib}],[System.Drawing.Bitmap, System.Drawing, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], {Mscorlib}"), false },
        { TypeName.Parse($"System.Collections.Generic.Dictionary`2[[System.String, {Mscorlib}],[System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], {Mscorlib}"), TypeName.Parse($"System.Collections.Generic.Dictionary`2[[System.String, {Mscorlib}],[System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], mscorlib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"), false },
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
        name.Should().Be(Mscorlib);
        typeof(int).TryGetForwardedFromName(out name).Should().BeTrue();
        name.Should().Be(Mscorlib);
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

    private const string Mscorlib = TypeInfo.MscorlibAssemblyName;

    [Fact]
    public void ForwardedTypeToTypeName()
    {
        TypeName name = typeof(ForwardedType).ToTypeName();
        name.FullName.Should().Be("System.Windows.Forms.Tests.TypeExtensionsTests+ForwardedType");
        name.AssemblyName!.FullName.Should().Be("Abc");

        name = typeof(ForwardedType[]).ToTypeName();
        name.FullName.Should().Be("System.Windows.Forms.Tests.TypeExtensionsTests+ForwardedType[]");
        name.AssemblyName!.FullName.Should().Be("Abc");

        name = typeof(ForwardedType?[]).ToTypeName();
        name.FullName.Should().Be("System.Windows.Forms.Tests.TypeExtensionsTests+ForwardedType[]");
        name.AssemblyName!.FullName.Should().Be("Abc");

        name = typeof(List<ForwardedType>).ToTypeName();
        name.FullName.Should().Be($"System.Collections.Generic.List`1[[System.Windows.Forms.Tests.TypeExtensionsTests+ForwardedType, Abc]]");
        name.AssemblyName!.FullName.Should().Be(Mscorlib);

        name = typeof(List<Dictionary<int, string>>).ToTypeName();
        name.FullName.Should().Be($"System.Collections.Generic.List`1[[System.Collections.Generic.Dictionary`2[[System.Int32, {Mscorlib}],[System.String, {Mscorlib}]], {Mscorlib}]]");
        name.AssemblyName!.FullName.Should().Be(Mscorlib);

        name = typeof(List<Dictionary<string, int?>>).ToTypeName();
        name.FullName.Should().Be($"System.Collections.Generic.List`1[[System.Collections.Generic.Dictionary`2[[System.String, {Mscorlib}],[System.Nullable`1[[System.Int32, {Mscorlib}]], {Mscorlib}]], {Mscorlib}]]");
        name.AssemblyName!.FullName.Should().Be(Mscorlib);

        name = typeof(List<Dictionary<int, string?>>).ToTypeName();
        name.FullName.Should().Be($"System.Collections.Generic.List`1[[System.Collections.Generic.Dictionary`2[[System.Int32, {Mscorlib}],[System.String, {Mscorlib}]], {Mscorlib}]]");
        name.AssemblyName!.FullName.Should().Be(Mscorlib);
    }

    [TypeForwardedFrom("Abc")]
    private class ForwardedType { }

    private class NonForwardedType { }
}
