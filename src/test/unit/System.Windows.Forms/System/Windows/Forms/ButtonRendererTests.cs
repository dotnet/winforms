// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.Tests;

public class ButtonRendererTests
{
    public static TheoryData<PushButtonState> ButtonStates => new()
    {
        PushButtonState.Normal,
        PushButtonState.Hot,
        PushButtonState.Pressed,
        PushButtonState.Disabled,
        PushButtonState.Default
    };

    [WinFormsTheory]
    [MemberData(nameof(ButtonStates))]
    public void DrawButton_DoesNotThrow(PushButtonState state)
    {
        using Bitmap bitmap = new(50, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new(0, 0, 50, 20);

        Exception? exception = Record.Exception(() => ButtonRenderer.DrawButton(graphics, bounds, state));
        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [MemberData(nameof(ButtonStates))]
    public void DrawButton_IDeviceContext_DoesNotThrow(PushButtonState state)
    {
        using Bitmap bitmap = new(60, 25);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new(0, 0, 60, 25);

        ButtonRenderer.RenderMatchingApplicationState = false;
        Exception? exception = Record.Exception(() => ButtonRenderer.DrawButton(graphics, bounds, state));
        exception.Should().BeNull();

        ButtonRenderer.RenderMatchingApplicationState = true;
        Exception? exception2 = Record.Exception(() => ButtonRenderer.DrawButton(graphics, bounds, state));
        exception2.Should().BeNull();
    }

    [WinFormsTheory]
    [InlineData(PushButtonState.Normal, true)]
    [InlineData(PushButtonState.Normal, false)]
    [InlineData(PushButtonState.Hot, true)]
    [InlineData(PushButtonState.Hot, false)]
    [InlineData(PushButtonState.Pressed, true)]
    [InlineData(PushButtonState.Pressed, false)]
    [InlineData(PushButtonState.Disabled, true)]
    [InlineData(PushButtonState.Disabled, false)]
    [InlineData(PushButtonState.Default, true)]
    [InlineData(PushButtonState.Default, false)]
    public void DrawButton_Focused_DoesNotThrow(PushButtonState state, bool focused)
    {
        using Bitmap bitmap = new(80, 30);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new(0, 0, 80, 30);

        Exception? exception = Record.Exception(() => ButtonRenderer.DrawButton(graphics, bounds, focused, state));
        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [InlineData(PushButtonState.Normal, "Test", true)]
    [InlineData(PushButtonState.Normal, "Test", false)]
    [InlineData(PushButtonState.Normal, null, true)]
    [InlineData(PushButtonState.Normal, null, false)]
    [InlineData(PushButtonState.Hot, "Test", true)]
    [InlineData(PushButtonState.Hot, "Test", false)]
    [InlineData(PushButtonState.Hot, null, true)]
    [InlineData(PushButtonState.Hot, null, false)]
    [InlineData(PushButtonState.Pressed, "Test", true)]
    [InlineData(PushButtonState.Pressed, "Test", false)]
    [InlineData(PushButtonState.Pressed, null, true)]
    [InlineData(PushButtonState.Pressed, null, false)]
    [InlineData(PushButtonState.Disabled, "Test", true)]
    [InlineData(PushButtonState.Disabled, "Test", false)]
    [InlineData(PushButtonState.Disabled, null, true)]
    [InlineData(PushButtonState.Disabled, null, false)]
    [InlineData(PushButtonState.Default, "Test", true)]
    [InlineData(PushButtonState.Default, "Test", false)]
    [InlineData(PushButtonState.Default, null, true)]
    [InlineData(PushButtonState.Default, null, false)]
    public void DrawButton_TextFontFocused_DoesNotThrow(PushButtonState state, string? buttonText, bool focused)
    {
        using Bitmap bitmap = new(100, 40);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new(0, 0, 100, 40);
        using Font font = new("Segoe UI", 10);

        Exception? exception = Record.Exception(() =>
            ButtonRenderer.DrawButton(graphics, bounds, buttonText, font, focused, state));
        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [InlineData(PushButtonState.Normal, "Sample", TextFormatFlags.Default, true)]
    [InlineData(PushButtonState.Normal, "Sample", TextFormatFlags.Default, false)]
    [InlineData(PushButtonState.Normal, null, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, true)]
    [InlineData(PushButtonState.Normal, "Button", TextFormatFlags.SingleLine, false)]
    [InlineData(PushButtonState.Hot, "Sample", TextFormatFlags.Default, true)]
    [InlineData(PushButtonState.Hot, "Sample", TextFormatFlags.Default, false)]
    [InlineData(PushButtonState.Hot, null, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, true)]
    [InlineData(PushButtonState.Hot, "Button", TextFormatFlags.SingleLine, false)]
    [InlineData(PushButtonState.Pressed, "Sample", TextFormatFlags.Default, true)]
    [InlineData(PushButtonState.Pressed, "Sample", TextFormatFlags.Default, false)]
    [InlineData(PushButtonState.Pressed, null, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, true)]
    [InlineData(PushButtonState.Pressed, "Button", TextFormatFlags.SingleLine, false)]
    [InlineData(PushButtonState.Disabled, "Sample", TextFormatFlags.Default, true)]
    [InlineData(PushButtonState.Disabled, "Sample", TextFormatFlags.Default, false)]
    [InlineData(PushButtonState.Disabled, null, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, true)]
    [InlineData(PushButtonState.Disabled, "Button", TextFormatFlags.SingleLine, false)]
    [InlineData(PushButtonState.Default, "Sample", TextFormatFlags.Default, true)]
    [InlineData(PushButtonState.Default, "Sample", TextFormatFlags.Default, false)]
    [InlineData(PushButtonState.Default, null, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, true)]
    [InlineData(PushButtonState.Default, "Button", TextFormatFlags.SingleLine, false)]
    public void DrawButton_TextFontFlagsFocused_DoesNotThrow(
        PushButtonState state, string? buttonText, TextFormatFlags flags, bool focused)
    {
        using Bitmap bitmap = new(120, 50);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new(0, 0, 120, 50);
        using Font font = new("Arial", 12);

        Exception? exception = Record.Exception(() =>
            ButtonRenderer.DrawButton(graphics, bounds, buttonText, font, flags, focused, state));
        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [InlineData(PushButtonState.Normal, true)]
    [InlineData(PushButtonState.Normal, false)]
    [InlineData(PushButtonState.Hot, true)]
    [InlineData(PushButtonState.Hot, false)]
    [InlineData(PushButtonState.Pressed, true)]
    [InlineData(PushButtonState.Pressed, false)]
    [InlineData(PushButtonState.Disabled, true)]
    [InlineData(PushButtonState.Disabled, false)]
    [InlineData(PushButtonState.Default, true)]
    [InlineData(PushButtonState.Default, false)]
    public void DrawButton_ImageImageBoundsFocused_DoesNotThrow(PushButtonState state, bool focused)
    {
        using Bitmap bitmap = new(120, 50);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new(0, 0, 120, 50);

        using Bitmap image = new(32, 32);
        Rectangle imageBounds = new(10, 10, 32, 32);

        Exception? exception = Record.Exception(() =>
            ButtonRenderer.DrawButton(graphics, bounds, image, imageBounds, focused, state));
        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [InlineData(PushButtonState.Normal, "Icon", true)]
    [InlineData(PushButtonState.Normal, "Icon", false)]
    [InlineData(PushButtonState.Normal, null, true)]
    [InlineData(PushButtonState.Normal, null, false)]
    [InlineData(PushButtonState.Hot, "Icon", true)]
    [InlineData(PushButtonState.Hot, "Icon", false)]
    [InlineData(PushButtonState.Hot, null, true)]
    [InlineData(PushButtonState.Hot, null, false)]
    [InlineData(PushButtonState.Pressed, "Icon", true)]
    [InlineData(PushButtonState.Pressed, "Icon", false)]
    [InlineData(PushButtonState.Pressed, null, true)]
    [InlineData(PushButtonState.Pressed, null, false)]
    [InlineData(PushButtonState.Disabled, "Icon", true)]
    [InlineData(PushButtonState.Disabled, "Icon", false)]
    [InlineData(PushButtonState.Disabled, null, true)]
    [InlineData(PushButtonState.Disabled, null, false)]
    [InlineData(PushButtonState.Default, "Icon", true)]
    [InlineData(PushButtonState.Default, "Icon", false)]
    [InlineData(PushButtonState.Default, null, true)]
    [InlineData(PushButtonState.Default, null, false)]
    public void DrawButton_TextFontImageImageBoundsFocused_DoesNotThrow(
        PushButtonState state, string? buttonText, bool focused)
    {
        using Bitmap bitmap = new(140, 60);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new(0, 0, 140, 60);
        using Font font = new("Tahoma", 11);
        using Bitmap image = new(24, 24);
        Rectangle imageBounds = new(20, 18, 24, 24);

        Exception? exception = Record.Exception(() =>
            ButtonRenderer.DrawButton(graphics, bounds, buttonText, font, image, imageBounds, focused, state));
        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [InlineData(PushButtonState.Normal, "Combo", TextFormatFlags.Default, true)]
    [InlineData(PushButtonState.Normal, "Combo", TextFormatFlags.Default, false)]
    [InlineData(PushButtonState.Normal, null, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, true)]
    [InlineData(PushButtonState.Normal, "Button", TextFormatFlags.SingleLine, false)]
    [InlineData(PushButtonState.Hot, "Combo", TextFormatFlags.Default, true)]
    [InlineData(PushButtonState.Hot, "Combo", TextFormatFlags.Default, false)]
    [InlineData(PushButtonState.Hot, null, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, true)]
    [InlineData(PushButtonState.Hot, "Button", TextFormatFlags.SingleLine, false)]
    [InlineData(PushButtonState.Pressed, "Combo", TextFormatFlags.Default, true)]
    [InlineData(PushButtonState.Pressed, "Combo", TextFormatFlags.Default, false)]
    [InlineData(PushButtonState.Pressed, null, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, true)]
    [InlineData(PushButtonState.Pressed, "Button", TextFormatFlags.SingleLine, false)]
    [InlineData(PushButtonState.Disabled, "Combo", TextFormatFlags.Default, true)]
    [InlineData(PushButtonState.Disabled, "Combo", TextFormatFlags.Default, false)]
    [InlineData(PushButtonState.Disabled, null, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, true)]
    [InlineData(PushButtonState.Disabled, "Button", TextFormatFlags.SingleLine, false)]
    [InlineData(PushButtonState.Default, "Combo", TextFormatFlags.Default, true)]
    [InlineData(PushButtonState.Default, "Combo", TextFormatFlags.Default, false)]
    [InlineData(PushButtonState.Default, null, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, true)]
    [InlineData(PushButtonState.Default, "Button", TextFormatFlags.SingleLine, false)]
    public void DrawButton_TextFontFlagsImageImageBoundsFocused_DoesNotThrow(
        PushButtonState state,
        string? buttonText,
        TextFormatFlags flags,
        bool focused)
    {
        using Bitmap bitmap = new(160, 70);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new(0, 0, 160, 70);
        using Font font = new("Verdana", 13);
        using Bitmap image = new(32, 32);
        Rectangle imageBounds = new(30, 20, 32, 32);

        Exception? exception = Record.Exception(() =>
            ButtonRenderer.DrawButton(graphics, bounds, buttonText, font, flags, image, imageBounds, focused, state));
        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [InlineData(PushButtonState.Normal, "Advanced", TextFormatFlags.Default, true)]
    [InlineData(PushButtonState.Normal, "Advanced", TextFormatFlags.Default, false)]
    [InlineData(PushButtonState.Normal, null, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, true)]
    [InlineData(PushButtonState.Normal, "Button", TextFormatFlags.SingleLine, false)]
    [InlineData(PushButtonState.Hot, "Advanced", TextFormatFlags.Default, true)]
    [InlineData(PushButtonState.Hot, "Advanced", TextFormatFlags.Default, false)]
    [InlineData(PushButtonState.Hot, null, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, true)]
    [InlineData(PushButtonState.Hot, "Button", TextFormatFlags.SingleLine, false)]
    [InlineData(PushButtonState.Pressed, "Advanced", TextFormatFlags.Default, true)]
    [InlineData(PushButtonState.Pressed, "Advanced", TextFormatFlags.Default, false)]
    [InlineData(PushButtonState.Pressed, null, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, true)]
    [InlineData(PushButtonState.Pressed, "Button", TextFormatFlags.SingleLine, false)]
    [InlineData(PushButtonState.Disabled, "Advanced", TextFormatFlags.Default, true)]
    [InlineData(PushButtonState.Disabled, "Advanced", TextFormatFlags.Default, false)]
    [InlineData(PushButtonState.Disabled, null, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, true)]
    [InlineData(PushButtonState.Disabled, "Button", TextFormatFlags.SingleLine, false)]
    [InlineData(PushButtonState.Default, "Advanced", TextFormatFlags.Default, true)]
    [InlineData(PushButtonState.Default, "Advanced", TextFormatFlags.Default, false)]
    [InlineData(PushButtonState.Default, null, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, true)]
    [InlineData(PushButtonState.Default, "Button", TextFormatFlags.SingleLine, false)]
    public void DrawButton_IDeviceContext_TextFontFlagsImageImageBoundsFocused_DoesNotThrow(
        PushButtonState state,
        string? buttonText,
        TextFormatFlags flags,
        bool focused)
    {
        using Bitmap bitmap = new(180, 80);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new(0, 0, 180, 80);
        using Font font = new("Calibri", 14);
        using Bitmap image = new(40, 40);
        Rectangle imageBounds = new(40, 30, 40, 40);

        ButtonRenderer.RenderMatchingApplicationState = false;
        Exception? exception = Record.Exception(() =>
            ButtonRenderer.DrawButton(graphics, bounds, buttonText, font, flags, image, imageBounds, focused, state));
        exception.Should().BeNull();

        ButtonRenderer.RenderMatchingApplicationState = true;
        Exception? exception2 = Record.Exception(() =>
            ButtonRenderer.DrawButton(graphics, bounds, buttonText, font, flags, image, imageBounds, focused, state));
        exception2.Should().BeNull();
    }
}
