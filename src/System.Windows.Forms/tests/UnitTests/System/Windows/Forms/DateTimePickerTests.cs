// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class DateTimePickerTests
{
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
    public void DateTimePicker_BackColorChangedEvent_Raised_Success()
    {
        using DateTimePicker control = new();
        bool eventWasRaised = false;

        control.BackColorChanged += (sender, args) => eventWasRaised = true;
        control.BackColor = Color.Red;

        eventWasRaised.Should().BeTrue();
    }

    [WinFormsFact]
    public void DateTimePicker_BackgroundImageChangedEvent_Raised_Success()
    {
        using DateTimePicker control = new();
        bool eventWasRaised = false;

        control.BackgroundImageChanged += (sender, args) => eventWasRaised = true;
        using (Bitmap bmp = new Bitmap(10, 10))
        {
            control.BackgroundImage = bmp;
        }

        eventWasRaised.Should().BeTrue();
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
            SYSTEMTIME systemTime = new();
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
        Globalization.DateTimeFormatInfo dateTimeFormat = Globalization.CultureInfo.CurrentCulture.DateTimeFormat;
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
        Globalization.DateTimeFormatInfo dateTimeFormat = Globalization.CultureInfo.CurrentCulture.DateTimeFormat;
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
        Globalization.DateTimeFormatInfo dateTimeFormat = Globalization.CultureInfo.CurrentCulture.DateTimeFormat;
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
        Globalization.DateTimeFormatInfo dateTimeFormat = Globalization.CultureInfo.CurrentCulture.DateTimeFormat;
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
    }
}
