// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.MSInternal", "CA905:SystemAndMicrosoftNamespacesRequireApproval", Scope="namespace", Target="System.Windows.Forms.VisualStyles")]

namespace System.Windows.Forms.VisualStyles {
    using System;
    using System.Drawing;
    using System.Windows.Forms.Internal;
    using System.Text;
    using System.Windows.Forms;
    using System.Collections;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Win32;
    using System.Security;
    using System.Security.Permissions;

    /// <include file='doc\visualStyleRenderer.uex' path='docs/doc[@for="visualStyleRenderer"]/*' />
    /// <devdoc>
    ///    <para>
    ///       This class provides full feature parity with UxTheme API.
    ///    </para>
    /// </devdoc>
    public sealed class VisualStyleRenderer {
        private const TextFormatFlags AllGraphicsProperties = TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.PreserveGraphicsTranslateTransform;

        internal const int EdgeAdjust = 0x2000; //used with Edges in VisualStyleRenderer.DrawThemeEdge
        private string _class;
        private int part;
        private int state;
        private int lastHResult = 0;
        private static int numberOfPossibleClasses = VisualStyleElement.Count; //used as size for themeHandles
        
        [ThreadStatic]
        private static Hashtable themeHandles = null; //per-thread cache of ThemeHandle objects.
        [ThreadStatic]
        private static long threadCacheVersion = 0; 

        private static long globalCacheVersion = 0;

