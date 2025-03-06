// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  This enum is used to determine placement of the ToolStripItem on the ToolStrip.
/// </summary>
public enum ToolStripItemOverflow
{
    Never,      // on the main ToolStrip itself,
    Always,     // on the overflow window
    AsNeeded    // DEFAULT try for main, overflow as necessary
}
