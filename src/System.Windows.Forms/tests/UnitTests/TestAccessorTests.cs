// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.CSharp.RuntimeBinder;

namespace System.Windows.Forms.Tests;

public class TestAccessorTests
{
    [Fact]
    public void TestAccessor_DynamicAccess_InstanceField()
    {
        PrivateTestClass testClass = new();
        dynamic access = testClass.TestAccessor().Dynamic;
        access._integer = 5;
        Assert.Equal(5, access._integer);
    }

    [Fact]
    public void TestAccessor_DynamicAccess_StaticField()
    {
        // Don't need to create an instance to access a static, can
        // use typeof instead
        dynamic access = typeof(PrivateTestClass).TestAccessor().Dynamic;
        access.s_integer = 16;
        Assert.Equal(16, access.s_integer);

        // Attempt using an instance as well
        PrivateTestClass testClass = new();
        access = testClass.TestAccessor().Dynamic;
        access.s_integer = 18;
        Assert.Equal(18, access.s_integer);

        // Try the static class version
        access = typeof(PrivateStaticTestClass).TestAccessor().Dynamic;
        access.s_integer = 21;
        Assert.Equal(21, access.s_integer);
    }

    [Fact]
    public void TestAccessor_DynamicAccess_ReadOnlyInstanceField()
    {
        PrivateTestClass testClass = new();
        dynamic access = testClass.TestAccessor().Dynamic;
        access._readOnlyInteger = 7;
        Assert.Equal(7, access._readOnlyInteger);
    }

    [Fact]
    public void TestAccessor_DynamicAccess_ObjectInstanceField()
    {
        PrivateTestClass testClass = new();
        dynamic access = testClass.TestAccessor().Dynamic;
        List<string> list = access._list;
        Assert.NotNull(list);
        Assert.Single(list);
        Assert.Equal("42", list[0]);
    }

    [Fact]
    public void TestAccessor_DynamicAccess_InstanceProperty()
    {
        PrivateTestClass testClass = new();
        dynamic access = testClass.TestAccessor().Dynamic;
        access.Long = 1970;
        Assert.Equal(1970, access.Long);
    }

    [Fact]
    public void TestAccessor_DynamicAccess_StaticProperty()
    {
        PrivateTestClass testClass = new();
        dynamic access = testClass.TestAccessor().Dynamic;
        access.Int = 1989;
        Assert.Equal(1989, access.Int);

        // Now try without an instance
        access = typeof(PrivateTestClass).TestAccessor().Dynamic;
        access.Int = 1988;
        Assert.Equal(1988, access.Int);

        // Try the static class version
        access = typeof(PrivateStaticTestClass).TestAccessor().Dynamic;
        access.Int = 1991;
        Assert.Equal(1991, access.Int);
    }

    [Fact]
    public void TestAccessor_DynamicAccess_PublicField()
    {
        PrivateTestClass testClass = new();
        dynamic access = testClass.TestAccessor().Dynamic;

        // If the API is public we want to access "normally", so we prevent this.
        Assert.Throws<RuntimeBinderException>(() => access.PublicField = 1918);
    }

    [Fact]
    public void TestAccessor_DynamicAccess_PublicProperty()
    {
        PrivateTestClass testClass = new();
        dynamic access = testClass.TestAccessor().Dynamic;

        // If the API is public we want to access "normally", so we prevent this.
        Assert.Throws<RuntimeBinderException>(() => access.PublicProperty = "What?");
    }

    [Fact]
    public void TestAccessor_DynamicAccess_InstanceMethod()
    {
        PrivateTestClass testClass = new();
        Assert.Equal(4, testClass.TestAccessor().Dynamic.ToStringLength(2001));
        Assert.Equal(7, testClass.TestAccessor().Dynamic.ToStringLength("Flubber"));
    }

