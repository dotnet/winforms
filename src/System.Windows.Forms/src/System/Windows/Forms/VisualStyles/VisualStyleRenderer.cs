// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using Microsoft.Win32;
using static Interop;
using static Interop.UxTheme;

namespace System.Windows.Forms.VisualStyles
{
    /// <summary>
    ///  This class provides full feature parity with UxTheme API.
    /// </summary>
    public sealed class VisualStyleRenderer : IHandle
    {
        private HRESULT _lastHResult;
        private const int NumberOfPossibleClasses = VisualStyleElement.Count; //used as size for themeHandles

        [ThreadStatic]
        private static Hashtable? t_themeHandles; // per-thread cache of ThemeHandle objects.

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
                    supported = (hTheme != IntPtr.Zero);
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
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            return IsCombinationDefined(element.ClassName, element.Part);
        }

        internal static bool IsCombinationDefined(string className, int part)
        {
            bool returnVal = false;

            if (!IsSupported)
            {
                throw new InvalidOperationException(VisualStyleInformation.IsEnabledByUser
                    ? SR.VisualStylesDisabledInClientArea
                    : SR.VisualStyleNotActive);
            }

            IntPtr hTheme = GetHandle(className, false);

            if (hTheme != IntPtr.Zero)
            {
                // IsThemePartDefined doesn't work for part = 0, although there are valid parts numbered 0. We
                // allow these explicitly here.
                if (part == 0)
                {
                    returnVal = true;
                }
                else
                {
                    returnVal = IsThemePartDefined(hTheme, part, 0).IsTrue();
                }
            }

            // If the combo isn't defined, check the validity of our theme handle cache
            if (!returnVal)
            {
                using (ThemeHandle? tHandle = ThemeHandle.Create(className, false))
                {
                    if (tHandle != null)
                    {
                        returnVal = IsThemePartDefined(tHandle, part, 0).IsTrue();
                    }

                    // If we did, in fact get a new correct theme handle, our cache is out of date -- update it now.
                    if (returnVal)
                    {
                        RefreshCache();
                    }
                }
            }

            return returnVal;
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
            if (className is null)
                throw new ArgumentNullException(nameof(className));

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
        ///  NOTE: The handle gets invalidated when the theme changes or the user disables theming. When that
        ///   happens, the user should requery this property to get the correct handle. To know when the
        ///   theme changed, hook on to SystemEvents.UserPreferenceChanged and look for ThemeChanged
        ///   category.
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                if (!IsSupported)
                {
                    throw new InvalidOperationException(VisualStyleInformation.IsEnabledByUser
                        ? SR.VisualStylesDisabledInClientArea
                        : SR.VisualStyleNotActive);
                }

                return GetHandle(Class);
            }
        }

        /// <summary>
        ///  Used to set a new VisualStyleElement on this VisualStyleRenderer instance.
        /// </summary>
        public void SetParameters(VisualStyleElement element)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

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
            if (dc is null)
                throw new ArgumentNullException(nameof(dc));

            using var hdc = new DeviceContextHdcScope(dc);
            DrawBackground(hdc, bounds, IntPtr.Zero);
        }

        internal unsafe void DrawBackground(Gdi32.HDC dc, Rectangle bounds, IntPtr hwnd = default)
        {
            if (bounds.Width < 0 || bounds.Height < 0)
                return;

            if (IntPtr.Zero != hwnd)
            {
                using var htheme = new UxTheme.OpenThemeDataScope(hwnd, Class);
                if (htheme.IsNull)
                {
                    throw new InvalidOperationException(SR.VisualStyleHandleCreationFailed);
                }
                RECT rect = bounds;
                _lastHResult = DrawThemeBackground(htheme, dc, Part, State, ref rect, null);
            }
            else
            {
                RECT rect  = bounds;
                _lastHResult = DrawThemeBackground(this, dc, Part, State, ref rect, null);
            }
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public void DrawBackground(IDeviceContext dc, Rectangle bounds, Rectangle clipRectangle)
        {
            if (dc is null)
                throw new ArgumentNullException(nameof(dc));

            using var hdc = new DeviceContextHdcScope(dc);
            DrawBackground(hdc, bounds, clipRectangle, IntPtr.Zero);
        }

        internal unsafe void DrawBackground(Gdi32.HDC dc, Rectangle bounds, Rectangle clipRectangle, IntPtr hwnd)
        {
            if (bounds.Width < 0 || bounds.Height < 0 || clipRectangle.Width < 0 || clipRectangle.Height < 0)
                return;

            if (IntPtr.Zero != hwnd)
            {
                using var htheme = new UxTheme.OpenThemeDataScope(hwnd, Class);
                if (htheme.IsNull)
                {
                    throw new InvalidOperationException(SR.VisualStyleHandleCreationFailed);
                }
                RECT rect = bounds;
                RECT clipRect = clipRectangle;
                _lastHResult = DrawThemeBackground(htheme, dc, Part, State, ref rect, &clipRect);
            }
            else
            {
                RECT rect = bounds;
                RECT clipRect = clipRectangle;
                _lastHResult = DrawThemeBackground(this, dc, Part, State, ref rect, &clipRect);
            }
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public Rectangle DrawEdge(IDeviceContext dc, Rectangle bounds, Edges edges, EdgeStyle style, EdgeEffects effects)
        {
            if (dc is null)
                throw new ArgumentNullException(nameof(dc));

            using var hdc = new DeviceContextHdcScope(dc);
            return DrawEdge(hdc, bounds, edges, style, effects);
        }

        internal Rectangle DrawEdge(Gdi32.HDC dc, Rectangle bounds, Edges edges, EdgeStyle style, EdgeEffects effects)
        {
            if (!ClientUtils.IsEnumValid_Masked(edges, (int)edges, (uint)(Edges.Left | Edges.Top | Edges.Right | Edges.Bottom | Edges.Diagonal)))
                throw new InvalidEnumArgumentException(nameof(edges), (int)edges, typeof(Edges));

            if (!ClientUtils.IsEnumValid_NotSequential(style, (int)style, (int)EdgeStyle.Raised, (int)EdgeStyle.Sunken, (int)EdgeStyle.Etched, (int)EdgeStyle.Bump))
                throw new InvalidEnumArgumentException(nameof(style), (int)style, typeof(EdgeStyle));

            if (!ClientUtils.IsEnumValid_Masked(effects, (int)effects, (uint)(EdgeEffects.FillInterior | EdgeEffects.Flat | EdgeEffects.Soft | EdgeEffects.Mono)))
                throw new InvalidEnumArgumentException(nameof(effects), (int)effects, typeof(EdgeEffects));

            RECT destRect = bounds;
            var contentRect = new RECT();
            _lastHResult = DrawThemeEdge(
                this,
                dc,
                Part,
                State,
                ref destRect,
                (User32.EDGE)style,
                (User32.BF)edges | (User32.BF)effects | User32.BF.ADJUST,
                ref contentRect);

            return contentRect;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        ///  This method uses Graphics.DrawImage as a backup if themed drawing does not work.
        /// </summary>
        public void DrawImage(Graphics g, Rectangle bounds, Image image)
        {
            if (g is null)
                throw new ArgumentNullException(nameof(g));

            if (image is null)
                throw new ArgumentNullException(nameof(image));

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
            if (g is null)
                throw new ArgumentNullException(nameof(g));

            if (imageList is null)
                throw new ArgumentNullException(nameof(imageList));

            if (imageIndex < 0 || imageIndex >= imageList.Images.Count)
                throw new ArgumentOutOfRangeException(nameof(imageIndex), imageIndex, string.Format(SR.InvalidArgument, nameof(imageIndex), imageIndex));

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
            if (dc is null)
                throw new ArgumentNullException(nameof(dc));

            if (childControl is null)
                throw new ArgumentNullException(nameof(childControl));

            if (bounds.Width < 0 || bounds.Height < 0)
                return;

            if (childControl.IsHandleCreated)
            {
                using var hdc = new DeviceContextHdcScope(dc);
                RECT rc = bounds;
                _lastHResult = DrawThemeParentBackground(childControl, hdc, ref rc);
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
            if (dc is null)
                throw new ArgumentNullException(nameof(dc));

            using var hdc = new DeviceContextHdcScope(dc);
            DrawText(hdc, bounds, textToDraw, drawDisabled, flags);
        }

        internal void DrawText(Gdi32.HDC dc, Rectangle bounds, string? textToDraw, bool drawDisabled, TextFormatFlags flags)
        {
            if (bounds.Width < 0 || bounds.Height < 0)
                return;

            if (!string.IsNullOrEmpty(textToDraw))
            {
                uint disableFlag = drawDisabled ? 0x1u : 0u;
                RECT rect = bounds;
                _lastHResult = DrawThemeText(this, dc, Part, State, textToDraw, textToDraw.Length, (User32.DT)flags, disableFlag, ref rect);
            }
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public Rectangle GetBackgroundContentRectangle(IDeviceContext dc, Rectangle bounds)
        {
            if (dc is null)
                throw new ArgumentNullException(nameof(dc));

            using var hdc = new DeviceContextHdcScope(dc);
            return GetBackgroundContentRectangle(hdc, bounds);
        }

        internal Rectangle GetBackgroundContentRectangle(Gdi32.HDC dc, Rectangle bounds)
        {
            if (bounds.Width < 0 || bounds.Height < 0)
                return Rectangle.Empty;

            RECT boundsRect = bounds;
            _lastHResult = GetThemeBackgroundContentRect(this, dc, Part, State, ref boundsRect, out RECT rect);
            return rect;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public Rectangle GetBackgroundExtent(IDeviceContext dc, Rectangle contentBounds)
        {
            if (dc is null)
                throw new ArgumentNullException(nameof(dc));

            if (contentBounds.Width < 0 || contentBounds.Height < 0)
                return Rectangle.Empty;

            using var hdc = new DeviceContextHdcScope(dc);
            RECT contentBoundsRect = contentBounds;
            _lastHResult = GetThemeBackgroundExtent(this, hdc, Part, State, ref contentBoundsRect, out RECT extentRect);
            return extentRect;
        }

        /// <summary>
        ///  Computes the region for a regular or partially transparent background that is bounded by a specified
        ///  rectangle. Return null if the region cannot be created.
        ///  [See win32 equivalent.]
        /// </summary>
        public Region? GetBackgroundRegion(IDeviceContext dc, Rectangle bounds)
        {
            if (dc is null)
                throw new ArgumentNullException(nameof(dc));

            if (bounds.Width < 0 || bounds.Height < 0)
                return null;

            using var hdc = new DeviceContextHdcScope(dc);
            RECT boundsRect = bounds;
            _lastHResult = GetThemeBackgroundRegion(this, hdc, Part, State, ref boundsRect, out Gdi32.HRGN hRegion);

            // GetThemeBackgroundRegion returns a null hRegion if it fails to create one, it could be because the bounding
            // box is too big. For more info see code in %xpsrc%\shell\themes\uxtheme\imagefile.cpp if you have an enlistment to it.

            if (hRegion.IsNull)
            {
                return null;
            }

            // From the GDI+ sources it doesn't appear as if they take ownership of the hRegion, so this is safe to do.
            // We need to DeleteObject in order to not leak.
            Region region = Region.FromHrgn(hRegion.Handle);
            Gdi32.DeleteObject(hRegion);
            return region;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public bool GetBoolean(BooleanProperty prop)
        {
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)BooleanProperty.Transparent, (int)BooleanProperty.SourceShrink))
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(BooleanProperty));

            BOOL val = BOOL.FALSE;
            _lastHResult = GetThemeBool(this, Part, State, (int)prop, ref val);
            return val.IsTrue();
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public Color GetColor(ColorProperty prop)
        {
            // Valid values are 0xed9 to 0xeef
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)ColorProperty.BorderColor, (int)ColorProperty.AccentColorHint))
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(ColorProperty));

            int color = 0;
            _lastHResult = GetThemeColor(this, Part, State, (int)prop, ref color);
            return ColorTranslator.FromWin32(color);
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public int GetEnumValue(EnumProperty prop)
        {
            // Valid values are 0xfa1 to 0xfaf
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)EnumProperty.BackgroundType, (int)EnumProperty.TrueSizeScalingType))
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(EnumProperty));

            int val = 0;
            _lastHResult = GetThemeEnumValue(this, Part, State, (int)prop, ref val);
            return val;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public unsafe string GetFilename(FilenameProperty prop)
        {
            // Valid values are 0xbb9 to 0xbc0
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)FilenameProperty.ImageFile, (int)FilenameProperty.GlyphImageFile))
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(FilenameProperty));

            Span<char> filename = stackalloc char[512];
            fixed (char* pFilename = filename)
            {
                _lastHResult = GetThemeFilename(this, Part, State, (int)prop, pFilename, filename.Length);
            }
            return filename.SliceAtFirstNull().ToString();
        }

        /// <summary>
        ///  [See win32 equivalent.]
        ///  Returns null if the returned font was not true type, since GDI+ does not support it.
        /// </summary>
        public Font? GetFont(IDeviceContext dc, FontProperty prop)
        {
            if (dc is null)
                throw new ArgumentNullException(nameof(dc));

            if (!ClientUtils.IsEnumValid_NotSequential(prop, (int)prop, (int)FontProperty.TextFont, (int)FontProperty.GlyphFont))
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(FontProperty));

            using var hdc = new DeviceContextHdcScope(dc);
            _lastHResult = GetThemeFont(this, hdc, Part, State, (int)prop, out User32.LOGFONTW logfont);

            // Check for a failed HR.
            if (!_lastHResult.Succeeded())
            {
                return null;
            }

            try
            {
                return Font.FromLogFont(logfont);
            }
            catch (Exception e)
            {
                if (ClientUtils.IsCriticalException(e))
                {
                    throw;
                }

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
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)IntegerProperty.ImageCount, (int)IntegerProperty.MinDpi5))
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(IntegerProperty));

            int val = 0;
            _lastHResult = GetThemeInt(this, Part, State, (int)prop, ref val);
            return val;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public Size GetPartSize(IDeviceContext dc, ThemeSizeType type)
        {
            if (dc is null)
                throw new ArgumentNullException(nameof(dc));

            using var hdc = new DeviceContextHdcScope(dc);
            return GetPartSize(hdc, type, IntPtr.Zero);
        }

        internal unsafe Size GetPartSize(Gdi32.HDC dc, ThemeSizeType type, IntPtr hwnd = default)
        {
            // Valid values are 0x0 to 0x2
            if (!ClientUtils.IsEnumValid(type, (int)type, (int)ThemeSizeType.Minimum, (int)ThemeSizeType.Draw))
                throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(ThemeSizeType));

            if (DpiHelper.IsPerMonitorV2Awareness && hwnd != IntPtr.Zero)
            {
                using var htheme = new UxTheme.OpenThemeDataScope(hwnd, Class);
                if (htheme.IsNull)
                {
                    throw new InvalidOperationException(SR.VisualStyleHandleCreationFailed);
                }
                _lastHResult = GetThemePartSize(htheme, dc, Part, State, null, type, out Size dpiSize);
                return dpiSize;
            }

            _lastHResult = GetThemePartSize(this, dc, Part, State, null, type, out Size size);
            return size;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public unsafe Size GetPartSize(IDeviceContext dc, Rectangle bounds, ThemeSizeType type)
        {
            if (dc is null)
                throw new ArgumentNullException(nameof(dc));

            // Valid values are 0x0 to 0x2
            if (!ClientUtils.IsEnumValid(type, (int)type, (int)ThemeSizeType.Minimum, (int)ThemeSizeType.Draw))
                throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(ThemeSizeType));

            using var hdc = new DeviceContextHdcScope(dc);
            RECT boundsRect = bounds;
            _lastHResult = GetThemePartSize(this, hdc, Part, State, &boundsRect, type, out Size size);
            return size;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public Point GetPoint(PointProperty prop)
        {
            //valid values are 0xd49 to 0xd50
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)PointProperty.Offset, (int)PointProperty.MinSize5))
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(PointProperty));

            _lastHResult = GetThemePosition(this, Part, State, (int)prop, out Point point);
            return point;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public unsafe Padding GetMargins(IDeviceContext dc, MarginProperty prop)
        {
            if (dc is null)
                throw new ArgumentNullException(nameof(dc));

            // Valid values are 0xe11 to 0xe13
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)MarginProperty.SizingMargins, (int)MarginProperty.CaptionMargins))
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(MarginProperty));

            using var hdc = new DeviceContextHdcScope(dc);
            _lastHResult = GetThemeMargins(this, hdc, Part, State, (int)prop, null, out MARGINS margins);

            return new Padding(margins.cxLeftWidth, margins.cyTopHeight, margins.cxRightWidth, margins.cyBottomHeight);
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public unsafe string GetString(StringProperty prop)
        {
            // Valid values are 0xc81 to 0xc81
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)StringProperty.Text, (int)StringProperty.Text))
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(StringProperty));

            Span<char> aString = stackalloc char[512];
            fixed (char* pString = aString)
            {
                _lastHResult = GetThemeString(this, Part, State, (int)prop, pString, aString.Length);
            }
            return aString.SliceAtFirstNull().ToString();
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public unsafe Rectangle GetTextExtent(IDeviceContext dc, string textToDraw, TextFormatFlags flags)
        {
            if (dc is null)
                throw new ArgumentNullException(nameof(dc));

            if (string.IsNullOrEmpty(textToDraw))
                throw new ArgumentNullException(nameof(textToDraw));

            using var hdc = new DeviceContextHdcScope(dc);
            _lastHResult = GetThemeTextExtent(this, hdc, Part, State, textToDraw, textToDraw.Length, (uint)flags, null, out RECT rect);
            return rect;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public unsafe Rectangle GetTextExtent(IDeviceContext dc, Rectangle bounds, string textToDraw, TextFormatFlags flags)
        {
            if (dc is null)
                throw new ArgumentNullException(nameof(dc));

            if (string.IsNullOrEmpty(textToDraw))
                throw new ArgumentNullException(nameof(textToDraw));

            using var hdc = new DeviceContextHdcScope(dc);
            RECT boundsRect = bounds;
            _lastHResult = GetThemeTextExtent(this, hdc, Part, State, textToDraw, textToDraw.Length, (uint)flags, &boundsRect, out RECT rect);
            return rect;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public TextMetrics GetTextMetrics(IDeviceContext dc)
        {
            if (dc is null)
                throw new ArgumentNullException(nameof(dc));

            using var hdc = new DeviceContextHdcScope(dc);
            _lastHResult = GetThemeTextMetrics(this, hdc, Part, State, out TextMetrics tm);
            return tm;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public HitTestCode HitTestBackground(IDeviceContext dc, Rectangle backgroundRectangle, Point pt, HitTestOptions options)
        {
            if (dc is null)
                throw new ArgumentNullException(nameof(dc));

            using var hdc = new DeviceContextHdcScope(dc);
            RECT backgroundRect = backgroundRectangle;
            _lastHResult = HitTestThemeBackground(this, hdc, Part, State, (uint)options, ref backgroundRect, IntPtr.Zero, pt, out ushort htCode);
            return (HitTestCode)htCode;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public HitTestCode HitTestBackground(Graphics g, Rectangle backgroundRectangle, Region region, Point pt, HitTestOptions options)
        {
            if (g is null)
                throw new ArgumentNullException(nameof(g));

            if (region is null)
                throw new ArgumentNullException(nameof(region));

            IntPtr hRgn = region.GetHrgn(g);
            return HitTestBackground(g, backgroundRectangle, hRgn, pt, options);
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public HitTestCode HitTestBackground(IDeviceContext dc, Rectangle backgroundRectangle, IntPtr hRgn, Point pt, HitTestOptions options)
        {
            if (dc is null)
                throw new ArgumentNullException(nameof(dc));

            using var hdc = new DeviceContextHdcScope(dc);
            RECT backgroundRect = backgroundRectangle;
            _lastHResult = HitTestThemeBackground(this, hdc, Part, State, (uint)options, ref backgroundRect, hRgn, pt, out ushort htCode);
            return (HitTestCode)htCode;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public bool IsBackgroundPartiallyTransparent()
        {
            return IsThemeBackgroundPartiallyTransparent(this, Part, State).IsTrue();
        }

        /// <summary>
        ///  This is similar to GetLastError in Win32.  It returns the last HRESULT returned from a native call
        ///  into theme apis.  We eat the errors and let the user handle any errors that occurred.
        /// </summary>
        public int LastHResult => (int)_lastHResult;

        /// <summary>
        ///  Instantiates the ThemeHandle cache hashtable.
        /// </summary>
        private static void CreateThemeHandleHashtable()
        {
            t_themeHandles = new Hashtable(NumberOfPossibleClasses);
        }

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
            ThemeHandle? tHandle = null;

            if (t_themeHandles != null)
            {
                string[] classNames = new string[t_themeHandles.Keys.Count];
                t_themeHandles.Keys.CopyTo(classNames, 0);

                foreach (string className in classNames)
                {
                    tHandle = (ThemeHandle?)t_themeHandles[className];
                    if (tHandle != null)
                    {
                        tHandle.Dispose();
                    }

                    // We don't call IsSupported here, since that could cause RefreshCache to be called again,
                    // leading to stack overflow.
                    if (AreClientAreaVisualStylesSupported)
                    {
                        tHandle = ThemeHandle.Create(className, false);
                        if (tHandle != null)
                        {
                            t_themeHandles[className] = tHandle;
                        }
                    }
                }
            }
        }

        private static IntPtr GetHandle(string className)
        {
            return GetHandle(className, true);
        }

        /// <summary>
        ///  Retrieves a IntPtr theme handle for the given class from the themeHandle cache. If its not
        ///  present in the cache, it creates a new ThemeHandle object and stores it there.
        /// </summary>
        private static IntPtr GetHandle(string className, bool throwExceptionOnFail)
        {
            if (t_themeHandles is null)
            {
                CreateThemeHandleHashtable();
            }

            if (t_threadCacheVersion != s_globalCacheVersion)
            {
                RefreshCache();
                t_threadCacheVersion = s_globalCacheVersion;
            }

            if (!t_themeHandles!.Contains(className))
            {
                // See if it is already in cache
                ThemeHandle? tHandle = ThemeHandle.Create(className, throwExceptionOnFail);
                if (tHandle is null)
                {
                    return IntPtr.Zero;
                }

                t_themeHandles.Add(className, tHandle);
                return tHandle.Handle;
            }

            return ((ThemeHandle)t_themeHandles[className]!).Handle;
        }

        // This wrapper class is needed for safely cleaning up TLS cache of handles.
        private class ThemeHandle : IDisposable, IHandle
        {
            private ThemeHandle(IntPtr hTheme)
            {
                Handle = hTheme;
            }

            public IntPtr Handle { get; private set; }

            public static ThemeHandle? Create(string className, bool throwExceptionOnFail)
            {
                return Create(className, throwExceptionOnFail, IntPtr.Zero);
            }

            internal static ThemeHandle? Create(string className, bool throwExceptionOnFail, IntPtr hWndRef)
            {
                // HThemes require an HWND when display scaling is different between monitors.
                IntPtr hTheme = OpenThemeData(hWndRef, className);

                if (hTheme == IntPtr.Zero)
                {
                    if (throwExceptionOnFail)
                    {
                        throw new InvalidOperationException(SR.VisualStyleHandleCreationFailed);
                    }

                    return null;
                }

                return new ThemeHandle(hTheme);
            }

            public void Dispose()
            {
                if (Handle != IntPtr.Zero)
                {
                    CloseThemeData(Handle);
                    Handle = IntPtr.Zero;
                }

                GC.SuppressFinalize(this);
            }

            ~ThemeHandle() => Dispose();
        }
    }
}
