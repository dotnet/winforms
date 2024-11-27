// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Tests;

public class DataGridViewColumnDesignTimeVisibleAttributeTests
{
    public static readonly DataGridViewColumnDesignTimeVisibleAttribute s_defaultAttribute = new();

    [WinFormsTheory]
    [BoolData]
    public void Ctor_Bool_SetsVisible(bool visible)
    {
        DataGridViewColumnDesignTimeVisibleAttribute attribute = new(visible);
        attribute.Visible.Should().Be(visible);
    }

    [WinFormsFact]
    public void Ctor_Default_VisibleIsFalse()
    {
        s_defaultAttribute.Visible.Should().BeFalse();
    }

    [WinFormsTheory]
    [BoolData]
    public void YesNo_ReturnsExpectedAttributeWithVisible(bool visible)
    {
        DataGridViewColumnDesignTimeVisibleAttribute attribute = visible ? DataGridViewColumnDesignTimeVisibleAttribute.Yes : DataGridViewColumnDesignTimeVisibleAttribute.No;
        attribute.Visible.Should().Be(visible);
    }

    [WinFormsFact]
    public void Default_ReturnsYes()
    {
        DataGridViewColumnDesignTimeVisibleAttribute.Default.Should().BeSameAs(DataGridViewColumnDesignTimeVisibleAttribute.Yes);
    }

    [WinFormsFact]
    public void Equals_SameInstance_ReturnsTrue()
    {
        s_defaultAttribute.Equals(s_defaultAttribute).Should().BeTrue();
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    public void Equals_DifferentInstances_ReturnsExpected(bool value1, bool value2)
    {
        DataGridViewColumnDesignTimeVisibleAttribute attribute1 = new(value1);
        DataGridViewColumnDesignTimeVisibleAttribute attribute2 = new(value2);
        attribute1.Equals(attribute2).Should().Be(value2);
    }

    [WinFormsTheory]
    [BoolData]
    public void GetHashCode_ReturnsExpected(bool visible)
    {
        DataGridViewColumnDesignTimeVisibleAttribute attribute = new(visible);
        int expectedHashCode = HashCode.Combine(typeof(DataGridViewColumnDesignTimeVisibleAttribute), visible);
        attribute.GetHashCode().Should().Be(expectedHashCode);
    }

    [WinFormsTheory]
    [BoolData]
    public void IsDefaultAttribute_ReturnsExpected(bool visible)
    {
        DataGridViewColumnDesignTimeVisibleAttribute attribute = new(visible);
        bool expected = visible;
        attribute.IsDefaultAttribute().Should().Be(expected);
    }
}
