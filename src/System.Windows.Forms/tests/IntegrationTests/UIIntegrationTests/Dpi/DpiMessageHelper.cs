// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms.UITests.Dpi
{
    internal static class DpiMessageHelper
    {
        public static void TriggerDpiMessage(User32.WM message, Control control, int newDpi)
        {
            double factor = newDpi / DpiHelper.LogicalDpi;
            WPARAM wParam = WPARAM.MAKEWPARAM(newDpi, newDpi);

            _ = message switch
            {
                User32.WM.DPICHANGED => SendWmDpiChangedMessage(message),
                User32.WM.DPICHANGED_BEFOREPARENT => PInvoke.SendMessage(control, message, wParam),
                User32.WM.DPICHANGED_AFTERPARENT => PInvoke.SendMessage(control, message),
                _ => throw new NotImplementedException()
            };

            nint SendWmDpiChangedMessage(User32.WM message)
            {
                RECT suggestedRect = new(0,
                    0,
                    (int)Math.Round(control.Width * factor),
                    (int)Math.Round(control.Height * factor));

                PInvoke.SendMessage(control, User32.WM.GETDPISCALEDSIZE, wParam, ref suggestedRect);
                return PInvoke.SendMessage(control, message, wParam, ref suggestedRect);
            }
        }
    }
}
