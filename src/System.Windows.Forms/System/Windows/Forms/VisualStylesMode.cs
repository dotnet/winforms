// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Represents the version of the visual renderer that a control or the application uses.
/// </summary>
/// <remarks>
///  <para>
///   The visual styles version controls how a control renders its adorners, borders, and layout.
///   Newer versions can adjust minimum sizes, padding, and margins to satisfy current accessibility
///   requirements without changing the behavior of applications that target an earlier version.
///  </para>
///  <para>
///   A visual styles version defines the latest rendering behavior a control may use; it does not require
///   every control to change. Each control determines which visual features and appearance values it supports
///   for a given version.
///  </para>
/// </remarks>
public enum VisualStylesMode : short
{
    /// <summary>
    ///  The control inherits its <see cref="Control.VisualStylesMode"/> from its parent, or, for a
    ///  top-level control, from <see cref="Application.DefaultVisualStylesMode"/>. This is the ambient default
    ///  returned by <see cref="Control.VisualStylesMode"/> when no local mode is set.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This value is the ambient sentinel: assigning it to <see cref="Control.VisualStylesMode"/>
    ///   clears any local override so the value is inherited again. It is not valid as the application
    ///   default and is rejected by <see cref="Application.SetDefaultVisualStylesMode(VisualStylesMode)"/>.
    ///  </para>
    /// </remarks>
    Inherit = -1,

    /// <summary>
    ///  The classic version of the visual renderer (.NET 8 and earlier), based on version 6 of the
    ///  common controls library.
    /// </summary>
    Classic = 0,

    /// <summary>
    ///  Visual renderers are not in use - see <see cref="Application.UseVisualStyles"/>.
    ///  Controls are based on version 5 of the common controls library.
    /// </summary>
    Disabled = 1,

    /// <summary>
    ///  The .NET 11 version of the visual renderer. Controls are rendered using the latest version
    ///  of the common controls library, and the adorner rendering or the layout of specific controls
    ///  has been improved based on the latest accessibility requirements.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Controls opt into the .NET 11 rendering changes they support. For example, support for
    ///   <see cref="Appearance.ToggleSwitch"/> depends on the control and its current state.
    ///  </para>
    /// </remarks>
    Net11 = 2,

    /// <summary>
    ///  The latest stable version of the visual renderer available in the running framework.
    /// </summary>
    Latest = short.MaxValue
}
