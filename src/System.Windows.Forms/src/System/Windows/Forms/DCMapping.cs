// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms;

/// <summary>
///  DCMapping is used to change the mapping and clip region of the specified device context to the given
///  bounds. When the DCMapping is disposed, the original mapping and clip rectangle are restored.
///
///  Example:
///
///  using(DCMapping mapping = new DCMapping(hDC, new Rectangle(10,10, 50, 50)
///  {
///      // inside here the hDC's mapping of (0,0) is inset by (10,10) and
///      // all painting is clipped at (0,0) - (50,50)
///  }
///
///  PERF: DCMapping is a structure so that it will allocate on the stack rather than in GC managed memory. This
///  way disposing the object does not force a GC. Since DCMapping objects aren't likely to be passed between
///  functions rather used and disposed in the same one, this reduces overhead.
/// </summary>
internal struct DCMapping : IDisposable
{
    private HDC _hdc;
    private int _savedState;

    public unsafe DCMapping(HDC hdc, Rectangle bounds)
    {
        ArgumentNullException.ThrowIfNull(hdc);

        _hdc = hdc;
        _savedState = PInvoke.SaveDC(hdc);

        // Retrieve the x-coordinates and y-coordinates of the viewport origin for the specified device context.
        Point viewportOrg = default;
        bool success = PInvoke.GetViewportOrgEx(hdc, &viewportOrg);
        Debug.Assert(success, "GetViewportOrgEx() failed.");

        // Create a new rectangular clipping region based off of the bounds specified, shifted over by the x & y specified in the viewport origin.
        PInvoke.RegionScope hClippingRegion = new(
            viewportOrg.X + bounds.Left,
            viewportOrg.Y + bounds.Top,
            viewportOrg.X + bounds.Right,
            viewportOrg.Y + bounds.Bottom);
        Debug.Assert(!hClippingRegion.IsNull, "CreateRectRgn() failed.");

        try
        {
            PInvoke.RegionScope hOriginalClippingRegion = new(hdc);

            // Shift the viewpoint origin by coordinates specified in "bounds".
            success = PInvoke.SetViewportOrgEx(
                hdc,
                viewportOrg.X + bounds.Left,
                viewportOrg.Y + bounds.Top,
                lppt: null);
            Debug.Assert(success, "SetViewportOrgEx() failed.");

            RegionType originalRegionType;
            if (!hOriginalClippingRegion.IsNull)
            {
                // Get the original clipping region so we can determine its type (we'll check later if we've restored the region back properly.)
                RECT originalClipRect = default;
                originalRegionType = (RegionType)PInvoke.GetRgnBox(hOriginalClippingRegion, &originalClipRect);
                Debug.Assert(
                    originalRegionType != RegionType.ERROR,
                    "ERROR returned from SelectClipRgn while selecting the original clipping region..");

                if (originalRegionType == RegionType.SIMPLEREGION)
                {
                    // Find the intersection of our clipping region and the current clipping region (our parent's)

                    RegionType combineResult = (RegionType)PInvoke.CombineRgn(
                        hClippingRegion,
                        hClippingRegion,
                        hOriginalClippingRegion,
                        RGN_COMBINE_MODE.RGN_AND);

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
            RegionType selectResult = (RegionType)PInvoke.SelectClipRgn(hdc, hClippingRegion);
            Debug.Assert(
                selectResult == RegionType.SIMPLEREGION || selectResult == RegionType.NULLREGION,
                "SIMPLEREGION or NULLLREGION expected.");
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
        }
    }

    public void Dispose()
    {
        if (!_hdc.IsNull)
        {
            PInvoke.RestoreDC(_hdc, _savedState);
        }
    }
}
