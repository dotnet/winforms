// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.UITests.Dpi;

internal static class DpiMessageHelper
{
    public static void TriggerDpiMessage(MessageId message, Control control, int newDpi)
    {
        double factor = newDpi / (double)ScaleHelper.OneHundredPercentLogicalDpi;
        WPARAM wParam = WPARAM.MAKEWPARAM(newDpi, newDpi);

        _ = (uint)message switch
        {
            PInvoke.WM_DPICHANGED => SendWmDpiChangedMessage(message),
            PInvoke.WM_DPICHANGED_BEFOREPARENT => PInvoke.SendMessage(control, message, wParam),
            PInvoke.WM_DPICHANGED_AFTERPARENT => PInvoke.SendMessage(control, message),
            _ => throw new NotImplementedException()
        };

        nint SendWmDpiChangedMessage(MessageId message)
        {
            RECT suggestedRect = new(0,
                0,
                (int)Math.Round(control.Width * factor),
                (int)Math.Round(control.Height * factor));

            PInvoke.SendMessage(control, PInvoke.WM_GETDPISCALEDSIZE, wParam, ref suggestedRect);
            return PInvoke.SendMessage(control, message, wParam, ref suggestedRect);
        }
    }
}
