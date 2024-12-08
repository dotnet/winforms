// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  Drawing helpers for device contexts. All methods restore original state unless otherwise specified.
/// </summary>
internal static partial class DeviceContextExtensions
{
    internal static void DrawRectangle(this DeviceContextHdcScope hdc, Rectangle rectangle, HPEN hpen) =>
        DrawRectangle(hdc.HDC, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, hpen);

    internal static void DrawRectangle(
        this DeviceContextHdcScope hdc,
        int left,
        int top,
        int right,
        int bottom,
        HPEN hpen) =>
        DrawRectangle(hdc.HDC, left, top, right, bottom, hpen);

    internal static void DrawRectangle(
        this HDC hdc,
        int left,
        int top,
        int right,
        int bottom,
        HPEN hpen)
    {
        using SelectObjectScope penScope = new(hdc, hpen);
        using SetRop2Scope ropScope = new(hdc, R2_MODE.R2_COPYPEN);
        using SelectObjectScope brushScope = new(hdc, PInvokeCore.GetStockObject(GET_STOCK_OBJECT_FLAGS.NULL_BRUSH));

        PInvoke.Rectangle(hdc, left, top, right, bottom);
    }

    internal static void FillRectangle(this DeviceContextHdcScope hdc, Rectangle rectangle, HBRUSH hbrush) =>
        FillRectangle(hdc.HDC, rectangle, hbrush);

    internal static void FillRectangle(this HDC hdc, Rectangle rectangle, HBRUSH hbrush)
    {
        Debug.Assert(!hbrush.IsNull);
        RECT rect = rectangle;
        PInvoke.FillRect(
            hdc,
            rect,
            hbrush);
    }

    internal static void DrawLine(this DeviceContextHdcScope hdc, HPEN hpen, Point p1, Point p2) =>
        DrawLine(hdc.HDC, hpen, p1.X, p1.Y, p2.X, p2.Y);

    internal static void DrawLine(this HDC hdc, HPEN hpen, Point p1, Point p2) =>
        DrawLine(hdc, hpen, p1.X, p1.Y, p2.X, p2.Y);

    internal static void DrawLine(this DeviceContextHdcScope hdc, HPEN hpen, int x1, int y1, int x2, int y2) =>
        DrawLine(hdc.HDC, hpen, x1, y1, x2, y2);

    internal static void DrawLine(this HDC hdc, HPEN hpen, int x1, int y1, int x2, int y2) =>
        DrawLines(hdc, hpen, [x1, y1, x2, y2]);

    /// <summary>
    ///  Draws lines with the <paramref name="hpen"/> using points defined in <paramref name="lines"/>.
    /// </summary>
    /// <param name="lines">
    ///  MUST be a multiple of 4. Each group of 4 represents x1, y1, x2, y2.
    /// </param>
    internal static void DrawLines(this DeviceContextHdcScope hdc, HPEN hpen, ReadOnlySpan<int> lines) =>
        DrawLines(hdc.HDC, hpen, lines);

    /// <summary>
    ///  Draws lines with the <paramref name="hpen"/> using points defined in <paramref name="lines"/>.
    /// </summary>
    /// <param name="lines">
    ///  MUST be a multiple of 4. Each group of 4 represents x1, y1, x2, y2.
    /// </param>
    internal static unsafe void DrawLines(this HDC hdc, HPEN hpen, ReadOnlySpan<int> lines)
    {
        Debug.Assert((lines.Length % 4) == 0);

        using SetRop2Scope ropScope = new(hdc, R2_MODE.R2_COPYPEN);
        using SetBkModeScope bkScope = new(hdc, BACKGROUND_MODE.TRANSPARENT);
        using SelectObjectScope selection = new(hdc, (HGDIOBJ)hpen.Value);

        Point oldPoint = default;

        for (int i = 0; i < lines.Length; i += 4)
        {
            PInvoke.MoveToEx(hdc, lines[i], lines[i + 1], &oldPoint);
            PInvoke.LineTo(hdc, lines[i + 2], lines[i + 3]);
            PInvoke.MoveToEx(hdc, oldPoint.X, oldPoint.Y, lppt: null);
        }
    }

    internal static Color FindNearestColor(this DeviceContextHdcScope hdc, Color color) => FindNearestColor(hdc.HDC, color);

