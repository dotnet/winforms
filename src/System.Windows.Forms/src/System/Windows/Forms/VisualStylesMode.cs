// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Represents the version of the visual renderer.
/// </summary>
[Experimental("WFO9000")]
public enum VisualStylesMode : short
{
    /// <summary>
    ///  Visual renderers are not in use. See <see cref="UseVisualStyles"/>; Controls are based on Version 5 of ComCtl.
    /// </summary>
    Disabled = 0,

    /// <summary>
    ///  The classic version of the visual renderer (.NET 8 and earlier), using the Version 6 of ComCtl.
    /// </summary>
    Classic = 1,

    /// <summary>
    ///  The latest version of the visual renderer. Controls are rendered using the latest version
    ///  of ComCtl and customized adorner rendering and layouting in addition.
    /// </summary>
    Latest = 2
}
