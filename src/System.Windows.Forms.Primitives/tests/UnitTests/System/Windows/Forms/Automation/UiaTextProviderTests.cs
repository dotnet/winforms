// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Automation;
using Moq;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace System.Windows.Forms.Primitives.Tests.Automation;

public unsafe class UiaTextProviderTests
{
    [StaFact]
    public void UiaTextProvider_GetEditStyle_ContainsMultilineStyle_ForMultilineTextBox()
    {
        // EditControl Multiline is true when EditControl has ES_MULTILINE style
        using EditControl textBox = new EditControl(
            style: WINDOW_STYLE.WS_OVERLAPPED
                | WINDOW_STYLE.WS_VISIBLE
                | (WINDOW_STYLE)(PInvoke.ES_MULTILINE | PInvoke.ES_LEFT | PInvoke.ES_AUTOHSCROLL | PInvoke.ES_AUTOVSCROLL));
        Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);

        WINDOW_STYLE actual = UiaTextProvider.GetWindowStyle(textBox);
        Assert.True(((int)actual & PInvoke.ES_MULTILINE) != 0);
    }

    [StaFact]
    public void UiaTextProvider_GetEditStyle_DoesntContainMultilineStyle_ForSinglelineTextBox()
    {
        // EditControl Multiline is false when EditControl doesn't have ES_MULTILINE style
        using EditControl textBox = new EditControl(
            style: WINDOW_STYLE.WS_OVERLAPPED
                | WINDOW_STYLE.WS_VISIBLE
                | (WINDOW_STYLE)(PInvoke.ES_LEFT | PInvoke.ES_AUTOHSCROLL | PInvoke.ES_AUTOVSCROLL));
        Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);

        WINDOW_STYLE actual = UiaTextProvider.GetWindowStyle(textBox);
        Assert.False(((int)actual & PInvoke.ES_MULTILINE) != 0);
    }

    [StaFact]
    public void UiaTextProvider_GetWindowStyle_ContainsVisible()
    {
        using EditControl textBox = new EditControl(
            style: WINDOW_STYLE.WS_OVERLAPPED
                | WINDOW_STYLE.WS_VISIBLE
                | (WINDOW_STYLE)(PInvoke.ES_MULTILINE | PInvoke.ES_LEFT | PInvoke.ES_AUTOHSCROLL | PInvoke.ES_AUTOVSCROLL));
        Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);

        WINDOW_STYLE actual = UiaTextProvider.GetWindowStyle(textBox);
        Assert.True(actual.HasFlag(WINDOW_STYLE.WS_VISIBLE));
    }

    [StaFact]
    public void UiaTextProvider_GetWindowExStyle_ContainsClientedge()
    {
        using EditControl textBox = new EditControl(
            style: WINDOW_STYLE.WS_OVERLAPPED | WINDOW_STYLE.WS_VISIBLE);
        Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);

        WINDOW_EX_STYLE actual = UiaTextProvider.GetWindowExStyle(textBox);
        Assert.True(actual.HasFlag(WINDOW_EX_STYLE.WS_EX_CLIENTEDGE));
    }

    [StaFact]
    public void UiaTextProvider_RectArrayToDoubleArray_ReturnsCorrectValue()
    {
        Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);

        double[] expected = { 0, 0, 10, 5, 10, 10, 20, 30 };
        using SafeArrayScope<double> actual = UiaTextProvider.RectListToDoubleArray(new List<Rectangle>
        {
            new(0, 0, 10, 5),
            new(10, 10, 20, 30)
        });

        Assert.Equal(8, actual.Length);

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], actual[i]);
        }
    }

#pragma warning disable CS8625 // RectArrayToDoubleArray doesn't accept a null parameter
    [StaFact]
    public void UiaTextProvider_RectArrayToDoubleArray_NullParameter_ReturnsNull()
    {
        Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);

        using SafeArrayScope<double> actual = UiaTextProvider.RectListToDoubleArray(null);
        Assert.True(actual.IsEmpty);
    }
#pragma warning restore CS8625

    [StaFact]
    public void UiaTextProvider_RectArrayToDoubleArray_EmptyArrayParameter_ReturnsEmptyArrayResult()
    {
        Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);

        using SafeArrayScope<double> actual = UiaTextProvider.RectListToDoubleArray(new List<Rectangle>());
        Assert.True(actual.IsEmpty);
    }

    [StaFact]
    public unsafe void UiaTextProvider_SendInput_SendsOneInput()
    {
        Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);
        INPUT keyboardInput = new INPUT();

        int actual = providerMock.Object.SendInput(1, ref keyboardInput, sizeof(INPUT));
        Assert.Equal(1, actual);
    }

    [StaFact]
    public void UiaTextProvider_SendKeyboardInputVK_SendsOneInput()
    {
        Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);

        int actual = providerMock.Object.SendKeyboardInputVK(VIRTUAL_KEY.VK_LEFT, true);
        Assert.Equal(1, actual);
    }
}
