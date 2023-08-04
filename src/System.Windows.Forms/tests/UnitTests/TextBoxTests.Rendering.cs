// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Metafiles;

namespace System.Windows.Forms.Tests;

public partial class TextBoxTests
{
    [WinFormsFact]
    public void TextBox_Disabled_PlaceholderText_RendersBackgroundCorrectly()
    {
        // Regression test for https://github.com/dotnet/winforms/issues/3706

        using Form form = new Form();
        using TextBox textBox = new TextBox
        {
            Size = new Size(80, 23),
            PlaceholderText = "abc",
            Enabled = false
        };
        form.Controls.Add(textBox);

        // Force the handle creation
        Assert.NotEqual(IntPtr.Zero, form.Handle);
        Assert.NotEqual(IntPtr.Zero, textBox.Handle);

        using var emf = new EmfScope();
        DeviceContextState state = new DeviceContextState(emf);

        textBox.TestAccessor().Dynamic.DrawPlaceholderText(emf.HDC);

        emf.Validate(
            state,
            Validate.TextOut(
                "abc",
                bounds: null,                                   // Don't care about the bounds for this test
                State.BackgroundMode(BACKGROUND_MODE.TRANSPARENT)));
    }
}
