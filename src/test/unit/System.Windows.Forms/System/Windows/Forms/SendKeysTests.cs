// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

namespace System.Windows.Forms.Tests;

public class SendKeysTests
{
    [WinFormsTheory]
    [InlineData("^a^c")]
    [InlineData("+a+c")]
    [InlineData("%f%t")]
    public void SendKeys_ParseKeys_SequentialImmediateModifiers_DoesNotThrow(string keys)
    {
        dynamic accessor = typeof(SendKeys).TestAccessor.Dynamic;
        Exception exception = Record.Exception(() => accessor.ParseKeys(keys, default(HWND)));
        Assert.Null(exception);
    }

    [WinFormsFact(Skip = "This test depends on focus and should be run manually.")]
    public void SendKeysGrouping()
    {
        // Regression https://github.com/dotnet/winforms/issues/6666

        using CaptureForm form = new();
        form.Show();
        form.Focus();
        SendKeys.SendWait("^(a)^(c)");

        Assert.Equal(4, form.KeyEvents.Count);
        Assert.Equal(Keys.ControlKey, form.KeyEvents[0].KeyCode);
        Assert.Equal(Keys.A, form.KeyEvents[1].KeyCode);
        Assert.Equal(Keys.Control, form.KeyEvents[1].Modifiers);
        Assert.Equal(Keys.ControlKey, form.KeyEvents[2].KeyCode);
        Assert.Equal(Keys.C, form.KeyEvents[3].KeyCode);
        Assert.Equal(Keys.Control, form.KeyEvents[3].Modifiers);
    }

    private class CaptureForm : Form
    {
        public List<KeyEventArgs> KeyEvents { get; } = [];

        protected override void OnKeyDown(KeyEventArgs e)
        {
            KeyEvents.Add(e);
            base.OnKeyDown(e);
        }
    }
}
