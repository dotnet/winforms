// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
///  Decodes Windows messages. This is in a separate class from Message so we can avoid
///  loading it in the 99% case where we don't need it.
/// </summary>
internal static class MessageDecoder
{
    public static string ToString(Message message) => ToString(
        message.HWnd,
        message.MsgInternal,
        message.WParamInternal,
        message.LParamInternal,
        message.ResultInternal);

    private static string ToString(IntPtr hWnd, MessageId msg, WPARAM wparam, LPARAM lparam, LRESULT result)
    {
        static string Parenthesize(string? input) => input is null ? string.Empty : $" ({input})";

        string id = Parenthesize(msg.MessageIdToString());

        string lDescription = string.Empty;
        if (msg == PInvoke.WM_PARENTNOTIFY)
        {
            lDescription = Parenthesize(((MessageId)(uint)wparam.LOWORD).MessageIdToString());
        }

        return $@"msg=0x{(uint)msg:x}{id} hwnd=0x{(long)hWnd:x} wparam=0x{(nint)wparam:x} lparam=0x{(nint)lparam:x}{lDescription} result=0x{(nint)result:x}";
    }
}
