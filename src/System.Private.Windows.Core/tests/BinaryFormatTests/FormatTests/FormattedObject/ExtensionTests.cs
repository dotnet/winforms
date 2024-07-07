// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.Core.BinaryFormat;

namespace FormatTests.FormattedObject;

public class ExtensionTests
{
    [Theory]
    [MemberData(nameof(FlattenedArrayTestData))]
    public void SetArrayValueByFlattenedIndex_MultiDimensionalArray(int[] lengths)
    {
        Array array = Array.CreateInstance(typeof(int), lengths);

        for (int i = 0; i < array.LongLength; i++)
        {
            array.SetArrayValueByFlattenedIndex(i, i);
        }

        int[] flattened = array.GetArrayData<int>().ToArray();

        int[] expected = new int[array.LongLength];
        for (int i = 0; i < expected.Length; i++)
        {
            expected[i] = i;
        }

        flattened.Should().BeEquivalentTo(expected);
    }

    public static TheoryData<int[]> FlattenedArrayTestData { get; } = new()
    {
        new int[] { 2, 2 },
        new int[] { 3, 2 },
        new int[] { 2, 3 },
        new int[] { 3, 3, 3 },
        new int[] { 2, 3, 4 },
        new int[] { 4, 3, 2 },
        new int[] { 2, 3, 4, 5 }
    };
}
