﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms
{
    /// <summary>
    /// Control Dock values.
    /// When a control is docked to an edge of it's container it will
    /// always be positioned flush against that edge while the container
    /// resizes. If more than one control is docked to an edge, the controls
    /// will not be placed on top of each other.
    /// </summary>
    [Editor("System.Windows.Forms.Design.DockEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
    public enum DockStyle
    {
        None,
        Top,
        Bottom,
        Left,
        Right,
        Fill,
    }
}
