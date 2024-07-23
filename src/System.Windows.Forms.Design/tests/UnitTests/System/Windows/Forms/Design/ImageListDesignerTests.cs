// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public sealed class ImageListDesignerTests
{
    [Fact]
    public void ActionLists_Get_ShouldContainExpectedItems()
    {
        using ImageListDesigner imageListDesigner = new();
        using ImageList imageList = new();
        imageListDesigner.Initialize(imageList);

        imageListDesigner.ActionLists.Should().NotBeNull();
        imageListDesigner.ActionLists.Cast<object>().Should().HaveCount(1);
        imageListDesigner.ActionLists[0].Should().BeOfType<ImageListActionList>();
    }
}
