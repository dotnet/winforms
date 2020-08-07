// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms.PropertyGridInternal;
using Moq;
using Moq.Protected;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class PropertyGridTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void PropertyGrid_Ctor_Default()
        {
            using var control = new SubPropertyGrid();
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.NotNull(control.ActiveControl);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoScroll);
            Assert.Equal(SizeF.Empty, control.AutoScaleDimensions);
            Assert.Equal(new SizeF(1, 1), control.AutoScaleFactor);
            Assert.Equal(Size.Empty, control.AutoScrollMargin);
            Assert.Equal(AutoScaleMode.None, control.AutoScaleMode);
            Assert.Equal(Size.Empty, control.AutoScrollMinSize);
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
            Assert.False(control.AutoSize);
            Assert.Equal(AutoValidate.EnablePreventFocusChange, control.AutoValidate);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.NotNull(control.BindingContext);
            Assert.Same(control.BindingContext, control.BindingContext);
            Assert.Equal(130, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 130, 130), control.Bounds);
            Assert.True(((BrowsableAttribute)Assert.Single(control.BrowsableAttributes)).Browsable);
            Assert.Same(control.BrowsableAttributes, control.BrowsableAttributes);
            Assert.False(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CanSelect);
            Assert.False(control.CanShowCommands);
            Assert.True(control.CanShowVisualStyleGlyphs);
            Assert.False(control.Capture);
            Assert.Equal(SystemColors.ControlText, control.CategoryForeColor);
            Assert.Equal(SystemColors.Control, control.CategorySplitterColor);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Rectangle(0, 0, 130, 130), control.ClientRectangle);
            Assert.Equal(new Size(130, 130), control.ClientSize);
            Assert.Equal(Color.Red, control.CommandsActiveLinkColor);
            Assert.Equal(SystemColors.Control, control.CommandsBackColor);
            Assert.Equal(SystemColors.ControlDark, control.CommandsBorderColor);
            Assert.Equal(Color.FromArgb(255, 133, 133, 133), control.CommandsDisabledLinkColor);
            Assert.Equal(SystemColors.ControlText, control.CommandsForeColor);
            Assert.Equal(Color.FromArgb(255, 0, 0, 255), control.CommandsLinkColor);
            Assert.False(control.CommandsVisible);
            Assert.True(control.CommandsVisibleIfAvailable);
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.NotEqual(Point.Empty, control.ContextMenuDefaultLocation);
            Assert.Null(control.ContextMenuStrip);
            Assert.Equal(4, control.Controls.Count);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Equal(SizeF.Empty, control.CurrentAutoScaleDimensions);
            Assert.Equal(Cursors.Default, control.Cursor);
            Assert.Equal(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(new Size(130, 130), control.DefaultSize);
            Assert.Equal(typeof(PropertiesTab), control.DefaultTabType);
            Assert.False(control.DesignMode);
            Assert.Equal(SystemColors.GrayText, control.DisabledItemForeColor);
            Assert.Equal(new Rectangle(0, 0, 130, 130), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.NotNull(control.DockPadding);
            Assert.Same(control.DockPadding, control.DockPadding);
            Assert.Equal(0, control.DockPadding.Top);
            Assert.Equal(0, control.DockPadding.Bottom);
            Assert.Equal(0, control.DockPadding.Left);
            Assert.Equal(0, control.DockPadding.Right);
            Assert.False(control.DoubleBuffered);
            Assert.False(control.DrawFlatToolbar);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.True(control.HasChildren);
            Assert.Equal(130, control.Height);
            Assert.Equal(SystemColors.Control, control.HelpBackColor);
            Assert.Equal(SystemColors.ControlText, control.HelpForeColor);
            Assert.Equal(SystemColors.ControlDark, control.HelpBorderColor);
            Assert.True(control.HelpVisible);
            Assert.NotNull(control.HorizontalScroll);
            Assert.Same(control.HorizontalScroll, control.HorizontalScroll);
            Assert.False(control.HScroll);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsMirrored);
            Assert.False(control.LargeButtons);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(0, control.Left);
            Assert.Equal(SystemColors.InactiveBorder, control.LineColor);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.NotEqual(Size.Empty, control.PreferredSize);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.NotEmpty(control.PropertyTabs);
            Assert.NotSame(control.PropertyTabs, control.PropertyTabs);
            Assert.Equal(PropertySort.Categorized | PropertySort.Alphabetical, control.PropertySort);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.False(control.ResizeRedraw);
            Assert.Equal(130, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Null(control.SelectedGridItem);
            Assert.Equal(SystemColors.Highlight, control.SelectedItemWithFocusBackColor);
            Assert.Equal(SystemColors.HighlightText, control.SelectedItemWithFocusForeColor);
            Assert.Null(control.SelectedObject);
            Assert.Empty(control.SelectedObjects);
            Assert.Same(control.SelectedObjects, control.SelectedObjects);
            Assert.IsType<PropertiesTab>(control.SelectedTab);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(new Size(130, 130), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.True(control.ToolbarVisible);
            Assert.NotNull(control.ToolStripRenderer);
            Assert.Same(control.ToolStripRenderer, control.ToolStripRenderer);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.NotNull(control.VerticalScroll);
            Assert.Same(control.VerticalScroll, control.VerticalScroll);
            Assert.Equal(SystemColors.Window, control.ViewBackColor);
            Assert.Equal(SystemColors.ControlDark, control.ViewBorderColor);
            Assert.Equal(SystemColors.WindowText, control.ViewForeColor);
            Assert.False(control.VScroll);
            Assert.Equal(130, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void PropertyGrid_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubPropertyGrid();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x10000, createParams.ExStyle);
            Assert.Equal(130, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56010000, createParams.Style);
            Assert.Equal(130, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 2, 5)]
        [InlineData(false, 1, 4)]
        public void PropertyGrid_AutoScroll_Set_GetReturnsExpected(bool value, int expectedLayoutCallCount1, int expectedLayoutCallCount2)
        {
            using var control = new SubPropertyGrid();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("AutoScroll", e.AffectedProperty);
                layoutCallCount++;
            };

            control.AutoScroll = value;
            Assert.Equal(value, control.AutoScroll);
            Assert.Equal(value, control.GetScrollState(SubPropertyGrid.ScrollStateAutoScrolling));
            Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AutoScroll = value;
            Assert.Equal(value, control.AutoScroll);
            Assert.Equal(value, control.GetScrollState(SubPropertyGrid.ScrollStateAutoScrolling));
            Assert.Equal(expectedLayoutCallCount1 * 2, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.AutoScroll = !value;
            Assert.Equal(!value, control.AutoScroll);
            Assert.Equal(!value, control.GetScrollState(SubPropertyGrid.ScrollStateAutoScrolling));
            Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 2, 5)]
        [InlineData(false, 1, 4)]
        public void PropertyGrid_AutoScroll_SetWithHandle_GetReturnsExpected(bool value, int expectedLayoutCallCount1, int expectedLayoutCallCount2)
        {
            using var control = new SubPropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("AutoScroll", e.AffectedProperty);
                layoutCallCount++;
            };

            control.AutoScroll = value;
            Assert.Equal(value, control.AutoScroll);
            Assert.Equal(value, control.GetScrollState(SubPropertyGrid.ScrollStateAutoScrolling));
            Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.AutoScroll = value;
            Assert.Equal(value, control.AutoScroll);
            Assert.Equal(value, control.GetScrollState(SubPropertyGrid.ScrollStateAutoScrolling));
            Assert.Equal(expectedLayoutCallCount1 * 2, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.AutoScroll = !value;
            Assert.Equal(!value, control.AutoScroll);
            Assert.Equal(!value, control.GetScrollState(SubPropertyGrid.ScrollStateAutoScrolling));
            Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBackColorTheoryData))]
        public void PropertyGrid_BackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new PropertyGrid
            {
                BackColor = value
            };
            Assert.Equal(expected, control.BackColor);
            Assert.Equal(expected, control.Controls.OfType<ToolStrip>().Single().BackColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.Equal(expected, control.Controls.OfType<ToolStrip>().Single().BackColor);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> BackColor_SetWithHandle_TestData()
        {
            yield return new object[] { Color.Red, Color.Red, 1 };
            yield return new object[] { Color.Empty, Control.DefaultBackColor, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(BackColor_SetWithHandle_TestData))]
        public void PropertyGrid_BackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.Equal(expected, control.Controls.OfType<ToolStrip>().Single().BackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.Equal(expected, control.Controls.OfType<ToolStrip>().Single().BackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_BackColor_SetWithHandler_CallsBackColorChanged()
        {
            using var control = new PropertyGrid();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BackColorChanged += handler;

            // Set different.
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(1, callCount);

            // Set same.
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(1, callCount);

            // Set different.
            control.BackColor = Color.Empty;
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.BackColorChanged -= handler;
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void PropertyGrid_BackColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.BackColor)];
            using var control = new PropertyGrid();
            Assert.False(property.CanResetValue(control));

            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_BackColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.BackColor)];
            using var control = new PropertyGrid();
            Assert.False(property.ShouldSerializeValue(control));

            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void PropertyGrid_BackgroundImage_Set_GetReturnsExpected(Image value)
        {
            using var control = new PropertyGrid
            {
                BackgroundImage = value
            };
            Assert.Same(value, control.BackgroundImage);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackgroundImage = value;
            Assert.Same(value, control.BackgroundImage);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void PropertyGrid_BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
        {
            using var control = new PropertyGrid();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BackgroundImageChanged += handler;

            // Set different.
            using var image1 = new Bitmap(10, 10);
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Equal(1, callCount);

            // Set same.
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Equal(1, callCount);

            // Set different.
            using var image2 = new Bitmap(10, 10);
            control.BackgroundImage = image2;
            Assert.Same(image2, control.BackgroundImage);
            Assert.Equal(2, callCount);

            // Set null.
            control.BackgroundImage = null;
            Assert.Null(control.BackgroundImage);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.BackgroundImageChanged -= handler;
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Equal(3, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ImageLayout))]
        public void PropertyGrid_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
        {
            using var control = new PropertyGrid
            {
                BackgroundImageLayout = value
            };
            Assert.Equal(value, control.BackgroundImageLayout);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackgroundImageLayout = value;
            Assert.Equal(value, control.BackgroundImageLayout);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void PropertyGrid_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
        {
            using var control = new PropertyGrid();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BackgroundImageLayoutChanged += handler;

            // Set different.
            control.BackgroundImageLayout = ImageLayout.Center;
            Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
            Assert.Equal(1, callCount);

            // Set same.
            control.BackgroundImageLayout = ImageLayout.Center;
            Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
            Assert.Equal(1, callCount);

            // Set different.
            control.BackgroundImageLayout = ImageLayout.Stretch;
            Assert.Equal(ImageLayout.Stretch, control.BackgroundImageLayout);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.BackgroundImageLayoutChanged -= handler;
            control.BackgroundImageLayout = ImageLayout.Center;
            Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ImageLayout))]
        public void PropertyGrid_BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
        {
            using var control = new PropertyGrid();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.BackgroundImageLayout = value);
        }

        public static IEnumerable<object[]> BrowsableAttributes_Set_TestData()
        {
            yield return new object[] { null, new AttributeCollection(new Attribute[] { BrowsableAttribute.Yes }) };
            yield return new object[] { AttributeCollection.Empty, new AttributeCollection(new Attribute[] { BrowsableAttribute.Yes }) };
            yield return new object[] { new AttributeCollection(), new AttributeCollection() };
            yield return new object[] { new AttributeCollection(new Attribute[] { BrowsableAttribute.Yes }), new AttributeCollection(new Attribute[] { BrowsableAttribute.Yes }) };
            yield return new object[] { new AttributeCollection(new Attribute[] { BrowsableAttribute.Yes, ReadOnlyAttribute.Yes }), new AttributeCollection(new Attribute[] { BrowsableAttribute.Yes, ReadOnlyAttribute.Yes }) };
        }

        [WinFormsTheory]
        [MemberData(nameof(BrowsableAttributes_Set_TestData))]
        public void PropertyGrid_BrowsableAttributes_Set_GetReturnsExpected(AttributeCollection value, AttributeCollection expected)
        {
            using var control = new PropertyGrid
            {
                BrowsableAttributes = value
            };
            Assert.Equal(expected, control.BrowsableAttributes);
            Assert.NotSame(value, control.BrowsableAttributes);
            Assert.Same(control.BrowsableAttributes, control.BrowsableAttributes);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BrowsableAttributes = value;
            Assert.Equal(expected, control.BrowsableAttributes);
            Assert.NotSame(value, control.BrowsableAttributes);
            Assert.Same(control.BrowsableAttributes, control.BrowsableAttributes);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(BrowsableAttributes_Set_TestData))]
        public void PropertyGrid_BrowsableAttributes_SetEmptySelectedObjects_GetReturnsExpected(AttributeCollection value, AttributeCollection expected)
        {
            using var control = new PropertyGrid
            {
                SelectedObjects = Array.Empty<object>(),
                BrowsableAttributes = value
            };
            Assert.Equal(expected, control.BrowsableAttributes);
            Assert.NotSame(value, control.BrowsableAttributes);
            Assert.Same(control.BrowsableAttributes, control.BrowsableAttributes);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BrowsableAttributes = value;
            Assert.Equal(expected, control.BrowsableAttributes);
            Assert.NotSame(value, control.BrowsableAttributes);
            Assert.Same(control.BrowsableAttributes, control.BrowsableAttributes);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(BrowsableAttributes_Set_TestData))]
        public void PropertyGrid_BrowsableAttributes_SetCustomSelectedObjects_GetReturnsExpected(AttributeCollection value, AttributeCollection expected)
        {
            using var control = new PropertyGrid
            {
                SelectedObjects = new object[] { 1 },
                BrowsableAttributes = value
            };
            Assert.Equal(expected, control.BrowsableAttributes);
            Assert.NotSame(value, control.BrowsableAttributes);
            Assert.Same(control.BrowsableAttributes, control.BrowsableAttributes);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BrowsableAttributes = value;
            Assert.Equal(expected, control.BrowsableAttributes);
            Assert.NotSame(value, control.BrowsableAttributes);
            Assert.Same(control.BrowsableAttributes, control.BrowsableAttributes);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(BrowsableAttributes_Set_TestData))]
        public void PropertyGrid_BrowsableAttributes_SetWithHandle_GetReturnsExpected(AttributeCollection value, AttributeCollection expected)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.BrowsableAttributes = value;
            Assert.Equal(expected, control.BrowsableAttributes);
            Assert.NotSame(value, control.BrowsableAttributes);
            Assert.Same(control.BrowsableAttributes, control.BrowsableAttributes);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.BrowsableAttributes = value;
            Assert.Equal(expected, control.BrowsableAttributes);
            Assert.NotSame(value, control.BrowsableAttributes);
            Assert.Same(control.BrowsableAttributes, control.BrowsableAttributes);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void PropertyGrid_CanShowVisualStyleGlyphs_Set_GetReturnsExpected(bool value)
        {
            using var control = new SubPropertyGrid
            {
                CanShowVisualStyleGlyphs = value
            };
            Assert.Equal(value, control.CanShowVisualStyleGlyphs);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.CanShowVisualStyleGlyphs = value;
            Assert.Equal(value, control.CanShowVisualStyleGlyphs);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.CanShowVisualStyleGlyphs = !value;
            Assert.Equal(!value, control.CanShowVisualStyleGlyphs);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void PropertyGrid_CanShowVisualStyleGlyphs_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.CanShowVisualStyleGlyphs = value;
            Assert.Equal(value, control.CanShowVisualStyleGlyphs);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.CanShowVisualStyleGlyphs = value;
            Assert.Equal(value, control.CanShowVisualStyleGlyphs);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.CanShowVisualStyleGlyphs = !value;
            Assert.Equal(!value, control.CanShowVisualStyleGlyphs);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> CategoryForeColor_Set_TestData()
        {
            yield return new object[] { Color.Empty };
            yield return new object[] { Color.Red };
            yield return new object[] { Color.FromArgb(254, 1, 2, 3) };
        }

        [WinFormsTheory]
        [MemberData(nameof(CategoryForeColor_Set_TestData))]
        public void PropertyGrid_CategoryForeColor_Set_GetReturnsExpected(Color value)
        {
            using var control = new PropertyGrid
            {
                CategoryForeColor = value
            };
            Assert.Equal(value, control.CategoryForeColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.CategoryForeColor = value;
            Assert.Equal(value, control.CategoryForeColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(CategoryForeColor_Set_TestData))]
        public void PropertyGrid_CategoryForeColor_SetWithHandle_GetReturnsExpected(Color value)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.CategoryForeColor = value;
            Assert.Equal(value, control.CategoryForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.CategoryForeColor = value;
            Assert.Equal(value, control.CategoryForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_CategoryForeColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.CategoryForeColor)];
            using var control = new PropertyGrid();
            Assert.False(property.CanResetValue(control));

            control.CategoryForeColor = Color.Red;
            Assert.Equal(Color.Red, control.CategoryForeColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.ControlText, control.CategoryForeColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_CategoryForeColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.CategoryForeColor)];
            using var control = new PropertyGrid();
            Assert.False(property.ShouldSerializeValue(control));

            control.CategoryForeColor = Color.Red;
            Assert.Equal(Color.Red, control.CategoryForeColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.ControlText, control.CategoryForeColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsTheory]
        [MemberData(nameof(CategoryForeColor_Set_TestData))]
        public void PropertyGrid_CategorySplitterColor_Set_GetReturnsExpected(Color value)
        {
            using var control = new PropertyGrid
            {
                CategorySplitterColor = value
            };
            Assert.Equal(value, control.CategorySplitterColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.CategorySplitterColor = value;
            Assert.Equal(value, control.CategorySplitterColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(CategoryForeColor_Set_TestData))]
        public void PropertyGrid_CategorySplitterColor_SetWithHandle_GetReturnsExpected(Color value)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.CategorySplitterColor = value;
            Assert.Equal(value, control.CategorySplitterColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.CategorySplitterColor = value;
            Assert.Equal(value, control.CategorySplitterColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_CategorySplitterColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.CategorySplitterColor)];
            using var control = new PropertyGrid();
            Assert.False(property.CanResetValue(control));

            control.CategorySplitterColor = Color.Red;
            Assert.Equal(Color.Red, control.CategorySplitterColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.Control, control.CategorySplitterColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_CategorySplitterColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.CategorySplitterColor)];
            using var control = new PropertyGrid();
            Assert.False(property.ShouldSerializeValue(control));

            control.CategorySplitterColor = Color.Red;
            Assert.Equal(Color.Red, control.CategorySplitterColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.Control, control.CategorySplitterColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        public static IEnumerable<object[]> CommandsActiveLinkColor_Set_TestData()
        {
            yield return new object[] { Color.Empty, Color.Red };
            yield return new object[] { Color.Red, Color.Red };
            yield return new object[] { Color.Blue, Color.Blue };
        }

        [WinFormsTheory]
        [MemberData(nameof(CommandsActiveLinkColor_Set_TestData))]
        public void PropertyGrid_CommandsActiveLinkColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new PropertyGrid
            {
                CommandsActiveLinkColor = value
            };
            Assert.Equal(expected, control.CommandsActiveLinkColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.CommandsActiveLinkColor = value;
            Assert.Equal(expected, control.CommandsActiveLinkColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(CommandsActiveLinkColor_Set_TestData))]
        public void PropertyGrid_CommandsActiveLinkColor_SetWithHandle_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.CommandsActiveLinkColor = value;
            Assert.Equal(expected, control.CommandsActiveLinkColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.CommandsActiveLinkColor = value;
            Assert.Equal(expected, control.CommandsActiveLinkColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_CommandsActiveLinkColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.CommandsActiveLinkColor)];
            using var control = new PropertyGrid();
            Assert.False(property.CanResetValue(control));

            control.CommandsActiveLinkColor = Color.Black;
            Assert.Equal(Color.Black, control.CommandsActiveLinkColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(Color.Red, control.CommandsActiveLinkColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_CommandsActiveLinkColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.CommandsActiveLinkColor)];
            using var control = new PropertyGrid();
            Assert.False(property.ShouldSerializeValue(control));

            control.CommandsActiveLinkColor = Color.Black;
            Assert.Equal(Color.Black, control.CommandsActiveLinkColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(Color.Red, control.CommandsActiveLinkColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBackColorTheoryData))]
        public void PropertyGrid_CommandsBackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new PropertyGrid
            {
                CommandsBackColor = value
            };
            Assert.Equal(expected, control.CommandsBackColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.CommandsBackColor = value;
            Assert.Equal(expected, control.CommandsBackColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBackColorTheoryData))]
        public void PropertyGrid_CommandsBackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.CommandsBackColor = value;
            Assert.Equal(expected, control.CommandsBackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.CommandsBackColor = value;
            Assert.Equal(expected, control.CommandsBackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_CommandsBackColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.CommandsBackColor)];
            using var control = new PropertyGrid();
            Assert.False(property.CanResetValue(control));

            control.CommandsBackColor = Color.Red;
            Assert.Equal(Color.Red, control.CommandsBackColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.Control, control.CommandsBackColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_CommandsBackColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.CommandsBackColor)];
            using var control = new PropertyGrid();
            Assert.False(property.ShouldSerializeValue(control));

            control.CommandsBackColor = Color.Red;
            Assert.Equal(Color.Red, control.CommandsBackColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.Control, control.CommandsBackColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_BackColor_SetTransparent_ThrowsArgmentException()
        {
            using var control = new PropertyGrid();
            Assert.Throws<ArgumentException>(null, () => control.CommandsBackColor = Color.FromArgb(254, 1, 2, 3));
        }

        [WinFormsTheory]
        [MemberData(nameof(CategoryForeColor_Set_TestData))]
        public void PropertyGrid_CommandsBorderColor_Set_GetReturnsExpected(Color value)
        {
            using var control = new PropertyGrid
            {
                CommandsBorderColor = value
            };
            Assert.Equal(value, control.CommandsBorderColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.CommandsBorderColor = value;
            Assert.Equal(value, control.CommandsBorderColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(CategoryForeColor_Set_TestData))]
        public void PropertyGrid_CommandsBorderColor_SetWithHandle_GetReturnsExpected(Color value)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.CommandsBorderColor = value;
            Assert.Equal(value, control.CommandsBorderColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.CommandsBorderColor = value;
            Assert.Equal(value, control.CommandsBorderColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_CommandsBorderColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.CommandsBorderColor)];
            using var control = new PropertyGrid();
            Assert.False(property.CanResetValue(control));

            control.CommandsBorderColor = Color.Red;
            Assert.Equal(Color.Red, control.CommandsBorderColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.ControlDark, control.CommandsBorderColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_CommandsBorderColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.CommandsBorderColor)];
            using var control = new PropertyGrid();
            Assert.False(property.ShouldSerializeValue(control));

            control.CommandsBorderColor = Color.Red;
            Assert.Equal(Color.Red, control.CommandsBorderColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.ControlDark, control.CommandsBorderColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        public static IEnumerable<object[]> CommandsDisabledLinkColor_Set_TestData()
        {
            yield return new object[] { Color.Empty, Color.FromArgb(255, 133, 133, 133) };
            yield return new object[] { Color.Red, Color.Red };
            yield return new object[] { Color.Blue, Color.Blue };
        }

        [WinFormsTheory]
        [MemberData(nameof(CommandsDisabledLinkColor_Set_TestData))]
        public void PropertyGrid_CommandsDisabledLinkColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new PropertyGrid
            {
                CommandsDisabledLinkColor = value
            };
            Assert.Equal(expected, control.CommandsDisabledLinkColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.CommandsDisabledLinkColor = value;
            Assert.Equal(expected, control.CommandsDisabledLinkColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(CommandsDisabledLinkColor_Set_TestData))]
        public void PropertyGrid_CommandsDisabledLinkColor_SetWithHandle_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.CommandsDisabledLinkColor = value;
            Assert.Equal(expected, control.CommandsDisabledLinkColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.CommandsDisabledLinkColor = value;
            Assert.Equal(expected, control.CommandsDisabledLinkColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_CommandsDisabledLinkColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.CommandsDisabledLinkColor)];
            using var control = new PropertyGrid();
            Assert.False(property.CanResetValue(control));

            control.CommandsDisabledLinkColor = Color.Red;
            Assert.Equal(Color.Red, control.CommandsDisabledLinkColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(Color.FromArgb(255, 133, 133, 133), control.CommandsDisabledLinkColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_CommandsDisabledLinkColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.CommandsDisabledLinkColor)];
            using var control = new PropertyGrid();
            Assert.False(property.ShouldSerializeValue(control));

            control.CommandsDisabledLinkColor = Color.Red;
            Assert.Equal(Color.Red, control.CommandsDisabledLinkColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(Color.FromArgb(255, 133, 133, 133), control.CommandsDisabledLinkColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetForeColorTheoryData))]
        public void PropertyGrid_CommandsForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new PropertyGrid
            {
                CommandsForeColor = value
            };
            Assert.Equal(expected, control.CommandsForeColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.CommandsForeColor = value;
            Assert.Equal(expected, control.CommandsForeColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetForeColorTheoryData))]
        public void PropertyGrid_CommandsForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.CommandsForeColor = value;
            Assert.Equal(expected, control.CommandsForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.CommandsForeColor = value;
            Assert.Equal(expected, control.CommandsForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_CommandsForeColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.CommandsForeColor)];
            using var control = new PropertyGrid();
            Assert.False(property.CanResetValue(control));

            control.CommandsForeColor = Color.Red;
            Assert.Equal(Color.Red, control.CommandsForeColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.ControlText, control.CommandsForeColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_CommandsForeColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.CommandsForeColor)];
            using var control = new PropertyGrid();
            Assert.False(property.ShouldSerializeValue(control));

            control.CommandsForeColor = Color.Red;
            Assert.Equal(Color.Red, control.CommandsForeColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.ControlText, control.CommandsForeColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        public static IEnumerable<object[]> CommandsLinkColor_Set_TestData()
        {
            yield return new object[] { Color.Empty, Color.FromArgb(255, 0, 0, 255) };
            yield return new object[] { Color.Red, Color.Red };
            yield return new object[] { Color.Blue, Color.Blue };
        }

        [WinFormsTheory]
        [MemberData(nameof(CommandsLinkColor_Set_TestData))]
        public void PropertyGrid_CommandsLinkColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new PropertyGrid
            {
                CommandsLinkColor = value
            };
            Assert.Equal(expected, control.CommandsLinkColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.CommandsLinkColor = value;
            Assert.Equal(expected, control.CommandsLinkColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(CommandsLinkColor_Set_TestData))]
        public void PropertyGrid_CommandsLinkColor_SetWithHandle_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.CommandsLinkColor = value;
            Assert.Equal(expected, control.CommandsLinkColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.CommandsLinkColor = value;
            Assert.Equal(expected, control.CommandsLinkColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_CommandsLinkColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.CommandsLinkColor)];
            using var control = new PropertyGrid();
            Assert.False(property.CanResetValue(control));

            control.CommandsLinkColor = Color.Red;
            Assert.Equal(Color.Red, control.CommandsLinkColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(Color.FromArgb(255, 0, 0, 255), control.CommandsLinkColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_CommandsLinkColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.CommandsLinkColor)];
            using var control = new PropertyGrid();
            Assert.False(property.ShouldSerializeValue(control));

            control.CommandsLinkColor = Color.Red;
            Assert.Equal(Color.Red, control.CommandsLinkColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(Color.FromArgb(255, 0, 0, 255), control.CommandsLinkColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void PropertyGrid_CommandsVisibleIfAvailable_Set_GetReturnsExpected(bool visible, bool value)
        {
            using var control = new PropertyGrid
            {
                Visible = visible
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.CommandsVisibleIfAvailable = value;
            Assert.Equal(value, control.CommandsVisibleIfAvailable);
            Assert.False(control.CommandsVisible);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.CommandsVisibleIfAvailable = value;
            Assert.Equal(value, control.CommandsVisibleIfAvailable);
            Assert.False(control.CommandsVisible);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.CommandsVisibleIfAvailable = !value;
            Assert.Equal(!value, control.CommandsVisibleIfAvailable);
            Assert.False(control.CommandsVisible);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true, 7)]
        [InlineData(true, false, 7)]
        [InlineData(false, true, 0)]
        [InlineData(false, false, 0)]
        public void PropertyGrid_CommandsVisibleIfAvailable_SetWithHandle_GetReturnsExpected(bool visible, bool value, int expectedLayoutCallCount)
        {
            using var control = new PropertyGrid
            {
                Visible = visible
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.CommandsVisibleIfAvailable = value;
            Assert.Equal(value, control.CommandsVisibleIfAvailable);
            Assert.False(control.CommandsVisible);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.CommandsVisibleIfAvailable = value;
            Assert.Equal(value, control.CommandsVisibleIfAvailable);
            Assert.False(control.CommandsVisible);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.CommandsVisibleIfAvailable = !value;
            Assert.Equal(!value, control.CommandsVisibleIfAvailable);
            Assert.False(control.CommandsVisible);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(CategoryForeColor_Set_TestData))]
        public void PropertyGrid_DisabledItemForeColor_Set_GetReturnsExpected(Color value)
        {
            using var control = new PropertyGrid
            {
                DisabledItemForeColor = value
            };
            Assert.Equal(value, control.DisabledItemForeColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.DisabledItemForeColor = value;
            Assert.Equal(value, control.DisabledItemForeColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(CategoryForeColor_Set_TestData))]
        public void PropertyGrid_DisabledItemForeColor_SetWithHandle_GetReturnsExpected(Color value)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.DisabledItemForeColor = value;
            Assert.Equal(value, control.DisabledItemForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.DisabledItemForeColor = value;
            Assert.Equal(value, control.DisabledItemForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_DisabledItemForeColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.DisabledItemForeColor)];
            using var control = new PropertyGrid();
            Assert.False(property.CanResetValue(control));

            control.DisabledItemForeColor = Color.Red;
            Assert.Equal(Color.Red, control.DisabledItemForeColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.GrayText, control.DisabledItemForeColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_DisabledItemForeColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.DisabledItemForeColor)];
            using var control = new PropertyGrid();
            Assert.False(property.ShouldSerializeValue(control));

            control.DisabledItemForeColor = Color.Red;
            Assert.Equal(Color.Red, control.DisabledItemForeColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.GrayText, control.DisabledItemForeColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void PropertyGrid_DrawFlatToolbar_Set_GetReturnsExpected(bool value)
        {
            using var control = new SubPropertyGrid
            {
                DrawFlatToolbar = value
            };
            Assert.Equal(value, control.DrawFlatToolbar);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.DrawFlatToolbar = value;
            Assert.Equal(value, control.DrawFlatToolbar);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.DrawFlatToolbar = !value;
            Assert.Equal(!value, control.DrawFlatToolbar);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void PropertyGrid_DrawFlatToolbar_SetWithCommandColors_GetReturnsExpected()
        {
            using var control = new SubPropertyGrid
            {
                CommandsBackColor = Color.FromArgb(255, 0, 0, 1),
                CommandsForeColor = Color.FromArgb(255, 0, 0, 2),
                CommandsLinkColor = Color.FromArgb(255, 0, 0, 3),
                CommandsActiveLinkColor = Color.FromArgb(255, 0, 0, 4),
                CommandsDisabledLinkColor = Color.FromArgb(255, 0, 0, 5)
            };

            // Set true.
            control.DrawFlatToolbar = true;
            Assert.True(control.DrawFlatToolbar);
            Assert.Equal(Color.FromArgb(255, 0, 0, 1), control.CommandsBackColor);
            Assert.Equal(Color.FromArgb(255, 0, 0, 2), control.CommandsForeColor);
            Assert.Equal(Color.FromArgb(255, 0, 0, 255), control.CommandsLinkColor);
            Assert.Equal(Color.Red, control.CommandsActiveLinkColor);
            Assert.Equal(Color.FromArgb(255, 133, 133, 133), control.CommandsDisabledLinkColor);
            Assert.False(control.IsHandleCreated);

            // Set false.
            control.DrawFlatToolbar = false;
            Assert.False(control.DrawFlatToolbar);
            Assert.Equal(Color.FromArgb(255, 0, 0, 1), control.CommandsBackColor);
            Assert.Equal(Color.FromArgb(255, 0, 0, 2), control.CommandsForeColor);
            Assert.Equal(Color.FromArgb(255, 0, 0, 255), control.CommandsLinkColor);
            Assert.Equal(Color.Red, control.CommandsActiveLinkColor);
            Assert.Equal(Color.FromArgb(255, 133, 133, 133), control.CommandsDisabledLinkColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void PropertyGrid_DrawFlatToolbar_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new SubPropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.DrawFlatToolbar = value;
            Assert.Equal(value, control.DrawFlatToolbar);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.DrawFlatToolbar = value;
            Assert.Equal(value, control.DrawFlatToolbar);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.DrawFlatToolbar = !value;
            Assert.Equal(!value, control.DrawFlatToolbar);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetForeColorTheoryData))]
        public void PropertyGrid_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new PropertyGrid
            {
                ForeColor = value
            };
            Assert.Equal(expected, control.ForeColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> ForeColor_SetWithHandle_TestData()
        {
            yield return new object[] { Color.Red, Color.Red, 1 };
            yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3), 1 };
            yield return new object[] { Color.Empty, Control.DefaultForeColor, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(ForeColor_SetWithHandle_TestData))]
        public void PropertyGrid_ForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_ForeColor_SetWithHandler_CallsForeColorChanged()
        {
            using var control = new PropertyGrid();
            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            }
            control.ForeColorChanged += handler;

            // Set different.
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(1, callCount);

            // Set same.
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(1, callCount);

            // Set different.
            control.ForeColor = Color.Empty;
            Assert.Equal(PropertyGrid.DefaultForeColor, control.ForeColor);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.ForeColorChanged -= handler;
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void PropertyGrid_ForeColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.ForeColor)];
            using var control = new PropertyGrid();
            Assert.False(property.CanResetValue(control));

            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_ForeColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.ForeColor)];
            using var control = new PropertyGrid();
            Assert.False(property.ShouldSerializeValue(control));

            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBackColorTheoryData))]
        public void PropertyGrid_HelpBackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new PropertyGrid
            {
                HelpBackColor = value
            };
            Assert.Equal(expected, control.HelpBackColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.HelpBackColor = value;
            Assert.Equal(expected, control.HelpBackColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBackColorTheoryData))]
        public void PropertyGrid_HelpBackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.HelpBackColor = value;
            Assert.Equal(expected, control.HelpBackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.HelpBackColor = value;
            Assert.Equal(expected, control.HelpBackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_HelpBackColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.HelpBackColor)];
            using var control = new PropertyGrid();
            Assert.False(property.CanResetValue(control));

            control.HelpBackColor = Color.Red;
            Assert.Equal(Color.Red, control.HelpBackColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.Control, control.HelpBackColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_HelpBackColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.HelpBackColor)];
            using var control = new PropertyGrid();
            Assert.False(property.ShouldSerializeValue(control));

            control.HelpBackColor = Color.Red;
            Assert.Equal(Color.Red, control.HelpBackColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.Control, control.HelpBackColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_HelpBackColor_SetTransparent_ThrowsArgmentException()
        {
            using var control = new PropertyGrid();
            Assert.Throws<ArgumentException>(null, () => control.HelpBackColor = Color.FromArgb(254, 1, 2, 3));
        }

        [WinFormsTheory]
        [MemberData(nameof(CategoryForeColor_Set_TestData))]
        public void PropertyGrid_HelpBorderColor_Set_GetReturnsExpected(Color value)
        {
            using var control = new PropertyGrid
            {
                HelpBorderColor = value
            };
            Assert.Equal(value, control.HelpBorderColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.HelpBorderColor = value;
            Assert.Equal(value, control.HelpBorderColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(CategoryForeColor_Set_TestData))]
        public void PropertyGrid_HelpBorderColor_SetWithHandle_GetReturnsExpected(Color value)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.HelpBorderColor = value;
            Assert.Equal(value, control.HelpBorderColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.HelpBorderColor = value;
            Assert.Equal(value, control.HelpBorderColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_HelpBorderColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.HelpBorderColor)];
            using var control = new PropertyGrid();
            Assert.False(property.CanResetValue(control));

            control.HelpBorderColor = Color.Red;
            Assert.Equal(Color.Red, control.HelpBorderColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.ControlDark, control.HelpBorderColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_HelpBorderColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.HelpBorderColor)];
            using var control = new PropertyGrid();
            Assert.False(property.ShouldSerializeValue(control));

            control.HelpBorderColor = Color.Red;
            Assert.Equal(Color.Red, control.HelpBorderColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.ControlDark, control.HelpBorderColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetForeColorTheoryData))]
        public void PropertyGrid_HelpForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new PropertyGrid
            {
                HelpForeColor = value
            };
            Assert.Equal(expected, control.HelpForeColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.HelpForeColor = value;
            Assert.Equal(expected, control.HelpForeColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetForeColorTheoryData))]
        public void PropertyGrid_HelpForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.HelpForeColor = value;
            Assert.Equal(expected, control.HelpForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.HelpForeColor = value;
            Assert.Equal(expected, control.HelpForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_HelpForeColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.HelpForeColor)];
            using var control = new PropertyGrid();
            Assert.False(property.CanResetValue(control));

            control.HelpForeColor = Color.Red;
            Assert.Equal(Color.Red, control.HelpForeColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.ControlText, control.HelpForeColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_HelpForeColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.HelpForeColor)];
            using var control = new PropertyGrid();
            Assert.False(property.ShouldSerializeValue(control));

            control.HelpForeColor = Color.Red;
            Assert.Equal(Color.Red, control.HelpForeColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.ControlText, control.HelpForeColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsTheory]
        [InlineData(true, true, 0, 0, 1)]
        [InlineData(true, false, 1, 1, 2)]
        [InlineData(false, true, 1, 2, 2)]
        [InlineData(false, false, 0, 0, 1)]
        public void PropertyGrid_HelpVisible_Set_GetReturnsExpected(bool visible, bool value, int expectedLayoutCallCount1, int expectedLayoutCallCount2, int expectedLayoutCallCount3)
        {
            using var control = new PropertyGrid
            {
                Visible = visible
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.HelpVisible = value;
            Assert.Equal(value, control.HelpVisible);
            Assert.False(control.CommandsVisible);
            Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.HelpVisible = value;
            Assert.Equal(value, control.HelpVisible);
            Assert.False(control.CommandsVisible);
            Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.HelpVisible = !value;
            Assert.Equal(!value, control.HelpVisible);
            Assert.False(control.CommandsVisible);
            Assert.Equal(expectedLayoutCallCount3, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true, 7, 2, 7, 10)]
        [InlineData(true, false, 10, 2, 10, 13)]
        [InlineData(false, true, 1, 1, 2, 2)]
        [InlineData(false, false, 0, 1, 0, 1)]
        public void PropertyGrid_HelpVisible_SetWithHandle_GetReturnsExpected(bool visible, bool value, int expectedLayoutCallCount1, int expectedInvalidatedCallCount, int expectedLayoutCallCount2, int expectedLayoutCallCount3)
        {
            using var control = new PropertyGrid
            {
                Visible = visible
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.HelpVisible = value;
            Assert.Equal(value, control.HelpVisible);
            Assert.False(control.CommandsVisible);
            Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.HelpVisible = value;
            Assert.Equal(value, control.HelpVisible);
            Assert.False(control.CommandsVisible);
            Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.HelpVisible = !value;
            Assert.Equal(!value, control.HelpVisible);
            Assert.False(control.CommandsVisible);
            Assert.Equal(expectedLayoutCallCount3, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 3, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void PropertyGrid_LargeButtons_Set_GetReturnsExpected(bool visible, bool value)
        {
            using var control = new PropertyGrid
            {
                Visible = visible
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.LargeButtons = value;
            Assert.Equal(value, control.LargeButtons);
            Assert.False(control.CommandsVisible);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.LargeButtons = value;
            Assert.Equal(value, control.LargeButtons);
            Assert.False(control.CommandsVisible);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.LargeButtons = !value;
            Assert.Equal(!value, control.LargeButtons);
            Assert.False(control.CommandsVisible);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true, 12, 2, 17, 4)]
        [InlineData(true, false, 7, 0, 12, 2)]
        [InlineData(false, true, 0, 1, 0, 2)]
        [InlineData(false, false, 0, 0, 0, 1)]
        public void PropertyGrid_LargeButtons_SetWithHandle_GetReturnsExpected(bool visible, bool value, int expectedLayoutCallCount1, int expectedInvalidatedCallCount1, int expectedLayoutCallCount2, int expectedInvalidatedCallCount2)
        {
            using var control = new PropertyGrid
            {
                Visible = visible
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.LargeButtons = value;
            Assert.Equal(value, control.LargeButtons);
            Assert.False(control.CommandsVisible);
            Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.LargeButtons = value;
            Assert.Equal(value, control.LargeButtons);
            Assert.False(control.CommandsVisible);
            Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.LargeButtons = !value;
            Assert.Equal(!value, control.LargeButtons);
            Assert.False(control.CommandsVisible);
            Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(CategoryForeColor_Set_TestData))]
        public void PropertyGrid_LineColor_Set_GetReturnsExpected(Color value)
        {
            using var control = new PropertyGrid
            {
                LineColor = value
            };
            Assert.Equal(value, control.LineColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.LineColor = value;
            Assert.Equal(value, control.LineColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(CategoryForeColor_Set_TestData))]
        public void PropertyGrid_LineColor_SetWithHandle_GetReturnsExpected(Color value)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.LineColor = value;
            Assert.Equal(value, control.LineColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.LineColor = value;
            Assert.Equal(value, control.LineColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_LineColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.LineColor)];
            using var control = new PropertyGrid();
            Assert.False(property.CanResetValue(control));

            control.LineColor = Color.Red;
            Assert.Equal(Color.Red, control.LineColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.InactiveBorder, control.LineColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_LineColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.LineColor)];
            using var control = new PropertyGrid();
            Assert.False(property.ShouldSerializeValue(control));

            control.LineColor = Color.Red;
            Assert.Equal(Color.Red, control.LineColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.InactiveBorder, control.LineColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaddingNormalizedTheoryData))]
        public void PropertyGrid_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
        {
            using var control = new PropertyGrid
            {
                Padding = value
            };
            Assert.Equal(expected, control.Padding);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaddingNormalizedTheoryData))]
        public void PropertyGrid_Padding_SetWithHandle_GetReturnsExpected(Padding value, Padding expected)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_Padding_SetWithHandler_CallsPaddingChanged()
        {
            using var control = new PropertyGrid();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                Assert.Equal(EventArgs.Empty, e);
                callCount++;
            };
            control.PaddingChanged += handler;

            // Set different.
            var padding1 = new Padding(1);
            control.Padding = padding1;
            Assert.Equal(padding1, control.Padding);
            Assert.Equal(1, callCount);

            // Set same.
            control.Padding = padding1;
            Assert.Equal(padding1, control.Padding);
            Assert.Equal(1, callCount);

            // Set different.
            var padding2 = new Padding(2);
            control.Padding = padding2;
            Assert.Equal(padding2, control.Padding);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.PaddingChanged -= handler;
            control.Padding = padding1;
            Assert.Equal(padding1, control.Padding);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(PropertySort))]
        public void PropertyGrid_PropertySort_Set_GetReturnsExpected(PropertySort value)
        {
            using var control = new PropertyGrid
            {
                PropertySort = value
            };
            Assert.Equal(value, control.PropertySort);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.PropertySort = value;
            Assert.Equal(value, control.PropertySort);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(PropertySort))]
        public void PropertyGrid_PropertySort_SetWithHandle_GetReturnsExpected(PropertySort value)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.PropertySort = value;
            Assert.Equal(value, control.PropertySort);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.PropertySort = value;
            Assert.Equal(value, control.PropertySort);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_SelectedGridItem_SetNull_ThrowsArgumentException()
        {
            using var control = new PropertyGrid();
            Assert.Throws<ArgumentException>(null, () => control.SelectedGridItem = null);
        }

        [WinFormsFact]
        public void PropertyGrid_SelectedGridItem_SetNotGridEntry_ThrowsInvalidCastException()
        {
            using var control = new PropertyGrid();
            var mockGridItem = new Mock<GridItem>(MockBehavior.Strict);
            Assert.Throws<InvalidCastException>(() => control.SelectedGridItem = mockGridItem.Object);
        }

        [WinFormsTheory]
        [MemberData(nameof(CategoryForeColor_Set_TestData))]
        public void PropertyGrid_SelectedItemWithFocusBackColor_Set_GetReturnsExpected(Color value)
        {
            using var control = new PropertyGrid
            {
                SelectedItemWithFocusBackColor = value
            };
            Assert.Equal(value, control.SelectedItemWithFocusBackColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SelectedItemWithFocusBackColor = value;
            Assert.Equal(value, control.SelectedItemWithFocusBackColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(CategoryForeColor_Set_TestData))]
        public void PropertyGrid_SelectedItemWithFocusBackColor_SetWithHandle_GetReturnsExpected(Color value)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SelectedItemWithFocusBackColor = value;
            Assert.Equal(value, control.SelectedItemWithFocusBackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.SelectedItemWithFocusBackColor = value;
            Assert.Equal(value, control.SelectedItemWithFocusBackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_SelectedItemWithFocusBackColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.SelectedItemWithFocusBackColor)];
            using var control = new PropertyGrid();
            Assert.False(property.CanResetValue(control));

            control.SelectedItemWithFocusBackColor = Color.Red;
            Assert.Equal(Color.Red, control.SelectedItemWithFocusBackColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.Highlight, control.SelectedItemWithFocusBackColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_SelectedItemWithFocusBackColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.SelectedItemWithFocusBackColor)];
            using var control = new PropertyGrid();
            Assert.False(property.ShouldSerializeValue(control));

            control.SelectedItemWithFocusBackColor = Color.Red;
            Assert.Equal(Color.Red, control.SelectedItemWithFocusBackColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.Highlight, control.SelectedItemWithFocusBackColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsTheory]
        [MemberData(nameof(CategoryForeColor_Set_TestData))]
        public void PropertyGrid_SelectedItemWithFocusForeColor_Set_GetReturnsExpected(Color value)
        {
            using var control = new PropertyGrid
            {
                SelectedItemWithFocusForeColor = value
            };
            Assert.Equal(value, control.SelectedItemWithFocusForeColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SelectedItemWithFocusForeColor = value;
            Assert.Equal(value, control.SelectedItemWithFocusForeColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(CategoryForeColor_Set_TestData))]
        public void PropertyGrid_SelectedItemWithFocusForeColor_SetWithHandle_GetReturnsExpected(Color value)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SelectedItemWithFocusForeColor = value;
            Assert.Equal(value, control.SelectedItemWithFocusForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.SelectedItemWithFocusForeColor = value;
            Assert.Equal(value, control.SelectedItemWithFocusForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_SelectedItemWithFocusForeColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.SelectedItemWithFocusForeColor)];
            using var control = new PropertyGrid();
            Assert.False(property.CanResetValue(control));

            control.SelectedItemWithFocusForeColor = Color.Red;
            Assert.Equal(Color.Red, control.SelectedItemWithFocusForeColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.HighlightText, control.SelectedItemWithFocusForeColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_SelectedItemWithFocusForeColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.SelectedItemWithFocusForeColor)];
            using var control = new PropertyGrid();
            Assert.False(property.ShouldSerializeValue(control));

            control.SelectedItemWithFocusForeColor = Color.Red;
            Assert.Equal(Color.Red, control.SelectedItemWithFocusForeColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.HighlightText, control.SelectedItemWithFocusForeColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        public static IEnumerable<object[]> SelectedObject_Set_TestData()
        {
            yield return new object[] { null, null, Array.Empty<object>() };
            yield return new object[] { 1, 1, new object[] { 1 } };
        }

        [WinFormsTheory]
        [MemberData(nameof(SelectedObject_Set_TestData))]
        public void PropertyGrid_SelectedObject_Set_GetReturnsExpected(object value, object expected, object[] expectedSelectedObjects)
        {
            using var control = new PropertyGrid
            {
                SelectedObject = value
            };
            Assert.Equal(expected, control.SelectedObject);
            Assert.Equal(expectedSelectedObjects, control.SelectedObjects);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SelectedObject = value;
            Assert.Equal(expected, control.SelectedObject);
            Assert.Equal(expectedSelectedObjects, control.SelectedObjects);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(SelectedObject_Set_TestData))]
        public void PropertyGrid_SelectedObject_SetWithHandle_GetReturnsExpected(object value, object expected, object[] expectedSelectedObjects)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SelectedObject = value;
            Assert.Equal(expected, control.SelectedObject);
            Assert.Equal(expectedSelectedObjects, control.SelectedObjects);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.SelectedObject = value;
            Assert.Equal(expected, control.SelectedObject);
            Assert.Equal(expectedSelectedObjects, control.SelectedObjects);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> SelectedObjects_Set_TestData()
        {
            yield return new object[] { null, Array.Empty<object>(), null };
            yield return new object[] { Array.Empty<object>(), Array.Empty<object>(), null };
            yield return new object[] { new object[] { 1 }, new object[] { 1 }, 1 };
            yield return new object[] { new object[] { 2 }, new object[] { 2 }, 2 };
            yield return new object[] { new object[] { "2" }, new object[] { "2" }, "2" };
            yield return new object[] { new object[] { 1, 2, 3 }, new object[] { 1, 2, 3 }, 1 };
            yield return new object[] { new object[] { 1, "2", 3 }, new object[] { 1, "2", 3 }, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(SelectedObjects_Set_TestData))]
        public void PropertyGrid_SelectedObjects_Set_GetReturnsExpected(object[] value, object[] expected, object expectedSelectedObject)
        {
            using var control = new PropertyGrid
            {
                SelectedObjects = value
            };
            Assert.Equal(expected, control.SelectedObjects);
            Assert.NotSame(value, control.SelectedObjects);
            Assert.NotSame(control.SelectedObjects, control.SelectedObjects);
            Assert.Equal(expectedSelectedObject, control.SelectedObject);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SelectedObjects = value;
            Assert.Equal(expected, control.SelectedObjects);
            Assert.NotSame(value, control.SelectedObjects);
            Assert.NotSame(control.SelectedObjects, control.SelectedObjects);
            Assert.Equal(expectedSelectedObject, control.SelectedObject);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(SelectedObjects_Set_TestData))]
        public void PropertyGrid_SelectedObjects_SetWithCustomOldValue_GetReturnsExpected(object[] value, object[] expected, object expectedSelectedObject)
        {
            using var control = new PropertyGrid
            {
                SelectedObjects = new object[] { 1 }
            };

            control.SelectedObjects = value;
            Assert.Equal(expected, control.SelectedObjects);
            Assert.NotSame(value, control.SelectedObjects);
            Assert.NotSame(control.SelectedObjects, control.SelectedObjects);
            Assert.Equal(expectedSelectedObject, control.SelectedObject);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SelectedObjects = value;
            Assert.Equal(expected, control.SelectedObjects);
            Assert.NotSame(value, control.SelectedObjects);
            Assert.NotSame(control.SelectedObjects, control.SelectedObjects);
            Assert.Equal(expectedSelectedObject, control.SelectedObject);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(SelectedObjects_Set_TestData))]
        public void PropertyGrid_SelectedObjects_SetWithHandle_GetReturnsExpected(object[] value, object[] expected, object expectedSelectedObject)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SelectedObjects = value;
            Assert.Equal(expected, control.SelectedObjects);
            Assert.NotSame(value, control.SelectedObjects);
            Assert.NotSame(control.SelectedObjects, control.SelectedObjects);
            Assert.Equal(expectedSelectedObject, control.SelectedObject);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.SelectedObjects = value;
            Assert.Equal(expected, control.SelectedObjects);
            Assert.NotSame(value, control.SelectedObjects);
            Assert.NotSame(control.SelectedObjects, control.SelectedObjects);
            Assert.Equal(expectedSelectedObject, control.SelectedObject);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_SelectedObjects_SetNullInValue_ThrowsArgumentException()
        {
            using var control = new SubPropertyGrid();
            Assert.Throws<ArgumentException>(null, () => control.SelectedObjects = new object[] { null });
        }

        public static IEnumerable<object[]> Site_Set_TestData()
        {
            yield return new object[] { null };

            var mockNullSite = new Mock<ISite>(MockBehavior.Strict);
            mockNullSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockNullSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockNullSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns((IDesignerHost)null);
            yield return new object[] { mockNullSite.Object };

#if false
            var mockInvalidSite = new Mock<ISite>(MockBehavior.Strict);
            mockInvalidSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockInvalidSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(new object());
            mockInvalidSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns((IDesignerHost)null);
            yield return new object[] { mockInvalidSite.Object };
#endif

            var mockNullDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockNullDesignerHost
                .Setup(h => h.Container)
                .Returns((IContainer)null);
            mockNullDesignerHost
                .Setup(h => h.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockNullDesignerHost
                .Setup(h => h.GetService(typeof(IPropertyValueUIService)))
                .Returns(null);
            var mockSite1 = new Mock<ISite>(MockBehavior.Strict);
            mockSite1
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite1
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(new AmbientProperties());
            mockSite1
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockNullDesignerHost.Object);
            yield return new object[] { mockSite1.Object };

            var mockComponentChangeService = new Mock<IComponentChangeService>(MockBehavior.Strict);
            var mockPropertyValueUIService = new Mock<IPropertyValueUIService>(MockBehavior.Strict);
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.Container)
                .Returns((IContainer)null);
            mockDesignerHost
                .Setup(h => h.GetService(typeof(IComponentChangeService)))
                .Returns(mockComponentChangeService.Object);
            mockDesignerHost
                .Setup(h => h.GetService(typeof(IPropertyValueUIService)))
                .Returns(mockPropertyValueUIService.Object);
            var mockSite2 = new Mock<ISite>(MockBehavior.Strict);
            mockSite2
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite2
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(new AmbientProperties());
            mockSite2
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object);
            yield return new object[] { mockSite2.Object };
        }

        [WinFormsTheory]
        [MemberData(nameof(Site_Set_TestData))]
        public void PropertyGrid_Site_Set_GetReturnsExpected(ISite value)
        {
            using var control = new PropertyGrid
            {
                Site = value
            };
            Assert.Same(value, control.Site);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Site = value;
            Assert.Same(value, control.Site);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Site_Set_TestData))]
        public void PropertyGrid_Site_SetWithHandle_GetReturnsExpected(ISite value)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Site = value;
            Assert.Same(value, control.Site);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Site = value;
            Assert.Same(value, control.Site);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_Site_SetInvalidDesignerHost_ThrowsInvalidCastException()
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(new AmbientProperties());
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(new object());
            using var control = new PropertyGrid();
            Assert.Throws<InvalidCastException>(() => control.Site = mockSite.Object);
            Assert.Same(mockSite.Object, control.Site);
            Assert.False(control.IsHandleCreated);

            // Set same.
            Assert.Throws<InvalidCastException>(() => control.Site = mockSite.Object);
            Assert.Same(mockSite.Object, control.Site);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void PropertyGrid_Site_SetInvalidDesignerHostComponentChangeService_ThrowsInvalidCastException()
        {
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.Container)
                .Returns((IContainer)null);
            mockDesignerHost
                .Setup(h => h.GetService(typeof(IComponentChangeService)))
                .Returns(new object());
            mockDesignerHost
                .Setup(h => h.GetService(typeof(IPropertyValueUIService)))
                .Returns(null);
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(new AmbientProperties());
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object);
            using var control = new PropertyGrid();
            Assert.Throws<InvalidCastException>(() => control.Site = mockSite.Object);
            Assert.Same(mockSite.Object, control.Site);
            Assert.False(control.IsHandleCreated);

            // Set same.
            Assert.Throws<InvalidCastException>(() => control.Site = mockSite.Object);
            Assert.Same(mockSite.Object, control.Site);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void PropertyGrid_Site_SetInvalidDesignerHostPropertyValueUIService_ThrowsInvalidCastException()
        {
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.Container)
                .Returns((IContainer)null);
            mockDesignerHost
                .Setup(h => h.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockDesignerHost
                .Setup(h => h.GetService(typeof(IPropertyValueUIService)))
                .Returns(new object());
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(new AmbientProperties());
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object);
            using var control = new PropertyGrid();
            Assert.Throws<InvalidCastException>(() => control.Site = mockSite.Object);
            Assert.Same(mockSite.Object, control.Site);
            Assert.False(control.IsHandleCreated);

            // Set same.
            Assert.Throws<InvalidCastException>(() => control.Site = mockSite.Object);
            Assert.Same(mockSite.Object, control.Site);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void PropertyGrid_Text_Set_GetReturnsExpected(string value, string expected)
        {
            using var control = new PropertyGrid
            {
                Text = value
            };
            Assert.Equal(expected, control.Text);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void PropertyGrid_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_Text_SetWithHandler_CallsTextChanged()
        {
            using var control = new PropertyGrid();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(EventArgs.Empty, e);
                callCount++;
            };
            control.TextChanged += handler;

            // Set different.
            control.Text = "text";
            Assert.Equal("text", control.Text);
            Assert.Equal(1, callCount);

            // Set same.
            control.Text = "text";
            Assert.Equal("text", control.Text);
            Assert.Equal(1, callCount);

            // Set different.
            control.Text = null;
            Assert.Empty(control.Text);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.TextChanged -= handler;
            control.Text = "text";
            Assert.Equal("text", control.Text);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [InlineData(true, true, 0, 0, 1)]
        [InlineData(true, false, 1, 1, 2)]
        [InlineData(false, true, 1, 2, 2)]
        [InlineData(false, false, 0, 0, 1)]
        public void PropertyGrid_ToolbarVisible_Set_GetReturnsExpected(bool visible, bool value, int expectedLayoutCallCount1, int expectedLayoutCallCount2, int expectedLayoutCallCount3)
        {
            using var control = new PropertyGrid
            {
                Visible = visible
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.ToolbarVisible = value;
            Assert.Equal(value, control.ToolbarVisible);
            Assert.Equal(visible && value, control.Controls.OfType<ToolStrip>().Single().Visible);
            Assert.False(control.CommandsVisible);
            Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ToolbarVisible = value;
            Assert.Equal(value, control.ToolbarVisible);
            Assert.Equal(visible && value, control.Controls.OfType<ToolStrip>().Single().Visible);
            Assert.False(control.CommandsVisible);
            Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.ToolbarVisible = !value;
            Assert.Equal(!value, control.ToolbarVisible);
            Assert.Equal(visible && !value, control.Controls.OfType<ToolStrip>().Single().Visible);
            Assert.False(control.CommandsVisible);
            Assert.Equal(expectedLayoutCallCount3, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true, 7, 2, 7, 4, 11, 6)]
        [InlineData(true, false, 11, 2, 11, 4, 15, 6)]
        [InlineData(false, true, 1, 1, 2, 2, 2, 3)]
        [InlineData(false, false, 0, 1, 0, 2, 1, 3)]
        public void PropertyGrid_ToolbarVisible_SetWithHandle_GetReturnsExpected(bool visible, bool value, int expectedLayoutCallCount1, int expectedInvalidatedCallCount1, int expectedLayoutCallCount2, int expectedInvalidatedCallCount2, int expectedLayoutCallCount3, int expectedInvalidatedCallCount3)
        {
            using var control = new PropertyGrid
            {
                Visible = visible
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ToolbarVisible = value;
            Assert.Equal(value, control.ToolbarVisible);
            Assert.Equal(visible && value, control.Controls.OfType<ToolStrip>().Single().Visible);
            Assert.False(control.CommandsVisible);
            Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.ToolbarVisible = value;
            Assert.Equal(value, control.ToolbarVisible);
            Assert.Equal(visible && value, control.Controls.OfType<ToolStrip>().Single().Visible);
            Assert.False(control.CommandsVisible);
            Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.ToolbarVisible = !value;
            Assert.Equal(!value, control.ToolbarVisible);
            Assert.Equal(visible && !value, control.Controls.OfType<ToolStrip>().Single().Visible);
            Assert.False(control.CommandsVisible);
            Assert.Equal(expectedLayoutCallCount3, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount3, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_ToolStripRenderer_Set_GetReturnsExpected()
        {
            var value = new SubToolStripRenderer();
            using var control = new SubPropertyGrid
            {
                ToolStripRenderer = value
            };
            Assert.Same(value, control.ToolStripRenderer);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ToolStripRenderer = value;
            Assert.Same(value, control.ToolStripRenderer);
            Assert.False(control.IsHandleCreated);

            // Set null.
            control.ToolStripRenderer = null;
            Assert.NotNull(control.ToolStripRenderer);
            Assert.Same(control.ToolStripRenderer, control.ToolStripRenderer);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBackColorTheoryData))]
        public void PropertyGrid_ViewBackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new PropertyGrid
            {
                ViewBackColor = value
            };
            Assert.Equal(expected, control.ViewBackColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ViewBackColor = value;
            Assert.Equal(expected, control.ViewBackColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBackColorTheoryData))]
        public void PropertyGrid_ViewBackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ViewBackColor = value;
            Assert.Equal(expected, control.ViewBackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.ViewBackColor = value;
            Assert.Equal(expected, control.ViewBackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_ViewBackColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.ViewBackColor)];
            using var control = new PropertyGrid();
            Assert.False(property.CanResetValue(control));

            control.ViewBackColor = Color.Red;
            Assert.Equal(Color.Red, control.ViewBackColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.Window, control.ViewBackColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_ViewBackColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.ViewBackColor)];
            using var control = new PropertyGrid();
            Assert.False(property.ShouldSerializeValue(control));

            control.ViewBackColor = Color.Red;
            Assert.Equal(Color.Red, control.ViewBackColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.Window, control.ViewBackColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_ViewBackColor_SetTransparent_ThrowsArgmentException()
        {
            using var control = new PropertyGrid();
            Assert.Throws<ArgumentException>(null, () => control.ViewBackColor = Color.FromArgb(254, 1, 2, 3));
        }

        [WinFormsTheory]
        [MemberData(nameof(CategoryForeColor_Set_TestData))]
        public void PropertyGrid_ViewBorderColor_Set_GetReturnsExpected(Color value)
        {
            using var control = new PropertyGrid
            {
                ViewBorderColor = value
            };
            Assert.Equal(value, control.ViewBorderColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ViewBorderColor = value;
            Assert.Equal(value, control.ViewBorderColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(CategoryForeColor_Set_TestData))]
        public void PropertyGrid_ViewBorderColor_SetWithHandle_GetReturnsExpected(Color value)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ViewBorderColor = value;
            Assert.Equal(value, control.ViewBorderColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.ViewBorderColor = value;
            Assert.Equal(value, control.ViewBorderColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_ViewBorderColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.ViewBorderColor)];
            using var control = new PropertyGrid();
            Assert.False(property.CanResetValue(control));

            control.ViewBorderColor = Color.Red;
            Assert.Equal(Color.Red, control.ViewBorderColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.ControlDark, control.ViewBorderColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_ViewBorderColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.ViewBorderColor)];
            using var control = new PropertyGrid();
            Assert.False(property.ShouldSerializeValue(control));

            control.ViewBorderColor = Color.Red;
            Assert.Equal(Color.Red, control.ViewBorderColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.ControlDark, control.ViewBorderColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        public static IEnumerable<object[]> ViewForeColor_Set_TestData()
        {
            yield return new object[] { Color.Empty, SystemColors.ControlText };
            yield return new object[] { Color.Red, Color.Red };
            yield return new object[] { Color.FromArgb(254, 1, 2, 3),Color.FromArgb(254, 1, 2, 3) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ViewForeColor_Set_TestData))]
        public void PropertyGrid_ViewForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new PropertyGrid
            {
                ViewForeColor = value
            };
            Assert.Equal(expected, control.ViewForeColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ViewForeColor = value;
            Assert.Equal(expected, control.ViewForeColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ViewForeColor_Set_TestData))]
        public void PropertyGrid_ViewForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new PropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ViewForeColor = value;
            Assert.Equal(expected, control.ViewForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.ViewForeColor = value;
            Assert.Equal(expected, control.ViewForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void PropertyGrid_ViewForeColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.ViewForeColor)];
            using var control = new PropertyGrid();
            Assert.False(property.CanResetValue(control));

            control.ViewForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ViewForeColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.WindowText, control.ViewForeColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_ViewForeColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[nameof(PropertyGrid.ViewForeColor)];
            using var control = new PropertyGrid();
            Assert.False(property.ShouldSerializeValue(control));

            control.ViewForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ViewForeColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(SystemColors.WindowText, control.ViewForeColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsFact]
        public void PropertyGrid_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubPropertyGrid();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        [WinFormsTheory]
        [InlineData(0, true)]
        [InlineData(SubPropertyGrid.ScrollStateAutoScrolling, false)]
        [InlineData(SubPropertyGrid.ScrollStateFullDrag, false)]
        [InlineData(SubPropertyGrid.ScrollStateHScrollVisible, false)]
        [InlineData(SubPropertyGrid.ScrollStateUserHasScrolled, false)]
        [InlineData(SubPropertyGrid.ScrollStateVScrollVisible, false)]
        [InlineData(int.MaxValue, false)]
        [InlineData((-1), false)]
        public void PropertyGrid_GetScrollState_Invoke_ReturnsExpected(int bit, bool expected)
        {
            using var control = new SubPropertyGrid();
            Assert.Equal(expected, control.GetScrollState(bit));
        }

        [WinFormsTheory]
        [InlineData(ControlStyles.ContainerControl, true)]
        [InlineData(ControlStyles.UserPaint, true)]
        [InlineData(ControlStyles.Opaque, false)]
        [InlineData(ControlStyles.ResizeRedraw, false)]
        [InlineData(ControlStyles.FixedWidth, false)]
        [InlineData(ControlStyles.FixedHeight, false)]
        [InlineData(ControlStyles.StandardClick, true)]
        [InlineData(ControlStyles.Selectable, true)]
        [InlineData(ControlStyles.UserMouse, false)]
        [InlineData(ControlStyles.SupportsTransparentBackColor, false)]
        [InlineData(ControlStyles.StandardDoubleClick, true)]
        [InlineData(ControlStyles.AllPaintingInWmPaint, false)]
        [InlineData(ControlStyles.CacheText, false)]
        [InlineData(ControlStyles.EnableNotifyMessage, false)]
        [InlineData(ControlStyles.DoubleBuffer, false)]
        [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
        [InlineData(ControlStyles.UseTextForAccessibility, false)]
        [InlineData((ControlStyles)0, true)]
        [InlineData((ControlStyles)int.MaxValue, false)]
        [InlineData((ControlStyles)(-1), false)]
        public void PropertyGrid_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubPropertyGrid();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsFact]
        public void PropertyGrid_GetTopLevel_Invoke_ReturnsExpected()
        {
            using var control = new SubPropertyGrid();
            Assert.False(control.GetTopLevel());
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void PropertyGrid_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
        {
            using var control = new SubPropertyGrid();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleCreated += handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void PropertyGrid_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(EventArgs eventArgs)
        {
            using var control = new SubPropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleCreated += handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void PropertyGrid_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubPropertyGrid();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleDestroyed += handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void PropertyGrid_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubPropertyGrid();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleDestroyed += handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyEventArgsTheoryData))]
        public void PropertyGrid_OnKeyDown_Invoke_CallsKeyDown(KeyEventArgs eventArgs)
        {
            using var control = new SubPropertyGrid();
            int callCount = 0;
            KeyEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.KeyDown += handler;
            control.OnKeyDown(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.KeyDown -= handler;
            control.OnKeyDown(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyPressEventArgsTheoryData))]
        public void PropertyGrid_OnKeyPress_Invoke_CallsKeyPress(KeyPressEventArgs eventArgs)
        {
            using var control = new SubPropertyGrid();
            int callCount = 0;
            KeyPressEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.KeyPress += handler;
            control.OnKeyPress(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.KeyPress -= handler;
            control.OnKeyPress(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyEventArgsTheoryData))]
        public void PropertyGrid_OnKeyUp_Invoke_CallsKeyUp(KeyEventArgs eventArgs)
        {
            using var control = new SubPropertyGrid();
            int callCount = 0;
            KeyEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.KeyUp += handler;
            control.OnKeyUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.KeyUp -= handler;
            control.OnKeyUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnMouseDown_TestData()
        {
            yield return new object[] { new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0), false };
            yield return new object[] { new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0), true };
            yield return new object[] { new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0), false };
            yield return new object[] { new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0), true };
            yield return new object[] { new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0), false };
            yield return new object[] { new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0), true };
            yield return new object[] { new MouseEventArgs(MouseButtons.Right, 2, 0, 0, 0), false };
            yield return new object[] { new MouseEventArgs(MouseButtons.Left, 3, 0, 0, 0), true };
            yield return new object[] { new MouseEventArgs(MouseButtons.Right, 3, 0, 0, 0), false };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnMouseDown_TestData))]
        public void PropertyGrid_OnMouseDown_Invoke_CallsMouseDown(MouseEventArgs eventArgs, bool expectedIsHandleCreated)
        {
            using var control = new SubPropertyGrid();
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseDown += handler;
            control.OnMouseDown(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);

            // Remove handler.
            control.MouseDown -= handler;
            control.OnMouseDown(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);
        }

        [WinFormsFact]
        public void PropertyGrid_OnMouseDown_NullE_ThrowsNullReferenceException()
        {
            using var control = new SubPropertyGrid();
            Assert.Throws<NullReferenceException>(() => control.OnMouseDown(null));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void PropertyGrid_OnMouseEnter_Invoke_CallsMouseEnter(EventArgs eventArgs)
        {
            using var control = new SubPropertyGrid();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseEnter += handler;
            control.OnMouseEnter(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.MouseEnter -= handler;
            control.OnMouseEnter(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void PropertyGrid_OnMouseLeave_Invoke_CallsMouseLeave(EventArgs eventArgs)
        {
            using var control = new SubPropertyGrid();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseLeave += handler;
            control.OnMouseLeave(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.MouseLeave -= handler;
            control.OnMouseLeave(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnMouseMove_TestData()
        {
            yield return new object[] { new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0) };
            yield return new object[] { new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0) };
            yield return new object[] { new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0) };
            yield return new object[] { new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0) };
            yield return new object[] { new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0) };
            yield return new object[] { new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0) };
            yield return new object[] { new MouseEventArgs(MouseButtons.Right, 2, 0, 0, 0) };
            yield return new object[] { new MouseEventArgs(MouseButtons.Left, 3, 0, 0, 0) };
            yield return new object[] { new MouseEventArgs(MouseButtons.Right, 3, 0, 0, 0) };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnMouseMove_TestData))]
        public void PropertyGrid_OnMouseMove_Invoke_CallsMouseMove(MouseEventArgs eventArgs)
        {
            using var control = new SubPropertyGrid();
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseMove += handler;
            control.OnMouseMove(eventArgs);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.MouseMove -= handler;
            control.OnMouseMove(eventArgs);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void PropertyGrid_OnMouseMove_NullE_ThrowsNullReferenceException()
        {
            using var control = new SubPropertyGrid();
            Assert.Throws<NullReferenceException>(() => control.OnMouseMove(null));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void PropertyGrid_OnMouseUp_Invoke_CallsMouseUp(MouseEventArgs eventArgs)
        {
            using var control = new SubPropertyGrid();
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseUp += handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.MouseUp -= handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }

        private class SubToolStripRenderer : ToolStripRenderer
        {
        }

        private class SubPropertyGrid : PropertyGrid
        {
            public new const int ScrollStateAutoScrolling = PropertyGrid.ScrollStateAutoScrolling;

            public new const int ScrollStateHScrollVisible = PropertyGrid.ScrollStateHScrollVisible;

            public new const int ScrollStateVScrollVisible = PropertyGrid.ScrollStateVScrollVisible;

            public new const int ScrollStateUserHasScrolled = PropertyGrid.ScrollStateUserHasScrolled;

            public new const int ScrollStateFullDrag = PropertyGrid.ScrollStateFullDrag;

            public new SizeF AutoScaleFactor => base.AutoScaleFactor;

            public new bool CanEnableIme => base.CanEnableIme;

            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new CreateParams CreateParams => base.CreateParams;

            public new Cursor DefaultCursor => base.DefaultCursor;

            public new ImeMode DefaultImeMode => base.DefaultImeMode;

            public new Padding DefaultMargin => base.DefaultMargin;

            public new Size DefaultMaximumSize => base.DefaultMaximumSize;

            public new Size DefaultMinimumSize => base.DefaultMinimumSize;

            public new Padding DefaultPadding => base.DefaultPadding;

            public new Size DefaultSize => base.DefaultSize;

            public new Type DefaultTabType => base.DefaultTabType;

            public new bool DesignMode => base.DesignMode;

            public new bool DoubleBuffered
            {
                get => base.DoubleBuffered;
                set => base.DoubleBuffered = value;
            }

            public new bool DrawFlatToolbar
            {
                get => base.DrawFlatToolbar;
                set => base.DrawFlatToolbar = value;
            }

            public new EventHandlerList Events => base.Events;

            public new int FontHeight
            {
                get => base.FontHeight;
                set => base.FontHeight = value;
            }

            public new bool HScroll
            {
                get => base.HScroll;
                set => base.HScroll = value;
            }

            public new ImeMode ImeModeBase
            {
                get => base.ImeModeBase;
                set => base.ImeModeBase = value;
            }

            public new bool ResizeRedraw
            {
                get => base.ResizeRedraw;
                set => base.ResizeRedraw = value;
            }

            public new bool ShowFocusCues => base.ShowFocusCues;

            public new bool ShowKeyboardCues => base.ShowKeyboardCues;

            public new ToolStripRenderer ToolStripRenderer
            {
                get => base.ToolStripRenderer;
                set => base.ToolStripRenderer = value;
            }

            public new bool VScroll
            {
                get => base.VScroll;
                set => base.VScroll = value;
            }

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetScrollState(int bit) => base.GetScrollState(bit);

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new bool GetTopLevel() => base.GetTopLevel();

            public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

            public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

            public new void OnKeyDown(KeyEventArgs e) => base.OnKeyDown(e);

            public new void OnKeyPress(KeyPressEventArgs e) => base.OnKeyPress(e);

            public new void OnKeyUp(KeyEventArgs e) => base.OnKeyUp(e);

            public new void OnMouseDown(MouseEventArgs e) => base.OnMouseDown(e);

            public new void OnMouseEnter(EventArgs eventargs) => base.OnMouseEnter(eventargs);

            public new void OnMouseLeave(EventArgs eventargs) => base.OnMouseLeave(eventargs);

            public new void OnMouseMove(MouseEventArgs eventargs) => base.OnMouseMove(eventargs);

            public new void OnMouseUp(MouseEventArgs eventargs) => base.OnMouseUp(eventargs);

            public new void WndProc(ref Message m) => base.WndProc(ref m);
        }
    }
}
