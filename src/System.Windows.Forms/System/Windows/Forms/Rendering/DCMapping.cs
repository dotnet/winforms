// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

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
internal readonly struct DCMapping : IDisposable
{
    private readonly HDC _hdc;
    private readonly int _savedState;

    public unsafe DCMapping(HDC hdc, Rectangle bounds)
    {
        if (hdc.IsNull)
        {
            throw new ArgumentNullException(nameof(hdc));
        }

        _hdc = hdc;
        _savedState = PInvokeCore.SaveDC(hdc);

        // Retrieve the x-coordinates and y-coordinates of the viewport origin for the specified device context.
        Point viewportOrg = default;
        bool success = PInvokeCore.GetViewportOrgEx(hdc, &viewportOrg);
        Debug.Assert(success, "GetViewportOrgEx() failed.");

        // Create a new rectangular clipping region based off of the bounds specified,
        // shifted over by the x & y specified in the viewport origin.
        RegionScope clippingRegion = new(
            viewportOrg.X + bounds.Left,
            viewportOrg.Y + bounds.Top,
            viewportOrg.X + bounds.Right,
            viewportOrg.Y + bounds.Bottom);
        Debug.Assert(!clippingRegion.IsNull, "CreateRectRgn() failed.");

        try
        {
            RegionScope originalRegion = new(hdc);

            // Shift the viewpoint origin by coordinates specified in "bounds".
            success = PInvoke.SetViewportOrgEx(
                hdc,
                viewportOrg.X + bounds.Left,
                viewportOrg.Y + bounds.Top,
                lppt: null);
            Debug.Assert(success, "SetViewportOrgEx() failed.");

            GDI_REGION_TYPE originalRegionType;
            if (!originalRegion.IsNull)
            {
                // Get the original clipping region so we can determine its type (we'll check later if we've restored the region back properly.)
                RECT originalClipRect = default;
                originalRegionType = PInvoke.GetRgnBox(originalRegion, &originalClipRect);
                Debug.Assert(
                    originalRegionType != GDI_REGION_TYPE.RGN_ERROR,
                    "ERROR returned from SelectClipRgn while selecting the original clipping region..");

                if (originalRegionType == GDI_REGION_TYPE.SIMPLEREGION)
                {
                    // Find the intersection of our clipping region and the current clipping region (our parent's)

                    GDI_REGION_TYPE combineResult = PInvokeCore.CombineRgn(
                        clippingRegion,
                        clippingRegion,
                        originalRegion,
                        RGN_COMBINE_MODE.RGN_AND);

                    Debug.Assert(
                        combineResult is GDI_REGION_TYPE.SIMPLEREGION or GDI_REGION_TYPE.NULLREGION,
                        "SIMPLEREGION or NULLREGION expected.");
                }
            }
            else
            {
                // If there was no clipping region, then the result is a simple region.
                originalRegionType = GDI_REGION_TYPE.SIMPLEREGION;
            }

            // Select the new clipping region; make sure it's a SIMPLEREGION or NULLREGION
            GDI_REGION_TYPE selectResult = PInvokeCore.SelectClipRgn(hdc, clippingRegion);
            Debug.Assert(
                selectResult is GDI_REGION_TYPE.SIMPLEREGION or GDI_REGION_TYPE.NULLREGION,
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
            PInvokeCore.RestoreDC(_hdc, _savedState);
        }
    }
}
