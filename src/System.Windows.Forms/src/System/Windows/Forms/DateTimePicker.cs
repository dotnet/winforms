// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;
using Microsoft.Win32;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Date/DateTime picker control
    /// </summary>
    [DefaultProperty(nameof(Value))]
    [DefaultEvent(nameof(ValueChanged))]
    [DefaultBindingProperty(nameof(Value))]
    [Designer("System.Windows.Forms.Design.DateTimePickerDesigner, " + AssemblyRef.SystemDesign)]
    [SRDescription(nameof(SR.DescriptionDateTimePicker))]
    public class DateTimePicker : Control
    {
        /// <summary>
        ///  Specifies the default title back color. This field is read-only.
        /// </summary>
        protected static readonly Color DefaultTitleBackColor = SystemColors.ActiveCaption;
        /// <summary>
        ///  Specifies the default foreground color. This field is read-only.
        /// </summary>
        protected static readonly Color DefaultTitleForeColor = SystemColors.ActiveCaptionText;
        /// <summary>
        ///  Specifies the default month background color. This field is read-only.
        /// </summary>
        protected static readonly Color DefaultMonthBackColor = SystemColors.Window;
        /// <summary>
        ///  Specifies the default trailing forground color. This field is read-only.
        /// </summary>
        protected static readonly Color DefaultTrailingForeColor = SystemColors.GrayText;

        private static readonly object s_formatChangedEvent = new object();

        private static readonly string s_dateTimePickerLocalizedControlTypeString = SR.DateTimePickerLocalizedControlType;

        private const DTS TIMEFORMAT_NOUPDOWN = DTS.TIMEFORMAT & (~DTS.UPDOWN);
        private EventHandler _onCloseUp;
        private EventHandler _onDropDown;
        private EventHandler _onValueChanged;
        private EventHandler _onRightToLeftLayoutChanged;

        // We need to restrict the available dates because of limitations in the comctl
        // DateTime and MonthCalendar controls
        //
        /// <summary>
        ///  Specifies the minimum date value. This field is read-only.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DateTime MinDateTime = new DateTime(1753, 1, 1);

        /// <summary>
        ///  Specifies the maximum date value. This field is read-only.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DateTime MaxDateTime = new DateTime(9998, 12, 31);

        private DTS _style;
        private short prefHeightCache = -1;

        /// <summary>
        ///  validTime determines whether the CheckBox in the DTP is checked.  The CheckBox is only
        ///  displayed when ShowCheckBox is true.
        /// </summary>
        private bool validTime = true;

        // DateTime changeover: DateTime is a value class, not an object, so we need to keep track
        // of whether or not its values have been initialised in a separate boolean.
        private bool userHasSetValue;
        private DateTime value = DateTime.Now;
        private DateTime creationTime = DateTime.Now;
        // Reconcile out-of-range min/max values in the property getters.
        private DateTime max = DateTime.MaxValue;
        private DateTime min = DateTime.MinValue;
        private Color calendarForeColor = DefaultForeColor;
        private Color calendarTitleBackColor = DefaultTitleBackColor;
        private Color calendarTitleForeColor = DefaultTitleForeColor;
        private Color calendarMonthBackground = DefaultMonthBackColor;
        private Color calendarTrailingText = DefaultTrailingForeColor;
        private Font calendarFont;
        private FontHandleWrapper calendarFontHandleWrapper;

        // Since there is no way to get the customFormat from the DTP, we need to
        // cache it. Also we have to track if the user wanted customFormat or
        // shortDate format (shortDate is the lack of being in Long or DateTime format
        // without a customFormat). What fun!
        //
        private string customFormat;

        private DateTimePickerFormat format;

        private bool rightToLeftLayout;

        /// <summary>
        ///  Initializes a new instance of the <see cref='DateTimePicker'/> class.
        /// </summary>
        public DateTimePicker()
        : base()
        {
            // this class overrides GetPreferredSizeCore, let Control automatically cache the result
            SetExtendedState(ExtendedStates.UserPreferredSizeCache, true);

            SetStyle(ControlStyles.FixedHeight, true);

            // Since DateTimePicker does its own mouse capturing, we do not want
            // to receive standard click events, or we end up with mismatched mouse
            // button up and button down messages.
            //
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.StandardClick, false);

            // Set default flags here...
            //
            format = DateTimePickerFormat.Long;

            SetStyle(ControlStyles.UseTextForAccessibility, false);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Color BackColor
        {
            get
            {
                if (ShouldSerializeBackColor())
                {
                    return base.BackColor;
                }
                else
                {
                    return SystemColors.Window;
                }
            }
            set => base.BackColor = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackColorChanged
        {
            add => base.BackColorChanged += value;
            remove => base.BackColorChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage
        {
            get => base.BackgroundImage;
            set => base.BackgroundImage = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageChanged
        {
            add => base.BackgroundImageChanged += value;
            remove => base.BackgroundImageChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout
        {
            get => base.BackgroundImageLayout;
            set => base.BackgroundImageLayout = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageLayoutChanged
        {
            add => base.BackgroundImageLayoutChanged += value;
            remove => base.BackgroundImageLayoutChanged -= value;
        }

        /// <summary>
        ///  The current value of the CalendarForeColor property.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.DateTimePickerCalendarForeColorDescr))]
        public Color CalendarForeColor
        {
            get
            {
                return calendarForeColor;
            }

            set
            {
                if (value.IsEmpty)
                {
                    throw new ArgumentException(string.Format(SR.InvalidNullArgument,
                                                              "value"));
                }
                if (!value.Equals(calendarForeColor))
                {
                    calendarForeColor = value;
                    SetControlColor(MCSC.TEXT, value);
                }
            }
        }

        /// <summary>
        ///  The current value of the CalendarFont property.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [Localizable(true)]
        [AmbientValue(null)]
        [SRDescription(nameof(SR.DateTimePickerCalendarFontDescr))]
        public Font CalendarFont
        {
            get
            {
                if (calendarFont is null)
                {
                    return Font;
                }
                return calendarFont;
            }

            set
            {
                if ((value is null && calendarFont != null) || (value != null && !value.Equals(calendarFont)))
                {
                    calendarFont = value;
                    calendarFontHandleWrapper = null;
                    SetControlCalendarFont();
                }
            }
        }

        private Gdi32.HFONT CalendarFontHandle
        {
            get
            {
                if (calendarFont is null)
                {
                    Debug.Assert(calendarFontHandleWrapper is null, "font handle out of sync with Font");
                    return FontHandle;
                }

                if (calendarFontHandleWrapper is null)
                {
                    calendarFontHandleWrapper = new FontHandleWrapper(CalendarFont);
                }

                return calendarFontHandleWrapper.Handle;
            }
        }

        /// <summary>
        ///  The current value of the CalendarTitleBackColor property.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.DateTimePickerCalendarTitleBackColorDescr))]
        public Color CalendarTitleBackColor
        {
            get
            {
                return calendarTitleBackColor;
            }

            set
            {
                if (value.IsEmpty)
                {
                    throw new ArgumentException(string.Format(SR.InvalidNullArgument,
                                                              "value"));
                }
                if (!value.Equals(calendarTitleBackColor))
                {
                    calendarTitleBackColor = value;
                    SetControlColor(MCSC.TITLEBK, value);
                }
            }
        }

        /// <summary>
        ///  The current value of the CalendarTitleForeColor property.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.DateTimePickerCalendarTitleForeColorDescr))]
        public Color CalendarTitleForeColor
        {
            get
            {
                return calendarTitleForeColor;
            }

            set
            {
                if (value.IsEmpty)
                {
                    throw new ArgumentException(string.Format(SR.InvalidNullArgument,
                                                              "value"));
                }
                if (!value.Equals(calendarTitleForeColor))
                {
                    calendarTitleForeColor = value;
                    SetControlColor(MCSC.TITLETEXT, value);
                }
            }
        }

        /// <summary>
        ///  The current value of the CalendarTrailingForeColor property.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.DateTimePickerCalendarTrailingForeColorDescr))]
        public Color CalendarTrailingForeColor
        {
            get
            {
                return calendarTrailingText;
            }

            set
            {
                if (value.IsEmpty)
                {
                    throw new ArgumentException(string.Format(SR.InvalidNullArgument,
                                                              "value"));
                }
                if (!value.Equals(calendarTrailingText))
                {
                    calendarTrailingText = value;
                    SetControlColor(MCSC.TRAILINGTEXT, value);
                }
            }
        }

        /// <summary>
        ///  The current value of the CalendarMonthBackground property.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.DateTimePickerCalendarMonthBackgroundDescr))]
        public Color CalendarMonthBackground
        {
            get
            {
                return calendarMonthBackground;
            }

            set
            {
                if (value.IsEmpty)
                {
                    throw new ArgumentException(string.Format(SR.InvalidNullArgument,
                                                              "value"));
                }
                if (!value.Equals(calendarMonthBackground))
                {
                    calendarMonthBackground = value;
                    SetControlColor(MCSC.MONTHBK, value);
                }
            }
        }

        /// <summary>
        ///  Indicates whether the <see cref='Value'/> property has been set.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [Bindable(true)]
        [SRDescription(nameof(SR.DateTimePickerCheckedDescr))]
        public bool Checked
        {
            get
            {
                // the information from win32 DateTimePicker is reliable only when ShowCheckBoxes is True
                if (ShowCheckBox && IsHandleCreated)
                {
                    var sys = new Kernel32.SYSTEMTIME();
                    GDT gdt = (GDT)User32.SendMessageW(this, (User32.WM)DTM.GETSYSTEMTIME, IntPtr.Zero, ref sys);
                    return gdt == GDT.VALID;
                }
                else
                {
                    return validTime;
                }
            }
            set
            {
                if (Checked != value)
                {
                    // set the information into the win32 DateTimePicker only if ShowCheckBoxes is True
                    if (ShowCheckBox && IsHandleCreated)
                    {
                        if (value)
                        {
                            Kernel32.SYSTEMTIME sys = DateTimePicker.DateTimeToSysTime(Value);
                            User32.SendMessageW(this, (User32.WM)DTM.SETSYSTEMTIME, (IntPtr)GDT.VALID, ref sys);
                        }
                        else
                        {
                            User32.SendMessageW(this, (User32.WM)DTM.SETSYSTEMTIME, (IntPtr)GDT.NONE);
                        }
                    }
                    // this.validTime is used when the DateTimePicker receives date time change notification
                    // from the Win32 control. this.validTime will be used to know when we transition from valid time to unvalid time
                    // also, validTime will be used when ShowCheckBox == false
                    validTime = value;
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler Click
        {
            add => base.Click += value;
            remove => base.Click -= value;
        }

        /// <summary>
        ///  Returns the CreateParams used to create this window.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassName = WindowClasses.WC_DATETIMEPICK;

                cp.Style |= (int)_style;

                switch (format)
                {
                    case DateTimePickerFormat.Long:
                        cp.Style |= (int)DTS.LONGDATEFORMAT;
                        break;
                    case DateTimePickerFormat.Short:
                        break;
                    case DateTimePickerFormat.Time:
                        cp.Style |= (int)TIMEFORMAT_NOUPDOWN;
                        break;
                    case DateTimePickerFormat.Custom:
                        break;
                }

                cp.ExStyle |= (int)User32.WS_EX.CLIENTEDGE;

                if (RightToLeft == RightToLeft.Yes && RightToLeftLayout)
                {
                    //We want to turn on mirroring for DateTimePicker explicitly.
                    cp.ExStyle |= (int)User32.WS_EX.LAYOUTRTL;
                    //Don't need these styles when mirroring is turned on.
                    cp.ExStyle &= ~(int)(User32.WS_EX.RTLREADING | User32.WS_EX.RIGHT | User32.WS_EX.LEFTSCROLLBAR);
                }

                return cp;
            }
        }

        /// <summary>
        ///  Returns the custom format.
        /// </summary>
        [DefaultValue(null)]
        [Localizable(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.DateTimePickerCustomFormatDescr))]
        public string CustomFormat
        {
            get
            {
                return customFormat;
            }

            set
            {
                if ((value != null && !value.Equals(customFormat)) ||
                    (value is null && customFormat != null))
                {
                    customFormat = value;

                    if (IsHandleCreated)
                    {
                        if (format == DateTimePickerFormat.Custom)
                        {
                            User32.SendMessageW(this, (User32.WM)DTM.SETFORMATW, IntPtr.Zero, customFormat);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(200, PreferredHeight);
            }
        }

        /// <summary>
        ///  This property is overridden and hidden from statement completion
        ///  on controls that are based on Win32 Native Controls.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override bool DoubleBuffered
        {
            get => base.DoubleBuffered;
            set => base.DoubleBuffered = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler DoubleClick
        {
            add => base.DoubleClick += value;
            remove => base.DoubleClick -= value;
        }

        /// <summary>
        ///  The current value of the dropDownAlign property.  The calendar
        ///  dropDown can be aligned to the left or right of the control.
        /// </summary>
        [DefaultValue(LeftRightAlignment.Left)]
        [SRCategory(nameof(SR.CatAppearance))]
        [Localizable(true)]
        [SRDescription(nameof(SR.DateTimePickerDropDownAlignDescr))]
        public LeftRightAlignment DropDownAlign
        {
            get
            {
                return (_style & DTS.RIGHTALIGN) != 0
                    ? LeftRightAlignment.Right
                    : LeftRightAlignment.Left;
            }

            set
            {
                //valid values are 0x0 to 0x1
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)LeftRightAlignment.Left, (int)LeftRightAlignment.Right))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(LeftRightAlignment));
                }

                SetStyleBit(value == LeftRightAlignment.Right, DTS.RIGHTALIGN);
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Color ForeColor
        {
            get
            {
                if (ShouldSerializeForeColor())
                {
                    return base.ForeColor;
                }
                else
                {
                    return SystemColors.WindowText;
                }
            }
            set => base.ForeColor = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler ForeColorChanged
        {
            add => base.ForeColorChanged += value;
            remove => base.ForeColorChanged -= value;
        }

        /// <summary>
        ///  Returns the current value of the format property. This determines the
        ///  style of format the date is displayed in.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [RefreshProperties(RefreshProperties.Repaint)]
        [SRDescription(nameof(SR.DateTimePickerFormatDescr))]
        public DateTimePickerFormat Format
        {
            get
            {
                return format;
            }

            set
            {
                //valid values are 0x1, 0x2,0x4,0x8. max number of bits on at a time is 1
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DateTimePickerFormat.Long, (int)DateTimePickerFormat.Custom, /*maxNumberOfBitsOn*/1))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DateTimePickerFormat));
                }

                if (format != value)
                {
                    format = value;
                    RecreateHandle();

                    OnFormatChanged(EventArgs.Empty);
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.DateTimePickerOnFormatChangedDescr))]
        public event EventHandler FormatChanged
        {
            add => Events.AddHandler(s_formatChangedEvent, value);
            remove => Events.RemoveHandler(s_formatChangedEvent, value);
        }

        /// <summary>
        ///  DateTimePicker Paint.
        /// </summary>
        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event PaintEventHandler Paint
        {
            add => base.Paint += value;
            remove => base.Paint -= value;
        }

        //Make sure the passed in minDate respects the current culture: this
        //is especially important if the culture changes from a Gregorian or
        //other calendar with a lowish minDate (see comment on MinimumDateTime)
        //to a calendar, which has a minimum date of 1/1/1912.
        static internal DateTime EffectiveMinDate(DateTime minDate)
        {
            DateTime minSupportedDate = DateTimePicker.MinimumDateTime;
            if (minDate < minSupportedDate)
            {
                return minSupportedDate;
            }
            return minDate;
        }

        //Similarly, make sure the maxDate respects the current culture.  No
        //problems are anticipated here: I don't believe there are calendars
        //around that have max dates on them.  But if there are, we'll deal with
        //them correctly.
        static internal DateTime EffectiveMaxDate(DateTime maxDate)
        {
            DateTime maxSupportedDate = DateTimePicker.MaximumDateTime;
            if (maxDate > maxSupportedDate)
            {
                return maxSupportedDate;
            }
            return maxDate;
        }

        /// <summary>
        ///  Indicates the maximum date and time
        ///  selectable in the control.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.DateTimePickerMaxDateDescr))]
        public DateTime MaxDate
        {
            get
            {
                return EffectiveMaxDate(max);
            }
            set
            {
                if (value != max)
                {
                    if (value < EffectiveMinDate(min))
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(MaxDate), FormatDateTime(value), nameof(MinDate)));
                    }
                    if (value > MaximumDateTime)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.DateTimePickerMaxDate, FormatDateTime(DateTimePicker.MaxDateTime)));
                    }

                    max = value;
                    SetRange();

                    //If Value (which was once valid) is suddenly greater than the max (since we just set it)
                    //then adjust this...
                    if (Value > max)
                    {
                        Value = max;
                    }
                }
            }
        }

        /// <summary>
        ///  Specifies the maximum date value. This property is read-only.
        /// </summary>
        public static DateTime MaximumDateTime
        {
            get
            {
                DateTime maxSupportedDateTime = CultureInfo.CurrentCulture.Calendar.MaxSupportedDateTime;
                if (maxSupportedDateTime.Year > MaxDateTime.Year)
                {
                    return MaxDateTime;
                }
                return maxSupportedDateTime;
            }
        }

        /// <summary>
        ///  Indicates the minimum date and time
        ///  selectable in the control.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.DateTimePickerMinDateDescr))]
        public DateTime MinDate
        {
            get
            {
                return EffectiveMinDate(min);
            }
            set
            {
                if (value != min)
                {
                    if (value > EffectiveMaxDate(max))
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidHighBoundArgument, nameof(MinDate), FormatDateTime(value), nameof(MaxDate)));
                    }
                    if (value < MinimumDateTime)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.DateTimePickerMinDate, FormatDateTime(DateTimePicker.MinimumDateTime)));
                    }

                    min = value;
                    SetRange();

                    //If Value (which was once valid) is suddenly less than the min (since we just set it)
                    //then adjust this...
                    if (Value < min)
                    {
                        Value = min;
                    }
                }
            }
        }

        // We restrict the available dates to >= 1753 because of oddness in the Gregorian calendar about
        // that time.  We do this even for cultures that don't use the Gregorian calendar -- we're not
        // really that worried about calendars for >250 years ago.
        //
        /// <summary>
        ///  Specifies the minimum date value. This property is read-only.
        /// </summary>
        public static DateTime MinimumDateTime
        {
            get
            {
                DateTime minSupportedDateTime = CultureInfo.CurrentCulture.Calendar.MinSupportedDateTime;
                if (minSupportedDateTime.Year < 1753)
                {
                    return new DateTime(1753, 1, 1);
                }
                return minSupportedDateTime;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event MouseEventHandler MouseClick
        {
            add => base.MouseClick += value;
            remove => base.MouseClick -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event MouseEventHandler MouseDoubleClick
        {
            add => base.MouseDoubleClick += value;
            remove => base.MouseDoubleClick -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Padding Padding
        {
            get => base.Padding;
            set => base.Padding = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler PaddingChanged
        {
            add => base.PaddingChanged += value;
            remove => base.PaddingChanged -= value;
        }

        /// <summary>
        ///  Indicates the preferred height of the DateTimePicker control. This property is read-only.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PreferredHeight
        {
            get
            {
                if (prefHeightCache > -1)
                {
                    return (int)prefHeightCache;
                }

                // Base the preferred height on the current font
                int height = FontHeight;

                // Adjust for the border
                height += SystemInformation.BorderSize.Height * 4 + 3;
                prefHeightCache = (short)height;

                return height;
            }
        }

        /// <summary>
        ///  This is used for international applications where the language is written from RightToLeft.
        ///  When this property is true, and the RightToLeft is true, mirroring will be turned on on
        ///  the form, and control placement and text will be from right to left.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [Localizable(true)]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.ControlRightToLeftLayoutDescr))]
        public virtual bool RightToLeftLayout
        {
            get
            {
                return rightToLeftLayout;
            }

            set
            {
                if (value != rightToLeftLayout)
                {
                    rightToLeftLayout = value;
                    using (new LayoutTransaction(this, this, PropertyNames.RightToLeftLayout))
                    {
                        OnRightToLeftLayoutChanged(EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        ///  Indicates whether a check box is displayed to toggle the NoValueSelected property
        ///  value.
        /// </summary>
        [DefaultValue(false)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.DateTimePickerShowNoneDescr))]
        public bool ShowCheckBox
        {
            get => (_style & DTS.SHOWNONE) != 0;
            set => SetStyleBit(value, DTS.SHOWNONE);
        }

        /// <summary>
        ///  Indicates
        ///  whether an up-down control is used to adjust the time values.
        /// </summary>
        [DefaultValue(false)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.DateTimePickerShowUpDownDescr))]
        public bool ShowUpDown
        {
            get => (_style & DTS.UPDOWN) != 0;
            set
            {
                if (ShowUpDown != value)
                {
                    SetStyleBit(value, DTS.UPDOWN);
                }
            }
        }

        /// <summary>
        ///  Overrides Text to allow for setting of the value via a string.  Also, returns
        ///  a formatted Value when getting the text.  The DateTime class will throw
        ///  an exception if the string (value) being passed in is invalid.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get => base.Text;
            set
            {
                // Clause to check length
                //
                if (value is null || value.Length == 0)
                {
                    ResetValue();
                }
                else
                {
                    Value = DateTime.Parse(value, CultureInfo.CurrentCulture);
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        new public event EventHandler TextChanged
        {
            add => base.TextChanged += value;
            remove => base.TextChanged -= value;
        }

        /// <summary>
        ///  Indicates the DateTime value assigned to the control.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [Bindable(true)]
        [RefreshProperties(RefreshProperties.All)]
        [SRDescription(nameof(SR.DateTimePickerValueDescr))]
        public DateTime Value
        {
            get
            {
                //checkbox clicked, no value set - no value set state should never occur, but just in case
                if (!userHasSetValue && validTime)
                {
                    return creationTime;
                }
                else
                {
                    return value;
                }
            }
            set
            {
                bool valueChanged = !DateTime.Equals(Value, value);
                // Check for value set here; if we've not set the value yet, it'll be Now, so the second
                // part of the test will fail.
                // So, if userHasSetValue isn't set, we don't care if the value is still the same - and we'll
                // update anyway.
                if (!userHasSetValue || valueChanged)
                {
                    if ((value < MinDate) || (value > MaxDate))
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidBoundArgument, nameof(Value), FormatDateTime(value), $"'{nameof(MinDate)}'", $"'{nameof(MaxDate)}'"));
                    }

                    string oldText = Text;

                    this.value = value;
                    userHasSetValue = true;

                    if (IsHandleCreated)
                    {
                        // Make sure any changes to this code get propagated to createHandle
                        Kernel32.SYSTEMTIME sys = DateTimePicker.DateTimeToSysTime(value);
                        User32.SendMessageW(this, (User32.WM)DTM.SETSYSTEMTIME, (IntPtr)GDT.VALID, ref sys);
                    }

                    if (valueChanged)
                    {
                        OnValueChanged(EventArgs.Empty);
                    }

                    if (!oldText.Equals(Text))
                    {
                        OnTextChanged(EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        ///  Occurs when the dropdown calendar is dismissed and disappears.
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.DateTimePickerOnCloseUpDescr))]
        public event EventHandler CloseUp
        {
            add => _onCloseUp += value;
            remove => _onCloseUp -= value;
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
        public event EventHandler RightToLeftLayoutChanged
        {
            add => _onRightToLeftLayoutChanged += value;
            remove => _onRightToLeftLayoutChanged -= value;
        }

        /// <summary>
        ///  Occurs when the value for the control changes.
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.valueChangedEventDescr))]
        public event EventHandler ValueChanged
        {
            add => _onValueChanged += value;
            remove => _onValueChanged -= value;
        }

        /// <summary>
        ///  Occurs when the drop down calendar is shown.
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.DateTimePickerOnDropDownDescr))]
        public event EventHandler DropDown
        {
            add => _onDropDown += value;
            remove => _onDropDown -= value;
        }

        /// <summary>
        ///  Constructs the new instance of the accessibility object for this control. Subclasses
        ///  should not call base.CreateAccessibilityObject.
        /// </summary>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DateTimePickerAccessibleObject(this);
        }

        /// <summary>
        ///  Creates the physical window handle.
        /// </summary>
        protected override void CreateHandle()
        {
            if (!RecreatingHandle)
            {
                IntPtr userCookie = ThemingScope.Activate(Application.UseVisualStyles);

                try
                {
                    var icc = new INITCOMMONCONTROLSEX
                    {
                        dwICC = ICC.DATE_CLASSES
                    };
                    InitCommonControlsEx(ref icc);
                }
                finally
                {
                    ThemingScope.Deactivate(userCookie);
                }
            }

            creationTime = DateTime.Now;

            base.CreateHandle();

            if (userHasSetValue && validTime)
            {
                // Make sure any changes to this code get propagated to setValue
                Kernel32.SYSTEMTIME sys = DateTimePicker.DateTimeToSysTime(Value);
                User32.SendMessageW(this, (User32.WM)DTM.SETSYSTEMTIME, (IntPtr)GDT.VALID, ref sys);
            }
            else if (!validTime)
            {
                User32.SendMessageW(this, (User32.WM)DTM.SETSYSTEMTIME, (IntPtr)GDT.NONE);
            }

            if (format == DateTimePickerFormat.Custom)
            {
                User32.SendMessageW(this, (User32.WM)DTM.SETFORMATW, IntPtr.Zero, customFormat);
            }

            UpdateUpDown();
            SetAllControlColors();
            SetControlCalendarFont();
            SetRange();
        }

        /// <summary>
        ///  Destroys the physical window handle.
        /// </summary>
        protected override void DestroyHandle()
        {
            value = Value;
            base.DestroyHandle();
        }

        // Return a localized string representation of the given DateTime value.
        // Used for throwing exceptions, etc.
        //
        private static string FormatDateTime(DateTime value)
        {
            return value.ToString("G", CultureInfo.CurrentCulture);
        }

        // GetPreferredSize and SetBoundsCore call this method to allow controls to self impose
        // constraints on their size.
        internal override Rectangle ApplyBoundsConstraints(int suggestedX, int suggestedY, int proposedWidth, int proposedHeight)
        {
            // Lock DateTimePicker to its preferred height.
            return base.ApplyBoundsConstraints(suggestedX, suggestedY, proposedWidth, PreferredHeight);
        }

        internal override Size GetPreferredSizeCore(Size proposedConstraints)
        {
            int height = PreferredHeight;
            int width = CommonProperties.GetSpecifiedBounds(this).Width;
            return new Size(width, height);
        }

        /// <summary>
        ///  Handling special input keys, such as pgup, pgdown, home, end, etc...
        /// </summary>
        protected override bool IsInputKey(Keys keyData)
        {
            if ((keyData & Keys.Alt) == Keys.Alt)
            {
                return false;
            }

            switch (keyData & Keys.KeyCode)
            {
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.Home:
                case Keys.End:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        /// <summary>
        ///  Raises the <see cref='CloseUp'/>
        ///  event.
        /// </summary>
        protected virtual void OnCloseUp(EventArgs eventargs)
        {
            _onCloseUp?.Invoke(this, eventargs);
        }

        /// <summary>
        ///  Raises the <see cref='DropDown'/> event.
        /// </summary>
        protected virtual void OnDropDown(EventArgs eventargs)
        {
            _onDropDown?.Invoke(this, eventargs);
        }

        protected virtual void OnFormatChanged(EventArgs e)
        {
            if (Events[s_formatChangedEvent] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        /// <summary>
        ///  Add/remove SystemEvents in OnHandleCreated/Destroyed for robustness
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(MarshaledUserPreferenceChanged);
        }

        /// <summary>
        ///  Add/remove SystemEvents in OnHandleCreated/Destroyed for robustness
        /// </summary>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(MarshaledUserPreferenceChanged);
            base.OnHandleDestroyed(e);
        }

        /// <summary>
        ///  Raises the <see cref='ValueChanged'/> event.
        /// </summary>
        protected virtual void OnValueChanged(EventArgs eventargs)
        {
            _onValueChanged?.Invoke(this, eventargs);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnRightToLeftLayoutChanged(EventArgs e)
        {
            if (GetAnyDisposingInHierarchy())
            {
                return;
            }

            if (RightToLeft == RightToLeft.Yes)
            {
                RecreateHandle();
            }

            _onRightToLeftLayoutChanged?.Invoke(this, e);
        }

        /// <summary>
        ///  Occurs when a property for the control changes.
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            //clear the pref height cache
            prefHeightCache = -1;

            Height = PreferredHeight;

            if (calendarFont is null)
            {
                calendarFontHandleWrapper = null;
                SetControlCalendarFont();
            }
        }

        private void ResetCalendarForeColor()
        {
            CalendarForeColor = DefaultForeColor;
        }

        private void ResetCalendarFont()
        {
            CalendarFont = null;
        }

        private void ResetCalendarMonthBackground()
        {
            CalendarMonthBackground = DefaultMonthBackColor;
        }

        private void ResetCalendarTitleBackColor()
        {
            CalendarTitleBackColor = DefaultTitleBackColor;
        }

        private void ResetCalendarTitleForeColor()
        {
            CalendarTitleBackColor = DefaultForeColor;
        }

        private void ResetCalendarTrailingForeColor()
        {
            CalendarTrailingForeColor = DefaultTrailingForeColor;
        }

        /// <summary>
        ///  Resets the <see cref='Format'/> property to its default
        ///  value.
        /// </summary>
        private void ResetFormat()
        {
            Format = DateTimePickerFormat.Long;
        }

        /// <summary>
        ///  Resets the <see cref='MaxDate'/> property to its default value.
        /// </summary>
        private void ResetMaxDate()
        {
            MaxDate = MaximumDateTime;
        }

        /// <summary>
        ///  Resets the <see cref='MinDate'/> property to its default value.
        /// </summary>
        private void ResetMinDate()
        {
            MinDate = MinimumDateTime;
        }

        /// <summary>
        ///  Resets the <see cref='Value'/> property to its default value.
        /// </summary>
        private void ResetValue()
        {
            // always update on reset with ShowNone = false -- as it'll take the current time.
            value = DateTime.Now;

            // If ShowCheckBox = true, then userHasSetValue can be false (null value).
            // otherwise, userHasSetValue is valid...
            // userHasSetValue = !ShowCheckBox;

            // After ResetValue() the flag indicating whether the user
            // has set the value should be false.
            userHasSetValue = false;

            // Update the text displayed in the DateTimePicker
            if (IsHandleCreated)
            {
                Kernel32.SYSTEMTIME sys = DateTimePicker.DateTimeToSysTime(value);
                User32.SendMessageW(this, (User32.WM)DTM.SETSYSTEMTIME, (IntPtr)GDT.VALID, ref sys);
            }

            // Updating Checked to false will set the control to "no date",
            // and clear its checkbox.
            Checked = false;

            OnValueChanged(EventArgs.Empty);
            OnTextChanged(EventArgs.Empty);
        }

        /// <summary>
        ///  If the handle has been created, this applies the color to the control
        /// </summary>
        private void SetControlColor(MCSC colorIndex, Color value)
        {
            if (IsHandleCreated)
            {
                User32.SendMessageW(this, (User32.WM)DTM.SETMCCOLOR, (IntPtr)colorIndex, PARAM.FromColor(value));
            }
        }

        /// <summary>
        ///  If the handle has been created, this applies the font to the control.
        /// </summary>
        private void SetControlCalendarFont()
        {
            if (IsHandleCreated)
            {
                User32.SendMessageW(this, (User32.WM)DTM.SETMCFONT, (IntPtr)CalendarFontHandle, NativeMethods.InvalidIntPtr);
            }
        }

        /// <summary>
        ///  Applies all the colors to the control.
        /// </summary>
        private void SetAllControlColors()
        {
            SetControlColor(MCSC.MONTHBK, calendarMonthBackground);
            SetControlColor(MCSC.TEXT, calendarForeColor);
            SetControlColor(MCSC.TITLEBK, calendarTitleBackColor);
            SetControlColor(MCSC.TITLETEXT, calendarTitleForeColor);
            SetControlColor(MCSC.TRAILINGTEXT, calendarTrailingText);
        }

        /// <summary>
        ///  Updates the window handle with the min/max ranges if it has been
        ///  created.
        /// </summary>
        private void SetRange()
        {
            SetRange(EffectiveMinDate(min), EffectiveMaxDate(max));
        }

        private void SetRange(DateTime min, DateTime max)
        {
            if (IsHandleCreated)
            {
                Span<Kernel32.SYSTEMTIME> sa = stackalloc Kernel32.SYSTEMTIME[2];
                sa[0] = DateTimeToSysTime(min);
                sa[1] = DateTimeToSysTime(max);
                GDTR flags = GDTR.MIN | GDTR.MAX;
                User32.SendMessageW(this, (User32.WM)DTM.SETRANGE, (IntPtr)flags, ref sa[0]);
            }
        }

        /// <summary>
        ///  Turns on or off a given style bit
        /// </summary>
        private void SetStyleBit(bool flag, DTS bit)
        {
            if (((_style & bit) != 0) == flag)
            {
                return;
            }

            if (flag)
            {
                _style |= bit;
            }
            else
            {
                _style &= ~bit;
            }

            if (IsHandleCreated)
            {
                RecreateHandle();
                Invalidate();
                Update();
            }
        }

        /// <summary>
        ///  Determines if the <see cref='CalendarForeColor'/> property needs to be
        ///  persisted.
        /// </summary>
        private bool ShouldSerializeCalendarForeColor()
        {
            return !CalendarForeColor.Equals(DefaultForeColor);
        }

        /// <summary>
        ///  Determines if the <see cref='CalendarFont'/> property needs to be persisted.
        /// </summary>
        private bool ShouldSerializeCalendarFont()
        {
            return calendarFont != null;
        }

        /// <summary>
        ///  Determines if the <see cref='CalendarTitleBackColor'/> property needs to be persisted.
        /// </summary>
        private bool ShouldSerializeCalendarTitleBackColor()
        {
            return !calendarTitleBackColor.Equals(DefaultTitleBackColor);
        }

        /// <summary>
        ///  Determines if the <see cref='CalendarTitleForeColor'/> property needs to be persisted.
        /// </summary>
        private bool ShouldSerializeCalendarTitleForeColor()
        {
            return !calendarTitleForeColor.Equals(DefaultTitleForeColor);
        }

        /// <summary>
        ///  Determines if the <see cref='CalendarTrailingForeColor'/> property needs to be persisted.
        /// </summary>
        private bool ShouldSerializeCalendarTrailingForeColor()
        {
            return !calendarTrailingText.Equals(DefaultTrailingForeColor);
        }

        /// <summary>
        ///  Determines if the <see cref='CalendarMonthBackground'/> property needs to be persisted.
        /// </summary>
        private bool ShouldSerializeCalendarMonthBackground()
        {
            return !calendarMonthBackground.Equals(DefaultMonthBackColor);
        }

        /// <summary>
        ///  Determines if the <see cref='MaxDate'/> property needs to be persisted.
        /// </summary>
        private bool ShouldSerializeMaxDate()
        {
            return max != MaximumDateTime && max != DateTime.MaxValue;
        }

        /// <summary>
        ///  Determines if the <see cref='MinDate'/> property needs to be persisted.
        /// </summary>
        private bool ShouldSerializeMinDate()
        {
            return min != MinimumDateTime && min != DateTime.MinValue;
        }

        /// <summary>
        ///  Determines if the <see cref='Value'/> property needs to be persisted.
        /// </summary>
        private bool ShouldSerializeValue()
        {
            return userHasSetValue;
        }

        /// <summary>
        ///  Determines if the <see cref='Format'/> property needs to be persisted.
        /// </summary>
        private bool ShouldSerializeFormat()
        {
            return (Format != DateTimePickerFormat.Long);
        }

        /// <summary>
        ///  Returns the control as a string
        /// </summary>
        public override string ToString()
        {
            string s = base.ToString();
            return s + ", Value: " + FormatDateTime(Value);
        }

        /// <summary>
        ///  Forces a repaint of the updown control if it is displayed.
        /// </summary>
        private unsafe void UpdateUpDown()
        {
            // The upDown control doesn't repaint correctly.
            if (ShowUpDown)
            {
                EnumChildren c = new EnumChildren();
                User32.EnumChildWindows(this, c.enumChildren);
                if (c.hwndFound != IntPtr.Zero)
                {
                    User32.InvalidateRect(new HandleRef(c, c.hwndFound), null, BOOL.TRUE);
                    User32.UpdateWindow(c.hwndFound);
                }
            }
        }

        private void MarshaledUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs pref)
        {
            try
            {
                //use begininvoke instead of invoke in case the destination thread is not processing messages.
                BeginInvoke(new UserPreferenceChangedEventHandler(UserPreferenceChanged), new object[] { sender, pref });
            }
            catch (InvalidOperationException) { } //if the destination thread does not exist, don't send.
        }

        private void UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs pref)
        {
            if (pref.Category == UserPreferenceCategory.Locale)
            {
                // We need to recreate the monthcalendar handle when the locale changes, because
                // the day names etc. are only updated on a handle recreate (comctl32 limitation).
                //
                RecreateHandle();
            }
        }

        /// <summary>
        ///  Handles the DTN_CLOSEUP notification
        /// </summary>
        private void WmCloseUp(ref Message m)
        {
            OnCloseUp(EventArgs.Empty);
        }

        /// <summary>
        ///  Handles the DTN_DATETIMECHANGE notification
        /// </summary>
        private unsafe void WmDateTimeChange(ref Message m)
        {
            NMDATETIMECHANGE* nmdtc = (NMDATETIMECHANGE*)m.LParam;
            DateTime temp = value;
            bool oldvalid = validTime;
            if (nmdtc->dwFlags != GDT.NONE)
            {
                validTime = true;
                value = DateTimePicker.SysTimeToDateTime(nmdtc->st);
                userHasSetValue = true;
            }
            else
            {
                validTime = false;
            }
            if (value != temp || oldvalid != validTime)
            {
                OnValueChanged(EventArgs.Empty);
                OnTextChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        ///  Handles the DTN_DROPDOWN notification
        /// </summary>
        private void WmDropDown(ref Message m)
        {
            if (RightToLeftLayout == true && RightToLeft == RightToLeft.Yes)
            {
                IntPtr handle = User32.SendMessageW(this, (User32.WM)DTM.GETMONTHCAL);
                if (handle != IntPtr.Zero)
                {
                    int style = unchecked((int)((long)User32.GetWindowLong(new HandleRef(this, handle), User32.GWL.EXSTYLE)));
                    style |= (int)(User32.WS_EX.LAYOUTRTL | User32.WS_EX.NOINHERITLAYOUT);
                    style &= ~(int)(User32.WS_EX.RIGHT | User32.WS_EX.RTLREADING);
                    User32.SetWindowLong(new HandleRef(this, handle), User32.GWL.EXSTYLE, new HandleRef(this, (IntPtr)style));
                }
            }
            OnDropDown(EventArgs.Empty);
        }

        /// <summary>
        ///  Handles system color changes
        /// </summary>
        protected override void OnSystemColorsChanged(EventArgs e)
        {
            SetAllControlColors();
            base.OnSystemColorsChanged(e);
        }

        /// <summary>
        ///  Handles the WM_COMMAND messages reflected from the parent control.
        /// </summary>
        private unsafe void WmReflectCommand(ref Message m)
        {
            if (m.HWnd == Handle)
            {
                User32.NMHDR* nmhdr = (User32.NMHDR*)m.LParam;
                switch ((DTN)nmhdr->code)
                {
                    case DTN.CLOSEUP:
                        WmCloseUp(ref m);
                        break;
                    case DTN.DATETIMECHANGE:
                        WmDateTimeChange(ref m);
                        break;
                    case DTN.DROPDOWN:
                        WmDropDown(ref m);
                        break;
                }
            }
        }

        /// <summary>
        ///  Overrided wndProc
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            switch ((User32.WM)m.Msg)
            {
                case User32.WM.LBUTTONDOWN:
                    Focus();
                    if (!ValidationCancelled)
                    {
                        base.WndProc(ref m);
                    }
                    break;
                case User32.WM.REFLECT | User32.WM.NOTIFY:
                    WmReflectCommand(ref m);
                    base.WndProc(ref m);
                    break;
                case User32.WM.WINDOWPOSCHANGED:
                    base.WndProc(ref m);
                    UpdateUpDown();
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <summary>
        ///  Takes a DateTime value and returns a SYSTEMTIME struct
        ///  Note: 1 second granularity
        /// </summary>
        internal static Kernel32.SYSTEMTIME DateTimeToSysTime(DateTime time)
        {
            var sys = new Kernel32.SYSTEMTIME
            {
                wYear = (short)time.Year,
                wMonth = (short)time.Month,
                wDayOfWeek = (short)time.DayOfWeek,
                wDay = (short)time.Day,
                wHour = (short)time.Hour,
                wMinute = (short)time.Minute,
                wSecond = (short)time.Second,
                wMilliseconds = 0
            };
            return sys;
        }

        /// <summary>
        ///  Takes a SYSTEMTIME struct and returns a DateTime value
        ///  Note: 1 second granularity.
        /// </summary>
        internal static DateTime SysTimeToDateTime(Kernel32.SYSTEMTIME s)
        {
            if (s.wYear <= 0 || s.wMonth <= 0 || s.wDay <= 0)
            {
                Debug.Fail("Incorrect SYSTEMTIME info!");
                return DateTime.MinValue;
            }

            return new DateTime(s.wYear, s.wMonth, s.wDay, s.wHour, s.wMinute, s.wSecond);
        }

        private sealed class EnumChildren
        {
            public IntPtr hwndFound = IntPtr.Zero;

            public BOOL enumChildren(IntPtr hwnd)
            {
                hwndFound = hwnd;
                return BOOL.TRUE;
            }
        }

        public class DateTimePickerAccessibleObject : ControlAccessibleObject
        {
            public DateTimePickerAccessibleObject(DateTimePicker owner) : base(owner)
            {
            }

            public override string KeyboardShortcut
            {
                get
                {
                    // APP COMPAT. When computing DateTimePickerAccessibleObject::get_KeyboardShorcut the previous label
                    // takes precedence over DTP::Text.
                    // This code was copied from the Everett sources.
                    Label previousLabel = PreviousLabel;

                    if (previousLabel != null)
                    {
                        char previousLabelMnemonic = WindowsFormsUtils.GetMnemonic(previousLabel.Text, false /*convertToUpperCase*/);
                        if (previousLabelMnemonic != (char)0)
                        {
                            return "Alt+" + previousLabelMnemonic;
                        }
                    }

                    string baseShortcut = base.KeyboardShortcut;

                    if ((baseShortcut is null || baseShortcut.Length == 0))
                    {
                        char ownerTextMnemonic = WindowsFormsUtils.GetMnemonic(Owner.Text, false /*convertToUpperCase*/);
                        if (ownerTextMnemonic != (char)0)
                        {
                            return "Alt+" + ownerTextMnemonic;
                        }
                    }

                    return baseShortcut;
                }
            }

            public override string Value
            {
                get
                {
                    string baseValue = base.Value;
                    if (baseValue is null || baseValue.Length == 0)
                    {
                        return Owner.Text;
                    }
                    return baseValue;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = base.State;

                    if (((DateTimePicker)Owner).ShowCheckBox &&
                       ((DateTimePicker)Owner).Checked)
                    {
                        state |= AccessibleStates.Checked;
                    }

                    return state;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = Owner.AccessibleRole;
                    if (role != AccessibleRole.Default)
                    {
                        return role;
                    }

                    return AccessibleRole.ComboBox;
                }
            }

            internal override bool IsIAccessibleExSupported() => true;

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.IsTogglePatternAvailablePropertyId:
                        return IsPatternSupported(UiaCore.UIA.TogglePatternId);
                    case UiaCore.UIA.LocalizedControlTypePropertyId:
                        return s_dateTimePickerLocalizedControlTypeString;
                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.TogglePatternId && ((DateTimePicker)Owner).ShowCheckBox)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            #region Toggle Pattern

            internal override UiaCore.ToggleState ToggleState
            {
                get
                {
                    return ((DateTimePicker)Owner).Checked ?
                        UiaCore.ToggleState.On :
                        UiaCore.ToggleState.Off;
                }
            }

            internal override void Toggle()
            {
                if (Owner.IsHandleCreated)
                {
                    ((DateTimePicker)Owner).Checked = !((DateTimePicker)Owner).Checked;
                }
            }

            #endregion
        }
    }
}
