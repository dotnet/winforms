// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public class MaskDescriptorComparerTests
{
    internal class TestMaskDescriptor : MaskDescriptor
    {
        public override string? Mask { get; }
        public override string? Name { get; }
        public override string? Sample { get; }
        public override Type? ValidatingType { get; }

        public TestMaskDescriptor(string name, string sample, Type validatingType)
        {
            Name = name;
            Sample = sample;
            ValidatingType = validatingType;
        }
    }

    [WinFormsFact]
    public void Constructor_ShouldAssignSortTypeAndSortOrder()
    {
        MaskDescriptorComparer comparer = new(MaskDescriptorComparer.SortType.ByName, SortOrder.Ascending);
        comparer.Should().NotBeNull();
    }

    [WinFormsTheory]
    [InlineData(MaskDescriptorComparer.SortType.ByName, SortOrder.Ascending, -1)]
    [InlineData(MaskDescriptorComparer.SortType.ByName, SortOrder.Descending, 1)]
    [InlineData(MaskDescriptorComparer.SortType.BySample, SortOrder.Ascending, -1)]
    [InlineData(MaskDescriptorComparer.SortType.BySample, SortOrder.Descending, 1)]
    [InlineData(MaskDescriptorComparer.SortType.ByValidatingTypeName, SortOrder.Ascending, -1)]
    [InlineData(MaskDescriptorComparer.SortType.ByValidatingTypeName, SortOrder.Descending, 1)]
    internal void Compare_ShouldSortCorrectlyAccordingToSortTypeAndOrder(MaskDescriptorComparer.SortType sortType, SortOrder sortOrder, int expectedComparisonSign)
    {
        TestMaskDescriptor maskDescriptorA = new("A", "SampleA", typeof(int));
        TestMaskDescriptor maskDescriptorB = new("B", "SampleB", typeof(string));
        MaskDescriptorComparer comparer = new(sortType, sortOrder);

        int comparisonResult = comparer.Compare(maskDescriptorA, maskDescriptorB);
        comparisonResult.Should().Be(expectedComparisonSign);
    }

    [WinFormsTheory]
    [InlineData("Name", "Sample", typeof(int), "NameA", "SampleA", typeof(int), true)]
    [InlineData("NameA", "SampleA", typeof(int), "NameB", "SampleB", typeof(string), false)]
    public void GetHashCode_TestCases(string nameA, string sampleA, Type typeA, string nameB, string sampleB, Type typeB, bool expectSameHashCode)
    {
        TestMaskDescriptor maskDescriptorA = new(nameA, sampleA, typeA);
        TestMaskDescriptor maskDescriptorB = new(nameB, sampleB, typeB);

        int hashCodeA = MaskDescriptorComparer.GetHashCode(maskDescriptorA);
        int hashCodeB = MaskDescriptorComparer.GetHashCode(maskDescriptorB);

        if (expectSameHashCode)
        {
            hashCodeA.Should().Be(hashCodeB);
        }
        else
        {
            hashCodeA.Should().NotBe(hashCodeB);
        }
    }

    [WinFormsTheory]
    [InlineData(null, null, true)]
    [InlineData("NameA", null, false)]
    [InlineData("NameA", "NameA", false)]
    [InlineData("NameA", "NameB", false)]
    public void Equals_ShouldReturnExpectedResults(string? nameA, string? nameB, bool expectedResult)
    {
        MaskDescriptor? maskDescriptorA = nameA is null ? null : new TestMaskDescriptor(nameA, "SampleA", typeof(int));
        MaskDescriptor? maskDescriptorB = nameB is null ? null : new TestMaskDescriptor(nameB, "SampleB", typeof(int));

        bool result = MaskDescriptorComparer.Equals(maskDescriptorA, maskDescriptorB);
        result.Should().Be(expectedResult);

        if (nameA is not null)
        {
            bool resultSameInstance = MaskDescriptorComparer.Equals(maskDescriptorA, maskDescriptorA);
            resultSameInstance.Should().BeTrue();
        }

        if (nameA is not null && nameB is not null && nameA == nameB)
        {
            TestMaskDescriptor maskDescriptorC = new(nameA, "SampleA", typeof(int));
            TestMaskDescriptor maskDescriptorD = new(nameA, "SampleA", typeof(int));
            bool resultDifferentInstances = MaskDescriptorComparer.Equals(maskDescriptorC, maskDescriptorD);
            resultDifferentInstances.Should().BeFalse();
        }
    }
}
