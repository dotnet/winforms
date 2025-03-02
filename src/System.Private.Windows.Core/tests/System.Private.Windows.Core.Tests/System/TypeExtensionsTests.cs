// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Formats.Nrbf;
using System.Reflection.Metadata;
using System.Runtime.Serialization;

namespace System.Tests;

public class TypeExtensionsTests
{
    public static TheoryData<Type, TypeName, int, bool> Matches_Type_Data() => new()
    {
        { typeof(int), TypeName.Parse(typeof(int).AssemblyQualifiedName), (int)TypeNameComparison.All, true },
        { typeof(int), TypeName.Parse(typeof(int).AssemblyQualifiedName), (int)TypeNameComparison.AllButAssemblyVersion, true },
        { typeof(int), TypeName.Parse(typeof(int).AssemblyQualifiedName), (int)TypeNameComparison.TypeFullName, true },
        { typeof(int), TypeName.Parse(typeof(int).AssemblyQualifiedName), (int)TypeNameComparison.AssemblyVersion, true },
        { typeof(int), TypeName.Parse(typeof(int).AssemblyQualifiedName), (int)TypeNameComparison.AssemblyCultureName, true },
        { typeof(int), TypeName.Parse(typeof(int).AssemblyQualifiedName), (int)TypeNameComparison.AssemblyName, true },
        { typeof(int), TypeName.Parse(typeof(int).AssemblyQualifiedName), (int)TypeNameComparison.AssemblyPublicKeyToken, true },
        { typeof(int), TypeName.Parse($"System.Int32, {Assemblies.Mscorlib}"), (int)TypeNameComparison.AllButAssemblyVersion, false },
        { typeof(int), TypeName.Parse($"System.Int32, {Assemblies.Mscorlib}"), (int)TypeNameComparison.TypeFullName, true },
        { typeof(int), TypeName.Parse($"Int32, {Assemblies.Mscorlib}"), (int)TypeNameComparison.TypeFullName, false },
        { typeof(int?), TypeName.Parse(typeof(int).AssemblyQualifiedName), (int)TypeNameComparison.AllButAssemblyVersion, false },
        { typeof(int?), TypeName.Parse(typeof(int?).AssemblyQualifiedName), (int)TypeNameComparison.All, true },
        { typeof(int?[]), TypeName.Parse($"System.Nullable`1[[System.Int32, {Assemblies.Mscorlib}]][], {Assemblies.Mscorlib}"), (int)TypeNameComparison.AllButAssemblyVersion, false},
        { typeof(DayOfWeek), TypeName.Parse($"System.Nullable`1[[System.DayOfWeek, {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}"), (int)TypeNameComparison.AllButAssemblyVersion, false },
        // Culture is incorrect.
        { typeof(int), TypeName.Parse(typeof(int).AssemblyQualifiedName!.Replace("neutral", "en-US")), (int)TypeNameComparison.AssemblyCultureName, false },
        // Namespace name is incorrect.
        { typeof(int), TypeName.Parse("Int32"), (int)TypeNameComparison.TypeFullName, false },
        // Namespace name is cased differently.
        { typeof(int), TypeName.Parse("SYSTEM.Int32"), (int)TypeNameComparison.TypeFullName, false },
        // Assembly information is missing.
        { typeof(int), TypeName.Parse("System.Int32"), (int)TypeNameComparison.AllButAssemblyVersion, false },
        // Assembly name is cased differently.
        { typeof(int), TypeName.Parse($"System.Int32, {typeof(int).Assembly.FullName!.ToUpperInvariant()}"), (int)TypeNameComparison.AssemblyName, false },
    };

    [Theory]
    [MemberData(nameof(Matches_Type_Data))]
    public void Matches_Type(Type type, TypeName typeName, int comparison, bool matches)
    {
        type.Matches(typeName, (TypeNameComparison)comparison).Should().Be(matches);
        TypeName.Parse(type.AssemblyQualifiedName).Matches(typeName, (TypeNameComparison)comparison).Should().Be(matches);
    }

    [Fact]
    public void Matches_Type_InvalidKey()
    {
        // We assert here as we're not expecting to see this exception in normal usage.
        using NoAssertContext noAsserts = new();
        bool success = typeof(int).Matches(
            TypeName.Parse(typeof(int).AssemblyQualifiedName!.Replace("7cec85d7bea7798e", "7cec00000ea7798e")),
            TypeNameComparison.AllButAssemblyVersion);
        success.Should().BeFalse();
    }

    public static TheoryData<TypeName, TypeName, bool> Matches_Type_Mscorlib_Data() => new()
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
    [MemberData(nameof(Matches_Type_Mscorlib_Data))]
    public void Matches_Type_Mscorlib(TypeName x, TypeName y, bool matches) =>
        // Match TypeName objects including assembly version.
        x.Matches(y, TypeNameComparison.All).Should().Be(matches);

    [Serializable]
    private class MyGenericClass<T>
    {
    }

    [Serializable]
    private class MyClass
    {
    }

    public static TheoryData<object, string[]> BinaryFormatter_BinderTypes_Data => new()
    {
        { 42, ["System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"] },
        { new List<int>(), ["System.Collections.Generic.List`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"] },
        {
            new List<MyClass>(),
            [
                "System.Collections.Generic.List`1[[System.Tests.TypeExtensionsTests+MyClass, System.Private.Windows.Core.Tests, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                "System.Tests.TypeExtensionsTests+MyClass, System.Private.Windows.Core.Tests, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
            ]
        }
    };

    [Theory]
    [MemberData(nameof(BinaryFormatter_BinderTypes_Data))]
    public void BinaryFormatter_BinderTypes(object value, string[] expected)
    {
        using MemoryStream stream = new();
        stream.WriteBinaryFormat(value);
        stream.Position = 0;

        List<string> bindings = [];

        _ = BinarySerialization.DeserializeFromStream(
            stream,
            binder: new BindToTypeBinder(
                (string assembly, string type) =>
                {
                    bindings.Add($"{type}, {assembly}");
                    return null;
                }));

        bindings.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [MemberData(nameof(BinaryFormatter_BinderTypes_Data))]
    public void BinaryFormatter_NrbfDecoder_Types(object value, string[] expected)
    {
        SerializationRecord record = BinarySerialization.SerializeAndDecode(value, out _);
        record.TypeName.AssemblyQualifiedName.Should().Be(expected[0]);
    }

    public class BindToTypeBinder : SerializationBinder
    {
        private readonly Func<string, string, Type?> _bindToType;
        public BindToTypeBinder(Func<string, string, Type?> bindToType) => _bindToType = bindToType;
        public override Type BindToType(string assemblyName, string typeName) => _bindToType(assemblyName, typeName)!;
        public override void BindToName(Type serializedType, out string? assemblyName, out string typeName) => throw new NotImplementedException();
    }
}
