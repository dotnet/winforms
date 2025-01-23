// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class DateTimePickerTests : IDisposable
{
    private readonly DateTimePicker _dateTimePicker;

    public DateTimePickerTests() => _dateTimePicker = new();

    public void Dispose() => _dateTimePicker.Dispose();

    [WinFormsFact]
    public void DateTimePicker_Ctor_Default()
    {
        using SubDateTimePicker control = new();
        Assert.Null(control.AccessibleDefaultActionDescription);
        Assert.Null(control.AccessibleDescription);
        Assert.Null(control.AccessibleName);
        Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
        Assert.False(control.AllowDrop);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.False(control.AutoSize);
        Assert.Equal(SystemColors.Window, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.Null(control.BindingContext);
        Assert.Equal(control.PreferredHeight, control.Bottom);
        Assert.Equal(new Rectangle(0, 0, 200, control.PreferredHeight), control.Bounds);
        Assert.Equal(Control.DefaultForeColor, control.CalendarForeColor);
        Assert.Equal(Control.DefaultFont, control.CalendarFont);
        Assert.Equal(SystemColors.Window, control.CalendarMonthBackground);
        Assert.Equal(SystemColors.ActiveCaption, control.CalendarTitleBackColor);
        Assert.Equal(SystemColors.ActiveCaptionText, control.CalendarTitleForeColor);
        Assert.Equal(SystemColors.GrayText, control.CalendarTrailingForeColor);
        Assert.True(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.True(control.CanRaiseEvents);
        Assert.True(control.CanSelect);
        Assert.False(control.Capture);
        Assert.True(control.CausesValidation);
        Assert.True(control.Checked);
        Assert.Equal(new Size(196, control.PreferredHeight - 4), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 196, control.PreferredHeight - 4), control.ClientRectangle);
        Assert.Null(control.Container);
        Assert.False(control.ContainsFocus);
        Assert.Null(control.ContextMenuStrip);
        Assert.Empty(control.Controls);
        Assert.Same(control.Controls, control.Controls);
        Assert.False(control.Created);
        Assert.Same(Cursors.Default, control.Cursor);
        Assert.Null(control.CustomFormat);
        Assert.Same(Cursors.Default, control.DefaultCursor);
        Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
        Assert.Equal(new Padding(3), control.DefaultMargin);
        Assert.Equal(Size.Empty, control.DefaultMaximumSize);
        Assert.Equal(Size.Empty, control.DefaultMinimumSize);
        Assert.Equal(Padding.Empty, control.DefaultPadding);
        Assert.Equal(new Size(200, control.PreferredHeight), control.DefaultSize);
        Assert.False(control.DesignMode);
        Assert.Equal(new Rectangle(0, 0, 196, control.PreferredHeight - 4), control.DisplayRectangle);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.False(control.DoubleBuffered);
        Assert.Equal(LeftRightAlignment.Left, control.DropDownAlign);
        Assert.True(control.Enabled);
        Assert.NotNull(control.Events);
        Assert.Same(control.Events, control.Events);
        Assert.False(control.Focused);
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.Equal(SystemColors.WindowText, control.ForeColor);
        Assert.Equal(DateTimePickerFormat.Long, control.Format);
        Assert.False(control.HasChildren);
        Assert.Equal(control.PreferredHeight, control.Height);
        Assert.Equal(ImeMode.NoControl, control.ImeMode);
        Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
        Assert.False(control.IsAccessible);
        Assert.False(control.IsMirrored);
        Assert.NotNull(control.LayoutEngine);
        Assert.Same(control.LayoutEngine, control.LayoutEngine);
        Assert.Equal(0, control.Left);
        Assert.Equal(Point.Empty, control.Location);
        Assert.Equal(new Padding(3), control.Margin);
        Assert.Equal(new DateTime(9998, 12, 31), control.MaxDate);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.Equal(new DateTime(1753, 1, 1), control.MinDate);
        Assert.Equal(Size.Empty, control.MinimumSize);
        Assert.Equal(Padding.Empty, control.Padding);
        Assert.Null(control.Parent);
        Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
        Assert.Equal(new Size(200, control.PreferredHeight), control.PreferredSize);
        Assert.Equal(Control.DefaultFont.Height + 7, control.PreferredHeight);
        Assert.Equal(control.PreferredHeight, control.PreferredHeight);
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.False(control.ResizeRedraw);
        Assert.Equal(200, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.False(control.RightToLeftLayout);
        Assert.False(control.ShowCheckBox);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowKeyboardCues);
        Assert.False(control.ShowUpDown);
        Assert.Null(control.Site);
        Assert.Equal(new Size(200, control.PreferredHeight), control.Size);
        Assert.Equal(0, control.TabIndex);
        Assert.True(control.TabStop);
        Assert.Equal(string.Empty, control.Text);
        Assert.Equal(0, control.Top);
        Assert.Null(control.TopLevelControl);
        Assert.False(control.UseWaitCursor);
        Assert.True(control.Value > DateTime.MinValue);
        Assert.True(control.Visible);
        Assert.Equal(200, control.Width);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DateTimePicker_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubDateTimePicker control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("SysDateTimePick32", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0x200, createParams.ExStyle);
        Assert.Equal(control.PreferredHeight, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x56010004, createParams.Style);
        Assert.Equal(200, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DateTimePicker_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubDateTimePicker control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    [WinFormsTheory]
    [EnumData<DateTimePickerFormat>]
    public void DateTimePicker_Format_Set_GetReturnsExpected(DateTimePickerFormat value)
    {
        using SubDateTimePicker control = new();

        control.Format = value;
        Assert.Equal(value, control.Format);
    }

    [WinFormsTheory]
    [InvalidEnumData<DateTimePickerFormat>]
    public void DateTimePicker_Format_SetInvalid_ThrowsInvalidEnumArgumentException(DateTimePickerFormat value)
    {
        using DateTimePicker control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.Format = value);
    }

    [WinFormsFact]
    public void DateTimePicker_CalendarTitleBackColor_GetSet_ReturnsExpected()
    {
        using DateTimePicker control = new();
        var expectedColor = Color.Red;

        control.CalendarTitleBackColor = expectedColor;

        control.CalendarTitleBackColor.Should().Be(expectedColor);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid color")]
    public void DateTimePicker_CalendarTitleBackColor_SetInvalid_ThrowsArgumentException(string value)
    {
        using DateTimePicker control = new();
        Action act = () => control.CalendarTitleBackColor = ColorTranslator.FromHtml(value);
        act.Should().Throw<ArgumentException>();
    }

    [WinFormsFact]
    public void DateTimePicker_CalendarForeColor_GetSet_ReturnsExpected()
    {
        using DateTimePicker control = new();
        var expectedColor = Color.Red;

        control.CalendarForeColor = expectedColor;

        control.CalendarForeColor.Should().Be(expectedColor);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid color")]
    public void DateTimePicker_CalendarForeColor_SetInvalid_ThrowsArgumentException(string value)
    {
        using DateTimePicker control = new();
        Action act = () => control.CalendarForeColor = ColorTranslator.FromHtml(value);
        act.Should().Throw<ArgumentException>();
    }

    [WinFormsFact]
    public void DateTimePicker_CalendarTitleForeColor_GetSet_ReturnsExpected()
    {
        using DateTimePicker control = new();
        var expectedColor = Color.Red;

        control.CalendarTitleForeColor = expectedColor;

        control.CalendarTitleForeColor.Should().Be(expectedColor);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid color")]
    public void DateTimePicker_CalendarTitleForeColor_SetInvalid_ThrowsArgumentException(string value)
    {
        using DateTimePicker control = new();
        Action act = () => control.CalendarTitleForeColor = ColorTranslator.FromHtml(value);
        act.Should().Throw<ArgumentException>();
    }

    [WinFormsFact]
    public void DateTimePicker_CalendarTrailingForeColor_GetSet_ReturnsExpected()
    {
        using DateTimePicker control = new();
        var expectedColor = Color.Red;

        control.CalendarTrailingForeColor = expectedColor;

        control.CalendarTrailingForeColor.Should().Be(expectedColor);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid color")]
    public void DateTimePicker_CalendarTrailingForeColor_SetInvalid_ThrowsArgumentException(string value)
    {
        using DateTimePicker control = new();
        Action act = () => control.CalendarTrailingForeColor = ColorTranslator.FromHtml(value);
        act.Should().Throw<ArgumentException>();
    }

    [WinFormsFact]
    public void DateTimePicker_CalendarMonthBackground_GetSet_ReturnsExpected()
    {
        using DateTimePicker control = new();
        var expectedColor = Color.Red;

        control.CalendarMonthBackground = expectedColor;

        control.CalendarMonthBackground.Should().Be(expectedColor);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid color")]
    public void DateTimePicker_CalendarMonthBackground_SetInvalid_ThrowsArgumentException(string value)
    {
        using DateTimePicker control = new();
        Action act = () => control.CalendarMonthBackground = ColorTranslator.FromHtml(value);
        act.Should().Throw<ArgumentException>();
    }

    [WinFormsFact]
    public void DateTimePicker_CalendarFont_GetSet_ReturnsExpected()
    {
        using (Font expectedFont = new("Arial", 8.25f))
        {
            _dateTimePicker.CalendarFont = expectedFont;
            _dateTimePicker.CalendarFont.Should().Be(expectedFont);
        }

        using (Font differentFont = new("Times New Roman", 10f))
        {
            _dateTimePicker.CalendarFont = differentFont;
            _dateTimePicker.CalendarFont.Should().Be(differentFont);
        }

        _dateTimePicker.CalendarFont = null;
        _dateTimePicker.CalendarFont.Should().Be(_dateTimePicker.Font);
    }

    [WinFormsFact]
    public void DateTimePicker_Checked_WhenShowCheckBoxTrueAndHandleCreated_ReturnsExpected()
    {
        _dateTimePicker.ShowCheckBox = true;
        _dateTimePicker.CreateControl();

        _dateTimePicker.Checked = true;
        _dateTimePicker.Checked.Should().BeTrue();

        _dateTimePicker.Checked = false;
        _dateTimePicker.Checked.Should().BeFalse();

        _dateTimePicker.Checked = true;
        _dateTimePicker.Checked.Should().BeTrue();
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DateTimePicker_Checked_WhenShowCheckBoxAndHandleNotCreated_ReturnsExpected(bool value)
    {
        _dateTimePicker.ShowCheckBox = value;

        _dateTimePicker.Checked = true;
        _dateTimePicker.Checked.Should().BeTrue();

        _dateTimePicker.Checked = false;
        _dateTimePicker.Checked.Should().BeFalse();
    }

    [WinFormsFact]
    public void DateTimePicker_CustomFormat_GetSet_ReturnsExpected()
    {
        _dateTimePicker.CustomFormat.Should().BeNull();

        _dateTimePicker.Value = new(2021, 12, 31);
        _dateTimePicker.CreateControl();
        _dateTimePicker.Format = DateTimePickerFormat.Custom;

        string newCustomFormat = "yyyy/MM/dd";
        _dateTimePicker.CustomFormat = newCustomFormat;
        _dateTimePicker.CustomFormat.Should().Be(newCustomFormat);
        _dateTimePicker.Text.Should().Be("2021/12/31");

        string newCustomFormat2 = "MM/dd/yyyy";
        _dateTimePicker.CustomFormat = newCustomFormat2;
        _dateTimePicker.CustomFormat.Should().Be(newCustomFormat2);
        _dateTimePicker.Text.Should().Be("12/31/2021");

        _dateTimePicker.CustomFormat = null;
        _dateTimePicker.CustomFormat.Should().BeNull();
    }

    [WinFormsFact]
    public void DateTimePicker_DropDownAlign_GetSet_ReturnsExpected()
    {
        _dateTimePicker.DropDownAlign.Should().Be(LeftRightAlignment.Left);

        _dateTimePicker.DropDownAlign = LeftRightAlignment.Right;
        _dateTimePicker.DropDownAlign.Should().Be(LeftRightAlignment.Right);

        _dateTimePicker.DropDownAlign = LeftRightAlignment.Left;
        _dateTimePicker.DropDownAlign.Should().Be(LeftRightAlignment.Left);
    }

    [WinFormsFact]
    public void DateTimePicker_MaxDate_GetSet_ReturnsExpected()
    {
        _dateTimePicker.MaxDate.ToString().Should().Be("12/31/9998 12:00:00 AM");

        DateTime expectedDate = new(2022, 12, 31);
        _dateTimePicker.MaxDate = expectedDate;
        _dateTimePicker.MaxDate.Should().Be(expectedDate);

        DateTime initialMaxDate = _dateTimePicker.MaxDate;
        _dateTimePicker.MaxDate = initialMaxDate;
        _dateTimePicker.MaxDate.Should().Be(initialMaxDate);
    }

    [WinFormsTheory]
    [InlineData("0001-01-01")]
    [InlineData("9999-12-31")]
    public void DateTimePicker_MaxDate_SetInvalid_ThrowsArgumentOutOfRangeException(string value)
    {
        Action act = () => _dateTimePicker.MaxDate = DateTime.Parse(value);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [WinFormsFact]
    public void DateTimePicker_MaximumDateTime_ReturnsExpected()
    {
        DateTime maxSupportedDateTime = CultureInfo.CurrentCulture.Calendar.MaxSupportedDateTime;
        DateTime expectedDate = maxSupportedDateTime.Year > DateTimePicker.MaxDateTime.Year ? DateTimePicker.MaxDateTime : maxSupportedDateTime;

        DateTime result = DateTimePicker.MaximumDateTime;

        result.Should().Be(expectedDate);
    }

    [WinFormsFact]
    public void DateTimePicker_MinDate_GetSet_ReturnsExpected()
    {
        _dateTimePicker.MinDate.ToString().Should().Be("1/1/1753 12:00:00 AM");

        DateTime expectedDate = DateTimePicker.MinimumDateTime.AddDays(1);
        _dateTimePicker.MinDate = expectedDate;
        _dateTimePicker.MinDate.Should().Be(expectedDate);
    }

    [WinFormsFact]
    public void DateTimePicker_MinDate_SetGreaterThanMaxDate_LessThanMinimumDateTime_Should_ThrowArgumentOutOfRangeException()
    {
        Action act = () => _dateTimePicker.MinDate = DateTimePicker.MaximumDateTime.AddDays(1);
        act.Should().Throw<ArgumentOutOfRangeException>();

        act = () => _dateTimePicker.MinDate = DateTimePicker.MinimumDateTime.AddDays(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [WinFormsFact]
    public void DateTimePicker_MinDate_Set_AdjustsValueIfNeeded()
    {
        _dateTimePicker.Value = DateTimePicker.MinimumDateTime.AddDays(5);
        DateTime newMinDate = DateTimePicker.MinimumDateTime.AddDays(10);
        _dateTimePicker.MinDate = newMinDate;

        _dateTimePicker.MinDate.Should().Be(newMinDate);
        _dateTimePicker.Value.Should().Be(newMinDate);
    }

    [WinFormsFact]
    public void DateTimePicker_MinimumDateTime_ReturnsExpected()
    {
        DateTime minSupportedDateTime = CultureInfo.CurrentCulture.Calendar.MinSupportedDateTime;
        DateTime expectedDate = minSupportedDateTime.Year < 1753 ? new(1753, 1, 1) : minSupportedDateTime;

        DateTimePicker.MinimumDateTime.Should().BeOnOrAfter(new DateTime(1753, 1, 1, 0, 0, 0));

        DateTime result = DateTimePicker.MinimumDateTime;

        result.Should().Be(expectedDate);
    }

    [WinFormsFact]
    public void DateTimePicker_RightToLeftLayout_GetSet_ReturnsExpected()
    {
        _dateTimePicker.RightToLeftLayout.Should().Be(false);

        _dateTimePicker.RightToLeftLayout = true;
        _dateTimePicker.RightToLeftLayout.Should().Be(true);

        _dateTimePicker.RightToLeftLayout = false;
        _dateTimePicker.RightToLeftLayout.Should().Be(false);
    }

    [WinFormsFact]
    public void DateTimePicker_ShowUpDown_GetSet_ReturnsExpected()
    {
        _dateTimePicker.ShowUpDown.Should().Be(false);

        _dateTimePicker.ShowUpDown = true;
        _dateTimePicker.ShowUpDown.Should().Be(true);

        _dateTimePicker.ShowUpDown = false;
        _dateTimePicker.ShowUpDown.Should().Be(false);
    }

    [WinFormsFact]
    public void DateTimePicker_Text_GetSet_ReturnsExpected()
    {
        string validDateString = "2022-01-01";
        _dateTimePicker.CreateControl();

        _dateTimePicker.Text = validDateString;
        _dateTimePicker.Text.Should().Be(DateTime.Parse(validDateString, CultureInfo.CurrentCulture).ToString("dddd, MMMM d, yyyy"));

        _dateTimePicker.Text = null;
        _dateTimePicker.Text.Should().Be(DateTime.Parse(DateTime.Now.Date.ToString(), CultureInfo.CurrentCulture).ToString("dddd, MMMM d, yyyy"));

        _dateTimePicker.Text = string.Empty;
        _dateTimePicker.Text.Should().Be(DateTime.Parse(DateTime.Now.Date.ToString(), CultureInfo.CurrentCulture).ToString("dddd, MMMM d, yyyy"));
    }

    [WinFormsFact]
    public void DateTimePicker_Value_GetSet_ReturnsExpected()
    {
        DateTime initialDate = new(2022, 1, 1);
        DateTime newDate = new(2023, 1, 1);

        _dateTimePicker.Value = initialDate;
        _dateTimePicker.Value.Should().Be(initialDate);

        _dateTimePicker.Value = newDate;
        _dateTimePicker.Value.Should().Be(newDate);

        _dateTimePicker.Value = DateTimePicker.MinimumDateTime;
        _dateTimePicker.Value.Should().Be(DateTimePicker.MinimumDateTime);

        _dateTimePicker.Value = DateTimePicker.MaximumDateTime;
        _dateTimePicker.Value.Should().Be(DateTimePicker.MaximumDateTime);
    }

    [WinFormsTheory]
    [InlineData("0001-01-01")]
    [InlineData("9999-12-31")]
    public void DateTimePicker_Value_SetInvalid_ThrowsArgumentOutOfRangeException(string value)
    {
        Action act = () => _dateTimePicker.Value = DateTime.Parse(value);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [WinFormsFact]
    public void DateTimePicker_FormatChangedEvent_Raised_Success()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(_dateTimePicker);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        _dateTimePicker.FormatChanged += handler;
        _dateTimePicker.Format = DateTimePickerFormat.Short;
        callCount.Should().Be(1);

        _dateTimePicker.FormatChanged -= handler;
        _dateTimePicker.Format = DateTimePickerFormat.Long;
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void DateTimePicker_PaintEvent_Raised_Success()
    {
        using SubDateTimePicker control = new();
        int callCount = 0;
        PaintEventHandler handler = (sender, e) =>
        {
            sender.Should().Be(control);
            e.Should().NotBeNull();
            callCount++;
        };

        control.Paint += handler;
        using (Bitmap bmp = new(1, 1))
        {
            control.OnPaint(new(Graphics.FromImage(bmp), default));
        }

        callCount.Should().Be(1);

        control.Paint -= handler;
        using (Bitmap bmp = new(1, 1))
        {
            control.OnPaint(new(Graphics.FromImage(bmp), default));
        }

        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void DateTimePicker_MouseClickEvent_Raised_Success()
    {
        using SubDateTimePicker control = new();
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            callCount++;
            sender.Should().Be(control);
            e.Should().NotBeNull();
        };

        control.MouseClick += handler;
        control.OnMouseClick(new(MouseButtons.Left, 1, 0, 0, 0));
        callCount.Should().Be(1);

        control.MouseClick -= handler;
        control.OnMouseClick(new(MouseButtons.Left, 1, 0, 0, 0));
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void DateTimePicker_MouseDoubleClickEvent_Raised_Success()
    {
        using SubDateTimePicker control = new();
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            callCount++;
            sender.Should().Be(control);
            e.Should().NotBeNull();
        };

        control.MouseDoubleClick += handler;
        control.OnMouseDoubleClick(new(MouseButtons.Left, 1, 0, 0, 0));
        callCount.Should().Be(1);

        control.MouseDoubleClick -= handler;
        control.OnMouseDoubleClick(new(MouseButtons.Left, 1, 0, 0, 0));
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void DateTimePicker_CloseUpEvent_Raised_Success()
    {
        using SubDateTimePicker control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(control);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        control.CloseUp += handler;
        control.OnCloseUp(EventArgs.Empty);
        callCount.Should().Be(1);

        control.CloseUp -= handler;
        control.OnCloseUp(EventArgs.Empty);
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void DateTimePicker_RightToLeftLayoutChangedEvent_Raised_Success()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(_dateTimePicker);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        _dateTimePicker.RightToLeftLayoutChanged += handler;
        _dateTimePicker.RightToLeftLayout = !_dateTimePicker.RightToLeftLayout;
        callCount.Should().Be(1);

        _dateTimePicker.RightToLeftLayoutChanged -= handler;
        _dateTimePicker.RightToLeftLayout = !_dateTimePicker.RightToLeftLayout;
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void DateTimePicker_ValueChangedEvent_Raised_Success()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(_dateTimePicker);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        _dateTimePicker.ValueChanged += handler;
        _dateTimePicker.Value = DateTime.Now;
        callCount.Should().Be(1);

        _dateTimePicker.ValueChanged -= handler;
        _dateTimePicker.Value = DateTime.Now.AddDays(1);
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void DateTimePicker_DropDownEvent_Raised_Success()
    {
        using SubDateTimePicker control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(control);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        control.DropDown += handler;
        control.OnDropDown(EventArgs.Empty);
        callCount.Should().Be(1);

        control.DropDown -= handler;
        control.OnDropDown(EventArgs.Empty);
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void DateTimePicker_BackColorChangedEvent_Raised_Success()
    {
        using DateTimePicker control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(control);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        control.BackColorChanged += handler;
        control.BackColor = Color.Red;
        callCount.Should().Be(1);

        control.BackColorChanged -= handler;
        control.BackColor = Color.Green;
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void DateTimePicker_BackgroundImageChangedEvent_Raised_Success()
    {
        using DateTimePicker control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(control);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        control.BackgroundImageChanged += handler;
        using (Bitmap bmp = new(10, 10))
        {
            control.BackgroundImage = bmp;
        }

        callCount.Should().Be(1);

        control.BackgroundImageChanged -= handler;
        using (Bitmap bmp = new(20, 20))
        {
            control.BackgroundImage = bmp;
        }

        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void DateTimePicker_BackgroundImageLayoutChangedEvent_Raised_Success()
    {
        using DateTimePicker control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(control);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        control.BackgroundImageLayoutChanged += handler;
        control.BackgroundImageLayout = ImageLayout.Center;
        callCount.Should().Be(1);

        control.BackgroundImageLayoutChanged -= handler;
        control.BackgroundImageLayout = ImageLayout.Stretch;
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void DateTimePicker_ClickEvent_Raised_Success()
    {
        using SubDateTimePicker control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(control);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        control.Click += handler;
        control.OnClick(EventArgs.Empty);
        callCount.Should().Be(1);

        control.Click -= handler;
        control.OnClick(EventArgs.Empty);
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void DateTimePicker_DoubleClickEvent_Raised_Success()
    {
        using SubDateTimePicker control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(control);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        control.DoubleClick += handler;
        control.OnDoubleClick(EventArgs.Empty);
        callCount.Should().Be(1);

        control.DoubleClick -= handler;
        control.OnDoubleClick(EventArgs.Empty);
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void DateTimePicker_ForeColorChangedEvent_Raised_Success()
    {
        using DateTimePicker control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(control);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        control.ForeColorChanged += handler;
        control.ForeColor = Color.Red;
        callCount.Should().Be(1);

        control.ForeColorChanged -= handler;
        control.ForeColor = Color.Blue;
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void DateTimePicker_PaddingChangedEvent_Raised_Success()
    {
        using DateTimePicker control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(control);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        control.PaddingChanged += handler;
        control.Padding = new(10);
        callCount.Should().Be(1);

        control.PaddingChanged -= handler;
        control.Padding = new(20);
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void DateTimePicker_TextChangedEvent_Raised_Success()
    {
        using SubDateTimePicker control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(control);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        control.TextChanged += handler;
        control.OnTextChanged(EventArgs.Empty);
        callCount.Should().Be(1);

        control.TextChanged -= handler;
        control.OnTextChanged(EventArgs.Empty);
        callCount.Should().Be(1);
    }

    [WinFormsTheory]
    [InlineData(ControlStyles.ContainerControl, false)]
    [InlineData(ControlStyles.UserPaint, false)]
    [InlineData(ControlStyles.Opaque, false)]
    [InlineData(ControlStyles.ResizeRedraw, false)]
    [InlineData(ControlStyles.FixedWidth, false)]
    [InlineData(ControlStyles.FixedHeight, true)]
    [InlineData(ControlStyles.StandardClick, false)]
    [InlineData(ControlStyles.Selectable, true)]
    [InlineData(ControlStyles.UserMouse, false)]
    [InlineData(ControlStyles.SupportsTransparentBackColor, false)]
    [InlineData(ControlStyles.StandardDoubleClick, true)]
    [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
    [InlineData(ControlStyles.CacheText, false)]
    [InlineData(ControlStyles.EnableNotifyMessage, false)]
    [InlineData(ControlStyles.DoubleBuffer, false)]
    [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
    [InlineData(ControlStyles.UseTextForAccessibility, false)]
    [InlineData((ControlStyles)0, true)]
    [InlineData((ControlStyles)int.MaxValue, false)]
    [InlineData((ControlStyles)(-1), false)]
    public void DateTimePicker_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubDateTimePicker control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void DateTimePicker_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubDateTimePicker control = new();
        Assert.False(control.GetTopLevel());
    }

    [WinFormsFact]
    public void DateTimePicker_SysTimeToDateTime_DoesnThrowException_If_SYSTEMTIME_IsIncorrect()
    {
        // We expect to hit Debug.Fail in this test and unless we clear listeners we will crash to xUnit runner:
        // "The active test run was aborted. Reason: Test host process crashed : Process terminated. Assertion failed."
        using (new NoAssertContext())
        {
            // An empty SYSTEMTIME has year, month and day as 0, but DateTime can't have these parameters.
            // So an empty SYSTEMTIME is incorrect in this case.
            SYSTEMTIME systemTime = default;
            DateTime dateTime = (DateTime)systemTime;
            Assert.Equal(DateTime.MinValue, dateTime);
        }
    }

    [WinFormsFact]
    public void DateTimePicker_CustomFormat_Null_Text_ReturnsExpected()
    {
        using DateTimePicker dateTimePicker = new();
        DateTime dt = new(2000, 1, 2, 3, 4, 5);
        dateTimePicker.Value = dt;
        dateTimePicker.CreateControl();

        Assert.Null(dateTimePicker.CustomFormat);
        Assert.Equal(dt.ToLongDateString(), dateTimePicker.Text);
    }

    [WinFormsFact]
    public void DateTimePicker_CustomFormat_Null_Format_Short_Text_ReturnsExpected()
    {
        using DateTimePicker dateTimePicker = new();
        DateTime dt = new(2000, 1, 2, 3, 4, 5);
        dateTimePicker.Value = dt;
        dateTimePicker.Format = DateTimePickerFormat.Short;
        dateTimePicker.CreateControl();

        Assert.Null(dateTimePicker.CustomFormat);
        Assert.Equal(dt.ToShortDateString(), dateTimePicker.Text);
    }

    [WinFormsFact]
    public void DateTimePicker_CustomFormat_Null_Format_Time_Text_ReturnsExpected()
    {
        using DateTimePicker dateTimePicker = new();
        DateTime dt = new(2000, 1, 2, 3, 4, 5);
        dateTimePicker.Value = dt;
        dateTimePicker.Format = DateTimePickerFormat.Time;
        dateTimePicker.CreateControl();

        Assert.Null(dateTimePicker.CustomFormat);
        Assert.Equal(dt.ToLongTimeString(), dateTimePicker.Text);
    }

    [WinFormsFact]
    public void DateTimePicker_CustomFormat_LongDatePattern_Text_ReturnsExpected()
    {
        using DateTimePicker dateTimePicker = new();
        DateTime dt = new(2000, 1, 2, 3, 4, 5);
        dateTimePicker.Value = dt;
        DateTimeFormatInfo dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
        dateTimePicker.Format = DateTimePickerFormat.Custom;
        dateTimePicker.CustomFormat = dateTimeFormat.LongDatePattern;
        dateTimePicker.CreateControl();

        Assert.Equal(dt.ToLongDateString(), dateTimePicker.Text);
    }

    [WinFormsFact]
    public void DateTimePicker_CustomFormat_ShortDatePattern_Text_ReturnsExpected()
    {
        using DateTimePicker dateTimePicker = new();
        DateTime dt = new(2000, 1, 2, 3, 4, 5);
        dateTimePicker.Value = dt;
        DateTimeFormatInfo dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
        dateTimePicker.Format = DateTimePickerFormat.Custom;
        dateTimePicker.CustomFormat = dateTimeFormat.ShortDatePattern;
        dateTimePicker.CreateControl();

        Assert.Equal(dt.ToShortDateString(), dateTimePicker.Text);
    }

    [WinFormsFact]
    public void DateTimePicker_CustomFormat_LongTimePattern_Text_ReturnsExpected()
    {
        using DateTimePicker dateTimePicker = new();
        DateTime dt = new(2000, 1, 2, 3, 4, 5);
        dateTimePicker.Value = dt;
        DateTimeFormatInfo dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
        dateTimePicker.Format = DateTimePickerFormat.Custom;
        dateTimePicker.CustomFormat = dateTimeFormat.LongTimePattern;
        dateTimePicker.CreateControl();

        Assert.Equal(dt.ToLongTimeString(), dateTimePicker.Text);
    }

    [WinFormsFact]
    public void DateTimePicker_CustomFormat_ShortTimePattern_Text_ReturnsExpected()
    {
        using DateTimePicker dateTimePicker = new();
        DateTime dt = new(2000, 1, 2, 3, 4, 5);
        dateTimePicker.Value = dt;
        DateTimeFormatInfo dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
        dateTimePicker.Format = DateTimePickerFormat.Custom;
        dateTimePicker.CustomFormat = dateTimeFormat.ShortTimePattern;
        dateTimePicker.CreateControl();

        Assert.Equal(dt.ToShortTimeString(), dateTimePicker.Text);
    }

    public class SubDateTimePicker : DateTimePicker
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

        public new void OnClick(EventArgs e) => base.OnClick(e);

        public new void OnDoubleClick(EventArgs e) => base.OnDoubleClick(e);

        public new void OnTextChanged(EventArgs e) => base.OnTextChanged(e);

        public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);

        public new void OnMouseClick(MouseEventArgs e) => base.OnMouseClick(e);

        public new void OnMouseDoubleClick(MouseEventArgs e) => base.OnMouseDoubleClick(e);

        public new void OnCloseUp(EventArgs e) => base.OnCloseUp(e);

        public new void OnDropDown(EventArgs e) => base.OnDropDown(e);
    }
}
