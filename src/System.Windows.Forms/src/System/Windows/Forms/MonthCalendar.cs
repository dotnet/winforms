// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



namespace System.Windows.Forms {
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;

    using System.Diagnostics;

    using System;
    using System.Security.Permissions;
    using System.Globalization;

    using System.Windows.Forms.Internal;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using ArrayList = System.Collections.ArrayList;

    using System.Drawing;
    using Microsoft.Win32;
    using System.Windows.Forms.Layout;

    /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar"]/*' />
    /// <devdoc>
    ///     This control is an encapsulateion of the Windows month calendar control.
    ///     A month calendar control implements a calendar-like user interface, that
    ///     provides the user with a very intuitive and recognizable method of entering
    ///     or selecting a date.
    ///     Users can also select which days bold.  The most efficient way to add the
    ///     bolded dates is via an array all at once.  (The below descriptions can be applied
    ///     equally to annually and monthly bolded dates as well)
    ///     The following is an example of this:
    /// <code>
    ///     MonthCalendar mc = new MonthCalendar();
    ///     //     add specific dates to bold
    ///     DateTime[] time = new DateTime[3];
    ///     time[0] = DateTime.Now;
    ///     time[1] = time[0].addDays(2);
    ///     time[2] = time[1].addDays(2);
    ///     mc.setBoldedDates(time);
    /// </code>
    ///     Removal of all bolded dates is accomplished with:
    /// <code>
    ///     mc.removeAllBoldedDates();
    /// </code>
    ///     Although less efficient, the user may need to add or remove bolded dates one at
    ///     a time.  To improve the performance of this, neither addBoldedDate nor
    ///     removeBoldedDate repaints the monthcalendar.  The user must call updateBoldedDates
    ///     to force the repaint of the bolded dates, otherwise the monthCalendar will not
    ///     paint properly.
    ///     The following is an example of this:
    /// <code>
    ///     DateTime time1 = new DateTime("3/5/98");
    ///     DateTime time2 = new DateTime("4/19/98");
    ///     mc.addBoldedDate(time1);
    ///     mc.addBoldedDate(time2);
    ///     mc.removeBoldedDate(time1);
    ///     mc.updateBoldedDates();
    /// </code>
    ///     The same applies to addition and removal of annual and monthly bolded dates.
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultProperty(nameof(SelectionRange)),
    DefaultEvent(nameof(DateChanged)),
    DefaultBindingProperty(nameof(SelectionRange)),
    Designer("System.Windows.Forms.Design.MonthCalendarDesigner, " + AssemblyRef.SystemDesign),
    SRDescription(nameof(SR.DescriptionMonthCalendar))
    ]
    public class MonthCalendar : Control {
        const long DAYS_TO_1601 = 548229;
        const long DAYS_TO_10000 = 3615900;
        static readonly Color DEFAULT_TITLE_BACK_COLOR = SystemColors.ActiveCaption;
        static readonly Color DEFAULT_TITLE_FORE_COLOR = SystemColors.ActiveCaptionText;
        static readonly Color DEFAULT_TRAILING_FORE_COLOR = SystemColors.GrayText;
        private const int MINIMUM_ALLOC_SIZE = 12;  // minimum size to expand the buffer by
        private const int MONTHS_IN_YEAR = 12;
        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.INSERT_WIDTH_SIZE"]/*' />
        /// <devdoc>
        ///     This is the arbitrary number of pixels that the Win32 control
        ///     inserts between calendars horizontally, regardless of font.
        /// </devdoc>
        /// <internalonly/>
        private const int   INSERT_WIDTH_SIZE = 6;
        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.INSERT_HEIGHT_SIZE"]/*' />
        /// <devdoc>
        ///     This is the arbitrary number of pixels that the Win32 control
        ///     inserts between calendars vertically, regardless of font.
        /// </devdoc>
        /// <internalonly/>
        private const int         INSERT_HEIGHT_SIZE = 6;       // From comctl32 MonthCalendar sources CALBORDER
        private const Day    DEFAULT_FIRST_DAY_OF_WEEK = Day.Default;
        private const int         DEFAULT_MAX_SELECTION_COUNT = 7;
        private const int         DEFAULT_SCROLL_CHANGE = 0;
        private const int         UNIQUE_DATE = 0;
        private const int         ANNUAL_DATE = 1;
        private const int         MONTHLY_DATE = 2;

        private static readonly Size           DefaultSingleMonthSize = new Size(176, 153);

        private const int   MaxScrollChange = 20000;

        private const int   ExtraPadding = 2;
        private int         scaledExtraPadding = ExtraPadding;

        private IntPtr         mdsBuffer = IntPtr.Zero;
        private int         mdsBufferSize = 0;

        // styles
        private Color       titleBackColor = DEFAULT_TITLE_BACK_COLOR;
        private Color       titleForeColor = DEFAULT_TITLE_FORE_COLOR;
        private Color       trailingForeColor = DEFAULT_TRAILING_FORE_COLOR;
        private bool        showToday = true;
        private bool        showTodayCircle = true;
        private bool        showWeekNumbers = false;
        private bool        rightToLeftLayout = false;


        // properties
        private Size        dimensions = new Size(1, 1);
        private int         maxSelectionCount = DEFAULT_MAX_SELECTION_COUNT;
        // Reconcile out-of-range min/max values in the property getters.
        private DateTime    maxDate = DateTime.MaxValue; 
        private DateTime    minDate = DateTime.MinValue; 
        private int         scrollChange = DEFAULT_SCROLL_CHANGE;
        private bool        todayDateSet = false;           // Has TodayDate been explicitly set?
        private DateTime    todayDate = DateTime.Now.Date;
        private DateTime    selectionStart;
        private DateTime    selectionEnd;
        private Day     firstDayOfWeek = DEFAULT_FIRST_DAY_OF_WEEK;
        private NativeMethods.MONTCALENDAR_VIEW_MODE mcCurView = NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_MONTH;
        private NativeMethods.MONTCALENDAR_VIEW_MODE mcOldView = NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_MONTH;

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.monthsOfYear"]/*' />
        /// <devdoc>
        ///     Bitmask for the annually bolded dates.  Months start on January.
        /// </devdoc>
        /// <internalonly/>
        private int[]       monthsOfYear = new int[12];
        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.datesToBoldMonthly"]/*' />
        /// <devdoc>
        ///     Bitmask for the dates bolded monthly.
        /// </devdoc>
        /// <internalonly/>
        private int         datesToBoldMonthly = 0;
        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.arrayOfDates"]/*' />
        /// <devdoc>
        ///     Lists are slow, so this section can be optimized.
        ///     Implementation is such that inserts are fast, removals are slow.
        /// </devdoc>
        /// <internalonly/>
        private ArrayList   arrayOfDates = new ArrayList();
        private ArrayList   annualArrayOfDates = new ArrayList(); // we have to maintain these lists too.
        private ArrayList   monthlyArrayOfDates = new ArrayList();

        // notifications
        private DateRangeEventHandler       onDateChanged;
        private DateRangeEventHandler       onDateSelected;
        private EventHandler                   onRightToLeftLayoutChanged;

        private int          nativeWndProcCount = 0;
        private static bool? restrictUnmanagedCode;

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.MonthCalendar"]/*' />
        /// <devdoc>
        ///     Creates a new MonthCalendar object.  Styles are the default for a
        ///     regular month calendar control.
        /// </devdoc>
        public MonthCalendar()
        : base() {

            PrepareForDrawing();

            selectionStart = todayDate;
            selectionEnd = todayDate;
            SetStyle(ControlStyles.UserPaint, false);
            SetStyle(ControlStyles.StandardClick, false);
            
            TabStop = true;

            if (!restrictUnmanagedCode.HasValue) {
                bool demandUnmanagedFailed = false;
                try {
                    IntSecurity.UnmanagedCode.Demand();
                    restrictUnmanagedCode = false;
                }
                catch {
                    // ensure that the static field is written to exactly once to avoid race conditions
                    demandUnmanagedFailed = true;
                }

                if (demandUnmanagedFailed) {
                    // Demand for unmanaged code failed, thus we are running in partial trust.
                    // We need to assert a registry access permission, this is safe because
                    // we are reading the registry and are not returning the information 
                    // from the registry to the user.
                    new RegistryPermission(PermissionState.Unrestricted).Assert();
                    try {
                        // for 32 bit applications on 64 bit machines this code is reading 
                        // HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node node

                        // to opt out, set a DWORD value AllowWindowsFormsReentrantDestroy=1
                        RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework");
                        if (key != null) {
                            object o = key.GetValue("AllowWindowsFormsReentrantDestroy");
                            if ((o != null) && (o is int) && ((int)o == 1)) {
                                restrictUnmanagedCode = false;
                            }
                            else {
                                restrictUnmanagedCode = true;
                            }
                        }
                        else {
                            restrictUnmanagedCode = true;
                        }
                    }
                    catch {
                        restrictUnmanagedCode = true;
                    }
                    finally {
                        System.Security.CodeAccessPermission.RevertAssert();
                    }
                }
            }
        }

        /// <summary>
        /// MonthCalendar control  accessbile object.
        /// </summary>
        /// <returns></returns>
        protected override AccessibleObject CreateAccessibilityInstance() {
            if (AccessibilityImprovements.Level1) {
                return new MonthCalendarAccessibleObject(this);
            }
            else {
                return base.CreateAccessibilityInstance();
            }
        }

        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew) {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            PrepareForDrawing();
        }

        private void PrepareForDrawing() {
            if (DpiHelper.IsScalingRequirementMet) {
                scaledExtraPadding = LogicalToDeviceUnits(ExtraPadding);
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.AnnuallyBoldedDates"]/*' />
        /// <devdoc>
        ///     The array of DateTime objects that determines which annual days are shown
        ///     in bold.
        /// </devdoc>
        [
        Localizable(true),
        SRDescription(nameof(SR.MonthCalendarAnnuallyBoldedDatesDescr))
        ]
        public DateTime[] AnnuallyBoldedDates {
            get {
                DateTime[] dateTimes = new DateTime[annualArrayOfDates.Count];

                for (int i=0;i < annualArrayOfDates.Count; ++i) {
                    dateTimes[i] = (DateTime)this.annualArrayOfDates[i];
                }
                return dateTimes;
            }
            set {
                // 



                this.annualArrayOfDates.Clear();
                for (int i=0; i<MONTHS_IN_YEAR; ++i)
                    monthsOfYear[i] = 0;

                if (value != null && value.Length > 0) {

                    //add each boldeddate to our ArrayList...
                    for (int i = 0; i < value.Length; i++) {
                        this.annualArrayOfDates.Add(value[i]);
                    }

                    for (int i = 0; i < value.Length; ++i) {
                        monthsOfYear[value[i].Month-1] |= 0x00000001<<(value[i].Day-1);
                    }

                }
                RecreateHandle();
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.BackColor"]/*' />
        [SRDescription(nameof(SR.MonthCalendarMonthBackColorDescr))]
        public override Color BackColor {
            get {
                if (ShouldSerializeBackColor()) {
                    return base.BackColor;
                }
                else {
                    return SystemColors.Window;
                }
            }
            set {
                base.BackColor = value;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.BackgroundImage"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage {
            get {
                return base.BackgroundImage;
            }
            set {
                base.BackgroundImage = value;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.BackgroundImageChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageChanged {
            add {
                base.BackgroundImageChanged += value;
            }
            remove {
                base.BackgroundImageChanged -= value;
            }
        }


        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.BackgroundImageLayout"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout {
            get {
                return base.BackgroundImageLayout;
            }
            set {
                base.BackgroundImageLayout = value;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.BackgroundImageLayoutChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageLayoutChanged {
            add {
                base.BackgroundImageLayoutChanged += value;
            }
            remove {
                base.BackgroundImageLayoutChanged -= value;
            }
        }


        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.BoldedDates"]/*' />
        /// <devdoc>
        ///     The array of DateTime objects that determines which non-recurring
        ///     specified dates are shown in bold.
        /// </devdoc>
        /*Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),*/
        [Localizable(true)]
        public DateTime[] BoldedDates {
            get {
                DateTime[] dateTimes = new DateTime[arrayOfDates.Count];

                for (int i=0;i < arrayOfDates.Count; ++i) {
                    dateTimes[i] = (DateTime)this.arrayOfDates[i];
                }
                return dateTimes;
            }
            set {
                // 



                this.arrayOfDates.Clear();
                if (value != null && value.Length > 0) {

                    //add each boldeddate to our ArrayList...
                    for (int i = 0; i < value.Length; i++) {
                        this.arrayOfDates.Add(value[i]);
                    }

                }
                RecreateHandle();
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.CalendarDimensions"]/*' />
        /// <devdoc>
        ///     The number of columns and rows of months that will be displayed
        ///     in the MonthCalendar control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        SRDescription(nameof(SR.MonthCalendarDimensionsDescr))
        ]
        public Size CalendarDimensions {
            get {
                return dimensions;
            }
            set {
                if (!this.dimensions.Equals(value))
                    SetCalendarDimensions(value.Width, value.Height);
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.CreateParams"]/*' />
        /// <devdoc>
        ///     This is called when creating a window.  Inheriting classes can ovveride
        ///     this to add extra functionality, but should not forget to first call
        ///     base.getCreateParams() to make sure the control continues to work
        ///     correctly.
        /// </devdoc>
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get {
                CreateParams cp = base.CreateParams;
                cp.ClassName = NativeMethods.WC_MONTHCAL;
                cp.Style |= NativeMethods.MCS_MULTISELECT | NativeMethods.MCS_DAYSTATE;
                if (!showToday) cp.Style |= NativeMethods.MCS_NOTODAY;
                if (!showTodayCircle) cp.Style |= NativeMethods.MCS_NOTODAYCIRCLE;
                if (showWeekNumbers) cp.Style |= NativeMethods.MCS_WEEKNUMBERS;

                if (RightToLeft == RightToLeft.Yes && RightToLeftLayout == true) {
                    //We want to turn on mirroring for Form explicitly.
                    cp.ExStyle |= NativeMethods.WS_EX_LAYOUTRTL;
                    //Don't need these styles when mirroring is turned on.
                    cp.ExStyle &= ~(NativeMethods.WS_EX_RTLREADING | NativeMethods.WS_EX_RIGHT | NativeMethods.WS_EX_LEFTSCROLLBAR);
                }

                return cp;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.DefaultImeMode"]/*' />
        protected override ImeMode DefaultImeMode {
            get {
                return ImeMode.Disable;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.DefaultMargin"]/*' />
        protected override Padding DefaultMargin {
            get { return new Padding(9); }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.DefaultSize"]/*' />
        protected override Size DefaultSize {
            get {
                return GetMinReqRect();
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.DoubleBuffered"]/*' />
        /// <devdoc>
        ///     This property is overridden and hidden from statement completion
        ///     on controls that are based on Win32 Native Controls.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override bool DoubleBuffered {
            get {
                return base.DoubleBuffered;
            }
            set {
                base.DoubleBuffered = value;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.FirstDayOfWeek"]/*' />
        /// <devdoc>
        ///     The first day of the week for the month calendar control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        DefaultValue(DEFAULT_FIRST_DAY_OF_WEEK),
        SRDescription(nameof(SR.MonthCalendarFirstDayOfWeekDescr))
        ]
        public Day FirstDayOfWeek {
            get {
                return firstDayOfWeek;
            }

            set {
                //valid values are 0x0 to 0x7
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)Day.Monday, (int)Day.Default)){
                     throw new InvalidEnumArgumentException(nameof(FirstDayOfWeek), (int)value, typeof(Day));
                }

                if (value != firstDayOfWeek) {
                    firstDayOfWeek = value;
                    if (IsHandleCreated) {
                        if (value == Day.Default) {
                            RecreateHandle();
                        }
                        else {
                            SendMessage(NativeMethods.MCM_SETFIRSTDAYOFWEEK, 0, (int) value);
                        }
                    }
                }
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.ForeColor"]/*' />
        [SRDescription(nameof(SR.MonthCalendarForeColorDescr))]
        public override Color ForeColor {
            get {
                if (ShouldSerializeForeColor()) {
                    return base.ForeColor;
                }
                else {
                    return SystemColors.WindowText;
                }
            }
            set {
                base.ForeColor = value;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.ImeMode"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public ImeMode ImeMode {
            get {
                return base.ImeMode;
            }
            set {
                base.ImeMode = value;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.ImeModeChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler ImeModeChanged {
            add {
                base.ImeModeChanged += value;
            }
            remove {
                base.ImeModeChanged -= value;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.MaxDate"]/*' />
        /// <devdoc>
        ///     The maximum allowable date that can be selected.  By default, there
        ///     is no maximum date.  The maximum date is not set if max less than the
        ///     current minimum date.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MonthCalendarMaxDateDescr))
        ]
        public DateTime MaxDate {
            get {
                return DateTimePicker.EffectiveMaxDate(maxDate);
            }
            set {
                if (value != maxDate) {
                    if (value < DateTimePicker.EffectiveMinDate(minDate)) {
                        throw new ArgumentOutOfRangeException(nameof(MaxDate), string.Format(SR.InvalidLowBoundArgumentEx, "MaxDate", FormatDate(value), "MinDate"));
                    }
                    maxDate = value;
                    SetRange();
                }
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.MaxSelectionCount"]/*' />
        /// <devdoc>
        ///     The maximum number of days that can be selected in a
        ///     month calendar control.  This method does not affect the current
        ///     selection range.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(DEFAULT_MAX_SELECTION_COUNT),
        SRDescription(nameof(SR.MonthCalendarMaxSelectionCountDescr))
        ]
        public int MaxSelectionCount {
            get {
                return maxSelectionCount;
            }
            set {
                if (value < 1) {
                    throw new ArgumentOutOfRangeException(nameof(MaxSelectionCount), string.Format(SR.InvalidLowBoundArgumentEx, "MaxSelectionCount", (value).ToString("D", CultureInfo.CurrentCulture), (1).ToString(CultureInfo.CurrentCulture)));
                }

                if (value != maxSelectionCount) {
                    if (IsHandleCreated) {
                        if (unchecked( (int) (long)SendMessage(NativeMethods.MCM_SETMAXSELCOUNT, value, 0)) == 0)
                            throw new ArgumentException(string.Format(SR.MonthCalendarMaxSelCount, (value).ToString("D", CultureInfo.CurrentCulture)), "MaxSelectionCount");
                    }
                    maxSelectionCount = value;
                }
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.MinDate"]/*' />
        /// <devdoc>
        ///     The minimum allowable date that can be selected.  By default, there
        ///     is no minimum date.  The minimum date is not set if min greater than the
        ///     current maximum date.  MonthCalendar does not support dates prior to 1753.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MonthCalendarMinDateDescr))
        ]
        public DateTime MinDate {
            get {
                return DateTimePicker.EffectiveMinDate(minDate);
            }
            set {
                if (value != minDate) {
                    if (value > DateTimePicker.EffectiveMaxDate(maxDate)) {
                        throw new ArgumentOutOfRangeException(nameof(MinDate), string.Format(SR.InvalidHighBoundArgument, "MinDate", FormatDate(value), "MaxDate"));
                    }

                    // If trying to set the minimum less than DateTimePicker.MinimumDateTime, throw
                    // an exception.
                    if (value < DateTimePicker.MinimumDateTime) {
                        throw new ArgumentOutOfRangeException(nameof(MinDate), string.Format(SR.InvalidLowBoundArgumentEx, "MinDate", FormatDate(value), FormatDate(DateTimePicker.MinimumDateTime)));
                    }

                    minDate = value;
                    SetRange();
                }
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.MonthlyBoldedDates"]/*' />
        /// <devdoc>
        ///     The array of DateTime objects that determine which monthly days to bold.
        /// </devdoc>
        [
        Localizable(true),
        SRDescription(nameof(SR.MonthCalendarMonthlyBoldedDatesDescr))
        ]
        public DateTime[] MonthlyBoldedDates {
            get {
                DateTime[] dateTimes = new DateTime[monthlyArrayOfDates.Count];

                for (int i=0;i < monthlyArrayOfDates.Count; ++i) {
                    dateTimes[i] = (DateTime)this.monthlyArrayOfDates[i];
                }
                return dateTimes;
            }
            set {
                // 



                this.monthlyArrayOfDates.Clear();
                datesToBoldMonthly = 0;

                if (value != null && value.Length > 0) {

                    //add each boldeddate to our ArrayList...
                    for (int i = 0; i < value.Length; i++) {
                        this.monthlyArrayOfDates.Add(value[i]);
                    }

                    for (int i = 0; i < value.Length; ++i) {
                        datesToBoldMonthly |= 0x00000001<<(value[i].Day-1);
                    }

                }
                RecreateHandle();
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.Now"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private DateTime Now {
            get {
                return DateTime.Now.Date;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.Padding"]/*' />
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new Padding Padding {
            get { return base.Padding; }
            set { base.Padding = value;}
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public new event EventHandler PaddingChanged {
            add { base.PaddingChanged += value; }
            remove { base.PaddingChanged -= value; }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.RightToLeftLayout"]/*' />
        /// <devdoc>
        ///     This is used for international applications where the language
        ///     is written from RightToLeft. When this property is true,
        //      and the RightToLeft is true, mirroring will be turned on on the form, and
        ///     control placement and text will be from right to left.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        DefaultValue(false),
        SRDescription(nameof(SR.ControlRightToLeftLayoutDescr))
        ]
        public virtual bool RightToLeftLayout {
            get {

                return rightToLeftLayout;
            }

            set {
                if (value != rightToLeftLayout) {
                    rightToLeftLayout = value;
                    using(new LayoutTransaction(this, this, PropertyNames.RightToLeftLayout)) {
                        OnRightToLeftLayoutChanged(EventArgs.Empty);
                    }
                }
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.ScrollChange"]/*' />
        /// <devdoc>
        ///     The scroll rate for a month calendar control. The scroll
        ///     rate is the number of months that the control moves its display
        ///     when the user clicks a scroll button.  If this value is zero,
        ///     the month delta is reset to the default, which is the number of
        ///     months displayed in the control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(DEFAULT_SCROLL_CHANGE),
        SRDescription(nameof(SR.MonthCalendarScrollChangeDescr))
        ]
        public int ScrollChange {
            get {
                return scrollChange;
            }
            set {
                if (scrollChange != value) {

                    if (value < 0) {
                        throw new ArgumentOutOfRangeException(nameof(ScrollChange), string.Format(SR.InvalidLowBoundArgumentEx, "ScrollChange", (value).ToString("D", CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
                    }
                    if (value > MaxScrollChange) {
                        throw new ArgumentOutOfRangeException(nameof(ScrollChange), string.Format(SR.InvalidHighBoundArgumentEx, "ScrollChange", (value).ToString("D", CultureInfo.CurrentCulture), (MaxScrollChange).ToString("D", CultureInfo.CurrentCulture)));
                    }

                    if (IsHandleCreated) {
                        SendMessage(NativeMethods.MCM_SETMONTHDELTA, value, 0);
                    }
                    scrollChange = value;
                }
            }
        }


        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.SelectionEnd"]/*' />
        /// <devdoc>
        ///    <para>Indicates the end date of the selected range of dates.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.MonthCalendarSelectionEndDescr))
        ]
        public DateTime SelectionEnd {
            get {
                return selectionEnd;
            }
            set {
                if (selectionEnd != value) {

                    // Keep SelectionEnd within min and max
                    if (value < MinDate) {
                        throw new ArgumentOutOfRangeException(nameof(SelectionEnd), string.Format(SR.InvalidLowBoundArgumentEx, "SelectionEnd", FormatDate(value), "MinDate"));
                    }
                    if (value > MaxDate) {
                        throw new ArgumentOutOfRangeException(nameof(SelectionEnd), string.Format(SR.InvalidHighBoundArgumentEx, "SelectionEnd", FormatDate(value), "MaxDate"));
                    }

                    // If we've moved SelectionEnd before SelectionStart, move SelectionStart back
                    if (selectionStart > value) {
                        selectionStart = value;
                    }

                    // If we've moved SelectionEnd too far beyond SelectionStart, move SelectionStart forward
                    if ((value - selectionStart).Days >= maxSelectionCount) {
                        selectionStart = value.AddDays(1 - maxSelectionCount);
                    }

                    // Set the new selection range
                    SetSelRange(selectionStart, value);
                }
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.SelectionStart"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Indicates
        ///       the start date of the selected range of dates.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.MonthCalendarSelectionStartDescr))
        ]
        public DateTime SelectionStart {
            get {
                return selectionStart;
            }
            set {
                if (selectionStart != value) {

                    // Keep SelectionStart within min and max
                    //
                    if (value < minDate) {
                        throw new ArgumentOutOfRangeException(nameof(SelectionStart), string.Format(SR.InvalidLowBoundArgumentEx, "SelectionStart", FormatDate(value), "MinDate"));
                    }
                    if (value > maxDate) {
                        throw new ArgumentOutOfRangeException(nameof(SelectionStart), string.Format(SR.InvalidHighBoundArgumentEx, "SelectionStart", FormatDate(value), "MaxDate"));
                    }

                    // If we've moved SelectionStart beyond SelectionEnd, move SelectionEnd forward
                    if (selectionEnd < value) {
                        selectionEnd = value;
                    }

                    // If we've moved SelectionStart too far back from SelectionEnd, move SelectionEnd back
                    if ((selectionEnd - value).Days >= maxSelectionCount) {
                        selectionEnd = value.AddDays(maxSelectionCount - 1);
                    }

                    // Set the new selection range
                    SetSelRange(value, selectionEnd);
                }
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.SelectionRange"]/*' />
        /// <devdoc>
        ///     Retrieves the selection range for a month calendar control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MonthCalendarSelectionRangeDescr)),
        Bindable(true)
        ]
        public SelectionRange SelectionRange {
            get {
                return new SelectionRange(SelectionStart, SelectionEnd);
            }
            set {
                SetSelectionRange(value.Start, value.End);
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.ShowToday"]/*' />
        /// <devdoc>
        ///     Indicates whether the month calendar control will display
        ///     the "today" date at the bottom of the control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.MonthCalendarShowTodayDescr))
        ]
        public bool ShowToday {
            get {
                return showToday;
            }
            set {
                if (showToday != value) {
                    showToday = value;
                    UpdateStyles();
                    AdjustSize();
                }
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.ShowTodayCircle"]/*' />
        /// <devdoc>
        ///     Indicates whether the month calendar control will circle
        ///     the "today" date.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.MonthCalendarShowTodayCircleDescr))
        ]
        public bool ShowTodayCircle {
            get {
                return showTodayCircle;
            }
            set {
                if (showTodayCircle != value) {
                    showTodayCircle = value;
                    UpdateStyles();
                }
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.ShowWeekNumbers"]/*' />
        /// <devdoc>
        ///     Indicates whether the month calendar control will the display
        ///     week numbers (1-52) to the left of each row of days.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        DefaultValue(false),
        SRDescription(nameof(SR.MonthCalendarShowWeekNumbersDescr))
        ]
        public bool ShowWeekNumbers {
            get {
                return showWeekNumbers;
            }
            set {
                if (showWeekNumbers != value) {
                    showWeekNumbers = value;
                    UpdateStyles();
                    AdjustSize();
                }
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.SingleMonthSize"]/*' />
        /// <devdoc>
        ///     The minimum size required to display a full month.  The size
        ///     information is presented in the form of a Point, with the x
        ///     and y members representing the minimum width and height required
        ///     for the control.  The minimum required window size for a month calendar
        ///     control depends on the currently selected font.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.MonthCalendarSingleMonthSizeDescr))
        ]
        public Size SingleMonthSize {
            get {
                NativeMethods.RECT rect = new NativeMethods.RECT();

                if (IsHandleCreated) {

                    if (unchecked( (int) (long)SendMessage(NativeMethods.MCM_GETMINREQRECT, 0, ref rect)) == 0)
                        throw new InvalidOperationException(SR.InvalidSingleMonthSize);

                    return new Size(rect.right, rect.bottom);
                }

                return DefaultSingleMonthSize;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.Size"]/*' />
        /// <devdoc>
        ///     Unlike most controls, serializing the MonthCalendar's Size is really bad:
        ///     when it's restored at runtime, it uses a a default SingleMonthSize, which 
        ///     may not be right, especially for JPN/CHS machines.
        /// </devdoc>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Localizable(false)
        ]
        public new Size Size {
            get {
                return base.Size;
            }
            set {
                base.Size = value;
            }
        }


        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.Text"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Bindable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.TextChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged {
            add {
                base.TextChanged += value;
            }
            remove {
                base.TextChanged -= value;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.TodayDate"]/*' />
        /// <devdoc>
        ///     The date shown as "Today" in the Month Calendar control.
        ///     By default, "Today" is the current date at the time
        ///     the MonthCalendar control is created.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MonthCalendarTodayDateDescr))
        ]
        public DateTime TodayDate {
            get {
                if (todayDateSet) return todayDate;
                if (IsHandleCreated) {
                    NativeMethods.SYSTEMTIME st = new NativeMethods.SYSTEMTIME();
                    int res = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.MCM_GETTODAY, 0, st);
                    Debug.Assert(res != 0, "MCM_GETTODAY failed");
                    return DateTimePicker.SysTimeToDateTime(st).Date;
                }
                else return Now.Date;
            }
            set {
                if (!(todayDateSet) || (DateTime.Compare(value, todayDate) != 0)) {

                    // throw if trying to set the TodayDate to a value greater than MaxDate
                    if (DateTime.Compare(value, maxDate) > 0) {
                        throw new ArgumentOutOfRangeException(nameof(TodayDate), string.Format(SR.InvalidHighBoundArgumentEx, "TodayDate", FormatDate(value), FormatDate(maxDate)));
                    }

                    // throw if trying to set the TodayDate to a value less than MinDate
                    if (DateTime.Compare(value, minDate) < 0) {
                        throw new ArgumentOutOfRangeException(nameof(TodayDate), string.Format(SR.InvalidLowBoundArgument, "TodayDate", FormatDate(value), FormatDate(minDate)));
                    }

                    todayDate = value.Date;
                    todayDateSet = true;
                    UpdateTodayDate();
                }
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.TodayDateSet"]/*' />
        /// <devdoc>
        ///     Indicates whether or not the TodayDate property has been explicitly
        ///     set by the user. If TodayDateSet is true, TodayDate will return whatever
        ///     the user has set it to. If TodayDateSet is false, TodayDate will follow
        ///     wall-clock time; ie. TodayDate will always equal the current system date.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.MonthCalendarTodayDateSetDescr))
        ]
        public bool TodayDateSet {
            get {
                return todayDateSet;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.TitleBackColor"]/*' />
        /// <devdoc>
        ///     The background color displayed in the month calendar's
        ///     title.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.MonthCalendarTitleBackColorDescr))
        ]
        public Color TitleBackColor {
            get {
                return titleBackColor;
            }
            set {
                if (value.IsEmpty) {
                    throw new ArgumentException(string.Format(SR.InvalidNullArgument,
                                                              "value"));
                }
                titleBackColor = value;
                SetControlColor(NativeMethods.MCSC_TITLEBK, value);
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.TitleForeColor"]/*' />
        /// <devdoc>
        ///     The foreground color used to display text within the month
        ///     calendar's title.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.MonthCalendarTitleForeColorDescr))
        ]
        public Color TitleForeColor {
            get {
                return titleForeColor;
            }
            set {
                if (value.IsEmpty) {
                    throw new ArgumentException(string.Format(SR.InvalidNullArgument,
                                                              "value"));
                }
                titleForeColor = value;
                SetControlColor(NativeMethods.MCSC_TITLETEXT, value);
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.TrailingForeColor"]/*' />
        /// <devdoc>
        ///     The color used to display the previous and following months that
        ///     appear on the current month calendar.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.MonthCalendarTrailingForeColorDescr))
        ]
        public Color TrailingForeColor {
            get {
                return trailingForeColor;
            }
            set {
                if (value.IsEmpty) {
                    throw new ArgumentException(string.Format(SR.InvalidNullArgument,
                                                              "value"));
                }
                trailingForeColor = value;
                SetControlColor(NativeMethods.MCSC_TRAILINGTEXT, value);
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.AddAnnuallyBoldedDate"]/*' />
        /// <devdoc>
        ///     Adds a day that will be bolded annually on the month calendar.
        ///     Be sure to call updateBoldedDates() afterwards.
        /// </devdoc>
        public void AddAnnuallyBoldedDate(DateTime date) {
            annualArrayOfDates.Add(date);
            monthsOfYear[date.Month-1] |= 0x00000001<<(date.Day-1);
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.AddBoldedDate"]/*' />
        /// <devdoc>
        ///     Adds a day that will be bolded on the month calendar.
        ///     Be sure to call updateBoldedDates() afterwards.
        /// </devdoc>
        public void AddBoldedDate(DateTime date) {
            if (!this.arrayOfDates.Contains(date)) {
                this.arrayOfDates.Add(date);
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.AddMonthlyBoldedDate"]/*' />
        /// <devdoc>
        ///     Adds a day that will be bolded monthly on the month calendar.
        ///     Be sure to call updateBoldedDates() afterwards.
        /// </devdoc>
        public void AddMonthlyBoldedDate(DateTime date) {
            this.monthlyArrayOfDates.Add(date);
            datesToBoldMonthly |= 0x00000001<<(date.Day-1);
        }


        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.Click"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler Click {
            add {
                base.Click += value;
            }
            remove {
                base.Click -= value;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.DateChanged"]/*' />
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.MonthCalendarOnDateChangedDescr))]
        public event DateRangeEventHandler DateChanged {
            add {
                onDateChanged += value;
            }
            remove {
                onDateChanged -= value;
            }
        }


        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.DateSelected"]/*' />
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.MonthCalendarOnDateSelectedDescr))]
        public event DateRangeEventHandler DateSelected {
            add {
                onDateSelected += value;
            }
            remove {
                onDateSelected -= value;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.DoubleClick"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler DoubleClick {
            add {
                base.DoubleClick += value;
            }
            remove {
                base.DoubleClick -= value;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.MouseClick"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event MouseEventHandler MouseClick {
            add {
                base.MouseClick += value;
            }
            remove {
                base.MouseClick -= value;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.MouseDoubleClick"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event MouseEventHandler MouseDoubleClick {
            add {
                base.MouseDoubleClick += value;
            }
            remove {
                base.MouseDoubleClick -= value;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.OnPaint"]/*' />
        /// <devdoc>
        ///     MonthCalendar Onpaint.
        /// </devdoc>
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event PaintEventHandler Paint {
            add {
                base.Paint += value;
            }
            remove {
                base.Paint -= value;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.RightToLeftLayoutChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
        public event EventHandler RightToLeftLayoutChanged {
            add {
                onRightToLeftLayoutChanged += value;
            }
            remove {
                onRightToLeftLayoutChanged -= value;
            }
        }


        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.AdjustSize"]/*' />
        /// <devdoc>
        ///     Used to auto-size the control.  The requested number of rows and columns are
        ///     restricted by the maximum size of the parent control, hence the requested number
        ///     of rows and columns may not be what you get.
        /// </devdoc>
        /// <internalonly/>
        private void AdjustSize() {
            Size minSize = GetMinReqRect();
            Size = minSize;
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.BoldDates"]/*' />
        /// <devdoc>
        ///     Event handler that bolds dates indicated by arrayOfDates
        /// </devdoc>
        /// <internalonly/>
        private void BoldDates(DateBoldEventArgs e) {
            int months = e.Size;
            e.DaysToBold = new int[months];
            SelectionRange range = GetDisplayRange(false);
            int startMonth = range.Start.Month;
            int startYear = range.Start.Year;
            int numDates = arrayOfDates.Count;
            for (int i=0; i<numDates; ++i) {
                DateTime date = (DateTime) arrayOfDates[i];
                if (DateTime.Compare(date, range.Start) >= 0 && DateTime.Compare(date, range.End) <= 0) {
                    int month = date.Month;
                    int year = date.Year;
                    int index = (year == startYear) ? month - startMonth : month + year*MONTHS_IN_YEAR - startYear*MONTHS_IN_YEAR - startMonth;
                    e.DaysToBold[index] |= (0x00000001<<(date.Day-1));
                }
            }
            //now we figure out which monthly and annual dates to bold
            --startMonth;
            for (int i=0; i<months; ++i, ++startMonth)
                e.DaysToBold[i] |= monthsOfYear[startMonth % MONTHS_IN_YEAR] | datesToBoldMonthly;
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.CompareDayAndMonth"]/*' />
        /// <devdoc>
        ///     Compares only the day and month of each time.
        /// </devdoc>
        /// <internalonly/>
        private bool CompareDayAndMonth(DateTime t1, DateTime t2) {
            return(t1.Day == t2.Day && t1.Month == t2.Month);
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.CreateHandle"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override void CreateHandle() {
            if (!RecreatingHandle) {
                IntPtr userCookie = UnsafeNativeMethods.ThemingScope.Activate();   
                try {
                    NativeMethods.INITCOMMONCONTROLSEX icc = new NativeMethods.INITCOMMONCONTROLSEX();
                    icc.dwICC = NativeMethods.ICC_DATE_CLASSES;
                    SafeNativeMethods.InitCommonControlsEx(icc);
                }
                finally {
                    UnsafeNativeMethods.ThemingScope.Deactivate(userCookie);
                }
            }
            base.CreateHandle();
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.Dispose"]/*' />
        /// <devdoc>
        ///     Called to cleanup a MonthCalendar.  Normally you do not need
        ///     to call this as the garbage collector will cleanup the buffer
        ///     for you.  However, there may be times when you may want to expedite
        ///     the garbage collectors cleanup.
        /// </devdoc>
        protected override void Dispose(bool disposing) {
            if (mdsBuffer != IntPtr.Zero) {
                Marshal.FreeHGlobal(mdsBuffer);
                mdsBuffer = IntPtr.Zero;
            }
            base.Dispose(disposing);
        }

        // Return a localized string representation of the given DateTime value.
        // Used for throwing exceptions, etc.
        //
        private static string FormatDate(DateTime value) {
            return value.ToString("d", CultureInfo.CurrentCulture);
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.GetDisplayRange"]/*' />
        /// <devdoc>
        ///     Retrieves date information that represents the low and high limits of the
        ///     control's display.
        /// </devdoc>
        public SelectionRange GetDisplayRange(bool visible) {
            if (visible)
                return GetMonthRange(NativeMethods.GMR_VISIBLE);
            else
                return GetMonthRange(NativeMethods.GMR_DAYSTATE);
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.GetHitArea"]/*' />
        /// <devdoc>
        ///     Retrieves the enumeration value corresponding to the hit area.
        /// </devdoc>
        /// <internalonly/>
        private HitArea GetHitArea(int hit) {
            switch (hit) {
                case NativeMethods.MCHT_TITLEBK:
                    return HitArea.TitleBackground;
                case NativeMethods.MCHT_TITLEMONTH:
                    return HitArea.TitleMonth;
                case NativeMethods.MCHT_TITLEYEAR:
                    return HitArea.TitleYear;
                case NativeMethods.MCHT_TITLEBTNNEXT:
                    return HitArea.NextMonthButton;
                case NativeMethods.MCHT_TITLEBTNPREV:
                    return HitArea.PrevMonthButton;
                case NativeMethods.MCHT_CALENDARBK:
                    return HitArea.CalendarBackground;
                case NativeMethods.MCHT_CALENDARDATE:
                    return HitArea.Date;
                case NativeMethods.MCHT_CALENDARDATENEXT:
                    return HitArea.NextMonthDate;
                case NativeMethods.MCHT_CALENDARDATEPREV:
                    return HitArea.PrevMonthDate;
                case NativeMethods.MCHT_CALENDARDAY:
                    return HitArea.DayOfWeek;
                case NativeMethods.MCHT_CALENDARWEEKNUM:
                    return HitArea.WeekNumbers;
                case NativeMethods.MCHT_TODAYLINK:
                    return HitArea.TodayLink;
                default:
                    return HitArea.Nowhere;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.GetMinReqRect"]/*' />
        /// <devdoc>
        ///     stub for getMinReqRect (int, boolean)
        /// </devdoc>
        /// <internalonly/>
        private Size GetMinReqRect() {
            return GetMinReqRect(0, false, false);
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.GetMinReqRect1"]/*' />
        /// <devdoc>
        ///     Used internally to get the minimum size needed to display the
        ///     MonthCalendar.  This is needed because
        ///     NativeMethods.MCM_GETMINREQRECT returns an incorrect value if showToday
        ///     is set to false.  If updateRows is true, then the
        ///     number of rows will be updated according to height.
        /// </devdoc>
        /// <internalonly/>
        private Size GetMinReqRect(int newDimensionLength, bool updateRows, bool updateCols) {
            Size minSize = SingleMonthSize;

            // Calculate calendar height
            //
            Size textExtent;
            using (WindowsFont font = WindowsFont.FromFont(this.Font))
            {
                // this is the string that Windows uses to determine the extent of the today string
                textExtent = WindowsGraphicsCacheManager.MeasurementGraphics.GetTextExtent(DateTime.Now.ToShortDateString(), font);
            }
            int todayHeight = textExtent.Height + 4;  // The constant 4 is from the comctl32 MonthCalendar source code
            int calendarHeight = minSize.Height;
            if (ShowToday) {
                // If ShowToday is true, then minSize already includes the height of the today string.
                // So we remove it to get the actual calendar height.
                //
                calendarHeight -= todayHeight;
            }

            if (updateRows) {
                Debug.Assert(calendarHeight > INSERT_HEIGHT_SIZE, "Divide by 0");
                int nRows = (newDimensionLength - todayHeight + INSERT_HEIGHT_SIZE)/(calendarHeight + INSERT_HEIGHT_SIZE);
                this.dimensions.Height = (nRows < 1) ? 1 : nRows;
            }

            if (updateCols) {
                Debug.Assert(minSize.Width > INSERT_WIDTH_SIZE, "Divide by 0");
                int nCols = (newDimensionLength - scaledExtraPadding) /minSize.Width;
                this.dimensions.Width = (nCols < 1) ? 1 : nCols;
            }

            minSize.Width = (minSize.Width + INSERT_WIDTH_SIZE) * dimensions.Width - INSERT_WIDTH_SIZE;
            minSize.Height = (calendarHeight + INSERT_HEIGHT_SIZE) * dimensions.Height - INSERT_HEIGHT_SIZE + todayHeight;

            // If the width we've calculated is too small to fit the Today string, enlarge the width to fit
            //
            if (IsHandleCreated) {
                int maxTodayWidth = unchecked( (int) (long)SendMessage(NativeMethods.MCM_GETMAXTODAYWIDTH, 0, 0));
                if (maxTodayWidth > minSize.Width) {
                    minSize.Width = maxTodayWidth;
                }
            }

            // Fudge factor
            //
            minSize.Width += scaledExtraPadding;
            minSize.Height += scaledExtraPadding;
            return minSize;
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.GetMonthRange"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private SelectionRange GetMonthRange(int flag) {
            NativeMethods.SYSTEMTIMEARRAY sa = new NativeMethods.SYSTEMTIMEARRAY();
            SelectionRange range = new SelectionRange();
            UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.MCM_GETMONTHRANGE, flag, sa);

            NativeMethods.SYSTEMTIME st = new NativeMethods.SYSTEMTIME();
            st.wYear = sa.wYear1;
            st.wMonth = sa.wMonth1;
            st.wDayOfWeek = sa.wDayOfWeek1;
            st.wDay = sa.wDay1;

            range.Start = DateTimePicker.SysTimeToDateTime(st);
            st.wYear = sa.wYear2;
            st.wMonth = sa.wMonth2;
            st.wDayOfWeek = sa.wDayOfWeek2;
            st.wDay = sa.wDay2;
            range.End = DateTimePicker.SysTimeToDateTime(st);

            return range;
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.GetPreferredHeight"]/*' />
        /// <devdoc>
        ///     Called by setBoundsCore.  If updateRows is true, then the
        ///     number of rows will be updated according to height.
        /// </devdoc>
        /// <internalonly/>
        private int GetPreferredHeight(int height, bool updateRows) {
            Size preferredSize = GetMinReqRect(height, updateRows, false);
            return preferredSize.Height;
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.GetPreferredWidth"]/*' />
        /// <devdoc>
        ///     Called by setBoundsCore.  If updateCols is true, then the
        ///     number of columns will be updated according to width.
        /// </devdoc>
        /// <internalonly/>
        private int GetPreferredWidth(int width, bool updateCols) {
            Size preferredSize = GetMinReqRect(width, false, updateCols);
            return preferredSize.Width;
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.HitTest"]/*' />
        /// <devdoc>
        ///     Determines which portion of a month calendar control is at
        ///     at a given point on the screen.
        /// </devdoc>
        public HitTestInfo HitTest(int x, int y) {
            NativeMethods.MCHITTESTINFO mchi = new NativeMethods.MCHITTESTINFO();
            mchi.pt_x = x;
            mchi.pt_y = y;
            mchi.cbSize = Marshal.SizeOf(typeof(NativeMethods.MCHITTESTINFO));
            UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.MCM_HITTEST, 0, mchi);

            // If the hit area has an associated valid date, get it
            //
            HitArea hitArea = GetHitArea(mchi.uHit);
            if (HitTestInfo.HitAreaHasValidDateTime(hitArea)) {
                NativeMethods.SYSTEMTIME sys = new NativeMethods.SYSTEMTIME();
                sys.wYear = mchi.st_wYear;
                sys.wMonth = mchi.st_wMonth;
                sys.wDayOfWeek = mchi.st_wDayOfWeek;
                sys.wDay = mchi.st_wDay;
                sys.wHour = mchi.st_wHour;
                sys.wMinute = mchi.st_wMinute;
                sys.wSecond = mchi.st_wSecond;
                sys.wMilliseconds = mchi.st_wMilliseconds;
                return new HitTestInfo(new Point(mchi.pt_x, mchi.pt_y), hitArea, DateTimePicker.SysTimeToDateTime(sys));
            }
            else {
                return new HitTestInfo(new Point(mchi.pt_x, mchi.pt_y), hitArea);
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.HitTest1"]/*' />
        /// <devdoc>
        ///     Determines which portion of a month calendar control is at
        ///     at a given point on the screen.
        /// </devdoc>
        public HitTestInfo HitTest(Point point) {
            return HitTest(point.X, point.Y);
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.IsInputKey"]/*' />
        /// <devdoc>
        ///      Handling special input keys, such as pgup, pgdown, home, end, etc...
        /// </devdoc>
        protected override bool IsInputKey(Keys keyData) {
            if ((keyData & Keys.Alt) == Keys.Alt) return false;
            switch (keyData & Keys.KeyCode) {
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.Home:
                case Keys.End:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.OnHandleCreated"]/*' />
        /// <devdoc>
        ///     Overrides Control.OnHandleCreated()
        /// </devdoc>
        /// <internalonly/>
        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            SetSelRange(selectionStart, selectionEnd);
            if (maxSelectionCount != DEFAULT_MAX_SELECTION_COUNT) {
                SendMessage(NativeMethods.MCM_SETMAXSELCOUNT, maxSelectionCount, 0);
            }
            AdjustSize();

            if (todayDateSet) {
                NativeMethods.SYSTEMTIME st = DateTimePicker.DateTimeToSysTime(todayDate);
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.MCM_SETTODAY, 0, st);
            }

            SetControlColor(NativeMethods.MCSC_TEXT, ForeColor);
            SetControlColor(NativeMethods.MCSC_MONTHBK, BackColor);
            SetControlColor(NativeMethods.MCSC_TITLEBK, titleBackColor);
            SetControlColor(NativeMethods.MCSC_TITLETEXT, titleForeColor);
            SetControlColor(NativeMethods.MCSC_TRAILINGTEXT, trailingForeColor);

            int firstDay;
            if (firstDayOfWeek == Day.Default) {
                firstDay = NativeMethods.LOCALE_IFIRSTDAYOFWEEK;
            }
            else {
                firstDay = (int)firstDayOfWeek;
            }
            SendMessage(NativeMethods.MCM_SETFIRSTDAYOFWEEK, 0, firstDay);

            SetRange();
            if (scrollChange != DEFAULT_SCROLL_CHANGE) {
                SendMessage(NativeMethods.MCM_SETMONTHDELTA, scrollChange, 0);
            }

            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(this.MarshaledUserPreferenceChanged);
        }


        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.OnHandleDestroyed"]/*' />
        /// <devdoc>
        ///     Overrides Control.OnHandleDestroyed()
        /// </devdoc>
        /// <internalonly/>
        protected override void OnHandleDestroyed(EventArgs e) {
            SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(this.MarshaledUserPreferenceChanged);
            base.OnHandleDestroyed(e);
        }


        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.OnDateChanged"]/*' />
        /// <devdoc>
        ///     Fires the event indicating that the currently selected date
        ///     or range of dates has changed.
        /// </devdoc>
        protected virtual void OnDateChanged(DateRangeEventArgs drevent) {
            if (onDateChanged != null) {
                onDateChanged(this, drevent);
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.OnDateSelected"]/*' />
        /// <devdoc>
        ///     Fires the event indicating that the user has changed his\her selection.
        /// </devdoc>
        protected virtual void OnDateSelected(DateRangeEventArgs drevent) {
            if (onDateSelected != null) {
                onDateSelected(this, drevent);
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.OnFontChanged"]/*' />
        protected override void OnFontChanged(EventArgs e) {
            base.OnFontChanged(e);
            AdjustSize();
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.OnForeColorChanged"]/*' />
        protected override void OnForeColorChanged(EventArgs e) {
            base.OnForeColorChanged(e);
            SetControlColor(NativeMethods.MCSC_TEXT, ForeColor);
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.OnBackColorChanged"]/*' />
        protected override void OnBackColorChanged(EventArgs e) {
            base.OnBackColorChanged(e);
            SetControlColor(NativeMethods.MCSC_MONTHBK, BackColor);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnRightToLeftLayoutChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnRightToLeftLayoutChanged(EventArgs e) {
            if (GetAnyDisposingInHierarchy()) {
                return;
            }

            if (RightToLeft == RightToLeft.Yes) {
                RecreateHandle();
            }

            if (onRightToLeftLayoutChanged != null) {
                 onRightToLeftLayoutChanged(this, e);
            }
        }



        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.RemoveAllAnnuallyBoldedDates"]/*' />
        /// <devdoc>
        ///     Removes all annually bolded days.  Be sure to call updateBoldedDates() afterwards.
        /// </devdoc>
        public void RemoveAllAnnuallyBoldedDates() {
            this.annualArrayOfDates.Clear();
            for (int i=0; i<MONTHS_IN_YEAR; ++i)
                monthsOfYear[i] = 0;
        }


        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.RemoveAllBoldedDates"]/*' />
        /// <devdoc>
        ///     Removes all the bolded days.  Be sure to call updateBoldedDates() afterwards.
        /// </devdoc>
        public void RemoveAllBoldedDates() {
            this.arrayOfDates.Clear();
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.RemoveAllMonthlyBoldedDates"]/*' />
        /// <devdoc>
        ///     Removes all monthly bolded days.  Be sure to call updateBoldedDates() afterwards.
        /// </devdoc>
        public void RemoveAllMonthlyBoldedDates() {
            this.monthlyArrayOfDates.Clear();
            datesToBoldMonthly = 0;
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.RemoveAnnuallyBoldedDate"]/*' />
        /// <devdoc>
        ///     Removes an annually bolded date.  If the date is not found in the
        ///     bolded date list, then no action is taken.  If date occurs more than
        ///     once in the bolded date list, then only the first date is removed.  When
        ///     comparing dates, only the day and month are used. Be sure to call
        ///     updateBoldedDates afterwards.
        /// </devdoc>
        public void RemoveAnnuallyBoldedDate(DateTime date) {
            int length = annualArrayOfDates.Count;
            int i=0;
            for (; i<length; ++i) {
                if (CompareDayAndMonth((DateTime) annualArrayOfDates[i], date)) {
                    annualArrayOfDates.RemoveAt(i);
                    break;
                }
            }
            --length;
            for (int j=i; j<length; ++j) {
                if (CompareDayAndMonth((DateTime) annualArrayOfDates[j], date)) {
                    return;
                }
            }
            monthsOfYear[date.Month-1] &= ~(0x00000001<<(date.Day-1));
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.RemoveBoldedDate"]/*' />
        /// <devdoc>
        ///     Removes a bolded date.  If the date is not found in the
        ///     bolded date list, then no action is taken.  If date occurs more than
        ///     once in the bolded date list, then only the first date is removed.
        ///     Be sure to call updateBoldedDates() afterwards.
        /// </devdoc>
        public void RemoveBoldedDate(DateTime date) {
            int length = arrayOfDates.Count;
            for (int i=0; i<length; ++i) {
                if (DateTime.Compare( ((DateTime)arrayOfDates[i]).Date, date.Date) == 0) {
                    arrayOfDates.RemoveAt(i);
                    Invalidate();
                    return;
                }
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.RemoveMonthlyBoldedDate"]/*' />
        /// <devdoc>
        ///     Removes a monthly bolded date.  If the date is not found in the
        ///     bolded date list, then no action is taken.  If date occurs more than
        ///     once in the bolded date list, then only the first date is removed.  When
        ///     comparing dates, only the day and month are used.  Be sure to call
        ///     updateBoldedDates afterwards.
        /// </devdoc>
        public void RemoveMonthlyBoldedDate(DateTime date) {
            int length = monthlyArrayOfDates.Count;
            int i=0;
            for (; i<length; ++i) {
                if (CompareDayAndMonth((DateTime) monthlyArrayOfDates[i], date)) {
                    monthlyArrayOfDates.RemoveAt(i);
                    break;
                }
            }
            --length;
            for (int j=i; j<length; ++j) {
                if (CompareDayAndMonth((DateTime) monthlyArrayOfDates[j], date)) {
                    return;
                }
            }
            datesToBoldMonthly &= ~(0x00000001<<(date.Day-1));
        }

        private void ResetAnnuallyBoldedDates() {
            annualArrayOfDates.Clear();
        }

        private void ResetBoldedDates() {
            arrayOfDates.Clear();
        }

        private void ResetCalendarDimensions() {
            CalendarDimensions = new Size(1,1);
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.ResetMaxDate"]/*' />
        /// <devdoc>
        ///     Resets the maximum selectable date.  By default value, there is no
        ///     upper limit.
        /// </devdoc>
        private void ResetMaxDate() {
            MaxDate = DateTime.MaxValue;
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.ResetMinDate"]/*' />
        /// <devdoc>
        ///     Resets the minimum selectable date.  By default value, there is no
        ///     lower limit.
        /// </devdoc>
        private void ResetMinDate() {
            MinDate = DateTime.MinValue;
        }


        private void ResetMonthlyBoldedDates() {
            monthlyArrayOfDates.Clear();
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.ResetSelectionRange"]/*' />
        /// <devdoc>
        ///     Resets the limits of the selection range.  By default value, the upper
        ///     and lower limit is the current date.
        /// </devdoc>
        private void ResetSelectionRange() {
            SetSelectionRange(Now, Now);
        }

        private void ResetTrailingForeColor() {
            TrailingForeColor = DEFAULT_TRAILING_FORE_COLOR;
        }

        private void ResetTitleForeColor() {
            TitleForeColor = DEFAULT_TITLE_FORE_COLOR;
        }

        private void ResetTitleBackColor() {
            TitleBackColor = DEFAULT_TITLE_BACK_COLOR;
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.ResetTodayDate"]/*' />
        /// <devdoc>
        ///     Resets the "today"'s date.  By default value, "today" is the
        ///     current date (and is automatically updated when the clock crosses
        ///     over to the next day).
        ///     If you set the today date yourself (using the TodayDate property)
        ///     the control will no longer automatically update the current day
        ///     for you. To re-enable this behavior, ResetTodayDate() is used.
        /// </devdoc>
        private void ResetTodayDate() {
            todayDateSet = false;
            UpdateTodayDate();
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.RequestBuffer"]/*' />
        /// <devdoc>
        ///     reqSize = # elements in int[] array
        ///
        ///     The size argument should be greater than 0.
        ///     Because of the nature of MonthCalendar, we can expect that
        ///     the requested size will not be ridiculously large, hence
        ///     it is not necessary to decrease the size of an allocated
        ///     block if the new requested size is smaller.
        /// </devdoc>
        /// <internalonly/>
        private IntPtr RequestBuffer(int reqSize) {
            Debug.Assert(reqSize > 0, "Requesting a ridiculously small buffer");
            int intSize = 4;
            // if the current buffer size is insufficient...
            if (reqSize * intSize > mdsBufferSize) {
                // free and expand the buffer,
                if (mdsBuffer != IntPtr.Zero) {
                    Marshal.FreeHGlobal(mdsBuffer);
                    mdsBuffer = IntPtr.Zero;
                }

                // Round up to the nearest multiple of MINIMUM_ALLOC_SIZE
                float quotient = (float) (reqSize-1) / MINIMUM_ALLOC_SIZE;
                int actualSize = ((int) (quotient+1)) * MINIMUM_ALLOC_SIZE;
                Debug.Assert(actualSize >= reqSize, "Tried to round up, but got it wrong");

                mdsBufferSize = actualSize * intSize;
                mdsBuffer = Marshal.AllocHGlobal(mdsBufferSize);
                return mdsBuffer;
            }
            return mdsBuffer;
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.SetBoundsCore"]/*' />
        /// <devdoc>
        ///     Overrides Control.SetBoundsCore to enforce auto-sizing.
        /// </devdoc>
        /// <internalonly/>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
            Rectangle oldBounds = Bounds;
            Size max = SystemInformation.MaxWindowTrackSize;

            // Second argument to GetPreferredWidth and GetPreferredHeight is a boolean specifying if we should update the number of rows/columns.
            // We only want to update the number of rows/columns if we are not currently being scaled.
            bool updateRowsAndColumns = !DpiHelper.IsScalingRequirementMet || !IsCurrentlyBeingScaled;

            if (width != oldBounds.Width) {
                if (width > max.Width)
                    width = max.Width;
                width = GetPreferredWidth(width, updateRowsAndColumns);
            }
            if (height != oldBounds.Height) {
                if (height > max.Height)
                    height = max.Height;
                height = GetPreferredHeight(height, updateRowsAndColumns);
            }
            base.SetBoundsCore(x, y, width, height, specified);
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.SetControlColor"]/*' />
        /// <devdoc>
        ///     If the handle has been created, this applies the color to the control
        /// </devdoc>
        /// <internalonly/>
        private void SetControlColor(int colorIndex, Color value) {
            if (IsHandleCreated) {
                SendMessage(NativeMethods.MCM_SETCOLOR, colorIndex, ColorTranslator.ToWin32(value));
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.SetRange"]/*' />
        /// <devdoc>
        ///     Updates the window handle with the min/max ranges if it has been
        ///     created.
        /// </devdoc>
        /// <internalonly/>
        private void SetRange() {
            SetRange(DateTimePicker.EffectiveMinDate(minDate), DateTimePicker.EffectiveMaxDate(maxDate));
        }

        private void SetRange(DateTime minDate, DateTime maxDate) {
            // Keep selection range within passed in minDate and maxDate
            if (selectionStart < minDate) {
                selectionStart = minDate;
            }
            if (selectionStart > maxDate) {
                selectionStart = maxDate;
            }
            if (selectionEnd < minDate) {
                selectionEnd = minDate;
            }
            if (selectionEnd > maxDate) {
                selectionEnd = maxDate;
            }
            SetSelRange(selectionStart, selectionEnd);

            // Updated the calendar range
            //
            if (IsHandleCreated) {
                int flag = 0;

                NativeMethods.SYSTEMTIMEARRAY sa = new NativeMethods.SYSTEMTIMEARRAY();
                flag |= NativeMethods.GDTR_MIN | NativeMethods.GDTR_MAX;
                NativeMethods.SYSTEMTIME sys = DateTimePicker.DateTimeToSysTime(minDate);
                sa.wYear1 = sys.wYear;
                sa.wMonth1 = sys.wMonth;
                sa.wDayOfWeek1 = sys.wDayOfWeek;
                sa.wDay1 = sys.wDay;
                sys = DateTimePicker.DateTimeToSysTime(maxDate);
                sa.wYear2 = sys.wYear;
                sa.wMonth2 = sys.wMonth;
                sa.wDayOfWeek2 = sys.wDayOfWeek;
                sa.wDay2 = sys.wDay;

                if ((int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.MCM_SETRANGE, flag, sa) == 0)
                    throw new InvalidOperationException(string.Format(SR.MonthCalendarRange, minDate.ToShortDateString(), maxDate.ToShortDateString()));
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.SetCalendarDimensions"]/*' />
        /// <devdoc>
        ///     Sets the number of columns and rows to display.
        /// </devdoc>
        public void SetCalendarDimensions(int x, int y) {
            if (x < 1) {
                throw new ArgumentOutOfRangeException(nameof(x), string.Format(SR.MonthCalendarInvalidDimensions, (x).ToString("D", CultureInfo.CurrentCulture), (y).ToString("D", CultureInfo.CurrentCulture)));
            }
            if (y < 1) {
                throw new ArgumentOutOfRangeException(nameof(y), string.Format(SR.MonthCalendarInvalidDimensions, (x).ToString("D", CultureInfo.CurrentCulture), (y).ToString("D", CultureInfo.CurrentCulture)));
            }

            // MonthCalendar limits the dimensions to x * y <= 12
            // i.e. a maximum of twelve months can be displayed at a time
            // The following code emulates what is done inside monthcalendar (in comctl32.dll):
            // The dimensions are gradually reduced until the inequality above holds.
            //
            while (x * y > 12) {
                if (x > y) {
                    x--;
                }
                else {
                    y--;
                }
            }

            if (dimensions.Width != x || dimensions.Height != y) {
                this.dimensions.Width = x;
                this.dimensions.Height = y;
                AdjustSize();
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.SetDate"]/*' />
        /// <devdoc>
        ///     Sets date as the current selected date.  The start and begin of
        ///     the selection range will both be equal to date.
        /// </devdoc>
        public void SetDate(DateTime date) {

            if (date.Ticks < minDate.Ticks) {
                throw new ArgumentOutOfRangeException(nameof(date), string.Format(SR.InvalidLowBoundArgumentEx, "date", FormatDate(date), "MinDate"));
            }
            if (date.Ticks > maxDate.Ticks) {
                throw new ArgumentOutOfRangeException(nameof(date), string.Format(SR.InvalidHighBoundArgumentEx, "date", FormatDate(date), "MaxDate"));
            }

            SetSelectionRange(date, date);
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.SetSelectionRange"]/*' />
        /// <devdoc>
        ///     Sets the selection for a month calendar control to a given date range.
        ///     The selection range will not be set if the selection range exceeds the
        ///     maximum selection count.
        /// </devdoc>
        public void SetSelectionRange(DateTime date1, DateTime date2) {

            // Keep the dates within the min and max dates
            if (date1.Ticks < minDate.Ticks) {
                throw new ArgumentOutOfRangeException(nameof(date1), string.Format(SR.InvalidLowBoundArgumentEx, "SelectionStart", FormatDate(date1), "MinDate"));
            }
            if (date1.Ticks > maxDate.Ticks) {
                throw new ArgumentOutOfRangeException(nameof(date1), string.Format(SR.InvalidHighBoundArgumentEx, "SelectionEnd", FormatDate(date1), "MaxDate"));
            }
            if (date2.Ticks < minDate.Ticks) {
                throw new ArgumentOutOfRangeException(nameof(date2), string.Format(SR.InvalidLowBoundArgumentEx, "SelectionStart", FormatDate(date2), "MinDate"));
            }
            if (date2.Ticks > maxDate.Ticks) {
                throw new ArgumentOutOfRangeException(nameof(date2), string.Format(SR.InvalidHighBoundArgumentEx, "SelectionEnd", FormatDate(date2), "MaxDate"));
            }

            // If date1 > date2, we just select date2 (compat)
            //
            if (date1 > date2) {
                date2 = date1;
            }

            // If the range exceeds maxSelectionCount, compare with the previous range and adjust whichever
            // limit hasn't changed.
            //
            if ((date2 - date1).Days >= maxSelectionCount) {

                if (date1.Ticks == selectionStart.Ticks) {
                    // Bring start date forward
                    //
                    date1 = date2.AddDays(1 - maxSelectionCount);
                }
                else {
                    // Bring end date back
                    //
                    date2 = date1.AddDays(maxSelectionCount - 1);
                }
            }

            // Set the range
            SetSelRange(date1, date2);
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.SetSelRange"]/*' />
        /// <devdoc>
        ///     Upper must be greater than Lower
        /// </devdoc>
        /// <internalonly/>
        private void SetSelRange(DateTime lower, DateTime upper) {

            Debug.Assert(lower.Ticks <= upper.Ticks, "lower must be less than upper");

            bool changed = false;
            if (selectionStart != lower || selectionEnd != upper) {
                changed = true;
                selectionStart = lower;
                selectionEnd = upper;
            }

            // always set the value on the control, to ensure that
            // it is up to date.
            //
            if (IsHandleCreated) {
                NativeMethods.SYSTEMTIMEARRAY sa = new NativeMethods.SYSTEMTIMEARRAY();

                NativeMethods.SYSTEMTIME sys = DateTimePicker.DateTimeToSysTime(lower);
                sa.wYear1 = sys.wYear;
                sa.wMonth1 = sys.wMonth;
                sa.wDayOfWeek1 = sys.wDayOfWeek;
                sa.wDay1 = sys.wDay;
                sys = DateTimePicker.DateTimeToSysTime(upper);
                sa.wYear2 = sys.wYear;
                sa.wMonth2 = sys.wMonth;
                sa.wDayOfWeek2 = sys.wDayOfWeek;
                sa.wDay2 = sys.wDay;
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.MCM_SETSELRANGE , 0, sa);
            }

            if (changed) {
                OnDateChanged(new DateRangeEventArgs(lower, upper));
            }
        }

        private bool ShouldSerializeAnnuallyBoldedDates() {
            return annualArrayOfDates.Count > 0;
        }

        private bool ShouldSerializeBoldedDates() {
            return arrayOfDates.Count > 0;
        }

        private bool ShouldSerializeCalendarDimensions() {
            return !dimensions.Equals(new Size(1, 1));
        }

        private bool ShouldSerializeTrailingForeColor() {
            return !TrailingForeColor.Equals(DEFAULT_TRAILING_FORE_COLOR);
        }

        private bool ShouldSerializeTitleForeColor() {
            return !TitleForeColor.Equals(DEFAULT_TITLE_FORE_COLOR);
        }

        private bool ShouldSerializeTitleBackColor() {
            return !TitleBackColor.Equals(DEFAULT_TITLE_BACK_COLOR);
        }

        private bool ShouldSerializeMonthlyBoldedDates() {
            return monthlyArrayOfDates.Count > 0;
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.ShouldSerializeMaxDate"]/*' />
        /// <devdoc>
        ///     Retrieves true if the maxDate should be persisted in code gen.
        /// </devdoc>
        private bool ShouldSerializeMaxDate() {
            return maxDate != DateTimePicker.MaximumDateTime && maxDate != DateTime.MaxValue;
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.ShouldSerializeMinDate"]/*' />
        /// <devdoc>
        ///     Retrieves true if the minDate should be persisted in code gen.
        /// </devdoc>
        private bool ShouldSerializeMinDate() {
            return minDate != DateTimePicker.MinimumDateTime && minDate != DateTime.MinValue;
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.ShouldSerializeSelectionRange"]/*' />
        /// <devdoc>
        ///     Retrieves true if the selectionRange should be persisted in code gen.
        /// </devdoc>
        private bool ShouldSerializeSelectionRange() {
            return !DateTime.Equals(selectionEnd, selectionStart);
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.ShouldSerializeTodayDate"]/*' />
        /// <devdoc>
        ///     Retrieves true if the todayDate should be persisted in code gen.
        /// </devdoc>
        private bool ShouldSerializeTodayDate() {
            return todayDateSet;
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.ToString"]/*' />
        /// <devdoc>
        ///     Returns a string representation for this control.
        /// </devdoc>
        /// <internalonly/>
        public override string ToString() {

            string s = base.ToString();
            return s + ", " + SelectionRange.ToString();
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.UpdateBoldedDates"]/*' />
        /// <devdoc>
        ///     Forces month calendar to display the current set of bolded dates.
        /// </devdoc>
        public void UpdateBoldedDates() {
            RecreateHandle();
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.UpdateTodayDate"]/*' />
        /// <devdoc>
        ///     Updates the current setting for "TODAY" in the MonthCalendar control
        ///     If the today date is set, the control will be set to that. Otherwise,
        ///     it will be set to null (running clock mode - the today date will be
        ///     automatically updated).
        /// </devdoc>
        private void UpdateTodayDate() {
            if (IsHandleCreated) {
                NativeMethods.SYSTEMTIME st = null;
                if (todayDateSet) {
                    st = DateTimePicker.DateTimeToSysTime(todayDate);
                }
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.MCM_SETTODAY, 0, st);
            }
        }

        private void MarshaledUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs pref) {
            try {
                //use begininvoke instead of invoke in case the destination thread is not processing messages.
                BeginInvoke(new UserPreferenceChangedEventHandler(this.UserPreferenceChanged), new object[] { sender, pref });
            }
            catch (InvalidOperationException) { } //if the destination thread does not exist, don't send.
        }

        private void UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs pref) {
            if (pref.Category == UserPreferenceCategory.Locale) {
               // We need to recreate the monthcalendar handle when the locale changes, because
                // the day names etc. are only updated on a handle recreate (comctl32 limitation).
                //
                RecreateHandle();
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.WmDateChanged"]/*' />
        /// <devdoc>
        ///     Handles the MCN_SELCHANGE notification
        /// </devdoc>
        /// <internalonly/>
        private void WmDateChanged(ref Message m) {
            NativeMethods.NMSELCHANGE nmmcsc = (NativeMethods.NMSELCHANGE)m.GetLParam(typeof(NativeMethods.NMSELCHANGE));
            DateTime start = selectionStart = DateTimePicker.SysTimeToDateTime(nmmcsc.stSelStart);
            DateTime end = selectionEnd = DateTimePicker.SysTimeToDateTime(nmmcsc.stSelEnd);

            if (AccessibilityImprovements.Level1) {
                AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
                AccessibilityNotifyClients(AccessibleEvents.ValueChange, -1);
            }
            
            //subhag
            if (start.Ticks < minDate.Ticks || end.Ticks < minDate.Ticks)
                SetSelRange(minDate,minDate);
            else if (start.Ticks > maxDate.Ticks || end.Ticks > maxDate.Ticks)
                SetSelRange(maxDate,maxDate);
            //end subhag
            OnDateChanged(new DateRangeEventArgs(start, end));
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.WmDateBold"]/*' />
        /// <devdoc>
        ///     Handles the MCN_GETDAYSTATE notification
        /// </devdoc>
        /// <internalonly/>
        private void WmDateBold(ref Message m) {
            NativeMethods.NMDAYSTATE nmmcds = (NativeMethods.NMDAYSTATE)m.GetLParam(typeof(NativeMethods.NMDAYSTATE));
            DateTime start = DateTimePicker.SysTimeToDateTime(nmmcds.stStart);
            DateBoldEventArgs boldEvent = new DateBoldEventArgs(start, nmmcds.cDayState);
            BoldDates(boldEvent);
            mdsBuffer = RequestBuffer(boldEvent.Size);
            // copy boldEvent into mdsBuffer
            Marshal.Copy(boldEvent.DaysToBold, 0, mdsBuffer, boldEvent.Size);
            // now we replug DateBoldEventArgs info into NMDAYSTATE
            nmmcds.prgDayState = mdsBuffer;
            Marshal.StructureToPtr(nmmcds, m.LParam, false);
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.WmCalViewChanged"]/*' />
        /// <devdoc>
        ///     Handles the MCN_VIEWCHANGE  notification
        /// </devdoc>
        /// <internalonly/>

        private void WmCalViewChanged (ref Message m) {
            NativeMethods.NMVIEWCHANGE nmmcvm = (NativeMethods.NMVIEWCHANGE)m.GetLParam(typeof(NativeMethods.NMVIEWCHANGE));
            Debug.Assert(mcCurView == (NativeMethods.MONTCALENDAR_VIEW_MODE)nmmcvm.uOldView, "Calendar view mode is out of sync with native control");
            if (mcCurView != (NativeMethods.MONTCALENDAR_VIEW_MODE)nmmcvm.uNewView) {
                mcOldView = mcCurView;
                mcCurView = (NativeMethods.MONTCALENDAR_VIEW_MODE)nmmcvm.uNewView;
                if (AccessibilityImprovements.Level1) {
                    AccessibilityNotifyClients(AccessibleEvents.ValueChange, -1);
                    AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
                }
            }
        }
        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.WmDateSelected"]/*' />
        /// <devdoc>
        ///     Handles the MCN_SELECT notification
        /// </devdoc>
        /// <internalonly/>
        private void WmDateSelected(ref Message m) {
            NativeMethods.NMSELCHANGE nmmcsc = (NativeMethods.NMSELCHANGE)m.GetLParam(typeof(NativeMethods.NMSELCHANGE));
            DateTime start = selectionStart = DateTimePicker.SysTimeToDateTime(nmmcsc.stSelStart);
            DateTime end = selectionEnd = DateTimePicker.SysTimeToDateTime(nmmcsc.stSelEnd);

            if (AccessibilityImprovements.Level1) {
                AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
                AccessibilityNotifyClients(AccessibleEvents.ValueChange, -1);
            }

            //subhag
            if (start.Ticks < minDate.Ticks || end.Ticks < minDate.Ticks)
                SetSelRange(minDate,minDate);
            else if (start.Ticks > maxDate.Ticks || end.Ticks > maxDate.Ticks)
                SetSelRange(maxDate,maxDate);

            //end subhag
            OnDateSelected(new DateRangeEventArgs(start, end));

        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.WmGetDlgCode"]/*' />
        /// <devdoc>
        ///     Handles the WM_GETDLGCODE message
        /// </devdoc>
        /// <internalonly/>
        private void WmGetDlgCode(ref Message m) {
            // The MonthCalendar does its own handling of arrow keys
            m.Result = (IntPtr)NativeMethods.DLGC_WANTARROWS;
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.WmReflectCommand"]/*' />
        /// <devdoc>
        ///     Handles the WM_COMMAND messages reflected from the parent control.
        /// </devdoc>
        /// <internalonly/>
        private void WmReflectCommand(ref Message m) {
            if (m.HWnd == Handle) {
                NativeMethods.NMHDR nmhdr = (NativeMethods.NMHDR)m.GetLParam(typeof(NativeMethods.NMHDR));
                switch (nmhdr.code) {
                    case NativeMethods.MCN_SELECT:
                        WmDateSelected(ref m);
                        break;
                    case NativeMethods.MCN_SELCHANGE:
                        WmDateChanged(ref m);
                        break;
                    case NativeMethods.MCN_GETDAYSTATE:
                        WmDateBold(ref m);
                        break;
                    case NativeMethods.MCN_VIEWCHANGE:
                        if (AccessibilityImprovements.Level1) {
                            WmCalViewChanged(ref m);
                        }
                        break;
                }
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.WndProc"]/*' />
        /// <devdoc>
        ///     Overrided wndProc
        /// </devdoc>
        /// <internalonly/>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_LBUTTONDOWN:
                    FocusInternal();
                    if (!ValidationCancelled) {
                        base.WndProc(ref m);
                    }
                    break;
                case NativeMethods.WM_GETDLGCODE:
                    WmGetDlgCode(ref m);
                    break;
                case NativeMethods.WM_REFLECT + NativeMethods.WM_NOTIFY:
                    WmReflectCommand(ref m);
                    base.WndProc(ref m);
                    break;
                case NativeMethods.WM_DESTROY:
                    if (restrictUnmanagedCode == true && nativeWndProcCount > 0) {
                        throw new InvalidOperationException();
                    }
                    base.WndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.DefWndProc"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    Calls the default window procedure for the MonthCalendar control. 
        /// </devdoc>
        [
            SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        protected override void DefWndProc(ref Message m) {
            if (restrictUnmanagedCode == true) {
                nativeWndProcCount++;
                try {
                    base.DefWndProc(ref m);
                }
                finally {
                    nativeWndProcCount--;
                }

                return;
            }

            base.DefWndProc(ref m);
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.HitTestInfo"]/*' />
        /// <devdoc>
        ///     HitTestInfo objects are returned by MonthCalendar in response to the hitTest method.
        ///     HitTestInfo is for informational purposes only; the user should not construct these objects, and
        ///     cannot modify any of the members.
        /// </devdoc>
        public sealed class HitTestInfo {
            readonly Point       point;
            readonly HitArea     hitArea;
            readonly DateTime    time;

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.HitTestInfo.HitTestInfo"]/*' />
            /// <devdoc>
            /// </devdoc>
            /// <internalonly/>
            internal HitTestInfo(Point pt, HitArea area, DateTime time) {
                this.point = pt;
                this.hitArea = area;
                this.time = time;
            }

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.HitTestInfo.HitTestInfo1"]/*' />
            /// <devdoc>
            ///      This constructor is used when the DateTime member is invalid.
            /// </devdoc>
            /// <internalonly/>
            internal HitTestInfo(Point pt, HitArea area) {
                this.point = pt;
                this.hitArea = area;
            }

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.HitTestInfo.Point"]/*' />
            /// <devdoc>
            ///     The point that was hit-tested
            /// </devdoc>
            public Point Point {
                get { return point; }
            }

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.HitTestInfo.HitArea"]/*' />
            /// <devdoc>
            ///     Output member that receives an enumeration value from System.Windows.Forms.MonthCalendar.HitArea
            ///     representing the result of the hit-test operation.
            /// </devdoc>
            public HitArea HitArea {
                get { return hitArea; }
            }

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.HitTestInfo.Time"]/*' />
            /// <devdoc>
            ///     The time information specific to the location that was hit-tested.  This value
            ///     will only be valid at certain values of hitArea.
            /// </devdoc>
            public DateTime Time {
                get { return time; }
            }

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.HitTestInfo.HitAreaHasValidDateTime"]/*' />
            /// <devdoc>
            ///      Determines whether a given HitArea should have a corresponding valid DateTime
            /// </devdoc>
            /// <internalonly/>
            internal static bool HitAreaHasValidDateTime(HitArea hitArea) {
                switch (hitArea) {
                    case HitArea.Date:
                        //case HitArea.DayOfWeek:   comCtl does not provide a valid date
                    case HitArea.WeekNumbers:
                        return true;
                }
                return false;
            }
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="HitArea"]/*' />
        /// <devdoc>
        /// This enumeration has specific areas of the MonthCalendar control as its enumerated values.
        /// The hitArea member of System.Windows.Forms.Win32.HitTestInfo will be one of these enumerated values, and
        /// indicates which portion of a month calendar is under a specific point.
        /// </devdoc>
        public enum HitArea {
            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="HitArea.Nowhere"]/*' />
            /// <devdoc>
            /// The given point was not on the month calendar control, or it was in an inactive portion of the control.
            /// </devdoc>
            Nowhere = 0,

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="HitArea.TitleBackground"]/*' />
            /// <devdoc>
            /// The given point was over the background of a month's title
            /// </devdoc>
            TitleBackground = 1,

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="HitArea.TitleMonth"]/*' />
            /// <devdoc>
            /// The given point was in a month's title bar, over a month name
            /// </devdoc>
            TitleMonth = 2,

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="HitArea.TitleYear"]/*' />
            /// <devdoc>
            /// The given point was in a month's title bar, over the year value
            /// </devdoc>
            TitleYear = 3,

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="HitArea.NextMonthButton"]/*' />
            /// <devdoc>
            /// The given point was over the button at the top right corner of the control.
            /// If the user clicks here, the month calendar will scroll its display to the next
            /// month or set of months
            /// </devdoc>
            NextMonthButton = 4,

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="HitArea.PrevMonthButton"]/*' />
            /// <devdoc>
            /// The given point was over the button at the top left corner of the control. If the
            /// user clicks here, the month calendar will scroll its display to the previous month
            /// or set of months
            /// </devdoc>
            PrevMonthButton = 5,

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="HitArea.CalendarBackground"]/*' />
            /// <devdoc>
            /// The given point was in the calendar's background
            /// </devdoc>
            CalendarBackground = 6,

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="HitArea.Date"]/*' />
            /// <devdoc>
            /// The given point was on a particular date within the calendar, and the time member of
            /// HitTestInfo will be set to the date at the given point.
            /// </devdoc>
            Date = 7,

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="HitArea.NextMonthDate"]/*' />
            /// <devdoc>
            /// The given point was over a date from the next month (partially displayed at the end of
            /// the currently displayed month). If the user clicks here, the month calendar will scroll
            /// its display to the next month or set of months.
            /// </devdoc>
            NextMonthDate = 8,

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="HitArea.PrevMonthDate"]/*' />
            /// <devdoc>
            /// The given point was over a date from the previous month (partially displayed at the end
            /// of the currently displayed month). If the user clicks here, the month calendar will scroll
            /// its display to the previous month or set of months.
            /// </devdoc>
            PrevMonthDate = 9,

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="HitArea.DayOfWeek"]/*' />
            /// <devdoc>
            /// The given point was over a day abbreviation ("Fri", for example). The time member
            /// of HitTestInfo will be set to the corresponding date on the top row.
            /// </devdoc>
            DayOfWeek = 10,

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="HitArea.WeekNumbers"]/*' />
            /// <devdoc>
            /// The given point was over a week number.  This will only occur if the showWeekNumbers
            /// property of MonthCalendar is enabled.  The time member of HitTestInfo will be set to
            /// the corresponding date in the leftmost column.
            /// </devdoc>
            WeekNumbers = 11,

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="HitArea.TodayLink"]/*' />
            /// <devdoc>
            /// The given point was on the "today" link at the bottom of the month calendar control
            /// </devdoc>
            TodayLink = 12,
        }

        /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.MonthCalendarAccessibleObject"]/*' />
        /// <internalonly/>        
        /// <devdoc>
        /// </devdoc>
        [System.Runtime.InteropServices.ComVisible(true)]
        internal class MonthCalendarAccessibleObject : ControlAccessibleObject {

            private MonthCalendar calendar;

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.CheckBoxAccessibleObject.MonthCalendarAccessibleObject"]/*' />
            public MonthCalendarAccessibleObject(Control owner)
                : base(owner) {
                    calendar = owner as MonthCalendar;
            }

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.MonthCalendarAccessibleObject.Role"]/*' />
            public override AccessibleRole Role {
                get {
                    if (calendar != null) {
                        AccessibleRole role = calendar.AccessibleRole;
                        if (role != AccessibleRole.Default) {
                            return role;
                        }
                    }
                    return AccessibleRole.Table;
                }
            }

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.MonthCalendarAccessibleObject.Help"]/*' />
            public override string Help {
                get {
                    var help = base.Help;
                    if (help != null) {
                        return help;
                    }
                    else {
                        if (calendar != null) {
                            return calendar.GetType().Name + "(" + calendar.GetType().BaseType.Name + ")";
                        }
                    }
                    return string.Empty;
                }
            }

            /// <include file='doc\MonthCalendar.uex' path='docs/doc[@for="MonthCalendar.MonthCalendarAccessibleObject.Name"]/*' />
            public override string Name {
                get {
                    string name = base.Name;
                    if (name != null) {
                        return name;
                    }

                    if (calendar != null) {

                        if (calendar.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_MONTH) {
                            if (System.DateTime.Equals(calendar.SelectionStart.Date, calendar.SelectionEnd.Date)) {
                                name = string.Format(SR.MonthCalendarSingleDateSelected, calendar.SelectionStart.ToLongDateString());
                            }
                            else {
                                name = string.Format(SR.MonthCalendarRangeSelected, calendar.SelectionStart.ToLongDateString(), calendar.SelectionEnd.ToLongDateString());
                            }
                        }
                        else if (this.calendar.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_YEAR) {
                            if (System.DateTime.Equals(this.calendar.SelectionStart.Month, this.calendar.SelectionEnd.Month)) {
                                name = string.Format(SR.MonthCalendarSingleDateSelected, calendar.SelectionStart.ToString("y"));
                            }
                            else {
                                name = string.Format(SR.MonthCalendarRangeSelected, calendar.SelectionStart.ToString("y"), calendar.SelectionEnd.ToString("y"));
                            }
                        }
                        else if (calendar.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_DECADE) {
                            if (System.DateTime.Equals(calendar.SelectionStart.Year, calendar.SelectionEnd.Year)) {
                                name = string.Format(SR.MonthCalendarSingleYearSelected, calendar.SelectionStart.ToString("yyyy"));
                            }
                            else {
                                name = string.Format(SR.MonthCalendarYearRangeSelected, calendar.SelectionStart.ToString("yyyy"), calendar.SelectionEnd.ToString("yyyy"));
                            }
                        }
                        else if (calendar.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_CENTURY) {
                            name = string.Format(SR.MonthCalendarSingleDecadeSelected, calendar.SelectionStart.ToString("yyyy"));
                        }
                    }
                    return name;
                }
            }

            public override string Value {
                get {
                    var value = string.Empty;
                    try {
                        if (calendar != null) {
                            if (calendar.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_MONTH) {
                                if (System.DateTime.Equals(calendar.SelectionStart.Date, calendar.SelectionEnd.Date)) {
                                    value = calendar.SelectionStart.ToLongDateString();
                                }
                                else {
                                    value = string.Format("{0} - {1}", calendar.SelectionStart.ToLongDateString(), calendar.SelectionEnd.ToLongDateString());
                                }
                            }
                            else if (this.calendar.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_YEAR) {
                                if (System.DateTime.Equals(this.calendar.SelectionStart.Month, this.calendar.SelectionEnd.Month)) {
                                    value = calendar.SelectionStart.ToString("y");
                                }
                                else {
                                    value = string.Format("{0} - {1}", calendar.SelectionStart.ToString("y"), calendar.SelectionEnd.ToString("y"));
                                }
                            }
                            else {
                                value = string.Format("{0} - {1}", calendar.SelectionRange.Start.ToString(), calendar.SelectionRange.End.ToString());
                            }
                        }
                    }
                    catch {
                        value = base.Value;
                    }
                    return value;
                }
                set {
                    base.Value = value;
                }
            }                      
        }

    } // end class MonthCalendar
}

