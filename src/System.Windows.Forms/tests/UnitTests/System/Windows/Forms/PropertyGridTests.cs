// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.PropertyGridInternal;
using Moq;
using System.Windows.Forms.TestUtilities;
using System.Runtime.CompilerServices;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.Tests;

public partial class PropertyGridTests
{
    [WinFormsFact]
    public void PropertyGrid_Ctor_Default()
    {
        using SubPropertyGrid control = new();
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
        Assert.Equal(AutoScaleMode.Inherit, control.AutoScaleMode);
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

        PropertyGridView propertyGridView = control.TestAccessor().GridView;
        Assert.NotNull(propertyGridView);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void PropertyGrid_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubPropertyGrid control = new();
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
        using SubPropertyGrid control = new();
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
        using SubPropertyGrid control = new();
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
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
    public void PropertyGrid_BackColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using PropertyGrid control = new()
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
        int callCount = 0;

        void handler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        }

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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void PropertyGrid_BackgroundImage_Set_GetReturnsExpected(Image value)
    {
        using PropertyGrid control = new()
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
        using PropertyGrid control = new();
        int callCount = 0;

        void handler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        }

        control.BackgroundImageChanged += handler;

        // Set different.
        using Bitmap image1 = new(10, 10);
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Equal(1, callCount);

        // Set same.
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Equal(1, callCount);

