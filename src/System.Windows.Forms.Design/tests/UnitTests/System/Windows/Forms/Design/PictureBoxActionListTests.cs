// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design.Tests;

public sealed class PictureBoxActionListTests
{
    [Fact]
    public void SizeMode_ShouldReturnCorrectValue()
    {
        using PictureBox pictureBox = new() { SizeMode = PictureBoxSizeMode.StretchImage };
        using PictureBoxDesigner designer = new();
        designer.Initialize(pictureBox);
        PictureBoxActionList actionList = new(designer);

        actionList.SizeMode.Should().Be(PictureBoxSizeMode.StretchImage);
    }

    [Fact]
    public void SizeMode_ShouldUpdateCorrectly()
    {
        using PictureBox pictureBox = new();
        using PictureBoxDesigner designer = new();
        designer.Initialize(pictureBox);
        PictureBoxActionList actionList = new(designer)
        {
            SizeMode = PictureBoxSizeMode.Zoom
        };

        pictureBox.SizeMode.Should().Be(PictureBoxSizeMode.Zoom);
    }

    [Fact]
    public void GetSortedActionItems_ShouldReturnCorrectItems()
    {
        using PictureBox pictureBox = new();
        using PictureBoxDesigner designer = new();
        designer.Initialize(pictureBox);
        PictureBoxActionList actionList = new(designer);

        DesignerActionItemCollection items = actionList.GetSortedActionItems();

        items.Should().NotBeNull();
        items.Count.Should().Be(2);
        DesignerActionMethodItem methodItem = items[0].Should().BeOfType<DesignerActionMethodItem>().Which;
        DesignerActionPropertyItem propertyItem = items[1].Should().BeOfType<DesignerActionPropertyItem>().Which;

        methodItem.MemberName.Should().Be("ChooseImage");
        propertyItem.MemberName.Should().Be("SizeMode");
    }
}
