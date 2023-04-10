// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace System.Windows.Forms
{
    public enum SearchDirectionHint
    {
        Up = VIRTUAL_KEY.VK_UP,
        Down = VIRTUAL_KEY.VK_DOWN,
        Left = VIRTUAL_KEY.VK_LEFT,
        Right = VIRTUAL_KEY.VK_RIGHT,
    }
}
