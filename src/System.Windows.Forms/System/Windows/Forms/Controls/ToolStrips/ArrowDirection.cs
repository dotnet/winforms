// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  This enum is tightly coupled to Orientation so you can determine quickly an orientation
///  from a direction. (direction &amp; Orientation.Vertical == Orientation.Vertical)
/// </summary>
public enum ArrowDirection
{
    Up = 0x00 | Orientation.Vertical,
    Down = 0x10 | Orientation.Vertical,
    Left = 0x00 | Orientation.Horizontal,
    Right = 0x10 | Orientation.Horizontal,
}
