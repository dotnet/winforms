// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class PopupEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_IWin32Window_Control_Bool_Size_TestData()
        {
            yield return new object[] { null, null, false, Size.Empty };
            yield return new object[] { new SubWin32Window(), new Button(), true, new Size(1, 2) };
            yield return new object[] { new SubWin32Window(), new Button(), true, new Size(-1, -2) };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_IWin32Window_Control_Bool_Size_TestData))]
        public void Ctor_IWin32Window_Control_Bool_Size(IWin32Window associatedWindow, Control associatedControl, bool isBalloon, Size size)
        {
            var e = new PopupEventArgs(associatedWindow, associatedControl, isBalloon, size);
            Assert.Equal(associatedWindow, e.AssociatedWindow);
            Assert.Equal(associatedControl, e.AssociatedControl);
            Assert.Equal(isBalloon, e.IsBalloon);
            Assert.Equal(size, e.ToolTipSize);
            Assert.False(e.Cancel);
        }

        public static IEnumerable<object[]> ToolTipSize_TestData()
        {
            yield return new object[] { Size.Empty };
            yield return new object[] { new Size(1, 2) };
            yield return new object[] { new Size(-1, -2) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolTipSize_TestData))]
        public void ToolTipSize_Set_GetReturnsExpected(Size value)
        {
            var e = new PopupEventArgs(new SubWin32Window(), new Button(), true, new Size(1, 2))
            {
                ToolTipSize = value
            };
            Assert.Equal(value, e.ToolTipSize);
        }

        private class SubWin32Window : IWin32Window
        {
            public IntPtr Handle { get; }
        }
    }
}
