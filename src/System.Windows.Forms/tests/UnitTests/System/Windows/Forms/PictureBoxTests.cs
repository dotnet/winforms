// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Moq;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class PictureBoxTests
{
    private const string PathImageLocation = "bitmaps/nature24bits.jpg";
    private const string UrlImageLocation = "https://github.com/dotnet/winforms/blob/main/src/System.Windows.Forms/tests/UnitTests/bitmaps/nature24bits.jpg?raw=true";

    [WinFormsFact]
    public void PictureBox_Ctor_Default()
    {
        using SubPictureBox control = new();
        Assert.Null(control.AccessibleDefaultActionDescription);
        Assert.Null(control.AccessibleDescription);
        Assert.Null(control.AccessibleName);
        Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
        Assert.False(control.AllowDrop);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.False(control.AutoSize);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.Null(control.BindingContext);
        Assert.Equal(BorderStyle.None, control.BorderStyle);
        Assert.Equal(50, control.Bottom);
        Assert.Equal(new Rectangle(0, 0, 100, 50), control.Bounds);
        Assert.False(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.True(control.CanRaiseEvents);
        Assert.False(control.CanSelect);
        Assert.False(control.Capture);
        Assert.True(control.CausesValidation);
        Assert.Equal(new Size(100, 50), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 100, 50), control.ClientRectangle);
        Assert.Null(control.Container);
        Assert.False(control.ContainsFocus);
        Assert.Null(control.ContextMenuStrip);
        Assert.Empty(control.Controls);
        Assert.Same(control.Controls, control.Controls);
        Assert.False(control.IsHandleCreated);
        Assert.Same(Cursors.Default, control.Cursor);
        Assert.Same(Cursors.Default, control.DefaultCursor);
        Assert.Equal(ImeMode.Disable, control.DefaultImeMode);
        Assert.Equal(new Padding(3), control.DefaultMargin);
        Assert.Equal(Size.Empty, control.DefaultMaximumSize);
        Assert.Equal(Size.Empty, control.DefaultMinimumSize);
        Assert.Equal(Padding.Empty, control.DefaultPadding);
        Assert.Equal(new Size(100, 50), control.DefaultSize);
        Assert.False(control.DesignMode);
        Assert.Equal(new Rectangle(0, 0, 100, 50), control.DisplayRectangle);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.True(control.DoubleBuffered);
        Assert.True(control.Enabled);
        Assert.NotNull(control.ErrorImage);
        Assert.Same(control.ErrorImage, control.ErrorImage);
        Assert.NotNull(control.Events);
        Assert.Same(control.Events, control.Events);
        Assert.False(control.Focused);
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(control.FontHeight, control.FontHeight);
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        Assert.False(control.HasChildren);
        Assert.Null(control.Image);
        Assert.Null(control.ImageLocation);
        Assert.Equal(ImeMode.Disable, control.ImeMode);
        Assert.Equal(ImeMode.Disable, control.ImeModeBase);
        Assert.NotNull(control.InitialImage);
        Assert.Same(control.InitialImage, control.InitialImage);
        Assert.False(control.IsAccessible);
        Assert.False(control.IsMirrored);
        Assert.NotNull(control.LayoutEngine);
        Assert.Same(control.LayoutEngine, control.LayoutEngine);
        Assert.Equal(0, control.Left);
        Assert.Equal(Point.Empty, control.Location);
        Assert.Equal(new Padding(3), control.Margin);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.Equal(Size.Empty, control.MinimumSize);
        Assert.Equal(Padding.Empty, control.Padding);
        Assert.Null(control.Parent);
        Assert.Equal(new Size(100, 50), control.PreferredSize);
        Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.False(control.ResizeRedraw);
        Assert.Equal(100, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowKeyboardCues);
        Assert.Null(control.Site);
        Assert.Equal(new Size(100, 50), control.Size);
        Assert.Equal(PictureBoxSizeMode.Normal, control.SizeMode);
        Assert.Equal(0, control.TabIndex);
        Assert.False(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(0, control.Top);
        Assert.Null(control.TopLevelControl);
        Assert.False(control.UseWaitCursor);
        Assert.True(control.Visible);
        Assert.False(control.WaitOnLoad);
        Assert.Equal(100, control.Width);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(BorderStyle.None, 0x56000000, 0)]
    [InlineData(BorderStyle.FixedSingle, 0x56800000, 0)]
    [InlineData(BorderStyle.Fixed3D, 0x56000000, 0x200)]
    public void PictureBox_CreateParams_Get_ReturnsExpected(BorderStyle borderStyle, int expectedStyle, int expectedExStyle)
    {
        using SubPictureBox control = new()
        {
            BorderStyle = borderStyle
        };
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(expectedExStyle, createParams.ExStyle);
        Assert.Equal(50, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(100, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
    }

    [WinFormsTheory]
    [EnumData<BorderStyle>]
    public void PictureBox_BorderStyle_Set_GetReturnsExpected(BorderStyle value)
    {
        using PictureBox pictureBox = new()
        {
            BorderStyle = value
        };
        Assert.Equal(value, pictureBox.BorderStyle);

        // Set same.
        pictureBox.BorderStyle = value;
        Assert.Equal(value, pictureBox.BorderStyle);
    }

    [WinFormsTheory]
    [EnumData<BorderStyle>]
    public void PictureBox_BorderStyle_SetWithHandle_GetReturnsExpected(BorderStyle value)
    {
        using PictureBox pictureBox = new();
        Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

        pictureBox.BorderStyle = value;
        Assert.Equal(value, pictureBox.BorderStyle);

        // Set same.
        pictureBox.BorderStyle = value;
        Assert.Equal(value, pictureBox.BorderStyle);
    }

    [WinFormsTheory]
    [InvalidEnumData<BorderStyle>]
    public void PictureBox_BorderStyle_SetInvalid_ThrowsInvalidEnumArgumentException(BorderStyle value)
    {
        using PictureBox pictureBox = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => pictureBox.BorderStyle = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void PictureBox_CausesValidation_Set_GetReturnsExpected(bool value)
    {
        using PictureBox control = new()
        {
            CausesValidation = value
        };
        Assert.Equal(value, control.CausesValidation);

        // Set same
        control.CausesValidation = value;
        Assert.Equal(value, control.CausesValidation);

        // Set different
        control.CausesValidation = !value;
        Assert.Equal(!value, control.CausesValidation);
    }

    [WinFormsFact]
    public void PictureBox_CausesValidation_SetWithHandler_CallsCausesValidationChanged()
    {
        using PictureBox control = new()
        {
            CausesValidation = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.CausesValidationChanged += handler;

        // Set different.
        control.CausesValidation = false;
        Assert.False(control.CausesValidation);
        Assert.Equal(1, callCount);

        // Set same.
        control.CausesValidation = false;
        Assert.False(control.CausesValidation);
        Assert.Equal(1, callCount);

        // Set different.
        control.CausesValidation = true;
        Assert.True(control.CausesValidation);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.CausesValidationChanged -= handler;
        control.CausesValidation = false;
        Assert.False(control.CausesValidation);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void PictureBox_Enabled_Set_GetReturnsExpected(bool value)
    {
        using Control control = new()
        {
            Enabled = value
        };
        Assert.Equal(value, control.Enabled);

        // Set same.
        control.Enabled = value;
        Assert.Equal(value, control.Enabled);

        // Set different.
        control.Enabled = value;
        Assert.Equal(value, control.Enabled);
    }

    [WinFormsTheory]
    [BoolData]
    public void PictureBox_Enabled_SetWithHandle_GetReturnsExpected(bool value)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.Enabled = value;
        Assert.Equal(value, control.Enabled);

        // Set same.
        control.Enabled = value;
        Assert.Equal(value, control.Enabled);

        // Set different.
        control.Enabled = value;
        Assert.Equal(value, control.Enabled);
    }

    [WinFormsFact]
    public void PictureBox_Enabled_SetWithHandler_CallsEnabledChanged()
    {
        using Control control = new()
        {
            Enabled = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.EnabledChanged += handler;

        // Set different.
        control.Enabled = false;
        Assert.False(control.Enabled);
        Assert.Equal(1, callCount);

        // Set same.
        control.Enabled = false;
        Assert.False(control.Enabled);
        Assert.Equal(1, callCount);

        // Set different.
        control.Enabled = true;
        Assert.True(control.Enabled);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.EnabledChanged -= handler;
        control.Enabled = false;
        Assert.False(control.Enabled);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void PictureBox_ErrorImage_Set_GetReturnsExpected(Image value)
    {
        using PictureBox pictureBox = new()
        {
            ErrorImage = value
        };
        Assert.Same(value, pictureBox.ErrorImage);
        Assert.Null(pictureBox.Image);

        // Set same.
        pictureBox.ErrorImage = value;
        Assert.Same(value, pictureBox.ErrorImage);
        Assert.Null(pictureBox.Image);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void PictureBox_ErrorImage_SetWithNonNullOldValue_GetReturnsExpected(Image value)
    {
        using PictureBox pictureBox = new()
        {
            ErrorImage = new Bitmap(10, 10)
        };

        pictureBox.ErrorImage = value;
        Assert.Same(value, pictureBox.ErrorImage);
        Assert.Null(pictureBox.Image);

        // Set same.
        pictureBox.ErrorImage = value;
        Assert.Same(value, pictureBox.ErrorImage);
        Assert.Null(pictureBox.Image);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void PictureBox_ErrorImage_SetWithHandle_GetReturnsExpected(Image value)
    {
        using PictureBox pictureBox = new();
        Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

        pictureBox.ErrorImage = value;
        Assert.Same(value, pictureBox.ErrorImage);
        Assert.Null(pictureBox.Image);

        // Set same.
        pictureBox.ErrorImage = value;
        Assert.Same(value, pictureBox.ErrorImage);
        Assert.Null(pictureBox.Image);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void PictureBox_ErrorImage_SetWithNonNullOldValueWithHandle_GetReturnsExpected(Image value)
    {
        using PictureBox pictureBox = new()
        {
            ErrorImage = new Bitmap(10, 10)
        };
        Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

        pictureBox.ErrorImage = value;
        Assert.Same(value, pictureBox.ErrorImage);
        Assert.Null(pictureBox.Image);

        // Set same.
        pictureBox.ErrorImage = value;
        Assert.Same(value, pictureBox.ErrorImage);
        Assert.Null(pictureBox.Image);
    }

    [WinFormsFact]
    public void PictureBox_ErrorImage_ResetValue_Success()
    {
        using Bitmap image = new(10, 10);
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PictureBox))[nameof(PictureBox.ErrorImage)];
        using PictureBox pictureBox = new();
        Assert.False(property.CanResetValue(pictureBox));

        pictureBox.ErrorImage = image;
        Assert.True(property.CanResetValue(pictureBox));

        property.ResetValue(pictureBox);
        Assert.NotSame(image, pictureBox.ErrorImage);
        Assert.NotNull(image);
        Assert.False(property.CanResetValue(pictureBox));
    }

    [WinFormsFact]
    public void PictureBox_ErrorImage_ShouldSerializeValue_Success()
    {
        using Bitmap image = new(10, 10);
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PictureBox))[nameof(PictureBox.ErrorImage)];
        using PictureBox pictureBox = new();
        Assert.False(property.ShouldSerializeValue(pictureBox));

        pictureBox.ErrorImage = image;
        Assert.True(property.ShouldSerializeValue(pictureBox));

        property.ResetValue(pictureBox);
        Assert.NotSame(image, pictureBox.ErrorImage);
        Assert.NotNull(image);
        Assert.False(property.ShouldSerializeValue(pictureBox));
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetForeColorTheoryData))]
    public void PictureBox_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using PictureBox control = new()
        {
            ForeColor = value
        };
        Assert.Equal(expected, control.ForeColor);

        // Set same.
        control.ForeColor = value;
        Assert.Equal(expected, control.ForeColor);
    }

    [WinFormsFact]
    public void PictureBox_ForeColor_SetWithHandler_CallsForeColorChanged()
    {
        using PictureBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
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

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void PictureBox_Font_Set_GetReturnsExpected(Font value)
    {
        using SubPictureBox control = new()
        {
            Font = value
        };
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void PictureBox_Font_SetWithHandler_CallsFontChanged()
    {
        using PictureBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.FontChanged += handler;

        // Set different.
        using Font font1 = new("Arial", 8.25f);
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Equal(1, callCount);

        // Set same.
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Equal(1, callCount);

        // Set different.
        using var font2 = SystemFonts.DialogFont;
        control.Font = font2;
        Assert.Same(font2, control.Font);
        Assert.Equal(2, callCount);

        // Set null.
        control.Font = null;
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.FontChanged -= handler;
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Equal(3, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void PictureBox_Image_Set_GetReturnsExpected(Image value)
    {
        using PictureBox pictureBox = new()
        {
            Image = value
        };
        Assert.Same(value, pictureBox.Image);

        // Set same.
        pictureBox.Image = value;
        Assert.Same(value, pictureBox.Image);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void PictureBox_Image_SetWithNonNullOldValue_GetReturnsExpected(Image value)
    {
        using PictureBox pictureBox = new()
        {
            Image = new Bitmap(10, 10)
        };

        pictureBox.Image = value;
        Assert.Same(value, pictureBox.Image);

        // Set same.
        pictureBox.Image = value;
        Assert.Same(value, pictureBox.Image);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void PictureBox_Image_SetWithHandle_GetReturnsExpected(Image value)
    {
        using PictureBox pictureBox = new();
        Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

        pictureBox.Image = value;
        Assert.Same(value, pictureBox.Image);

        // Set same.
        pictureBox.Image = value;
        Assert.Same(value, pictureBox.Image);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void PictureBox_Image_SetWithNonNullOldValueWithHandle_GetReturnsExpected(Image value)
    {
        using PictureBox pictureBox = new()
        {
            Image = new Bitmap(10, 10)
        };
        Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

        pictureBox.Image = value;
        Assert.Same(value, pictureBox.Image);

        // Set same.
        pictureBox.Image = value;
        Assert.Same(value, pictureBox.Image);
    }

    [WinFormsFact]
    public void PictureBox_Image_ResetValue_Success()
    {
        using Bitmap image = new(10, 10);
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PictureBox))[nameof(PictureBox.Image)];
        using PictureBox pictureBox = new();
        Assert.False(property.CanResetValue(pictureBox));

        pictureBox.Image = image;
        Assert.True(property.CanResetValue(pictureBox));

        property.ResetValue(pictureBox);
        Assert.NotSame(image, pictureBox.Image);
        Assert.NotNull(image);
        Assert.False(property.CanResetValue(pictureBox));
    }

    [WinFormsFact]
    public void PictureBox_Image_ShouldSerializeValue_Success()
    {
        using Bitmap image = new(10, 10);
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PictureBox))[nameof(PictureBox.Image)];
        using PictureBox pictureBox = new();
        Assert.False(property.ShouldSerializeValue(pictureBox));

        pictureBox.Image = image;
        Assert.True(property.ShouldSerializeValue(pictureBox));

        property.ResetValue(pictureBox);
        Assert.NotSame(image, pictureBox.Image);
        Assert.NotNull(image);
        Assert.False(property.ShouldSerializeValue(pictureBox));
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void PictureBox_ImageLocation_Set_GetReturnsExpected(string value)
    {
        using PictureBox pictureBox = new()
        {
            ImageLocation = value
        };
        Assert.Equal(value, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);

        // Set same.
        pictureBox.ImageLocation = value;
        Assert.Equal(value, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);
    }

    [WinFormsTheory]
    [StringWithNullData]
    [InlineData("bitmaps/nature24bits.jpg")]
    [InlineData("  ")]
    public void PictureBox_ImageLocation_SetWithImage_GetReturnsExpected(string value)
    {
        using Bitmap image = new(10, 10);
        using PictureBox pictureBox = new()
        {
            Image = image,
            ImageLocation = value
        };
        Assert.Equal(value, pictureBox.ImageLocation);
        Assert.Same(image, pictureBox.Image);

        // Set same.
        pictureBox.ImageLocation = value;
        Assert.Equal(value, pictureBox.ImageLocation);
        Assert.Same(image, pictureBox.Image);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void PictureBox_ImageLocation_SetWithHandle_GetReturnsExpected(string value)
    {
        using PictureBox pictureBox = new();
        Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

        pictureBox.ImageLocation = value;
        Assert.Equal(value, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);

        // Set same.
        pictureBox.ImageLocation = value;
        Assert.Equal(value, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);
    }

    [WinFormsFact]
    public void PictureBox_ImageLocation_Set_CallsInvalidated()
    {
        using PictureBox pictureBox = new()
        {
            ImageLocation = null
        };
        int invalidatedCallCount = 0;
        pictureBox.Invalidated += (sender, e) => invalidatedCallCount++;
        Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

        // Set different.
        pictureBox.ImageLocation = "Location";
        Assert.Equal("Location", pictureBox.ImageLocation);
        Assert.Equal(1, invalidatedCallCount);

        // Set same.
        pictureBox.ImageLocation = "Location";
        Assert.Equal("Location", pictureBox.ImageLocation);
        Assert.Equal(2, invalidatedCallCount);

        // Set null.
        pictureBox.ImageLocation = null;
        Assert.Null(pictureBox.ImageLocation);
        Assert.Equal(3, invalidatedCallCount);

        // Set empty.
        pictureBox.ImageLocation = string.Empty;
        Assert.Same(string.Empty, pictureBox.ImageLocation);
        Assert.Equal(4, invalidatedCallCount);
    }

    [WinFormsFact]
    public void PictureBox_ImageLocation_SetValidWithWaitOnLoadTrueLocal_GetReturnsExpected()
    {
        using PictureBox pictureBox = new()
        {
            WaitOnLoad = true
        };

        Size expectedImageSize = new(110, 100);

        pictureBox.ImageLocation = PathImageLocation;
        Assert.Equal(expectedImageSize, pictureBox.Image.Size);
        Assert.Equal(PathImageLocation, pictureBox.ImageLocation);

        // Set same.
        pictureBox.ImageLocation = PathImageLocation;
        Assert.Equal(expectedImageSize, pictureBox.Image.Size);
        Assert.Equal(PathImageLocation, pictureBox.ImageLocation);
    }

    [WinFormsTheory]
    [BoolData]
    public void PictureBox_ImageLocation_SetValidWithWaitOnLoadTrueUri_ConfigSwitch_CheckCRL_GetReturnsExpected(bool switchValue)
    {
        using ServicePointManagerCheckCrlScope scope = new(switchValue);

        try
        {
            using PictureBox pictureBox = new()
            {
                WaitOnLoad = true
            };

            Size expectedImageSize = new(110, 100);

            pictureBox.ImageLocation = UrlImageLocation;
            Assert.Equal(expectedImageSize, pictureBox.Image.Size);
            Assert.Same(UrlImageLocation, pictureBox.ImageLocation);

            // Set same.
            pictureBox.ImageLocation = UrlImageLocation;
            Assert.Equal(expectedImageSize, pictureBox.Image.Size);
            Assert.Same(UrlImageLocation, pictureBox.ImageLocation);
        }
        catch
        {
            // Swallow network errors.
        }
    }

    [WinFormsTheory]
    [NullAndEmptyStringData]
    public void PictureBox_ImageLocation_SetNullOrEmptyWithWaitOnLoadTrue_GetReturnsExpected(string value)
    {
        using PictureBox pictureBox = new()
        {
            WaitOnLoad = true
        };

        pictureBox.ImageLocation = value;
        Assert.Null(pictureBox.Image);
        Assert.Equal(value, pictureBox.ImageLocation);

        // Set same.
        pictureBox.ImageLocation = value;
        Assert.Null(pictureBox.Image);
        Assert.Equal(value, pictureBox.ImageLocation);
    }

    [WinFormsTheory]
    [InlineData("NoSuchValue")]
    [InlineData("  ")]
    public void PictureBox_ImageLocation_SetInvalidWithWaitOnLoadTrue_ThrowsFileNotFoundException(string value)
    {
        using PictureBox pictureBox = new()
        {
            WaitOnLoad = true
        };

        Assert.ThrowsAny<Exception>(() => pictureBox.ImageLocation = value);
        Assert.Null(pictureBox.Image);
        Assert.Equal(value, pictureBox.ImageLocation);

        // Set same.
        Assert.ThrowsAny<Exception>(() => pictureBox.ImageLocation = value);
        Assert.Null(pictureBox.Image);
        Assert.Equal(value, pictureBox.ImageLocation);
    }

    public static IEnumerable<object[]> ImageLocation_SetInvalidWithWaitOnLoadTrueDesignMode_TestData()
    {
        foreach (string value in new string[] { " ", "NoSuchImage" })
        {
            yield return new object[] { null, null, value };
            yield return new object[] { new Bitmap(10, 10), new Bitmap(10, 10), value };
            yield return new object[] { new Bitmap(10, 10), null, value };
            yield return new object[] { null, new Bitmap(10, 10), value };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageLocation_SetInvalidWithWaitOnLoadTrueDesignMode_TestData))]
    public void PictureBox_ImageLocation_SetInvalidWithWaitOnLoadTrueDesignMode_GetReturnsExpected(Image initialImage, Image errorImage, string value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(new AmbientProperties());
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        using PictureBox pictureBox = new()
        {
            InitialImage = initialImage,
            ErrorImage = errorImage,
            WaitOnLoad = true,
            Site = mockSite.Object
        };

        pictureBox.ImageLocation = value;
        Assert.Same(pictureBox.ErrorImage, pictureBox.Image);
        Assert.Equal(value, pictureBox.ImageLocation);

        // Set same.
        pictureBox.ImageLocation = value;
        Assert.Same(pictureBox.ErrorImage, pictureBox.Image);
        Assert.Equal(value, pictureBox.ImageLocation);

        // NB: disposing the component with strictly mocked object causes tests to fail
        // Moq.MockException : ISite.Container invocation failed with mock behavior Strict.
        // All invocations on the mock must have a corresponding setup.
        pictureBox.Site = null;
    }

    [WinFormsTheory]
    [NullAndEmptyStringData]
    public void PictureBox_ImageLocation_SetNullOrEmptyWithError_ResetsImage(string value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(new AmbientProperties());
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        using PictureBox pictureBox = new()
        {
            WaitOnLoad = true,
            Site = mockSite.Object
        };
        pictureBox.ImageLocation = "Invalid";
        Assert.Equal("Invalid", pictureBox.ImageLocation);
        Assert.Same(pictureBox.ErrorImage, pictureBox.Image);

        pictureBox.ImageLocation = value;
        Assert.Equal(value, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);

        // NB: disposing the component with strictly mocked object causes tests to fail
        // Moq.MockException : ISite.Container invocation failed with mock behavior Strict.
        // All invocations on the mock must have a corresponding setup.
        pictureBox.Site = null;
    }

    [WinFormsTheory]
    [StringWithNullData]
    [InlineData("  ")]
    [InlineData("bitmaps/nature24bits.jpg")]
    public void PictureBox_ImageLocation_SetInitializing_GetReturnsExpected(string value)
    {
        using PictureBox pictureBox = new()
        {
            WaitOnLoad = true
        };
        ((ISupportInitialize)pictureBox).BeginInit();

        pictureBox.ImageLocation = value;
        Assert.Null(pictureBox.Image);
        Assert.Equal(value, pictureBox.ImageLocation);

        // Set same.
        pictureBox.ImageLocation = value;
        Assert.Null(pictureBox.Image);
        Assert.Equal(value, pictureBox.ImageLocation);
    }

    public static IEnumerable<object[]> ImeMode_Set_TestData()
    {
        yield return new object[] { ImeMode.Inherit, ImeMode.NoControl };
        yield return new object[] { ImeMode.NoControl, ImeMode.NoControl };
        yield return new object[] { ImeMode.On, ImeMode.On };
        yield return new object[] { ImeMode.Off, ImeMode.Off };
        yield return new object[] { ImeMode.Disable, ImeMode.Disable };
        yield return new object[] { ImeMode.Hiragana, ImeMode.Hiragana };
        yield return new object[] { ImeMode.Katakana, ImeMode.Katakana };
        yield return new object[] { ImeMode.KatakanaHalf, ImeMode.KatakanaHalf };
        yield return new object[] { ImeMode.AlphaFull, ImeMode.AlphaFull };
        yield return new object[] { ImeMode.Alpha, ImeMode.Alpha };
        yield return new object[] { ImeMode.HangulFull, ImeMode.HangulFull };
        yield return new object[] { ImeMode.Hangul, ImeMode.Hangul };
        yield return new object[] { ImeMode.Close, ImeMode.Close };
        yield return new object[] { ImeMode.OnHalf, ImeMode.On };
    }

    [WinFormsTheory]
    [MemberData(nameof(ImeMode_Set_TestData))]
    public void PictureBox_ImeMode_Set_GetReturnsExpected(ImeMode value, ImeMode expected)
    {
        using PictureBox control = new()
        {
            ImeMode = value
        };
        Assert.Equal(expected, control.ImeMode);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImeMode = value;
        Assert.Equal(expected, control.ImeMode);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ImeMode_Set_TestData))]
    public void PictureBox_ImeMode_SetWithHandle_GetReturnsExpected(ImeMode value, ImeMode expected)
    {
        using PictureBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ImeMode = value;
        Assert.Equal(expected, control.ImeMode);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ImeMode = value;
        Assert.Equal(expected, control.ImeMode);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void PictureBox_ImeMode_SetWithHandler_DoesNotCallImeModeChanged()
    {
        using PictureBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ImeModeChanged += handler;

        // Set different.
        control.ImeMode = ImeMode.On;
        Assert.Equal(ImeMode.On, control.ImeMode);
        Assert.Equal(0, callCount);

        // Set same.
        control.ImeMode = ImeMode.On;
        Assert.Equal(ImeMode.On, control.ImeMode);
        Assert.Equal(0, callCount);

        // Set different.
        control.ImeMode = ImeMode.Off;
        Assert.Equal(ImeMode.Off, control.ImeMode);
        Assert.Equal(0, callCount);

        // Remove handler.
        control.ImeModeChanged -= handler;
        control.ImeMode = ImeMode.Off;
        Assert.Equal(ImeMode.Off, control.ImeMode);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<ImeMode>]
    public void PictureBox_ImeMode_SetInvalid_ThrowsInvalidEnumArgumentException(ImeMode value)
    {
        using PictureBox control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.ImeMode = value);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void PictureBox_InitialImage_Set_GetReturnsExpected(Image value)
    {
        using PictureBox pictureBox = new()
        {
            InitialImage = value
        };
        Assert.Same(value, pictureBox.InitialImage);
        Assert.Null(pictureBox.Image);

        // Set same.
        pictureBox.InitialImage = value;
        Assert.Same(value, pictureBox.InitialImage);
        Assert.Null(pictureBox.Image);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void PictureBox_InitialImage_SetWithNonNullOldValue_GetReturnsExpected(Image value)
    {
        using PictureBox pictureBox = new()
        {
            InitialImage = new Bitmap(10, 10)
        };

        pictureBox.InitialImage = value;
        Assert.Same(value, pictureBox.InitialImage);
        Assert.Null(pictureBox.Image);

        // Set same.
        pictureBox.InitialImage = value;
        Assert.Same(value, pictureBox.InitialImage);
        Assert.Null(pictureBox.Image);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void PictureBox_InitialImage_SetWithHandle_GetReturnsExpected(Image value)
    {
        using PictureBox pictureBox = new();
        Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

        pictureBox.InitialImage = value;
        Assert.Same(value, pictureBox.InitialImage);
        Assert.Null(pictureBox.Image);

        // Set same.
        pictureBox.InitialImage = value;
        Assert.Same(value, pictureBox.InitialImage);
        Assert.Null(pictureBox.Image);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void PictureBox_InitialImage_SetWithNonNullOldValueWithHandle_GetReturnsExpected(Image value)
    {
        using PictureBox pictureBox = new()
        {
            InitialImage = new Bitmap(10, 10)
        };
        Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

        pictureBox.InitialImage = value;
        Assert.Same(value, pictureBox.InitialImage);
        Assert.Null(pictureBox.Image);

        // Set same.
        pictureBox.InitialImage = value;
        Assert.Same(value, pictureBox.InitialImage);
        Assert.Null(pictureBox.Image);
    }

    [WinFormsFact]
    public void PictureBox_InitialImage_ResetValue_Success()
    {
        using Bitmap image = new(10, 10);
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PictureBox))[nameof(PictureBox.InitialImage)];
        using PictureBox pictureBox = new();
        Assert.False(property.CanResetValue(pictureBox));

        pictureBox.InitialImage = image;
        Assert.True(property.CanResetValue(pictureBox));

        property.ResetValue(pictureBox);
        Assert.NotSame(image, pictureBox.InitialImage);
        Assert.NotNull(image);
        Assert.False(property.CanResetValue(pictureBox));
    }

    [WinFormsFact]
    public void PictureBox_InitialImage_ShouldSerializeValue_Success()
    {
        using Bitmap image = new(10, 10);
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PictureBox))[nameof(PictureBox.InitialImage)];
        using PictureBox pictureBox = new();
        Assert.False(property.ShouldSerializeValue(pictureBox));

        pictureBox.InitialImage = image;
        Assert.True(property.ShouldSerializeValue(pictureBox));

        property.ResetValue(pictureBox);
        Assert.NotSame(image, pictureBox.InitialImage);
        Assert.NotNull(image);
        Assert.False(property.ShouldSerializeValue(pictureBox));
    }

    public static IEnumerable<object[]> Parent_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new Control() };
        yield return new object[] { new Form() };
    }

    [WinFormsTheory]
    [MemberData(nameof(Parent_Set_TestData))]
    public void PictureBox_Parent_Set_GetReturnsExpected(Control value)
    {
        using PictureBox control = new()
        {
            Parent = value
        };
        Assert.Same(value, control.Parent);

        // Set same.
        control.Parent = value;
        Assert.Same(value, control.Parent);
    }

    [WinFormsTheory]
    [MemberData(nameof(Parent_Set_TestData))]
    public void PictureBox_Parent_SetWithNonNullOldParent_GetReturnsExpected(Control value)
    {
        using Control oldParent = new();
        using PictureBox control = new()
        {
            Parent = oldParent
        };

        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.Empty(oldParent.Controls);

        // Set same.
        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.Empty(oldParent.Controls);
    }

    [WinFormsFact]
    public void PictureBox_Parent_SetNonNull_AddsToControls()
    {
        using Control parent = new();
        using PictureBox control = new()
        {
            Parent = parent
        };
        Assert.Same(parent, control.Parent);
        Assert.Same(control, Assert.Single(parent.Controls));

        // Set same.
        control.Parent = parent;
        Assert.Same(parent, control.Parent);
        Assert.Same(control, Assert.Single(parent.Controls));
    }

    [WinFormsFact]
    public void PictureBox_Parent_SetWithHandler_CallsParentChanged()
    {
        using Control parent = new();
        using PictureBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ParentChanged += handler;

        // Set different.
        control.Parent = parent;
        Assert.Same(parent, control.Parent);
        Assert.Equal(1, callCount);

        // Set same.
        control.Parent = parent;
        Assert.Same(parent, control.Parent);
        Assert.Equal(1, callCount);

        // Set null.
        control.Parent = null;
        Assert.Null(control.Parent);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ParentChanged -= handler;
        control.Parent = parent;
        Assert.Same(parent, control.Parent);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void PictureBox_Parent_SetSame_ThrowsArgumentException()
    {
        using PictureBox control = new();
        Assert.Throws<ArgumentException>(() => control.Parent = control);
        Assert.Null(control.Parent);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetRightToLeftTheoryData))]
    public void PictureBox_RightToLeft_Set_GetReturnsExpected(RightToLeft value, RightToLeft expected)
    {
        using PictureBox control = new()
        {
            RightToLeft = value
        };
        Assert.Equal(expected, control.RightToLeft);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.RightToLeft = value;
        Assert.Equal(expected, control.RightToLeft);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void PictureBox_RightToLeft_SetWithHandler_CallsRightToLeftChanged()
    {
        using PictureBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.RightToLeftChanged += handler;

        // Set different.
        control.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
        Assert.Equal(1, callCount);

        // Set same.
        control.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
        Assert.Equal(1, callCount);

        // Set different.
        control.RightToLeft = RightToLeft.Inherit;
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.RightToLeftChanged -= handler;
        control.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<RightToLeft>]
    public void PictureBox_RightToLeft_SetInvalid_ThrowsInvalidEnumArgumentException(RightToLeft value)
    {
        using PictureBox control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.RightToLeft = value);
    }

    public static IEnumerable<object[]> SizeMode_Set_TestData()
    {
        yield return new object[] { PictureBoxSizeMode.Normal, false };
        yield return new object[] { PictureBoxSizeMode.StretchImage, false };
        yield return new object[] { PictureBoxSizeMode.AutoSize, true };
        yield return new object[] { PictureBoxSizeMode.CenterImage, false };
        yield return new object[] { PictureBoxSizeMode.Zoom, false };
    }

    [WinFormsTheory]
    [MemberData(nameof(SizeMode_Set_TestData))]
    public void PictureBox_SizeMode_Set_GetReturnsExpected(PictureBoxSizeMode value, bool expectedAutoSize)
    {
        using SubPictureBox pictureBox = new()
        {
            SizeMode = value
        };
        Assert.Equal(value, pictureBox.SizeMode);
        Assert.Equal(expectedAutoSize, pictureBox.AutoSize);
        Assert.Equal(expectedAutoSize, pictureBox.GetStyle(ControlStyles.FixedHeight | ControlStyles.FixedWidth));

        // Set same.
        pictureBox.SizeMode = value;
        Assert.Equal(value, pictureBox.SizeMode);
        Assert.Equal(expectedAutoSize, pictureBox.AutoSize);
        Assert.Equal(expectedAutoSize, pictureBox.GetStyle(ControlStyles.FixedHeight | ControlStyles.FixedWidth));
    }

    [WinFormsTheory]
    [MemberData(nameof(SizeMode_Set_TestData))]
    public void PictureBox_SizeMode_SetWithHandle_GetReturnsExpected(PictureBoxSizeMode value, bool expectedAutoSize)
    {
        using SubPictureBox pictureBox = new();
        Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

        pictureBox.SizeMode = value;
        Assert.Equal(value, pictureBox.SizeMode);
        Assert.Equal(expectedAutoSize, pictureBox.AutoSize);
        Assert.Equal(expectedAutoSize, pictureBox.GetStyle(ControlStyles.FixedHeight | ControlStyles.FixedWidth));

        // Set same.
        pictureBox.SizeMode = value;
        Assert.Equal(value, pictureBox.SizeMode);
        Assert.Equal(expectedAutoSize, pictureBox.AutoSize);
        Assert.Equal(expectedAutoSize, pictureBox.GetStyle(ControlStyles.FixedHeight | ControlStyles.FixedWidth));
    }

    [WinFormsFact]
    public void PictureBox_SizeMode_Set_CallsInvalidated()
    {
        using PictureBox pictureBox = new()
        {
            SizeMode = PictureBoxSizeMode.Normal
        };
        int invalidatedCallCount = 0;
        pictureBox.Invalidated += (sender, e) => invalidatedCallCount++;
        Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

        // Set different.
        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        Assert.Equal(PictureBoxSizeMode.StretchImage, pictureBox.SizeMode);
        Assert.Equal(1, invalidatedCallCount);

        // Set same.
        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        Assert.Equal(PictureBoxSizeMode.StretchImage, pictureBox.SizeMode);
        Assert.Equal(1, invalidatedCallCount);
    }

    [WinFormsFact]
    public void PictureBox_SizeMode_SetWithHandler_CallsSizeModeChanged()
    {
        using PictureBox pictureBox = new()
        {
            SizeMode = PictureBoxSizeMode.Normal
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(pictureBox, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        pictureBox.SizeModeChanged += handler;

        // Set different.
        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        Assert.Equal(PictureBoxSizeMode.StretchImage, pictureBox.SizeMode);
        Assert.Equal(1, callCount);

        // Set same.
        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        Assert.Equal(PictureBoxSizeMode.StretchImage, pictureBox.SizeMode);
        Assert.Equal(1, callCount);

        // Set different.
        pictureBox.SizeMode = PictureBoxSizeMode.Normal;
        Assert.Equal(PictureBoxSizeMode.Normal, pictureBox.SizeMode);
        Assert.Equal(2, callCount);

        // Remove handler.
        pictureBox.SizeModeChanged -= handler;
        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        Assert.Equal(PictureBoxSizeMode.StretchImage, pictureBox.SizeMode);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<PictureBoxSizeMode>]
    public void PictureBox_SizeMode_SetInvalid_ThrowsInvalidEnumArgumentException(PictureBoxSizeMode value)
    {
        using PictureBox pictureBox = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => pictureBox.SizeMode = value);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void PictureBox_TabIndex_Set_GetReturnsExpected(int value)
    {
        using PictureBox control = new()
        {
            TabIndex = value
        };
        Assert.Equal(value, control.TabIndex);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.TabIndex = value;
        Assert.Equal(value, control.TabIndex);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void PictureBox_TabIndex_SetWithHandler_CallsTabIndexChanged()
    {
        using PictureBox control = new()
        {
            TabIndex = 0
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.TabIndexChanged += handler;

        // Set different.
        control.TabIndex = 1;
        Assert.Equal(1, control.TabIndex);
        Assert.Equal(1, callCount);

        // Set same.
        control.TabIndex = 1;
        Assert.Equal(1, control.TabIndex);
        Assert.Equal(1, callCount);

        // Set different.
        control.TabIndex = 2;
        Assert.Equal(2, control.TabIndex);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.TabIndexChanged -= handler;
        control.TabIndex = 1;
        Assert.Equal(1, control.TabIndex);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void PictureBox_TabIndex_SetNegative_CallsArgumentOutOfRangeException()
    {
        using PictureBox control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.TabIndex = -1);
    }

    [WinFormsTheory]
    [BoolData]
    public void PictureBox_TabStop_Set_GetReturnsExpected(bool value)
    {
        using PictureBox control = new()
        {
            TabStop = value
        };
        Assert.Equal(value, control.TabStop);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.TabStop = value;
        Assert.Equal(value, control.TabStop);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.TabStop = value;
        Assert.Equal(value, control.TabStop);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void PictureBox_TabStop_SetWithHandle_GetReturnsExpected(bool value)
    {
        using PictureBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.TabStop = value;
        Assert.Equal(value, control.TabStop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TabStop = value;
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.TabStop = value;
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void PictureBox_TabStop_SetWithHandler_CallsTabStopChanged()
    {
        using PictureBox control = new()
        {
            TabStop = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.TabStopChanged += handler;

        // Set different.
        control.TabStop = false;
        Assert.False(control.TabStop);
        Assert.Equal(1, callCount);

        // Set same.
        control.TabStop = false;
        Assert.False(control.TabStop);
        Assert.Equal(1, callCount);

        // Set different.
        control.TabStop = true;
        Assert.True(control.TabStop);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.TabStopChanged -= handler;
        control.TabStop = false;
        Assert.False(control.TabStop);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void PictureBox_Text_Set_GetReturnsExpected(string value, string expected)
    {
        using PictureBox control = new()
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
    public void PictureBox_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
    {
        using PictureBox control = new();
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
    public void PictureBox_Text_SetWithHandler_CallsTextChanged()
    {
        using PictureBox control = new();
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
    [BoolData]
    public void PictureBox_Visible_Set_GetReturnsExpected(bool value)
    {
        using PictureBox control = new()
        {
            Visible = value
        };
        Assert.Equal(value, control.Visible);

        // Set same.
        control.Visible = value;
        Assert.Equal(value, control.Visible);

        // Set different.
        control.Visible = !value;
        Assert.Equal(!value, control.Visible);
    }

    [WinFormsTheory]
    [BoolData]
    public void PictureBox_Visible_SetWithHandle_GetReturnsExpected(bool value)
    {
        using PictureBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.Visible = value;
        Assert.Equal(value, control.Visible);

        // Set same.
        control.Visible = value;
        Assert.Equal(value, control.Visible);

        // Set different.
        control.Visible = value;
        Assert.Equal(value, control.Visible);
    }

    [WinFormsFact]
    public void PictureBox_Visible_SetWithHandler_CallsVisibleChanged()
    {
        using PictureBox control = new()
        {
            Visible = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.VisibleChanged += handler;

        // Set different.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.Equal(1, callCount);

        // Set same.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.Equal(1, callCount);

        // Set different.
        control.Visible = true;
        Assert.True(control.Visible);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.VisibleChanged -= handler;
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void PictureBox_WaitOnLoad_Set_GetReturnsExpected(bool value)
    {
        using PictureBox pictureBox = new()
        {
            WaitOnLoad = value
        };
        Assert.Equal(value, pictureBox.WaitOnLoad);

        // Set same
        pictureBox.WaitOnLoad = value;
        Assert.Equal(value, pictureBox.WaitOnLoad);

        // Set different
        pictureBox.WaitOnLoad = !value;
        Assert.Equal(!value, pictureBox.WaitOnLoad);
    }

    [WinFormsFact]
    public void PictureBox_BeginInit_InvokeMultipleTimes_Success()
    {
        using PictureBox pictureBox = new();
        ISupportInitialize supportInitialize = pictureBox;
        supportInitialize.BeginInit();
        supportInitialize.BeginInit();
    }

    [WinFormsFact]
    public void PictureBox_BeginInit_EndInitValidImageWaitOnLoadTrue_SetsImage()
    {
        using PictureBox pictureBox = new()
        {
            WaitOnLoad = true
        };
        ISupportInitialize supportInitialize = pictureBox;
        supportInitialize.BeginInit();

        pictureBox.ImageLocation = PathImageLocation;
        Assert.Same(PathImageLocation, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);

        supportInitialize.EndInit();
        Assert.Same(PathImageLocation, pictureBox.ImageLocation);
        Assert.Equal(new Size(110, 100), pictureBox.Image.Size);
    }

    [WinFormsTheory]
    [NullAndEmptyStringData]
    public void PictureBox_BeginInit_EndInitNullOrEmptyImageWaitOnLoadTrue_DoesNotSetImage(string imageLocation)
    {
        using PictureBox pictureBox = new()
        {
            WaitOnLoad = true
        };
        ISupportInitialize supportInitialize = pictureBox;
        supportInitialize.BeginInit();

        pictureBox.ImageLocation = imageLocation;
        Assert.Same(imageLocation, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);

        supportInitialize.EndInit();
        Assert.Same(imageLocation, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);
    }

    [WinFormsFact]
    public void PictureBox_BeginInit_EndInitWaitOnLoadFalse_DoesNotSetImage()
    {
        using PictureBox pictureBox = new()
        {
            WaitOnLoad = false
        };
        ISupportInitialize supportInitialize = pictureBox;
        supportInitialize.BeginInit();

        pictureBox.ImageLocation = PathImageLocation;
        Assert.Same(PathImageLocation, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);

        supportInitialize.EndInit();
        Assert.Same(PathImageLocation, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);
    }

    [WinFormsTheory]
    [StringWithNullData]
    [InlineData("bitmaps/nature24bits.jpg")]
    [InlineData("  ")]
    public void PictureBox_EndInit_InvokeMultipleTimes_Nop(string imageLocation)
    {
        using PictureBox pictureBox = new()
        {
            ImageLocation = imageLocation
        };
        Assert.Equal(imageLocation, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);

        ISupportInitialize supportInitialize = pictureBox;
        supportInitialize.EndInit();
        Assert.Equal(imageLocation, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);

        supportInitialize.EndInit();
        Assert.Equal(imageLocation, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);
    }

    [WinFormsTheory]
    [StringWithNullData]
    [InlineData("bitmaps/nature24bits.jpg")]
    [InlineData("  ")]
    public void PictureBox_EndInit_InvokeMultipleTimesWaitOnLoad_DoesNotLoadImage(string imageLocation)
    {
        using PictureBox pictureBox = new()
        {
            ImageLocation = imageLocation
        };
        Assert.Equal(imageLocation, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);

        pictureBox.WaitOnLoad = true;
        Assert.Equal(imageLocation, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);

        ISupportInitialize supportInitialize = pictureBox;
        supportInitialize.EndInit();
        Assert.Equal(imageLocation, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);

        supportInitialize.EndInit();
        Assert.Equal(imageLocation, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);
    }

    [WinFormsFact]
    public void PictureBox_CancelAsync_InvokeWithoutStarting_Nop()
    {
        using PictureBox pictureBox = new();
        pictureBox.CancelAsync();
        pictureBox.CancelAsync();
    }

    [WinFormsFact]
    public void PictureBox_Dispose_Success()
    {
        using PictureBox pictureBox = new();
        pictureBox.Dispose();
        pictureBox.Dispose();
    }

    [WinFormsFact]
    public void PictureBox_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubPictureBox control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    [WinFormsTheory]
    [InlineData(ControlStyles.ContainerControl, false)]
    [InlineData(ControlStyles.UserPaint, true)]
    [InlineData(ControlStyles.Opaque, false)]
    [InlineData(ControlStyles.ResizeRedraw, false)]
    [InlineData(ControlStyles.FixedWidth, false)]
    [InlineData(ControlStyles.FixedHeight, false)]
    [InlineData(ControlStyles.StandardClick, true)]
    [InlineData(ControlStyles.Selectable, false)]
    [InlineData(ControlStyles.UserMouse, false)]
    [InlineData(ControlStyles.SupportsTransparentBackColor, true)]
    [InlineData(ControlStyles.StandardDoubleClick, true)]
    [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
    [InlineData(ControlStyles.CacheText, false)]
    [InlineData(ControlStyles.EnableNotifyMessage, false)]
    [InlineData(ControlStyles.DoubleBuffer, false)]
    [InlineData(ControlStyles.OptimizedDoubleBuffer, true)]
    [InlineData(ControlStyles.UseTextForAccessibility, true)]
    [InlineData((ControlStyles)0, true)]
    [InlineData((ControlStyles)int.MaxValue, false)]
    [InlineData((ControlStyles)(-1), false)]
    public void PictureBox_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubPictureBox control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void PictureBox_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubPictureBox control = new();
        Assert.False(control.GetTopLevel());
    }

    [WinFormsTheory]
    [InlineData("NoSuchImage")]
    [InlineData("  ")]
    public void PictureBox_Load_InvalidUrl_ThrowsException(string value)
    {
        using PictureBox pictureBox = new();

        Assert.ThrowsAny<Exception>(() => pictureBox.Load(value));
        Assert.Equal(value, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);

        // Call again.
        Assert.ThrowsAny<Exception>(() => pictureBox.Load(value));
        Assert.Equal(value, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);
    }

    [WinFormsTheory]
    [InlineData("NoSuchImage")]
    [InlineData("  ")]
    public void PictureBox_Load_InvalidUrlWithImage_ThrowsException(string value)
    {
        using Bitmap image = new(10, 10);
        using PictureBox pictureBox = new()
        {
            Image = image
        };

        Assert.ThrowsAny<Exception>(() => pictureBox.Load(value));
        Assert.Equal(value, pictureBox.ImageLocation);
        Assert.Same(image, pictureBox.Image);

        // Call again.
        Assert.ThrowsAny<Exception>(() => pictureBox.Load(value));
        Assert.Equal(value, pictureBox.ImageLocation);
        Assert.Same(image, pictureBox.Image);
    }

    [WinFormsFact]
    public void PictureBox_Load_Url_CallsInvalidated()
    {
        using PictureBox pictureBox = new();
        int invalidatedCallCount = 0;
        pictureBox.Invalidated += (sender, e) => invalidatedCallCount++;
        Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

        // Call different.
        Assert.ThrowsAny<Exception>(() => pictureBox.Load("Location"));
        Assert.Equal("Location", pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);
        Assert.Equal(1, invalidatedCallCount);

        // Call again.
        Assert.ThrowsAny<Exception>(() => pictureBox.Load("Location"));
        Assert.Equal("Location", pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);
        Assert.Equal(2, invalidatedCallCount);

        // Call null.
        Assert.Throws<InvalidOperationException>(() => pictureBox.Load(null));
        Assert.Null(pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);
        Assert.Equal(3, invalidatedCallCount);

        // Call empty.
        Assert.Throws<InvalidOperationException>(() => pictureBox.Load(string.Empty));
        Assert.Same(string.Empty, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);
        Assert.Equal(4, invalidatedCallCount);

        // Call valid.
        pictureBox.Load(PathImageLocation);
        Assert.Equal(PathImageLocation, pictureBox.ImageLocation);
        Assert.Equal(new Size(110, 100), pictureBox.Image.Size);
        Assert.Equal(6, invalidatedCallCount);
    }

    [WinFormsFact]
    public void PictureBox_Load_UrlValidWithWaitOnLoadTrueLocal_GetReturnsExpected()
    {
        using PictureBox pictureBox = new()
        {
            WaitOnLoad = true
        };

        pictureBox.Load(PathImageLocation);
        Assert.Same(PathImageLocation, pictureBox.ImageLocation);
        Assert.Equal(new Size(110, 100), pictureBox.Image.Size);

        // Call again.
        pictureBox.Load(PathImageLocation);
        Assert.Same(PathImageLocation, pictureBox.ImageLocation);
        Assert.Equal(new Size(110, 100), pictureBox.Image.Size);
    }

    [WinFormsTheory]
    [BoolData]
    public void PictureBox_Load_UrlValidWithWaitOnLoadTrueUri_ConfigSwitch_CheckCRL_GetReturnsExpected(bool switchValue)
    {
        using ServicePointManagerCheckCrlScope scope = new(switchValue);

        try
        {
            using PictureBox pictureBox = new()
            {
                WaitOnLoad = true
            };

            Size expectedImageSize = new(110, 100);

            pictureBox.Load(UrlImageLocation);
            Assert.Same(UrlImageLocation, pictureBox.ImageLocation);
            Assert.Equal(expectedImageSize, pictureBox.Image.Size);

            // Call again.
            pictureBox.Load(UrlImageLocation);
            Assert.Same(UrlImageLocation, pictureBox.ImageLocation);
            Assert.Equal(expectedImageSize, pictureBox.Image.Size);
        }
        catch
        {
            // Swallow network errors.
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageLocation_SetInvalidWithWaitOnLoadTrueDesignMode_TestData))]
    public void PictureBox_Load_UrlInvalidWithWaitOnLoadTrueDesignMode_GetReturnsExpected(Image initialImage, Image errorImage, string value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(new AmbientProperties());
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        using PictureBox pictureBox = new()
        {
            InitialImage = initialImage,
            ErrorImage = errorImage,
            WaitOnLoad = true,
            Site = mockSite.Object
        };

        pictureBox.Load(value);
        Assert.Same(pictureBox.ErrorImage, pictureBox.Image);
        Assert.Equal(value, pictureBox.ImageLocation);

        // Set same.
        pictureBox.Load(value);
        Assert.Same(pictureBox.ErrorImage, pictureBox.Image);
        Assert.Equal(value, pictureBox.ImageLocation);

        // NB: disposing the component with strictly mocked object causes tests to fail
        // Moq.MockException : ISite.Container invocation failed with mock behavior Strict.
        // All invocations on the mock must have a corresponding setup.
        pictureBox.Site = null;
    }

    [WinFormsTheory]
    [NullAndEmptyStringData]
    public void PictureBox_Load_UrlNullOrEmptyWithError_ResetsImage(string value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(new AmbientProperties());
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        using PictureBox pictureBox = new()
        {
            WaitOnLoad = true,
            Site = mockSite.Object
        };
        pictureBox.ImageLocation = "Invalid";
        Assert.Equal("Invalid", pictureBox.ImageLocation);
        Assert.Same(pictureBox.ErrorImage, pictureBox.Image);

        pictureBox.ImageLocation = value;
        Assert.Equal(value, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);

        // NB: disposing the component with strictly mocked object causes tests to fail
        // Moq.MockException : ISite.Container invocation failed with mock behavior Strict.
        // All invocations on the mock must have a corresponding setup.
        pictureBox.Site = null;
    }

    [WinFormsFact]
    public void PictureBox_Load_UrlInitializing_GetReturnsExpected()
    {
        using PictureBox pictureBox = new()
        {
            WaitOnLoad = true
        };
        ((ISupportInitialize)pictureBox).BeginInit();

        pictureBox.Load(PathImageLocation);
        Assert.Equal(PathImageLocation, pictureBox.ImageLocation);
        Assert.Equal(new Size(110, 100), pictureBox.Image.Size);

        // Call again.
        pictureBox.Load(PathImageLocation);
        Assert.Equal(PathImageLocation, pictureBox.ImageLocation);
        Assert.Equal(new Size(110, 100), pictureBox.Image.Size);
    }

    [WinFormsTheory]
    [NullAndEmptyStringData]
    public void PictureBox_Load_InvokeWithoutImageLocation_ThrowsInvalidOperationException(string imageLocation)
    {
        using PictureBox pictureBox = new()
        {
            ImageLocation = imageLocation
        };
        Assert.Throws<InvalidOperationException>(pictureBox.Load);
        Assert.Throws<InvalidOperationException>(() => pictureBox.Load(imageLocation));
    }

    [WinFormsTheory]
    [NullAndEmptyStringData]
    public void PictureBox_LoadAsync_InvokeWithoutImageLocation_ThrowsInvalidOperationException(string imageLocation)
    {
        using PictureBox pictureBox = new()
        {
            ImageLocation = imageLocation
        };
        Assert.Throws<InvalidOperationException>(pictureBox.LoadAsync);
        Assert.Throws<InvalidOperationException>(() => pictureBox.LoadAsync(imageLocation));
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void PictureBox_OnEnabledChanged_Invoke_CallsEnabledChanged(EventArgs eventArgs)
    {
        using SubPictureBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.EnabledChanged += handler;
        control.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.EnabledChanged -= handler;
        control.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void PictureBox_OnEnter_Invoke_CallsEnter(EventArgs eventArgs)
    {
        using SubPictureBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Enter += handler;
        control.OnEnter(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.Enter -= handler;
        control.OnEnter(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void PictureBox_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
    {
        using SubPictureBox control = new();
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
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.HandleCreated -= handler;
        control.OnHandleCreated(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void PictureBox_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(EventArgs eventArgs)
    {
        using SubPictureBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.True(control.GetStyle(ControlStyles.UserPaint));

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
        Assert.True(control.IsHandleCreated);

        // Remove handler.
        control.HandleCreated -= handler;
        control.OnHandleCreated(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void PictureBox_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using SubPictureBox control = new();
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
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.HandleDestroyed -= handler;
        control.OnHandleDestroyed(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void PictureBox_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using SubPictureBox control = new();
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
        Assert.True(control.IsHandleCreated);

        // Remove handler.
        control.HandleDestroyed -= handler;
        control.OnHandleDestroyed(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetKeyEventArgsTheoryData))]
    public void PictureBox_OnKeyDown_Invoke_CallsKeyDown(KeyEventArgs eventArgs)
    {
        using SubPictureBox control = new();
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

        // Remove handler.
        control.KeyDown -= handler;
        control.OnKeyDown(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetKeyPressEventArgsTheoryData))]
    public void PictureBox_OnKeyPress_Invoke_CallsKeyPress(KeyPressEventArgs eventArgs)
    {
        using SubPictureBox control = new();
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

        // Remove handler.
        control.KeyPress -= handler;
        control.OnKeyPress(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetKeyEventArgsTheoryData))]
    public void PictureBox_OnKeyUp_Invoke_CallsKeyUp(KeyEventArgs eventArgs)
    {
        using SubPictureBox control = new();
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

        // Remove handler.
        control.KeyUp -= handler;
        control.OnKeyUp(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void PictureBox_OnLeave_Invoke_CallsLeave(EventArgs eventArgs)
    {
        using SubPictureBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Leave += handler;
        control.OnLeave(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.Leave -= handler;
        control.OnLeave(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> OnLoadCompleted_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new AsyncCompletedEventArgs(null, false, null) };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnLoadCompleted_TestData))]
    public void PictureBox_OnLoadCompleted_Invoke_CallsLoadCompleted(AsyncCompletedEventArgs eventArgs)
    {
        using SubPictureBox control = new();
        int callCount = 0;
        AsyncCompletedEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.LoadCompleted += handler;
        control.OnLoadCompleted(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.LoadCompleted -= handler;
        control.OnLoadCompleted(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> OnLoadProgressChanged_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new ProgressChangedEventArgs(0, null) };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnLoadProgressChanged_TestData))]
    public void PictureBox_OnLoadProgressChanged_Invoke_CallsLoadProgressChanged(ProgressChangedEventArgs eventArgs)
    {
        using SubPictureBox control = new();
        int callCount = 0;
        ProgressChangedEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.LoadProgressChanged += handler;
        control.OnLoadProgressChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.LoadProgressChanged -= handler;
        control.OnLoadProgressChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaintEventArgsTheoryData))]
    public void PictureBox_OnPaint_Invoke_CallsPaint(PaintEventArgs eventArgs)
    {
        using SubPictureBox control = new();
        int callCount = 0;
        PaintEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Paint += handler;
        control.OnPaint(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.Paint -= handler;
        control.OnPaint(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> OnPaint_Image_TestData()
    {
        foreach (PaintEventArgs testData in CommonTestHelperEx.GetPaintEventArgsTheoryData())
        {
            foreach (PictureBoxSizeMode sizeMode in Enum.GetValues(typeof(PictureBoxSizeMode)))
            {
                yield return new object[] { sizeMode, testData };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnPaint_Image_TestData))]
    public void PictureBox_OnPaint_InvokeWithValidImage_CallsLoad(PictureBoxSizeMode sizeMode, PaintEventArgs eventArgs)
    {
        using Bitmap image = new(10, 10);
        using SubPictureBox pictureBox = new()
        {
            SizeMode = sizeMode,
            Image = image
        };
        Assert.Same(image, pictureBox.Image);

        pictureBox.OnPaint(eventArgs);
        Assert.Same(image, pictureBox.Image);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnPaint_Image_TestData))]
    public void PictureBox_OnPaint_InvokeWithValidImageLocationWaitOnLoadTrue_CallsLoad(PictureBoxSizeMode sizeMode, PaintEventArgs eventArgs)
    {
        using SubPictureBox pictureBox = new()
        {
            SizeMode = sizeMode,
            ImageLocation = PathImageLocation
        };
        Assert.Null(pictureBox.Image);
        Assert.Equal(PathImageLocation, pictureBox.ImageLocation);

        pictureBox.WaitOnLoad = true;
        pictureBox.OnPaint(eventArgs);
        Assert.Equal(new Size(110, 100), pictureBox.Image.Size);
        Assert.Equal(PathImageLocation, pictureBox.ImageLocation);
    }

    public static IEnumerable<object[]> OnPaint_NullOrEmptyImageLocation_TestData()
    {
        foreach (PaintEventArgs testData in CommonTestHelperEx.GetPaintEventArgsTheoryData())
        {
            yield return new object[] { string.Empty, testData };
            yield return new object[] { null, testData };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnPaint_NullOrEmptyImageLocation_TestData))]
    public void PictureBox_OnPaint_InvokeWithNullOrEmptyImageLocationWaitOnLoadTrue_CallsLoad(string imageLocation, PaintEventArgs eventArgs)
    {
        using SubPictureBox pictureBox = new()
        {
            ImageLocation = imageLocation
        };
        Assert.Same(imageLocation, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);

        pictureBox.WaitOnLoad = true;
        pictureBox.OnPaint(eventArgs);
        Assert.Same(imageLocation, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);
    }

    public static IEnumerable<object[]> OnPaint_InvalidImageLocation_TestData()
    {
        foreach (PaintEventArgs testData in CommonTestHelperEx.GetPaintEventArgsTheoryData())
        {
            foreach (PictureBoxSizeMode sizeMode in Enum.GetValues(typeof(PictureBoxSizeMode)))
            {
                yield return new object[] { sizeMode, "NoSuchImage", testData };
                yield return new object[] { sizeMode, "  ", testData };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnPaint_InvalidImageLocation_TestData))]
    public void PictureBox_OnPaint_InvokeWithInvalidImageLocationWaitOnLoadTrue_CallsLoad(PictureBoxSizeMode sizeMode, string imageLocation, PaintEventArgs eventArgs)
    {
        using SubPictureBox pictureBox = new()
        {
            SizeMode = sizeMode,
            ImageLocation = imageLocation
        };
        Assert.Same(imageLocation, pictureBox.ImageLocation);
        Assert.Null(pictureBox.Image);

        pictureBox.WaitOnLoad = true;
        pictureBox.OnPaint(eventArgs);
        Assert.Same(imageLocation, pictureBox.ImageLocation);
        Assert.Same(pictureBox.ErrorImage, pictureBox.Image);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaintEventArgsTheoryData))]
    public void PictureBox_OnPaint_InvokeWithImage_CallsPaint(PaintEventArgs eventArgs)
    {
        using SubPictureBox control = new()
        {
            Image = new Bitmap(10, 10)
        };
        int callCount = 0;
        PaintEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Paint += handler;
        control.OnPaint(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.Paint -= handler;
        control.OnPaint(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void PictureBox_OnParentChanged_Invoke_CallsParentChanged(EventArgs eventArgs)
    {
        using SubPictureBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.ParentChanged += handler;
        control.OnParentChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.ParentChanged -= handler;
        control.OnParentChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void PictureBox_OnResize_Invoke_CallsResize(EventArgs eventArgs)
    {
        using SubPictureBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };

        // Call with handler.
        control.Resize += handler;
        control.OnResize(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(1, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.Resize -= handler;
        control.OnResize(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(2, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnResize_WithHandle_TestData()
    {
        yield return new object[] { true, null, 1 };
        yield return new object[] { true, new EventArgs(), 1 };
        yield return new object[] { false, null, 0 };
        yield return new object[] { false, new EventArgs(), 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnResize_WithHandle_TestData))]
    public void PictureBox_OnResize_InvokeWithHandle_CallsResize(bool resizeRedraw, EventArgs eventArgs, int expectedInvalidatedCallCount)
    {
        using SubPictureBox control = new();
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        // Call with handler.
        control.Resize += handler;
        control.OnResize(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(1, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.Resize -= handler;
        control.OnResize(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(2, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(PictureBoxSizeMode.Normal, 0)]
    [InlineData(PictureBoxSizeMode.StretchImage, 1)]
    [InlineData(PictureBoxSizeMode.AutoSize, 0)]
    [InlineData(PictureBoxSizeMode.CenterImage, 1)]
    [InlineData(PictureBoxSizeMode.Zoom, 1)]
    public void PictureBox_OnResize_Invoke_CallsInvalidate(PictureBoxSizeMode sizeMode, int expectedInvalidatedCallCount)
    {
        using SubPictureBox control = new()
        {
            SizeMode = sizeMode
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        // Call with handler.
        control.Resize += handler;
        control.OnResize(EventArgs.Empty);
        Assert.Equal(1, callCount);
        Assert.Equal(1, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.Resize -= handler;
        control.OnResize(EventArgs.Empty);
        Assert.Equal(1, callCount);
        Assert.Equal(2, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void PictureBox_OnSizeModeChanged_Invoke_CallsSizeModeChanged(EventArgs eventArgs)
    {
        using SubPictureBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.SizeModeChanged += handler;
        control.OnSizeModeChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.SizeModeChanged -= handler;
        control.OnSizeModeChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void PictureBox_OnVisibleChanged_Invoke_CallsVisibleChanged(EventArgs eventArgs)
    {
        using SubPictureBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.VisibleChanged += handler;
        control.OnVisibleChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.VisibleChanged -= handler;
        control.OnVisibleChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void PictureBox_ToString_Invoke_ReturnsExpected()
    {
        using PictureBox control = new();
        Assert.Equal("System.Windows.Forms.PictureBox, SizeMode: Normal", control.ToString());
    }

    private class SubPictureBox : PictureBox
    {
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

        public new bool DesignMode => base.DesignMode;

        public new bool DoubleBuffered
        {
            get => base.DoubleBuffered;
            set => base.DoubleBuffered = value;
        }

        public new EventHandlerList Events => base.Events;

        public new int FontHeight
        {
            get => base.FontHeight;
            set => base.FontHeight = value;
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

        public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new bool GetTopLevel() => base.GetTopLevel();

        public new void OnEnabledChanged(EventArgs e) => base.OnEnabledChanged(e);

        public new void OnEnter(EventArgs e) => base.OnEnter(e);

        public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

        public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

        public new void OnKeyDown(KeyEventArgs e) => base.OnKeyDown(e);

        public new void OnKeyPress(KeyPressEventArgs e) => base.OnKeyPress(e);

        public new void OnKeyUp(KeyEventArgs e) => base.OnKeyUp(e);

        public new void OnLeave(EventArgs e) => base.OnLeave(e);

        public new void OnLoadCompleted(AsyncCompletedEventArgs e) => base.OnLoadCompleted(e);

        public new void OnLoadProgressChanged(ProgressChangedEventArgs e) => base.OnLoadProgressChanged(e);

        public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);

        public new void OnParentChanged(EventArgs e) => base.OnParentChanged(e);

        public new void OnResize(EventArgs e) => base.OnResize(e);

        public new void OnSizeModeChanged(EventArgs e) => base.OnSizeModeChanged(e);

        public new void OnVisibleChanged(EventArgs e) => base.OnVisibleChanged(e);

        public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
    }
}
