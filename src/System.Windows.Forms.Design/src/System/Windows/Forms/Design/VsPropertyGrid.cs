// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Design;

internal class VsPropertyGrid : PropertyGrid
{
    private static readonly Size s_defaultIconSize = new(16, 16);

    public VsPropertyGrid() : base()
    {
    }

    protected override Bitmap SortByPropertyImage => GetBitmap("PBAlpha");

    protected override Bitmap SortByCategoryImage => GetBitmap("PBCategory");

    protected override Bitmap ShowPropertyPageImage => GetBitmap("PBPPage");

    private static Bitmap GetBitmap(string resourceName)
        => ScaleHelper.GetIconResourceAsBestMatchBitmap(
            BitmapSelector.GetResourceStream(typeof(PropertyGrid), resourceName) ?? throw new InvalidOperationException(),
            ScaleHelper.ScaleToDpi(s_defaultIconSize, ScaleHelper.InitialSystemDpi));
}
