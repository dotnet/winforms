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
/// </remarks>
public enum VisualStylesMode : short
{
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
    Net11 = 2,

    /// <summary>
    ///  The latest version of the visual renderer available in the running framework.
    /// </summary>
    Latest = short.MaxValue
}
