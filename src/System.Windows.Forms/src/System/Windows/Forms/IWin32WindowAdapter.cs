// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    internal struct IWin32WindowAdapter : IHandle
    {
        private IWin32Window _window;

        public IWin32WindowAdapter(IWin32Window window)
        {
            _window = window;
        }

        public IntPtr Handle => Control.GetSafeHandle(_window);
    }
}
