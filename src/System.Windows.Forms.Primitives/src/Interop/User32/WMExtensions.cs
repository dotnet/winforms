// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

internal static class WMExtensions
{
    public static bool IsMouseMessage(this ref Message message)
        => message.IsBetween(PInvokeCore.WM_MOUSEFIRST, PInvokeCore.WM_MOUSELAST);

    public static bool IsMouseMessage(this ref MSG message)
        => message.IsBetween(PInvokeCore.WM_MOUSEFIRST, PInvokeCore.WM_MOUSELAST);

    public static bool IsKeyMessage(this ref Message message)
        => message.IsBetween(PInvokeCore.WM_KEYFIRST, PInvokeCore.WM_KEYLAST);

    public static bool IsKeyMessage(this ref MSG message)
        => message.IsBetween(PInvokeCore.WM_KEYFIRST, PInvokeCore.WM_KEYLAST);

    /// <summary>
    /// Returns true if the message is between <paramref name="firstMessage"/> and
    /// <paramref name="secondMessage"/>, inclusive.
    /// </summary>
    public static bool IsBetween(
        this ref Message message,
        MessageId firstMessage,
        MessageId secondMessage)
        => message.Msg >= (int)firstMessage && message.Msg <= (int)secondMessage;

    /// <summary>
    /// Returns true if the message is between <paramref name="firstMessage"/> and
    /// <paramref name="secondMessage"/>, inclusive.
    /// </summary>
    public static bool IsBetween(
        this ref MSG message,
        MessageId firstMessage,
        MessageId secondMessage)
        => message.message >= (uint)firstMessage && message.message <= (uint)secondMessage;
}
