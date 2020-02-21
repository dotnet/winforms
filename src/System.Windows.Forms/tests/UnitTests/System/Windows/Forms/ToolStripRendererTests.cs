// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    using Size = System.Drawing.Size;

    public class ToolStripRendererTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripRenderer_CreateDisabledImage_Invoke_Success()
        {
            using var image = new Bitmap(10, 11);
            Image result = Assert.IsType<Bitmap>(ToolStripRenderer.CreateDisabledImage(image));
            Assert.NotSame(result, image);
            Assert.Equal(new Size(10, 11), result.Size);
        }

        [WinFormsFact]
        public void ToolStripRenderer_CreateDisabledImage_NullImage_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("normalImage", () => ToolStripRenderer.CreateDisabledImage(null));
        }

        public static IEnumerable<object[]> ToolStripArrowRenderEventArgs_TestData()
        {
            foreach (ArrowDirection arrowDirection in Enum.GetValues(typeof(ArrowDirection)))
            {
                yield return new object[] { null, Rectangle.Empty, Color.Empty, arrowDirection };
                yield return new object[] { new SubToolStripItem(), new Rectangle(1, 2, 3, 4), Color.Red, arrowDirection };
            }

            yield return new object[] { null, Rectangle.Empty, Color.Empty, ArrowDirection.Left - 1 };
            yield return new object[] { null, Rectangle.Empty, Color.Empty, ArrowDirection.Up + 1 };
            yield return new object[] { new SubToolStripItem(), new Rectangle(1, 2, 3, 4), Color.Red, ArrowDirection.Left - 1 };
            yield return new object[] { new SubToolStripItem(), new Rectangle(1, 2, 3, 4), Color.Red, ArrowDirection.Up + 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripArrowRenderEventArgs_TestData))]
        public void ToolStripRenderer_DrawArrow_Invoke_CallsRenderArrow(ToolStripItem toolStripItem, Rectangle arrowRectangle, Color arrowColor, ArrowDirection arrowDirection)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new ToolStripArrowRenderEventArgs(graphics, toolStripItem, arrowRectangle, arrowColor, arrowDirection);

            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            ToolStripArrowRenderEventHandler handler = (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            renderer.RenderArrow += handler;
            renderer.DrawArrow(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            renderer.RenderArrow -= handler;
            renderer.DrawArrow(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void ToolStripRenderer_DrawArrow_NullE_ThrowsArgumentNullException()
        {
            var renderer = new SubToolStripRenderer();
            Assert.Throws<ArgumentNullException>("e", () => renderer.DrawArrow(null));
        }

        [WinFormsFact]
        public void ToolStripRenderer_DrawArrow_NullEGraphics_ThrowsNullReferenceException()
        {
            var renderer = new SubToolStripRenderer();
            var e = new ToolStripArrowRenderEventArgs(null, new SubToolStripItem(), Rectangle.Empty, Color.Red, ArrowDirection.Up);
            Assert.Throws<NullReferenceException>(() => renderer.DrawArrow(e));
        }

        public static IEnumerable<object[]> ToolStripItemRenderEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ToolStripItemRenderEventArgs(null, null) };

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            yield return new object[] { new ToolStripItemRenderEventArgs(graphics, new SubToolStripItem()) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemRenderEventArgs_TestData))]
        public void ToolStripRenderer_DrawButtonBackground_Invoke_CallsRenderButtonBackground(ToolStripItemRenderEventArgs eventArgs)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            ToolStripItemRenderEventHandler handler = (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            renderer.RenderButtonBackground += handler;
            renderer.DrawButtonBackground(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            renderer.RenderButtonBackground -= handler;
            renderer.DrawButtonBackground(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemRenderEventArgs_TestData))]
        public void ToolStripRenderer_DrawDropDownButtonBackground_Invoke_CallsRenderDropDownButtonBackground(ToolStripItemRenderEventArgs eventArgs)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            ToolStripItemRenderEventHandler handler = (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            renderer.RenderDropDownButtonBackground += handler;
            renderer.DrawDropDownButtonBackground(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            renderer.RenderDropDownButtonBackground -= handler;
            renderer.DrawDropDownButtonBackground(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> ToolStripGripRenderEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ToolStripGripRenderEventArgs(null, new ToolStrip()) };

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            yield return new object[] { new ToolStripGripRenderEventArgs(graphics, new ToolStrip()) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripGripRenderEventArgs_TestData))]
        public void ToolStripRenderer_DrawGrip_Invoke_CallsRenderGrip(ToolStripGripRenderEventArgs eventArgs)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            ToolStripGripRenderEventHandler handler = (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            renderer.RenderGrip += handler;
            renderer.DrawGrip(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            renderer.RenderGrip -= handler;
            renderer.DrawGrip(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemRenderEventArgs_TestData))]
        public void ToolStripRenderer_DrawItemBackground_Invoke_CallsRenderItemBackground(ToolStripItemRenderEventArgs eventArgs)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            ToolStripItemRenderEventHandler handler = (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            renderer.RenderItemBackground += handler;
            renderer.DrawItemBackground(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            renderer.RenderItemBackground -= handler;
            renderer.DrawItemBackground(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> ToolStripItemImageRenderEventArgs_TestData()
        {
            yield return new object[] { null, null, null, Rectangle.Empty };
            yield return new object[] { null, null, null, new Rectangle(1, 2, 3, 4) };
            yield return new object[] { null, null, new Bitmap(10, 10), Rectangle.Empty };

            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);
            yield return new object[] { graphics, new SubToolStripItem(), new Bitmap(10, 10), new Rectangle(1, 2, 3, 4) };
            yield return new object[] { graphics, new SubToolStripItem { Enabled = false }, new Bitmap(10, 10), new Rectangle(1, 2, 3, 4) };

            foreach (ToolStripItemImageScaling imageScaling in Enum.GetValues(typeof(ToolStripItemImageScaling)))
            {
                yield return new object[] { graphics, new SubToolStripItem { ImageScaling = imageScaling }, new Bitmap(10, 10), new Rectangle(1, 2, 3, 4) };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemImageRenderEventArgs_TestData))]
        public void ToolStripRenderer_DrawItemCheck_Invoke_CallsRenderItemCheck(Graphics graphics, ToolStripItem item, Image image, Rectangle imageRectangle)
        {
            var eventArgs = new ToolStripItemImageRenderEventArgs(graphics, item, image, imageRectangle);

            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            ToolStripItemImageRenderEventHandler handler = (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            renderer.RenderItemCheck += handler;
            renderer.DrawItemCheck(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            renderer.RenderItemCheck -= handler;
            renderer.DrawItemCheck(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void ToolStripRenderer_DrawItemCheck_NullE_ThrowsArgumentNullException()
        {
            var renderer = new SubToolStripRenderer();
            Assert.Throws<ArgumentNullException>("e", () => renderer.DrawItemCheck(null));
        }

        [WinFormsFact]
        public void ToolStripRenderer_DrawItemCheck_NullEGraphics_ThrowsNullReferenceException()
        {
            var renderer = new SubToolStripRenderer();
            using var image = new Bitmap(10, 10);
            using var item = new SubToolStripItem();
            var e = new ToolStripItemImageRenderEventArgs(null, item, image, new Rectangle(1, 2, 3, 4));
            Assert.Throws<NullReferenceException>(() => renderer.DrawItemCheck(e));
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemImageRenderEventArgs_TestData))]
        public void ToolStripRenderer_DrawItemImage_Invoke_CallsRenderItemImage(Graphics graphics, ToolStripItem item, Image image, Rectangle imageRectangle)
        {
            var eventArgs = new ToolStripItemImageRenderEventArgs(graphics, item, image, imageRectangle);

            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            ToolStripItemImageRenderEventHandler handler = (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            renderer.RenderItemImage += handler;
            renderer.DrawItemImage(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            renderer.RenderItemImage -= handler;
            renderer.DrawItemImage(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void ToolStripRenderer_DrawItemImage_NullE_ThrowsArgumentNullException()
        {
            var renderer = new SubToolStripRenderer();
            Assert.Throws<ArgumentNullException>("e", () => renderer.DrawItemImage(null));
        }

        [WinFormsFact]
        public void ToolStripRenderer_DrawItemImage_NullEGraphics_ThrowsNullReferenceException()
        {
            var renderer = new SubToolStripRenderer();
            using var image = new Bitmap(10, 10);
            using var item = new SubToolStripItem();
            var e = new ToolStripItemImageRenderEventArgs(null, item, image, new Rectangle(1, 2, 3, 4));
            Assert.Throws<NullReferenceException>(() => renderer.DrawItemImage(e));
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripRenderEventArgs_TestData))]
        public void ToolStripRenderer_DrawImageMargin_Invoke_CallsRenderImageMargin(ToolStripRenderEventArgs eventArgs)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            ToolStripRenderEventHandler handler = (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            renderer.RenderImageMargin += handler;
            renderer.DrawImageMargin(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            renderer.RenderImageMargin -= handler;
            renderer.DrawImageMargin(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> ToolStripItemTextRenderEventArgs_TestData()
        {
            yield return new object[] { new SubToolStripItem(), null, Rectangle.Empty, Color.Empty, null, TextFormatFlags.Left };
            yield return new object[] { new SubToolStripItem(), null, new Rectangle(1, 2, 0, 4), Color.Empty, null, TextFormatFlags.Left };
            yield return new object[] { new SubToolStripItem(), null, new Rectangle(1, 2, 3, 0), Color.Empty, null, TextFormatFlags.Left };
            yield return new object[] { new SubToolStripItem(), null, new Rectangle(1, 2, 3, 4), Color.Empty, null, TextFormatFlags.Left };

            yield return new object[] { new SubToolStripItem(), "Text", Rectangle.Empty, Color.Red, SystemFonts.MenuFont, TextFormatFlags.Left };
            yield return new object[] { new SubToolStripItem(), "Text", new Rectangle(1, 2, 0, 4), Color.Red, SystemFonts.MenuFont, TextFormatFlags.Left };
            yield return new object[] { new SubToolStripItem(), "Text", new Rectangle(1, 2, 3, 0), Color.Red, SystemFonts.MenuFont, TextFormatFlags.Left };
            yield return new object[] { new SubToolStripItem(), "Text", new Rectangle(1, 2, 3, 4), Color.Red, SystemFonts.MenuFont, TextFormatFlags.Left };
            yield return new object[] { new SubToolStripItem() { Enabled = false }, "Text", new Rectangle(1, 2, 3, 4), Color.Red, SystemFonts.MenuFont, TextFormatFlags.Left };
            yield return new object[] { new SubToolStripItem() { TextDirection = ToolStripTextDirection.Vertical270 }, "Text", new Rectangle(1, 2, 3, 4), Color.Red, SystemFonts.MenuFont, TextFormatFlags.Left };
            yield return new object[] { new SubToolStripItem() { TextDirection = ToolStripTextDirection.Vertical90 }, "Text", new Rectangle(1, 2, 3, 4), Color.Red, SystemFonts.MenuFont, TextFormatFlags.Left };
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemTextRenderEventArgs_TestData))]
        public void ToolStripRenderer_DrawItemText_Invoke_CallsRenderItemText(ToolStripItem item, string text, Rectangle textRectangle, Color textColor, Font textFont, TextFormatFlags format)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new ToolStripItemTextRenderEventArgs(graphics, item, text, textRectangle, textColor, textFont, format);

            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            ToolStripItemTextRenderEventHandler handler = (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            renderer.RenderItemText += handler;
            renderer.DrawItemText(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            renderer.RenderItemText -= handler;
            renderer.DrawItemText(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void ToolStripRenderer_DrawItemText_NullE_ThrowsArgumentNullException()
        {
            var renderer = new SubToolStripRenderer();
            Assert.Throws<ArgumentNullException>("e", () => renderer.DrawItemText(null));
        }

        [WinFormsFact]
        public void ToolStripRenderer_DrawItemText_NullEGraphics_ThrowsNullReferenceException()
        {
            var renderer = new SubToolStripRenderer();
            using var item = new SubToolStripItem();
            var e = new ToolStripItemTextRenderEventArgs(null, item, string.Empty, Rectangle.Empty, Color.Red, SystemFonts.MenuFont, TextFormatFlags.Left);
            Assert.Throws<ArgumentNullException>("dc", () => renderer.DrawItemText(e));
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemRenderEventArgs_TestData))]
        public void ToolStripRenderer_DrawLabelBackground_Invoke_CallsRenderLabelBackground(ToolStripItemRenderEventArgs eventArgs)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            ToolStripItemRenderEventHandler handler = (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            renderer.RenderLabelBackground += handler;
            renderer.DrawLabelBackground(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            renderer.RenderLabelBackground -= handler;
            renderer.DrawLabelBackground(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemRenderEventArgs_TestData))]
        public void ToolStripRenderer_DrawMenuItemBackground_Invoke_CallsRenderMenuItemBackground(ToolStripItemRenderEventArgs eventArgs)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            ToolStripItemRenderEventHandler handler = (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            renderer.RenderMenuItemBackground += handler;
            renderer.DrawMenuItemBackground(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            renderer.RenderMenuItemBackground -= handler;
            renderer.DrawMenuItemBackground(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemRenderEventArgs_TestData))]
        public void ToolStripRenderer_DrawOverflowButtonBackground_Invoke_CallsRenderOverflowButtonBackground(ToolStripItemRenderEventArgs eventArgs)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            ToolStripItemRenderEventHandler handler = (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            renderer.RenderOverflowButtonBackground += handler;
            renderer.DrawOverflowButtonBackground(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            renderer.RenderOverflowButtonBackground -= handler;
            renderer.DrawOverflowButtonBackground(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> ToolStripSeparatorRenderEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ToolStripSeparatorRenderEventArgs(null, null, true) };
            yield return new object[] { new ToolStripSeparatorRenderEventArgs(null, null, false) };

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            yield return new object[] { new ToolStripSeparatorRenderEventArgs(graphics, null, true) };
            yield return new object[] { new ToolStripSeparatorRenderEventArgs(graphics, null, false) };
            yield return new object[] { new ToolStripSeparatorRenderEventArgs(graphics, new ToolStripSeparator(), true) };
            yield return new object[] { new ToolStripSeparatorRenderEventArgs(graphics, new ToolStripSeparator(), false) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripSeparatorRenderEventArgs_TestData))]
        public void ToolStripRenderer_DrawSeparator_Invoke_CallsRenderSeparator(ToolStripSeparatorRenderEventArgs eventArgs)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            ToolStripSeparatorRenderEventHandler handler = (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            renderer.RenderSeparator += handler;
            renderer.DrawSeparator(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            renderer.RenderSeparator -= handler;
            renderer.DrawSeparator(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemRenderEventArgs_TestData))]
        public void ToolStripRenderer_DrawSplitButton_Invoke_CallsRenderSplitButtonBackground(ToolStripItemRenderEventArgs eventArgs)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            ToolStripItemRenderEventHandler handler = (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            renderer.RenderSplitButtonBackground += handler;
            renderer.DrawSplitButton(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            renderer.RenderSplitButtonBackground -= handler;
            renderer.DrawSplitButton(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> StatusStripSizingGrip_TestData()
        {
            yield return new object[] { new ToolStrip() };
            yield return new object[] { new StatusStrip() };
            yield return new object[] { new StatusStrip { GripStyle = ToolStripGripStyle.Hidden } };
            yield return new object[] { new StatusStrip { RightToLeft = RightToLeft.Yes } };
        }

        [WinFormsTheory]
        [MemberData(nameof(StatusStripSizingGrip_TestData))]
        public void ToolStripRenderer_DrawStatusStripSizingGrip_Invoke_CallsRenderStatusStripSizingGrip(ToolStrip toolStrip)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new ToolStripRenderEventArgs(graphics, toolStrip);

            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            ToolStripRenderEventHandler handler = (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            renderer.RenderStatusStripSizingGrip += handler;
            renderer.DrawStatusStripSizingGrip(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            renderer.RenderStatusStripSizingGrip -= handler;
            renderer.DrawStatusStripSizingGrip(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void DrawStatusStripSizingGrip_NullE_ThrowsArgumentNullException()
        {
            var renderer = new SubToolStripRenderer();
            Assert.Throws<ArgumentNullException>("e", () => renderer.DrawStatusStripSizingGrip(null));
        }

        [WinFormsFact]
        public void DrawStatusStripSizingGrip_NullEGraphics_ThrowsNullReferenceException()
        {
            var renderer = new SubToolStripRenderer();
            var e = new ToolStripRenderEventArgs(null, new StatusStrip());
            Assert.Throws<NullReferenceException>(() => renderer.DrawStatusStripSizingGrip(e));
        }

        public static IEnumerable<object[]> ToolStripRenderEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ToolStripRenderEventArgs(null, new ToolStrip()) };

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            yield return new object[] { new ToolStripRenderEventArgs(graphics, new ToolStrip()) };

            yield return new object[] { new ToolStripRenderEventArgs(graphics, new StatusStrip()) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripRenderEventArgs_TestData))]
        public void ToolStripRenderer_DrawToolStripBackground_Invoke_CallsRenderToolStripBackground(ToolStripRenderEventArgs eventArgs)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            ToolStripRenderEventHandler handler = (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            renderer.RenderToolStripBackground += handler;
            renderer.DrawToolStripBackground(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            renderer.RenderToolStripBackground -= handler;
            renderer.DrawToolStripBackground(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripRenderEventArgs_TestData))]
        public void ToolStripRenderer_DrawToolStripBorder_Invoke_CallsRenderToolStripBorder(ToolStripRenderEventArgs eventArgs)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            ToolStripRenderEventHandler handler = (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            renderer.RenderToolStripBorder += handler;
            renderer.DrawToolStripBorder(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            renderer.RenderToolStripBorder -= handler;
            renderer.DrawToolStripBorder(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripContentPanelRenderEventArgs_TestData))]
        public void ToolStripRenderer_DrawToolStripContentPanelBackground_Invoke_CallsRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs eventArgs)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            ToolStripContentPanelRenderEventHandler handler = (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            renderer.RenderToolStripContentPanelBackground += handler;
            renderer.DrawToolStripContentPanelBackground(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            renderer.RenderToolStripContentPanelBackground -= handler;
            renderer.DrawToolStripContentPanelBackground(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> ToolStripPanelRenderEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ToolStripPanelRenderEventArgs(null, null) };

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            yield return new object[] { new ToolStripPanelRenderEventArgs(graphics, null) };
            yield return new object[] { new ToolStripPanelRenderEventArgs(graphics, new ToolStripPanel()) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripPanelRenderEventArgs_TestData))]
        public void ToolStripRenderer_DrawToolStripPanelBackground_Invoke_CallsRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs eventArgs)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            ToolStripPanelRenderEventHandler handler = (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            renderer.RenderToolStripPanelBackground += handler;
            renderer.DrawToolStripPanelBackground(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            renderer.RenderToolStripPanelBackground -= handler;
            renderer.DrawToolStripPanelBackground(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemRenderEventArgs_TestData))]
        public void ToolStripRenderer_DrawToolStripStatusLabelBackground_Invoke_CallsRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs eventArgs)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            ToolStripItemRenderEventHandler handler = (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            renderer.RenderToolStripStatusLabelBackground += handler;
            renderer.DrawToolStripStatusLabelBackground(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            renderer.RenderToolStripStatusLabelBackground -= handler;
            renderer.DrawToolStripStatusLabelBackground(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> ToolStripContentPanelRenderEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ToolStripContentPanelRenderEventArgs(null, null) };

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            yield return new object[] { new ToolStripContentPanelRenderEventArgs(graphics, null) };
            yield return new object[] { new ToolStripContentPanelRenderEventArgs(graphics, new ToolStripContentPanel()) };
        }

        public static IEnumerable<object[]> Initialize_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ToolStrip() };
        }

        [WinFormsTheory]
        [MemberData(nameof(Initialize_TestData))]
        public void ToolStripRenderer_Initialize_Invoke_Nop(ToolStrip toolStrip)
        {
            var renderer = new SubToolStripRenderer();
            renderer.Initialize(toolStrip);
        }

        public static IEnumerable<object[]> InitializeContentPanel_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ToolStripContentPanel() };
        }

        [WinFormsTheory]
        [MemberData(nameof(InitializeContentPanel_TestData))]
        public void ToolStripRenderer_InitializeContentPanel_Invoke_Nop(ToolStripContentPanel contentPanel)
        {
            var renderer = new SubToolStripRenderer();
            renderer.InitializeContentPanel(contentPanel);
        }

        public static IEnumerable<object[]> InitializeItem_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new SubToolStripItem() };
        }

        [WinFormsTheory]
        [MemberData(nameof(InitializeItem_TestData))]
        public void ToolStripRenderer_InitializeItem_Invoke_Nop(ToolStripItem item)
        {
            var renderer = new SubToolStripRenderer();
            renderer.InitializeItem(item);
        }

        public static IEnumerable<object[]> InitializePanel_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ToolStripPanel() };
        }

        [WinFormsTheory]
        [MemberData(nameof(InitializePanel_TestData))]
        public void ToolStripRenderer_InitializePanel_Invoke_Nop(ToolStripPanel toolStripPanel)
        {
            var renderer = new SubToolStripRenderer();
            renderer.InitializePanel(toolStripPanel);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripArrowRenderEventArgs_TestData))]
        public void ToolStripRenderer_OnRenderArrow_Invoke_Nop(ToolStripItem toolStripItem, Rectangle arrowRectangle, Color arrowColor, ArrowDirection arrowDirection)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var e = new ToolStripArrowRenderEventArgs(graphics, toolStripItem, arrowRectangle, arrowColor, arrowDirection);

            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            renderer.RenderArrow += (sender, e) => callCount++;
            renderer.OnRenderArrow(e);
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public void ToolStripRenderer_OnRenderArrow_NullE_ThrowsArgumentNullException()
        {
            var renderer = new SubToolStripRenderer();
            Assert.Throws<ArgumentNullException>("e", () => renderer.OnRenderArrow(null));
        }

        [WinFormsFact]
        public void ToolStripRenderer_OnRenderArrow_NullEGraphics_ThrowsNullReferenceException()
        {
            var renderer = new SubToolStripRenderer();
            var e = new ToolStripArrowRenderEventArgs(null, new SubToolStripItem(), Rectangle.Empty, Color.Red, ArrowDirection.Up);
            Assert.Throws<NullReferenceException>(() => renderer.OnRenderArrow(e));
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemRenderEventArgs_TestData))]
        public void ToolStripRenderer_OnRenderButtonBackground_Invoke_Nop(ToolStripItemRenderEventArgs e)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            renderer.RenderButtonBackground += (sender, e) => callCount++;
            renderer.OnRenderButtonBackground(e);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemRenderEventArgs_TestData))]
        public void ToolStripRenderer_OnRenderDropDownButtonBackground_Invoke_Nop(ToolStripItemRenderEventArgs e)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            renderer.RenderDropDownButtonBackground += (sender, e) => callCount++;
            renderer.OnRenderDropDownButtonBackground(e);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripGripRenderEventArgs_TestData))]
        public void ToolStripRenderer_OnRenderGrip_Invoke_Nop(ToolStripGripRenderEventArgs e)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            renderer.RenderGrip += (sender, e) => callCount++;
            renderer.OnRenderGrip(e);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripRenderEventArgs_TestData))]
        public void ToolStripRenderer_OnRenderImageMargin_Invoke_Nop(ToolStripRenderEventArgs e)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            renderer.RenderImageMargin += (sender, e) => callCount++;
            renderer.OnRenderImageMargin(e);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemRenderEventArgs_TestData))]
        public void ToolStripRenderer_OnRenderItemBackground_Invoke_Nop(ToolStripItemRenderEventArgs e)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            renderer.RenderItemBackground += (sender, e) => callCount++;
            renderer.OnRenderItemBackground(e);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemImageRenderEventArgs_TestData))]
        public void ToolStripRenderer_OnRenderItemCheck_Invoke_Nop(Graphics graphics, ToolStripItem item, Image image, Rectangle imageRectangle)
        {
            var e = new ToolStripItemImageRenderEventArgs(graphics, item, image, imageRectangle);

            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            renderer.RenderItemCheck += (sender, e) => callCount++;
            renderer.OnRenderItemCheck(e);
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public void ToolStripRenderer_OnRenderItemCheck_NullE_ThrowsArgumentNullException()
        {
            var renderer = new SubToolStripRenderer();
            Assert.Throws<ArgumentNullException>("e", () => renderer.OnRenderItemCheck(null));
        }

        [WinFormsFact]
        public void ToolStripRenderer_OnRenderItemCheck_NullEGraphics_ThrowsNullReferenceException()
        {
            var renderer = new SubToolStripRenderer();
            using var image = new Bitmap(10, 10);
            using var item = new SubToolStripItem();
            var e = new ToolStripItemImageRenderEventArgs(null, item, image, new Rectangle(1, 2, 3, 4));
            Assert.Throws<NullReferenceException>(() => renderer.OnRenderItemCheck(e));
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemImageRenderEventArgs_TestData))]
        public void ToolStripRenderer_OnRenderItemImage_Invoke_Nop(Graphics graphics, ToolStripItem item, Image image, Rectangle imageRectangle)
        {
            var e = new ToolStripItemImageRenderEventArgs(graphics, item, image, imageRectangle);

            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            renderer.RenderItemImage += (sender, e) => callCount++;
            renderer.OnRenderItemImage(e);
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public void ToolStripRenderer_OnRenderItemImage_NullE_ThrowsArgumentNullException()
        {
            var renderer = new SubToolStripRenderer();
            Assert.Throws<ArgumentNullException>("e", () => renderer.OnRenderItemImage(null));
        }

        [WinFormsFact]
        public void ToolStripRenderer_OnRenderItemImage_NullEGraphics_ThrowsNullReferenceException()
        {
            var renderer = new SubToolStripRenderer();
            using var image = new Bitmap(10, 10);
            using var item = new SubToolStripItem();
            var e = new ToolStripItemImageRenderEventArgs(null, item, image, new Rectangle(1, 2, 3, 4));
            Assert.Throws<NullReferenceException>(() => renderer.OnRenderItemImage(e));
        }

        [WinFormsFact]
        public void ToolStripRenderer_OnRenderItemImage_NullEItem_ThrowsNullReferenceException()
        {
            var renderer = new SubToolStripRenderer();
            using var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);
            var e = new ToolStripItemImageRenderEventArgs(graphics, null, image, new Rectangle(1, 2, 3, 4));
            Assert.Throws<NullReferenceException>(() => renderer.OnRenderItemImage(e));
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemTextRenderEventArgs_TestData))]
        public void ToolStripRenderer_OnRenderItemText_Invoke_Nop(ToolStripItem item, string text, Rectangle textRectangle, Color textColor, Font textFont, TextFormatFlags format)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var e = new ToolStripItemTextRenderEventArgs(graphics, item, text, textRectangle, textColor, textFont, format);

            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            renderer.RenderItemText += (sender, e) => callCount++;
            renderer.OnRenderItemText(e);
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public void ToolStripRenderer_OnRenderItemText_NullE_ThrowsArgumentNullException()
        {
            var renderer = new SubToolStripRenderer();
            Assert.Throws<ArgumentNullException>("e", () => renderer.OnRenderItemText(null));
        }

        [WinFormsFact]
        public void ToolStripRenderer_OnRenderItemText_NullEGraphics_ThrowsNullReferenceException()
        {
            var renderer = new SubToolStripRenderer();
            using var item = new SubToolStripItem();
            var e = new ToolStripItemTextRenderEventArgs(null, item, string.Empty, Rectangle.Empty, Color.Red, SystemFonts.MenuFont, TextFormatFlags.Left);
            Assert.Throws<ArgumentNullException>("dc", () => renderer.OnRenderItemText(e));
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemRenderEventArgs_TestData))]
        public void ToolStripRenderer_OnRenderLabelBackground_Invoke_Nop(ToolStripItemRenderEventArgs e)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            renderer.RenderLabelBackground += (sender, e) => callCount++;
            renderer.OnRenderLabelBackground(e);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemRenderEventArgs_TestData))]
        public void ToolStripRenderer_OnRenderMenuItemBackground_Invoke_Nop(ToolStripItemRenderEventArgs e)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            renderer.RenderMenuItemBackground += (sender, e) => callCount++;
            renderer.OnRenderMenuItemBackground(e);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemRenderEventArgs_TestData))]
        public void ToolStripRenderer_OnRenderOverflowButtonBackground_Invoke_Nop(ToolStripItemRenderEventArgs e)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            renderer.RenderOverflowButtonBackground += (sender, e) => callCount++;
            renderer.OnRenderOverflowButtonBackground(e);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripSeparatorRenderEventArgs_TestData))]
        public void ToolStripRenderer_OnRenderSeparator_Invoke_Nop(ToolStripSeparatorRenderEventArgs e)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            renderer.RenderSeparator += (sender, e) => callCount++;
            renderer.OnRenderSeparator(e);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemRenderEventArgs_TestData))]
        public void ToolStripRenderer_OnRenderSplitButtonBackground_Invoke_Nop(ToolStripItemRenderEventArgs e)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            renderer.RenderSplitButtonBackground += (sender, e) => callCount++;
            renderer.OnRenderSplitButtonBackground(e);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(StatusStripSizingGrip_TestData))]
        public void ToolStripRenderer_OnRenderStatusStripSizingGrip_Invoke_Nop(ToolStrip toolStrip)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var e = new ToolStripRenderEventArgs(graphics, toolStrip);

            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            renderer.RenderStatusStripSizingGrip += (sender, e) => callCount++;
            renderer.OnRenderStatusStripSizingGrip(e);
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public void ToolStripRenderer_OnRenderStatusStripSizingGrip_NullE_ThrowsArgumentNullException()
        {
            var renderer = new SubToolStripRenderer();
            Assert.Throws<ArgumentNullException>("e", () => renderer.OnRenderStatusStripSizingGrip(null));
        }

        [WinFormsFact]
        public void ToolStripRenderer_OnRenderStatusStripSizingGrip_NullEGraphics_ThrowsNullReferenceException()
        {
            var renderer = new SubToolStripRenderer();
            var e = new ToolStripRenderEventArgs(null, new StatusStrip());
            Assert.Throws<NullReferenceException>(() => renderer.OnRenderStatusStripSizingGrip(e));
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripRenderEventArgs_TestData))]
        public void ToolStripRenderer_OnRenderToolStripBackground_Invoke_Nop(ToolStripRenderEventArgs e)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            renderer.RenderToolStripBackground += (sender, e) => callCount++;
            renderer.OnRenderToolStripBackground(e);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripRenderEventArgs_TestData))]
        public void ToolStripRenderer_OnRenderToolStripBorder_Invoke_Nop(ToolStripRenderEventArgs e)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            renderer.RenderToolStripBorder += (sender, e) => callCount++;
            renderer.OnRenderToolStripBorder(e);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripContentPanelRenderEventArgs_TestData))]
        public void ToolStripRenderer_OnRenderToolStripContentPanelBackground_Invoke_Nop(ToolStripContentPanelRenderEventArgs e)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            renderer.RenderToolStripContentPanelBackground += (sender, e) => callCount++;
            renderer.OnRenderToolStripContentPanelBackground(e);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripPanelRenderEventArgs_TestData))]
        public void ToolStripRenderer_OnRenderToolStripPanelBackground_Invoke_Nop(ToolStripPanelRenderEventArgs e)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            renderer.RenderToolStripPanelBackground += (sender, e) => callCount++;
            renderer.OnRenderToolStripPanelBackground(e);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemRenderEventArgs_TestData))]
        public void ToolStripRenderer_OnRenderToolStripStatusLabelBackground_Invoke_Nop(ToolStripItemRenderEventArgs e)
        {
            var renderer = new SubToolStripRenderer();
            int callCount = 0;
            renderer.RenderToolStripStatusLabelBackground += (sender, e) => callCount++;
            renderer.OnRenderToolStripStatusLabelBackground(e);
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public void ToolStripRenderer_ScaleArrowOffsetsIfNeeded_Invoke_Success()
        {
            SubToolStripRenderer.ScaleArrowOffsetsIfNeeded();

            // Call again.
            SubToolStripRenderer.ScaleArrowOffsetsIfNeeded();
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(96)]
        public void ToolStripRenderer_ScaleArrowOffsetsIfNeeded_InvokeInt_Success(int dpi)
        {
            SubToolStripRenderer.ScaleArrowOffsetsIfNeeded(dpi);

            // Call again.
            SubToolStripRenderer.ScaleArrowOffsetsIfNeeded(dpi);
        }

        private class SubToolStripItem : ToolStripItem
        {
        }

        private class SubToolStripRenderer : ToolStripRenderer
        {
            public new void Initialize(ToolStrip toolStrip) => base.Initialize(toolStrip);

            public new void InitializeContentPanel(ToolStripContentPanel contentPanel) => base.InitializeContentPanel(contentPanel);

            public new void InitializeItem(ToolStripItem item) => base.InitializeItem(item);

            public new void InitializePanel(ToolStripPanel toolStripPanel) => base.InitializePanel(toolStripPanel);

            public new void OnRenderArrow(ToolStripArrowRenderEventArgs e) => base.OnRenderArrow(e);

            public new void OnRenderButtonBackground(ToolStripItemRenderEventArgs e) => base.OnRenderButtonBackground(e);

            public new void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e) => base.OnRenderDropDownButtonBackground(e);

            public new void OnRenderGrip(ToolStripGripRenderEventArgs e) => base.OnRenderGrip(e);

            public new void OnRenderImageMargin(ToolStripRenderEventArgs e) => base.OnRenderImageMargin(e);

            public new void OnRenderItemBackground(ToolStripItemRenderEventArgs e) => base.OnRenderItemBackground(e);

            public new void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e) => base.OnRenderItemCheck(e);

            public new void OnRenderItemImage(ToolStripItemImageRenderEventArgs e) => base.OnRenderItemImage(e);

            public new void OnRenderItemText(ToolStripItemTextRenderEventArgs e) => base.OnRenderItemText(e);

            public new void OnRenderLabelBackground(ToolStripItemRenderEventArgs e) => base.OnRenderLabelBackground(e);

            public new void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) => base.OnRenderMenuItemBackground(e);

            public new void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e) => base.OnRenderOverflowButtonBackground(e);

            public new void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e) => base.OnRenderSeparator(e);

            public new void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e) => base.OnRenderSplitButtonBackground(e);

            public new void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e) => base.OnRenderStatusStripSizingGrip(e);

            public new void OnRenderToolStripBackground(ToolStripRenderEventArgs e) => base.OnRenderToolStripBackground(e);

            public new void OnRenderToolStripBorder(ToolStripRenderEventArgs e) => base.OnRenderToolStripBorder(e);

            public new void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e) => base.OnRenderToolStripContentPanelBackground(e);

            public new void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e) => base.OnRenderToolStripPanelBackground(e);

            public new void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e) => base.OnRenderToolStripStatusLabelBackground(e);

            public static new void ScaleArrowOffsetsIfNeeded() => ToolStripRenderer.ScaleArrowOffsetsIfNeeded();

            public static new void ScaleArrowOffsetsIfNeeded(int dpi) => ToolStripRenderer.ScaleArrowOffsetsIfNeeded(dpi);
        }
    }
}
