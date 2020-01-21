// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;
using static Interop.User32;

internal static class WMExtensions
{
    public static bool IsMouseMessage(this ref Message message)
        => message.IsBetween(WM.MOUSEFIRST, WM.MOUSELAST);

    public static bool IsMouseMessage(this ref MSG message)
        => message.IsBetween(WM.MOUSEFIRST, WM.MOUSELAST);

    public static bool IsKeyMessage(this ref Message message)
        => message.IsBetween(WM.KEYFIRST, WM.KEYLAST);

    public static bool IsKeyMessage(this ref MSG message)
        => message.IsBetween(WM.KEYFIRST, WM.KEYLAST);

    /// <summary>
    /// Returns true if the message is between <paramref name="firstMessage"/> and
    /// <paramref name="secondMessage"/>, inclusive.
    /// </summary>
    public static bool IsBetween(
        this ref Message message,
        WM firstMessage,
        WM secondMessage)
        => message.Msg >= (int)firstMessage && message.Msg <= (int)secondMessage;

    /// <summary>
    /// Returns true if the message is between <paramref name="firstMessage"/> and
    /// <paramref name="secondMessage"/>, inclusive.
    /// </summary>
    public static bool IsBetween(
        this ref MSG message,
        WM firstMessage,
        WM secondMessage)
        => (uint)message.message >= (uint)firstMessage && (uint)message.message <= (uint)secondMessage;
}

