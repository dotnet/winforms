// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Tests;

public class EnumExtensionsTests
{
    private enum TestEnum
    {
        None = 0,
        First = 1,
        Second = 2,
        Third = 3,
    }

    [Flags]
    private enum TestFlagsEnum
    {
        None = 0,
        A = 1,
        B = 2,
        C = 4,
        All = A | B | C,
    }

    [Fact]
    public void IsDefined_DefinedValue_ReturnsTrue()
    {
        Enum.IsDefined(TestEnum.First).Should().BeTrue();
    }

    [Fact]
    public void IsDefined_UndefinedValue_ReturnsFalse()
    {
        Enum.IsDefined((TestEnum)99).Should().BeFalse();
    }

    [Fact]
    public void IsDefined_Zero_DefinedAsNone_ReturnsTrue()
    {
        Enum.IsDefined(TestEnum.None).Should().BeTrue();
    }

    [Fact]
    public void IsDefined_FlagsEnum_DefinedSingleValue_ReturnsTrue()
    {
        Enum.IsDefined(TestFlagsEnum.A).Should().BeTrue();
    }

    [Fact]
    public void IsDefined_FlagsEnum_DefinedCombinedValue_ReturnsTrue()
    {
        Enum.IsDefined(TestFlagsEnum.All).Should().BeTrue();
    }

    [Fact]
    public void IsDefined_FlagsEnum_UndefinedCombination_ReturnsFalse()
    {
        // A | B = 3 is not explicitly defined in the enum (All = 7)
        Enum.IsDefined((TestFlagsEnum)(TestFlagsEnum.A | TestFlagsEnum.B)).Should().BeFalse();
    }

    [Fact]
    public void IsDefined_AllDefinedValues_ReturnTrue()
    {
        Enum.IsDefined(TestEnum.None).Should().BeTrue();
        Enum.IsDefined(TestEnum.First).Should().BeTrue();
        Enum.IsDefined(TestEnum.Second).Should().BeTrue();
        Enum.IsDefined(TestEnum.Third).Should().BeTrue();
    }

    [Fact]
    public void GetValues_ReturnsAllDefinedValues()
    {
        TestEnum[] values = Enum.GetValues<TestEnum>();
        values.Should().BeEquivalentTo([TestEnum.None, TestEnum.First, TestEnum.Second, TestEnum.Third]);
    }

    [Fact]
    public void GetValues_FlagsEnum_ReturnsAllDefinedValues()
    {
        TestFlagsEnum[] values = Enum.GetValues<TestFlagsEnum>();
        values.Should().BeEquivalentTo([TestFlagsEnum.None, TestFlagsEnum.A, TestFlagsEnum.B, TestFlagsEnum.C, TestFlagsEnum.All]);
    }

    [Fact]
    public void GetValues_ReturnsCorrectOrder()
    {
        TestEnum[] values = Enum.GetValues<TestEnum>();
        values.Should().ContainInOrder(TestEnum.None, TestEnum.First, TestEnum.Second, TestEnum.Third);
    }
}
