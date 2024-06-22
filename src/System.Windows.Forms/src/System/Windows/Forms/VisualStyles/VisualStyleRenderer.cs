// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Interop;
using Microsoft.Win32;

namespace System.Windows.Forms.VisualStyles;

/// <summary>
///  This class provides full feature parity with UxTheme API.
/// </summary>
public sealed partial class VisualStyleRenderer : IHandle<HTHEME>
{
    private HRESULT _lastHResult;
    private const int NumberOfPossibleClasses = VisualStyleElement.Count; // used as size for themeHandles

    [ThreadStatic]
    private static Dictionary<string, ThemeHandle>? t_themeHandles; // per-thread cache of ThemeHandle objects.

    [ThreadStatic]
    private static long t_threadCacheVersion;

    private static long s_globalCacheVersion;

    static VisualStyleRenderer()
    {
        SystemEvents.UserPreferenceChanging += new UserPreferenceChangingEventHandler(OnUserPreferenceChanging);
    }

    /// <summary>
    ///  Check if visual styles is supported for client area.
    /// </summary>
    private static bool AreClientAreaVisualStylesSupported
    {
        get
        {
            return (VisualStyleInformation.IsEnabledByUser &&
               ((Application.VisualStyleState & VisualStyleState.ClientAreaEnabled) == VisualStyleState.ClientAreaEnabled));
        }
    }

    /// <summary>
    ///  Returns true if visual styles are 1) supported by the OS 2) enabled in the client area
    ///  and 3) currently applied to this application. Otherwise, it returns false. Note that
    ///  if it returns false, attempting to instantiate/use objects of this class
    ///  will result in exceptions being thrown.
    /// </summary>
    public static bool IsSupported
    {
        get
        {
            bool supported = AreClientAreaVisualStylesSupported;

            if (supported)
            {
                // In some cases, this check isn't enough, since the theme handle creation
                // could fail for some other reason. Try creating a theme handle here - if successful, return true,
                // else return false.
                IntPtr hTheme = GetHandle("BUTTON", false); // Button is an arbitrary choice.
                supported = hTheme != IntPtr.Zero;
            }

            return supported;
        }
    }

    /// <summary>
    ///  Returns true if the element is defined by the current visual style, else false.
    ///  Note:
    ///  1) Throws an exception if IsSupported is false, since it is illegal to call it in that case.
    ///  2) The underlying API does not validate states. So if you pass in invalid state values,
    ///   we might still return true. When you use an invalid state to render, you get the default
    ///   state instead.
    /// </summary>
    public static bool IsElementDefined(VisualStyleElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return IsCombinationDefined(element.ClassName, element.Part);
    }

    internal static bool IsCombinationDefined(string className, int part)
    {
        bool result = false;

        if (!IsSupported)
        {
            throw new InvalidOperationException(VisualStyleInformation.IsEnabledByUser
                ? SR.VisualStylesDisabledInClientArea
                : SR.VisualStyleNotActive);
        }

        HTHEME hTheme = GetHandle(className, false);

        if (!hTheme.IsNull)
        {
            // IsThemePartDefined doesn't work for part = 0, although there are valid parts numbered 0. We
            // allow these explicitly here.
            result = part == 0 || (bool)PInvoke.IsThemePartDefined(hTheme, part, 0);
        }

        // If the combo isn't defined, check the validity of our theme handle cache.
        if (!result)
        {
            using PInvoke.OpenThemeDataScope handle = new(HWND.Null, className);

            if (!handle.IsNull)
            {
                result = PInvoke.IsThemePartDefined(handle, part, 0);
            }

            // If we did, in fact get a new correct theme handle, our cache is out of date -- update it now.
            if (result)
            {
                RefreshCache();
            }
        }

        return result;
    }

    /// <summary>
    ///  Constructor takes a VisualStyleElement.
    /// </summary>
    public VisualStyleRenderer(VisualStyleElement element) : this(element.ClassName, element.Part, element.State)
    {
    }

