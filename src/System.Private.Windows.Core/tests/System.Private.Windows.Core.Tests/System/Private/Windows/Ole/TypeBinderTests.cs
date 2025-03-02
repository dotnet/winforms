// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Nrbf;
using System.Private.Windows.Nrbf;
using System.Reflection.Metadata;

namespace System.Private.Windows.Ole;

public class TypeBinderTests
{
    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(string))]
    [InlineData(typeof(MyClass))]
    public void BindToType_NoResolver_UntypedRequest_AlwaysSucceeds(Type type)
    {
        // Suppressing asserts. We normally don't want to be letting through SerializationBinder calls
        // without BinaryFormatting being enabled, but we don't need to turn it on for this test.
        using NoAssertContext noAsserts = new();
        DataRequest request = new("test") { TypedRequest = false };
        TypeBinder<AlwaysDefaultSerializer> binder = new(type, in request);
        binder.BindToType("Foo", "Bar").Should().BeNull();
    }

    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(string))]
    [InlineData(typeof(MyClass))]
    public void BindToType_NoResolver_TypedRequest_MatchesSucceed(Type type)
    {
        DataRequest request = new("test") { TypedRequest = true };
        TypeBinder<AlwaysDefaultSerializer> binder = new(type, in request);

        // You must have a resolver for typed requests when being called through the BinaryFormatter path.
        Action action = () => binder.BindToType(type.Assembly.FullName!, type.FullName!).Should().Be(type);
        action.Should().Throw<InvalidOperationException>();
    }

    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(string))]
    [InlineData(typeof(int[]))]
    [InlineData(typeof(List<string>))]
    public void BindToType_NoResolver_TypedRequest_CoreSerializer_Fails(Type type)
    {
        DataRequest request = new()
        {
            Format = "test",
            TypedRequest = true
        };

        TypeBinder<CoreNrbfSerializer> binder = new(typeof(MyClass), in request);

        // You must have a resolver for typed requests when being called through the BinaryFormatter path.
        Action action = () => binder.BindToType(type.Assembly.FullName!, type.FullName!).Should().Be(type);
        action.Should().Throw<InvalidOperationException>();

        TypeName typeName = TypeName.Parse(type.AssemblyQualifiedName);
        binder.TryBindToType(typeName, out Type? boundType).Should().BeFalse();
        boundType.Should().BeNull();

        action = () => binder.BindToType(typeName);
        action.Should().Throw<NotSupportedException>();
    }

    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(string))]
    [InlineData(typeof(int[]))]
    [InlineData(typeof(List<string>))]
    public void BindToType_Resolver_TypedRequest_CoreSerializer_SupportedTypesSucceed(Type type)
    {
        DataRequest request = new()
        {
            Format = "test",
            TypedRequest = true,
            Resolver = (TypeName typeName) => typeof(MyClass).Matches(typeName) ? typeof(MyClass) : null
        };

        TypeBinder<CoreNrbfSerializer> binder = new(typeof(MyClass), in request);

        binder.BindToType(type.Assembly.FullName!, type.FullName!).Should().Be(type);

        TypeName typeName = TypeName.Parse(type.AssemblyQualifiedName);
        binder.TryBindToType(typeName, out Type? boundType).Should().BeTrue();
        boundType.Should().Be(type);

        binder.BindToType(typeName).Should().Be(type);
    }

    [Theory]
    [InlineData(typeof(Hashtable))]
    [InlineData(typeof(ArrayList))]
    public void BindToType_NoResolver_TypedRequest_CoreSerializer_UnsupportedTypesFail(Type type)
    {
        DataRequest request = new("test") { TypedRequest = true };
        TypeBinder<CoreNrbfSerializer> binder = new(typeof(MyClass), in request);
        Action action = () => binder.BindToType(type.Assembly.FullName!, type.FullName!);
        action.Should().Throw<InvalidOperationException>();

        TypeName typeName = TypeName.Parse(type.AssemblyQualifiedName);
        binder.TryBindToType(typeName, out Type? boundType).Should().Be(false);
        boundType.Should().BeNull();

        action = () => binder.BindToType(typeName);
        action.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void BindToType_Resolver_CoreSerializer_Matches_DoesNotReturnSupportedType()
    {
        DataRequest request = new("test")
        {
            TypedRequest = true,
            Resolver = (TypeName typeName) => typeof(MyClass)
        };

        // Shouldn't fall back to the core serializer.
        TypeBinder<CoreNrbfSerializer> binder = new(typeof(int), in request);
        binder.TryBindToType(TypeName.Parse(typeof(int).AssemblyQualifiedName), out Type? boundType).Should().Be(true);
        boundType.Should().Be<MyClass>();
    }

    [Fact]
    public void BindToType_Resolver_CoreSerializer_Throws_DoesNotReturnSupportedType()
    {
        DataRequest request = new("test")
        {
            TypedRequest = true,
            Resolver = (TypeName typeName) => throw new NotSupportedException()
        };

        // Shouldn't fall back to the core serializer.
        TypeBinder<CoreNrbfSerializer> binder = new(typeof(int), in request);
        binder.TryBindToType(TypeName.Parse(typeof(int).AssemblyQualifiedName), out Type? boundType).Should().Be(false);
        boundType.Should().BeNull();
    }

    [Fact]
    public void BindToType_Resolver_CoreSerializer_Null_ReturnsSupportedType()
    {
        DataRequest request = new("test")
        {
            TypedRequest = true,
            Resolver = (TypeName typeName) => null!
        };

        // Shouldn't fall back to the core serializer.
        TypeBinder<CoreNrbfSerializer> binder = new(typeof(int), in request);
        binder.TryBindToType(TypeName.Parse(typeof(int).AssemblyQualifiedName), out Type? boundType).Should().Be(true);
        boundType.Should().Be<int>();
    }

    internal sealed class AlwaysDefaultSerializer : INrbfSerializer
    {
        private AlwaysDefaultSerializer() { }

        public static bool TryBindToType(TypeName typeName, [NotNullWhen(true)] out Type? type)
        {
            type = null;
            return false;
        }

        public static bool TryGetObject(SerializationRecord record, [NotNullWhen(true)] out object? value)
        {
            value = null;
            return false;
        }

        public static bool TryWriteObject(Stream stream, object value) => false;

        public static bool IsFullySupportedType(Type type) => false;
    }

    internal sealed class BindingAlwaysSucceedsSerializer : INrbfSerializer
    {
        private BindingAlwaysSucceedsSerializer() { }

        public static bool TryBindToType(TypeName typeName, [NotNullWhen(true)] out Type? type)
        {
            type = Type.GetType(typeName.AssemblyQualifiedName);
            return type is not null;
        }

        public static bool TryGetObject(SerializationRecord record, [NotNullWhen(true)] out object? value)
        {
            value = null;
            return false;
        }

        public static bool TryWriteObject(Stream stream, object value) => false;

        public static bool IsFullySupportedType(Type type) => true;
    }

    internal sealed class AlwaysReturnsTypeSerializer<T> : INrbfSerializer
        where T : class, new()
    {
        private AlwaysReturnsTypeSerializer() { }

        public static bool TryBindToType(TypeName typeName, [NotNullWhen(true)] out Type? type)
        {
            type = typeof(T);
            return true;
        }

        public static bool TryGetObject(SerializationRecord record, [NotNullWhen(true)] out object? value)
        {
            value = null;
            return false;
        }

        public static bool TryWriteObject(Stream stream, object value) => false
        ;
        public static bool IsFullySupportedType(Type type) => true;
    }

    private class MyClass { }
}
