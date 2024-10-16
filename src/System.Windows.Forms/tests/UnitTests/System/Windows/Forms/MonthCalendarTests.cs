// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms.TestUtilities;
using static System.Windows.Forms.MonthCalendar;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

[UseDefaultXunitCulture]
public class MonthCalendarTests
{
    [WinFormsFact]
    public void MonthCalendar_Ctor_Default()
    {
        using SubMonthCalendar control = new();
        Assert.Null(control.AccessibleDefaultActionDescription);
        Assert.Null(control.AccessibleDescription);
        Assert.Null(control.AccessibleName);
        Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
        Assert.False(control.AllowDrop);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.Empty(control.AnnuallyBoldedDates);
        Assert.Same(control.AnnuallyBoldedDates, control.AnnuallyBoldedDates);
        Assert.False(control.AutoSize);
        Assert.Equal(SystemColors.Window, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.Null(control.BindingContext);
        Assert.Empty(control.BoldedDates);
        Assert.Same(control.BoldedDates, control.BoldedDates);
        Assert.True(control.Bottom > 0);
        Assert.Equal(0, control.Bounds.X);
        Assert.Equal(0, control.Bounds.Y);
        Assert.True(control.Bounds.Width > 0);
        Assert.True(control.Bounds.Height > 0);
        Assert.Equal(new Size(1, 1), control.CalendarDimensions);
        Assert.False(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.True(control.CanRaiseEvents);
        Assert.True(control.CanSelect);
        Assert.False(control.Capture);
        Assert.True(control.CausesValidation);
        Assert.True(control.ClientSize.Width > 0);
        Assert.True(control.ClientSize.Height > 0);
        Assert.Equal(0, control.ClientRectangle.X);
        Assert.Equal(0, control.ClientRectangle.Y);
        Assert.True(control.ClientRectangle.Width > 0);
        Assert.True(control.ClientRectangle.Height > 0);
        Assert.Null(control.Container);
        Assert.False(control.ContainsFocus);
        Assert.Null(control.ContextMenuStrip);
        Assert.Empty(control.Controls);
        Assert.Same(control.Controls, control.Controls);
        Assert.False(control.Created);
        Assert.Same(Cursors.Default, control.Cursor);
        Assert.Same(Cursors.Default, control.DefaultCursor);
        Assert.Equal(ImeMode.Disable, control.DefaultImeMode);
        Assert.Equal(new Padding(9), control.DefaultMargin);
        Assert.Equal(Size.Empty, control.DefaultMaximumSize);
        Assert.Equal(Size.Empty, control.DefaultMinimumSize);
        Assert.Equal(Padding.Empty, control.DefaultPadding);
        Assert.True(control.DefaultSize.Width > 0);
        Assert.True(control.DefaultSize.Height > 0);
        Assert.False(control.DesignMode);
        Assert.Equal(0, control.DisplayRectangle.X);
        Assert.Equal(0, control.DisplayRectangle.Y);
        Assert.True(control.DisplayRectangle.Width > 0);
        Assert.True(control.DisplayRectangle.Height > 0);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.False(control.DoubleBuffered);
        Assert.True(control.Enabled);
        Assert.NotNull(control.Events);
        Assert.Same(control.Events, control.Events);
        Assert.Equal(Day.Default, control.FirstDayOfWeek);
        Assert.False(control.Focused);
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.Equal(SystemColors.WindowText, control.ForeColor);
        Assert.False(control.HasChildren);
        Assert.True(control.Height > 0);
        Assert.Equal(ImeMode.Disable, control.ImeMode);
        Assert.Equal(ImeMode.Disable, control.ImeModeBase);
        Assert.False(control.IsAccessible);
        Assert.False(control.IsMirrored);
        Assert.NotNull(control.LayoutEngine);
        Assert.Same(control.LayoutEngine, control.LayoutEngine);
        Assert.Equal(0, control.Left);
        Assert.Equal(Point.Empty, control.Location);
        Assert.Equal(new Padding(9), control.Margin);
        Assert.Equal(new DateTime(9998, 12, 31), control.MaxDate);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.Equal(7, control.MaxSelectionCount);
        Assert.Equal(new DateTime(1753, 1, 1), control.MinDate);
        Assert.Equal(Size.Empty, control.MinimumSize);
        Assert.Empty(control.MonthlyBoldedDates);
        Assert.Same(control.MonthlyBoldedDates, control.MonthlyBoldedDates);
        Assert.Equal(Padding.Empty, control.Padding);
        Assert.Null(control.Parent);
        Assert.True(control.PreferredSize.Width > 0);
        Assert.True(control.PreferredSize.Height > 0);
        Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.False(control.ResizeRedraw);
        Assert.True(control.Right > 0);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.False(control.RightToLeftLayout);
        Assert.Equal(0, control.ScrollChange);
        Assert.Equal(DateTime.Now.Date, control.SelectionEnd);
        Assert.Equal(DateTime.Now.Date, control.SelectionRange.Start);
        Assert.Equal(DateTime.Now.Date, control.SelectionRange.End);
        Assert.NotSame(control.SelectionRange, control.SelectionRange);
        Assert.Equal(DateTime.Now.Date, control.SelectionStart);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowKeyboardCues);
        Assert.True(control.ShowToday);
        Assert.True(control.ShowTodayCircle);
        Assert.False(control.ShowWeekNumbers);
        Assert.Equal(new Size(176, 153), control.SingleMonthSize);
        Assert.Null(control.Site);
        Assert.True(control.Size.Width > 0);
        Assert.True(control.Size.Height > 0);
        Assert.Equal(0, control.TabIndex);
        Assert.True(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(SystemColors.ActiveCaption, control.TitleBackColor);
        Assert.Equal(SystemColors.ActiveCaptionText, control.TitleForeColor);
        Assert.Equal(DateTime.Now.Date, control.TodayDate);
        Assert.False(control.TodayDateSet);
        Assert.Equal(0, control.Top);
        Assert.Null(control.TopLevelControl);
        Assert.Equal(SystemColors.GrayText, control.TrailingForeColor);
        Assert.False(control.UseWaitCursor);
        Assert.True(control.Visible);
        Assert.True(control.Width > 0);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void MonthCalendar_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubMonthCalendar control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("SysMonthCal32", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(control.Height, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x56010003, createParams.Style);
        Assert.Equal(control.Width, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> AnnuallyBoldedDates_Set_TestData()
    {
        yield return new object[] { null, Array.Empty<DateTime>() };
        yield return new object[] { Array.Empty<DateTime>(), Array.Empty<DateTime>() };

        yield return new object[] { new DateTime[] { new(2019, 01, 1), new(2019, 01, 20) }, new DateTime[] { new(2019, 01, 1), new(2019, 01, 20) } };
        yield return new object[] { new DateTime[] { new(2017, 01, 1), new(2018, 01, 20) }, new DateTime[] { new(2017, 01, 1), new(2018, 01, 20) } };
        yield return new object[] { new DateTime[] { new(2019, 01, 1), new(2019, 01, 1), new(2018, 01, 1) }, new DateTime[] { new(2019, 01, 1), new(2019, 01, 1), new(2018, 01, 1) } };
        yield return new object[] { new DateTime[] { DateTime.MinValue, DateTime.MaxValue }, new DateTime[] { DateTime.MinValue, DateTime.MaxValue } };

        var everyMonth = new DateTime[]
        {
            new(2019, 01, 1),
            new(2019, 02, 2),
            new(2019, 03, 3),
            new(2019, 04, 4),
            new(2019, 05, 5),
            new(2019, 06, 6),
            new(2019, 07, 7),
            new(2019, 08, 8),
            new(2019, 09, 9),
            new(2019, 10, 10),
            new(2019, 11, 11),
            new(2019, 12, 12),
        };
        yield return new object[] { everyMonth, everyMonth };
    }

    [WinFormsTheory]
    [MemberData(nameof(AnnuallyBoldedDates_Set_TestData))]
    public void MonthCalendar_AnnuallyBoldedDates_Set_GetReturnsExpected(DateTime[] value, DateTime[] expected)
    {
        using MonthCalendar calendar = new()
        {
            AnnuallyBoldedDates = value
        };
        Assert.Equal(expected, calendar.AnnuallyBoldedDates);
        Assert.NotSame(value, calendar.AnnuallyBoldedDates);
        if (value?.Length > 0)
        {
            Assert.NotSame(calendar.AnnuallyBoldedDates, calendar.AnnuallyBoldedDates);
        }
        else
        {
            Assert.Same(calendar.AnnuallyBoldedDates, calendar.AnnuallyBoldedDates);
        }

        Assert.False(calendar.IsHandleCreated);

        // Set same.
        calendar.AnnuallyBoldedDates = value;
        Assert.Equal(expected, calendar.AnnuallyBoldedDates);
        Assert.NotSame(value, calendar.AnnuallyBoldedDates);
        if (value?.Length > 0)
        {
            Assert.NotSame(calendar.AnnuallyBoldedDates, calendar.AnnuallyBoldedDates);
        }
        else
        {
            Assert.Same(calendar.AnnuallyBoldedDates, calendar.AnnuallyBoldedDates);
        }

        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(AnnuallyBoldedDates_Set_TestData))]
    public void MonthCalendar_AnnuallyBoldedDates_SetWithHandle_GetReturnsExpected(DateTime[] value, DateTime[] expected)
    {
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.AnnuallyBoldedDates = value;
        Assert.Equal(expected, calendar.AnnuallyBoldedDates);
        Assert.NotSame(value, calendar.AnnuallyBoldedDates);
        if (value?.Length > 0)
        {
            Assert.NotSame(calendar.AnnuallyBoldedDates, calendar.AnnuallyBoldedDates);
        }
        else
        {
            Assert.Same(calendar.AnnuallyBoldedDates, calendar.AnnuallyBoldedDates);
        }

        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        calendar.AnnuallyBoldedDates = value;
        Assert.Equal(expected, calendar.AnnuallyBoldedDates);
        Assert.NotSame(value, calendar.AnnuallyBoldedDates);
        if (value?.Length > 0)
        {
            Assert.NotSame(calendar.AnnuallyBoldedDates, calendar.AnnuallyBoldedDates);
        }
        else
        {
            Assert.Same(calendar.AnnuallyBoldedDates, calendar.AnnuallyBoldedDates);
        }

        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> BackColor_Set_TestData()
    {
        yield return new object[] { Color.Empty, SystemColors.Window };
        yield return new object[] { Color.Red, Color.Red };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_Set_TestData))]
    public void MonthCalendar_BackColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using MonthCalendar control = new()
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

    public static IEnumerable<object[]> BackColor_SetWithHandle_TestData()
    {
        yield return new object[] { Color.Empty, SystemColors.Window, 0 };
        yield return new object[] { Color.Red, Color.Red, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_SetWithHandle_TestData))]
    public void MonthCalendar_BackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
    {
        using MonthCalendar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void MonthCalendar_BackColor_SetWithHandler_CallsBackColorChanged()
    {
        using MonthCalendar control = new();
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
        Assert.Equal(SystemColors.Window, control.BackColor);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.BackColorChanged -= handler;
        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void MonthCalendar_BackgroundImage_Set_GetReturnsExpected(Image value)
    {
        using MonthCalendar control = new()
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
    public void MonthCalendar_BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
    {
        using MonthCalendar control = new();
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
    public void MonthCalendar_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
    {
        using SubMonthCalendar control = new()
        {
            BackgroundImageLayout = value
        };
        Assert.Equal(value, control.BackgroundImageLayout);
        Assert.False(control.DoubleBuffered);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackgroundImageLayout = value;
        Assert.Equal(value, control.BackgroundImageLayout);
        Assert.False(control.DoubleBuffered);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void MonthCalendar_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
    {
        using MonthCalendar control = new();
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
    [InvalidEnumData<ImageLayout>]
    public void MonthCalendar_BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
    {
        using MonthCalendar control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.BackgroundImageLayout = value);
    }

    public static IEnumerable<object[]> BoldedDates_Set_TestData()
    {
        yield return new object[] { null, Array.Empty<DateTime>() };
        yield return new object[] { Array.Empty<DateTime>(), Array.Empty<DateTime>() };

        yield return new object[] { new DateTime[] { new(2019, 01, 1), new(2019, 01, 20) }, new DateTime[] { new(2019, 01, 1), new(2019, 01, 20) } };
        yield return new object[] { new DateTime[] { new(2017, 01, 1), new(2018, 01, 20) }, new DateTime[] { new(2017, 01, 1), new(2018, 01, 20) } };
        yield return new object[] { new DateTime[] { new(2019, 01, 1), new(2019, 01, 1), new(2018, 01, 1) }, new DateTime[] { new(2019, 01, 1), new(2019, 01, 1), new(2018, 01, 1) } };
        yield return new object[] { new DateTime[] { DateTime.MinValue, DateTime.MaxValue }, new DateTime[] { DateTime.MinValue, DateTime.MaxValue } };

        var everyMonth = new DateTime[]
        {
            new(2019, 01, 1),
            new(2019, 02, 2),
            new(2019, 03, 3),
            new(2019, 04, 4),
            new(2019, 05, 5),
            new(2019, 06, 6),
            new(2019, 07, 7),
            new(2019, 08, 8),
            new(2019, 09, 9),
            new(2019, 10, 10),
            new(2019, 11, 11),
            new(2019, 12, 12),
        };
        yield return new object[] { everyMonth, everyMonth };
    }

    [WinFormsTheory]
    [MemberData(nameof(BoldedDates_Set_TestData))]
    public void MonthCalendar_BoldedDates_Set_GetReturnsExpected(DateTime[] value, DateTime[] expected)
    {
        using MonthCalendar calendar = new()
        {
            BoldedDates = value
        };
        Assert.Equal(expected, calendar.BoldedDates);
        Assert.NotSame(value, calendar.BoldedDates);
        if (value?.Length > 0)
        {
            Assert.NotSame(calendar.BoldedDates, calendar.BoldedDates);
        }
        else
        {
            Assert.Same(calendar.BoldedDates, calendar.BoldedDates);
        }

        Assert.False(calendar.IsHandleCreated);

        // Set same.
        calendar.BoldedDates = value;
        Assert.Equal(expected, calendar.BoldedDates);
        Assert.NotSame(value, calendar.BoldedDates);
        if (value?.Length > 0)
        {
            Assert.NotSame(calendar.BoldedDates, calendar.BoldedDates);
        }
        else
        {
            Assert.Same(calendar.BoldedDates, calendar.BoldedDates);
        }

        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(BoldedDates_Set_TestData))]
    public void MonthCalendar_BoldedDates_SetWithHandle_GetReturnsExpected(DateTime[] value, DateTime[] expected)
    {
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.BoldedDates = value;
        Assert.Equal(expected, calendar.BoldedDates);
        Assert.NotSame(value, calendar.BoldedDates);
        if (value?.Length > 0)
        {
            Assert.NotSame(calendar.BoldedDates, calendar.BoldedDates);
        }
        else
        {
            Assert.Same(calendar.BoldedDates, calendar.BoldedDates);
        }

        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        calendar.BoldedDates = value;
        Assert.Equal(expected, calendar.BoldedDates);
        Assert.NotSame(value, calendar.BoldedDates);
        if (value?.Length > 0)
        {
            Assert.NotSame(calendar.BoldedDates, calendar.BoldedDates);
        }
        else
        {
            Assert.Same(calendar.BoldedDates, calendar.BoldedDates);
        }

        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> CalendarDimensions_Set_TestData()
    {
        yield return new object[] { new Size(1, 1), new Size(1, 1) };
        yield return new object[] { new Size(1, 2), new Size(1, 2) };
        yield return new object[] { new Size(2, 1), new Size(2, 1) };
        yield return new object[] { new Size(2, 3), new Size(2, 3) };
        yield return new object[] { new Size(3, 4), new Size(3, 4) };
        yield return new object[] { new Size(3, 5), new Size(3, 4) };
        yield return new object[] { new Size(3, 10), new Size(3, 4) };
        yield return new object[] { new Size(5, 3), new Size(4, 3) };
        yield return new object[] { new Size(10, 3), new Size(4, 3) };
    }

    [WinFormsTheory]
    [MemberData(nameof(CalendarDimensions_Set_TestData))]
    public void MonthCalendar_CalendarDimensions_Set_GetReturnsExpected(Size value, Size expected)
    {
        using MonthCalendar calendar = new()
        {
            CalendarDimensions = value
        };
        Assert.Equal(expected, calendar.CalendarDimensions);
        Assert.False(calendar.IsHandleCreated);

        // Set same.
        calendar.CalendarDimensions = value;
        Assert.Equal(expected, calendar.CalendarDimensions);
        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(1, 12)]
    [InlineData(12, 1)]
    public void MonthCalendar_CalendarDimensions_SetAreaOfTwelve_GetReturnsExpected(int width, int height)
    {
        Size value = new(width, height);
        using MonthCalendar calendar = new()
        {
            CalendarDimensions = value
        };
        Assert.True(calendar.CalendarDimensions.Width is > 0 and <= 12);
        Assert.True(calendar.CalendarDimensions.Height is > 0 and <= 12);
        Assert.False(calendar.IsHandleCreated);

        // Set same.
        var previousCalendarDimensions = calendar.CalendarDimensions;
        calendar.CalendarDimensions = value;
        Assert.True(calendar.CalendarDimensions.Width is > 0 and <= 12);
        Assert.True(calendar.CalendarDimensions.Height is > 0 and <= 12);
        Assert.Equal(previousCalendarDimensions, calendar.CalendarDimensions);
        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(CalendarDimensions_Set_TestData))]
    public void MonthCalendar_CalendarDimensions_SetWithHandle_GetReturnsExpected(Size value, Size expected)
    {
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.CalendarDimensions = value;
        Assert.Equal(expected, calendar.CalendarDimensions);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        calendar.CalendarDimensions = value;
        Assert.Equal(expected, calendar.CalendarDimensions);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(1, 12)]
    [InlineData(12, 1)]
    public void MonthCalendar_CalendarDimensions_SetWithHandleAreaOfTwelve_GetReturnsExpected(int width, int height)
    {
        Size value = new(width, height);
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.CalendarDimensions = value;
        Assert.True(calendar.CalendarDimensions.Width is > 0 and <= 12);
        Assert.True(calendar.CalendarDimensions.Height is > 0 and <= 12);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        var previousCalendarDimensions = calendar.CalendarDimensions;
        calendar.CalendarDimensions = value;
        Assert.True(calendar.CalendarDimensions.Width is > 0 and <= 12);
        Assert.True(calendar.CalendarDimensions.Height is > 0 and <= 12);
        Assert.Equal(previousCalendarDimensions, calendar.CalendarDimensions);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(-1)]
    public void MonthCalendar_CalendarDimensions_SetNegativeX_ThrowsArgumentOutOfRangeException(int x)
    {
        using MonthCalendar calendar = new();
        Assert.Throws<ArgumentOutOfRangeException>("x", () => calendar.CalendarDimensions = new Size(x, 1));
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(-1)]
    public void MonthCalendar_CalendarDimensions_SetNegativeY_ThrowsArgumentOutOfRangeException(int y)
    {
        using MonthCalendar calendar = new();
        Assert.Throws<ArgumentOutOfRangeException>("y", () => calendar.CalendarDimensions = new Size(1, y));
    }

    [WinFormsTheory]
    [BoolData]
    public void MonthCalendar_DoubleBuffered_Set_GetReturnsExpected(bool value)
    {
        using SubMonthCalendar control = new()
        {
            DoubleBuffered = value
        };
        Assert.Equal(value, control.DoubleBuffered);
        Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.DoubleBuffered = value;
        Assert.Equal(value, control.DoubleBuffered);
        Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.DoubleBuffered = !value;
        Assert.Equal(!value, control.DoubleBuffered);
        Assert.Equal(!value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void MonthCalendar_DoubleBuffered_SetWithHandle_GetReturnsExpected(bool value)
    {
        using SubMonthCalendar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.DoubleBuffered = value;
        Assert.Equal(value, control.DoubleBuffered);
        Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.DoubleBuffered = value;
        Assert.Equal(value, control.DoubleBuffered);
        Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.DoubleBuffered = !value;
        Assert.Equal(!value, control.DoubleBuffered);
        Assert.Equal(!value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [EnumData<Day>]
    public void MonthCalendar_FirstDayOfWeek_Set_GetReturnsExpected(Day value)
    {
        using MonthCalendar calendar = new()
        {
            FirstDayOfWeek = value
        };
        Assert.Equal(value, calendar.FirstDayOfWeek);
        Assert.False(calendar.IsHandleCreated);

        // Set same.
        calendar.FirstDayOfWeek = value;
        Assert.Equal(value, calendar.FirstDayOfWeek);
        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<Day>]
    public void MonthCalendar_FirstDayOfWeek_SetWithCustomOldValue_GetReturnsExpected(Day value)
    {
        using MonthCalendar calendar = new()
        {
            FirstDayOfWeek = Day.Monday
        };

        calendar.FirstDayOfWeek = value;
        Assert.Equal(value, calendar.FirstDayOfWeek);
        Assert.False(calendar.IsHandleCreated);

        // Set same.
        calendar.FirstDayOfWeek = value;
        Assert.Equal(value, calendar.FirstDayOfWeek);
        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<Day>]
    public void MonthCalendar_FirstDayOfWeek_SetWithHandle_GetReturnsExpected(Day value)
    {
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.FirstDayOfWeek = value;
        Assert.Equal(value, calendar.FirstDayOfWeek);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        calendar.FirstDayOfWeek = value;
        Assert.Equal(value, calendar.FirstDayOfWeek);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(Day.Default, 1)]
    [InlineData(Day.Monday, 0)]
    [InlineData(Day.Tuesday, 0)]
    [InlineData(Day.Wednesday, 0)]
    [InlineData(Day.Thursday, 0)]
    [InlineData(Day.Friday, 0)]
    [InlineData(Day.Saturday, 0)]
    [InlineData(Day.Sunday, 0)]
    public void MonthCalendar_FirstDayOfWeek_SetWithHandleWithCustomOldValue_GetReturnsExpected(Day value, int expectedCreatedCallCount)
    {
        using MonthCalendar calendar = new()
        {
            FirstDayOfWeek = Day.Monday
        };
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.FirstDayOfWeek = value;
        Assert.Equal(value, calendar.FirstDayOfWeek);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        calendar.FirstDayOfWeek = value;
        Assert.Equal(value, calendar.FirstDayOfWeek);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<Day>]
    public void MonthCalendar_FirstDayOfWeek_SetInvalidValue_ThrowsInvalidEnumArgumentException(Day value)
    {
        using MonthCalendar calendar = new();
        Assert.Throws<InvalidEnumArgumentException>("FirstDayOfWeek", () => calendar.FirstDayOfWeek = value);
    }

    public static IEnumerable<object[]> ForeColor_Set_TestData()
    {
        yield return new object[] { Color.Empty, SystemColors.WindowText };
        yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3) };
        yield return new object[] { Color.White, Color.White };
        yield return new object[] { Color.Black, Color.Black };
        yield return new object[] { Color.Red, Color.Red };
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_Set_TestData))]
    public void MonthCalendar_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using MonthCalendar control = new()
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
        yield return new object[] { Color.Empty, SystemColors.WindowText, 0 };
        yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3), 1 };
        yield return new object[] { Color.White, Color.White, 1 };
        yield return new object[] { Color.Black, Color.Black, 1 };
        yield return new object[] { Color.Red, Color.Red, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_SetWithHandle_TestData))]
    public void MonthCalendar_ForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
    {
        using MonthCalendar control = new();
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
    public void MonthCalendar_ForeColor_SetWithHandler_CallsForeColorChanged()
    {
        using MonthCalendar control = new();
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
        Assert.Equal(SystemColors.WindowText, control.ForeColor);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ForeColorChanged -= handler;
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void MonthCalendar_Handle_GetWithSelectionRange_Success()
    {
        DateTime lower = new(2019, 1, 30, 3, 4, 5, 6);
        DateTime upper = new(2019, 2, 3, 4, 5, 6, 7);
        using SubMonthCalendar control = new()
        {
            SelectionRange = new SelectionRange(lower, upper)
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Span<SYSTEMTIME> range = stackalloc SYSTEMTIME[2];
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.MCM_GETSELRANGE, 0, ref range[0]));
        Assert.Equal(2019, range[0].wYear);
        Assert.Equal(1, range[0].wMonth);
        Assert.Equal(30, range[0].wDay);
        Assert.Equal(3, range[0].wDayOfWeek);
        Assert.True(range[0].wHour >= 0);
        Assert.True(range[0].wMinute >= 0);
        Assert.True(range[0].wSecond >= 0);
        Assert.True(range[0].wMilliseconds >= 0);
        Assert.Equal(2019, range[1].wYear);
        Assert.Equal(2, range[1].wMonth);
        Assert.Equal(3, range[1].wDay);
        Assert.Equal(0, range[1].wDayOfWeek);
        Assert.True(range[1].wHour >= 0);
        Assert.True(range[1].wMinute >= 0);
        Assert.True(range[1].wSecond >= 0);
        Assert.True(range[1].wMilliseconds >= 0);
    }

    [WinFormsFact]
    public void MonthCalendar_Handle_GetWithMaxSelectionCount_Success()
    {
        using SubMonthCalendar control = new()
        {
            MaxSelectionCount = 10
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(10, (int)PInvokeCore.SendMessage(control, PInvoke.MCM_GETMAXSELCOUNT));
    }

    [WinFormsFact]
    public void MonthCalendar_Handle_GetWithTodayDate_Success()
    {
        using SubMonthCalendar control = new()
        {
            TodayDate = new DateTime(2019, 1, 30, 3, 4, 5, 6)
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        SYSTEMTIME date = default;
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.MCM_GETTODAY, 0, ref date));
        Assert.Equal(2019, date.wYear);
        Assert.Equal(1, date.wMonth);
        Assert.Equal(30, date.wDay);
        Assert.Equal(3, date.wDayOfWeek);
        Assert.Equal(0, date.wHour);
        Assert.Equal(0, date.wMinute);
        Assert.Equal(0, date.wSecond);
        Assert.Equal(0, date.wMilliseconds);
    }

    [WinFormsFact]
    public void MonthCalendar_Handle_GetWithForeColor_Success()
    {
        using SubMonthCalendar control = new()
        {
            ForeColor = Color.FromArgb(0x12, 0x34, 0x56, 0x78)
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(0x785634, (int)PInvokeCore.SendMessage(control, PInvoke.MCM_GETCOLOR, (WPARAM)(int)PInvoke.MCSC_TEXT));
    }

    [WinFormsFact]
    public void MonthCalendar_Handle_GetWithBackColor_Success()
    {
        using SubMonthCalendar control = new()
        {
            BackColor = Color.FromArgb(0xFF, 0x12, 0x34, 0x56)
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(0x563412, (int)PInvokeCore.SendMessage(control, PInvoke.MCM_GETCOLOR, (WPARAM)(int)PInvoke.MCSC_MONTHBK));
    }

    [WinFormsFact]
    public void MonthCalendar_Handle_GetWithTitleBackColor_Success()
    {
        using SubMonthCalendar control = new()
        {
            TitleBackColor = Color.FromArgb(0x12, 0x34, 0x56, 0x78)
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(0x785634, (int)PInvokeCore.SendMessage(control, PInvoke.MCM_GETCOLOR, (WPARAM)(int)PInvoke.MCSC_TITLEBK));
    }

    [WinFormsFact]
    public void MonthCalendar_Handle_GetWithTitleForeColor_Success()
    {
        using SubMonthCalendar control = new()
        {
            TitleForeColor = Color.FromArgb(0x12, 0x34, 0x56, 0x78)
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(0x785634, (int)PInvokeCore.SendMessage(control, PInvoke.MCM_GETCOLOR, (WPARAM)(int)PInvoke.MCSC_TITLETEXT));
    }

    [WinFormsFact]
    public void MonthCalendar_Handle_GetWithTrailingForeColor_Success()
    {
        using SubMonthCalendar control = new()
        {
            TrailingForeColor = Color.FromArgb(0x12, 0x34, 0x56, 0x78)
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(0x785634, (int)PInvokeCore.SendMessage(control, PInvoke.MCM_GETCOLOR, (WPARAM)(int)PInvoke.MCSC_TRAILINGTEXT));
    }

    [WinFormsFact]
    public void MonthCalendar_Handle_GetWithDefaultFirstDayOfWeek_Success()
    {
        using SubMonthCalendar control = new()
        {
            FirstDayOfWeek = Day.Default
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int expected = (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek + 6;
        while (expected > 6)
        {
            expected -= 7;
        }

        Assert.Equal(expected, (int)PInvokeCore.SendMessage(control, PInvoke.MCM_GETFIRSTDAYOFWEEK));
    }

    [WinFormsFact]
    public void MonthCalendar_Handle_GetWithFirstDayOfWeek_Success()
    {
        using SubMonthCalendar control = new()
        {
            FirstDayOfWeek = Day.Tuesday
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(0x10001, (int)PInvokeCore.SendMessage(control, PInvoke.MCM_GETFIRSTDAYOFWEEK));
    }

    [WinFormsFact]
    public void MonthCalendar_Handle_GetWithRange_Success()
    {
        using SubMonthCalendar control = new()
        {
            MinDate = new DateTime(2019, 1, 2, 3, 4, 5, 6),
            MaxDate = new DateTime(2020, 2, 3, 4, 5, 6, 7)
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Span<SYSTEMTIME> range = stackalloc SYSTEMTIME[2];
        Assert.Equal(3, (int)PInvokeCore.SendMessage(control, PInvoke.MCM_GETRANGE, 0, ref range[0]));
        Assert.Equal(2019, range[0].wYear);
        Assert.Equal(1, range[0].wMonth);
        Assert.Equal(2, range[0].wDay);
        Assert.Equal(3, range[0].wDayOfWeek);
        Assert.Equal(3, range[0].wHour);
        Assert.Equal(4, range[0].wMinute);
        Assert.Equal(5, range[0].wSecond);
        Assert.Equal(6, range[0].wMilliseconds);
        Assert.Equal(2020, range[1].wYear);
        Assert.Equal(2, range[1].wMonth);
        Assert.Equal(3, range[1].wDay);
        Assert.Equal(1, range[1].wDayOfWeek);
        Assert.Equal(4, range[1].wHour);
        Assert.Equal(5, range[1].wMinute);
        Assert.Equal(6, range[1].wSecond);
        Assert.Equal(7, range[1].wMilliseconds);
    }

    [WinFormsFact]
    public void MonthCalendar_Handle_GetWithScrollChange_Success()
    {
        using SubMonthCalendar control = new()
        {
            ScrollChange = 10
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(10, (int)PInvokeCore.SendMessage(control, PInvoke.MCM_GETMONTHDELTA));
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
    public void MonthCalendar_ImeMode_Set_GetReturnsExpected(ImeMode value, ImeMode expected)
    {
        using MonthCalendar control = new()
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
    public void MonthCalendar_ImeMode_SetWithHandle_GetReturnsExpected(ImeMode value, ImeMode expected)
    {
        using MonthCalendar control = new();
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
    public void MonthCalendar_ImeMode_SetWithHandler_CallsImeModeChanged()
    {
        using MonthCalendar control = new();
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
    public void MonthCalendar_ImeMode_SetInvalid_ThrowsInvalidEnumArgumentException(ImeMode value)
    {
        using MonthCalendar control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.ImeMode = value);
    }

    public static IEnumerable<object[]> MaxDate_Set_TestData()
    {
        yield return new object[] { new DateTime(1753, 1, 1), new DateTime(1753, 1, 1), new DateTime(1753, 1, 1) };
        yield return new object[] { new DateTime(2019, 1, 29), new DateTime(2019, 1, 29), new DateTime(2019, 1, 29) };
        yield return new object[] { new DateTime(9998, 12, 31), new DateTime(9998, 12, 31), DateTime.Now.Date };
        yield return new object[] { new DateTime(9999, 1, 1), new DateTime(9998, 12, 31), DateTime.Now.Date };
        yield return new object[] { DateTime.MaxValue, new DateTime(9998, 12, 31), DateTime.Now.Date };
    }

    [WinFormsTheory]
    [MemberData(nameof(MaxDate_Set_TestData))]
    public void MonthCalendar_MaxDate_Set_GetReturnsExpected(DateTime value, DateTime expected, DateTime expectedSelection)
    {
        using MonthCalendar calendar = new()
        {
            MaxDate = value
        };
        Assert.Equal(expected, calendar.MaxDate);
        Assert.Equal(expectedSelection, calendar.SelectionStart);
        Assert.Equal(expectedSelection, calendar.SelectionEnd);
        Assert.False(calendar.IsHandleCreated);

        // Set same.
        calendar.MaxDate = value;
        Assert.Equal(expected, calendar.MaxDate);
        Assert.Equal(expectedSelection, calendar.SelectionStart);
        Assert.Equal(expectedSelection, calendar.SelectionEnd);
        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(MaxDate_Set_TestData))]
    public void MonthCalendar_MaxDate_SetWithHandle_GetReturnsExpected(DateTime value, DateTime expected, DateTime expectedSelection)
    {
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.MaxDate = value;
        Assert.Equal(expected, calendar.MaxDate);
        Assert.Equal(expectedSelection, calendar.SelectionStart);
        Assert.Equal(expectedSelection, calendar.SelectionEnd);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        calendar.MaxDate = value;
        Assert.Equal(expected, calendar.MaxDate);
        Assert.Equal(expectedSelection, calendar.SelectionStart);
        Assert.Equal(expectedSelection, calendar.SelectionEnd);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void MonthCalendar_MaxDate_SetLessThanMinDate_ThrowsArgumentOutOfRangeException()
    {
        using MonthCalendar calendar = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => calendar.MaxDate = calendar.MinDate.AddTicks(-1));
    }

    public static IEnumerable<object[]> MaxSelectionCount_Set_TestData()
    {
        yield return new object[] { 1 };
        yield return new object[] { 2 };
        yield return new object[] { 7 };
        yield return new object[] { 8 };
        yield return new object[] { int.MaxValue };
    }

    [WinFormsTheory]
    [MemberData(nameof(MaxSelectionCount_Set_TestData))]
    public void MonthCalendar_MaxSelectionCount_Set_GetReturnsExpected(int value)
    {
        using MonthCalendar calendar = new()
        {
            MaxSelectionCount = value
        };
        Assert.Equal(value, calendar.MaxSelectionCount);
        Assert.False(calendar.IsHandleCreated);

        // Set same.
        calendar.MaxSelectionCount = value;
        Assert.Equal(value, calendar.MaxSelectionCount);
        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(MaxSelectionCount_Set_TestData))]
    public void MonthCalendar_MaxSelectionCount_SetWithHandle_GetReturnsExpected(int value)
    {
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.MaxSelectionCount = value;
        Assert.Equal(value, calendar.MaxSelectionCount);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        calendar.MaxSelectionCount = value;
        Assert.Equal(value, calendar.MaxSelectionCount);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(-1)]
    public void MonthCalendar_MaxSelectionCount_SetLessThanOne_ThrowsArgumentOutOfRangeException(int value)
    {
        using MonthCalendar calendar = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => calendar.MaxSelectionCount = value);
    }

    public static IEnumerable<object[]> MinDate_Set_TestData()
    {
        yield return new object[] { DateTime.MinValue, new DateTime(1753, 1, 1), DateTime.Now.Date };
        yield return new object[] { new DateTime(1753, 1, 1), new DateTime(1753, 1, 1), DateTime.Now.Date };
        yield return new object[] { new DateTime(2019, 1, 29), new DateTime(2019, 1, 29), DateTime.Now.Date };
        yield return new object[] { new DateTime(9998, 12, 31), new DateTime(9998, 12, 31), new DateTime(9998, 12, 31) };
    }

    [WinFormsTheory]
    [MemberData(nameof(MinDate_Set_TestData))]
    public void MonthCalendar_MinDate_Set_GetReturnsExpected(DateTime value, DateTime expected, DateTime expectedSelection)
    {
        using MonthCalendar calendar = new()
        {
            MinDate = value
        };
        Assert.Equal(expected, calendar.MinDate);
        Assert.Equal(expectedSelection, calendar.SelectionStart);
        Assert.Equal(expectedSelection, calendar.SelectionEnd);
        Assert.False(calendar.IsHandleCreated);

        // Set same.
        calendar.MinDate = value;
        Assert.Equal(expected, calendar.MinDate);
        Assert.Equal(expectedSelection, calendar.SelectionStart);
        Assert.Equal(expectedSelection, calendar.SelectionEnd);
        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(MinDate_Set_TestData))]
    public void MonthCalendar_MinDate_SetWithHandle_GetReturnsExpected(DateTime value, DateTime expected, DateTime expectedSelection)
    {
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.MinDate = value;
        Assert.Equal(expected, calendar.MinDate);
        Assert.Equal(expectedSelection, calendar.SelectionStart);
        Assert.Equal(expectedSelection, calendar.SelectionEnd);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        calendar.MinDate = value;
        Assert.Equal(expected, calendar.MinDate);
        Assert.Equal(expectedSelection, calendar.SelectionStart);
        Assert.Equal(expectedSelection, calendar.SelectionEnd);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void MonthCalendar_MinDate_SetLessThanMaxDate_ThrowsArgumentOutOfRangeException()
    {
        using MonthCalendar calendar = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => calendar.MinDate = calendar.MaxDate.AddTicks(1));
    }

    [WinFormsFact]
    public void MonthCalendar_MinDate_SetLessThanMinDate_ThrowsArgumentOutOfRangeException()
    {
        using MonthCalendar calendar = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => calendar.MinDate = calendar.MinDate.AddTicks(-1));
    }

    public static IEnumerable<object[]> MonthlyBoldedDates_Set_TestData()
    {
        yield return new object[] { null, Array.Empty<DateTime>() };
        yield return new object[] { Array.Empty<DateTime>(), Array.Empty<DateTime>() };

        yield return new object[] { new DateTime[] { new(2019, 01, 1), new(2019, 01, 20) }, new DateTime[] { new(2019, 01, 1), new(2019, 01, 20) } };
        yield return new object[] { new DateTime[] { new(2017, 01, 1), new(2018, 01, 20) }, new DateTime[] { new(2017, 01, 1), new(2018, 01, 20) } };
        yield return new object[] { new DateTime[] { new(2019, 01, 1), new(2019, 01, 1), new(2018, 01, 1) }, new DateTime[] { new(2019, 01, 1), new(2019, 01, 1), new(2018, 01, 1) } };
        yield return new object[] { new DateTime[] { DateTime.MinValue, DateTime.MaxValue }, new DateTime[] { DateTime.MinValue, DateTime.MaxValue } };

        var everyMonth = new DateTime[]
        {
            new(2019, 01, 1),
            new(2019, 02, 2),
            new(2019, 03, 3),
            new(2019, 04, 4),
            new(2019, 05, 5),
            new(2019, 06, 6),
            new(2019, 07, 7),
            new(2019, 08, 8),
            new(2019, 09, 9),
            new(2019, 10, 10),
            new(2019, 11, 11),
            new(2019, 12, 12),
        };
        yield return new object[] { everyMonth, everyMonth };
    }

    [WinFormsTheory]
    [MemberData(nameof(MonthlyBoldedDates_Set_TestData))]
    public void MonthCalendar_MonthlyBoldedDates_Set_GetReturnsExpected(DateTime[] value, DateTime[] expected)
    {
        using MonthCalendar calendar = new()
        {
            MonthlyBoldedDates = value
        };
        Assert.Equal(expected, calendar.MonthlyBoldedDates);
        Assert.NotSame(value, calendar.MonthlyBoldedDates);
        if (value?.Length > 0)
        {
            Assert.NotSame(calendar.MonthlyBoldedDates, calendar.MonthlyBoldedDates);
        }
        else
        {
            Assert.Same(calendar.MonthlyBoldedDates, calendar.MonthlyBoldedDates);
        }

        Assert.False(calendar.IsHandleCreated);

        // Set same.
        calendar.MonthlyBoldedDates = value;
        Assert.Equal(expected, calendar.MonthlyBoldedDates);
        Assert.NotSame(value, calendar.MonthlyBoldedDates);
        if (value?.Length > 0)
        {
            Assert.NotSame(calendar.MonthlyBoldedDates, calendar.MonthlyBoldedDates);
        }
        else
        {
            Assert.Same(calendar.MonthlyBoldedDates, calendar.MonthlyBoldedDates);
        }

        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(MonthlyBoldedDates_Set_TestData))]
    public void MonthCalendar_MonthlyBoldedDates_SetWithHandle_GetReturnsExpected(DateTime[] value, DateTime[] expected)
    {
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.MonthlyBoldedDates = value;
        Assert.Equal(expected, calendar.MonthlyBoldedDates);
        Assert.NotSame(value, calendar.MonthlyBoldedDates);
        if (value?.Length > 0)
        {
            Assert.NotSame(calendar.MonthlyBoldedDates, calendar.MonthlyBoldedDates);
        }
        else
        {
            Assert.Same(calendar.MonthlyBoldedDates, calendar.MonthlyBoldedDates);
        }

        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        calendar.MonthlyBoldedDates = value;
        Assert.Equal(expected, calendar.MonthlyBoldedDates);
        Assert.NotSame(value, calendar.MonthlyBoldedDates);
        if (value?.Length > 0)
        {
            Assert.NotSame(calendar.MonthlyBoldedDates, calendar.MonthlyBoldedDates);
        }
        else
        {
            Assert.Same(calendar.MonthlyBoldedDates, calendar.MonthlyBoldedDates);
        }

        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingNormalizedTheoryData))]
    public void MonthCalendar_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
    {
        using MonthCalendar control = new()
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
    public void MonthCalendar_Padding_SetWithHandle_GetReturnsExpected(Padding value, Padding expected)
    {
        using MonthCalendar control = new();
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
    public void MonthCalendar_Padding_SetWithHandler_CallsPaddingChanged()
    {
        using MonthCalendar control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Equal(control, sender);
            Assert.Equal(EventArgs.Empty, e);
            callCount++;
        };
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
    [InlineData(RightToLeft.Yes, true, 1)]
    [InlineData(RightToLeft.Yes, false, 0)]
    [InlineData(RightToLeft.No, true, 1)]
    [InlineData(RightToLeft.No, false, 0)]
    [InlineData(RightToLeft.Inherit, true, 1)]
    [InlineData(RightToLeft.Inherit, false, 0)]
    public void MonthCalendar_RightToLeftLayout_Set_GetReturnsExpected(RightToLeft rightToLeft, bool value, int expectedLayoutCallCount)
    {
        using MonthCalendar control = new()
        {
            RightToLeft = rightToLeft
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("RightToLeftLayout", e.AffectedProperty);
            layoutCallCount++;
        };

        control.RightToLeftLayout = value;
        Assert.Equal(value, control.RightToLeftLayout);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.RightToLeftLayout = value;
        Assert.Equal(value, control.RightToLeftLayout);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.RightToLeftLayout = !value;
        Assert.Equal(!value, control.RightToLeftLayout);
        Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.Yes, true, 1, 1, 2)]
    [InlineData(RightToLeft.Yes, false, 0, 0, 1)]
    [InlineData(RightToLeft.No, true, 1, 0, 0)]
    [InlineData(RightToLeft.No, false, 0, 0, 0)]
    [InlineData(RightToLeft.Inherit, true, 1, 0, 0)]
    [InlineData(RightToLeft.Inherit, false, 0, 0, 0)]
    public void MonthCalendar_RightToLeftLayout_SetWithHandle_GetReturnsExpected(RightToLeft rightToLeft, bool value, int expectedLayoutCallCount, int expectedCreatedCallCount1, int expectedCreatedCallCount2)
    {
        using MonthCalendar control = new()
        {
            RightToLeft = rightToLeft
        };
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
            Assert.Equal("RightToLeftLayout", e.AffectedProperty);
            layoutCallCount++;
        };

        control.RightToLeftLayout = value;
        Assert.Equal(value, control.RightToLeftLayout);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount1, createdCallCount);

        // Set same.
        control.RightToLeftLayout = value;
        Assert.Equal(value, control.RightToLeftLayout);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount1, createdCallCount);

        // Set different.
        control.RightToLeftLayout = !value;
        Assert.Equal(!value, control.RightToLeftLayout);
        Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount2, createdCallCount);
    }

    [WinFormsFact]
    public void MonthCalendar_RightToLeftLayout_SetWithHandler_CallsRightToLeftLayoutChanged()
    {
        using MonthCalendar control = new()
        {
            RightToLeftLayout = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.RightToLeftLayoutChanged += handler;

        // Set different.
        control.RightToLeftLayout = false;
        Assert.False(control.RightToLeftLayout);
        Assert.Equal(1, callCount);

        // Set same.
        control.RightToLeftLayout = false;
        Assert.False(control.RightToLeftLayout);
        Assert.Equal(1, callCount);

        // Set different.
        control.RightToLeftLayout = true;
        Assert.True(control.RightToLeftLayout);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.RightToLeftLayoutChanged -= handler;
        control.RightToLeftLayout = false;
        Assert.False(control.RightToLeftLayout);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void MonthCalendar_RightToLeftLayout_SetWithHandlerInDisposing_DoesNotRightToLeftLayoutChanged()
    {
        using MonthCalendar control = new()
        {
            RightToLeft = RightToLeft.Yes
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        int callCount = 0;
        control.RightToLeftLayoutChanged += (sender, e) => callCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int disposedCallCount = 0;
        control.Disposed += (sender, e) =>
        {
            control.RightToLeftLayout = true;
            Assert.True(control.RightToLeftLayout);
            Assert.Equal(0, callCount);
            Assert.Equal(0, createdCallCount);
            disposedCallCount++;
        };

        control.Dispose();
        Assert.Equal(1, disposedCallCount);
    }

    public static IEnumerable<object[]> ScrollChange_Set_TestData()
    {
        yield return new object[] { 0 };
        yield return new object[] { 1 };
        yield return new object[] { 2 };
        yield return new object[] { 20000 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ScrollChange_Set_TestData))]
    public void MonthCalendar_ScrollChange_Set_GetReturnsExpected(int value)
    {
        using MonthCalendar calendar = new()
        {
            ScrollChange = value
        };
        Assert.Equal(value, calendar.ScrollChange);
        Assert.False(calendar.IsHandleCreated);

        // Set same.
        calendar.ScrollChange = value;
        Assert.Equal(value, calendar.ScrollChange);
        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ScrollChange_Set_TestData))]
    public void MonthCalendar_ScrollChange_SetWithHandle_GetReturnsExpected(int value)
    {
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.ScrollChange = value;
        Assert.Equal(value, calendar.ScrollChange);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        calendar.ScrollChange = value;
        Assert.Equal(value, calendar.ScrollChange);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(200001)]
    public void MonthCalendar_ScrollChange_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
    {
        using MonthCalendar calendar = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => calendar.ScrollChange = value);
    }

    public static IEnumerable<object[]> SelectionStart_Set_TestData()
    {
        yield return new object[] { DateTime.MinValue, new DateTime(1, 1, 7) };
        yield return new object[] { new DateTime(1753, 1, 1), new DateTime(1753, 1, 7) };
        yield return new object[] { new DateTime(1753, 1, 1).AddHours(1), new DateTime(1753, 1, 7).AddHours(1) };
        yield return new object[] { new DateTime(2019, 1, 29), new DateTime(2019, 2, 4) };
        yield return new object[] { DateTime.Now.Date.AddDays(-1), DateTime.Now.Date };
        yield return new object[] { DateTime.Now.Date, DateTime.Now.Date };
        yield return new object[] { new DateTime(9998, 12, 31), new DateTime(9998, 12, 31) };
        yield return new object[] { DateTime.MaxValue, DateTime.MaxValue };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionStart_Set_TestData))]
    public void MonthCalendar_SelectionStart_Set_GetReturnsExpected(DateTime value, DateTime expectedSelectionEnd)
    {
        using MonthCalendar calendar = new()
        {
            SelectionStart = value
        };
        Assert.Equal(value, calendar.SelectionStart);
        Assert.Equal(expectedSelectionEnd, calendar.SelectionEnd);
        Assert.False(calendar.IsHandleCreated);

        // Set same.
        calendar.SelectionStart = value;
        Assert.Equal(value, calendar.SelectionStart);
        Assert.Equal(expectedSelectionEnd, calendar.SelectionEnd);
        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionStart_Set_TestData))]
    public void MonthCalendar_SelectionStart_SetWithHandle_GetReturnsExpected(DateTime value, DateTime expectedSelectionEnd)
    {
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.SelectionStart = value;
        Assert.Equal(value, calendar.SelectionStart);
        Assert.Equal(expectedSelectionEnd, calendar.SelectionEnd);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        calendar.SelectionStart = value;
        Assert.Equal(value, calendar.SelectionStart);
        Assert.Equal(expectedSelectionEnd, calendar.SelectionEnd);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void MonthCalendar_SelectionStart_SetLessThanMinDate_ThrowsArgumentOutOfRangeException()
    {
        using MonthCalendar calendar = new();
        calendar.SelectionStart = calendar.MinDate.AddTicks(-1);
        Assert.Equal(calendar.MinDate.AddTicks(-1), calendar.SelectionStart);

        calendar.MinDate = new DateTime(2019, 10, 3);
        Assert.Throws<ArgumentOutOfRangeException>("value", () => calendar.SelectionStart = calendar.MinDate.AddTicks(-1));
    }

    [WinFormsFact]
    public void MonthCalendar_SelectionStart_SetGreaterThanMaxDate_ThrowsArgumentOutOfRangeException()
    {
        using MonthCalendar calendar = new();
        calendar.SelectionStart = calendar.MaxDate.AddTicks(1);
        Assert.Equal(calendar.MaxDate.AddTicks(1), calendar.SelectionStart);

        calendar.MaxDate = new DateTime(2019, 9, 3);
        Assert.Throws<ArgumentOutOfRangeException>("value", () => calendar.SelectionStart = calendar.MaxDate.AddTicks(1));
    }

    public static IEnumerable<object[]> SelectionEnd_Set_TestData()
    {
        yield return new object[] { new DateTime(1753, 1, 1), new DateTime(1753, 1, 1) };
        yield return new object[] { new DateTime(1753, 1, 1).AddHours(1), new DateTime(1753, 1, 1).AddHours(1) };
        yield return new object[] { new DateTime(2019, 1, 29), new DateTime(2019, 1, 29) };
        yield return new object[] { DateTime.Now.Date.AddDays(1), DateTime.Now.Date };
        yield return new object[] { DateTime.Now.Date, DateTime.Now.Date };
        yield return new object[] { new DateTime(9998, 12, 31), new DateTime(9998, 12, 25) };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionEnd_Set_TestData))]
    public void MonthCalendar_SelectionEnd_Set_GetReturnsExpected(DateTime value, DateTime expectedSelectionStart)
    {
        using MonthCalendar calendar = new()
        {
            SelectionEnd = value
        };
        Assert.Equal(value, calendar.SelectionEnd);
        Assert.Equal(expectedSelectionStart, calendar.SelectionStart);
        Assert.False(calendar.IsHandleCreated);

        // Set same.
        calendar.SelectionEnd = value;
        Assert.Equal(value, calendar.SelectionEnd);
        Assert.Equal(expectedSelectionStart, calendar.SelectionStart);
        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionEnd_Set_TestData))]
    public void MonthCalendar_SelectionEnd_SetWithHandle_GetReturnsExpected(DateTime value, DateTime expectedSelectionStart)
    {
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.SelectionEnd = value;
        Assert.Equal(value, calendar.SelectionEnd);
        Assert.Equal(expectedSelectionStart, calendar.SelectionStart);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        calendar.SelectionEnd = value;
        Assert.Equal(value, calendar.SelectionEnd);
        Assert.Equal(expectedSelectionStart, calendar.SelectionStart);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void MonthCalendar_SelectionEnd_SetLessThanMinDate_ThrowsArgumentOutOfRangeException()
    {
        using MonthCalendar calendar = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => calendar.SelectionEnd = calendar.MinDate.AddTicks(-1));

        calendar.MinDate = new DateTime(2019, 10, 3);
        Assert.Throws<ArgumentOutOfRangeException>("value", () => calendar.SelectionEnd = calendar.MinDate.AddTicks(-1));
    }

    [WinFormsFact]
    public void MonthCalendar_SelectionEnd_SetGreaterThanMaxDate_ThrowsArgumentOutOfRangeException()
    {
        using MonthCalendar calendar = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => calendar.SelectionEnd = calendar.MaxDate.AddTicks(1));

        calendar.MaxDate = new DateTime(2019, 9, 3);
        Assert.Throws<ArgumentOutOfRangeException>("value", () => calendar.SelectionEnd = calendar.MaxDate.AddTicks(1));
    }

    public static IEnumerable<object[]> SelectionRange_Set_TestData()
    {
        yield return new object[] { new SelectionRange(DateTime.MinValue, DateTime.MinValue), DateTime.MinValue, DateTime.MinValue };
        yield return new object[] { new SelectionRange(new DateTime(1753, 1, 1).AddTicks(-1), new DateTime(1753, 1, 1).AddTicks(-1)), new DateTime(1752, 12, 31), new DateTime(1752, 12, 31) };
        yield return new object[] { new SelectionRange(new DateTime(1753, 1, 1), new DateTime(1753, 1, 1)), new DateTime(1753, 1, 1), new DateTime(1753, 1, 1) };
        yield return new object[] { new SelectionRange(new DateTime(1753, 1, 1), new DateTime(1753, 1, 2)), new DateTime(1753, 1, 1), new DateTime(1753, 1, 2) };

        yield return new object[] { new SelectionRange(new DateTime(2019, 9, 1), new DateTime(2019, 9, 1)), new DateTime(2019, 9, 1), new DateTime(2019, 9, 1) };
        yield return new object[] { new SelectionRange(new DateTime(2019, 9, 1), new DateTime(2019, 9, 2)), new DateTime(2019, 9, 1), new DateTime(2019, 9, 2) };
        yield return new object[] { new SelectionRange(new DateTime(2019, 9, 1).AddHours(1), new DateTime(2019, 9, 2).AddHours(1)), new DateTime(2019, 9, 1), new DateTime(2019, 9, 2) };
        yield return new object[] { new SelectionRange(new DateTime(2019, 9, 2), new DateTime(2019, 9, 1)), new DateTime(2019, 9, 1), new DateTime(2019, 9, 2) };
        yield return new object[] { new SelectionRange(new DateTime(2019, 9, 1), new DateTime(2019, 9, 7)), new DateTime(2019, 9, 1), new DateTime(2019, 9, 7) };
        yield return new object[] { new SelectionRange(new DateTime(2019, 9, 1), new DateTime(2019, 9, 8)), new DateTime(2019, 9, 1), new DateTime(2019, 9, 7) };

        yield return new object[] { new SelectionRange(DateTime.Now.Date, DateTime.Now.Date), DateTime.Now.Date, DateTime.Now.Date };
        yield return new object[] { new SelectionRange(DateTime.Now.Date, DateTime.Now.Date.AddDays(1)), DateTime.Now.Date, DateTime.Now.Date.AddDays(1) };
        yield return new object[] { new SelectionRange(DateTime.Now.Date.AddHours(1), DateTime.Now.Date.AddHours(1)), DateTime.Now.Date, DateTime.Now.Date };
        yield return new object[] { new SelectionRange(DateTime.Now.Date.AddDays(1), DateTime.Now.Date), DateTime.Now.Date, DateTime.Now.Date.AddDays(1) };
        yield return new object[] { new SelectionRange(DateTime.Now.Date, DateTime.Now.Date.AddDays(6)), DateTime.Now.Date, DateTime.Now.Date.AddDays(6) };
        yield return new object[] { new SelectionRange(DateTime.Now.Date, DateTime.Now.Date.AddDays(7)), DateTime.Now.Date.AddDays(1), DateTime.Now.Date.AddDays(7) };

        yield return new object[] { new SelectionRange(new DateTime(9998, 12, 30), new DateTime(9998, 12, 31)), new DateTime(9998, 12, 30), new DateTime(9998, 12, 31) };
        yield return new object[] { new SelectionRange(new DateTime(9998, 12, 31), new DateTime(9998, 12, 31)), new DateTime(9998, 12, 31), new DateTime(9998, 12, 31) };
        yield return new object[] { new SelectionRange(new DateTime(9998, 12, 31).AddTicks(1), new DateTime(9998, 12, 31).AddTicks(1)), new DateTime(9998, 12, 31), new DateTime(9998, 12, 31) };
        yield return new object[] { new SelectionRange(DateTime.MaxValue, DateTime.MaxValue), DateTime.MaxValue.Date, DateTime.MaxValue.Date };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionRange_Set_TestData))]
    public void MonthCalendar_SelectionRange_Set_GetReturnsExpected(SelectionRange value, DateTime expectedSelectionStart, DateTime expectedSelectionEnd)
    {
        using MonthCalendar calendar = new()
        {
            SelectionRange = value
        };
        Assert.Equal(expectedSelectionStart, calendar.SelectionRange.Start);
        Assert.Equal(expectedSelectionEnd, calendar.SelectionRange.End);
        Assert.Equal(expectedSelectionStart, calendar.SelectionStart);
        Assert.Equal(expectedSelectionEnd, calendar.SelectionEnd);
        Assert.NotSame(value, calendar.SelectionRange);
        Assert.False(calendar.IsHandleCreated);

        // Set same.
        calendar.SelectionRange = new SelectionRange(expectedSelectionStart, expectedSelectionEnd);
        Assert.Equal(expectedSelectionStart, calendar.SelectionRange.Start);
        Assert.Equal(expectedSelectionEnd, calendar.SelectionRange.End);
        Assert.Equal(expectedSelectionStart, calendar.SelectionStart);
        Assert.Equal(expectedSelectionEnd, calendar.SelectionEnd);
        Assert.NotSame(value, calendar.SelectionRange);
        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionRange_Set_TestData))]
    public void MonthCalendar_SelectionRange_SetWithHandle_GetReturnsExpected(SelectionRange value, DateTime expectedSelectionStart, DateTime expectedSelectionEnd)
    {
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.SelectionRange = value;
        Assert.Equal(expectedSelectionStart, calendar.SelectionRange.Start);
        Assert.Equal(expectedSelectionEnd, calendar.SelectionRange.End);
        Assert.Equal(expectedSelectionStart, calendar.SelectionStart);
        Assert.Equal(expectedSelectionEnd, calendar.SelectionEnd);
        Assert.NotSame(value, calendar.SelectionRange);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        calendar.SelectionRange = new SelectionRange(expectedSelectionStart, expectedSelectionEnd);
        Assert.Equal(expectedSelectionStart, calendar.SelectionRange.Start);
        Assert.Equal(expectedSelectionEnd, calendar.SelectionRange.End);
        Assert.Equal(expectedSelectionStart, calendar.SelectionStart);
        Assert.Equal(expectedSelectionEnd, calendar.SelectionEnd);
        Assert.NotSame(value, calendar.SelectionRange);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void MonthCalendar_SelectionRange_SetLessThanMinDate_ThrowsArgumentOutOfRangeException()
    {
        using MonthCalendar calendar = new();
        calendar.SelectionRange = new SelectionRange(calendar.MinDate.AddTicks(-1), calendar.MinDate);
        Assert.Equal(calendar.MinDate.AddTicks(-1).Date, calendar.SelectionStart);
        Assert.Equal(calendar.MinDate, calendar.SelectionEnd);

        calendar.SelectionRange = new SelectionRange(calendar.MinDate, calendar.MinDate.AddTicks(-1));
        Assert.Equal(calendar.MinDate.AddTicks(-1).Date, calendar.SelectionStart);
        Assert.Equal(calendar.MinDate, calendar.SelectionEnd);

        calendar.MinDate = new DateTime(2019, 10, 3);
        Assert.Throws<ArgumentOutOfRangeException>("date1", () => calendar.SelectionRange = new SelectionRange(calendar.MinDate.AddTicks(-1), calendar.MinDate));
        Assert.Throws<ArgumentOutOfRangeException>("date1", () => calendar.SelectionRange = new SelectionRange(calendar.MinDate, calendar.MinDate.AddTicks(-1)));
    }

    [WinFormsFact]
    public void MonthCalendar_SelectionRange_SetGreaterThanMaxDate_ThrowsArgumentOutOfRangeException()
    {
        using MonthCalendar calendar = new();
        calendar.SelectionRange = new SelectionRange(calendar.MaxDate.AddTicks(1), calendar.MaxDate);
        Assert.Equal(calendar.MaxDate, calendar.SelectionStart);
        Assert.Equal(calendar.MaxDate.AddTicks(1).Date, calendar.SelectionEnd);

        calendar.SelectionRange = new SelectionRange(calendar.MaxDate, calendar.MaxDate.AddTicks(1));
        Assert.Equal(calendar.MaxDate, calendar.SelectionStart);
        Assert.Equal(calendar.MaxDate.AddTicks(1).Date, calendar.SelectionEnd);

        calendar.MaxDate = new DateTime(2019, 9, 3);
        Assert.Throws<ArgumentOutOfRangeException>("date2", () => calendar.SelectionRange = new SelectionRange(calendar.MaxDate.AddDays(1), calendar.MaxDate));
        Assert.Throws<ArgumentOutOfRangeException>("date2", () => calendar.SelectionRange = new SelectionRange(calendar.MaxDate, calendar.MaxDate.AddDays(1)));
    }

    [WinFormsTheory]
    [BoolData]
    public void MonthCalendar_ShowToday_Set_GetReturnsExpected(bool value)
    {
        using SubMonthCalendar control = new()
        {
            ShowToday = value
        };
        Assert.Equal(value, control.ShowToday);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ShowToday = value;
        Assert.Equal(value, control.ShowToday);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.ShowToday = !value;
        Assert.Equal(!value, control.ShowToday);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 0)]
    [InlineData(false, 1)]
    public void MonthCalendar_ShowToday_SetWithHandle_GetReturnsExpected(bool value, int expectedStyleChangedCallCount)
    {
        using SubMonthCalendar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ShowToday = value;
        Assert.Equal(value, control.ShowToday);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ShowToday = value;
        Assert.Equal(value, control.ShowToday);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.ShowToday = !value;
        Assert.Equal(!value, control.ShowToday);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount + 1, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount + 1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void MonthCalendar_CreateAccessibilityInstance_Invoke_ReturnsExpected()
    {
        using SubMonthCalendar control = new();
        Control.ControlAccessibleObject instance = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(control.CreateAccessibilityInstance());
        Assert.NotNull(instance);
        Assert.Same(control, instance.Owner);
        Assert.Equal(AccessibleRole.Table, instance.Role);
        Assert.NotSame(control.CreateAccessibilityInstance(), instance);
        Assert.NotSame(control.AccessibilityObject, instance);
    }

    [WinFormsTheory]
    [BoolData]
    public void MonthCalendar_ShowTodayCircle_Set_GetReturnsExpected(bool value)
    {
        using SubMonthCalendar control = new()
        {
            ShowTodayCircle = value
        };
        Assert.Equal(value, control.ShowTodayCircle);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ShowTodayCircle = value;
        Assert.Equal(value, control.ShowTodayCircle);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.ShowTodayCircle = !value;
        Assert.Equal(!value, control.ShowTodayCircle);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 0)]
    [InlineData(false, 1)]
    public void MonthCalendar_ShowTodayCircle_SetWithHandle_GetReturnsExpected(bool value, int expectedStyleChangedCallCount)
    {
        using SubMonthCalendar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ShowTodayCircle = value;
        Assert.Equal(value, control.ShowTodayCircle);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ShowTodayCircle = value;
        Assert.Equal(value, control.ShowTodayCircle);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.ShowTodayCircle = !value;
        Assert.Equal(!value, control.ShowTodayCircle);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount + 1, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount + 1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void MonthCalendar_ShowWeekNumbers_Set_GetReturnsExpected(bool value)
    {
        using SubMonthCalendar control = new()
        {
            ShowWeekNumbers = value
        };
        Assert.Equal(value, control.ShowWeekNumbers);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ShowWeekNumbers = value;
        Assert.Equal(value, control.ShowWeekNumbers);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.ShowWeekNumbers = !value;
        Assert.Equal(!value, control.ShowWeekNumbers);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void MonthCalendar_ShowWeekNumbers_SetWithHandle_GetReturnsExpected(bool value, int expectedStyleChangedCallCount)
    {
        using SubMonthCalendar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ShowWeekNumbers = value;
        Assert.Equal(value, control.ShowWeekNumbers);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ShowWeekNumbers = value;
        Assert.Equal(value, control.ShowWeekNumbers);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.ShowWeekNumbers = !value;
        Assert.Equal(!value, control.ShowWeekNumbers);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount + 1, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount + 1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void MonthCalendar_SingleMonthSize_GetWithHandle_ReturnsExpected()
    {
        using MonthCalendar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Size size = control.SingleMonthSize;
        Assert.True(size.Width >= 169);
        Assert.True(size.Height >= 153);
        Assert.Equal(size, control.SingleMonthSize);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SingleMonthSize_GetCustomGetMinReqRect_TestData()
    {
        yield return new object[] { new RECT(0, 0, 0, 0), Size.Empty };
        yield return new object[] { new RECT(1, 2, 3, 4), new Size(3, 4) };
        yield return new object[] { new RECT(1, 2, 1, 6), new Size(1, 6) };
        yield return new object[] { new RECT(1, 2, 6, 1), new Size(6, 1) };
        yield return new object[] { new RECT(1, 2, 6, 6), new Size(6, 6) };
        yield return new object[] { new RECT(1, 2, 30, 40), new Size(30, 40) };
    }

    [WinFormsTheory]
    [MemberData(nameof(SingleMonthSize_GetCustomGetMinReqRect_TestData))]
    public void MonthCalendar_SingleMonthSize_GetCustomGetMinReqRect_ReturnsExpected(object getMinReqRectResult, Size expected)
    {
        using CustomGetMinReqRectMonthCalendar control = new()
        {
            GetMinReqRectResult = (RECT)getMinReqRectResult
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        Assert.Equal(expected, control.SingleMonthSize);
    }

    private class CustomGetMinReqRectMonthCalendar : MonthCalendar
    {
        public RECT GetMinReqRectResult { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvoke.MCM_GETMINREQRECT)
            {
                RECT* pRect = (RECT*)m.LParam;
                *pRect = GetMinReqRectResult;
                m.Result = 1;
                return;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsFact]
    public void MonthCalendar_SingleMonthSize_GetInvalidGetMinReqRect_ThrowsInvalidOperationException()
    {
        using InvalidGetMinReqRectMonthCalendar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.MakeInvalid = true;
        Assert.Throws<InvalidOperationException>(() => control.SingleMonthSize);
    }

    private class InvalidGetMinReqRectMonthCalendar : MonthCalendar
    {
        public bool MakeInvalid { get; set; }

        protected override void WndProc(ref Message m)
        {
            if (MakeInvalid && m.Msg == (int)PInvoke.MCM_GETMINREQRECT)
            {
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }
    }

    public static IEnumerable<object[]> Size_Set_TestData()
    {
        yield return new object[] { new Size(1, 2) };
        yield return new object[] { new Size(0, 0) };
        yield return new object[] { new Size(-1, -2) };
        yield return new object[] { new Size(-1, 2) };
        yield return new object[] { new Size(1, -2) };
    }

    [WinFormsTheory]
    [MemberData(nameof(Size_Set_TestData))]
    public void MonthCalendar_Size_Set_GetReturnsExpected(Size value)
    {
        using MonthCalendar control = new();
        Size size = control.ClientSize;

        control.Size = value;
        Assert.Equal(size, control.ClientSize);
        Assert.Equal(new Rectangle(Point.Empty, size), control.ClientRectangle);
        Assert.Equal(new Rectangle(Point.Empty, size), control.DisplayRectangle);
        Assert.Equal(size, control.Size);
        Assert.Equal(new Rectangle(Point.Empty, size), control.Bounds);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Size = value;
        Assert.Equal(size, control.ClientSize);
        Assert.Equal(new Rectangle(Point.Empty, size), control.ClientRectangle);
        Assert.Equal(new Rectangle(Point.Empty, size), control.DisplayRectangle);
        Assert.Equal(size, control.Size);
        Assert.Equal(new Rectangle(Point.Empty, size), control.Bounds);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Size_Set_TestData))]
    public void MonthCalendar_Size_SetWithHandle_GetReturnsExpected(Size value)
    {
        using MonthCalendar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Size size = control.Size;

        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Size = value;
        Assert.Equal(size, control.ClientSize);
        Assert.Equal(new Rectangle(Point.Empty, size), control.ClientRectangle);
        Assert.Equal(new Rectangle(Point.Empty, size), control.DisplayRectangle);
        Assert.Equal(size, control.Size);
        Assert.Equal(new Rectangle(Point.Empty, size), control.Bounds);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Size = value;
        Assert.Equal(size, control.ClientSize);
        Assert.Equal(new Rectangle(Point.Empty, size), control.ClientRectangle);
        Assert.Equal(new Rectangle(Point.Empty, size), control.DisplayRectangle);
        Assert.Equal(size, control.Size);
        Assert.Equal(new Rectangle(Point.Empty, size), control.Bounds);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void MonthCalendar_Size_SetWithHandler_CallsSizeChanged()
    {
        using MonthCalendar control = new();
        Size size = control.Size;

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        int clientSizeChangedCallCount = 0;
        EventHandler clientSizeChangedHandler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            clientSizeChangedCallCount++;
        };
        control.SizeChanged += handler;
        control.ClientSizeChanged += clientSizeChangedHandler;

        control.Size = new Size(10, 10);
        Assert.Equal(size, control.Size);
        Assert.Equal(0, callCount);
        Assert.Equal(0, clientSizeChangedCallCount);

        // Set same.
        control.Size = new Size(10, 10);
        Assert.Equal(size, control.Size);
        Assert.Equal(0, callCount);
        Assert.Equal(0, clientSizeChangedCallCount);

        // Set different.
        control.Size = new Size(11, 11);
        Assert.Equal(size, control.Size);
        Assert.Equal(0, callCount);
        Assert.Equal(0, clientSizeChangedCallCount);

        // Remove handler.
        control.SizeChanged -= handler;
        control.ClientSizeChanged -= clientSizeChangedHandler;
        control.Size = new Size(10, 10);
        Assert.Equal(size, control.Size);
        Assert.Equal(0, callCount);
        Assert.Equal(0, clientSizeChangedCallCount);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void MonthCalendar_Text_Set_GetReturnsExpected(string value, string expected)
    {
        using MonthCalendar control = new()
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
    public void MonthCalendar_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
    {
        using MonthCalendar control = new();
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
    public void MonthCalendar_Text_SetWithHandler_CallsTextChanged()
    {
        using MonthCalendar control = new();
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

    public static IEnumerable<object[]> TitleBackColor_Set_TestData()
    {
        yield return new object[] { Color.Red };
        yield return new object[] { Color.FromArgb(254, 1, 2, 3) };
    }

    [WinFormsTheory]
    [MemberData(nameof(TitleBackColor_Set_TestData))]
    public void MonthCalendar_TitleBackColor_Set_GetReturnsExpected(Color value)
    {
        using MonthCalendar calendar = new()
        {
            TitleBackColor = value
        };
        Assert.Equal(value, calendar.TitleBackColor);
        Assert.False(calendar.IsHandleCreated);

        // Call again.
        calendar.TitleBackColor = value;
        Assert.Equal(value, calendar.TitleBackColor);
        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(TitleBackColor_Set_TestData))]
    public void MonthCalendar_TitleBackColor_SetWithHandle_GetReturnsExpected(Color value)
    {
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.TitleBackColor = value;
        Assert.Equal(value, calendar.TitleBackColor);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        calendar.TitleBackColor = value;
        Assert.Equal(value, calendar.TitleBackColor);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void MonthCalendar_TitleBackColor_SetEmpty_ThrowsArgumentException()
    {
        using MonthCalendar calendar = new();
        Assert.Throws<ArgumentException>("value", () => calendar.TitleBackColor = Color.Empty);
    }

    public static IEnumerable<object[]> TitleForeColor_Set_TestData()
    {
        yield return new object[] { Color.Red };
        yield return new object[] { Color.FromArgb(254, 1, 2, 3) };
    }

    [WinFormsTheory]
    [MemberData(nameof(TitleForeColor_Set_TestData))]
    public void MonthCalendar_TitleForeColor_Set_GetReturnsExpected(Color value)
    {
        using MonthCalendar calendar = new()
        {
            TitleForeColor = value
        };
        Assert.Equal(value, calendar.TitleForeColor);
        Assert.False(calendar.IsHandleCreated);

        // Call again.
        calendar.TitleForeColor = value;
        Assert.Equal(value, calendar.TitleForeColor);
        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(TitleForeColor_Set_TestData))]
    public void MonthCalendar_TitleForeColor_SetWithHandle_GetReturnsExpected(Color value)
    {
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.TitleForeColor = value;
        Assert.Equal(value, calendar.TitleForeColor);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        calendar.TitleForeColor = value;
        Assert.Equal(value, calendar.TitleForeColor);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void MonthCalendar_TitleForeColor_SetEmpty_ThrowsArgumentException()
    {
        using MonthCalendar calendar = new();
        Assert.Throws<ArgumentException>("value", () => calendar.TitleForeColor = Color.Empty);
    }

    [WinFormsFact]
    public void MonthCalendar_TodayDate_GetWithHandle_ReturnsExpected()
    {
        using MonthCalendar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(DateTime.Now.Date, control.TodayDate);
        Assert.False(control.TodayDateSet);
    }

    public static IEnumerable<object[]> TodayDate_Set_TestData()
    {
        yield return new object[] { DateTime.MinValue };
        yield return new object[] { new DateTime(1753, 1, 1).AddTicks(-1) };
        yield return new object[] { new DateTime(1753, 1, 1) };
        yield return new object[] { new DateTime(2019, 9, 1) };
        yield return new object[] { new DateTime(2019, 9, 1).AddHours(1) };
        yield return new object[] { DateTime.Now.Date };
        yield return new object[] { DateTime.Now.Date.AddHours(1) };
        yield return new object[] { new DateTime(9998, 12, 31) };
        yield return new object[] { new DateTime(9998, 12, 31).AddTicks(1) };
        yield return new object[] { DateTime.MaxValue };
    }

    [WinFormsTheory]
    [MemberData(nameof(TodayDate_Set_TestData))]
    public void MonthCalendar_TodayDate_Set_GetReturnsExpected(DateTime value)
    {
        using MonthCalendar calendar = new()
        {
            TodayDate = value
        };
        Assert.Equal(value.Date, calendar.TodayDate);
        Assert.True(calendar.TodayDateSet);
        Assert.False(calendar.IsHandleCreated);

        // Call again.
        calendar.TodayDate = value;
        Assert.Equal(value.Date, calendar.TodayDate);
        Assert.True(calendar.TodayDateSet);
        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(TodayDate_Set_TestData))]
    public void MonthCalendar_TodayDate_SetWithHandle_GetReturnsExpected(DateTime value)
    {
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.TodayDate = value;
        Assert.Equal(value.Date, calendar.TodayDate);
        Assert.True(calendar.TodayDateSet);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        calendar.TodayDate = value;
        Assert.Equal(value.Date, calendar.TodayDate);
        Assert.True(calendar.TodayDateSet);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void MonthCalendar_TodayDate_SetLessThanMinDate_ThrowsArgumentOutOfRangeException()
    {
        using MonthCalendar calendar = new();
        calendar.TodayDate = calendar.MinDate.AddTicks(-1);
        Assert.Equal(calendar.MinDate.AddTicks(-1).Date, calendar.TodayDate);

        calendar.MinDate = new DateTime(2019, 10, 3);
        Assert.Throws<ArgumentOutOfRangeException>("value", () => calendar.TodayDate = calendar.MinDate.AddTicks(-1));
    }

    [WinFormsFact]
    public void MonthCalendar_TodayDate_SetGreaterThanMaxDate_ThrowsArgumentOutOfRangeException()
    {
        using MonthCalendar calendar = new();
        calendar.TodayDate = calendar.MaxDate.AddTicks(1);
        Assert.Equal(calendar.MaxDate.AddTicks(1).Date, calendar.TodayDate);

        calendar.MaxDate = new DateTime(2019, 9, 3);
        Assert.Throws<ArgumentOutOfRangeException>("value", () => calendar.TodayDate = calendar.MaxDate.AddDays(1));
    }

    public static IEnumerable<object[]> TrailingForeColor_Set_TestData()
    {
        yield return new object[] { Color.Red };
        yield return new object[] { Color.FromArgb(254, 1, 2, 3) };
    }

    [WinFormsTheory]
    [MemberData(nameof(TrailingForeColor_Set_TestData))]
    public void MonthCalendar_TrailingForeColor_Set_GetReturnsExpected(Color value)
    {
        using MonthCalendar calendar = new()
        {
            TrailingForeColor = value
        };
        Assert.Equal(value, calendar.TrailingForeColor);
        Assert.False(calendar.IsHandleCreated);

        // Call again.
        calendar.TrailingForeColor = value;
        Assert.Equal(value, calendar.TrailingForeColor);
        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(TrailingForeColor_Set_TestData))]
    public void MonthCalendar_TrailingForeColor_SetWithHandle_GetReturnsExpected(Color value)
    {
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.TrailingForeColor = value;
        Assert.Equal(value, calendar.TrailingForeColor);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        calendar.TrailingForeColor = value;
        Assert.Equal(value, calendar.TrailingForeColor);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void MonthCalendar_TrailingForeColor_SetEmpty_ThrowsArgumentException()
    {
        using MonthCalendar calendar = new();
        Assert.Throws<ArgumentException>("value", () => calendar.TrailingForeColor = Color.Empty);
    }

    [WinFormsFact]
    public void MonthCalendar_AddAnnuallyBoldedDate_Invoke_AddsToAnnuallyBoldedDates()
    {
        using MonthCalendar calendar = new();
        calendar.AddAnnuallyBoldedDate(new DateTime(2019, 10, 3));
        Assert.Equal([new(2019, 10, 3)], calendar.AnnuallyBoldedDates);
        Assert.False(calendar.IsHandleCreated);

        // Different day.
        calendar.AddAnnuallyBoldedDate(new DateTime(2019, 10, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5)], calendar.AnnuallyBoldedDates);
        Assert.False(calendar.IsHandleCreated);

        // Different month.
        calendar.AddAnnuallyBoldedDate(new DateTime(2019, 09, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5)], calendar.AnnuallyBoldedDates);
        Assert.False(calendar.IsHandleCreated);

        // Different year.
        calendar.AddAnnuallyBoldedDate(new DateTime(2018, 09, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5)], calendar.AnnuallyBoldedDates);
        Assert.False(calendar.IsHandleCreated);

        // Duplicate.
        calendar.AddAnnuallyBoldedDate(new DateTime(2018, 09, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5), new(2018, 09, 5)], calendar.AnnuallyBoldedDates);
        Assert.False(calendar.IsHandleCreated);

        // MinValue.
        calendar.AddAnnuallyBoldedDate(DateTime.MinValue);
        Assert.Equal(new DateTime[] { new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5), new(2018, 09, 5), DateTime.MinValue }, calendar.AnnuallyBoldedDates);
        Assert.False(calendar.IsHandleCreated);

        // MaxValue.
        calendar.AddAnnuallyBoldedDate(DateTime.MaxValue);
        Assert.Equal(new DateTime[] { new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5), new(2018, 09, 5), DateTime.MinValue, DateTime.MaxValue }, calendar.AnnuallyBoldedDates);
        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsFact]
    public void MonthCalendar_AddAnnuallyBoldedDate_InvokeWithHandle_AddsToAnnuallyBoldedDates()
    {
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.AddAnnuallyBoldedDate(new DateTime(2019, 10, 3));
        Assert.Equal([new(2019, 10, 3)], calendar.AnnuallyBoldedDates);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Different day.
        calendar.AddAnnuallyBoldedDate(new DateTime(2019, 10, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5)], calendar.AnnuallyBoldedDates);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Different month.
        calendar.AddAnnuallyBoldedDate(new DateTime(2019, 09, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5)], calendar.AnnuallyBoldedDates);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Different year.
        calendar.AddAnnuallyBoldedDate(new DateTime(2018, 09, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5)], calendar.AnnuallyBoldedDates);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Duplicate.
        calendar.AddAnnuallyBoldedDate(new DateTime(2018, 09, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5), new(2018, 09, 5)], calendar.AnnuallyBoldedDates);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // MinValue.
        calendar.AddAnnuallyBoldedDate(DateTime.MinValue);
        Assert.Equal(new DateTime[] { new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5), new(2018, 09, 5), DateTime.MinValue }, calendar.AnnuallyBoldedDates);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // MaxValue.
        calendar.AddAnnuallyBoldedDate(DateTime.MaxValue);
        Assert.Equal(new DateTime[] { new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5), new(2018, 09, 5), DateTime.MinValue, DateTime.MaxValue }, calendar.AnnuallyBoldedDates);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void MonthCalendar_AddBoldedDate_Invoke_AddsToBoldedDates()
    {
        using MonthCalendar calendar = new();
        calendar.AddBoldedDate(new DateTime(2019, 10, 3));
        Assert.Equal([new(2019, 10, 3)], calendar.BoldedDates);
        Assert.False(calendar.IsHandleCreated);

        // Different day.
        calendar.AddBoldedDate(new DateTime(2019, 10, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5)], calendar.BoldedDates);
        Assert.False(calendar.IsHandleCreated);

        // Different month.
        calendar.AddBoldedDate(new DateTime(2019, 09, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5)], calendar.BoldedDates);
        Assert.False(calendar.IsHandleCreated);

        // Different year.
        calendar.AddBoldedDate(new DateTime(2018, 09, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5)], calendar.BoldedDates);
        Assert.False(calendar.IsHandleCreated);

        // Duplicate.
        calendar.AddBoldedDate(new DateTime(2018, 09, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5)], calendar.BoldedDates);
        Assert.False(calendar.IsHandleCreated);

        // MinValue.
        calendar.AddBoldedDate(DateTime.MinValue);
        Assert.Equal(new DateTime[] { new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5), DateTime.MinValue }, calendar.BoldedDates);
        Assert.False(calendar.IsHandleCreated);

        // MaxValue.
        calendar.AddBoldedDate(DateTime.MaxValue);
        Assert.Equal(new DateTime[] { new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5), DateTime.MinValue, DateTime.MaxValue }, calendar.BoldedDates);
        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsFact]
    public void MonthCalendar_AddBoldedDate_InvokeWithHandle_AddsToBoldedDates()
    {
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.AddBoldedDate(new DateTime(2019, 10, 3));
        Assert.Equal([new(2019, 10, 3)], calendar.BoldedDates);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Different day.
        calendar.AddBoldedDate(new DateTime(2019, 10, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5)], calendar.BoldedDates);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Different month.
        calendar.AddBoldedDate(new DateTime(2019, 09, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5)], calendar.BoldedDates);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Different year.
        calendar.AddBoldedDate(new DateTime(2018, 09, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5)], calendar.BoldedDates);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Duplicate.
        calendar.AddBoldedDate(new DateTime(2018, 09, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5)], calendar.BoldedDates);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // MinValue.
        calendar.AddBoldedDate(DateTime.MinValue);
        Assert.Equal(new DateTime[] { new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5), DateTime.MinValue }, calendar.BoldedDates);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // MaxValue.
        calendar.AddBoldedDate(DateTime.MaxValue);
        Assert.Equal(new DateTime[] { new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5), DateTime.MinValue, DateTime.MaxValue }, calendar.BoldedDates);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void MonthCalendar_AddMonthlyBoldedDate_Invoke_AddsToMonthlyBoldedDates()
    {
        using MonthCalendar calendar = new();
        calendar.AddMonthlyBoldedDate(new DateTime(2019, 10, 3));
        Assert.Equal([new(2019, 10, 3)], calendar.MonthlyBoldedDates);
        Assert.False(calendar.IsHandleCreated);

        // Different day.
        calendar.AddMonthlyBoldedDate(new DateTime(2019, 10, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5)], calendar.MonthlyBoldedDates);
        Assert.False(calendar.IsHandleCreated);

        // Different month.
        calendar.AddMonthlyBoldedDate(new DateTime(2019, 09, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5)], calendar.MonthlyBoldedDates);
        Assert.False(calendar.IsHandleCreated);

        // Different year.
        calendar.AddMonthlyBoldedDate(new DateTime(2018, 09, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5)], calendar.MonthlyBoldedDates);
        Assert.False(calendar.IsHandleCreated);

        // Duplicate.
        calendar.AddMonthlyBoldedDate(new DateTime(2018, 09, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5), new(2018, 09, 5)], calendar.MonthlyBoldedDates);
        Assert.False(calendar.IsHandleCreated);

        // MinValue.
        calendar.AddMonthlyBoldedDate(DateTime.MinValue);
        Assert.Equal(new DateTime[] { new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5), new(2018, 09, 5), DateTime.MinValue }, calendar.MonthlyBoldedDates);
        Assert.False(calendar.IsHandleCreated);

        // MaxValue.
        calendar.AddMonthlyBoldedDate(DateTime.MaxValue);
        Assert.Equal(new DateTime[] { new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5), new(2018, 09, 5), DateTime.MinValue, DateTime.MaxValue }, calendar.MonthlyBoldedDates);
        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsFact]
    public void MonthCalendar_AddMonthlyBoldedDate_InvokeWithHandle_AddsToMonthlyBoldedDates()
    {
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.AddMonthlyBoldedDate(new DateTime(2019, 10, 3));
        Assert.Equal([new(2019, 10, 3)], calendar.MonthlyBoldedDates);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Different day.
        calendar.AddMonthlyBoldedDate(new DateTime(2019, 10, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5)], calendar.MonthlyBoldedDates);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Different month.
        calendar.AddMonthlyBoldedDate(new DateTime(2019, 09, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5)], calendar.MonthlyBoldedDates);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Different year.
        calendar.AddMonthlyBoldedDate(new DateTime(2018, 09, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5)], calendar.MonthlyBoldedDates);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Duplicate.
        calendar.AddMonthlyBoldedDate(new DateTime(2018, 09, 5));
        Assert.Equal([new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5), new(2018, 09, 5)], calendar.MonthlyBoldedDates);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // MinValue.
        calendar.AddMonthlyBoldedDate(DateTime.MinValue);
        Assert.Equal(new DateTime[] { new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5), new(2018, 09, 5), DateTime.MinValue }, calendar.MonthlyBoldedDates);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // MaxValue.
        calendar.AddMonthlyBoldedDate(DateTime.MaxValue);
        Assert.Equal(new DateTime[] { new(2019, 10, 3), new(2019, 10, 5), new(2019, 09, 5), new(2018, 09, 5), new(2018, 09, 5), DateTime.MinValue, DateTime.MaxValue }, calendar.MonthlyBoldedDates);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void MonthCalendar_CreateHandle_Invoke_Success()
    {
        using SubMonthCalendar control = new();
        control.CreateHandle();
        Assert.True(control.Created);
        Assert.True(control.IsHandleCreated);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
    }

    [WinFormsFact]
    public void MonthCalendar_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubMonthCalendar control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    [WinFormsTheory]
    [InlineData(ControlStyles.ContainerControl, false)]
    [InlineData(ControlStyles.UserPaint, false)]
    [InlineData(ControlStyles.Opaque, false)]
    [InlineData(ControlStyles.ResizeRedraw, false)]
    [InlineData(ControlStyles.FixedWidth, false)]
    [InlineData(ControlStyles.FixedHeight, false)]
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
    [InlineData(ControlStyles.UseTextForAccessibility, true)]
    [InlineData((ControlStyles)0, true)]
    [InlineData((ControlStyles)int.MaxValue, false)]
    [InlineData((ControlStyles)(-1), false)]
    public void MonthCalendar_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubMonthCalendar control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void MonthCalendar_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubMonthCalendar control = new();
        Assert.False(control.GetTopLevel());
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void MonthCalendar_OnBackColorChanged_Invoke_CallsBackColorChanged(EventArgs eventArgs)
    {
        using SubMonthCalendar control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.BackColorChanged += handler;
        control.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.BackColorChanged -= handler;
        control.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void MonthCalendar_OnBackColorChanged_InvokeWithHandle_CallsBackColorChanged(EventArgs eventArgs)
    {
        using SubMonthCalendar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        int invalidatedCallCount = 0;
        InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        EventHandler styleChangedHandler = (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        EventHandler createdHandler = (sender, e) => createdCallCount++;

        // Call with handler.
        control.BackColorChanged += handler;
        control.Invalidated += invalidatedHandler;
        control.StyleChanged += styleChangedHandler;
        control.HandleCreated += createdHandler;
        control.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(control.IsHandleCreated);

        // Remove handler.
        control.BackColorChanged -= handler;
        control.Invalidated -= invalidatedHandler;
        control.StyleChanged -= styleChangedHandler;
        control.HandleCreated -= createdHandler;
        control.OnBackColorChanged(eventArgs);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void MonthCalendar_OnClick_Invoke_CallsClick(EventArgs eventArgs)
    {
        using SubMonthCalendar control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Click += handler;
        control.OnClick(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.Click -= handler;
        control.OnClick(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> DateRangeEventArgs_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new DateRangeEventArgs(DateTime.Now, DateTime.Now) };
    }

    [WinFormsTheory]
    [MemberData(nameof(DateRangeEventArgs_TestData))]
    public void MonthCalendar_Calendar_OnDateChanged_Invoke_CallsDateChanged(DateRangeEventArgs eventArgs)
    {
        using SubMonthCalendar calendar = new();
        int callCount = 0;
        DateRangeEventHandler handler = (sender, e) =>
        {
            Assert.Same(calendar, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        calendar.DateChanged += handler;
        calendar.OnDateChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        calendar.DateChanged -= handler;
        calendar.OnDateChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(DateRangeEventArgs_TestData))]
    public void MonthCalendar_Calendar_OnDateSelected_Invoke_CallsDateSelected(DateRangeEventArgs eventArgs)
    {
        using SubMonthCalendar calendar = new();
        int callCount = 0;
        DateRangeEventHandler handler = (sender, e) =>
        {
            Assert.Same(calendar, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        calendar.DateSelected += handler;
        calendar.OnDateSelected(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        calendar.DateSelected -= handler;
        calendar.OnDateSelected(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void MonthControl_OnDoubleClick_Invoke_CallsDoubleClick(EventArgs eventArgs)
    {
        using SubMonthCalendar control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.DoubleClick += handler;
        control.OnDoubleClick(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.DoubleClick -= handler;
        control.OnDoubleClick(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void MonthCalendar_OnForeColorChanged_Invoke_CallsForeColorChanged(EventArgs eventArgs)
    {
        using SubMonthCalendar control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.ForeColorChanged += handler;
        control.OnForeColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.ForeColorChanged -= handler;
        control.OnForeColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void MonthCalendar_OnForeColorChanged_InvokeWithHandle_CallsForeColorChanged(EventArgs eventArgs)
    {
        using SubMonthCalendar control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        int invalidatedCallCount = 0;
        InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        EventHandler styleChangedHandler = (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        EventHandler createdHandler = (sender, e) => createdCallCount++;

        // Call with handler.
        control.ForeColorChanged += handler;
        control.Invalidated += invalidatedHandler;
        control.StyleChanged += styleChangedHandler;
        control.HandleCreated += createdHandler;
        control.OnForeColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(control.IsHandleCreated);

        // Remove handler.
        control.ForeColorChanged -= handler;
        control.Invalidated -= invalidatedHandler;
        control.StyleChanged -= styleChangedHandler;
        control.HandleCreated -= createdHandler;
        control.OnForeColorChanged(eventArgs);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void MonthCalendar_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
    {
        using SubMonthCalendar control = new();
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
    [NewAndDefaultData<EventArgs>]
    public void MonthCalendar_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(EventArgs eventArgs)
    {
        using SubMonthCalendar control = new();
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
    [NewAndDefaultData<EventArgs>]
    public void MonthCalendar_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using SubMonthCalendar control = new();
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
    [NewAndDefaultData<EventArgs>]
    public void MonthCalendar_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using SubMonthCalendar control = new();
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
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetMouseEventArgsTheoryData))]
    public void MonthCalendar_OnMouseClick_Invoke_CallsMouseClick(MouseEventArgs eventArgs)
    {
        using SubMonthCalendar control = new();
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.MouseClick += handler;
        control.OnMouseClick(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.MouseClick -= handler;
        control.OnMouseClick(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetMouseEventArgsTheoryData))]
    public void MonthCalendar_OnMouseDoubleClick_Invoke_CallsMouseDoubleClick(MouseEventArgs eventArgs)
    {
        using SubMonthCalendar control = new();
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.MouseDoubleClick += handler;
        control.OnMouseDoubleClick(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.MouseDoubleClick -= handler;
        control.OnMouseDoubleClick(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaintEventArgsTheoryData))]
    public void MonthCalendar_OnPaint_Invoke_CallsPaint(PaintEventArgs eventArgs)
    {
        using SubMonthCalendar control = new();
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

    public static IEnumerable<object[]> OnRightToLeftLayoutChanged_TestData()
    {
        yield return new object[] { RightToLeft.Yes, null };
        yield return new object[] { RightToLeft.Yes, new EventArgs() };
        yield return new object[] { RightToLeft.No, null };
        yield return new object[] { RightToLeft.No, new EventArgs() };
        yield return new object[] { RightToLeft.Inherit, null };
        yield return new object[] { RightToLeft.Inherit, new EventArgs() };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnRightToLeftLayoutChanged_TestData))]
    public void MonthCalendar_OnRightToLeftLayoutChanged_Invoke_CallsRightToLeftLayoutChanged(RightToLeft rightToLeft, EventArgs eventArgs)
    {
        using SubMonthCalendar control = new()
        {
            RightToLeft = rightToLeft
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.RightToLeftLayoutChanged += handler;
        control.OnRightToLeftLayoutChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.RightToLeftLayoutChanged -= handler;
        control.OnRightToLeftLayoutChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnRightToLeftLayoutChanged_WithHandle_TestData()
    {
        yield return new object[] { RightToLeft.Yes, null, 1 };
        yield return new object[] { RightToLeft.Yes, new EventArgs(), 1 };
        yield return new object[] { RightToLeft.No, null, 0 };
        yield return new object[] { RightToLeft.No, new EventArgs(), 0 };
        yield return new object[] { RightToLeft.Inherit, null, 0 };
        yield return new object[] { RightToLeft.Inherit, new EventArgs(), 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnRightToLeftLayoutChanged_WithHandle_TestData))]
    public void MonthCalendar_OnRightToLeftLayoutChanged_InvokeWithHandle_CallsRightToLeftLayoutChanged(RightToLeft rightToLeft, EventArgs eventArgs, int expectedCreatedCallCount)
    {
        using SubMonthCalendar control = new()
        {
            RightToLeft = rightToLeft
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.RightToLeftLayoutChanged += handler;
        control.OnRightToLeftLayoutChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Remove handler.
        control.RightToLeftLayoutChanged -= handler;
        control.OnRightToLeftLayoutChanged(eventArgs);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount * 2, createdCallCount);
    }

    [WinFormsFact]
    public void MonthCalendar_OnRightToLeftLayoutChanged_InvokeInDisposing_DoesNotCallRightToLeftLayoutChanged()
    {
        using SubMonthCalendar control = new()
        {
            RightToLeft = RightToLeft.Yes
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        int callCount = 0;
        control.RightToLeftLayoutChanged += (sender, e) => callCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int disposedCallCount = 0;
        control.Disposed += (sender, e) =>
        {
            control.OnRightToLeftLayoutChanged(EventArgs.Empty);
            Assert.Equal(0, callCount);
            Assert.Equal(0, createdCallCount);
            disposedCallCount++;
        };

        control.Dispose();
        Assert.Equal(1, disposedCallCount);
    }

    [WinFormsFact]
    public void MonthCalendar_RecreateHandle_InvokeWithHandle_Success()
    {
        using SubMonthCalendar control = new();
        IntPtr handle1 = control.Handle;
        Assert.NotEqual(IntPtr.Zero, handle1);
        Assert.True(control.IsHandleCreated);

        control.RecreateHandle();
        IntPtr handle2 = control.Handle;
        Assert.NotEqual(IntPtr.Zero, handle2);
        Assert.NotEqual(handle1, handle2);
        Assert.True(control.IsHandleCreated);

        // Invoke again.
        control.RecreateHandle();
        IntPtr handle3 = control.Handle;
        Assert.NotEqual(IntPtr.Zero, handle3);
        Assert.NotEqual(handle2, handle3);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void MonthCalendar_RecreateHandle_InvokeWithoutHandle_Nop()
    {
        using SubMonthCalendar control = new();
        control.RecreateHandle();
        Assert.False(control.IsHandleCreated);

        // Invoke again.
        control.RecreateHandle();
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(1, 2)]
    [InlineData(0, 0)]
    [InlineData(-1, -2)]
    public void MonthControl_RescaleConstantsForDpi_Invoke_Nop(int deviceDpiOld, int deviceDpiNew)
    {
        using SubMonthCalendar control = new();
        control.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> SetDate_TestData()
    {
        yield return new object[] { DateTime.MinValue };
        yield return new object[] { new DateTime(1753, 1, 1).AddTicks(-1) };
        yield return new object[] { new DateTime(1753, 1, 1) };
        yield return new object[] { new DateTime(2019, 9, 1) };
        yield return new object[] { new DateTime(2019, 9, 1).AddHours(1) };
        yield return new object[] { DateTime.Now.Date };
        yield return new object[] { DateTime.Now.Date.AddHours(1) };
        yield return new object[] { new DateTime(9998, 12, 31) };
        yield return new object[] { new DateTime(9998, 12, 31).AddTicks(1) };
        yield return new object[] { DateTime.MaxValue };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetDate_TestData))]
    public void MonthCalendar_SetDate_Invoke_GetReturnsExpected(DateTime date)
    {
        using MonthCalendar calendar = new();
        calendar.SetDate(date);
        Assert.Equal(date.Date, calendar.SelectionRange.Start);
        Assert.Equal(date.Date, calendar.SelectionRange.End);
        Assert.Equal(date, calendar.SelectionStart);
        Assert.Equal(date, calendar.SelectionEnd);
        Assert.False(calendar.IsHandleCreated);

        // Call again.
        calendar.SetDate(date);
        Assert.Equal(date.Date, calendar.SelectionRange.Start);
        Assert.Equal(date.Date, calendar.SelectionRange.End);
        Assert.Equal(date, calendar.SelectionStart);
        Assert.Equal(date, calendar.SelectionEnd);
        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetDate_TestData))]
    public void MonthCalendar_SetDate_InvokeWithHandle_GetReturnsExpected(DateTime date)
    {
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.SetDate(date);
        Assert.Equal(date.Date, calendar.SelectionRange.Start);
        Assert.Equal(date.Date, calendar.SelectionRange.End);
        Assert.Equal(date, calendar.SelectionStart);
        Assert.Equal(date, calendar.SelectionEnd);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        calendar.SetDate(date);
        Assert.Equal(date.Date, calendar.SelectionRange.Start);
        Assert.Equal(date.Date, calendar.SelectionRange.End);
        Assert.Equal(date, calendar.SelectionStart);
        Assert.Equal(date, calendar.SelectionEnd);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void MonthCalendar_SetDate_DateLessThanMinDate_ThrowsArgumentOutOfRangeException()
    {
        using MonthCalendar calendar = new();
        calendar.SetDate(calendar.MinDate.AddTicks(-1));
        Assert.Equal(calendar.MinDate.AddTicks(-1), calendar.SelectionStart);
        Assert.Equal(calendar.MinDate.AddTicks(-1), calendar.SelectionEnd);

        calendar.MinDate = new DateTime(2019, 10, 3);
        Assert.Throws<ArgumentOutOfRangeException>("date", () => calendar.SetDate(calendar.MinDate.AddTicks(-1)));
    }

    [WinFormsFact]
    public void MonthCalendar_SetDate_DateGreaterThanMaxDate_ThrowsArgumentOutOfRangeException()
    {
        using MonthCalendar calendar = new();
        calendar.SetDate(calendar.MaxDate.AddTicks(1));
        Assert.Equal(calendar.MaxDate.AddTicks(1), calendar.SelectionStart);
        Assert.Equal(calendar.MaxDate.AddTicks(1), calendar.SelectionEnd);

        calendar.MaxDate = new DateTime(2019, 9, 3);
        Assert.Throws<ArgumentOutOfRangeException>("date", () => calendar.SetDate(calendar.MaxDate.AddDays(1)));
    }

    public static IEnumerable<object[]> SetSelectionRange_TestData()
    {
        yield return new object[] { DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue };
        yield return new object[] { new DateTime(1753, 1, 1).AddTicks(-1), new DateTime(1753, 1, 1).AddTicks(-1), new DateTime(1753, 1, 1).AddTicks(-1), new DateTime(1753, 1, 1).AddTicks(-1) };
        yield return new object[] { new DateTime(1753, 1, 1), new DateTime(1753, 1, 1), new DateTime(1753, 1, 1), new DateTime(1753, 1, 1) };
        yield return new object[] { new DateTime(1753, 1, 1), new DateTime(1753, 1, 2), new DateTime(1753, 1, 1), new DateTime(1753, 1, 2) };

        yield return new object[] { new DateTime(2019, 9, 1), new DateTime(2019, 9, 1), new DateTime(2019, 9, 1), new DateTime(2019, 9, 1) };
        yield return new object[] { new DateTime(2019, 9, 1), new DateTime(2019, 9, 2), new DateTime(2019, 9, 1), new DateTime(2019, 9, 2) };
        yield return new object[] { new DateTime(2019, 9, 1).AddHours(1), new DateTime(2019, 9, 2).AddHours(1), new DateTime(2019, 9, 1).AddHours(1), new DateTime(2019, 9, 2).AddHours(1) };
        yield return new object[] { new DateTime(2019, 9, 2), new DateTime(2019, 9, 1), new DateTime(2019, 9, 2), new DateTime(2019, 9, 2) };
        yield return new object[] { new DateTime(2019, 9, 1), new DateTime(2019, 9, 7), new DateTime(2019, 9, 1), new DateTime(2019, 9, 7) };
        yield return new object[] { new DateTime(2019, 9, 1), new DateTime(2019, 9, 8), new DateTime(2019, 9, 1), new DateTime(2019, 9, 7) };

        yield return new object[] { DateTime.Now.Date, DateTime.Now.Date, DateTime.Now.Date, DateTime.Now.Date };
        yield return new object[] { DateTime.Now.Date, DateTime.Now.Date.AddDays(1), DateTime.Now.Date, DateTime.Now.Date.AddDays(1) };
        yield return new object[] { DateTime.Now.Date.AddHours(1), DateTime.Now.Date.AddHours(1), DateTime.Now.Date.AddHours(1), DateTime.Now.Date.AddHours(1) };
        yield return new object[] { DateTime.Now.Date.AddDays(1), DateTime.Now.Date, DateTime.Now.Date.AddDays(1), DateTime.Now.Date.AddDays(1) };
        yield return new object[] { DateTime.Now.Date, DateTime.Now.Date.AddDays(6), DateTime.Now.Date, DateTime.Now.Date.AddDays(6) };
        yield return new object[] { DateTime.Now.Date, DateTime.Now.Date.AddDays(7), DateTime.Now.Date.AddDays(1), DateTime.Now.Date.AddDays(7) };

        yield return new object[] { new DateTime(9998, 12, 30), new DateTime(9998, 12, 31), new DateTime(9998, 12, 30), new DateTime(9998, 12, 31) };
        yield return new object[] { new DateTime(9998, 12, 31), new DateTime(9998, 12, 31), new DateTime(9998, 12, 31), new DateTime(9998, 12, 31) };
        yield return new object[] { new DateTime(9998, 12, 31).AddTicks(1), new DateTime(9998, 12, 31).AddTicks(1), new DateTime(9998, 12, 31).AddTicks(1), new DateTime(9998, 12, 31).AddTicks(1) };
        yield return new object[] { DateTime.MaxValue, DateTime.MaxValue, DateTime.MaxValue, DateTime.MaxValue };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetSelectionRange_TestData))]
    public void MonthCalendar_SetSelectionRange_Invoke_GetReturnsExpected(DateTime date1, DateTime date2, DateTime expectedSelectionStart, DateTime expectedSelectionEnd)
    {
        using MonthCalendar calendar = new();
        calendar.SetSelectionRange(date1, date2);
        Assert.Equal(expectedSelectionStart.Date, calendar.SelectionRange.Start);
        Assert.Equal(expectedSelectionEnd.Date, calendar.SelectionRange.End);
        Assert.Equal(expectedSelectionStart, calendar.SelectionStart);
        Assert.Equal(expectedSelectionEnd, calendar.SelectionEnd);
        Assert.False(calendar.IsHandleCreated);

        // Call again.
        calendar.SetSelectionRange(expectedSelectionStart, expectedSelectionEnd);
        Assert.Equal(expectedSelectionStart.Date, calendar.SelectionRange.Start);
        Assert.Equal(expectedSelectionEnd.Date, calendar.SelectionRange.End);
        Assert.Equal(expectedSelectionStart, calendar.SelectionStart);
        Assert.Equal(expectedSelectionEnd, calendar.SelectionEnd);
        Assert.False(calendar.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetSelectionRange_TestData))]
    public void MonthCalendar_SetSelectionRange_InvokeWithHandle_GetReturnsExpected(DateTime date1, DateTime date2, DateTime expectedSelectionStart, DateTime expectedSelectionEnd)
    {
        using MonthCalendar calendar = new();
        Assert.NotEqual(IntPtr.Zero, calendar.Handle);
        int invalidatedCallCount = 0;
        calendar.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        calendar.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        calendar.HandleCreated += (sender, e) => createdCallCount++;

        calendar.SetSelectionRange(date1, date2);
        Assert.Equal(expectedSelectionStart.Date, calendar.SelectionRange.Start);
        Assert.Equal(expectedSelectionEnd.Date, calendar.SelectionRange.End);
        Assert.Equal(expectedSelectionStart, calendar.SelectionStart);
        Assert.Equal(expectedSelectionEnd, calendar.SelectionEnd);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        calendar.SetSelectionRange(expectedSelectionStart, expectedSelectionEnd);
        Assert.Equal(expectedSelectionStart.Date, calendar.SelectionRange.Start);
        Assert.Equal(expectedSelectionEnd.Date, calendar.SelectionRange.End);
        Assert.Equal(expectedSelectionStart, calendar.SelectionStart);
        Assert.Equal(expectedSelectionEnd, calendar.SelectionEnd);
        Assert.True(calendar.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void MonthCalendar_SetSelectionRange_DateLessThanMinDate_ThrowsArgumentOutOfRangeException()
    {
        using MonthCalendar calendar = new();
        calendar.SetSelectionRange(calendar.MinDate.AddTicks(-1), calendar.MinDate);
        Assert.Equal(calendar.MinDate.AddTicks(-1), calendar.SelectionStart);
        Assert.Equal(calendar.MinDate, calendar.SelectionEnd);

        calendar.SetSelectionRange(calendar.MinDate, calendar.MinDate.AddTicks(-1));
        Assert.Equal(calendar.MinDate, calendar.SelectionStart);
        Assert.Equal(calendar.MinDate, calendar.SelectionEnd);

        calendar.MinDate = new DateTime(2019, 10, 3);
        Assert.Throws<ArgumentOutOfRangeException>("date1", () => calendar.SetSelectionRange(calendar.MinDate.AddTicks(-1), calendar.MinDate));
        Assert.Throws<ArgumentOutOfRangeException>("date2", () => calendar.SetSelectionRange(calendar.MinDate, calendar.MinDate.AddTicks(-1)));
    }

    [WinFormsFact]
    public void MonthCalendar_SetSelectionRange_DateGreaterThanMaxDate_ThrowsArgumentOutOfRangeException()
    {
        using MonthCalendar calendar = new();
        calendar.SetSelectionRange(calendar.MaxDate.AddTicks(1), calendar.MaxDate);
        Assert.Equal(calendar.MaxDate.AddTicks(1), calendar.SelectionStart);
        Assert.Equal(calendar.MaxDate.AddTicks(1), calendar.SelectionEnd);

        calendar.SetSelectionRange(calendar.MaxDate, calendar.MaxDate.AddTicks(1));
        Assert.Equal(calendar.MaxDate, calendar.SelectionStart);
        Assert.Equal(calendar.MaxDate.AddTicks(1), calendar.SelectionEnd);

        calendar.MaxDate = new DateTime(2019, 9, 3);
        Assert.Throws<ArgumentOutOfRangeException>("date1", () => calendar.SetSelectionRange(calendar.MaxDate.AddDays(1), calendar.MaxDate));
        Assert.Throws<ArgumentOutOfRangeException>("date2", () => calendar.SetSelectionRange(calendar.MaxDate, calendar.MaxDate.AddDays(1)));
    }

    public static IEnumerable<object[]> MonthCalendar_FillMonthDayStates_ReturnsExpected_TestData()
    {
        // This test set of dates is designed for a specific test case:
        // when a calendar has 12 fully visible months + 2 not fully visible.
        // This test calendar has (08/29/2021 - 09/10/2022) dates range.

        yield return new object[] { new DateTime(2021, 8, 31) }; // Make this date of the not fully visible previous month bold

        // Make visible dates of 2021 year (Sep - Dec) bold
        for (int i = 9; i <= 12; i++)
        {
            yield return new object[] { new DateTime(2021, i, i) };
        }

        // Make visible dates of 2022 year (Jan - Aug) bold
        for (int i = 1; i <= 8; i++)
        {
            yield return new object[] { new DateTime(2022, i, i) };
        }

        yield return new object[] { new DateTime(2022, 9, 1) }; // Make this date of the not fully visible last month bold
    }

    [WinFormsTheory]
    [MemberData(nameof(MonthCalendar_FillMonthDayStates_ReturnsExpected_TestData))]
    public unsafe void MonthCalendar_FillMonthDayStates_ReturnsExpected(DateTime currentDate)
    {
        const int MonthsInYear = 12;
        // Create a calendar with (600x600) size, that contains 3 columns and 4 row of months (12 months total).
        // Set the first day of week to have a stable test case in different environments.
        using MonthCalendar calendar = new() { Size = new Size(600, 600), FirstDayOfWeek = Day.Sunday };

        calendar.CreateControl();
        // Set a visible range (08/29/2021 - 09/10/2022) to have a stable test case
        calendar.SetSelectionRange(new DateTime(2021, 9, 1), new DateTime(2022, 8, 31));
        MONTH_CALDENDAR_MESSAGES_VIEW view = calendar.TestAccessor().Dynamic._mcCurView;
        SelectionRange displayRange = calendar.GetDisplayRange(visible: false);

        Assert.Equal(MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH, view);
        Assert.Equal(new DateTime(2021, 8, 29), displayRange.Start);
        Assert.Equal(new DateTime(2022, 9, 10), displayRange.End);

        int monthsCount = calendar.TestAccessor().Dynamic.GetMonthsCountOfRange(displayRange);
        int currentMonthIndex = (currentDate.Year - displayRange.Start.Year) * MonthsInYear + currentDate.Month - displayRange.Start.Month;
        calendar.AddBoldedDate(currentDate);
        Span<uint> boldedDates = stackalloc uint[monthsCount];

        calendar.FillMonthDayStates(boldedDates, displayRange);

        uint expectedState = 1U << (currentDate.Day - 1);
        uint actualState = boldedDates[currentMonthIndex] & expectedState;

        Assert.Equal(expectedState, actualState);
    }

    public static IEnumerable<object[]> MonthCalendar_GetIndexInMonths_ReturnsExpected_TestData()
    {
        int expectedIndex = 0;

        // This test set of dates is designed to check dates in different years.
        // The start date is 08/01/2021.

        // Check dates of 2021 year (Aug - Dec)
        for (int i = 8; i <= 12; i++)
        {
            yield return new object[] { new DateTime(2021, i, i), expectedIndex++ };
        }

        // Check dates of 2022 year (Jan - Sep) bold
        for (int i = 1; i <= 9; i++)
        {
            yield return new object[] { new DateTime(2022, i, i), expectedIndex++ };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(MonthCalendar_GetIndexInMonths_ReturnsExpected_TestData))]
    public unsafe void MonthCalendar_GetIndexInMonths_ReturnsExpected(DateTime currentDate, int expectedIndex)
    {
        DateTime startDate = new(2021, 8, 1);
        using MonthCalendar calendar = new();
        int actualIndex = calendar.TestAccessor().Dynamic.GetIndexInMonths(startDate, currentDate);

        Assert.Equal(expectedIndex, actualIndex);
    }

    [WinFormsFact]
    public void SelectionRange_CopyConstructor_CopiesStartAndEndDatesCorrectly()
    {
        DateTime startDate = new(2023, 1, 1);
        DateTime endDate = new(2023, 12, 31);
        SelectionRange originalRange = new(startDate, endDate);
        SelectionRange copiedRange = new(originalRange);

        copiedRange.Start.Should().Be(startDate);
        copiedRange.End.Should().Be(endDate);
    }

    [WinFormsFact]
    public void GetFocused_Returns_FocusedCellAccessibleObject()
    {
        using MonthCalendar monthCalendar = new();

        MonthCalendarAccessibleObject accessibleObject = new(monthCalendar);
        var result = accessibleObject.GetFocused();

        result.Should().Be(accessibleObject.FocusedCell);
    }

    [WinFormsFact]
    public void MonthCalendarAccessibleObject_Help_ReturnsExpected()
    {
        using MonthCalendar monthCalendar = new();

        monthCalendar.CreateControl();
        MonthCalendarAccessibleObject accessibleObject = new(monthCalendar);
        string expectedHelp = "MonthCalendar(Control)";

        accessibleObject.Help.Should().Be(expectedHelp);
    }

    [WinFormsTheory]
    [InlineData("42")]
    public void CalendarWeekNumberCellAccessibleObject_Name_ReturnsExpected(string weekNumber)
    {
        using MonthCalendar monthCalendar = new();

        monthCalendar.CreateControl();
        MonthCalendarAccessibleObject monthCalendarAccessibleObject = new(monthCalendar);
        CalendarAccessibleObject calendarAccessibleObject = new(monthCalendarAccessibleObject, 1, "Main Calendar");
        CalendarBodyAccessibleObject calendarBodyAccessibleObject = new(calendarAccessibleObject, monthCalendarAccessibleObject, 1);
        CalendarRowAccessibleObject calendarRowAccessibleObject = new(calendarBodyAccessibleObject, monthCalendarAccessibleObject, 1, 1);

        CalendarWeekNumberCellAccessibleObject accessibleObject = new(
            calendarRowAccessibleObject,
            calendarBodyAccessibleObject,
            monthCalendarAccessibleObject,
            calendarIndex: 0,
            rowIndex: 0,
            columnIndex: 0,
            weekNumber);

        string name = accessibleObject.Name;

        name.Should().Be($"Week {weekNumber}");
    }

    [WinFormsFact]
    public void CalendarTodayLinkAccessibleObject_Bounds_ReturnsExpected()
    {
        using MonthCalendar monthCalendar = new();

        monthCalendar.CreateControl();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;
        CalendarTodayLinkAccessibleObject todayLinkAccessibleObject = new(controlAccessibleObject);
        Rectangle actual = todayLinkAccessibleObject.Bounds;
        Rectangle expected = controlAccessibleObject.GetCalendarPartRectangle(MCGRIDINFO_PART.MCGIP_FOOTER);

        actual.Should().Be(expected);
    }

    [WinFormsFact]
    public void CalendarPreviousButtonAccessibleObject_Bounds_ReturnsExpected()
    {
        using MonthCalendar monthCalendar = new();

        monthCalendar.CreateControl();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;
        CalendarPreviousButtonAccessibleObject previousButtonAccessibleObject = new(controlAccessibleObject);
        Rectangle bounds = previousButtonAccessibleObject.Bounds;
        Rectangle expectedBounds = new(13, 42, 16, 16);

        bounds.Should().Be(expectedBounds);
    }

    private (CalendarRowAccessibleObject, CalendarCellAccessibleObject) CreateCalendarObjects(MonthCalendar control, int calendarIndex = 0, int rowIndex = 0, int columnIndex = 0)
    {
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, calendarIndex, "Main Calendar");
        CalendarBodyAccessibleObject bodyAccessibleObject = new(calendarAccessibleObject, controlAccessibleObject, calendarIndex);
        CalendarRowAccessibleObject rowAccessibleObject = new(bodyAccessibleObject, controlAccessibleObject, calendarIndex, rowIndex);
        CalendarCellAccessibleObject cellAccessibleObject = new(rowAccessibleObject, bodyAccessibleObject, controlAccessibleObject, calendarIndex, rowIndex, columnIndex);

        return (rowAccessibleObject, cellAccessibleObject);
    }

    [WinFormsFact]
    public void CalendarRowAccessibleObject_Bounds_ReturnsExpected()
    {
        using MonthCalendar monthCalendar = new();

        monthCalendar.CreateControl();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;
        var (rowAccessibleObject, _) = CreateCalendarObjects(monthCalendar);
        Rectangle actual = rowAccessibleObject.Bounds;
        Rectangle expected = controlAccessibleObject.GetCalendarPartRectangle(MCGRIDINFO_PART.MCGIP_CALENDARROW, 0, 0);

        actual.Should().Be(expected);
    }

    [WinFormsTheory]
    [InlineData(0, "Week 23, Monday")]
    [InlineData(1, null)]
    public void CalendarCellAccessibleObject_Description_ReturnsExpected(int view, string expected)
    {
        using MonthCalendar monthCalendar = new();

        monthCalendar.CreateControl();
        monthCalendar.FirstDayOfWeek = Day.Monday;
        monthCalendar.SelectionStart = new DateTime(2021, 6, 16);
        PInvokeCore.SendMessage(monthCalendar, PInvoke.MCM_SETCURRENTVIEW, 0, view);

        var (_, cellAccessibleObject) = CreateCalendarObjects(monthCalendar);

        cellAccessibleObject.Description.Should().Be(expected);
    }

    [WinFormsFact]
    public void CalendarCellAccessibleObject_Select_SetsSelectionRange()
    {
        using MonthCalendar monthCalendar = new();

        monthCalendar.CreateControl();
        var (_, cellAccessibleObject) = CreateCalendarObjects(monthCalendar);

        DateTime startDate = new(2022, 10, 1);
        DateTime endDate = new(2022, 10, 7);
        cellAccessibleObject.TestAccessor().Dynamic._dateRange = new SelectionRange(startDate, endDate);

        cellAccessibleObject.Select(AccessibleSelection.TakeSelection);

        monthCalendar.SelectionStart.Should().Be(startDate);
        monthCalendar.SelectionEnd.Should().Be(endDate);
    }

    [WinFormsTheory]
    [InlineData(AccessibleStates.Focusable | AccessibleStates.Selectable | AccessibleStates.Selected)]
    public void CalendarCellAccessibleObject_State_ReturnsExpected(AccessibleStates expectedState)
    {
        using MonthCalendar monthCalendar = new();

        monthCalendar.CreateControl();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;
        var (_, cellAccessibleObject) = CreateCalendarObjects(monthCalendar);

        bool shouldFocus = expectedState.HasFlag(AccessibleStates.Focused);
        bool selectExactRange = expectedState.HasFlag(AccessibleStates.Selected);

        if (shouldFocus)
        {
            monthCalendar.Focus();
        }

        if (selectExactRange)
        {
            monthCalendar.SetSelectionRange(cellAccessibleObject.DateRange.Start, cellAccessibleObject.DateRange.End);
        }
        else
        {
            monthCalendar.SetSelectionRange(cellAccessibleObject.DateRange.Start.AddDays(-1), cellAccessibleObject.DateRange.End.AddDays(1));
        }

        cellAccessibleObject.State.Should().Be(expectedState);
    }

    [WinFormsTheory]
    [InlineData(AccessibleSelection.AddSelection)]
    [InlineData(AccessibleSelection.RemoveSelection)]
    [InlineData(AccessibleSelection.TakeSelection)]
    public void CalendarCellAccessibleObject_Select_Invoke_SetsSelectionRange(AccessibleSelection selectionFlag)
    {
        using MonthCalendar monthCalendar = new();
        monthCalendar.CreateControl();
        var (_, cellAccessibleObject) = CreateCalendarObjects(monthCalendar);

        DateTime expectedStart = cellAccessibleObject.DateRange.Start;
        DateTime expectedEnd = cellAccessibleObject.DateRange.End;

        cellAccessibleObject.Select(selectionFlag);
        monthCalendar.SelectionStart.Should().Be(expectedStart);
        monthCalendar.SelectionEnd.Should().Be(expectedEnd);
    }

    [WinFormsFact]
    public void CalendarCellAccessibleObject_Role_ReturnsExpected()
    {
        using MonthCalendar monthCalendar = new();

        monthCalendar.CreateControl();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;
        var (_, cellAccessibleObject) = CreateCalendarObjects(monthCalendar);

        cellAccessibleObject.Role.Should().Be(AccessibleRole.Cell);
    }

    private MonthCalendar InitializeCalendarWithDates()
    {
        MonthCalendar calendar = new();

        calendar.AddAnnuallyBoldedDate(new DateTime(2022, 1, 1));
        calendar.AddAnnuallyBoldedDate(new DateTime(2022, 2, 2));
        calendar.AddBoldedDate(new DateTime(2022, 1, 1));
        calendar.AddBoldedDate(new DateTime(2022, 2, 2));
        calendar.AddMonthlyBoldedDate(new DateTime(2022, 1, 1));
        calendar.AddMonthlyBoldedDate(new DateTime(2022, 2, 2));

        return calendar;
    }

    [WinFormsTheory]
    [InlineData("RemoveAllAnnuallyBoldedDates", 0, "AnnuallyBoldedDates")]
    [InlineData("RemoveAnnuallyBoldedDate", 1, "AnnuallyBoldedDates")]
    [InlineData("RemoveAllBoldedDates", 0, "BoldedDates")]
    [InlineData("RemoveBoldedDate", 1, "BoldedDates")]
    [InlineData("RemoveAllMonthlyBoldedDates", 0, "MonthlyBoldedDates")]
    [InlineData("RemoveMonthlyBoldedDate", 1, "MonthlyBoldedDates")]
    public void MonthCalendar_RemoveBoldedDates_Invoke_Success(string methodName, int expectedCount, string propertyName)
    {
        using MonthCalendar calendar = InitializeCalendarWithDates();

        typeof(MonthCalendar).GetMethod(methodName).Invoke(calendar, methodName.Contains("All") ? null : new object[] { new DateTime(2022, 1, 1) });
        ((DateTime[])typeof(MonthCalendar).GetProperty(propertyName).GetValue(calendar)).Length.Should().Be(expectedCount);
    }

    [WinFormsTheory]
    [InlineData(0, 0, HitArea.Nowhere, "0001-01-01T00:00:00")]
    [InlineData(100, 100, HitArea.Date, null)]
    [InlineData(-100, -100, HitArea.Nowhere, "0001-01-01T00:00:00")]
    public void MonthCalendar_HitTest_ReturnsExpected(int x, int y, HitArea expectedHitArea, string expectedTimeString)
    {
        using MonthCalendar calendar = new();
        HitTestInfo hitTestInfo = calendar.HitTest(new Point(x, y));

        hitTestInfo.Point.X.Should().Be(x);
        hitTestInfo.Point.Y.Should().Be(y);
        hitTestInfo.HitArea.Should().Be(expectedHitArea);

        DateTime? expectedTime = expectedTimeString is null ? null : DateTime.Parse(expectedTimeString);

        if (expectedTime is null)
        {
            hitTestInfo.Time.Should().NotBe(DateTime.MinValue);
        }
        else
        {
            hitTestInfo.Time.Should().Be(expectedTime.Value);
        }
    }

    [WinFormsFact]
    public void MonthCalendar_ShouldSerializeProperties_ReturnsExpected()
    {
        using MonthCalendar calendar = new();
        calendar.MaxDate = DateTime.MaxValue;

        ShouldSerializeProperty(() => calendar.TestAccessor().Dynamic.ShouldSerializeTodayDate(), () => calendar.TodayDate = DateTime.Now);
        ShouldSerializeProperty(() => calendar.TestAccessor().Dynamic.ShouldSerializeAnnuallyBoldedDates(), () => calendar.AddAnnuallyBoldedDate(DateTime.Now));
        ShouldSerializeProperty(() => calendar.TestAccessor().Dynamic.ShouldSerializeBoldedDates(), () => calendar.AddBoldedDate(DateTime.Now));
        ShouldSerializeProperty(() => calendar.TestAccessor().Dynamic.ShouldSerializeMonthlyBoldedDates(), () => calendar.AddMonthlyBoldedDate(DateTime.Now));
        ShouldSerializeProperty(() => calendar.TestAccessor().Dynamic.ShouldSerializeMaxDate(), () => calendar.MaxDate = DateTime.Now.AddYears(2));
        ShouldSerializeProperty(() => calendar.TestAccessor().Dynamic.ShouldSerializeMinDate(), () => calendar.MinDate = DateTime.Now);
        ShouldSerializeProperty(() => calendar.TestAccessor().Dynamic.ShouldSerializeTrailingForeColor(), () => calendar.TrailingForeColor = Color.Red);
        ShouldSerializeProperty(() => calendar.TestAccessor().Dynamic.ShouldSerializeTitleForeColor(), () => calendar.TitleForeColor = Color.Red);
        ShouldSerializeProperty(() => calendar.TestAccessor().Dynamic.ShouldSerializeTitleBackColor(), () => calendar.TitleBackColor = Color.Red);
    }

    private void ShouldSerializeProperty(Func<bool> shouldSerializeFunc, Action setPropertyAction)
    {
        bool result1 = shouldSerializeFunc();
        result1.Should().BeFalse();

        setPropertyAction();

        bool result2 = shouldSerializeFunc();
        result2.Should().BeTrue();
    }

    [WinFormsTheory]
    [BoolData]
    public void MonthCalendar_ResetTodayDate_InvokeWithSetTodayDate_ReturnsExpected(bool createHandle)
    {
        using MonthCalendar calendar = new();
        calendar.TodayDate = new DateTime(2000, 1, 1);

        if (createHandle)
        {
            nint handle = calendar.Handle;
        }

        DateTime todayDateBeforeReset = calendar.TodayDate;
        calendar.TestAccessor().Dynamic.ResetTodayDate();
        DateTime todayDateAfterReset = calendar.TodayDate;

        todayDateBeforeReset.Should().NotBe(todayDateAfterReset);
        todayDateAfterReset.Should().Be(DateTime.Today);
    }

    [WinFormsTheory]
    [BoolData]
    public void MonthCalendar_ResetTodayDate_InvokeWithoutSetTodayDate_ReturnsExpected(bool createHandle)
    {
        using MonthCalendar calendar = new();

        if (createHandle)
        {
            nint handle = calendar.Handle;
        }

        DateTime todayDateBeforeReset = calendar.TodayDate;
        calendar.TestAccessor().Dynamic.ResetTodayDate();
        DateTime todayDateAfterReset = calendar.TodayDate;

        todayDateBeforeReset.Should().Be(todayDateAfterReset);
    }

    private void TestResetColorProperty(Func<MonthCalendar, Color> getColor, Action<MonthCalendar, Color> setColor, Action<dynamic> resetColor)
    {
        using MonthCalendar calendar = new();
        var testAccessor = calendar.TestAccessor();

        Color originalColor = getColor(calendar);

        setColor(calendar, Color.Red);
        getColor(calendar).Should().Be(Color.Red);

        resetColor(testAccessor.Dynamic);
        getColor(calendar).Should().Be(originalColor);

        setColor(calendar, Color.Red);
        calendar.CreateControl();
        resetColor(testAccessor.Dynamic);
        getColor(calendar).Should().Be(originalColor);
    }

    [WinFormsFact]
    public void MonthCalendar_ResetTitleBackColor_Invoke_Success()
    {
        TestResetColorProperty(
            getColor: calendar => calendar.TitleBackColor,
            setColor: (calendar, color) => calendar.TitleBackColor = color,
            resetColor: dynamicAccessor => dynamicAccessor.ResetTitleBackColor()
        );
    }

    [WinFormsFact]
    public void MonthCalendar_ResetTrailingForeColor_Invoke_Success()
    {
        TestResetColorProperty(
            getColor: calendar => calendar.TrailingForeColor,
            setColor: (calendar, color) => calendar.TrailingForeColor = color,
            resetColor: dynamicAccessor => dynamicAccessor.ResetTrailingForeColor()
        );
    }

    [WinFormsFact]
    public void MonthCalendar_ResetTitleForeColor_Invoke_Success()
    {
        TestResetColorProperty(
            getColor: calendar => calendar.TitleForeColor,
            setColor: (calendar, color) => calendar.TitleForeColor = color,
            resetColor: dynamicAccessor => dynamicAccessor.ResetTitleForeColor()
        );
    }

    private void TestResetProperty<T>(MonthCalendar calendar, Action<MonthCalendar, T> setProperty, Func<MonthCalendar, T> getProperty, T testValue, T expectedValue, Action<dynamic> resetProperty)
    {
        setProperty(calendar, testValue);
        getProperty(calendar).Should().Be(testValue);

        resetProperty(calendar.TestAccessor().Dynamic);
        getProperty(calendar).Should().Be(expectedValue);
    }

    [WinFormsFact]
    public void MonthCalendar_ResetSelectionRange_Invoke_Success()
    {
        using MonthCalendar calendar = new();
        DateTime originalDate = DateTime.Now.Date;
        TestResetProperty(
            calendar,
            (cal, range) => cal.SetSelectionRange(range.Item1, range.Item2),
            cal => (cal.SelectionStart, cal.SelectionEnd),
            (originalDate.AddDays(-1), originalDate.AddDays(1)),
            (originalDate, originalDate),
            dynamicAccessor => dynamicAccessor.ResetSelectionRange()
        );
    }

    [WinFormsFact]
    public void MonthCalendar_ResetMonthlyBoldedDates_Invoke_Success()
    {
        using MonthCalendar calendar = new();
        TestResetProperty(
            calendar,
            (cal, date) => cal.AddMonthlyBoldedDate(date),
            cal => cal.MonthlyBoldedDates.FirstOrDefault(),
            DateTime.Now,
            DateTime.MinValue,
            dynamicAccessor => dynamicAccessor.ResetMonthlyBoldedDates()
        );
    }

    [WinFormsFact]
    public void MonthCalendar_ResetMaxDate_Invoke_Success()
    {
        using MonthCalendar calendar = new();
        calendar.MaxDate = new DateTime(2022, 12, 31);

        calendar.TestAccessor().Dynamic.ResetMaxDate();

        calendar.MaxDate.Should().Be(new DateTime(9998, 12, 31));
    }

    [WinFormsFact]
    public void MonthCalendar_ResetCalendarDimensions_Invoke_Success()
    {
        using MonthCalendar calendar = new();
        calendar.CalendarDimensions = new Size(2, 2);

        calendar.TestAccessor().Dynamic.ResetCalendarDimensions();

        calendar.CalendarDimensions.Should().Be(new Size(1, 1));
    }

    [WinFormsFact]
    public void MonthCalendar_ResetAnnuallyBoldedDates_Invoke_Success()
    {
        using MonthCalendar calendar = new();
        calendar.AddAnnuallyBoldedDate(DateTime.Now);

        calendar.TestAccessor().Dynamic.ResetAnnuallyBoldedDates();

        calendar.AnnuallyBoldedDates.Should().BeEmpty();
    }

    [WinFormsFact]
    public void MonthCalendar_ResetBoldedDates_Invoke_Success()
    {
        using MonthCalendar calendar = new();
        calendar.AddBoldedDate(DateTime.Now);

        calendar.TestAccessor().Dynamic.ResetBoldedDates();

        calendar.BoldedDates.Should().BeEmpty();
    }

    [WinFormsFact]
    public void MonthCalendar_OnCalendarViewChanged_Invoke_Success()
    {
        using MonthCalendar calendar = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(calendar);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        calendar.TestAccessor().Dynamic._onCalendarViewChanged += handler;
        calendar.TestAccessor().Dynamic.OnCalendarViewChanged(EventArgs.Empty);
        callCount.Should().Be(1);

        calendar.TestAccessor().Dynamic._onCalendarViewChanged -= handler;
        calendar.TestAccessor().Dynamic.OnCalendarViewChanged(EventArgs.Empty);
        callCount.Should().Be(1);
    }

    private class SubMonthCalendar : MonthCalendar
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

        public new AccessibleObject CreateAccessibilityInstance() => base.CreateAccessibilityInstance();

        public new void CreateHandle() => base.CreateHandle();

        public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new bool GetTopLevel() => base.GetTopLevel();

        public new void OnBackColorChanged(EventArgs e) => base.OnBackColorChanged(e);

        public new void OnClick(EventArgs e) => base.OnClick(e);

        public new void OnDateChanged(DateRangeEventArgs e) => base.OnDateChanged(e);

        public new void OnDateSelected(DateRangeEventArgs e) => base.OnDateSelected(e);

        public new void OnDoubleClick(EventArgs e) => base.OnDoubleClick(e);

        public new void OnFontChanged(EventArgs e) => base.OnFontChanged(e);

        public new void OnForeColorChanged(EventArgs e) => base.OnForeColorChanged(e);

        public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

        public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

        public new void OnMouseClick(MouseEventArgs e) => base.OnMouseClick(e);

        public new void OnMouseDoubleClick(MouseEventArgs e) => base.OnMouseDoubleClick(e);

        public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);

        public new void OnRightToLeftLayoutChanged(EventArgs e) => base.OnRightToLeftLayoutChanged(e);

        public new void RecreateHandle() => base.RecreateHandle();

        public new void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew) => base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);

        public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
    }
}