    /// <summary>
    ///  Constructor takes weakly typed parameters - left for extensibility (using classes, parts or states
    ///  not defined in the VisualStyleElement class.)
    /// </summary>
    public VisualStyleRenderer(string className, int part, int state)
    {
        ArgumentNullException.ThrowIfNull(className);

        if (!IsCombinationDefined(className, part))
            throw new ArgumentException(SR.VisualStylesInvalidCombination);

        Class = className;
        Part = part;
        State = state;
    }

    /// <summary>
    ///  Returns the current _class. Use SetParameters to set.
    /// </summary>
    public string Class { get; private set; }

    /// <summary>
    ///  Returns the current part. Use SetParameters to set.
    /// </summary>
    public int Part { get; private set; }

    /// <summary>
    ///  Returns the current state. Use SetParameters to set.
    /// </summary>
    public int State { get; private set; }

    /// <summary>
    ///  Returns the underlying HTheme handle.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   NOTE: The handle gets invalidated when the theme changes or the user disables theming. When that
    ///   happens, the user should requery this property to get the correct handle. To know when the
    ///   theme changed, hook on to SystemEvents.UserPreferenceChanged and look for ThemeChanged.
    ///   category.
    /// </para>
    /// </remarks>
    public IntPtr Handle
        => !IsSupported
            ? throw new InvalidOperationException(VisualStyleInformation.IsEnabledByUser
                ? SR.VisualStylesDisabledInClientArea
                : SR.VisualStyleNotActive)
            : (nint)GetHandle(Class);

    HTHEME IHandle<HTHEME>.Handle => (HTHEME)Handle;

    internal HTHEME HTHEME => (HTHEME)Handle;

    /// <summary>
    ///  Used to set a new VisualStyleElement on this VisualStyleRenderer instance.
    /// </summary>
    public void SetParameters(VisualStyleElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        SetParameters(element.ClassName, element.Part, element.State);
    }

