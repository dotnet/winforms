// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.ComponentModel;

    using System.Diagnostics;
    using System;

    /// <include file='doc\ControlStyles.uex' path='docs/doc[@for="ControlStyles"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies control functionality.
    ///       
    ///    </para>
    /// </devdoc>
    [Flags]
    public enum ControlStyles {
        /// <include file='doc\ControlStyles.uex' path='docs/doc[@for="ControlStyles.ContainerControl"]/*' />
        /// <devdoc>
        ///     Indicates whether the control is a container-like control.
        /// </devdoc>
        ContainerControl= 0x00000001,
        /// <include file='doc\ControlStyles.uex' path='docs/doc[@for="ControlStyles.UserPaint"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The control paints itself; WM_PAINT and WM_ERASEBKGND messages are not passed 
        ///       on to the underlying NativeWindow.
        ///    </para>
        ///    <para>
        ///       This style only applies to subclasses of Control.
        ///    </para>
        /// </devdoc>
        UserPaint       = 0x00000002,
        /// <include file='doc\ControlStyles.uex' path='docs/doc[@for="ControlStyles.Opaque"]/*' />
        /// <devdoc>
        ///    <para>
        ///       If specified, a PaintBackground event will not be raised, OnPaintBackground will not be called,
        ///       and Invalidate() will not invalidate the background of the HWND.
        ///    </para>
        /// </devdoc>
        Opaque          = 0x00000004,
        /// <include file='doc\ControlStyles.uex' path='docs/doc[@for="ControlStyles.ResizeRedraw"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The control is completely redrawn when it is resized.
        ///    </para>
        /// </devdoc>
        ResizeRedraw   = 0x00000010,
        /// <include file='doc\ControlStyles.uex' path='docs/doc[@for="ControlStyles.FixedWidth"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The control has a fixed width.
        ///    </para>
        /// </devdoc>
        FixedWidth     = 0x00000020,
        /// <include file='doc\ControlStyles.uex' path='docs/doc[@for="ControlStyles.FixedHeight"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The control has a fixed height.
        ///    </para>
        /// </devdoc>
        FixedHeight    = 0x00000040,
        /// <include file='doc\ControlStyles.uex' path='docs/doc[@for="ControlStyles.StandardClick"]/*' />
        /// <devdoc>
        ///    <para>
        ///        If set, windows forms calls OnClick and raises the Click event when the control is clicked
        ///        (unless it's the second click of a double-click and StandardDoubleClick is specified).
        ///        Regardless of this style, the control may call OnClick directly.
        ///    </para>
        /// </devdoc>
        StandardClick        = 0x00000100,
        /// <include file='doc\ControlStyles.uex' path='docs/doc[@for="ControlStyles.Selectable"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The control can get the focus.
        ///    </para>
        /// </devdoc>
        Selectable = 0x00000200,
        /// <include file='doc\ControlStyles.uex' path='docs/doc[@for="ControlStyles.UserMouse"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The control does its own mouse processing; WM_MOUSEDOWN, WM_MOUSEMOVE, and WM_MOUSEUP messages are not passed 
        ///       on to the underlying NativeWindow.
        ///    </para>
        /// </devdoc>
        UserMouse       = 0x00000400,
        /// <include file='doc\ControlStyles.uex' path='docs/doc[@for="ControlStyles.SupportsTransparentBackColor"]/*' />
        /// <devdoc>
        ///    <para>
        ///       If the BackColor is set to a color whose alpha component is
        ///       less than 255 (i.e., BackColor.A &lt; 255), OnPaintBackground will simulate transparency
        ///       by asking its parent control to paint our background.  This is not true transparency --
        ///       if there is another control between us and our parent, we will not show the control in the middle.
        ///    </para>
        ///    <para>
        ///       This style only applies to subclasses of Control.  It only works if UserPaint is set,
        ///       and the parent control is a Control.
        ///    </para>
        /// </devdoc>
        SupportsTransparentBackColor          = 0x00000800,
        /// <include file='doc\ControlStyles.uex' path='docs/doc[@for="ControlStyles.StandardDoubleClick"]/*' />
        /// <devdoc>
        ///    <para>
        ///        If set, windows forms calls OnDoubleClick and raises the DoubleClick event when the control is double clicked.
        ///        Regardless of whether it is set, the control may call OnDoubleClick directly.
        ///        This style is ignored if StandardClick is not set.
        ///    </para>
        /// </devdoc>
        StandardDoubleClick                   = 0x00001000,
        /// <include file='doc\ControlStyles.uex' path='docs/doc[@for="ControlStyles.AllPaintingInWmPaint"]/*' />
        /// <devdoc>
        ///    <para>
        ///        If true, WM_ERASEBKGND is ignored, and both OnPaintBackground and OnPaint are called directly from
        ///        WM_PAINT.  This generally reduces flicker, but can cause problems if other controls
        ///        send WM_ERASEBKGND messages to us.  (This is sometimes done to achieve a pseudo-transparent effect similar to
        ///        ControlStyles.SupportsTransparentBackColor; for instance, ToolBar with flat appearance does this).
        ///        This style only makes sense if UserPaint is true.
        ///    </para>
        /// </devdoc>
        AllPaintingInWmPaint                  = 0x00002000,
        /// <include file='doc\ControlStyles.uex' path='docs/doc[@for="ControlStyles.CacheText"]/*' />
        /// <devdoc>
        ///    <para>
        ///         If true, the control keeps a copy of the text rather than going to the hWnd for the
        ///         text every time. This improves performance but makes it difficult to keep the control
        ///         and hWnd's text synchronized. 
        ///         This style defaults to false.
        ///    </para>
        /// </devdoc>
        CacheText                           = 0x00004000, 
        /// <include file='doc\ControlStyles.uex' path='docs/doc[@for="ControlStyles.EnableNotifyMessage"]/*' />
        /// <devdoc>
        ///    <para>
        ///         If true, the OnNotifyMessage method will be called for every message sent to 
        ///         the control's WndProc. 
        ///         This style defaults to false.
        ///    </para>
        /// </devdoc>
        EnableNotifyMessage                 = 0x00008000, 
        /// <include file='doc\ControlStyles.uex' path='docs/doc[@for="ControlStyles.DoubleBuffer"]/*' />
        /// <devdoc>
        ///    <para>
        ///         If set, all control painting will be double buffered. You must also
        ///         set the UserPaint and AllPaintingInWmPaint style.  Note: This is obsolete, please 
        ///         use OptimizedDoubleBuffer instead.
        ///    </para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)] // It is recommended that you use the DoubleBuffer property instead.
        DoubleBuffer                        = 0x00010000, 
        /// <include file='doc\ControlStyles.uex' path='docs/doc[@for="ControlStyles.OptimizedDoubleBuffer"]/*' />
        /// <devdoc>
        ///    <para>
        ///         If set, all control painting will be double buffered.
        ///    </para>
        /// </devdoc>
        OptimizedDoubleBuffer               = 0x00020000,
        /// <include file='doc\ControlStyles.uex' path='docs/doc[@for="ControlStyles.UseTextForAccessibility"]/*' />
        /// <devdoc>
        ///    <para>
        ///         If this style is set, and there is a value in the control's Text property, that value will be
        ///         used to determine the control's default Active Accessibility name and shortcut key. Otherwise,
        ///         the text of the preceding Label control will be used instead.
        ///
        ///         This style is set by default. Certain built-in control types such as TextBox and ComboBox
        ///         un-set this style, so that their current text will not be used by Active Accessibility.
        ///    </para>
        /// </devdoc>
        UseTextForAccessibility               = 0x00040000,
    }
}

