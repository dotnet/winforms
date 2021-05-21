// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.Automation;
using Moq;
using Xunit;
using static Interop.User32;

namespace System.Windows.Forms.Primitives.Tests.Automation
{
    public class UiaTextProviderTests
    {
        [StaFact]
        public void UiaTextProvider_GetEditStyle_ContainsMultilineStyle_ForMultilineTextBox()
        {
            // EditControl Multiline is true when EditControl has ES_MULTILINE style
            using EditControl textBox = new EditControl(
                editStyle: ES.MULTILINE | ES.LEFT | ES.AUTOHSCROLL | ES.AUTOVSCROLL,
                style: WS.OVERLAPPED | WS.VISIBLE);
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);

            ES actual = providerMock.Object.GetEditStyle(textBox);
            Assert.True(actual.HasFlag(ES.MULTILINE));
        }

        [StaFact]
        public void UiaTextProvider_GetEditStyle_DoesntContainMultilineStyle_ForSinglelineTextBox()
        {
            // EditControl Multiline is false when EditControl doesn't have ES_MULTILINE style
            using EditControl textBox = new EditControl(
                editStyle: ES.LEFT | ES.AUTOHSCROLL | ES.AUTOVSCROLL,
                style: WS.OVERLAPPED | WS.VISIBLE);
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);

            ES actual = providerMock.Object.GetEditStyle(textBox);
            Assert.False(actual.HasFlag(ES.MULTILINE));
        }

        [StaFact]
        public void UiaTextProvider_GetWindowStyle_ContainsVisible()
        {
            using EditControl textBox = new EditControl(
                editStyle: ES.MULTILINE | ES.LEFT | ES.AUTOHSCROLL | ES.AUTOVSCROLL,
                style: WS.OVERLAPPED | WS.VISIBLE);
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);

            WS actual = providerMock.Object.GetWindowStyle(textBox);
            Assert.True(actual.HasFlag(WS.VISIBLE));
        }

        [StaFact]
        public void UiaTextProvider_GetWindowExStyle_ContainsClientedge()
        {
            using EditControl textBox = new EditControl(
                style: WS.OVERLAPPED | WS.VISIBLE);
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);

            WS_EX actual = providerMock.Object.GetWindowExStyle(textBox);
            Assert.True(actual.HasFlag(WS_EX.CLIENTEDGE));
        }

        [StaFact]
        public void UiaTextProvider_RectArrayToDoubleArray_ReturnsCorrectValue()
        {
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);

            double[] expected = { 0, 0, 10, 5, 10, 10, 20, 30 };
            double[] actual = providerMock.Object.RectListToDoubleArray(new List<Rectangle>
            {
                new Rectangle(0, 0, 10, 5),
                new Rectangle(10, 10, 20, 30)
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

            double[] actual = providerMock.Object.RectListToDoubleArray(null);
            Assert.Empty(actual);
        }
#pragma warning restore CS8625

        [StaFact]
        public void UiaTextProvider_RectArrayToDoubleArray_EmptyArrayParameter_ReturnsEmptyArrayResult()
        {
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);

            double[] actual = providerMock.Object.RectListToDoubleArray(new List<Rectangle>());
            Assert.Empty(actual);
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

            int actual = providerMock.Object.SendKeyboardInputVK(VK.LEFT, true);
            Assert.Equal(1, actual);
        }
    }
}