    /// <summary>
    ///  Used to set the _class, part and state that the VisualStyleRenderer object references.
    ///  These parameters cannot be set individually.
    ///  This method is present for extensibility.
    /// </summary>
    public void SetParameters(string className, int part, int state)
    {
        if (!IsCombinationDefined(className, part))
            throw new ArgumentException(SR.VisualStylesInvalidCombination);

        Class = className;
        Part = part;
        State = state;
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public void DrawBackground(IDeviceContext dc, Rectangle bounds)
    {
        ArgumentNullException.ThrowIfNull(dc);

        using DeviceContextHdcScope hdc = dc.ToHdcScope();
        DrawBackground(hdc, bounds, HWND.Null);
    }

    internal unsafe void DrawBackground(HDC dc, Rectangle bounds, HWND hwnd = default)
    {
        if (bounds.Width < 0 || bounds.Height < 0)
        {
            return;
        }

        if (!hwnd.IsNull)
        {
            using var htheme = OpenThemeData(hwnd, Class);
            _lastHResult = PInvoke.DrawThemeBackground(htheme, dc, Part, State, bounds, null);
        }
        else
        {
            _lastHResult = PInvoke.DrawThemeBackground(HTHEME, dc, Part, State, bounds, null);
        }
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public void DrawBackground(IDeviceContext dc, Rectangle bounds, Rectangle clipRectangle)
    {
        ArgumentNullException.ThrowIfNull(dc);

        using DeviceContextHdcScope hdc = dc.ToHdcScope();
        DrawBackground(hdc, bounds, clipRectangle, HWND.Null);
    }

    internal unsafe void DrawBackground(HDC dc, Rectangle bounds, Rectangle clipRectangle, HWND hwnd)
    {
        if (bounds.Width < 0 || bounds.Height < 0 || clipRectangle.Width < 0 || clipRectangle.Height < 0)
            return;

        if (!hwnd.IsNull)
        {
            using var htheme = OpenThemeData(hwnd, Class);
            _lastHResult = PInvoke.DrawThemeBackground(htheme, dc, Part, State, bounds, clipRectangle);
        }
        else
        {
            _lastHResult = PInvoke.DrawThemeBackground(HTHEME, dc, Part, State, bounds, clipRectangle);
        }
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public Rectangle DrawEdge(IDeviceContext dc, Rectangle bounds, Edges edges, EdgeStyle style, EdgeEffects effects)
    {
        ArgumentNullException.ThrowIfNull(dc);

        using DeviceContextHdcScope hdc = dc.ToHdcScope();
        return DrawEdge(hdc, bounds, edges, style, effects);
    }

    internal unsafe Rectangle DrawEdge(HDC dc, Rectangle bounds, Edges edges, EdgeStyle style, EdgeEffects effects)
    {
        SourceGenerated.EnumValidator.Validate(edges, nameof(edges));
        SourceGenerated.EnumValidator.Validate(style, nameof(style));
        SourceGenerated.EnumValidator.Validate(effects, nameof(effects));

        RECT contentRect;
        _lastHResult = PInvoke.DrawThemeEdge(
            HTHEME,
            dc,
            Part,
            State,
            bounds,
            (DRAWEDGE_FLAGS)style,
            (DRAW_EDGE_FLAGS)edges | (DRAW_EDGE_FLAGS)effects | DRAW_EDGE_FLAGS.BF_ADJUST,
            &contentRect);

        return contentRect;
    }

    /// <summary>
    ///  [See win32 equivalent.]
    ///  This method uses Graphics.DrawImage as a backup if themed drawing does not work.
    /// </summary>
    public void DrawImage(Graphics g, Rectangle bounds, Image image)
    {
        ArgumentNullException.ThrowIfNull(g);
        ArgumentNullException.ThrowIfNull(image);

        if (bounds.Width < 0 || bounds.Height < 0)
            return;

        g.DrawImage(image, bounds);
    }

    /// <summary>
    ///  [See win32 equivalent.]
    ///  This method uses Graphics.DrawImage as a backup if themed drawing does not work.
    /// </summary>
    public void DrawImage(Graphics g, Rectangle bounds, ImageList imageList, int imageIndex)
    {
        ArgumentNullException.ThrowIfNull(g);
        ArgumentNullException.ThrowIfNull(imageList);

        ArgumentOutOfRangeException.ThrowIfNegative(imageIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(imageIndex, imageList.Images.Count);

        if (bounds.Width < 0 || bounds.Height < 0)
            return;

        // DrawThemeIcon currently seems to do nothing, but still return S_OK. As a workaround,
        // we call DrawImage on the graphics object itself for now.
        g.DrawImage(imageList.Images[imageIndex], bounds);
    }

    /// <summary>
    ///  Given a graphics object and bounds to draw in, this method effectively asks the passed in
    ///  control's parent to draw itself in there (it sends WM_ERASEBKGND &amp; WM_PRINTCLIENT messages
    ///  to the parent).
    /// </summary>
    public void DrawParentBackground(IDeviceContext dc, Rectangle bounds, Control childControl)
    {
        ArgumentNullException.ThrowIfNull(dc);
        ArgumentNullException.ThrowIfNull(childControl);

        if (bounds.Width < 0 || bounds.Height < 0)
        {
            return;
        }

        if (childControl.IsHandleCreated)
        {
            using DeviceContextHdcScope hdc = dc.ToHdcScope();
            _lastHResult = PInvoke.DrawThemeParentBackground(childControl.HWND, hdc, bounds);
        }
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public void DrawText(IDeviceContext dc, Rectangle bounds, string? textToDraw)
    {
        DrawText(dc, bounds, textToDraw, false);
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public void DrawText(IDeviceContext dc, Rectangle bounds, string? textToDraw, bool drawDisabled)
    {
        DrawText(dc, bounds, textToDraw, drawDisabled, TextFormatFlags.HorizontalCenter);
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public void DrawText(IDeviceContext dc, Rectangle bounds, string? textToDraw, bool drawDisabled, TextFormatFlags flags)
    {
        ArgumentNullException.ThrowIfNull(dc);

        using DeviceContextHdcScope hdc = dc.ToHdcScope();
        DrawText(hdc, bounds, textToDraw, drawDisabled, flags);
    }

    internal void DrawText(HDC dc, Rectangle bounds, string? textToDraw, bool drawDisabled, TextFormatFlags flags)
    {
        if (bounds.Width < 0 || bounds.Height < 0)
        {
            return;
        }

        if (!string.IsNullOrEmpty(textToDraw))
        {
            uint disableFlag = drawDisabled ? 0x1u : 0u;
            _lastHResult = PInvoke.DrawThemeText(
                HTHEME,
                dc,
                Part,
                State,
                textToDraw,
                textToDraw.Length,
                (DRAW_TEXT_FORMAT)flags,
                disableFlag,
                bounds);
        }
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public Rectangle GetBackgroundContentRectangle(IDeviceContext dc, Rectangle bounds)
    {
        ArgumentNullException.ThrowIfNull(dc);

        using DeviceContextHdcScope hdc = dc.ToHdcScope();
        return GetBackgroundContentRectangle(hdc, bounds);
    }

    internal Rectangle GetBackgroundContentRectangle(HDC dc, Rectangle bounds)
    {
        if (bounds.Width < 0 || bounds.Height < 0)
        {
            return Rectangle.Empty;
        }

        _lastHResult = PInvoke.GetThemeBackgroundContentRect(HTHEME, dc, Part, State, bounds, out RECT rect);
        return rect;
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public Rectangle GetBackgroundExtent(IDeviceContext dc, Rectangle contentBounds)
    {
        ArgumentNullException.ThrowIfNull(dc);

        if (contentBounds.Width < 0 || contentBounds.Height < 0)
        {
            return Rectangle.Empty;
        }

        using DeviceContextHdcScope hdc = dc.ToHdcScope();
        _lastHResult = PInvoke.GetThemeBackgroundExtent(HTHEME, hdc, Part, State, contentBounds, out RECT extents);
        return extents;
    }

    /// <summary>
    ///  Computes the region for a regular or partially transparent background that is bounded by a specified
    ///  rectangle. Return null if the region cannot be created.
    ///  [See win32 equivalent.]
    /// </summary>
    public unsafe Region? GetBackgroundRegion(IDeviceContext dc, Rectangle bounds)
    {
        ArgumentNullException.ThrowIfNull(dc);

        if (bounds.Width < 0 || bounds.Height < 0)
        {
            return null;
        }

        using DeviceContextHdcScope hdc = dc.ToHdcScope();
        HRGN hrgn;
        _lastHResult = PInvoke.GetThemeBackgroundRegion(HTHEME, hdc, Part, State, bounds, &hrgn);

        // GetThemeBackgroundRegion returns a null hRegion if it fails to create one, it could be because the bounding
        // box is too big. For more info see code in %xpsrc%\shell\themes\uxtheme\imagefile.cpp if you have an enlistment to it.

        if (hrgn.IsNull)
        {
            return null;
        }

        // From the GDI+ sources it doesn't appear as if they take ownership of the hRegion, so this is safe to do.
        // We need to DeleteObject in order to not leak.
        Region region = Region.FromHrgn(hrgn);
        PInvokeCore.DeleteObject(hrgn);
        return region;
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public bool GetBoolean(BooleanProperty prop)
    {
        SourceGenerated.EnumValidator.Validate(prop, nameof(prop));

        _lastHResult = PInvoke.GetThemeBool(HTHEME, Part, State, (THEME_PROPERTY_SYMBOL_ID)prop, out BOOL value);
        return value;
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public Color GetColor(ColorProperty prop)
    {
        // Valid values are 0xed9 to 0xeef
        SourceGenerated.EnumValidator.Validate(prop, nameof(prop));

        _lastHResult = PInvoke.GetThemeColor(HTHEME, Part, State, (THEME_PROPERTY_SYMBOL_ID)prop, out COLORREF color);
        return color;
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public int GetEnumValue(EnumProperty prop)
    {
        // Valid values are 0xfa1 to 0xfaf
        SourceGenerated.EnumValidator.Validate(prop, nameof(prop));

        _lastHResult = PInvoke.GetThemeEnumValue(HTHEME, Part, State, (THEME_PROPERTY_SYMBOL_ID)prop, out int value);
        return value;
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public unsafe string GetFilename(FilenameProperty prop)
    {
        // Valid values are 0xbb9 to 0xbc0
        SourceGenerated.EnumValidator.Validate(prop, nameof(prop));

        Span<char> filename = stackalloc char[512];
        fixed (char* pFilename = filename)
        {
            _lastHResult = PInvoke.GetThemeFilename(HTHEME, Part, State, (THEME_PROPERTY_SYMBOL_ID)prop, pFilename, filename.Length);
        }

        return filename.SliceAtFirstNull().ToString();
    }

    /// <summary>
    ///  [See win32 equivalent.]
    ///  Returns null if the returned font was not true type, since GDI+ does not support it.
    /// </summary>
    public Font? GetFont(IDeviceContext dc, FontProperty prop)
    {
        ArgumentNullException.ThrowIfNull(dc);

        SourceGenerated.EnumValidator.Validate(prop, nameof(prop));

        using DeviceContextHdcScope hdc = dc.ToHdcScope();
        _lastHResult = PInvoke.GetThemeFont(this, hdc, Part, State, (int)prop, out LOGFONT logfont);

        // Check for a failed HR.
        if (!_lastHResult.Succeeded)
        {
            return null;
        }

        try
        {
            return Font.FromLogFont(logfont);
        }
        catch (Exception e) when (!e.IsCriticalException())
        {
            // Looks like the font was not true type
            return null;
        }
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public int GetInteger(IntegerProperty prop)
    {
        // Valid values are 0x961 to 0x978
        SourceGenerated.EnumValidator.Validate(prop, nameof(prop));

        _lastHResult = PInvoke.GetThemeInt(HTHEME, Part, State, (THEME_PROPERTY_SYMBOL_ID)prop, out int value);
        return value;
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public Size GetPartSize(IDeviceContext dc, ThemeSizeType type)
    {
        ArgumentNullException.ThrowIfNull(dc);

        using DeviceContextHdcScope hdc = dc.ToHdcScope();
        return GetPartSize(hdc, type, HWND.Null);
    }

    internal unsafe Size GetPartSize(HDC dc, ThemeSizeType type, HWND hwnd = default)
    {
        // Valid values are 0x0 to 0x2
        SourceGenerated.EnumValidator.Validate(type, nameof(type));

        if (!hwnd.IsNull && ScaleHelper.IsThreadPerMonitorV2Aware)
        {
            using var htheme = OpenThemeData(hwnd, Class);
            _lastHResult = PInvoke.GetThemePartSize(htheme, dc, Part, State, null, (THEMESIZE)type, out SIZE dpiSize);
            return dpiSize;
        }

        _lastHResult = PInvoke.GetThemePartSize(HTHEME, dc, Part, State, null, (THEMESIZE)type, out SIZE size);
        return size;
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public unsafe Size GetPartSize(IDeviceContext dc, Rectangle bounds, ThemeSizeType type)
    {
        ArgumentNullException.ThrowIfNull(dc);

        // Valid values are 0x0 to 0x2
        SourceGenerated.EnumValidator.Validate(type, nameof(type));

        using DeviceContextHdcScope hdc = dc.ToHdcScope();
        _lastHResult = PInvoke.GetThemePartSize(HTHEME, hdc, Part, State, bounds, (THEMESIZE)type, out SIZE size);
        return size;
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public Point GetPoint(PointProperty prop)
    {
        // valid values are 0xd49 to 0xd50
        SourceGenerated.EnumValidator.Validate(prop, nameof(prop));

        _lastHResult = PInvoke.GetThemePosition(HTHEME, Part, State, (THEME_PROPERTY_SYMBOL_ID)prop, out Point point);
        return point;
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public unsafe Padding GetMargins(IDeviceContext dc, MarginProperty prop)
    {
        ArgumentNullException.ThrowIfNull(dc);

        // Valid values are 0xe11 to 0xe13
        SourceGenerated.EnumValidator.Validate(prop, nameof(prop));

        using DeviceContextHdcScope hdc = dc.ToHdcScope();
        _lastHResult = PInvoke.GetThemeMargins(HTHEME, hdc, Part, State, (THEME_PROPERTY_SYMBOL_ID)prop, null, out MARGINS margins);

        return new Padding(margins.cxLeftWidth, margins.cyTopHeight, margins.cxRightWidth, margins.cyBottomHeight);
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public unsafe string GetString(StringProperty prop)
    {
        // Valid values are 0xc81 to 0xc81
        SourceGenerated.EnumValidator.Validate(prop, nameof(prop));

        Span<char> aString = stackalloc char[512];
        fixed (char* pString = aString)
        {
            _lastHResult = PInvoke.GetThemeString(HTHEME, Part, State, (int)prop, pString, aString.Length);
        }

        return aString.SliceAtFirstNull().ToString();
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public unsafe Rectangle GetTextExtent(IDeviceContext dc, string textToDraw, TextFormatFlags flags)
    {
        ArgumentNullException.ThrowIfNull(dc);
        textToDraw.ThrowIfNullOrEmpty();

        using DeviceContextHdcScope hdc = dc.ToHdcScope();
        _lastHResult = PInvoke.GetThemeTextExtent(
            HTHEME,
            hdc,
            Part,
            State,
            textToDraw,
            textToDraw.Length,
            (DRAW_TEXT_FORMAT)flags,
            null,
            out RECT rect);

        return rect;
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public unsafe Rectangle GetTextExtent(IDeviceContext dc, Rectangle bounds, string textToDraw, TextFormatFlags flags)
    {
        ArgumentNullException.ThrowIfNull(dc);
        textToDraw.ThrowIfNullOrEmpty();

        using DeviceContextHdcScope hdc = dc.ToHdcScope();
        _lastHResult = PInvoke.GetThemeTextExtent(
            HTHEME,
            hdc,
            Part,
            State,
            textToDraw,
            textToDraw.Length,
            (DRAW_TEXT_FORMAT)flags,
            bounds,
            out RECT rect);

        return rect;
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public TextMetrics GetTextMetrics(IDeviceContext dc)
    {
        ArgumentNullException.ThrowIfNull(dc);

        using DeviceContextHdcScope hdc = dc.ToHdcScope();
        _lastHResult = PInvoke.GetThemeTextMetrics(HTHEME, hdc, Part, State, out TEXTMETRICW tm);
        return TextMetrics.FromTEXTMETRICW(tm);
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public HitTestCode HitTestBackground(IDeviceContext dc, Rectangle backgroundRectangle, Point pt, HitTestOptions options)
    {
        ArgumentNullException.ThrowIfNull(dc);

        using DeviceContextHdcScope hdc = dc.ToHdcScope();
        _lastHResult = PInvoke.HitTestThemeBackground(
            HTHEME,
            hdc,
            Part,
            State,
            (HIT_TEST_BACKGROUND_OPTIONS)options,
            backgroundRectangle,
            HRGN.Null,
            pt,
            out ushort code);

        return (HitTestCode)code;
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public HitTestCode HitTestBackground(Graphics g, Rectangle backgroundRectangle, Region region, Point pt, HitTestOptions options)
    {
        ArgumentNullException.ThrowIfNull(g);
        ArgumentNullException.ThrowIfNull(region);

        IntPtr hRgn = region.GetHrgn(g);
        return HitTestBackground(g, backgroundRectangle, hRgn, pt, options);
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public HitTestCode HitTestBackground(IDeviceContext dc, Rectangle backgroundRectangle, IntPtr hRgn, Point pt, HitTestOptions options)
    {
        ArgumentNullException.ThrowIfNull(dc);

        using DeviceContextHdcScope hdc = dc.ToHdcScope();
        _lastHResult = PInvoke.HitTestThemeBackground(
            HTHEME,
            hdc,
            Part,
            State,
            (HIT_TEST_BACKGROUND_OPTIONS)options,
            backgroundRectangle,
            (HRGN)hRgn,
            pt,
            out ushort code);

        return (HitTestCode)code;
    }

    /// <summary>
    ///  [See win32 equivalent.]
    /// </summary>
    public bool IsBackgroundPartiallyTransparent()
    {
        return PInvoke.IsThemeBackgroundPartiallyTransparent(HTHEME, Part, State);
    }

    /// <summary>
    ///  This is similar to GetLastError in Win32.  It returns the last HRESULT returned from a native call
    ///  into theme apis.  We eat the errors and let the user handle any errors that occurred.
    /// </summary>
    public int LastHResult => (int)_lastHResult;

    /// <summary>
    ///  Handles the ThemeChanged event. Basically, we need to ensure all per-thread theme handle
    ///  caches are refreshed.
    /// </summary>
    private static void OnUserPreferenceChanging(object sender, UserPreferenceChangingEventArgs ea)
    {
        if (ea.Category == UserPreferenceCategory.VisualStyle)
        {
            // Let all threads know their cached handles are no longer valid;
            // cache refresh will happen at next handle access.
            // Note that if the theme changes 2^sizeof(long) times before a thread uses
            // its handle, this whole version check won't work, but it is unlikely to happen.

            // this is not ideal.
            s_globalCacheVersion++;
        }
    }

    /// <summary>
    ///  Refreshes this thread's theme handle cache.
    /// </summary>
    private static void RefreshCache()
    {
        if (t_themeHandles is null)
        {
            return;
        }

        string[] classNames = new string[t_themeHandles.Keys.Count];
        t_themeHandles.Keys.CopyTo(classNames, 0);

        foreach (string className in classNames)
        {
            ThemeHandle? themeHandle = t_themeHandles[className];
            themeHandle?.Dispose();

            // We don't call IsSupported here, since that could cause RefreshCache to be called again,
            // leading to stack overflow.
            if (AreClientAreaVisualStylesSupported)
            {
                themeHandle = ThemeHandle.Create(className, false);
                if (themeHandle is not null)
                {
                    t_themeHandles[className] = themeHandle;
                }
            }
        }
    }

    private static HTHEME GetHandle(string className)
    {
        return GetHandle(className, true);
    }

    /// <summary>
    ///  Retrieves a theme handle for the given class from the handle cache. If its not
    ///  present in the cache, it creates a new object and stores it there.
    /// </summary>
    private static HTHEME GetHandle(string className, bool throwExceptionOnFail)
    {
        t_themeHandles ??= new(NumberOfPossibleClasses);
        if (t_threadCacheVersion != s_globalCacheVersion)
        {
            RefreshCache();
            t_threadCacheVersion = s_globalCacheVersion;
        }

        if (!t_themeHandles.TryGetValue(className, out ThemeHandle? themeHandle))
        {
            // See if it is already in cache
            themeHandle = ThemeHandle.Create(className, throwExceptionOnFail);
            if (themeHandle is null)
            {
                return HTHEME.Null;
            }

            t_themeHandles[className] = themeHandle;
        }

        return themeHandle.Handle;
    }

    private static PInvoke.OpenThemeDataScope OpenThemeData(HWND hwnd, string classList)
    {
        PInvoke.OpenThemeDataScope htheme = new(hwnd, classList);
        return htheme.IsNull ? throw new InvalidOperationException(SR.VisualStyleHandleCreationFailed) : htheme;
    }
}
