﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Moq;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DrawToolTipEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_Graphics_IWin32Window_Control_Rectangle_String_Color_Color_Font_TestData()
        {
            yield return new object[] { null, null, Rectangle.Empty, null, Color.Empty, Color.Empty, null };
            yield return new object[] { new Mock<IWin32Window>(MockBehavior.Strict).Object, new Button(), new Rectangle(1, 2, 3, 4), "", Color.Red, Color.Blue, SystemFonts.DefaultFont };
            yield return new object[] { new Mock<IWin32Window>(MockBehavior.Strict).Object, new Button(), new Rectangle(-1, -2, -3, -4), "toolTipText", Color.Red, Color.Blue, SystemFonts.DefaultFont };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_IWin32Window_Control_Rectangle_String_Color_Color_Font_TestData))]
        public void Ctor_Graphics_IWin32Window_Control_Rectangle_String_Color_Color_Font(IWin32Window associatedWindow, Control associatedControl, Rectangle bounds, string toolTipText, Color backColor, Color foreColor, Font font)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawToolTipEventArgs(graphics, associatedWindow, associatedControl, bounds, toolTipText, backColor, foreColor, font);
                Assert.Same(graphics, e.Graphics);
                Assert.Same(associatedWindow, e.AssociatedWindow);
                Assert.Same(associatedControl, e.AssociatedControl);
                Assert.Equal(bounds, e.Bounds);
                Assert.Equal(toolTipText, e.ToolTipText);
                Assert.Same(font, e.Font);
            }
        }

        public static IEnumerable<object[]> Draw_TestData()
        {
            yield return new object[] { null, null, new Rectangle(-1, -2, -3, -4), null, Color.Empty, Color.Empty, null };
            yield return new object[] { null, null, new Rectangle(-1, -2, -3, -4), string.Empty, Color.Empty, Color.Empty, null };
            yield return new object[] { null, null, new Rectangle(-1, -2, -3, -4), "tooltipText", Color.Empty, Color.Empty, null };
            yield return new object[] { new SubWin32Window(), new Button(), new Rectangle(1, 2, 3, 4), null, Color.Red, Color.Blue, SystemFonts.DefaultFont };
            yield return new object[] { new SubWin32Window(), new Button(), new Rectangle(1, 2, 3, 4), string.Empty, Color.Red, Color.Blue, SystemFonts.DefaultFont };
            yield return new object[] { new SubWin32Window(), new Button(), new Rectangle(1, 2, 3, 4), "tooltipText", Color.Red, Color.Blue, SystemFonts.DefaultFont };
        }

        [Theory]
        [MemberData(nameof(Draw_TestData))]
        public void DrawBackground_Invoke_Success(IWin32Window associatedWindow, Control associatedControl, Rectangle bounds, string toolTipText, Color backColor, Color foreColor, Font font)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawToolTipEventArgs(graphics, associatedWindow, associatedControl, bounds, toolTipText, backColor, foreColor, font);
                e.DrawBackground();
            }
        }

        public static IEnumerable<object[]> DrawText_TestData()
        {
            yield return new object[] { "tooltipText", SystemFonts.DefaultFont };
            yield return new object[] { "tooltipText", null };
            yield return new object[] { string.Empty, SystemFonts.DefaultFont };
            yield return new object[] { null, SystemFonts.DefaultFont };
        }

        [Theory]
        [MemberData(nameof(Draw_TestData))]
        public void DrawText_Invoke_Success(IWin32Window associatedWindow, Control associatedControl, Rectangle bounds, string toolTipText, Color backColor, Color foreColor, Font font)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawToolTipEventArgs(graphics, associatedWindow, associatedControl, bounds, toolTipText, backColor, foreColor, font);
                e.DrawText();
            }
        }

        [Theory]
        [MemberData(nameof(Draw_TestData))]
        public void DrawText_InvokeTextFormatFlags_Success(IWin32Window associatedWindow, Control associatedControl, Rectangle bounds, string toolTipText, Color backColor, Color foreColor, Font font)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawToolTipEventArgs(graphics, associatedWindow, associatedControl, bounds, toolTipText, backColor, foreColor, font);
                e.DrawText(TextFormatFlags.Bottom);
            }
        }

        [Theory]
        [MemberData(nameof(Draw_TestData))]
        public void DrawBorder_Invoke_Success(IWin32Window associatedWindow, Control associatedControl, Rectangle bounds, string toolTipText, Color backColor, Color foreColor, Font font)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawToolTipEventArgs(graphics, associatedWindow, associatedControl, bounds, toolTipText, backColor, foreColor, font);
                e.DrawBorder();
            }
        }

        private class SubWin32Window : IWin32Window
        {
            public IntPtr Handle { get; }
        }
    }
}