        // Set different.
        using Bitmap image2 = new(10, 10);
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
    [EnumData<ImageLayout>]
    public void PropertyGrid_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
    {
        using PropertyGrid control = new()
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
        using PropertyGrid control = new();
        int callCount = 0;

        void handler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        }

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
    [InvalidEnumData<ImageLayout>]
    public void PropertyGrid_BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
    {
        using PropertyGrid control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.BackgroundImageLayout = value);
    }

    public static IEnumerable<object[]> BrowsableAttributes_Set_TestData()
    {
        yield return new object[] { null, new AttributeCollection([BrowsableAttribute.Yes]) };
        yield return new object[] { AttributeCollection.Empty, new AttributeCollection([BrowsableAttribute.Yes]) };
        yield return new object[] { new AttributeCollection(), new AttributeCollection() };
        yield return new object[] { new AttributeCollection([BrowsableAttribute.Yes]), new AttributeCollection([BrowsableAttribute.Yes]) };
        yield return new object[] { new AttributeCollection([BrowsableAttribute.Yes, ReadOnlyAttribute.Yes]), new AttributeCollection([BrowsableAttribute.Yes, ReadOnlyAttribute.Yes]) };
    }

    [WinFormsTheory]
    [MemberData(nameof(BrowsableAttributes_Set_TestData))]
    public void PropertyGrid_BrowsableAttributes_Set_GetReturnsExpected(AttributeCollection value, AttributeCollection expected)
    {
        using PropertyGrid control = new()
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
        using PropertyGrid control = new()
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
        using PropertyGrid control = new()
        {
            SelectedObjects = [1],
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
        using PropertyGrid control = new();
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
    [BoolData]
    public void PropertyGrid_CanShowVisualStyleGlyphs_Set_GetReturnsExpected(bool value)
    {
        using SubPropertyGrid control = new()
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
    [BoolData]
    public void PropertyGrid_CanShowVisualStyleGlyphs_SetWithHandle_GetReturnsExpected(bool value)
    {
        using PropertyGrid control = new();
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
        using PropertyGrid control = new()
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new()
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new()
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.CommandsActiveLinkColor = Color.Black;
        Assert.Equal(Color.Black, control.CommandsActiveLinkColor);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(Color.Red, control.CommandsActiveLinkColor);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
    public void PropertyGrid_CommandsBackColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using PropertyGrid control = new()
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
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
    public void PropertyGrid_CommandsBackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected)
    {
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
        Assert.Throws<ArgumentException>(() => control.CommandsBackColor = Color.FromArgb(254, 1, 2, 3));
    }

    [WinFormsTheory]
    [MemberData(nameof(CategoryForeColor_Set_TestData))]
    public void PropertyGrid_CommandsBorderColor_Set_GetReturnsExpected(Color value)
    {
        using PropertyGrid control = new()
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new()
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.CommandsDisabledLinkColor = Color.Red;
        Assert.Equal(Color.Red, control.CommandsDisabledLinkColor);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(Color.FromArgb(255, 133, 133, 133), control.CommandsDisabledLinkColor);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetForeColorTheoryData))]
    public void PropertyGrid_CommandsForeColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using PropertyGrid control = new()
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
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetForeColorTheoryData))]
    public void PropertyGrid_CommandsForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected)
    {
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new()
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new()
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
        using PropertyGrid control = new()
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
        using PropertyGrid control = new()
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.DisabledItemForeColor = Color.Red;
        Assert.Equal(Color.Red, control.DisabledItemForeColor);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(SystemColors.GrayText, control.DisabledItemForeColor);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsTheory]
    [BoolData]
    public void PropertyGrid_DrawFlatToolbar_Set_GetReturnsExpected(bool value)
    {
        using SubPropertyGrid control = new()
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
        using SubPropertyGrid control = new()
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
    [BoolData]
    public void PropertyGrid_DrawFlatToolbar_SetWithHandle_GetReturnsExpected(bool value)
    {
        using SubPropertyGrid control = new();
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
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetForeColorTheoryData))]
    public void PropertyGrid_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using PropertyGrid control = new()
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
    public void PropertyGrid_HelpBackColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using PropertyGrid control = new()
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
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
    public void PropertyGrid_HelpBackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected)
    {
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
        Assert.Throws<ArgumentException>(() => control.HelpBackColor = Color.FromArgb(254, 1, 2, 3));
    }

    [WinFormsTheory]
    [MemberData(nameof(CategoryForeColor_Set_TestData))]
    public void PropertyGrid_HelpBorderColor_Set_GetReturnsExpected(Color value)
    {
        using PropertyGrid control = new()
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.HelpBorderColor = Color.Red;
        Assert.Equal(Color.Red, control.HelpBorderColor);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(SystemColors.ControlDark, control.HelpBorderColor);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetForeColorTheoryData))]
    public void PropertyGrid_HelpForeColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using PropertyGrid control = new()
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
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetForeColorTheoryData))]
    public void PropertyGrid_HelpForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected)
    {
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new()
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
    public void PropertyGrid_HelpVisible_SetWithHandle_GetReturnsExpected(
        bool visible,
        bool value,
        int expectedLayoutCallCount1,
        int expectedInvalidatedCallCount,
        int expectedLayoutCallCount2,
        int expectedLayoutCallCount3)
    {
        using PropertyGrid control = new()
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
        using PropertyGrid control = new()
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
        using PropertyGrid control = new()
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
        using PropertyGrid control = new()
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.LineColor = Color.Red;
        Assert.Equal(Color.Red, control.LineColor);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(SystemColors.InactiveBorder, control.LineColor);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingNormalizedTheoryData))]
    public void PropertyGrid_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
    {
        using PropertyGrid control = new()
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
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingNormalizedTheoryData))]
    public void PropertyGrid_Padding_SetWithHandle_GetReturnsExpected(Padding value, Padding expected)
    {
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
        int callCount = 0;

        void handler(object sender, EventArgs e)
        {
            Assert.Equal(control, sender);
            Assert.Equal(EventArgs.Empty, e);
            callCount++;
        }

        control.PaddingChanged += handler;

        // Set different.
        Padding padding1 = new(1);
        control.Padding = padding1;
        Assert.Equal(padding1, control.Padding);
        Assert.Equal(1, callCount);

        // Set same.
        control.Padding = padding1;
        Assert.Equal(padding1, control.Padding);
        Assert.Equal(1, callCount);

        // Set different.
        Padding padding2 = new(2);
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
    [EnumData<PropertySort>]
    public void PropertyGrid_PropertySort_Set_GetReturnsExpected(PropertySort value)
    {
        using PropertyGrid control = new()
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
    [EnumData<PropertySort>]
    public void PropertyGrid_PropertySort_SetWithHandle_GetReturnsExpected(PropertySort value)
    {
        using PropertyGrid control = new();
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
    public void PropertyGrid_PropertyTabCollection_AddAndRemoveTabType_Success()
    {
        using PropertyGrid grid = new();
        Assert.Single(grid.PropertyTabs);

        grid.PropertyTabs.AddTabType(typeof(TestPropertyTab));
        Assert.Equal(2, grid.PropertyTabs.Count);

        grid.PropertyTabs.RemoveTabType(typeof(TestPropertyTab));
        Assert.Single(grid.PropertyTabs);
    }

    private class TestPropertyTab : PropertyTab
    {
        public override string TabName => "TestTabName";

        public override Bitmap Bitmap => new(10, 10);

        public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes) => throw new NotImplementedException();
    }

    [WinFormsFact]
    public void PropertyGrid_SelectedGridItem_SetNull_ThrowsArgumentNullException()
    {
        using PropertyGrid control = new();
        Assert.Throws<ArgumentNullException>("items", () => control.SelectedGridItem = null);
    }

    [WinFormsFact]
    public void PropertyGrid_SelectedGridItem_SetNotGridEntry_ThrowsInvalidCastException()
    {
        using PropertyGrid control = new();
        Mock<GridItem> mockGridItem = new(MockBehavior.Strict);
        Assert.Throws<InvalidCastException>(() => control.SelectedGridItem = mockGridItem.Object);
    }

    [WinFormsTheory]
    [MemberData(nameof(CategoryForeColor_Set_TestData))]
    public void PropertyGrid_SelectedItemWithFocusBackColor_Set_GetReturnsExpected(Color value)
    {
        using PropertyGrid control = new()
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new()
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new()
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new()
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
        using PropertyGrid control = new()
        {
            SelectedObjects = [1]
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
        using PropertyGrid control = new();
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
        using SubPropertyGrid control = new();
        Assert.Throws<ArgumentException>(() => control.SelectedObjects = [null]);
    }

    private ISite CreateISiteObject()
    {
        Mock<IComponentChangeService> mockComponentChangeService = new(MockBehavior.Strict);
        Mock<IPropertyValueUIService> mockPropertyValueUIService = new(MockBehavior.Strict);
        Mock<IDesignerHost> mockDesignerHost = new(MockBehavior.Strict);
        mockDesignerHost
           .Setup(h => h.Container)
           .Returns((IContainer)null);
        mockDesignerHost
            .Setup(h => h.GetService(typeof(IComponentChangeService)))
            .Returns(mockComponentChangeService.Object);
        mockDesignerHost
            .Setup(h => h.GetService(typeof(IPropertyValueUIService)))
            .Returns(mockPropertyValueUIService.Object);
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(new AmbientProperties());
        mockSite
            .Setup(s => s.GetService(typeof(IDesignerHost)))
            .Returns(mockDesignerHost.Object);
        return mockSite.Object;
    }

    [WinFormsFact]
    public void PropertyGrid_Site_ShouldSaveSelectedTabIndex()
    {
        using PropertyGrid propertyGrid = new();
        var propertyGridTestAccessor = propertyGrid.TestAccessor();

        propertyGrid.Site = CreateISiteObject();
        Assert.NotNull(propertyGrid.ActiveDesigner);

        propertyGridTestAccessor.SaveSelectedTabIndex();
        Dictionary<int, int> _designerSelections = propertyGridTestAccessor._designerSelections;
        Assert.NotNull(_designerSelections);
        Assert.True(_designerSelections.ContainsKey(propertyGrid.ActiveDesigner.GetHashCode()));

        int savedTabIndex = _designerSelections[propertyGrid.ActiveDesigner.GetHashCode()];
        int selectedTabIndex = -1;
        Assert.NotEqual(savedTabIndex, selectedTabIndex);

        bool isInvokeMethodSuccessful = propertyGridTestAccessor.TryGetSavedTabIndex(out selectedTabIndex);
        Assert.True(isInvokeMethodSuccessful);
        Assert.Equal(savedTabIndex, selectedTabIndex);
    }

    [WinFormsFact]
    public void PropertyGrid_SiteChange_ShouldNotSaveSelectedTabIndex()
    {
        using PropertyGrid propertyGrid = new();
        var propertyGridTestAccessor = propertyGrid.TestAccessor();
        propertyGrid.Site = CreateISiteObject();
        var previousActiveDesigner = propertyGrid.ActiveDesigner;
        propertyGridTestAccessor.SaveSelectedTabIndex();

        // Set other Site
        propertyGrid.Site = CreateISiteObject();
        Assert.NotNull(propertyGrid.ActiveDesigner);
        Assert.NotEqual(previousActiveDesigner, propertyGrid.ActiveDesigner);

        int selectedTabIndex = -1;
        bool isInvokeMethodSuccessful = propertyGridTestAccessor.TryGetSavedTabIndex(out selectedTabIndex);
        Assert.False(isInvokeMethodSuccessful);
        Assert.Equal(-1, selectedTabIndex);

        Dictionary<int, int> _designerSelections = propertyGridTestAccessor._designerSelections;
        Assert.NotNull(_designerSelections);
        Assert.True(_designerSelections.ContainsKey(previousActiveDesigner.GetHashCode()));
    }

    public static IEnumerable<object[]> Site_Set_TestData()
    {
        yield return new object[] { null };

        Mock<ISite> mockNullSite = new(MockBehavior.Strict);
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
        Mock<ISite> mockInvalidSite = new(MockBehavior.Strict);
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

        Mock<IDesignerHost> mockNullDesignerHost = new(MockBehavior.Strict);
        mockNullDesignerHost
            .Setup(h => h.Container)
            .Returns((IContainer)null);
        mockNullDesignerHost
            .Setup(h => h.GetService(typeof(IComponentChangeService)))
            .Returns(null);
        mockNullDesignerHost
            .Setup(h => h.GetService(typeof(IPropertyValueUIService)))
            .Returns(null);
        Mock<ISite> mockSite1 = new(MockBehavior.Strict);
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

        Mock<IComponentChangeService> mockComponentChangeService = new(MockBehavior.Strict);
        Mock<IPropertyValueUIService> mockPropertyValueUIService = new(MockBehavior.Strict);
        Mock<IDesignerHost> mockDesignerHost = new(MockBehavior.Strict);
        mockDesignerHost
            .Setup(h => h.Container)
            .Returns((IContainer)null);
        mockDesignerHost
            .Setup(h => h.GetService(typeof(IComponentChangeService)))
            .Returns(mockComponentChangeService.Object);
        mockDesignerHost
            .Setup(h => h.GetService(typeof(IPropertyValueUIService)))
            .Returns(mockPropertyValueUIService.Object);
        Mock<ISite> mockSite2 = new(MockBehavior.Strict);
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
        using PropertyGrid control = new()
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
        using PropertyGrid control = new();
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
    public void PropertyGrid_Site_SetInvalidDesignerHost_DoesNotThrowInvalidCastException()
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(new AmbientProperties());
        mockSite
            .Setup(s => s.GetService(typeof(IDesignerHost)))
            .Returns(new object());
        using PropertyGrid control = new();
        control.Site = mockSite.Object;
        Assert.Same(mockSite.Object, control.Site);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Site = mockSite.Object;
        Assert.Same(mockSite.Object, control.Site);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void PropertyGrid_Site_SetInvalidDesignerHostComponentChangeService_DoesNotThrowInvalidCastException()
    {
        Mock<IDesignerHost> mockDesignerHost = new(MockBehavior.Strict);
        mockDesignerHost
            .Setup(h => h.Container)
            .Returns((IContainer)null);
        mockDesignerHost
            .Setup(h => h.GetService(typeof(IComponentChangeService)))
            .Returns(new object());
        mockDesignerHost
            .Setup(h => h.GetService(typeof(IPropertyValueUIService)))
            .Returns(null);
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(new AmbientProperties());
        mockSite
            .Setup(s => s.GetService(typeof(IDesignerHost)))
            .Returns(mockDesignerHost.Object);
        using PropertyGrid control = new();
        control.Site = mockSite.Object;
        Assert.Same(mockSite.Object, control.Site);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Site = mockSite.Object;
        Assert.Same(mockSite.Object, control.Site);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void PropertyGrid_Site_SetInvalidDesignerHostPropertyValueUIService_DoesNotThrowInvalidCastException()
    {
        Mock<IDesignerHost> mockDesignerHost = new(MockBehavior.Strict);
        mockDesignerHost
            .Setup(h => h.Container)
            .Returns((IContainer)null);
        mockDesignerHost
            .Setup(h => h.GetService(typeof(IComponentChangeService)))
            .Returns(null);
        mockDesignerHost
            .Setup(h => h.GetService(typeof(IPropertyValueUIService)))
            .Returns(new object());
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(new AmbientProperties());
        mockSite
            .Setup(s => s.GetService(typeof(IDesignerHost)))
            .Returns(mockDesignerHost.Object);
        using PropertyGrid control = new();
        control.Site = mockSite.Object;
        Assert.Same(mockSite.Object, control.Site);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Site = mockSite.Object;
        Assert.Same(mockSite.Object, control.Site);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void PropertyGrid_Text_Set_GetReturnsExpected(string value, string expected)
    {
        using PropertyGrid control = new()
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
    [NormalizedStringData]
    public void PropertyGrid_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
    {
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
        int callCount = 0;

        void handler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Equal(EventArgs.Empty, e);
            callCount++;
        }

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
        using PropertyGrid control = new()
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
        using PropertyGrid control = new()
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
        SubToolStripRenderer value = new();
        using SubPropertyGrid control = new()
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
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
    public void PropertyGrid_ViewBackColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using PropertyGrid control = new()
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
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
    public void PropertyGrid_ViewBackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected)
    {
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
        Assert.Throws<ArgumentException>(() => control.ViewBackColor = Color.FromArgb(254, 1, 2, 3));
    }

    [WinFormsTheory]
    [MemberData(nameof(CategoryForeColor_Set_TestData))]
    public void PropertyGrid_ViewBorderColor_Set_GetReturnsExpected(Color value)
    {
        using PropertyGrid control = new()
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3) };
    }

    [WinFormsTheory]
    [MemberData(nameof(ViewForeColor_Set_TestData))]
    public void PropertyGrid_ViewForeColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using PropertyGrid control = new()
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using PropertyGrid control = new();
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
        using SubPropertyGrid control = new();
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
        using SubPropertyGrid control = new();
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
    [InlineData(ControlStyles.UseTextForAccessibility, true)]
    [InlineData((ControlStyles)0, true)]
    [InlineData((ControlStyles)int.MaxValue, false)]
    [InlineData((ControlStyles)(-1), false)]
    public void PropertyGrid_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubPropertyGrid control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void PropertyGrid_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubPropertyGrid control = new();
        Assert.False(control.GetTopLevel());
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void PropertyGrid_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
    {
        using SubPropertyGrid control = new();
        int callCount = 0;

        void handler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

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
    [NewAndDefaultData<EventArgs>]
    public void PropertyGrid_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(EventArgs eventArgs)
    {
        using SubPropertyGrid control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int callCount = 0;

        void handler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

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
    [NewAndDefaultData<EventArgs>]
    public void PropertyGrid_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using SubPropertyGrid control = new();
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

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
    [NewAndDefaultData<EventArgs>]
    public void PropertyGrid_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using SubPropertyGrid control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int callCount = 0;

        void handler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

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
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetKeyEventArgsTheoryData))]
    public void PropertyGrid_OnKeyDown_Invoke_CallsKeyDown(KeyEventArgs eventArgs)
    {
        using SubPropertyGrid control = new();
        int callCount = 0;

        void handler(object sender, KeyEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

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
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetKeyPressEventArgsTheoryData))]
    public void PropertyGrid_OnKeyPress_Invoke_CallsKeyPress(KeyPressEventArgs eventArgs)
    {
        using SubPropertyGrid control = new();
        int callCount = 0;

        void handler(object sender, KeyPressEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

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
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetKeyEventArgsTheoryData))]
    public void PropertyGrid_OnKeyUp_Invoke_CallsKeyUp(KeyEventArgs eventArgs)
    {
        using SubPropertyGrid control = new();
        int callCount = 0;

        void handler(object sender, KeyEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

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
        using SubPropertyGrid control = new();
        int callCount = 0;

        void handler(object sender, MouseEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

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
        using SubPropertyGrid control = new();
        Assert.Throws<NullReferenceException>(() => control.OnMouseDown(null));
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void PropertyGrid_OnMouseEnter_Invoke_CallsMouseEnter(EventArgs eventArgs)
    {
        using SubPropertyGrid control = new();
        int callCount = 0;

        void handler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

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
    [NewAndDefaultData<EventArgs>]
    public void PropertyGrid_OnMouseLeave_Invoke_CallsMouseLeave(EventArgs eventArgs)
    {
        using SubPropertyGrid control = new();
        int callCount = 0;

        void handler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

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
        using SubPropertyGrid control = new();
        int callCount = 0;
        void handler(object sender, MouseEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

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
        using SubPropertyGrid control = new();
        Assert.Throws<NullReferenceException>(() => control.OnMouseMove(null));
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetMouseEventArgsTheoryData))]
    public void PropertyGrid_OnMouseUp_Invoke_CallsMouseUp(MouseEventArgs eventArgs)
    {
        using SubPropertyGrid control = new();
        int callCount = 0;

        void handler(object sender, MouseEventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        }

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

    [WinFormsFact]
    public void PropertyGrid_Buttons_AccessibleRole_IsRadiButton()
    {
        using PropertyGrid propertyGrid = new();
        ToolStripButton[] toolStripButtons = propertyGrid.TestAccessor().Dynamic._viewSortButtons;
        ToolStripButton categoryButton = toolStripButtons[0];
        ToolStripButton alphaButton = toolStripButtons[1];

        Assert.Equal(AccessibleRole.RadioButton, categoryButton.AccessibleRole);
        Assert.Equal(AccessibleRole.RadioButton, alphaButton.AccessibleRole);
    }

    [WinFormsFact]
    public void PropertyGrid_SystemColorsChanged_DoesNotLeakImageList()
    {
        using SubPropertyGrid propertyGrid = new();

        ImageList normalButtons = propertyGrid.TestAccessor().Dynamic._normalButtonImages;

        Assert.NotNull(normalButtons);

        propertyGrid.OnSystemColorsChanged(EventArgs.Empty);

        ImageList newNormalButtons = propertyGrid.TestAccessor().Dynamic._normalButtonImages;
        Assert.NotNull(newNormalButtons);
        Assert.NotSame(normalButtons, newNormalButtons);

        GC.Collect();
        GC.WaitForPendingFinalizers();

#if DEBUG
        Assert.True(normalButtons.IsDisposed);
#endif
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void PropertyGrid_SetEnableFalse_DoesntBreakEntries_AndFlagsReturnCorrectValue(bool enable)
    {
        using PropertyGrid propertyGrid = new();
        using Button button = new();
        propertyGrid.SelectedObject = button;
        propertyGrid.Enabled = enable;
        var entry = (GridEntry)propertyGrid.SelectedGridItem;

        Assert.NotEqual(0, (int)entry.EntryFlags);
        Assert.False(propertyGrid.IsHandleCreated);
        Assert.False(button.IsHandleCreated);
    }

    [WinFormsFact]
    public void PropertyGrid_BindObject()
    {
        using PropertyGrid propertyGrid = new();
        object @object = new();
        propertyGrid.SelectedObject = @object;
        GridItem selectedItem = propertyGrid.SelectedGridItem;
        Assert.True(selectedItem is not null);
        Assert.Equal("System.Object", selectedItem.Label);
        Assert.IsAssignableFrom<IRootGridEntry>(selectedItem);
        SingleSelectRootGridEntry gridEntry = Assert.IsAssignableFrom<SingleSelectRootGridEntry>(selectedItem);
        AttributeCollection browsableAttributes = gridEntry.BrowsableAttributes;
        Assert.Single(browsableAttributes);
        Assert.Equal(BrowsableAttribute.Yes, browsableAttributes[0]);
        Type propertyType = gridEntry.PropertyType;
        Assert.True(propertyType == typeof(object));

        AttributeCollection attributes = gridEntry.TestAccessor().Dynamic.Attributes;
        bool foundTypeForward = false;
        foreach (object attribute in attributes)
        {
            if (attribute is TypeForwardedFromAttribute forwardedFrom)
            {
                Assert.Equal(
                    "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                    forwardedFrom.AssemblyFullName);
                foundTypeForward = true;
                break;
            }
        }

        Assert.True(foundTypeForward, "Did not find TypeForwardedAttribute.");

        TypeConverter typeConverter = gridEntry.TypeConverter;
        Assert.IsType<TypeConverter>(typeConverter);
        propertyGrid.Enabled = true;
    }

    [WinFormsFact]
    public void PropertyGrid_NotifyParentChange()
    {
        // Regression test for https://github.com/dotnet/winforms/issues/10427
        using PropertyGrid propertyGrid = new();
        propertyGrid.Site = new MySite();
        MyClass myClass = new();
        propertyGrid.SelectedObject = myClass;
        PropertyGridView propertyGridView = (PropertyGridView)propertyGrid.Controls[2];
        GridEntry entry = propertyGridView.SelectedGridEntry;
        var descriptor = entry.GridItems[0] as PropertyDescriptorGridEntry;

        descriptor.SetPropertyTextValue("123");
        Assert.Equal("123", myClass.ParentGridEntry.NestedGridEntry);
    }

    [WinFormsFact]
    public void PropertyGrid_PropertyValueChanged_Invoke_CallsPropertyValueChanged()
    {
        using PropertyGrid propertyGrid = new();
        int callCount = 0;
        PropertyValueChangedEventHandler handler = (sender, e) =>
        {
            sender.Should().BeSameAs(propertyGrid);
            e.Should().NotBeNull();
            e.ChangedItem.Should().NotBeNull();
            e.OldValue.Should().Be(0);
            callCount++;
        };

        propertyGrid.PropertyValueChanged += handler;
        var accessor = propertyGrid.TestAccessor();
        var gridItem = new Mock<GridItem>().Object;
        accessor.Dynamic.OnPropertyValueChanged(new PropertyValueChangedEventArgs(gridItem, 0));
        callCount.Should().Be(1);

        propertyGrid.PropertyValueChanged -= handler;
        accessor.Dynamic.OnPropertyValueChanged(new PropertyValueChangedEventArgs(gridItem, 0));
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void PropertyGrid_PropertySortChangedEventTriggered()
    {
        using PropertyGrid propertyGrid = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().BeSameAs(propertyGrid);
            e.Should().BeSameAs(EventArgs.Empty);
            callCount++;
        };

        propertyGrid.PropertySortChanged += handler;
        propertyGrid.PropertySort = PropertySort.Alphabetical;
        callCount.Should().Be(1);

        propertyGrid.PropertySort = PropertySort.Categorized;
        callCount.Should().Be(2);

        propertyGrid.PropertySortChanged -= handler;
        propertyGrid.PropertySort = PropertySort.Alphabetical;
        callCount.Should().Be(2);
    }

    [WinFormsFact]
    public void PropertyGrid_SelectedObjectsChanged_Invoke_CallsSelectedObjectsChanged()
    {
        using PropertyGrid propertyGrid = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(propertyGrid);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        propertyGrid.SelectedObjectsChanged += handler;
        propertyGrid.SelectedObjects = [new object()];
        callCount.Should().Be(1);

        propertyGrid.SelectedObjects = [new object(), new object()];
        callCount.Should().Be(2);

        propertyGrid.SelectedObjectsChanged -= handler;
        propertyGrid.SelectedObjects = [new object()];
        callCount.Should().Be(2);
    }

    [WinFormsFact]
    public void PropertyGrid_ResetSelectedProperty_Invoke_Success()
    {
        using PropertyGrid propertyGrid = new();
        var testObject = new TestObject { Property1 = "ChangedValue1" };
        propertyGrid.SelectedObject = testObject;

        propertyGrid.ResetSelectedProperty();

        var selectedGridItem = propertyGrid.SelectedGridItem;
        selectedGridItem.Should().NotBeNull();
        selectedGridItem.Value.Should().Be("Default1");
    }

    public class TestObject
    {
        [DefaultValue("Default1")]
        public string Property1 { get; set; }
    }

    [WinFormsFact]
    public void PropertyGrid_PropertyTabChangedEventTriggered()
    {
        using PropertyGrid propertyGrid = new();
        int eventCallCount = 0;
        PropertyTabChangedEventArgs eventArgs = null;

        propertyGrid.PropertyTabChanged += (sender, e) =>
        {
            eventCallCount++;
            eventArgs = e;
        };

        TestPropertyTab tab1 = new();
        TestPropertyTab tab2 = new();

        var oldTab = tab2;
        var newTab = tab1;
        var accessor = propertyGrid.TestAccessor();
        accessor.Dynamic.OnPropertyTabChanged(new PropertyTabChangedEventArgs(oldTab, newTab));

        eventCallCount.Should().Be(1);
        eventArgs.Should().NotBeNull();
        eventArgs.OldTab.Should().Be(oldTab);
        eventArgs.NewTab.Should().Be(newTab);
    }

    [WinFormsFact]
    public void PropertyGrid_SelectedGridItemChanged_TriggeredCorrectly()
    {
        using PropertyGrid propertyGrid = new();
        int eventCallCount = 0;
        SelectedGridItemChangedEventArgs eventArgs = null;
        object actualSender = null;

        propertyGrid.SelectedGridItemChanged += (sender, e) =>
        {
            eventCallCount++;
            eventArgs = e;
            actualSender = sender;
        };

        Mock<GridItem> gridItemMock = new();
        var gridItem = gridItemMock.Object;

        var accessor = propertyGrid.TestAccessor();
        accessor.Dynamic.OnSelectedGridItemChanged(new SelectedGridItemChangedEventArgs(null, gridItem));

        eventCallCount.Should().Be(1);
        eventArgs.Should().NotBeNull();
        actualSender.Should().Be(propertyGrid);
        eventArgs.NewSelection.Should().Be(gridItem);
    }

    private class SubToolStripRenderer : ToolStripRenderer
    {
    }

    private class SubPropertyGrid : PropertyGrid
    {
        public new const int ScrollStateAutoScrolling = ScrollableControl.ScrollStateAutoScrolling;

        public new const int ScrollStateHScrollVisible = ScrollableControl.ScrollStateHScrollVisible;

        public new const int ScrollStateVScrollVisible = ScrollableControl.ScrollStateVScrollVisible;

        public new const int ScrollStateUserHasScrolled = ScrollableControl.ScrollStateUserHasScrolled;

        public new const int ScrollStateFullDrag = ScrollableControl.ScrollStateFullDrag;

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

        public new void OnSystemColorsChanged(EventArgs e) => base.OnSystemColorsChanged(e);

        public new void WndProc(ref Message m) => base.WndProc(ref m);
    }

    private class MyClass
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual MyExpandableClass ParentGridEntry { get; set; } = new() { };

        public class MyExpandableClass
        {
            [NotifyParentProperty(true)]
            [TypeConverter(typeof(StringConverter))]
            public string NestedGridEntry { get; set; }
        }
    }

    private class MySite : ISite
    {
        private readonly IComponentChangeService _componentChangeService = new ComponentChangeService();
        public IComponent Component => null;
        public IContainer Container => null;
        public bool DesignMode => false;
        public string Name { get; set; }

        public object GetService(Type serviceType)
            => serviceType == typeof(IComponentChangeService) ? _componentChangeService : null;

        public class ComponentChangeService : IComponentChangeService
        {
#pragma warning disable CS0067 // Required by Interface
            public event ComponentEventHandler ComponentAdded;
            public event ComponentEventHandler ComponentAdding;
            public event ComponentChangedEventHandler ComponentChanged;
            public event ComponentChangingEventHandler ComponentChanging;
            public event ComponentEventHandler ComponentRemoved;
            public event ComponentEventHandler ComponentRemoving;
            public event ComponentRenameEventHandler ComponentRename;
#pragma warning restore

            public void OnComponentChanged(object component, MemberDescriptor member, object oldValue, object newValue)
            {
            }

            public void OnComponentChanging(object component, MemberDescriptor member)
            {
                if (member is null)
                    return;
                Reflection.MemberInfo[] properties = component.GetType().GetMembers();
                Assert.False(properties.All(p => p.Name != member.Name), "Property not found!");
            }
        }
    }
}
