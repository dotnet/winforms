// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System
{
    internal class Window : IDisposable, IHandle
    {
        private readonly WindowClass _windowClass;

        public IntPtr Handle { get; }

        public Window(
            WindowClass windowClass,
            Rectangle bounds,
            string windowName = default,
            User32.WS style = User32.WS.OVERLAPPED,
            User32.WS_EX extendedStyle = User32.WS_EX.DEFAULT,
            bool isMainWindow = false,
            Window parentWindow = default,
            IntPtr parameters = default,
            IntPtr menuHandle = default)
        {
            _windowClass = windowClass;
            if (!_windowClass.IsRegistered)
            {
                _windowClass.Register();
            }

            Handle = _windowClass.CreateWindow(
                bounds,
                windowName,
                style,
                extendedStyle,
                isMainWindow,
                parentWindow?.Handle ?? default,
                parameters,
                menuHandle);
        }

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                User32.DestroyWindow(this);
            }

            GC.SuppressFinalize(this);
        }
    }
}