    /// <summary>
    ///  Calls <see cref="PInvoke.GetNearestColor(HDC, COLORREF)"/> to get the nearest color for the given
    ///  <paramref name="color"/>. Returns the original color if the color didn't actually change, retaining
    ///  the state of the color.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is important as the color only changes if <paramref name="hdc"/> is a very low color depth. This
    ///   is extremely rare for the normal case of HDC backed Graphics objects. Keeping the original color keeps the
    ///   state that would otherwise be stripped, notably things like <see cref="Color.IsKnownColor"/> which allows
    ///   us to later pull from a the various caches that <see cref="Drawing"/> maintains (saving allocations).
    ///  </para>
    ///  <para>
    ///   Ideally we'd drop checking at all and just support full color drawing to improve performance for the
    ///   expected normal case (more than 8 BITSPIXEL for the HDC).
    ///  </para>
    /// </remarks>
    internal static Color FindNearestColor(this HDC hdc, Color color)
    {
        Color newColor = ColorTranslator.FromWin32((int)PInvoke.GetNearestColor(hdc, (COLORREF)(uint)ColorTranslator.ToWin32(color)).Value);
        return newColor.ToArgb() == color.ToArgb() ? color : newColor;
    }

    internal static Graphics CreateGraphics(this HDC hdc) => Graphics.FromHdcInternal(hdc);
    internal static Graphics CreateGraphics(this CreateDcScope hdc) => Graphics.FromHdcInternal(hdc.HDC);
    internal static Graphics CreateGraphics(this GetDcScope hdc) => Graphics.FromHdcInternal(hdc.HDC);

    internal static void DrawAndFillEllipse(
        this DeviceContextHdcScope hdc,
        HPEN pen,
        HBRUSH brush,
        Rectangle bounds) =>
        DrawAndFillEllipse(hdc.HDC, pen, brush, bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);

    internal static unsafe void DrawAndFillEllipse(
        this HDC hdc,
        HPEN pen,
        HBRUSH brush,
        int left,
        int top,
        int right,
        int bottom)
    {
        using var penSelection = pen.IsNull ? default : new SelectObjectScope(hdc, (HGDIOBJ)pen.Value);
        using var brushSelection = brush.IsNull ? default : new SelectObjectScope(hdc, (HGDIOBJ)brush.Value);

        PInvoke.Ellipse(hdc, left, top, right, bottom);
    }

    internal static void FillRectangle(this GetDcScope hdc, HBRUSH hbrush, Rectangle rectangle) =>
        FillRectangle(hdc.HDC, hbrush, rectangle);

    internal static void FillRectangle(this HDC hdc, HBRUSH hbrush, Rectangle rectangle)
    {
        Debug.Assert(!hbrush.IsNull, "HBRUSH is null");
        RECT rect = rectangle;
        PInvoke.FillRect(
            hdc,
            rect,
            hbrush);
    }

    /// <summary>
    ///  Convert the <paramref name="deviceContext"/> into a <see cref="Graphics"/> object if possible.
    /// </summary>
    /// <param name="create">
    ///  Will create the <see cref="Graphics"/> if possible and it is not already created.
    /// </param>
    /// <remarks>
    ///  <para>
    ///   Do NOT dispose of the <see cref="Graphics"/> object. If it was created, the object creating it owns it.
    ///   </para>
    /// </remarks>
    internal static Graphics? TryGetGraphics(this IDeviceContext deviceContext, bool create = false) => deviceContext switch
    {
        Graphics graphics => graphics,
        IGraphicsHdcProvider provider => (Graphics?)provider.GetGraphics(create),
        _ => AssertNoGraphics(create)
    };

    internal static DeviceContextHdcScope ToHdcScope(
        this IDeviceContext deviceContext,
        ApplyGraphicsProperties applyProperties = ApplyGraphicsProperties.All) => deviceContext switch
        {
            IHdcContext hdc => new(hdc, applyProperties),
            _ => new(new IDeviceContextAdapter(deviceContext), applyProperties)
        };

    private static Graphics? AssertNoGraphics(bool create)
    {
        Debug.Assert(!create, "Couldn't get Graphics");
        return null;
    }
}
