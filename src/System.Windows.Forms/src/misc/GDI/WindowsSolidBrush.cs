// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINFORMS_NAMESPACE
namespace System.Windows.Forms.Internal
#elif DRAWING_NAMESPACE
namespace System.Drawing.Internal
#else
namespace System.Experimental.Gdi
#endif
{
    using System;
    using System.Internal;
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
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        protected override void CreateBrush()
        { 
            IntPtr nativeHandle = IntSafeNativeMethods.CreateSolidBrush(ColorTranslator.ToWin32( this.Color));
            if(nativeHandle == IntPtr.Zero) // Don't use Debug.Assert, DbgUtil.GetLastErrorStr would always be evaluated.
            {
                Debug.Fail("CreateSolidBrush failed : " + DbgUtil.GetLastErrorStr());
            }

            this.NativeHandle = nativeHandle;  // sets the handle value in the base class.
        }

        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public WindowsSolidBrush(DeviceContext dc)  : base(dc)
        {
            // CreateBrush() on demand.
        }

        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public WindowsSolidBrush(DeviceContext dc, Color color) : base( dc, color )
        {
            // CreateBrush() on demand.
        }

        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public override object Clone()
        { 
            return new WindowsSolidBrush(this.DC, this.Color);
        }

        public override string ToString()
        {
            return String.Format( CultureInfo.InvariantCulture, "{0}: Color={1}", this.GetType().Name,  this.Color );
        }
    }
}
