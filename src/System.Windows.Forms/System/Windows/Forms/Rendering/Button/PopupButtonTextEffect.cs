// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Rendering.Button;

/// <summary>
///  Determines how the caption of a <see cref="FlatStyle.Popup"/> key-cap button is physically integrated
///  into the key surface.
/// </summary>
internal enum PopupButtonTextEffect
{
    /// <summary>
    ///  The caption appears slightly raised above the key surface (embossed).
    /// </summary>
    Raised = 0,

    /// <summary>
    ///  The caption appears slightly recessed into the key surface (engraved/letterpress).
    /// </summary>
    Engraved = 1,

    /// <summary>
    ///  The caption is drawn flat, without any relief effect.
    /// </summary>
    Flat = 2
}
