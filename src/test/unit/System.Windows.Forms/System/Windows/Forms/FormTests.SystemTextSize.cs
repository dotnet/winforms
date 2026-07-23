// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

namespace System.Windows.Forms.Tests;

public partial class FormTests
{
#if NET11_0_OR_GREATER
    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void Form_OnSystemTextSizeChanged_Invoke_CallsSystemTextSizeChanged(EventArgs eventArgs)
    {
        using SubForm control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        control.SystemTextSizeChanged += handler;
        control.OnSystemTextSizeChanged(eventArgs);
        Assert.Equal(1, callCount);

        control.SystemTextSizeChanged -= handler;
        control.OnSystemTextSizeChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    public partial class SubForm
    {
        public new void OnSystemTextSizeChanged(EventArgs e) => base.OnSystemTextSizeChanged(e);
    }
#endif
}
