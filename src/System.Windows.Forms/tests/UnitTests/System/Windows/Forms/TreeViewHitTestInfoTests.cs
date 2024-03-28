// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class TreeViewHitTestInfoTests
{
    public static IEnumerable<object[]> Ctor_TreeNode_TreeViewHitTestLocations_TestData()
    {
        foreach (TreeViewHitTestLocations hitLocation in Enum.GetValues(typeof(TreeViewHitTestLocations)))
        {
            yield return new object[] { null, hitLocation };
            yield return new object[] { new TreeNode(), hitLocation };
        }

        yield return new object[] { null, 0 };
        yield return new object[] { new TreeNode(), 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_TreeNode_TreeViewHitTestLocations_TestData))]
    public void TreeViewHitTestInfo_Ctor_TreeNode_TreeViewHitTestLocations(TreeNode hitNode, TreeViewHitTestLocations hitLocation)
    {
        TreeViewHitTestInfo hitTestInfo = new(hitNode, hitLocation);
        Assert.Same(hitNode, hitTestInfo.Node);
        Assert.Equal(hitLocation, hitTestInfo.Location);
    }
}
