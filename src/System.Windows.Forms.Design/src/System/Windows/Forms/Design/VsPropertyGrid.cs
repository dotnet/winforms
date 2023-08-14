// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Design;

internal class VsPropertyGrid : PropertyGrid
{
    private static readonly Size ICON_SIZE = new(16, 16);
    private static Size IconSize = ICON_SIZE;
    private static bool IsScalingInitialized = false;

    public VsPropertyGrid(IServiceProvider serviceProvider) : base()
    {
    }

    protected override Bitmap SortByPropertyImage
    {
        get
        {
            return GetBitmap("PBAlpha");
        }
    }

    protected override Bitmap SortByCategoryImage
    {
        get
        {
            return GetBitmap("PBCategory", true);
        }
    }

    protected override Bitmap ShowPropertyPageImage
    {
        get
        {
            return GetBitmap("PBPPage");
        }
    }

    // try to find the best possible image
    private Bitmap GetBitmap(string resourceName, bool setMagentaTransparent = false)
    {
        Bitmap bitmap = null;
        // this resource might be present in System.Windows.Forms.VisualStudio.11.0.dll if this code is running on dev14 or newer
        Stream stream = BitmapSelector.GetResourceStream(typeof(PropertyGrid), resourceName);
        if (stream != null)
        {
            if (!IsScalingInitialized)
            {
                if (DpiHelper.IsScalingRequired)
                {
                    IconSize = DpiHelper.LogicalToDeviceUnits(ICON_SIZE);
                }

                IsScalingInitialized = true;
            }

            // retrieve icon closest to the desired size
            Icon icon = new Icon(stream, IconSize);
            bitmap = icon.ToBitmap();
            icon.Dispose();
        }
        else
        {
            // this resource must be present in System.Windows.Forms.dll if it is not available in System.Windows.Forms.VisualStudio.11.0.dll
            bitmap = new Bitmap(BitmapSelector.GetResourceStream(typeof(PropertyGrid), resourceName + ".bmp"));
            if (setMagentaTransparent)
            {
                bitmap.MakeTransparent(Color.Magenta);
            }
        }

        return bitmap;
    }
}
