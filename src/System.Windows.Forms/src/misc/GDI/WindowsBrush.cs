// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms.Internal
{
    /// <summary>
    ///  Encapsulates a GDI Brush object.
    /// </summary>
    internal abstract class WindowsBrush : ICloneable, IDisposable, IHandle
    {
        private Gdi32.HBRUSH _nativeHandle;

        public abstract object Clone();

        protected abstract void CreateBrush();

        public WindowsBrush(DeviceContext dc, Color color)
        {
            DC = dc;
            Color = color;
        }

        ~WindowsBrush() => Dispose(false);

        protected DeviceContext DC { get; }

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (DC != null && !_nativeHandle.IsNull)
            {
                DC.DeleteObject(_nativeHandle, GdiObjectType.Brush);
                _nativeHandle = default;
            }

            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        public Color Color { get; } = Color.White;

        /// <summary>
        ///  Gets the native Win32 brush handle. It creates it on demand.
        /// </summary>
        public Gdi32.HBRUSH HBrush
        {
            get
            {
                if (_nativeHandle.IsNull)
                {
                    CreateBrush();
                }

                return _nativeHandle;
            }
            protected set
            {
                Debug.Assert(_nativeHandle.IsNull, "WindowsBrush object is immutable");
                Debug.Assert(!value.IsNull, "WARNING: assigning null handle to the nativeHandle object.");

                _nativeHandle = value;
            }
        }

        public IntPtr Handle => (IntPtr)_nativeHandle;
    }
}
