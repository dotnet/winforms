// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms.Internal
{
    internal sealed class WindowsSolidBrush : WindowsBrush
    {
        protected override void CreateBrush()
        {
            IntPtr nativeHandle = Gdi32.CreateSolidBrush(ColorTranslator.ToWin32(Color));
            Debug.Assert(nativeHandle != IntPtr.Zero);
            HBrush = nativeHandle;
        }

        public WindowsSolidBrush(DeviceContext dc) : base(dc)
        {
        }

        public WindowsSolidBrush(DeviceContext dc, Color color) : base(dc, color)
        {
        }

        public override object Clone() => new WindowsSolidBrush(DC, Color);

        public override string ToString() => $"{nameof(WindowsSolidBrush)}: Color = {Color}";
    }
}
