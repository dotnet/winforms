// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  This enum represents the current layout of the ToolStripItem.
/// </summary>
public enum ToolStripItemPlacement
{
    Main,          // in the main ToolStrip itself
    Overflow,      // in the overflow window
    None           // either offscreen or visible == false so we didn't lay it out
}
