// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace System.ComponentModel.Design
{
    internal class VsPropertyGrid : PropertyGrid
    {
        private static readonly Size s_iCON_SIZE = new Size(16, 16);
        private static Size s_iconSize = s_iCON_SIZE;
        private static bool s_isScalingInitialized = false;

        public VsPropertyGrid(IServiceProvider serviceProvider) : base()
        {
        }

        protected override Bitmap SortByPropertyImage
        {
            get => GetBitmap("PBAlpha");
        }

        protected override Bitmap SortByCategoryImage
        {
            get => GetBitmap("PBCatego", true);
        }

        protected override Bitmap ShowPropertyPageImage
        {
            get => GetBitmap("PBPPage");
        }

        // try to find the best possible image
        private Bitmap GetBitmap(string resourceName, bool setMagentaTransparent = false)
        {
            // this resource might be present in System.Windows.Forms.VisualStudio.15.0.dll if this code is running on dev14 or newer
            Stream stream = BitmapSelector.GetResourceStream(typeof(PropertyGrid), resourceName + ".ico");
            Bitmap bitmap;
            if (stream != null)
            {
                if (!VsPropertyGrid.s_isScalingInitialized)
                {
                    if (DpiHelper.IsScalingRequired)
                    {
                        VsPropertyGrid.s_iconSize = DpiHelper.LogicalToDeviceUnits(s_iCON_SIZE);
                    }
                    VsPropertyGrid.s_isScalingInitialized = true;
                }
                // retrieve icon closest to the desired size
                Icon icon = new Icon(stream, VsPropertyGrid.s_iconSize);
                bitmap = icon.ToBitmap();
                icon.Dispose();
            }
            else
            {
                // this resource must be present in System.Windows.Forms.dll if it is not available in System.Windows.Forms.VisualStudio.15.0.dll
                bitmap = new Bitmap(BitmapSelector.GetResourceStream(typeof(PropertyGrid), resourceName + ".bmp"));
                if (setMagentaTransparent)
                {
                    bitmap.MakeTransparent(Color.Magenta);
                }
            }
            return bitmap;
        }
    }
}
