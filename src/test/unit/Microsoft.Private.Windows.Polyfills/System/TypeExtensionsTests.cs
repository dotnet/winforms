// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Tests;

public class TypeExtensionsTests
{
    [Fact]
    public void IsAssignableTo_SameType_ReturnsTrue()
    {
        typeof(string).IsAssignableTo(typeof(string)).Should().BeTrue();
    }

    [Fact]
    public void IsAssignableTo_DerivedToBase_ReturnsTrue()
    {
        typeof(string).IsAssignableTo(typeof(object)).Should().BeTrue();
    }

    [Fact]
    public void IsAssignableTo_BaseToDerived_ReturnsFalse()
    {
        typeof(object).IsAssignableTo(typeof(string)).Should().BeFalse();
    }

    [Fact]
    public void IsAssignableTo_ImplementsInterface_ReturnsTrue()
    {
        typeof(string).IsAssignableTo(typeof(IComparable)).Should().BeTrue();
    }

    [Fact]
    public void IsAssignableTo_DoesNotImplementInterface_ReturnsFalse()
    {
        typeof(int).IsAssignableTo(typeof(IDisposable)).Should().BeFalse();
    }

    [Fact]
    public void IsAssignableTo_NullTarget_ReturnsFalse()
    {
        typeof(string).IsAssignableTo(null!).Should().BeFalse();
    }

    [Fact]
    public void IsTypeDefinition_RegularClass_ReturnsTrue()
    {
        typeof(string).IsTypeDefinition.Should().BeTrue();
    }

    [Fact]
    public void IsTypeDefinition_Array_ReturnsFalse()
    {
        typeof(int[]).IsTypeDefinition.Should().BeFalse();
    }

    [Fact]
    public void IsTypeDefinition_ByRef_ReturnsFalse()
    {
        typeof(int).MakeByRefType().IsTypeDefinition.Should().BeFalse();
    }

    [Fact]
    public void IsTypeDefinition_Pointer_ReturnsFalse()
    {
        typeof(int).MakePointerType().IsTypeDefinition.Should().BeFalse();
    }

    [Fact]
    public void IsTypeDefinition_ConstructedGenericType_ReturnsFalse()
    {
        typeof(List<int>).IsTypeDefinition.Should().BeFalse();
    }

    [Fact]
    public void IsTypeDefinition_OpenGenericDefinition_ReturnsTrue()
    {
        typeof(List<>).IsTypeDefinition.Should().BeTrue();
    }

    [Fact]
    public void IsTypeDefinition_ValueType_ReturnsTrue()
    {
        typeof(int).IsTypeDefinition.Should().BeTrue();
    }

    [Fact]
    public void IsTypeDefinition_Interface_ReturnsTrue()
    {
        typeof(IDisposable).IsTypeDefinition.Should().BeTrue();
    }

    [Fact]
    public void IsSZArray_SingleDimensionalArray_ReturnsTrue()
    {
        typeof(int[]).IsSZArray.Should().BeTrue();
    }

    [Fact]
    public void IsSZArray_ReferenceTypeArray_ReturnsTrue()
    {
        typeof(string[]).IsSZArray.Should().BeTrue();
    }

    [Fact]
    public void IsSZArray_MultiDimensionalArray_ReturnsFalse()
    {
        typeof(int[,]).IsSZArray.Should().BeFalse();
    }

    [Fact]
    public void IsSZArray_JaggedArray_ReturnsTrue()
    {
        typeof(int[][]).IsSZArray.Should().BeTrue();
    }

    [Fact]
    public void IsSZArray_NonArrayType_ReturnsFalse()
    {
        typeof(int).IsSZArray.Should().BeFalse();
    }

    [Fact]
    public void IsSZArray_NonArrayClassType_ReturnsFalse()
    {
        typeof(string).IsSZArray.Should().BeFalse();
    }

    [Fact]
    public void IsSZArray_Rank1GeneralArray_ReturnsFalse()
    {
        // Array.CreateInstance with a lower bound creates a general rank-1 array (int[*]),
        // which is distinct from an SZ array (int[]).
        Type rank1General = Array.CreateInstance(typeof(int), [1], [1]).GetType();
        rank1General.IsSZArray.Should().BeFalse();
    }
}
