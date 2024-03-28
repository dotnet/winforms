// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using Moq;
using System.Windows.Forms.TestUtilities;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class MdiClientTests
{
    [WinFormsFact]
    public void MdiClient_Ctor_Default()
    {
        using MdiClient control = new();
        Assert.Null(control.AccessibleDefaultActionDescription);
        Assert.Null(control.AccessibleDescription);
        Assert.Null(control.AccessibleName);
        Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
        Assert.False(control.AllowDrop);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.False(control.AutoSize);
        Assert.Equal(SystemColors.AppWorkspace, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.Null(control.BindingContext);
        Assert.Equal(0, control.Bottom);
        Assert.Equal(Rectangle.Empty, control.Bounds);
        Assert.False(control.CanFocus);
        Assert.False(control.CanSelect);
        Assert.False(control.Capture);
        Assert.True(control.CausesValidation);
        Assert.Equal(Size.Empty, control.ClientSize);
        Assert.Equal(Rectangle.Empty, control.ClientRectangle);
        Assert.Null(control.Container);
        Assert.False(control.ContainsFocus);
        Assert.Null(control.ContextMenuStrip);
        Assert.Same(control, Assert.IsType<MdiClient.ControlCollection>(control.Controls).Owner);
        Assert.Empty(control.Controls);
        Assert.Same(control.Controls, control.Controls);
        Assert.False(control.Created);
        Assert.Same(Cursors.Default, control.Cursor);
        Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
        Assert.Equal(DockStyle.Fill, control.Dock);
        Assert.True(control.Enabled);
        Assert.False(control.Focused);
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        Assert.False(control.HasChildren);
        Assert.Equal(0, control.Height);
        Assert.Equal(ImeMode.NoControl, control.ImeMode);
        Assert.False(control.IsAccessible);
        Assert.False(control.IsMirrored);
        Assert.NotNull(control.LayoutEngine);
        Assert.Same(control.LayoutEngine, control.LayoutEngine);
        Assert.Equal(0, control.Left);
        Assert.Equal(Point.Empty, control.Location);
        Assert.Equal(new Padding(3), control.Margin);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.Empty(control.MdiChildren);
        Assert.Same(control.MdiChildren, control.MdiChildren);
        Assert.Equal(Size.Empty, control.MinimumSize);
        Assert.Equal(Padding.Empty, control.Padding);
        Assert.Null(control.Parent);
        Assert.Equal(Size.Empty, control.PreferredSize);
        Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.Equal(0, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowKeyboardCues);
        Assert.Null(control.Site);
        Assert.Equal(Size.Empty, control.Size);
        Assert.Equal(0, control.TabIndex);
        Assert.True(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(0, control.Top);
        Assert.Null(control.TopLevelControl);
        Assert.False(control.UseWaitCursor);
        Assert.True(control.Visible);
        Assert.Equal(0, control.Width);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
    public void MdiClient_BackColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using MdiClient control = new()
        {
            BackColor = value
        };
        Assert.Equal(expected, control.BackColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void MdiClient_BackColor_SetWithHandler_CallsBackColorChanged()
    {
        using MdiClient control = new();
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
    public void MdiClient_BackColor_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(MdiClient))[nameof(MdiClient.BackColor)];
        using MdiClient control = new();
        Assert.False(property.CanResetValue(control));

        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.True(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void MdiClient_BackColor_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(MdiClient))[nameof(MdiClient.BackColor)];
        using MdiClient control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.True(property.ShouldSerializeValue(control));
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void MdiClient_BackgroundImage_GetWithParent_GetReturnsExpected(Image parentBackgroundImage)
    {
        using Bitmap image = new(10, 10);
        using Control parent = new()
        {
            BackgroundImage = parentBackgroundImage
        };
        using MdiClient control = new()
        {
            Parent = parent
        };
        Assert.Same(parentBackgroundImage, control.BackgroundImage);

        // Set custom.
        control.BackgroundImage = image;
        Assert.Same(image, control.BackgroundImage);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void MdiClient_BackgroundImage_Set_GetReturnsExpected(Image value)
    {
        using MdiClient control = new()
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
    public void MdiClient_BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
    {
        using MdiClient control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
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
    public void MdiClient_BackgroundImageLayout_GetWithParent_GetReturnsExpected(ImageLayout parentBackgroundImageLayout)
    {
        using Bitmap image = new(10, 10);
        using Control parent = new()
        {
            BackgroundImageLayout = parentBackgroundImageLayout
        };
        using MdiClient control = new()
        {
            Parent = parent
        };
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);

        // Set custom.
        control.BackgroundImage = image;
        Assert.Equal(parentBackgroundImageLayout, control.BackgroundImageLayout);
    }

    [WinFormsTheory]
    [EnumData<ImageLayout>]
    public void MdiClient_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
    {
        using MdiClient control = new()
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
    public void MdiClient_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
    {
        using MdiClient control = new();
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
    [EnumData<RightToLeft>]
    public void MdiClient_Handle_Get_Success(RightToLeft rightToLeft)
    {
        using MdiClient control = new()
        {
            RightToLeft = rightToLeft
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.True(control.Enabled);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.Inherit, true)]
    [InlineData(RightToLeft.No, true)]
    [InlineData(RightToLeft.Yes, true)]
    public void MdiClient_Handle_GetDesignMode_Success(RightToLeft rightToleft, bool designMode)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(designMode);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(new AmbientProperties());
        using MdiClient control = new()
        {
            Site = mockSite.Object,
            RightToLeft = rightToleft
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.True(control.Enabled);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.Inherit, RightToLeft.Inherit)]
    [InlineData(RightToLeft.Inherit, RightToLeft.No)]
    [InlineData(RightToLeft.Inherit, RightToLeft.Yes)]
    [InlineData(RightToLeft.No, RightToLeft.Inherit)]
    [InlineData(RightToLeft.No, RightToLeft.No)]
    [InlineData(RightToLeft.No, RightToLeft.Yes)]
    [InlineData(RightToLeft.Yes, RightToLeft.Inherit)]
    [InlineData(RightToLeft.Yes, RightToLeft.No)]
    [InlineData(RightToLeft.Yes, RightToLeft.Yes)]
    public void MdiClient_Handle_GetWithParent_Success(RightToLeft parentRightToLeft, RightToLeft rightToLeft)
    {
        using Control parent = new()
        {
            RightToLeft = parentRightToLeft
        };
        using MdiClient control = new()
        {
            Parent = parent,
            RightToLeft = rightToLeft
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.True(control.Enabled);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.Inherit, RightToLeft.Inherit, true)]
    [InlineData(RightToLeft.Inherit, RightToLeft.Inherit, false)]
    [InlineData(RightToLeft.Inherit, RightToLeft.No, true)]
    [InlineData(RightToLeft.Inherit, RightToLeft.No, false)]
    [InlineData(RightToLeft.Inherit, RightToLeft.Yes, true)]
    [InlineData(RightToLeft.Inherit, RightToLeft.Yes, false)]
    [InlineData(RightToLeft.No, RightToLeft.Inherit, true)]
    [InlineData(RightToLeft.No, RightToLeft.Inherit, false)]
    [InlineData(RightToLeft.No, RightToLeft.No, true)]
    [InlineData(RightToLeft.No, RightToLeft.No, false)]
    [InlineData(RightToLeft.No, RightToLeft.Yes, true)]
    [InlineData(RightToLeft.No, RightToLeft.Yes, false)]
    [InlineData(RightToLeft.Yes, RightToLeft.Inherit, true)]
    [InlineData(RightToLeft.Yes, RightToLeft.Inherit, false)]
    [InlineData(RightToLeft.Yes, RightToLeft.No, true)]
    [InlineData(RightToLeft.Yes, RightToLeft.No, false)]
    [InlineData(RightToLeft.Yes, RightToLeft.Yes, true)]
    [InlineData(RightToLeft.Yes, RightToLeft.Yes, false)]
    public void MdiClient_Handle_GetParentDesignMode_SetsDisabled(RightToLeft parentRightToLeft, RightToLeft rightToLeft, bool designMode)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(designMode);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(new AmbientProperties());
        using Control parent = new()
        {
            Site = mockSite.Object,
            RightToLeft = parentRightToLeft
        };
        using MdiClient control = new()
        {
            Parent = parent,
            RightToLeft = rightToLeft
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(!designMode, control.Enabled);
    }

    [WinFormsFact]
    public void MdiClient_Location_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(MdiClient))[nameof(MdiClient.Location)];
        using MdiClient control = new();
        Assert.True(property.CanResetValue(control));

        control.Location = new Point(1, 0);
        Assert.Equal(new Point(1, 0), control.Location);
        Assert.True(property.CanResetValue(control));

        control.Location = new Point(0, 1);
        Assert.Equal(new Point(0, 1), control.Location);
        Assert.True(property.CanResetValue(control));

        control.Location = new Point(1, 2);
        Assert.Equal(new Point(1, 2), control.Location);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(Point.Empty, control.Location);
        Assert.True(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void MdiClient_Location_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(MdiClient))[nameof(MdiClient.Location)];
        using MdiClient control = new();
        Assert.True(property.ShouldSerializeValue(control));

        control.Location = new Point(1, 0);
        Assert.Equal(new Point(1, 0), control.Location);
        Assert.True(property.ShouldSerializeValue(control));

        control.Location = new Point(0, 1);
        Assert.Equal(new Point(0, 1), control.Location);
        Assert.True(property.ShouldSerializeValue(control));

        control.Location = new Point(1, 2);
        Assert.Equal(new Point(1, 2), control.Location);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(Point.Empty, control.Location);
        Assert.True(property.ShouldSerializeValue(control));
    }

    public static IEnumerable<object[]> Size_Set_TestData()
    {
        yield return new object[] { new Size(-3, -4), -7, -8, 1 };
        yield return new object[] { new Size(0, 0), 0, 0, 0 };
        yield return new object[] { new Size(1, 0), -3, -4, 1 };
        yield return new object[] { new Size(0, 2), -4, -2, 1 };
        yield return new object[] { new Size(30, 40), 26, 36, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Size_Set_TestData))]
    public void MdiClient_Size_Set_GetReturnsExpected(Size value, int expectedWidth, int expectedHeight, int expectedLayoutCallCount)
    {
        using MdiClient control = new();
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };

        control.Size = value;
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(value, control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(value.Width, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(value.Height, control.Bottom);
        Assert.Equal(value.Width, control.Width);
        Assert.Equal(value.Height, control.Height);
        Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Size = value;
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(value, control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(value.Width, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(value.Height, control.Bottom);
        Assert.Equal(value.Width, control.Width);
        Assert.Equal(value.Height, control.Height);
        Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Size_SetWithHandle_TestData()
    {
        yield return new object[] { new Size(0, 0), 0, 0, 0, 0, 0, 0 };
        yield return new object[] { new Size(-3, -4), 0, 0, 0, 0, 0, 0 };
        yield return new object[] { new Size(1, 0), 1, 0, 1, 0, 1, 0 };
        yield return new object[] { new Size(0, 2), 0, 2, 0, 2, 1, 0 };
        yield return new object[] { new Size(1, 2), 1, 2, 1, 2, 1, 0 };
        yield return new object[] { new Size(30, 40), 30, 40, 26, 36, 1, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Size_SetWithHandle_TestData))]
    public void MdiClient_Size_SetWithHandle_GetReturnsExpected(Size value, int expectedWidth, int expectedHeight, int expectedClientWidth, int expectedClientHeight, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
    {
        using MdiClient control = new();
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Size = value;
        Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(expectedWidth, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.Size = value;
        Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(expectedWidth, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Size_SetWithParentHandle_TestData()
    {
        yield return new object[] { new Size(0, 0), 0, 0, 0 };
        yield return new object[] { new Size(-3, -4), 0, 0, 1 };
        yield return new object[] { new Size(1, 0), 2, 0, 2 };
        yield return new object[] { new Size(0, 2), 2, 0, 2 };
        yield return new object[] { new Size(1, 2), 2, 0, 2 };
        yield return new object[] { new Size(30, 40), 2, 0, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Size_SetWithParentHandle_TestData))]
    public void MdiClient_Size_SetWithParentWithHandle_GetReturnsExpected(Size value, int expectedLayoutCallCount, int expectedInvalidatedCallCount, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using MdiClient control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        int parentLayoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            Assert.Equal(parentLayoutCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            Assert.Equal(parentLayoutCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        try
        {
            control.Size = value;
            Assert.Equal(Size.Empty, control.ClientSize);
            Assert.Equal(Rectangle.Empty, control.ClientRectangle);
            Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(Rectangle.Empty, control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Call again.
            control.Size = value;
            Assert.Equal(Size.Empty, control.ClientSize);
            Assert.Equal(Rectangle.Empty, control.ClientRectangle);
            Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(Rectangle.Empty, control.Bounds);
            Assert.Equal(expectedLayoutCallCount * 2, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount * 2, parentLayoutCallCount);
            Assert.Equal(expectedLayoutCallCount * 2, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount * 2, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount * 2, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Size_SetWithParentHandle_TestData))]
    public void MdiClient_Size_SetWithNonDesignModeParentWithHandle_GetReturnsExpected(Size value, int expectedLayoutCallCount, int expectedInvalidatedCallCount, int expectedParentLayoutCallCount)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(false);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(new AmbientProperties());
        using Control parent = new()
        {
            Site = mockSite.Object
        };
        using MdiClient control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        int parentLayoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            Assert.Equal(parentLayoutCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            Assert.Equal(parentLayoutCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        try
        {
            control.Size = value;
            Assert.Equal(Size.Empty, control.ClientSize);
            Assert.Equal(Rectangle.Empty, control.ClientRectangle);
            Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(Rectangle.Empty, control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Call again.
            control.Size = value;
            Assert.Equal(Size.Empty, control.ClientSize);
            Assert.Equal(Rectangle.Empty, control.ClientRectangle);
            Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(Rectangle.Empty, control.Bounds);
            Assert.Equal(expectedLayoutCallCount * 2, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount * 2, parentLayoutCallCount);
            Assert.Equal(expectedLayoutCallCount * 2, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount * 2, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount * 2, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> Size_SetWithDesignModeParentHandle_TestData()
    {
        yield return new object[] { new Size(0, 0), 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { new Size(-3, -4), 0, 0, 0, 0, 0, 0, 0, 1, 2 };
        yield return new object[] { new Size(1, 0), 0, 0, 0, 0, 2, 4, 0, 2, 4 };
        yield return new object[] { new Size(0, 2), 0, 0, 0, 0, 2, 4, 0, 2, 4 };
        yield return new object[] { new Size(1, 2), 0, 0, 0, 0, 2, 4, 0, 2, 4 };
        yield return new object[] { new Size(30, 40), 0, 0, 0, 0, 2, 4, 0, 2, 4 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Size_SetWithDesignModeParentHandle_TestData))]
    public void MdiClient_Size_SetWithDesignModeParentWithHandle_GetReturnsExpected(Size value, int expectedWidth, int expectedHeight, int expectedClientWidth, int expectedClientHeight, int expectedLayoutCallCount1, int expectedLayoutCallCount2, int expectedInvalidatedCallCount, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(new AmbientProperties());
        using Control parent = new()
        {
            Site = mockSite.Object
        };
        using MdiClient control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        int parentLayoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            Assert.Equal(parentLayoutCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            Assert.Equal(parentLayoutCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        try
        {
            control.Size = value;
            Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedWidth, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
            Assert.Equal(expectedLayoutCallCount1, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount1, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount1, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Call again.
            control.Size = value;
            Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedWidth, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
            Assert.Equal(expectedLayoutCallCount2, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount2, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount2, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void MdiClient_Size_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(MdiClient))[nameof(Control.Size)];
        using MdiClient control = new();
        Assert.False(property.CanResetValue(control));

        control.Size = new Size(1, 0);
        Assert.Equal(new Size(1, 0), control.Size);
        Assert.False(property.CanResetValue(control));

        control.Size = new Size(0, 1);
        Assert.Equal(new Size(0, 1), control.Size);
        Assert.False(property.CanResetValue(control));

        control.Size = new Size(1, 2);
        Assert.Equal(new Size(1, 2), control.Size);
        Assert.False(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(Size.Empty, control.Size);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void MdiClient_Size_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(MdiClient))[nameof(Control.Size)];
        using MdiClient control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.Size = new Size(1, 0);
        Assert.Equal(new Size(1, 0), control.Size);
        Assert.False(property.ShouldSerializeValue(control));

        control.Size = new Size(0, 1);
        Assert.Equal(new Size(0, 1), control.Size);
        Assert.False(property.ShouldSerializeValue(control));

        control.Size = new Size(1, 2);
        Assert.Equal(new Size(1, 2), control.Size);
        Assert.False(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(Size.Empty, control.Size);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsTheory]
    [EnumData<MdiLayout>]
    [InvalidEnumData<MdiLayout>]
    public void MdiClient_LayoutMdi_InvokeWithoutHandle_Nop(MdiLayout value)
    {
        using MdiClient control = new();
        control.LayoutMdi(value);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<MdiLayout>]
    [InvalidEnumData<MdiLayout>]
    public void MdiClient_LayoutMdi_InvokeWithHandle_Success(MdiLayout value)
    {
        using MdiClient control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.LayoutMdi(value);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }
}
