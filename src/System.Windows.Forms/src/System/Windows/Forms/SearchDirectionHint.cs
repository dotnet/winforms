// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")] // Maps to native enum.
    public enum SearchDirectionHint
    {
        Up = NativeMethods.VK_UP,
        Down = NativeMethods.VK_DOWN,
        Left = NativeMethods.VK_LEFT,
        Right = NativeMethods.VK_RIGHT
    }
}
