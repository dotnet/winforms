// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Design.Tests;

public class ImageListDesignerOriginalImageCollectionTests : IDisposable
{
    private readonly ImageListDesigner _owner = new();

    private readonly ImageListDesigner.OriginalImageCollection _originalImageCollection;

    private readonly Image _image = new Bitmap(10, 10);

    public ImageListDesignerOriginalImageCollectionTests()
    {
        _owner.Initialize(new ImageList());
        _originalImageCollection = new(_owner);
    }

    public void Dispose()
    {
        _owner.Dispose();
        _image.Dispose();
    }

    [Fact]
    public void Ctor_InitializesProperties_Correctly()
    {
        _originalImageCollection.Count.Should().Be(0);
        _originalImageCollection.IsReadOnly.Should().BeFalse();
    }

    [Fact]
    public void Ctor_WithNullOwner_ThrowsNulLReferenceException()
    {
        Action action = () => new ImageListDesigner.OriginalImageCollection(null!);

        action.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void Indexer_Get_ThrowsArgumentOutOfRangeException_ForInvalidIndex()
    {
        _originalImageCollection.Add(new ImageListImage(_image));

        Action action = () => { var image = _originalImageCollection[1]; };

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Indexer_Set_ThrowsArgumentOutOfRangeException_ForInvalidIndex()
    {
        _originalImageCollection.Add(new ImageListImage(_image));

        Action action = () => _originalImageCollection[1] = new ImageListImage(_image);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("NewName", "NewName")]
    public void SetKeyName_ChangesKeyName_ToExpectedValue(string? input, string expected)
    {
        _originalImageCollection.Add(new ImageListImage(_image, "OldName"));

        _originalImageCollection.SetKeyName(0, input!);

        _originalImageCollection[0].Name.Should().Be(expected);
    }

    [Fact]
    public void AddRange_AddsImages_GivenAmountAtATime()
    {
        _originalImageCollection.AddRange([new(_image), new(_image), new(_image)]);
        _originalImageCollection.Count.Should().Be(3);
    }

    [Fact]
    public void Add_AddsImage_OneAtATime()
    {
        _originalImageCollection.Add(new(_image));
        _originalImageCollection.Add(new(_image));
        _originalImageCollection.Count.Should().Be(2);
    }

    [Fact]
    public void Clear_RemovesAllImages()
    {
        _originalImageCollection.AddRange([new(_image), new(_image)]);
        _originalImageCollection.Clear();
        _originalImageCollection.Count.Should().Be(0);
    }

    [Fact]
    public void Contains_ReturnsTrue_IfImageIsInCollection()
    {
        ImageListImage image = new ImageListImage(_image);
        _originalImageCollection.Add(image);
        _originalImageCollection.Contains(image).Should().BeTrue();
    }

    [Fact]
    public void GetEnumerator_ReturnsEnumerator()
    {
        ImageListImage image = new ImageListImage(_image);
        _originalImageCollection.Add(image);

        using var enumerator = (IDisposable)_originalImageCollection.GetEnumerator();

        enumerator.Should().NotBeNull();
    }

    [Fact]
    public void RemoveAt_RemovesImageAtSpecifiedIndex()
    {
        _originalImageCollection.Add(new ImageListImage(_image));

        _originalImageCollection.RemoveAt(0);

        _originalImageCollection.Count.Should().Be(0);
    }

    [Fact]
    public void Remove_RemovesImageListImage()
    {
        _originalImageCollection.AddRange([new(_image), new(_image), new(_image)]);
        _originalImageCollection.Remove(_originalImageCollection[0]);
        _originalImageCollection.Count.Should().Be(2);
    }

    [Theory]
    [BoolData]
    public void IndexOf_Returns_ProperIndex(bool isNull)
    {
        if (isNull)
        {
            _originalImageCollection.IndexOf(null).Should().Be(-1);
            return;
        }

        ImageListImage image = new(_image);
        _originalImageCollection.Add(image);

        _originalImageCollection.IndexOf(image).Should().Be(0);
    }
}
