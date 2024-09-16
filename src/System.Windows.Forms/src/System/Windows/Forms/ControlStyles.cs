// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Windows.Forms.Analyzers.Diagnostics;

namespace System.Windows.Forms;

/// <summary>
///  Specifies control functionality.
/// </summary>
[Flags]
public enum ControlStyles
{
    /// <summary>
    ///  Indicates whether the control is a container-like control.
    /// </summary>
    ContainerControl = 0x00000001,

    /// <summary>
    ///  The control paints itself; WM_PAINT and WM_ERASEBKGND messages are not
    ///  passed on to the underlying NativeWindow.
    ///  This style only applies to subclasses of Control.
    /// </summary>
    UserPaint = 0x00000002,

    /// <summary>
    ///  If specified, a PaintBackground event will not be raised,
    ///  OnPaintBackground will not be called, and Invalidate() will not
    ///  invalidate the background of the HWND.
    /// </summary>
    Opaque = 0x00000004,

    /// <summary>
    ///  The control is completely redrawn when it is resized.
    /// </summary>
    ResizeRedraw = 0x00000010,

    /// <summary>
    ///  The control has a fixed width.
    /// </summary>
    FixedWidth = 0x00000020,

    /// <summary>
    ///  The control has a fixed height.
    /// </summary>
    FixedHeight = 0x00000040,

    /// <summary>
    ///  If set, windows forms calls OnClick and raises the Click event when the
    ///  control is clicked (unless it's the second click of a double-click and
    ///  StandardDoubleClick is specified).
    ///  Regardless of this style, the control may call OnClick directly.
    /// </summary>
    StandardClick = 0x00000100,

    /// <summary>
    ///  The control can get the focus.
    /// </summary>
    Selectable = 0x00000200,

    /// <summary>
    ///  The control does its own mouse processing; WM_MOUSEDOWN, WM_MOUSEMOVE,
    ///  and WM_MOUSEUP messages are not passed on to the underlying NativeWindow.
    /// </summary>
    UserMouse = 0x00000400,

    /// <summary>
    ///  If the BackColor is set to a color whose alpha component is less than
    ///  255 (i.e., BackColor.A is less than 255), OnPaintBackground will simulate
    ///  transparency by asking its parent control to paint our background.
    ///  This is not true transparency -- if there is another control between us
    ///  and our parent, we will not show the control in the middle.
    ///  This style only applies to subclasses of Control. It only works if
    ///  UserPaint is set, and the parent control is a Control.
    /// </summary>
    SupportsTransparentBackColor = 0x00000800,

    /// <summary>
    ///  If set, windows forms calls OnDoubleClick and raises the DoubleClick
    ///  event when the control is double clicked.
    ///  Regardless of whether it is set, the control may call OnDoubleClick
    ///  directly.
    ///  This style is ignored if StandardClick is not set.
    /// </summary>
    StandardDoubleClick = 0x00001000,

    /// <summary>
    ///  If true, WM_ERASEBKGND is ignored, and both OnPaintBackground and
    ///  OnPaint are called directly from WM_PAINT. This generally reduces
    ///  flicker, but can cause problems if other controls send WM_ERASEBKGND
    ///  messages to us. (This is sometimes done to achieve a pseudo-transparent
    ///  effect similar to ControlStyles.SupportsTransparentBackColor; for instance,
    ///  ToolBar with flat appearance does this).
    ///  This style only makes sense if UserPaint is true.
    /// </summary>
    AllPaintingInWmPaint = 0x00002000,

    /// <summary>
    ///  If true, the control keeps a copy of the text rather than going to the
    ///  hWnd for the text every time. This improves performance but makes it
    ///  difficult to keep the control and hWnd's text synchronized.
    ///  This style defaults to false.
    /// </summary>
    CacheText = 0x00004000,

    /// <summary>
    ///  If true, the OnNotifyMessage method will be called for every message
    ///  sent to the control's WndProc.
    ///  This style defaults to false.
    /// </summary>
    EnableNotifyMessage = 0x00008000,

    /// <summary>
    ///  If set, all control painting will be double buffered. You must also
    ///  set the UserPaint and AllPaintingInWmPaint style. Note: This is
    ///  obsolete, please
    ///  use OptimizedDoubleBuffer instead.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)] // It is recommended that you use the DoubleBuffer property instead.
    DoubleBuffer = 0x00010000,

    /// <summary>
    ///  If set, all control painting will be double buffered.
    /// </summary>
    OptimizedDoubleBuffer = 0x00020000,

    /// <summary>
    ///  If this style is set, and there is a value in the control's Text property,
    ///  that value will be used to determine the control's default Active
    ///  Accessibility name and shortcut key. Otherwise, the text of the preceding
    ///  Label control will be used instead.
    ///
    ///  This style is set by default. Certain built-in control types such as
    ///  TextBox and ComboBox un-set this style, so that their current text will
    ///  not be used by Active Accessibility.
    /// </summary>
    UseTextForAccessibility = 0x00040000,

    /// <summary>
    ///  For certain UI-related color modes (Dark Mode/Light Mode), controls
    ///  can opt-in to automatically apply the appropriate theming. Especially
    ///  controls which are utilizing system-managed scrollbars can benefit
    ///  from this setting. Note that using this settings will cause some
    ///  win32 control theming renderers to become inactive for a specific theme.
    /// </summary>
    [Experimental(DiagnosticIDs.ExperimentalDarkMode, UrlFormat = DiagnosticIDs.UrlFormat)]
    ApplyThemingImplicitly = 0x00080000
}