        static VisualStyleRenderer() {
            SystemEvents.UserPreferenceChanging += new UserPreferenceChangingEventHandler(OnUserPreferenceChanging);
        }


        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.IsSupported"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Returns true if visual styles are 1) supported by the OS 2) enabled in the client area 
        ///       and 3) currently applied to this application. Otherwise, it returns false. Note that
        ///       if it returns false, attempting to instantiate/use objects of this class 
        ///       will result in exceptions being thrown.
        ///    </para>
        /// </devdoc>
        public static bool IsSupported {
            get {
                bool supported =  (VisualStyleInformation.IsEnabledByUser &&
                   (Application.VisualStyleState == VisualStyleState.ClientAreaEnabled ||
                    Application.VisualStyleState == VisualStyleState.ClientAndNonClientAreasEnabled));

                
                if (supported) {
                    // In some cases, this check isn't enough, since the theme handle creation
                    // could fail for some other reason. Try creating a theme handle here - if successful, return true,
                    // else return false.
                    IntPtr hTheme = GetHandle("BUTTON", false); //Button is an arbitrary choice.
                    supported = (hTheme != IntPtr.Zero);
                }

                return supported;

            }
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.IsCombinationDefined"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Returns true if the element is defined by the current visual style, else false. 
        ///       Note: 
        ///          1) Throws an exception if IsSupported is false, since it is illegal to call it in that case.
        ///          2) The underlying API does not validate states. So if you pass in invalid state values,
        ///             we might still return true. When you use an invalid state to render, you get the default
        ///             state instead.
        ///    </para>
        /// </devdoc>
        public static bool IsElementDefined(VisualStyleElement element) {
            if (element == null) {
                throw new ArgumentNullException(nameof(element));
            }

            return IsCombinationDefined(element.ClassName, element.Part);
        }

        internal static bool IsCombinationDefined(string className, int part) {
            bool returnVal = false;

            if (!IsSupported) {
                if (!VisualStyleInformation.IsEnabledByUser) {
                    throw new InvalidOperationException(SR.VisualStyleNotActive);
                }
                else {
                    throw new InvalidOperationException(SR.VisualStylesDisabledInClientArea);
                }
            }

            if (className == null) {
                throw new ArgumentNullException(nameof(className));
            }

            IntPtr hTheme = GetHandle(className, false);

            if (hTheme != IntPtr.Zero) {
                // IsThemePartDefined doesn't work for part = 0, although there are valid parts numbered 0. We
                // allow these explicitly here.
                if (part == 0) {
                    returnVal = true;
                }
                else {
                    returnVal = SafeNativeMethods.IsThemePartDefined(new HandleRef(null, hTheme), part, 0);
                }
            }

            //if the combo isn't defined, check the validity of our theme handle cache
            if (!returnVal) {
                using (ThemeHandle tHandle = ThemeHandle.Create(className, false)) {
                    if (tHandle != null) {
                        returnVal = SafeNativeMethods.IsThemePartDefined(new HandleRef(null, tHandle.NativeHandle), part, 0);
                    }

                    //if we did, in fact get a new correct theme handle, our cache is out of date -- update it now.
                    if (returnVal) {
                        RefreshCache();
                    }
                }
            }

            return returnVal;
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.VisualStyleRenderer"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Constructor takes a VisualStyleElement.
        ///    </para>
        /// </devdoc>
        public VisualStyleRenderer(VisualStyleElement element) : this(element.ClassName, element.Part, element.State) {
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.VisualStyleRenderer1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Constructor takes weakly typed parameters - left for extensibility (using classes, parts or states
        ///       not defined in the VisualStyleElement class.)
        ///    </para>
        /// </devdoc>
        public VisualStyleRenderer(string className, int part, int state) {
            if (!IsCombinationDefined(className, part)) { //internally this call takes care of IsSupported. 
                throw new ArgumentException(SR.VisualStylesInvalidCombination);
            }

            this._class = className;
            this.part = part;
            this.state = state;
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.Class"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Returns the current _class. Use SetParameters to set.
        ///    </para>
        /// </devdoc>
        public string Class {
            get {
                return _class;
            }
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.Part"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Returns the current part. Use SetParameters to set.
        ///    </para>
        /// </devdoc>
        public int Part {
            get {
                return part;
            }
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.State"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Returns the current state. Use SetParameters to set.
        ///    </para>
        /// </devdoc>
        public int State {
            get {
                return state;
            }
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.Handle"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Returns the underlying HTheme handle.
        ///       NOTE: The handle gets invalidated when the theme changes or the user disables theming. When that
        ///             happens, the user should requery this property to get the correct handle. To know when the
        ///             theme changed, hook on to SystemEvents.UserPreferenceChanged and look for ThemeChanged 
        ///             category.
        ///    </para>
        /// </devdoc>
        public IntPtr Handle {
            get {
                if (!IsSupported) {
                    if (!VisualStyleInformation.IsEnabledByUser) {
                        throw new InvalidOperationException(SR.VisualStyleNotActive); 
                    }
                    else {
                        throw new InvalidOperationException(SR.VisualStylesDisabledInClientArea);
                    }
                }
        
                return GetHandle(_class);
            }
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.SetParameters"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Used to set a new VisualStyleElement on this VisualStyleRenderer instance.
        ///    </para>
        /// </devdoc>
        public void SetParameters(VisualStyleElement element) {
            if (element == null) {
                throw new ArgumentNullException(nameof(element));
            }

            SetParameters(element.ClassName, element.Part, element.State);
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.SetParameters"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Used to set the _class, part and state that the VisualStyleRenderer object references. 
        ///       These parameters cannot be set individually. 
        ///       This method is present for extensibility.
        ///    </para>
        /// </devdoc>
        public void SetParameters(string className, int part, int state) {
            if (!IsCombinationDefined(className, part)) { //internally this call takes care of IsSupported.
                throw new ArgumentException(SR.VisualStylesInvalidCombination); 
            }

            this._class = className;
            this.part = part;
            this.state = state;
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.DrawBackground"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public void DrawBackground(IDeviceContext dc, Rectangle bounds) {
            DrawBackground(dc, bounds, IntPtr.Zero);
        }

        internal void DrawBackground(IDeviceContext dc, Rectangle bounds, IntPtr hWnd) {
            if (dc == null) {
                throw new ArgumentNullException(nameof(dc));
            }
            if (bounds.Width < 0 || bounds.Height < 0) {
                return;
            }

            using( WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, AllGraphicsProperties ) ){
                HandleRef hdc = new HandleRef(wgr, wgr.WindowsGraphics.DeviceContext.Hdc);
                if (IntPtr.Zero != hWnd) {
                    using (ThemeHandle hTheme = ThemeHandle.Create(_class, true, new HandleRef(null, hWnd))) {
                        lastHResult = SafeNativeMethods.DrawThemeBackground(new HandleRef(this, hTheme.NativeHandle), hdc, part, state, new NativeMethods.COMRECT(bounds), null);
                    }
                } 
                else {
                    lastHResult = SafeNativeMethods.DrawThemeBackground(new HandleRef(this, Handle), hdc, part, state, new NativeMethods.COMRECT(bounds), null);
                }
            }
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.DrawBackground1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public void DrawBackground(IDeviceContext dc, Rectangle bounds, Rectangle clipRectangle) {
            DrawBackground(dc, bounds, clipRectangle, IntPtr.Zero);
        }

        internal void DrawBackground(IDeviceContext dc, Rectangle bounds, Rectangle clipRectangle, IntPtr hWnd) {
            if( dc == null ){
                throw new ArgumentNullException(nameof(dc));
            }
            if (bounds.Width < 0 || bounds.Height < 0) {
                return;
            }
            if (clipRectangle.Width < 0 || clipRectangle.Height < 0) {
                return;
            }

            using( WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, AllGraphicsProperties ) ) {
                HandleRef hdc = new HandleRef( wgr, wgr.WindowsGraphics.DeviceContext.Hdc );
                if (IntPtr.Zero != hWnd) {
                    using (ThemeHandle hTheme = ThemeHandle.Create(_class, true, new HandleRef(null, hWnd))) {
                        lastHResult = SafeNativeMethods.DrawThemeBackground(new HandleRef(this, hTheme.NativeHandle), hdc, part, state, new NativeMethods.COMRECT(bounds), new NativeMethods.COMRECT(clipRectangle));
                    }
                } 
                else {
                    lastHResult = SafeNativeMethods.DrawThemeBackground(new HandleRef(this, Handle), hdc, part, state, new NativeMethods.COMRECT(bounds), new NativeMethods.COMRECT(clipRectangle));
                }
            }
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.DrawEdge"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public Rectangle DrawEdge(IDeviceContext dc, Rectangle bounds, Edges edges, EdgeStyle style, EdgeEffects effects) {
            if (dc == null) {
                throw new ArgumentNullException(nameof(dc));
            }

            if (!ClientUtils.IsEnumValid_Masked(edges, (int)edges,(UInt32)(Edges.Left | Edges.Top | Edges.Right | Edges.Bottom | Edges.Diagonal))) {
                throw new InvalidEnumArgumentException(nameof(edges), (int)edges, typeof(Edges));
            }

            if (!ClientUtils.IsEnumValid_NotSequential(style, (int)style, (int)EdgeStyle.Raised,(int)EdgeStyle.Sunken,(int)EdgeStyle.Etched,(int)EdgeStyle.Bump )) {
                throw new InvalidEnumArgumentException(nameof(style), (int)style, typeof(EdgeStyle));
            }

            if (!ClientUtils.IsEnumValid_Masked(effects, (int)effects, (UInt32)(EdgeEffects.FillInterior | EdgeEffects.Flat | EdgeEffects.Soft | EdgeEffects.Mono))) {
                throw new InvalidEnumArgumentException(nameof(effects), (int)effects, typeof(EdgeEffects));
            }

            NativeMethods.COMRECT rect = new NativeMethods.COMRECT();

            using( WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, AllGraphicsProperties ) ) {
                HandleRef hdc = new HandleRef( wgr, wgr.WindowsGraphics.DeviceContext.Hdc );
                lastHResult = SafeNativeMethods.DrawThemeEdge( new HandleRef( this, Handle ), hdc, part, state, new NativeMethods.COMRECT( bounds ), (int) style, (int) edges | (int) effects | EdgeAdjust, rect );
            }

            return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.DrawImage"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///       This method uses Graphics.DrawImage as a backup if themed drawing does not work.
        ///    </para>
        /// </devdoc>
        public void DrawImage(Graphics g, Rectangle bounds, Image image) {
            if (g == null) {
                throw new ArgumentNullException(nameof(g));
            }

            if (image == null) {
                throw new ArgumentNullException(nameof(image));
            }

            if (bounds.Width < 0 || bounds.Height < 0) {
                return;
            }

            g.DrawImage(image, bounds);
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.DrawImage1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.] 
        ///       This method uses Graphics.DrawImage as a backup if themed drawing does not work.
        ///    </para>
        /// </devdoc>
        public void DrawImage(Graphics g, Rectangle bounds, ImageList imageList, int imageIndex) {
            if (g == null) {
                throw new ArgumentNullException(nameof(g));
            }

            if (imageList == null) {
                throw new ArgumentNullException(nameof(imageList));
            }
            
            if (imageIndex < 0 || imageIndex >= imageList.Images.Count) {
                throw new ArgumentOutOfRangeException(nameof(imageIndex), string.Format(SR.InvalidArgument, "imageIndex", imageIndex.ToString(CultureInfo.CurrentCulture)));
            }

            if (bounds.Width < 0 || bounds.Height < 0) {
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

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.DrawParentBackground"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Given a graphics object and bounds to draw in, this method effectively asks the passed in 
        ///       control's parent to draw itself in there (it sends WM_ERASEBKGND & WM_PRINTCLIENT messages
        ///       to the parent).
        ///    </para>
        /// </devdoc>
        public void DrawParentBackground(IDeviceContext dc, Rectangle bounds, Control childControl) {
            if (dc == null) {
                throw new ArgumentNullException(nameof(dc));
            }

            if (childControl == null) {
                throw new ArgumentNullException(nameof(childControl));
            }

            if (bounds.Width < 0 || bounds.Height < 0) {
                return;
            }

            if (childControl.Handle != IntPtr.Zero) {
                using( WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, AllGraphicsProperties ) ) {
                    HandleRef hdc = new HandleRef( wgr, wgr.WindowsGraphics.DeviceContext.Hdc );
                    lastHResult = SafeNativeMethods.DrawThemeParentBackground( new HandleRef( this, childControl.Handle ), hdc, new NativeMethods.COMRECT( bounds ) );
                }
            }
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.DrawText"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public void DrawText(IDeviceContext dc, Rectangle bounds, string textToDraw) {
            DrawText(dc, bounds, textToDraw, false);
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.DrawText1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public void DrawText(IDeviceContext dc, Rectangle bounds, string textToDraw, bool drawDisabled) {
            DrawText(dc, bounds, textToDraw, drawDisabled, TextFormatFlags.HorizontalCenter); 
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.DrawText2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public void DrawText(IDeviceContext dc, Rectangle bounds, string textToDraw, bool drawDisabled, TextFormatFlags flags) {
            if( dc == null ){
                throw new ArgumentNullException(nameof(dc));
            }

            if (bounds.Width < 0 || bounds.Height < 0) {
                return;
            }
            
            int disableFlag = drawDisabled?0x1:0;
            
            if (!String.IsNullOrEmpty(textToDraw)) {
                using( WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, AllGraphicsProperties ) ) {
                    HandleRef hdc = new HandleRef( wgr, wgr.WindowsGraphics.DeviceContext.Hdc );
                    lastHResult = SafeNativeMethods.DrawThemeText( new HandleRef( this, Handle ), hdc, part, state, textToDraw, textToDraw.Length, (int) flags, disableFlag, new NativeMethods.COMRECT( bounds ) );
                }
            }
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.GetBackgroundContentRectangle"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public Rectangle GetBackgroundContentRectangle(IDeviceContext dc, Rectangle bounds) {
            if( dc == null ){
                throw new ArgumentNullException(nameof(dc));
            }
            if (bounds.Width < 0 || bounds.Height < 0) {
                return Rectangle.Empty;
            }
            
            NativeMethods.COMRECT rect = new NativeMethods.COMRECT();

            using( WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, AllGraphicsProperties ) ) {
                HandleRef hdc = new HandleRef( wgr, wgr.WindowsGraphics.DeviceContext.Hdc );
                lastHResult = SafeNativeMethods.GetThemeBackgroundContentRect( new HandleRef( this, Handle ), hdc, part, state, new NativeMethods.COMRECT( bounds ), rect );
            }

            return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.GetBackgroundExtent"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public Rectangle GetBackgroundExtent(IDeviceContext dc, Rectangle contentBounds) {
            if( dc == null ){
                throw new ArgumentNullException(nameof(dc));
            }
            if (contentBounds.Width < 0 || contentBounds.Height < 0) {
                return Rectangle.Empty;
            }
            
            NativeMethods.COMRECT rect = new NativeMethods.COMRECT();

            using( WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, AllGraphicsProperties ) ) {
                HandleRef hdc = new HandleRef( wgr, wgr.WindowsGraphics.DeviceContext.Hdc );
                lastHResult = SafeNativeMethods.GetThemeBackgroundExtent( new HandleRef( this, Handle ), hdc, part, state, new NativeMethods.COMRECT( contentBounds ), rect );
            }

            return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.GetBackgroundRegion"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Computes the region for a regular or partially transparent background that is bounded by a specified 
        ///       rectangle. Return null if the region cannot be created.
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        [SuppressUnmanagedCodeSecurity, 
         SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
        public Region GetBackgroundRegion(IDeviceContext dc, Rectangle bounds) {
            if (dc == null) {
                throw new ArgumentNullException(nameof(dc));
            }
            if (bounds.Width < 0 || bounds.Height < 0) {
                return null;
            }

            IntPtr hRegion = IntPtr.Zero;

            using( WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, AllGraphicsProperties ) ) {
                HandleRef hdc = new HandleRef( wgr, wgr.WindowsGraphics.DeviceContext.Hdc );
                lastHResult = SafeNativeMethods.GetThemeBackgroundRegion( new HandleRef( this, Handle ), hdc, part, state, new NativeMethods.COMRECT( bounds ), ref hRegion );
            }

            // GetThemeBackgroundRegion returns a null hRegion if it fails to create one, it could be because the bounding
            // box is too big. For more info see code in %xpsrc%\shell\themes\uxtheme\imagefile.cpp if you have an enlistment to it.

		if (hRegion == IntPtr.Zero) {
			return null;
		}

		// From the GDI+ sources it doesn't appear as if they take ownership of the hRegion, so this is safe to do.
		// We need to DeleteObject in order to not leak.
		Region region = Region.FromHrgn(hRegion);
		SafeNativeMethods.ExternalDeleteObject(new HandleRef(null, hRegion));
		return region;

        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.GetBoolean"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public bool GetBoolean(BooleanProperty prop) {
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)BooleanProperty.Transparent, (int)BooleanProperty.SourceShrink)){
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(BooleanProperty));
            }

            bool val = false;
            lastHResult = SafeNativeMethods.GetThemeBool(new HandleRef(this, Handle), part, state, (int)prop, ref val);
            return val;
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.GetColor"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public Color GetColor(ColorProperty prop) {
            //valid values are 0xed9 to 0xeef
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)ColorProperty.BorderColor, (int)ColorProperty.AccentColorHint))
            {
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(ColorProperty));
            }

            int color = 0;
            lastHResult = SafeNativeMethods.GetThemeColor(new HandleRef(this, Handle), part, state, (int)prop, ref color);
            return ColorTranslator.FromWin32(color);
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.GetEnumValue"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public int GetEnumValue(EnumProperty prop) {
            //valid values are 0xfa1 to 0xfaf
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)EnumProperty.BackgroundType, (int)EnumProperty.TrueSizeScalingType))
            {
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(EnumProperty));
            }

