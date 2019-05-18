﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    /// <summary>
    // This enum is tightly coupled to Orientation so you can determine quickly
    // an orientation from a direction. (direction & Orientation.Vertical == Orientation.Vertical)
    /// </devdoc>
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    public enum ArrowDirection
    {
        Up = 0x00 | (int)Orientation.Vertical,
        Down = 0x10 | (int)Orientation.Vertical,
        Left = 0x00 | (int)Orientation.Horizontal,
        Right = 0x10 | (int)Orientation.Horizontal,
    }
}
