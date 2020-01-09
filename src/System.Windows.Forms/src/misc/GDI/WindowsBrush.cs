// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms.Internal
{
    /// <summary>
    ///  Encapsulates a GDI Brush object.
    /// </summary>
    internal abstract class WindowsBrush : MarshalByRefObject, ICloneable, IDisposable
    {
        private IntPtr _nativeHandle;

#if WINGRAPHICS_FINALIZATION_WATCH
        private string AllocationSite = DbgUtil.StackTrace;
#endif

        public abstract object Clone();

        protected abstract void CreateBrush();

        /// <summary>
        ///  Parameterless constructor to use default color.
        ///  Notice that the actual object construction is done in the derived classes.
        /// </summary>
        public WindowsBrush(DeviceContext dc)
        {
            DC = dc;
        }

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
            if (DC != null && _nativeHandle != IntPtr.Zero)
            {
                DbgUtil.AssertFinalization(this, disposing);

                DC.DeleteObject(_nativeHandle, GdiObjectType.Brush);

                _nativeHandle = IntPtr.Zero;
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
        public IntPtr HBrush
        {
            get
            {
                if (_nativeHandle == IntPtr.Zero)
                {
                    CreateBrush();
                }

                return _nativeHandle;
            }
            protected set
            {
                Debug.Assert(_nativeHandle == IntPtr.Zero, "WindowsBrush object is immutable");
                Debug.Assert(value != IntPtr.Zero, "WARNING: assigning IntPtr.Zero to the nativeHandle object.");

                _nativeHandle = value;
            }
        }
    }
}
