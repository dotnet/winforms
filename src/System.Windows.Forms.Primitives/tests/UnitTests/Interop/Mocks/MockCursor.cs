// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Primitives.Tests.Interop.Mocks;

public class MockCursor : IDisposable
{
    private HCURSOR _handle;
    private readonly bool _ownHandle = true;

    internal MockCursor(PCWSTR nResourceId)
    {
        // We don't delete stock cursors.
        _ownHandle = false;
        _handle = PInvoke.LoadCursor(HINSTANCE.Null, nResourceId);
        if (_handle.IsNull)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }

    public void Dispose()
    {
        if (!_handle.IsNull && _ownHandle)
        {
            PInvoke.DestroyCursor(_handle);
            _handle = HCURSOR.Null;
        }
    }

    internal HCURSOR Handle => _handle.IsNull ? throw new ObjectDisposedException(nameof(MockCursor)) : _handle;

    public Size Size => SystemInformation.CursorSize;
}
