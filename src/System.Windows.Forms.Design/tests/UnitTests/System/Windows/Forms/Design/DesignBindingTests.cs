// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public sealed class DesignBindingTests
{
    [Fact]
    public void Null_ReturnsDesignBindingWithNullValues()
    {
        var nullBinding = DesignBinding.Null;

        nullBinding.DataSource.Should().BeNull();
        nullBinding.DataMember.Should().BeNull();
    }

    [Fact]
    public void Constructor_InitializesProperties()
    {
        object dataSource = new();
        string dataMember = "TestMember";
        var binding = new DesignBinding(dataSource, dataMember);

        binding.DataSource.Should().Be(dataSource);
        binding.DataMember.Should().Be(dataMember);
    }

    [Fact]
    public void IsNull_ReturnsTrueWhenDataSourceIsNull()
    {
        var binding = new DesignBinding(null, "TestMember");
        binding.IsNull.Should().BeTrue();
    }

    [Fact]
    public void IsNull_ReturnsFalseWhenDataSourceIsNotNull()
    {
        var binding = new DesignBinding(new object(), "TestMember");
        binding.IsNull.Should().BeFalse();
    }

    public static IEnumerable<object[]> DataFieldTestData =>
    new List<object[]>
    {
        new object[] { "", "" },
        new object[] { "Field", "Field" },
        new object[] { "Object.Field", "Field" },
        new object[] { "Object.SubObject.Field", "Field" }
    };

    [Theory]
    [MemberData(nameof(DataFieldTestData))]
    public void DataField_ReturnsCorrectField(string dataMember, string expectedField)
    {
        var binding = new DesignBinding(new object(), dataMember);
        binding.DataField.Should().Be(expectedField);
    }

    [Fact]
    public void Equals_ReturnsTrueForMatchingDataSourceAndDataMember()
    {
        object dataSource = new();
        string dataMember = "TestMember";
        var binding = new DesignBinding(dataSource, dataMember);
        binding.Equals(dataSource, dataMember ?? string.Empty).Should().BeTrue();
    }

    [Fact]
    public void Equals_ReturnsFalseForNonMatchingDataSource()
    {
        object dataSource = new();
        string dataMember = "TestMember";
        var binding = new DesignBinding(dataSource, dataMember);
        binding.Equals(new object(), dataMember ?? string.Empty).Should().BeFalse();
    }

    [Fact]
    public void Equals_ReturnsFalseForNonMatchingDataMember()
    {
        object dataSource = new();
        string dataMember = "TestMember";
        var binding = new DesignBinding(dataSource, dataMember);
        binding.Equals(dataSource, "DifferentMember").Should().BeFalse();
    }
}
