// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public sealed class DesignBindingTests
{
    [Fact]
    public void Null_ReturnsPropertiesWithNullValues()
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
        DesignBinding binding = new(dataSource, dataMember);

        binding.DataSource.Should().Be(dataSource);
        binding.DataMember.Should().Be(dataMember);
    }

    [Fact]
    public void IsNull_ReturnsTrueWhenDataSourceIsNull()
    {
        DesignBinding binding = new(null, "TestMember");
        binding.IsNull.Should().BeTrue();
    }

    [Fact]
    public void IsNull_ReturnsFalseWhenDataSourceIsNotNull()
    {
        DesignBinding binding = new(new object(), "TestMember");
        binding.IsNull.Should().BeFalse();
    }

    public static TheoryData<string, string> DataFieldTestData =>
    new()
    {
        { "", "" },
        { "Field", "Field" },
        { "Object.Field", "Field" },
        { "Object.SubObject.Field", "Field" }
    };

    [Theory]
    [MemberData(nameof(DataFieldTestData))]
    public void DataField_ReturnsExpected(string dataMember, string expectedField)
    {
        DesignBinding binding = new(new object(), dataMember);
        binding.DataField.Should().Be(expectedField);
    }

    [Fact]
    public void Equals_ReturnsTrueForMatchingDataSourceAndDataMember()
    {
        object dataSource = new();
        string dataMember = "TestMember";
        DesignBinding binding = new(dataSource, dataMember);
        binding.Equals(dataSource, dataMember).Should().BeTrue();
    }

    [Fact]
    public void Equals_ReturnsFalseForNonMatchingDataSource()
    {
        object dataSource = new();
        string dataMember = "TestMember";
        DesignBinding binding = new(dataSource, dataMember);
        binding.Equals(new object(), dataMember).Should().BeFalse();
    }

    [Fact]
    public void Equals_ReturnsFalseForNonMatchingDataMember()
    {
        object dataSource = new();
        string dataMember = "TestMember";
        DesignBinding binding = new(dataSource, dataMember);
        binding.Equals(dataSource, "DifferentMember").Should().BeFalse();
    }
}