    [Fact]
    public void TestAccessor_FuncDelegateAccess_InstanceMethod()
    {
        PrivateTestClass testClass = new();
        ITestAccessor accessor = testClass.TestAccessor();
        Assert.Equal(4, accessor.CreateDelegate<Func<int, int>>("ToStringLength")(2001));
        Assert.Equal(7, accessor.CreateDelegate<Func<string, int>>("ToStringLength")("Flubber"));
    }

    [Fact]
    public void TestAccessor_NamedDelegateAccess_InstanceMethod()
    {
        PrivateTestClass testClass = new();
        ITestAccessor accessor = testClass.TestAccessor();
        Assert.Equal(5, accessor.CreateDelegate<ToStringLength>()("25624"));
    }

    [Fact]
    public void TestAccessor_DynamicAccess_StaticMethod()
    {
        PrivateTestClass testClass = new();
        Assert.Equal(2, testClass.TestAccessor().Dynamic.AddOne(1));

        // Hit the static class version
        Assert.Equal(3, typeof(PrivateStaticTestClass).TestAccessor().Dynamic.AddOne(2));
    }

    [Fact]
    public void TestAccessor_FuncDelegateAccess_StaticMethod()
    {
        PrivateTestClass testClass = new();
        Assert.Equal(2000, testClass.TestAccessor().CreateDelegate<Func<int, int>>("AddOne")(1999));

        // Hit the static class version
        Assert.Equal(21, typeof(PrivateStaticTestClass).TestAccessor().CreateDelegate<Func<int, int>>("AddOne")(20));
    }

    [Fact]
    public void TestAccessor_DynamicAccess_UpcastType()
    {
        A a = new B();
        dynamic accessor = a.TestAccessor().Dynamic;
        accessor._b = 3;
        Assert.Equal(3, accessor._b);
    }

    [Fact]
    public void TestAccessor_DynamicAccess_BaseClassField()
    {
        A a = new B();
        dynamic accessor = a.TestAccessor().Dynamic;
        accessor._a = 5;
        Assert.Equal(5, accessor._a);
    }

    [Fact]
    public void TestAccessor_DynamicAccess_BaseClassMethod()
    {
        A a = new B();
        dynamic accessor = a.TestAccessor().Dynamic;
        Assert.Equal(42, (int)accessor.AMethod());
    }

    // As you can't use a ref struct as a generic parameter to Action/Func, you
    // need to use a defined delegate to access an internal method that takes
    // or returns a ref struct (such as Spans).

    public delegate int ToStringLength(ReadOnlySpan<char> value);

#pragma warning disable IDE0044   // use readonly
#pragma warning disable IDE0051   // unaccessed private
#pragma warning disable IDE0052   // unaccessed private
#pragma warning disable CS0169    // unused field
#pragma warning disable CA1823    // unused field
#pragma warning disable CA1822    // Mark members as static
#pragma warning disable CA1051 // Do not declare visible instance fields

    public class A
    {
        private int AMethod() => 42;
        private int _a;
    }

    public class B : A
    {
        private int _b;
    }

    public class PrivateTestClass
    {
        private int _integer;
        private readonly int _readOnlyInteger;
        private List<string> _list = ["42"];

        private long Long { get; set; }

        public float PublicField;
        public string PublicProperty { get; set; }

        // It is important that we have multiple methods with the same name for testing.
        private int ToStringLength(int value) => ToStringLength(value.ToString());
        private int ToStringLength(string value) => value.Length;
        private int ToStringLength(ReadOnlySpan<char> value) => value.Length;

        private static int s_integer;
        private static int Int { get; set; }

        private static int AddOne(int value) => value + 1;
    }

    public static class PrivateStaticTestClass
    {
        private static int s_integer;
        private static int Int { get; set; }
        private static int AddOne(int value) => value + 1;
    }
#pragma warning restore IDE0044
#pragma warning restore IDE0051
#pragma warning restore IDE0052
#pragma warning restore CS0169
#pragma warning restore CA1823
#pragma warning restore CA1822
#pragma warning restore CA1051
}