            int val = 0;
            lastHResult = SafeNativeMethods.GetThemeEnumValue(new HandleRef(this, Handle), part, state, (int)prop, ref val);
            return val;
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.GetFilename"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public string GetFilename(FilenameProperty prop) {
            //valid values are 0xbb9 to 0xbc0
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)FilenameProperty.ImageFile, (int)FilenameProperty.GlyphImageFile))
            {
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(FilenameProperty));
            }

            StringBuilder filename = new StringBuilder(512);
            lastHResult = SafeNativeMethods.GetThemeFilename(new HandleRef(this, Handle), part, state, (int)prop, filename, filename.Capacity);
            return filename.ToString();
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.GetFont"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///       Returns null if the returned font was not true type, since GDI+ does not support it.
        ///    </para>
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2102:CatchNonClsCompliantExceptionsInGeneralHandlers")]
        public Font GetFont(IDeviceContext dc, FontProperty prop)
        {
            if( dc == null ){
                throw new ArgumentNullException(nameof(dc));
            }
            
            //valid values are 0xa29 to 0xa29
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)FontProperty.GlyphFont, (int)FontProperty.GlyphFont))
            {
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(FontProperty));
            }

            NativeMethods.LOGFONT logfont = new NativeMethods.LOGFONT();

            using( WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, AllGraphicsProperties ) ) {
                HandleRef hdc = new HandleRef( wgr, wgr.WindowsGraphics.DeviceContext.Hdc );
                lastHResult = SafeNativeMethods.GetThemeFont( new HandleRef( this, Handle ), hdc, part, state, (int) prop, logfont );
            }

            Font font = null;

            //check for a failed HR.
            if (NativeMethods.Succeeded(lastHResult)) {


                // 


                IntSecurity.ObjectFromWin32Handle.Assert();
                try {
                    font = Font.FromLogFont(logfont);
                }
                catch (Exception e) {
                    if (ClientUtils.IsSecurityOrCriticalException(e)) {
                        throw;
                    }

                    //Looks like the font was not true type
                    font = null;
                }
            }

            return font;
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.GetInteger"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public int GetInteger(IntegerProperty prop) {
            //valid values are 0x961 to 0x978
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)IntegerProperty.ImageCount, (int)IntegerProperty.MinDpi5))
            {
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(IntegerProperty));
            }

            int val = 0;
            lastHResult = SafeNativeMethods.GetThemeInt(new HandleRef(this, Handle), part, state, (int)prop, ref val);
            return val;
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.GetPartSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public Size GetPartSize(IDeviceContext dc, ThemeSizeType type) {
            return GetPartSize(dc, type, IntPtr.Zero);
        }

        internal Size GetPartSize(IDeviceContext dc, ThemeSizeType type, IntPtr hWnd) {
            if (dc == null) {
                throw new ArgumentNullException(nameof(dc));
            }
            
            // valid values are 0x0 to 0x2
            if (!ClientUtils.IsEnumValid(type, (int)type, (int)ThemeSizeType.Minimum, (int)ThemeSizeType.Draw)) {
                throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(ThemeSizeType));
            }

            NativeMethods.SIZE size = new NativeMethods.SIZE();

            using (WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, AllGraphicsProperties)) {
                HandleRef hdc = new HandleRef( wgr, wgr.WindowsGraphics.DeviceContext.Hdc );
                if (DpiHelper.IsPerMonitorV2Awareness && (IntPtr.Zero != hWnd)) {
                    using (ThemeHandle hTheme = ThemeHandle.Create(_class, true, new HandleRef(null, hWnd))) {
                        lastHResult = SafeNativeMethods.GetThemePartSize(new HandleRef(this, hTheme.NativeHandle), hdc, part, state, null, type, size);
                    }
                }
                else {
                    lastHResult = SafeNativeMethods.GetThemePartSize(new HandleRef(this, Handle), hdc, part, state, null, type, size);
                }
            }

            return new Size(size.cx, size.cy);
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.GetPartSize1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public Size GetPartSize(IDeviceContext dc, Rectangle bounds, ThemeSizeType type) {
            if( dc == null ){
                throw new ArgumentNullException(nameof(dc));
            }
            
            //valid values are 0x0 to 0x2
            if (!ClientUtils.IsEnumValid(type, (int)type, (int)ThemeSizeType.Minimum, (int)ThemeSizeType.Draw))
            {
                throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(ThemeSizeType));
            }

            NativeMethods.SIZE size = new NativeMethods.SIZE();

            using( WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, AllGraphicsProperties ) ) {
                HandleRef hdc = new HandleRef( wgr, wgr.WindowsGraphics.DeviceContext.Hdc );
                lastHResult = SafeNativeMethods.GetThemePartSize( new HandleRef( this, Handle ), hdc, part, state, new NativeMethods.COMRECT( bounds ), type, size );
            }

            return new Size(size.cx, size.cy);
        } 

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.GetPoint"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public Point GetPoint(PointProperty prop) {
            //valid values are 0xd49 to 0xd50
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)PointProperty.Offset, (int)PointProperty.MinSize5))
            {
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(PointProperty));
            }

            NativeMethods.POINT point = new NativeMethods.POINT();
            lastHResult = SafeNativeMethods.GetThemePosition(new HandleRef(this, Handle), part, state, (int)prop, point);
            return new Point(point.x, point.y);
        }        
        
        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.GetMargins"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public Padding GetMargins(IDeviceContext dc, MarginProperty prop) {
            if( dc == null ){
                throw new ArgumentNullException(nameof(dc));
            }
            
            //valid values are 0xe11 to 0xe13
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)MarginProperty.SizingMargins, (int)MarginProperty.CaptionMargins))
            {
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(MarginProperty));
            }

            NativeMethods.MARGINS margins = new NativeMethods.MARGINS();

            using( WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, AllGraphicsProperties ) ) {
                HandleRef hdc = new HandleRef( wgr, wgr.WindowsGraphics.DeviceContext.Hdc );
                lastHResult = SafeNativeMethods.GetThemeMargins( new HandleRef( this, Handle ), hdc, part, state, (int) prop, ref margins );
            }

            return new Padding(margins.cxLeftWidth, margins.cyTopHeight, margins.cxRightWidth, margins.cyBottomHeight);
        }
        

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.GetString"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public string GetString(StringProperty prop) {
            //valid values are 0xc81 to 0xc81
            if (!ClientUtils.IsEnumValid(prop, (int)prop, (int)StringProperty.Text, (int)StringProperty.Text))
            {
                throw new InvalidEnumArgumentException(nameof(prop), (int)prop, typeof(StringProperty));
            }

            StringBuilder aString = new StringBuilder(512);
            lastHResult = SafeNativeMethods.GetThemeString(new HandleRef(this, Handle), part, state, (int)prop, aString, aString.Capacity);
            return aString.ToString();
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.GetTextExtent"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public Rectangle GetTextExtent(IDeviceContext dc, string textToDraw, TextFormatFlags flags) {
            if( dc == null ){
                throw new ArgumentNullException(nameof(dc));
            }

            if (String.IsNullOrEmpty(textToDraw)) {
                throw new ArgumentNullException(nameof(textToDraw));
            }

            NativeMethods.COMRECT rect = new NativeMethods.COMRECT();

            using( WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, AllGraphicsProperties ) ) {
                HandleRef hdc = new HandleRef( wgr, wgr.WindowsGraphics.DeviceContext.Hdc );
                lastHResult = SafeNativeMethods.GetThemeTextExtent( new HandleRef( this, Handle ), hdc, part, state, textToDraw, textToDraw.Length, (int) flags, null, rect );
            }

            return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.GetTextExtent1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public Rectangle GetTextExtent(IDeviceContext dc, Rectangle bounds, string textToDraw, TextFormatFlags flags) {
            if( dc == null ){
                throw new ArgumentNullException(nameof(dc));
            }
            
            if (String.IsNullOrEmpty(textToDraw)) {
                throw new ArgumentNullException(nameof(textToDraw));
            }

            NativeMethods.COMRECT rect = new NativeMethods.COMRECT();

            using( WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, AllGraphicsProperties ) ) {
                HandleRef hdc = new HandleRef( wgr, wgr.WindowsGraphics.DeviceContext.Hdc );
                lastHResult = SafeNativeMethods.GetThemeTextExtent( new HandleRef( this, Handle ), hdc, part, state, textToDraw, textToDraw.Length, (int) flags, new NativeMethods.COMRECT( bounds ), rect );
            }

            return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.GetTextMetric"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public TextMetrics GetTextMetrics(IDeviceContext dc) {
            if( dc == null ){
                throw new ArgumentNullException(nameof(dc));
            }
            
            TextMetrics tm = new TextMetrics();

            using( WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, AllGraphicsProperties ) ) {
                HandleRef hdc = new HandleRef( wgr, wgr.WindowsGraphics.DeviceContext.Hdc );
                lastHResult = SafeNativeMethods.GetThemeTextMetrics( new HandleRef( this, Handle ), hdc, part, state, ref tm );
            }

            return tm;
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.HitTestBackground"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public HitTestCode HitTestBackground(IDeviceContext dc, Rectangle backgroundRectangle, Point pt, HitTestOptions options) {
            if( dc == null ){
                throw new ArgumentNullException(nameof(dc));
            }
            
            int htCode = 0;
            NativeMethods.POINTSTRUCT point = new NativeMethods.POINTSTRUCT(pt.X, pt.Y);

            using( WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, AllGraphicsProperties ) ) {
                HandleRef hdc = new HandleRef( wgr, wgr.WindowsGraphics.DeviceContext.Hdc );
                lastHResult = SafeNativeMethods.HitTestThemeBackground( new HandleRef( this, Handle ), hdc, part, state, (int) options, new NativeMethods.COMRECT( backgroundRectangle ), NativeMethods.NullHandleRef, point, ref htCode );
            }

            return (HitTestCode)htCode;
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.HitTestBackground1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public HitTestCode HitTestBackground(Graphics g, Rectangle backgroundRectangle, Region region, Point pt, HitTestOptions options) {
            if (g == null) {
                throw new ArgumentNullException(nameof(g));
            }

            IntPtr hRgn = region.GetHrgn(g);

            return HitTestBackground(g, backgroundRectangle, hRgn, pt, options);
        }


        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.HitTestBackground1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public HitTestCode HitTestBackground(IDeviceContext dc, Rectangle backgroundRectangle, IntPtr hRgn, Point pt, HitTestOptions options) {
            if( dc == null ){
                throw new ArgumentNullException(nameof(dc));
            }
            
            int htCode = 0;
            NativeMethods.POINTSTRUCT point = new NativeMethods.POINTSTRUCT(pt.X, pt.Y);

            using( WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, AllGraphicsProperties ) ) {
                HandleRef hdc = new HandleRef( wgr, wgr.WindowsGraphics.DeviceContext.Hdc );
                lastHResult = SafeNativeMethods.HitTestThemeBackground( new HandleRef( this, Handle ), hdc, part, state, (int) options, new NativeMethods.COMRECT( backgroundRectangle ), new HandleRef( this, hRgn ), point, ref htCode );
            }

            return (HitTestCode)htCode;
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.IsBackgroundPartiallyTransparent"]/*' />
        /// <devdoc>
        ///    <para>
        ///       [See win32 equivalent.]
        ///    </para>
        /// </devdoc>
        public bool IsBackgroundPartiallyTransparent() {
            return (SafeNativeMethods.IsThemeBackgroundPartiallyTransparent(new HandleRef(this, Handle), part, state));
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.GetLastHResult"]/*' />
        /// <devdoc>
        ///  This is similar to GetLastError in Win32.  It returns the last HRESULT returned from a native call
        ///  into theme apis.  We eat the errors and let the user handle any errors that occurred.
        /// </devdoc>
        public int LastHResult {
            get {
                return lastHResult;
            }
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.CreateThemeHandleHashTable"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Instantiates the ThemeHandle cache hashtable.
        ///    </para>
        /// </devdoc>
        private static void CreateThemeHandleHashtable() {
            themeHandles = new Hashtable(numberOfPossibleClasses);
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.OnThemeChanged"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Handles the ThemeChanged event. Basically, we need to ensure all per-thread theme handle
        ///       caches are refreshed.
        ///    </para>
        /// </devdoc>
        private static void OnUserPreferenceChanging(object sender, UserPreferenceChangingEventArgs ea) {
            if (ea.Category == UserPreferenceCategory.VisualStyle) {
                // Let all threads know their cached handles are no longer valid; 
                // cache refresh will happen at next handle access.
                // Note that if the theme changes 2^sizeof(long) times before a thread uses 
                // its handle, this whole version check won't work, but it is unlikely to happen.

                // this is not ideal.
                globalCacheVersion++; 
            }
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.RefreshCache"]/*' />
        /// <devdoc>
        ///    <para>
        ///     Refreshes this thread's theme handle cache.       
        ///    </para>
        /// </devdoc>
        private static void RefreshCache() {
            ThemeHandle tHandle = null;

            if (themeHandles != null) {
                string[] classNames = new string[themeHandles.Keys.Count];
                themeHandles.Keys.CopyTo(classNames, 0);

                // We don't call IsSupported here, since that could cause RefreshCache to be called again,
                // leading to stack overflow.
                bool isSupported = (VisualStyleInformation.IsEnabledByUser &&
                   (Application.VisualStyleState == VisualStyleState.ClientAreaEnabled ||
                    Application.VisualStyleState == VisualStyleState.ClientAndNonClientAreasEnabled));
                
                foreach (string className in classNames) {
                    tHandle = (ThemeHandle) themeHandles[className];
                    if (tHandle != null) {
                        tHandle.Dispose();
                    }

                    if (isSupported) {
                        tHandle = ThemeHandle.Create(className, false);
                        if (tHandle != null) {
                            themeHandles[className] = tHandle;
                        }
                    }
                }
            }
        }

        private static IntPtr GetHandle(string className) {
            return GetHandle(className, true);
        }

        /// <include file='doc\VisualStyleRenderer.uex' path='docs/doc[@for="VisualStyleRenderer.GetHandle"]/*' />
        /// <devdoc>
        ///    <para>
        ///     Retrieves a IntPtr theme handle for the given class from the themeHandle cache. If its not 
        ///     present in the cache, it creates a new ThemeHandle object and stores it there.
        ///    </para>
        /// </devdoc>
        private static IntPtr GetHandle(string className, bool throwExceptionOnFail) {
            ThemeHandle tHandle;

            if (themeHandles == null) {
                CreateThemeHandleHashtable();
            }


            if (threadCacheVersion != globalCacheVersion) {
                RefreshCache();
                threadCacheVersion = globalCacheVersion;
            }

            if (!themeHandles.Contains(className)) { // see if it is already in cache
                tHandle = ThemeHandle.Create(className, throwExceptionOnFail);
                if (tHandle == null) {
                    return IntPtr.Zero;
                }
                themeHandles.Add(className, tHandle);
            }
            else {
                tHandle = (ThemeHandle) themeHandles[className];
            }

            return tHandle.NativeHandle;
        }

        // This wrapper class is needed for safely cleaning up TLS cache of handles.
        private class ThemeHandle : IDisposable {
            private IntPtr _hTheme = IntPtr.Zero;

            private ThemeHandle(IntPtr hTheme) {
                _hTheme = hTheme;
            }            

            public IntPtr NativeHandle {
                get {
                    return _hTheme;
                }
            }

            public static ThemeHandle Create(string className, bool throwExceptionOnFail) {
                return Create(className, throwExceptionOnFail, new HandleRef(null, IntPtr.Zero));
            }

            internal static ThemeHandle Create(string className, bool throwExceptionOnFail, HandleRef hWndRef) {
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

                if (hTheme == IntPtr.Zero) {
                    if (throwExceptionOnFail) {
                        throw new InvalidOperationException(SR.VisualStyleHandleCreationFailed);
                    }
                    else {
                        return null;
                    }
                }
                return new ThemeHandle(hTheme);                                
            }

            public void Dispose() {
                if (_hTheme != IntPtr.Zero) {
                    SafeNativeMethods.CloseThemeData(new HandleRef(null, _hTheme));
                    _hTheme = IntPtr.Zero;
                }
                GC.SuppressFinalize(this);
            }

            ~ThemeHandle() {
                Dispose();
            }
        }
    }
}
