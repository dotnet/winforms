// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop.User32;

namespace System
{
    internal class EditControl : Window
    {
        private static readonly EditClass s_editClass = new();

        public EditControl(string windowName = default,
            ES editStyle = ES.LEFT,
            WINDOW_STYLE style = WINDOW_STYLE.WS_OVERLAPPED,
            WINDOW_EX_STYLE extendedStyle = WINDOW_EX_STYLE.WS_EX_CLIENTEDGE | WINDOW_EX_STYLE.WS_EX_LEFT | WINDOW_EX_STYLE.WS_EX_LTRREADING,
            bool isMainWindow = false,
            Window parentWindow = default,
            nint parameters = default,
            HMENU menuHandle = default)
            : base(s_editClass, new Rectangle(0, 0, 100, 50), windowName, style |= (WINDOW_STYLE)editStyle,
                   extendedStyle, isMainWindow, parentWindow, parameters, menuHandle)
        {
        }
    }
}
