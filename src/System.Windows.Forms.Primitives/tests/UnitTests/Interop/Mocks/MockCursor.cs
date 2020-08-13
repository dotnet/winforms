// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop.User32;

namespace System.Windows.Forms.Primitives.Tests.Interop.Mocks
{
    public class MockCursor : IDisposable
    {
        private IntPtr _handle = IntPtr.Zero;       // handle to loaded image
        private bool _ownHandle = true;
        private readonly int _resourceId;

        public MockCursor(int nResourceId)
        {
            // We don't delete stock cursors.
            _ownHandle = false;
            _resourceId = nResourceId;
            _handle = LoadCursorW(IntPtr.Zero, (IntPtr)nResourceId);
        }

        public void Dispose()
        {
            if (_handle != IntPtr.Zero)
            {
                if (_ownHandle)
                {
                    DestroyCursor(_handle);
                }
                _handle = IntPtr.Zero;
            }
        }

        public IntPtr Handle
        {
            get
            {
                if (_handle == IntPtr.Zero)
                {
                    throw new ObjectDisposedException(nameof(MockCursor));
                }
                return _handle;
            }
        }

        public Size Size
        {
            get => new Size(GetSystemMetrics(SystemMetric.SM_CXCURSOR), GetSystemMetrics(SystemMetric.SM_CYCURSOR));
        }
    }
}
