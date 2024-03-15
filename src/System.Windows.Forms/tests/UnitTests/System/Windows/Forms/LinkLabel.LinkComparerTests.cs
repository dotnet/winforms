// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class LinkLabel_LinkComparerTests
{
    public static IEnumerable<object[]> LinkCompare_Return_IsExpected_TestData()
    {
        yield return new object[] { null, null, 0 };
        yield return new object[] { null, new LinkLabel.Link(0, 1), -1 };
        yield return new object[] { new LinkLabel.Link(0, 1), null, 1 };
        yield return new object[] { new LinkLabel.Link(0, 1), new LinkLabel.Link(0, 1), 0 };
        yield return new object[] { new LinkLabel.Link(1, 5), new LinkLabel.Link(2, 5), -1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(LinkCompare_Return_IsExpected_TestData))]
    public void LinkCompare_Return_IsExpected(object link1, object link2, int expectedValue)
    {
        using LinkLabel linkLabel = new();
        var comparer = linkLabel.TestAccessor().Dynamic.s_linkComparer;
        var compareMethod = comparer.GetType().GetMethod("Compare");
        Assert.Equal(expectedValue, compareMethod.Invoke(comparer, (object[])[link1, link2]));
    }
}
