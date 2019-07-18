// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms.Internal
{
    internal sealed class WindowsSolidBrush : WindowsBrush
    {
        protected override void CreateBrush()
        {
            IntPtr nativeHandle = SafeNativeMethods.CreateSolidBrush(ColorTranslator.ToWin32(Color));
            if (nativeHandle == IntPtr.Zero) // Don't use Debug.Assert, DbgUtil.GetLastErrorStr would always be evaluated.
            {
                Debug.Fail("CreateSolidBrush failed : " + DbgUtil.GetLastErrorStr());
            }

            NativeHandle = nativeHandle;  // sets the handle value in the base class.
        }

        public WindowsSolidBrush(DeviceContext dc) : base(dc)
        {
            // CreateBrush() on demand.
        }

        public WindowsSolidBrush(DeviceContext dc, Color color) : base(dc, color)
        {
            // CreateBrush() on demand.
        }

        public override object Clone()
        {
            return new WindowsSolidBrush(DC, Color);
        }

        public override string ToString()
        {
            return $"{nameof(WindowsSolidBrush)}: Color = {Color}";
        }
    }
}
