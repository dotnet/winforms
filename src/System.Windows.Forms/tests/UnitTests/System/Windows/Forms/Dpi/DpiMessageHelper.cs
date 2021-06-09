// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms.Tests.Dpi
{
    internal static class DpiMessageHelper
    {
        public static IntPtr TriggerDpiMessage(User32.WM message, Control control, int newDpi)
        {
            double factor = newDpi / DpiHelper.LogicalDpi;
            var wParam = PARAM.FromLowHigh(newDpi, newDpi);
            RECT suggestedRect = new(0,
                0,
                (int)Math.Round(control.Width * factor),
                (int)Math.Round(control.Height * factor));
            return User32.SendMessageW(control, message, wParam, ref suggestedRect);
        }
    }
}
