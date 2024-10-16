// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class ExceptionHandlingTests
{
    [WinFormsFact]
    public void ValidateMessageLoopExceptionsAreThrown()
    {
        using ThrowControl control = new();
        control.CreateControl();
        Action action = () =>
        {
            PInvokeCore.PostMessage(control, 9876);
            Application.DoEvents();
        };

        Assert.Throws<InvalidOperationException>(() => action());
    }

    public class ThrowControl : Control
    {
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 9876)
            {
                throw new InvalidOperationException();
            }

            base.WndProc(ref m);
        }
    }
}
