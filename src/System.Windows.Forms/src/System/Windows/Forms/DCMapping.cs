// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms.Internal;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  DCMapping is used to change the mapping and clip region of the
    ///  the specified device context to the given bounds. When the
    ///  DCMapping is disposed, the original mapping and clip rectangle
    ///  are restored.
    ///
    ///  Example:
    ///
    ///  using(DCMapping mapping = new DCMapping(hDC, new Rectangle(10,10, 50, 50)
    ///  {
    ///      // inside here the hDC's mapping of (0,0) is inset by (10,10) and
    ///      // all painting is clipped at (0,0) - (50,50)
    ///  }
    ///
    ///  PERF: DCMapping is a structure so that it will allocate on the stack rather than in GC managed
    ///  memory. This way disposing the object does not force a GC. Since DCMapping objects aren't
    ///  likely to be passed between functions rather used and disposed in the same one, this reduces
    ///  overhead.
    /// </summary>
    internal struct DCMapping : IDisposable
    {
        private DeviceContext _dc;

        public unsafe DCMapping(Gdi32.HDC hDC, Rectangle bounds)
        {
            if (hDC.IsNull)
            {
                throw new ArgumentNullException(nameof(hDC));
            }

            bool success;

            _dc = DeviceContext.FromHdc(hDC);
            _dc.SaveHdc();

            // Retrieve the x-coordinates and y-coordinates of the viewport origin for the specified device context.
            success = Gdi32.GetViewportOrgEx(hDC, out Point viewportOrg).IsTrue();
            Debug.Assert(success, "GetViewportOrgEx() failed.");

            // Create a new rectangular clipping region based off of the bounds specified, shifted over by the x & y specified in the viewport origin.
            var hClippingRegion = new Gdi32.RegionScope(
                viewportOrg.X + bounds.Left,
                viewportOrg.Y + bounds.Top,
                viewportOrg.X + bounds.Right,
                viewportOrg.Y + bounds.Bottom);
            Debug.Assert(!hClippingRegion.IsNull, "CreateRectRgn() failed.");

            try
            {
                var hOriginalClippingRegion = new Gdi32.RegionScope(hDC);

                // Shift the viewpoint origint by coordinates specified in "bounds".
                var lastViewPort = new Point();
                success = Gdi32.SetViewportOrgEx(hDC, viewportOrg.X + bounds.Left, viewportOrg.Y + bounds.Top, &lastViewPort).IsTrue();
                Debug.Assert(success, "SetViewportOrgEx() failed.");

                RegionType originalRegionType;
                if (!hOriginalClippingRegion.IsNull)
                {
                    // Get the origninal clipping region so we can determine its type (we'll check later if we've restored the region back properly.)
                    RECT originalClipRect = new RECT();
                    originalRegionType = Gdi32.GetRgnBox(hOriginalClippingRegion, ref originalClipRect);
                    Debug.Assert(
                        originalRegionType != RegionType.ERROR,
                        "ERROR returned from SelectClipRgn while selecting the original clipping region..");

                    if (originalRegionType == RegionType.SIMPLEREGION)
                    {
                        // Find the intersection of our clipping region and the current clipping region (our parent's)

                        RegionType combineResult = Gdi32.CombineRgn(
                            hClippingRegion,
                            hClippingRegion,
                            hOriginalClippingRegion,
                            Gdi32.CombineMode.RGN_AND);

                        Debug.Assert(
                            (combineResult == RegionType.SIMPLEREGION) || (combineResult == RegionType.NULLREGION),
                            "SIMPLEREGION or NULLREGION expected.");
                    }
                }
                else
                {
                    // If there was no clipping region, then the result is a simple region.
                    originalRegionType = RegionType.SIMPLEREGION;
                }

                // Select the new clipping region; make sure it's a SIMPLEREGION or NULLREGION
                RegionType selectResult = Gdi32.SelectClipRgn((Gdi32.HDC)hDC, hClippingRegion);
                Debug.Assert(
                    selectResult == RegionType.SIMPLEREGION || selectResult == RegionType.NULLREGION,
                    "SIMPLEREGION or NULLLREGION expected.");
            }
            catch (Exception ex) when (!ClientUtils.IsCriticalException(ex))
            {
                _dc.RestoreHdc();
                _dc.Dispose();
            }
        }

        public void Dispose()
        {
            if (_dc != null)
            {
                // Now properly reset GDI.
                _dc.RestoreHdc();
                _dc.Dispose();
                _dc = null;
            }
        }
    }
}
