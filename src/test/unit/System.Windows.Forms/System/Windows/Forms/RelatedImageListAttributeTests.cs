// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Controls.Tests;

public class RelatedImageListAttributeTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("MainImageList")]
    public void RelatedImageList_ReturnsConstructorValue(string? value) =>
        new RelatedImageListAttribute(value).RelatedImageList.Should().Be(value);
}
