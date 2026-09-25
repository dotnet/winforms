// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace System.Windows.Forms.UITests.Input;

/// <summary>
///  Sends keyboard and mouse input and verifies that all events were inserted.
/// </summary>
internal class InputSimulator
{
    public InputSimulator()
    {
        Keyboard = new KeyboardSimulator(this);
        Mouse = new MouseSimulator(this);
    }

    public KeyboardSimulator Keyboard { get; }

    public MouseSimulator Mouse { get; }

    internal static void Send(ReadOnlySpan<INPUT> inputs)
    {
        uint inserted = PInvoke.SendInput(inputs, Marshal.SizeOf<INPUT>());
        // The generated PInvoke wrapper clears and captures the native last error.
        int error = Marshal.GetLastPInvokeError();
        VerifyInsertedCount(inputs.Length, inserted, error);
    }

    internal static void VerifyInsertedCount(int requested, uint inserted, int error)
    {
        if (inserted == requested)
        {
            return;
        }

        string message = $"SendInput inserted {inserted} of {requested} requested input events. Last Win32 error: {error}.";
        if (error == 0)
        {
            message += " No extended error information was reported.";
        }

        throw new InvalidOperationException(message);
    }
}
