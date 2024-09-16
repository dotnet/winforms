// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design.Tests;

public sealed class ImageListActionListTests : IDisposable
{
    private readonly ImageListDesigner _imageListDesigner;
    private readonly ImageList _imageList;
    private readonly ImageListActionList _actionList;
    public ImageListActionListTests()
    {
        _imageListDesigner = new();
        _imageList = new();
        _imageListDesigner.Initialize(_imageList);
        _actionList = new(_imageListDesigner);
    }

    public void Dispose()
    {
        _imageListDesigner.Dispose();
        _imageList.Dispose();
    }

    [Fact]
    public void Constructor_ShouldInitializeDesigner()
    {
        _actionList.Should().NotBeNull();
        _actionList.Should().BeOfType<ImageListActionList>();
        _actionList.Component.Should().Be(_imageListDesigner.Component);
    }

    [Fact]
    public void ColorDepth_Get_ShouldReturnColorDepth()
    {
        _actionList.ColorDepth.Should().Be(ColorDepth.Depth32Bit);
    }

    [Fact]
    public void ColorDepth_Set_ShouldUpdateColorDepth()
    {
        _actionList.ColorDepth = ColorDepth.Depth16Bit;
        _actionList.ColorDepth.Should().Be(ColorDepth.Depth16Bit);
    }

    [Fact]
    public void ImageSize_Get_ShouldReturnImageSize()
    {
        Size size = new(16, 16);
        _actionList.ImageSize.Should().Be(size);
    }

    [Fact]
    public void ImageSize_Set_ShouldUpdateImageSize()
    {
        Size size = new(32, 32);
        _actionList.ImageSize = size;
        _actionList.ImageSize.Should().Be(size);
    }

    [Fact]
    public void GetSortedActionItems_ShouldReturnExpectedItems()
    {
        DesignerActionItemCollection items = _actionList.GetSortedActionItems();
        items.Should().NotBeNull();
        items.Should().BeOfType<DesignerActionItemCollection>();

        items[0].Should().BeOfType<DesignerActionPropertyItem>();
        items[0].DisplayName.Should().Be(SR.ImageListActionList_ImageSizeDisplayName);
        items[0].Category.Should().Be(SR.PropertiesCategoryName);
        items[0].Description.Should().Be(SR.ImageListActionList_ImageSizeDescription);

        items[1].Should().BeOfType<DesignerActionPropertyItem>();
        items[1].DisplayName.Should().Be(SR.ImageListActionList_ColorDepthDisplayName);
        items[1].Category.Should().Be(SR.PropertiesCategoryName);
        items[1].Description.Should().Be(SR.ImageListActionList_ColorDepthDescription);

        items[2].Should().BeOfType<DesignerActionMethodItem>();
        items[2].DisplayName.Should().Be(SR.ImageListActionList_ChooseImagesDisplayName);
        items[2].Category.Should().Be(SR.LinksCategoryName);
        items[2].Description.Should().Be(SR.ImageListActionList_ChooseImagesDescription);
    }
}
