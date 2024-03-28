// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.Tests;

public class TextBoxRendererTests
{
    [WinFormsTheory]
    [InlineData(TextBoxState.Normal)]
    [InlineData(TextBoxState.Hot)]
    [InlineData(TextBoxState.Selected)]
    [InlineData(TextBoxState.Disabled)]
    public void TextBoxRenderer_DrawTextBox(TextBoxState state)
    {
        using Bitmap bitmap = new(100, 100);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new Rectangle(10, 20, 30, 40);

        TextBoxRenderer.DrawTextBox(graphics, bounds, state);
    }

    [WinFormsTheory]
    [InlineData(TextBoxState.Normal)]
    [InlineData(TextBoxState.Hot)]
    [InlineData(TextBoxState.Selected)]
    [InlineData(TextBoxState.Disabled)]
    public void TextBoxRenderer_DrawTextBox_OverloadWithTextAndFont(TextBoxState state)
    {
        using Bitmap bitmap = new(100, 100);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new Rectangle(10, 20, 30, 40);

        TextBoxRenderer.DrawTextBox(graphics, bounds, "text", SystemFonts.DefaultFont, state);
    }

    [WinFormsTheory]
    [InlineData(TextBoxState.Normal)]
    [InlineData(TextBoxState.Hot)]
    [InlineData(TextBoxState.Selected)]
    [InlineData(TextBoxState.Disabled)]
    public void TextBoxRenderer_DrawTextBox_OverloadWithTextFontAndTextBounds(TextBoxState state)
    {
        using Bitmap bitmap = new(100, 100);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new Rectangle(10, 20, 30, 40);
        Rectangle textBounds = new Rectangle(10, 20, 30, 40);

        TextBoxRenderer.DrawTextBox(graphics, bounds, "text", SystemFonts.DefaultFont, textBounds, state);
    }

    public static IEnumerable<object[]> DrawTextBox_FlagsAndState_TestData()
    {
        yield return new object[] { TextBoxState.Normal, TextFormatFlags.Default };
        yield return new object[] { TextBoxState.Normal, TextFormatFlags.TextBoxControl };
        yield return new object[] { TextBoxState.Normal, TextFormatFlags.PreserveGraphicsTranslateTransform };

        yield return new object[] { TextBoxState.Hot, TextFormatFlags.Default };
        yield return new object[] { TextBoxState.Hot, TextFormatFlags.TextBoxControl };
        yield return new object[] { TextBoxState.Hot, TextFormatFlags.PreserveGraphicsTranslateTransform };

        yield return new object[] { TextBoxState.Disabled, TextFormatFlags.Default };
        yield return new object[] { TextBoxState.Disabled, TextFormatFlags.TextBoxControl };
        yield return new object[] { TextBoxState.Disabled, TextFormatFlags.PreserveGraphicsTranslateTransform };

        yield return new object[] { TextBoxState.Selected, TextFormatFlags.Default };
        yield return new object[] { TextBoxState.Selected, TextFormatFlags.TextBoxControl };
        yield return new object[] { TextBoxState.Selected, TextFormatFlags.PreserveGraphicsTranslateTransform };
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawTextBox_FlagsAndState_TestData))]
    public void TextBoxRenderer_DrawTextBox_OverloadWithTextFontAndFlags(TextFormatFlags flag, TextBoxState state)
    {
        using Bitmap bitmap = new(100, 100);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new Rectangle(10, 20, 30, 40);

        TextBoxRenderer.DrawTextBox(graphics, bounds, "text", SystemFonts.DefaultFont, flag, state);
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawTextBox_FlagsAndState_TestData))]
    public void TextBoxRenderer_DrawTextBox_OverloadWithTextFontTextboundsAndFlags(TextFormatFlags flag, TextBoxState state)
    {
        using Bitmap bitmap = new(100, 100);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new Rectangle(10, 20, 30, 40);
        Rectangle textBounds = new Rectangle(10, 20, 30, 40);

        TextBoxRenderer.DrawTextBox(graphics, bounds, "text", SystemFonts.DefaultFont, textBounds, flag, state);
    }
}
