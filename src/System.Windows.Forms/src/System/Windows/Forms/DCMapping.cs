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
    ///  using(DCMapping mapping = new DCMapping(hDC, new Rectangle(10,10, 50, 50) {
    ///  // inside here the hDC's mapping of (0,0) is inset by (10,10) and
    ///  // all painting is clipped at (0,0) - (50,50)
    ///  }
    ///
    ///  To use with GDI+ you can get the hDC from the Graphics object. You'd want to do this in a situation where
    ///  you're handing off a graphics object to someone, and you want the world translated some amount X,Y. This
    ///  works better than g.TranslateTransform(x,y) - as if someone calls g.GetHdc and does a GDI operation - their
    ///  world is NOT transformed.
    ///
    ///  HandleRef hDC = new HandleRef(this, originalGraphics.GetHdc());
    ///  try {
    ///  using(DCMapping mapping = new DCMapping(hDC, new Rectangle(10,10, 50, 50) {
    ///
    ///  // DO NOT ATTEMPT TO USE originalGraphics here - you'll get an Object Busy error
    ///  // rather ask the mapping object for a graphics object.
    ///  mapping.Graphics.DrawRectangle(Pens.Black, rect);
    ///  }
    ///  }
    ///  finally { g.ReleaseHdc(hDC);}
    ///
    ///  PERF: DCMapping is a structure so that it will allocate on the stack rather than in GC managed
    ///  memory. This way disposing the object does not force a GC. Since DCMapping objects aren't
    ///  likely to be passed between functions rather used and disposed in the same one, this reduces
    ///  overhead.
    /// </summary>
    internal struct DCMapping : IDisposable
    {
        private DeviceContext _dc;
        private Graphics _graphics;
        private Rectangle _translatedBounds;

        public unsafe DCMapping(IntPtr hDC, Rectangle bounds)
        {
            if (hDC == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(hDC));
            }

            bool success;

            _translatedBounds = bounds;
            _graphics = null;
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
                // Create an empty region oriented at 0,0 so we can populate it with the original clipping region of the hDC passed in.
                var hOriginalClippingRegion = new Gdi32.RegionScope(0, 0, 0, 0);
                Debug.Assert(!hOriginalClippingRegion.IsNull, "CreateRectRgn() failed.");

                // Get the clipping region from the hDC: result = {-1 = error, 0 = no region, 1 = success} per MSDN
                int result = Gdi32.GetClipRgn(hDC, hOriginalClippingRegion);
                Debug.Assert(result != -1, "GetClipRgn() failed.");

                // Shift the viewpoint origint by coordinates specified in "bounds".
                var lastViewPort = new Point();
                success = Gdi32.SetViewportOrgEx(hDC, viewportOrg.X + bounds.Left, viewportOrg.Y + bounds.Top, &lastViewPort).IsTrue();
                Debug.Assert(success, "SetViewportOrgEx() failed.");

                RegionType originalRegionType;
                if (result != 0)
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
                RegionType selectResult = Gdi32.SelectClipRgn(hDC, hClippingRegion);
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
            if (_graphics != null)
            {
                // Reset GDI+ if used.
                // we need to dispose the graphics object first, as it will do
                // some restoration to the ViewPort and ClipRectangle to restore the hDC to
                // the same state it was created in
                _graphics.Dispose();
                _graphics = null;
            }

            if (_dc != null)
            {
                // Now properly reset GDI.
                _dc.RestoreHdc();
                _dc.Dispose();
                _dc = null;
            }
        }

        /// <summary>
        ///  Allows you to get the graphics object based off of the translated HDC.
        ///  Note this will be disposed when the DCMapping object is disposed.
        /// </summary>
        public Graphics Graphics
        {
            get
            {
                Debug.Assert(_dc != null, "unexpected null dc!");

                if (_graphics == null)
                {
                    _graphics = Graphics.FromHdcInternal(_dc.Hdc);
                    _graphics.SetClip(new Rectangle(Point.Empty, _translatedBounds.Size));
                }

                return _graphics;
            }
        }
    }
}
