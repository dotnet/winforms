// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if DRAWING_DESIGN_NAMESPACE
namespace System.Windows.Forms.Internal
#elif DRAWING_NAMESPACE
namespace System.Drawing.Internal
#else
namespace System.Experimental.Gdi
#endif
{
    using System;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.Versioning;

#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
    public
#else
    internal
#endif
    sealed class WindowsSolidBrush : WindowsBrush
    {


        protected override void CreateBrush()
        {
            IntPtr nativeHandle = IntSafeNativeMethods.CreateSolidBrush(ColorTranslator.ToWin32( this.Color));
            if(nativeHandle == IntPtr.Zero) // Don't use Debug.Assert, DbgUtil.GetLastErrorStr would always be evaluated.
            {
                Debug.Fail("CreateSolidBrush failed : " + DbgUtil.GetLastErrorStr());
            }

            this.NativeHandle = nativeHandle;  // sets the handle value in the base class.
        }



        public WindowsSolidBrush(DeviceContext dc)  : base(dc)
        {
            // CreateBrush() on demand.
        }



        public WindowsSolidBrush(DeviceContext dc, Color color) : base( dc, color )
        {
            // CreateBrush() on demand.
        }



        public override object Clone()
        {
            return new WindowsSolidBrush(this.DC, this.Color);
        }

        public override string ToString()
        {
            return string.Format( CultureInfo.InvariantCulture, "{0}: Color={1}", this.GetType().Name,  this.Color );
        }
    }
}
