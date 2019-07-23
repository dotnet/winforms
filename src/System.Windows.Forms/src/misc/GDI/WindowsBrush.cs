// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms.Internal
{
    /// <summary>
    ///  Encapsulates a GDI Brush object.
    /// </summary>
    internal abstract class WindowsBrush : MarshalByRefObject, ICloneable, IDisposable
    {
        // Handle to the native Windows brush object.
        //
        private readonly DeviceContext dc;
        private IntPtr nativeHandle;        // Cannot be protected because the class is internal (C# doesn't allow it).
        private readonly Color color = Color.White;  // GDI brushes have just one color as opposed to GDI+ that can have background color.
        // Note: We may need to implement background color too.

#if WINGRAPHICS_FINALIZATION_WATCH
        private string AllocationSite = DbgUtil.StackTrace;
#endif

        public abstract object Clone();     // Declaration required by C# even though this is an abstract class.

        protected abstract void CreateBrush();

        /// <summary>
        ///  Parameterless constructor to use default color.
        ///  Notice that the actual object construction is done in the derived classes.
        /// </summary>
        public WindowsBrush(DeviceContext dc)
        {
            this.dc = dc;
        }

        public WindowsBrush(DeviceContext dc, Color color)
        {
            this.dc = dc;
            this.color = color;
        }

        ~WindowsBrush()
        {
            Dispose(false);
        }

        protected DeviceContext DC
        {
            get
            {
                return dc;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (dc != null && nativeHandle != IntPtr.Zero)
            {
                DbgUtil.AssertFinalization(this, disposing);

                dc.DeleteObject(nativeHandle, GdiObjectType.Brush);

                nativeHandle = IntPtr.Zero;
            }

            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        public Color Color
        {
            get
            {
                return color;
            }
        }

        /// <summary>
        ///  Gets the native Win32 brush handle. It creates it on demand.
        /// </summary>
        protected IntPtr NativeHandle
        {
            get
            {
                if (nativeHandle == IntPtr.Zero)
                {
                    CreateBrush();
                }

                return nativeHandle;
            }

            set
            {
                Debug.Assert(nativeHandle == IntPtr.Zero, "WindowsBrush object is immutable");
                Debug.Assert(value != IntPtr.Zero, "WARNING: assigning IntPtr.Zero to the nativeHandle object.");

                nativeHandle = value;
            }
        }

        /// <summary>
        ///  Returns the native Win32 brush handle.
        /// </summary>
        public IntPtr HBrush
        {
            get
            {
                return NativeHandle;
            }
        }
    }

}
