﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// This enum is used to determine placement of the ToolStripItem on the ToolStrip.
    /// </devdoc>
    public enum ToolStripItemOverflow
    {
        Never,		// on the main winbar itself,
        Always,		// on the overflow window
        AsNeeded	// DEFAULT try for main, overflow as necessary
    }
}
