// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Private.Windows.Core.OLE;
using Windows.Win32.System.Com;

namespace System.Windows.Forms;

internal partial class WinFormsDataObject
{
    internal unsafe partial class Composition
    {
        private class DesktopToNativeAdapter : DesktopDataObject.Composition.DesktopToNativeAdapter
        {
            public DesktopToNativeAdapter(IDataObjectDesktop dataObject) : base(dataObject)
            {
            }

            public override DesktopDataObject.Composition.BinaryFormatUtilities GetBinaryFormatUtilities() => BinaryFormatUtilitiesInstance;

            public override bool IsDataImage(object data) => data is Image;

            public override HRESULT TryGetCompatibleBitmap(string format, object data, STGMEDIUM* pMedium)
            {
                // could use assembly load to load the Bitmap assembly but seems inefficient...
                if (format.Equals(DesktopDataFormats.BitmapConstant) && data is Bitmap bitmap)
                {
                    using var screenDC = GetDcScope.ScreenDC;

                    // GDI+ returns a DIBSECTION based HBITMAP. The clipboard only deals well with bitmaps created using
                    // CreateCompatibleBitmap(). So, we convert the DIBSECTION into a compatible bitmap.
                    HBITMAP hbitmap = bitmap.GetHBITMAP();

                    // Create a compatible DC to render the source bitmap.
                    using CreateDcScope sourceDC = new(screenDC);
                    using SelectObjectScope sourceBitmapSelection = new(sourceDC, hbitmap);

                    // Create a compatible DC and a new compatible bitmap.
                    using CreateDcScope destinationDC = new(screenDC);
                    HBITMAP compatibleBitmap = PInvokeCore.CreateCompatibleBitmap(screenDC, bitmap.Size.Width, bitmap.Size.Height);

                    // Select the new bitmap into a compatible DC and render the blt the original bitmap.
                    using SelectObjectScope destinationBitmapSelection = new(destinationDC, compatibleBitmap);
                    PInvokeCore.BitBlt(
                        destinationDC,
                        0,
                        0,
                        bitmap.Size.Width,
                        bitmap.Size.Height,
                        sourceDC,
                        0,
                        0,
                        ROP_CODE.SRCCOPY);

                    // Save bitmap
                    pMedium->u.hBitmap = compatibleBitmap;
                    return HRESULT.S_OK;
                }

                return HRESULT.DV_E_TYMED;
            }
        }
    }
}
