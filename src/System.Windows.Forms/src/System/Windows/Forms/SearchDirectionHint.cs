// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public enum SearchDirectionHint
    {
        Up = User32.VK.UP,
        Down = User32.VK.DOWN,
        Left = User32.VK.LEFT,
        Right = User32.VK.RIGHT
    }
}
