// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms.VisualStyles
{
    /// <summary>
    ///  This class provides full feature parity with UxTheme API.
    /// </summary>
    public sealed class VisualStyleRenderer
    {
        private const TextFormatFlags AllGraphicsProperties = TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.PreserveGraphicsTranslateTransform;

        internal const int EdgeAdjust = 0x2000; //used with Edges in VisualStyleRenderer.DrawThemeEdge
        private string _class;
        private int part;
        private int state;
        private int lastHResult = 0;
        private static readonly int numberOfPossibleClasses = VisualStyleElement.Count; //used as size for themeHandles

        [ThreadStatic]
        private static Hashtable themeHandles = null; //per-thread cache of ThemeHandle objects.
        [ThreadStatic]
        private static long threadCacheVersion = 0;

        private static long globalCacheVersion = 0;

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
                    IntPtr hTheme = GetHandle("BUTTON", false); //Button is an arbitrary choice.
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
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return IsCombinationDefined(element.ClassName, element.Part);
        }

        internal static bool IsCombinationDefined(string className, int part)
        {
            bool returnVal = false;

            if (!IsSupported)
            {
                if (!VisualStyleInformation.IsEnabledByUser)
                {
                    throw new InvalidOperationException(SR.VisualStyleNotActive);
                }
                else
                {
                    throw new InvalidOperationException(SR.VisualStylesDisabledInClientArea);
                }
            }

            if (className == null)
            {
                throw new ArgumentNullException(nameof(className));
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
                    returnVal = SafeNativeMethods.IsThemePartDefined(new HandleRef(null, hTheme), part, 0);
                }
            }

            //if the combo isn't defined, check the validity of our theme handle cache
            if (!returnVal)
            {
                using (ThemeHandle tHandle = ThemeHandle.Create(className, false))
                {
                    if (tHandle != null)
                    {
                        returnVal = SafeNativeMethods.IsThemePartDefined(new HandleRef(null, tHandle.NativeHandle), part, 0);
                    }

                    //if we did, in fact get a new correct theme handle, our cache is out of date -- update it now.
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
            if (!IsCombinationDefined(className, part))
            { //internally this call takes care of IsSupported.
                throw new ArgumentException(SR.VisualStylesInvalidCombination);
            }

            _class = className;
            this.part = part;
            this.state = state;
        }

        /// <summary>
        ///  Returns the current _class. Use SetParameters to set.
        /// </summary>
        public string Class
        {
            get
            {
                return _class;
            }
        }

        /// <summary>
        ///  Returns the current part. Use SetParameters to set.
        /// </summary>
        public int Part
        {
            get
            {
                return part;
            }
        }

        /// <summary>
        ///  Returns the current state. Use SetParameters to set.
        /// </summary>
        public int State
        {
            get
            {
                return state;
            }
        }

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
                    if (!VisualStyleInformation.IsEnabledByUser)
                    {
                        throw new InvalidOperationException(SR.VisualStyleNotActive);
                    }
                    else
                    {
                        throw new InvalidOperationException(SR.VisualStylesDisabledInClientArea);
                    }
                }

                return GetHandle(_class);
            }
        }

        /// <summary>
        ///  Used to set a new VisualStyleElement on this VisualStyleRenderer instance.
        /// </summary>
        public void SetParameters(VisualStyleElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

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
            { //internally this call takes care of IsSupported.
                throw new ArgumentException(SR.VisualStylesInvalidCombination);
            }

            _class = className;
            this.part = part;
            this.state = state;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public void DrawBackground(IDeviceContext dc, Rectangle bounds)
        {
            DrawBackground(dc, bounds, IntPtr.Zero);
        }

        internal void DrawBackground(IDeviceContext dc, Rectangle bounds, IntPtr hWnd)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }
            if (bounds.Width < 0 || bounds.Height < 0)
            {
                return;
            }

            using (WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper(dc, AllGraphicsProperties))
            {
                HandleRef hdc = new HandleRef(wgr, wgr.WindowsGraphics.DeviceContext.Hdc);
                if (IntPtr.Zero != hWnd)
                {
                    using (ThemeHandle hTheme = ThemeHandle.Create(_class, true, new HandleRef(null, hWnd)))
                    {
                        lastHResult = SafeNativeMethods.DrawThemeBackground(new HandleRef(this, hTheme.NativeHandle), hdc, part, state, new NativeMethods.COMRECT(bounds), null);
                    }
                }
                else
                {
                    lastHResult = SafeNativeMethods.DrawThemeBackground(new HandleRef(this, Handle), hdc, part, state, new NativeMethods.COMRECT(bounds), null);
                }
            }
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public void DrawBackground(IDeviceContext dc, Rectangle bounds, Rectangle clipRectangle)
        {
            DrawBackground(dc, bounds, clipRectangle, IntPtr.Zero);
        }

        internal void DrawBackground(IDeviceContext dc, Rectangle bounds, Rectangle clipRectangle, IntPtr hWnd)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }
            if (bounds.Width < 0 || bounds.Height < 0)
            {
                return;
            }
            if (clipRectangle.Width < 0 || clipRectangle.Height < 0)
            {
                return;
            }

            using (WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper(dc, AllGraphicsProperties))
            {
                HandleRef hdc = new HandleRef(wgr, wgr.WindowsGraphics.DeviceContext.Hdc);
                if (IntPtr.Zero != hWnd)
                {
                    using (ThemeHandle hTheme = ThemeHandle.Create(_class, true, new HandleRef(null, hWnd)))
                    {
                        lastHResult = SafeNativeMethods.DrawThemeBackground(new HandleRef(this, hTheme.NativeHandle), hdc, part, state, new NativeMethods.COMRECT(bounds), new NativeMethods.COMRECT(clipRectangle));
                    }
                }
                else
                {
                    lastHResult = SafeNativeMethods.DrawThemeBackground(new HandleRef(this, Handle), hdc, part, state, new NativeMethods.COMRECT(bounds), new NativeMethods.COMRECT(clipRectangle));
                }
            }
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public Rectangle DrawEdge(IDeviceContext dc, Rectangle bounds, Edges edges, EdgeStyle style, EdgeEffects effects)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            if (!ClientUtils.IsEnumValid_Masked(edges, (int)edges, (uint)(Edges.Left | Edges.Top | Edges.Right | Edges.Bottom | Edges.Diagonal)))
            {
                throw new InvalidEnumArgumentException(nameof(edges), (int)edges, typeof(Edges));
            }

            if (!ClientUtils.IsEnumValid_NotSequential(style, (int)style, (int)EdgeStyle.Raised, (int)EdgeStyle.Sunken, (int)EdgeStyle.Etched, (int)EdgeStyle.Bump))
            {
                throw new InvalidEnumArgumentException(nameof(style), (int)style, typeof(EdgeStyle));
            }

            if (!ClientUtils.IsEnumValid_Masked(effects, (int)effects, (uint)(EdgeEffects.FillInterior | EdgeEffects.Flat | EdgeEffects.Soft | EdgeEffects.Mono)))
            {
                throw new InvalidEnumArgumentException(nameof(effects), (int)effects, typeof(EdgeEffects));
            }

            NativeMethods.COMRECT rect = new NativeMethods.COMRECT();

            using (WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper(dc, AllGraphicsProperties))
            {
                HandleRef hdc = new HandleRef(wgr, wgr.WindowsGraphics.DeviceContext.Hdc);
                lastHResult = SafeNativeMethods.DrawThemeEdge(new HandleRef(this, Handle), hdc, part, state, new NativeMethods.COMRECT(bounds), (int)style, (int)edges | (int)effects | EdgeAdjust, rect);
            }

            return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }

        /// <summary>
        ///  [See win32 equivalent.]
        ///  This method uses Graphics.DrawImage as a backup if themed drawing does not work.
        /// </summary>
        public void DrawImage(Graphics g, Rectangle bounds, Image image)
        {
            if (g == null)
            {
                throw new ArgumentNullException(nameof(g));
            }

            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (bounds.Width < 0 || bounds.Height < 0)
            {
                return;
            }

            g.DrawImage(image, bounds);
        }

        /// <summary>
        ///  [See win32 equivalent.]
        ///  This method uses Graphics.DrawImage as a backup if themed drawing does not work.
        /// </summary>
        public void DrawImage(Graphics g, Rectangle bounds, ImageList imageList, int imageIndex)
        {
            if (g == null)
            {
                throw new ArgumentNullException(nameof(g));
            }

            if (imageList == null)
            {
                throw new ArgumentNullException(nameof(imageList));
            }

            if (imageIndex < 0 || imageIndex >= imageList.Images.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(imageIndex), imageIndex, string.Format(SR.InvalidArgument, nameof(imageIndex), imageIndex));
            }

            if (bounds.Width < 0 || bounds.Height < 0)
            {
                return;
            }

            // DrawThemeIcon currently seems to do nothing, but still return S_OK. As a workaround,
            // we call DrawImage on the graphics object itself for now.

            //int returnVal = NativeMethods.S_FALSE;
            //using( WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, AllGraphicsProperties ) ) {
            //    HandleRef hdc = new HandleRef( wgr, wgr.WindowsGraphics.DeviceContext.Hdc );
            //    returnVal = SafeNativeMethods.DrawThemeIcon( new HandleRef( this, Handle ), hdc, part, state, new NativeMethods.COMRECT( bounds ), new HandleRef( this, imageList.Handle ), imageIndex );
            //}

            //if (returnVal != NativeMethods.S_OK) {
            g.DrawImage(imageList.Images[imageIndex], bounds);
            //}
        }

        /// <summary>
        ///  Given a graphics object and bounds to draw in, this method effectively asks the passed in
        ///  control's parent to draw itself in there (it sends WM_ERASEBKGND & WM_PRINTCLIENT messages
        ///  to the parent).
        /// </summary>
        public void DrawParentBackground(IDeviceContext dc, Rectangle bounds, Control childControl)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            if (childControl == null)
            {
                throw new ArgumentNullException(nameof(childControl));
            }

            if (bounds.Width < 0 || bounds.Height < 0)
            {
                return;
            }

            if (childControl.Handle != IntPtr.Zero)
            {
                using (WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper(dc, AllGraphicsProperties))
                {
                    HandleRef hdc = new HandleRef(wgr, wgr.WindowsGraphics.DeviceContext.Hdc);
                    lastHResult = SafeNativeMethods.DrawThemeParentBackground(new HandleRef(this, childControl.Handle), hdc, new NativeMethods.COMRECT(bounds));
                }
            }
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public void DrawText(IDeviceContext dc, Rectangle bounds, string textToDraw)
        {
            DrawText(dc, bounds, textToDraw, false);
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public void DrawText(IDeviceContext dc, Rectangle bounds, string textToDraw, bool drawDisabled)
        {
            DrawText(dc, bounds, textToDraw, drawDisabled, TextFormatFlags.HorizontalCenter);
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public void DrawText(IDeviceContext dc, Rectangle bounds, string textToDraw, bool drawDisabled, TextFormatFlags flags)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            if (bounds.Width < 0 || bounds.Height < 0)
            {
                return;
            }

            int disableFlag = drawDisabled ? 0x1 : 0;

            if (!string.IsNullOrEmpty(textToDraw))
            {
                using (WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper(dc, AllGraphicsProperties))
                {
                    HandleRef hdc = new HandleRef(wgr, wgr.WindowsGraphics.DeviceContext.Hdc);
                    lastHResult = SafeNativeMethods.DrawThemeText(new HandleRef(this, Handle), hdc, part, state, textToDraw, textToDraw.Length, (int)flags, disableFlag, new NativeMethods.COMRECT(bounds));
                }
            }
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public Rectangle GetBackgroundContentRectangle(IDeviceContext dc, Rectangle bounds)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }
            if (bounds.Width < 0 || bounds.Height < 0)
            {
                return Rectangle.Empty;
            }

            NativeMethods.COMRECT rect = new NativeMethods.COMRECT();

            using (WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper(dc, AllGraphicsProperties))
            {
                HandleRef hdc = new HandleRef(wgr, wgr.WindowsGraphics.DeviceContext.Hdc);
                lastHResult = SafeNativeMethods.GetThemeBackgroundContentRect(new HandleRef(this, Handle), hdc, part, state, new NativeMethods.COMRECT(bounds), rect);
            }

            return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public Rectangle GetBackgroundExtent(IDeviceContext dc, Rectangle contentBounds)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }
            if (contentBounds.Width < 0 || contentBounds.Height < 0)
            {
                return Rectangle.Empty;
            }

            NativeMethods.COMRECT rect = new NativeMethods.COMRECT();

            using (WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper(dc, AllGraphicsProperties))
            {
                HandleRef hdc = new HandleRef(wgr, wgr.WindowsGraphics.DeviceContext.Hdc);
                lastHResult = SafeNativeMethods.GetThemeBackgroundExtent(new HandleRef(this, Handle), hdc, part, state, new NativeMethods.COMRECT(contentBounds), rect);
            }

            return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }

        /// <summary>
        ///  Computes the region for a regular or partially transparent background that is bounded by a specified
        ///  rectangle. Return null if the region cannot be created.
        ///  [See win32 equivalent.]
        /// </summary>
        public Region GetBackgroundRegion(IDeviceContext dc, Rectangle bounds)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }
            if (bounds.Width < 0 || bounds.Height < 0)
            {
                return null;
            }

            IntPtr hRegion = IntPtr.Zero;

            using (WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper(dc, AllGraphicsProperties))
            {
                HandleRef hdc = new HandleRef(wgr, wgr.WindowsGraphics.DeviceContext.Hdc);
                lastHResult = SafeNativeMethods.GetThemeBackgroundRegion(new HandleRef(this, Handle), hdc, part, state, new NativeMethods.COMRECT(bounds), ref hRegion);
            }

            // GetThemeBackgroundRegion returns a null hRegion if it fails to create one, it could be because the bounding
            // box is too big. For more info see code in %xpsrc%\shell\themes\uxtheme\imagefile.cpp if you have an enlistment to it.

            if (hRegion == IntPtr.Zero)
            {
                return null;
            }

            // From the GDI+ sources it doesn't appear as if they take ownership of the hRegion, so this is safe to do.
            // We need to DeleteObject in order to not leak.
            Region region = Region.FromHrgn(hRegion);
            Gdi32.DeleteObject(hRegion);
            return region;

        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public bool GetBoolean(BooleanProperty prop)
        {
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)BooleanProperty.Transparent, (int)BooleanProperty.SourceShrink))
            {
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(BooleanProperty));
            }

            bool val = false;
            lastHResult = SafeNativeMethods.GetThemeBool(new HandleRef(this, Handle), part, state, (int)prop, ref val);
            return val;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public Color GetColor(ColorProperty prop)
        {
            //valid values are 0xed9 to 0xeef
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)ColorProperty.BorderColor, (int)ColorProperty.AccentColorHint))
            {
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(ColorProperty));
            }

            int color = 0;
            lastHResult = SafeNativeMethods.GetThemeColor(new HandleRef(this, Handle), part, state, (int)prop, ref color);
            return ColorTranslator.FromWin32(color);
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public int GetEnumValue(EnumProperty prop)
        {
            //valid values are 0xfa1 to 0xfaf
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)EnumProperty.BackgroundType, (int)EnumProperty.TrueSizeScalingType))
            {
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(EnumProperty));
            }

            int val = 0;
            lastHResult = SafeNativeMethods.GetThemeEnumValue(new HandleRef(this, Handle), part, state, (int)prop, ref val);
            return val;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public string GetFilename(FilenameProperty prop)
        {
            //valid values are 0xbb9 to 0xbc0
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)FilenameProperty.ImageFile, (int)FilenameProperty.GlyphImageFile))
            {
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(FilenameProperty));
            }

            StringBuilder filename = new StringBuilder(512);
            lastHResult = SafeNativeMethods.GetThemeFilename(new HandleRef(this, Handle), part, state, (int)prop, filename, filename.Capacity);
            return filename.ToString();
        }

        /// <summary>
        ///  [See win32 equivalent.]
        ///  Returns null if the returned font was not true type, since GDI+ does not support it.
        /// </summary>
        public Font GetFont(IDeviceContext dc, FontProperty prop)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            //valid values are 0xa29 to 0xa29
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)FontProperty.GlyphFont, (int)FontProperty.GlyphFont))
            {
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(FontProperty));
            }

            NativeMethods.LOGFONTW logfont = new NativeMethods.LOGFONTW();

            using (WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper(dc, AllGraphicsProperties))
            {
                HandleRef hdc = new HandleRef(wgr, wgr.WindowsGraphics.DeviceContext.Hdc);
                lastHResult = SafeNativeMethods.GetThemeFont(new HandleRef(this, Handle), hdc, part, state, (int)prop, ref logfont);
            }

            Font font = null;

            //check for a failed HR.
            if (NativeMethods.Succeeded(lastHResult))
            {
                try
                {
                    font = Font.FromLogFont(logfont);
                }
                catch (Exception e)
                {
                    if (ClientUtils.IsSecurityOrCriticalException(e))
                    {
                        throw;
                    }

                    //Looks like the font was not true type
                    font = null;
                }
            }

            return font;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public int GetInteger(IntegerProperty prop)
        {
            //valid values are 0x961 to 0x978
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)IntegerProperty.ImageCount, (int)IntegerProperty.MinDpi5))
            {
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(IntegerProperty));
            }

            int val = 0;
            lastHResult = SafeNativeMethods.GetThemeInt(new HandleRef(this, Handle), part, state, (int)prop, ref val);
            return val;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public Size GetPartSize(IDeviceContext dc, ThemeSizeType type)
        {
            return GetPartSize(dc, type, IntPtr.Zero);
        }

        internal Size GetPartSize(IDeviceContext dc, ThemeSizeType type, IntPtr hWnd)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            // valid values are 0x0 to 0x2
            if (!ClientUtils.IsEnumValid(type, (int)type, (int)ThemeSizeType.Minimum, (int)ThemeSizeType.Draw))
            {
                throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(ThemeSizeType));
            }

            using (WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper(dc, AllGraphicsProperties))
            {
                HandleRef hdc = new HandleRef(wgr, wgr.WindowsGraphics.DeviceContext.Hdc);
                if (DpiHelper.IsPerMonitorV2Awareness && (IntPtr.Zero != hWnd))
                {
                    using (ThemeHandle hTheme = ThemeHandle.Create(_class, true, new HandleRef(null, hWnd)))
                    {
                        lastHResult = SafeNativeMethods.GetThemePartSize(new HandleRef(this, hTheme.NativeHandle), hdc, part, state, null, type, out Size size);
                        return size;
                    }
                }
                else
                {
                    lastHResult = SafeNativeMethods.GetThemePartSize(new HandleRef(this, Handle), hdc, part, state, null, type, out Size size);
                    return size;
                }
            }
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public Size GetPartSize(IDeviceContext dc, Rectangle bounds, ThemeSizeType type)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            //valid values are 0x0 to 0x2
            if (!ClientUtils.IsEnumValid(type, (int)type, (int)ThemeSizeType.Minimum, (int)ThemeSizeType.Draw))
            {
                throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(ThemeSizeType));
            }

            using (WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper(dc, AllGraphicsProperties))
            {
                HandleRef hdc = new HandleRef(wgr, wgr.WindowsGraphics.DeviceContext.Hdc);
                lastHResult = SafeNativeMethods.GetThemePartSize(new HandleRef(this, Handle), hdc, part, state, new NativeMethods.COMRECT(bounds), type, out Size size);
                return size;
            }
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public Point GetPoint(PointProperty prop)
        {
            //valid values are 0xd49 to 0xd50
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)PointProperty.Offset, (int)PointProperty.MinSize5))
            {
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(PointProperty));
            }

            lastHResult = SafeNativeMethods.GetThemePosition(new HandleRef(this, Handle), part, state, (int)prop, out Point point);
            return point;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public Padding GetMargins(IDeviceContext dc, MarginProperty prop)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            //valid values are 0xe11 to 0xe13
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)MarginProperty.SizingMargins, (int)MarginProperty.CaptionMargins))
            {
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(MarginProperty));
            }

            NativeMethods.MARGINS margins = new NativeMethods.MARGINS();

            using (WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper(dc, AllGraphicsProperties))
            {
                HandleRef hdc = new HandleRef(wgr, wgr.WindowsGraphics.DeviceContext.Hdc);
                lastHResult = SafeNativeMethods.GetThemeMargins(new HandleRef(this, Handle), hdc, part, state, (int)prop, prc: null, ref margins);
            }

            return new Padding(margins.cxLeftWidth, margins.cyTopHeight, margins.cxRightWidth, margins.cyBottomHeight);
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public string GetString(StringProperty prop)
        {
            //valid values are 0xc81 to 0xc81
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)StringProperty.Text, (int)StringProperty.Text))
            {
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(StringProperty));
            }

            StringBuilder aString = new StringBuilder(512);
            lastHResult = SafeNativeMethods.GetThemeString(new HandleRef(this, Handle), part, state, (int)prop, aString, aString.Capacity);
            return aString.ToString();
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public Rectangle GetTextExtent(IDeviceContext dc, string textToDraw, TextFormatFlags flags)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            if (string.IsNullOrEmpty(textToDraw))
            {
                throw new ArgumentNullException(nameof(textToDraw));
            }

            NativeMethods.COMRECT rect = new NativeMethods.COMRECT();

            using (WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper(dc, AllGraphicsProperties))
            {
                HandleRef hdc = new HandleRef(wgr, wgr.WindowsGraphics.DeviceContext.Hdc);
                lastHResult = SafeNativeMethods.GetThemeTextExtent(new HandleRef(this, Handle), hdc, part, state, textToDraw, textToDraw.Length, (int)flags, null, rect);
            }

            return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public Rectangle GetTextExtent(IDeviceContext dc, Rectangle bounds, string textToDraw, TextFormatFlags flags)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            if (string.IsNullOrEmpty(textToDraw))
            {
                throw new ArgumentNullException(nameof(textToDraw));
            }

            NativeMethods.COMRECT rect = new NativeMethods.COMRECT();

            using (WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper(dc, AllGraphicsProperties))
            {
                HandleRef hdc = new HandleRef(wgr, wgr.WindowsGraphics.DeviceContext.Hdc);
                lastHResult = SafeNativeMethods.GetThemeTextExtent(new HandleRef(this, Handle), hdc, part, state, textToDraw, textToDraw.Length, (int)flags, new NativeMethods.COMRECT(bounds), rect);
            }

            return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public TextMetrics GetTextMetrics(IDeviceContext dc)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            TextMetrics tm = new TextMetrics();

            using (WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper(dc, AllGraphicsProperties))
            {
                HandleRef hdc = new HandleRef(wgr, wgr.WindowsGraphics.DeviceContext.Hdc);
                lastHResult = SafeNativeMethods.GetThemeTextMetrics(new HandleRef(this, Handle), hdc, part, state, ref tm);
            }

            return tm;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public HitTestCode HitTestBackground(IDeviceContext dc, Rectangle backgroundRectangle, Point pt, HitTestOptions options)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            ushort htCode = 0;
            using (WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper(dc, AllGraphicsProperties))
            {
                HandleRef hdc = new HandleRef(wgr, wgr.WindowsGraphics.DeviceContext.Hdc);
                lastHResult = SafeNativeMethods.HitTestThemeBackground(new HandleRef(this, Handle), hdc, part, state, (int)options, new NativeMethods.COMRECT(backgroundRectangle), NativeMethods.NullHandleRef, pt, ref htCode);
            }

            return (HitTestCode)htCode;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public HitTestCode HitTestBackground(Graphics g, Rectangle backgroundRectangle, Region region, Point pt, HitTestOptions options)
        {
            if (g == null)
            {
                throw new ArgumentNullException(nameof(g));
            }

            IntPtr hRgn = region.GetHrgn(g);

            return HitTestBackground(g, backgroundRectangle, hRgn, pt, options);
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public HitTestCode HitTestBackground(IDeviceContext dc, Rectangle backgroundRectangle, IntPtr hRgn, Point pt, HitTestOptions options)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            ushort htCode = 0;
            using (WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper(dc, AllGraphicsProperties))
            {
                HandleRef hdc = new HandleRef(wgr, wgr.WindowsGraphics.DeviceContext.Hdc);
                lastHResult = SafeNativeMethods.HitTestThemeBackground(new HandleRef(this, Handle), hdc, part, state, (int)options, new NativeMethods.COMRECT(backgroundRectangle), new HandleRef(this, hRgn), pt, ref htCode);
            }

            return (HitTestCode)htCode;
        }

        /// <summary>
        ///  [See win32 equivalent.]
        /// </summary>
        public bool IsBackgroundPartiallyTransparent()
        {
            return (SafeNativeMethods.IsThemeBackgroundPartiallyTransparent(new HandleRef(this, Handle), part, state));
        }

        /// <summary>
        ///  This is similar to GetLastError in Win32.  It returns the last HRESULT returned from a native call
        ///  into theme apis.  We eat the errors and let the user handle any errors that occurred.
        /// </summary>
        public int LastHResult
        {
            get
            {
                return lastHResult;
            }
        }

        /// <summary>
        ///  Instantiates the ThemeHandle cache hashtable.
        /// </summary>
        private static void CreateThemeHandleHashtable()
        {
            themeHandles = new Hashtable(numberOfPossibleClasses);
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
                globalCacheVersion++;
            }
        }

        /// <summary>
        ///  Refreshes this thread's theme handle cache.
        /// </summary>
        private static void RefreshCache()
        {
            ThemeHandle tHandle = null;

            if (themeHandles != null)
            {
                string[] classNames = new string[themeHandles.Keys.Count];
                themeHandles.Keys.CopyTo(classNames, 0);

                foreach (string className in classNames)
                {
                    tHandle = (ThemeHandle)themeHandles[className];
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
                            themeHandles[className] = tHandle;
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
            ThemeHandle tHandle;

            if (themeHandles == null)
            {
                CreateThemeHandleHashtable();
            }

            if (threadCacheVersion != globalCacheVersion)
            {
                RefreshCache();
                threadCacheVersion = globalCacheVersion;
            }

            if (!themeHandles.Contains(className))
            { // see if it is already in cache
                tHandle = ThemeHandle.Create(className, throwExceptionOnFail);
                if (tHandle == null)
                {
                    return IntPtr.Zero;
                }
                themeHandles.Add(className, tHandle);
            }
            else
            {
                tHandle = (ThemeHandle)themeHandles[className];
            }

            return tHandle.NativeHandle;
        }

        // This wrapper class is needed for safely cleaning up TLS cache of handles.
        private class ThemeHandle : IDisposable
        {
            private IntPtr _hTheme = IntPtr.Zero;

            private ThemeHandle(IntPtr hTheme)
            {
                _hTheme = hTheme;
            }

            public IntPtr NativeHandle
            {
                get
                {
                    return _hTheme;
                }
            }

            public static ThemeHandle Create(string className, bool throwExceptionOnFail)
            {
                return Create(className, throwExceptionOnFail, new HandleRef(null, IntPtr.Zero));
            }

            internal static ThemeHandle Create(string className, bool throwExceptionOnFail, HandleRef hWndRef)
            {
                // HThemes require an HWND when display scaling is different between monitors.
                IntPtr hTheme = IntPtr.Zero;

                try
                {
                    hTheme = SafeNativeMethods.OpenThemeData(hWndRef, className);
                }
                catch (Exception e)
                {
                    //We don't want to eat critical exceptions
                    if (ClientUtils.IsSecurityOrCriticalException(e))
                    {
                        throw;
                    }

                    if (throwExceptionOnFail)
                    {
                        throw new InvalidOperationException(SR.VisualStyleHandleCreationFailed, e);
                    }
                    else
                    {
                        return null;
                    }
                }

                if (hTheme == IntPtr.Zero)
                {
                    if (throwExceptionOnFail)
                    {
                        throw new InvalidOperationException(SR.VisualStyleHandleCreationFailed);
                    }
                    else
                    {
                        return null;
                    }
                }
                return new ThemeHandle(hTheme);
            }

            public void Dispose()
            {
                if (_hTheme != IntPtr.Zero)
                {
                    SafeNativeMethods.CloseThemeData(new HandleRef(null, _hTheme));
                    _hTheme = IntPtr.Zero;
                }
                GC.SuppressFinalize(this);
            }

            ~ThemeHandle()
            {
                Dispose();
            }
        }
    }
}
