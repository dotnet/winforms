// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop.User32;

namespace System
{
    internal class EditControl : Window
    {
        private static readonly EditClass s_editClass = new EditClass();

        public EditControl(string windowName = default, ES editStyle = ES.LEFT,
                           WS style = WS.OVERLAPPED,
                           WS_EX extendedStyle = WS_EX.CLIENTEDGE | WS_EX.LEFT | WS_EX.LTRREADING,
                           bool isMainWindow = false, Window parentWindow = default,
                           IntPtr parameters = default, IntPtr menuHandle = default)
            : base(s_editClass, new Rectangle(0, 0, 100, 50), windowName, style |= (WS)editStyle,
                   extendedStyle, isMainWindow, parentWindow, parameters, menuHandle)
        {
        }
    }
}
