// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Design.Tests;

public class VsPropertyGridTests
{
    [WinFormsFact]
    public void ValidateIcons()
    {
        using TestPropertyGrid propertyGrid = new();
        using Bitmap sortPropertyImage = propertyGrid.GetSortByPropertyImage();
        sortPropertyImage.Should().NotBeNull();
        using Bitmap sortCategoryImage = propertyGrid.GetSortByCategoryImage();
        sortCategoryImage.Should().NotBeNull();
        using Bitmap showPageImage = propertyGrid.GetShowPropertyPageImage();
        showPageImage.Should().NotBeNull();
    }

    private class TestPropertyGrid : VsPropertyGrid
    {
        public Bitmap GetSortByPropertyImage() => SortByPropertyImage;
        public Bitmap GetSortByCategoryImage() => SortByCategoryImage;
        public Bitmap GetShowPropertyPageImage() => ShowPropertyPageImage;
    }
}
