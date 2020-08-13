// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Globalization;

namespace System.Windows.Forms.Design
{
    internal partial class FormatControl : UserControl
    {
        private const int NoFormattingIndex = 0;
        private const int NumericIndex = 1;
        private const int CurrencyIndex = 2;
        private const int DateTimeIndex = 3;
        private const int ScientificIndex = 4;
        private const int CustomIndex = 5;

        private TextBox customStringTextBox = new TextBox();

        // static because we want this value to be the same across a
        // VS session
        private static DateTime s_dateTimeFormatValue = DateTime.Now;
        private bool _loaded;

        public FormatControl()
        {
            InitializeComponent();
        }

        public bool Dirty { get; set; }

        public string FormatType
        {
            get
            {
                FormatTypeClass formatType = formatTypeListBox.SelectedItem as FormatTypeClass;

                if (formatType != null)
                {
                    return formatType.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                formatTypeListBox.SelectedIndex = 0;

                for (int i = 0; i < formatTypeListBox.Items.Count; i++)
                {
                    FormatTypeClass formatType = formatTypeListBox.Items[i] as FormatTypeClass;

                    if (formatType.ToString().Equals(value))
                    {
                        formatTypeListBox.SelectedIndex = i;
                    }
                }
            }
        }

        public FormatTypeClass FormatTypeItem
        {
            get
            {
                return formatTypeListBox.SelectedItem as FormatTypeClass;
            }
        }

        public string NullValue
        {
            get
            {
                // If text box is empty or contains just whitespace, return 'null' for the NullValue.
                // Otherwise we end up pushing empty string as the NullValue for every binding, which breaks non-string
                // bindings. This does mean that setting the NullValue to empty string now becomes a code-only scenario
                // for users, but that is an acceptable trade-off.
                string nullValue = nullValueTextBox.Text.Trim();

                return (nullValue.Length == 0) ? null : nullValue;
            }
            set
            {
                nullValueTextBox.TextChanged -= new System.EventHandler(nullValueTextBox_TextChanged);
                nullValueTextBox.Text = value;
                nullValueTextBox.TextChanged += new System.EventHandler(nullValueTextBox_TextChanged);
            }
        }

        public bool NullValueTextBoxEnabled
        {
            set
            {
                nullValueTextBox.Enabled = value;
            }
        }

        void customStringTextBox_TextChanged(object sender, EventArgs e)
        {
            CustomFormatType customFormatType = formatTypeListBox.SelectedItem as CustomFormatType;
            sampleLabel.Text = customFormatType.SampleString;
            Dirty = true;
        }

        private void dateTimeFormatsListBox_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // recompute the SampleLabel
            FormatTypeClass item = formatTypeListBox.SelectedItem as FormatTypeClass;
            sampleLabel.Text = item.SampleString;
            Dirty = true;
        }

        private void decimalPlacesUpDown_ValueChanged(object sender, EventArgs e)
        {
            // update the sample label
            FormatTypeClass item = formatTypeListBox.SelectedItem as FormatTypeClass;
            sampleLabel.Text = item.SampleString;
            Dirty = true;
        }

        private void formatGroupBox_Enter(object sender, EventArgs e)
        {
        }

        private void formatTypeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FormatTypeClass formatType = formatTypeListBox.SelectedItem as FormatTypeClass;
            UpdateControlVisibility(formatType);
            sampleLabel.Text = formatType.SampleString;
            explanationLabel.Text = formatType.TopLabelString;
            Dirty = true;
        }

        // given a formatString/formatInfo combination, this method suggest the most appropriate user control
        // the result of the function is one of the strings "Numeric", "Currency", "DateTime", "Percentage", "Scientific", "Custom"
        public static string FormatTypeStringFromFormatString(string formatString)
        {
            if (string.IsNullOrEmpty(formatString))
            {
                return SR.BindingFormattingDialogFormatTypeNoFormatting;
            }
            if (NumericFormatType.ParseStatic(formatString))
            {
                return SR.BindingFormattingDialogFormatTypeNumeric;
            }
            else if (CurrencyFormatType.ParseStatic(formatString))
            {
                return SR.BindingFormattingDialogFormatTypeCurrency;
            }
            else if (DateTimeFormatType.ParseStatic(formatString))
            {
                return SR.BindingFormattingDialogFormatTypeDateTime;
            }
            else if (ScientificFormatType.ParseStatic(formatString))
            {
                return SR.BindingFormattingDialogFormatTypeScientific;
            }
            else
            {
                return SR.BindingFormattingDialogFormatTypeCustom;
            }
        }

        protected override bool ProcessMnemonic(char charCode)
        {
            if (IsMnemonic(charCode, formatTypeLabel.Text))
            {
                formatTypeListBox.Focus();
                return true;
            }

            if (IsMnemonic(charCode, nullValueLabel.Text))
            {
                nullValueTextBox.Focus();
                return true;
            }

            int selIndex = formatTypeListBox.SelectedIndex;
            switch (selIndex)
            {
                case NoFormattingIndex:
                    return false;
                case ScientificIndex:
                // FALL THRU
                case CurrencyIndex:
                // FALL THRU
                case NumericIndex:
                    Debug.Assert(decimalPlacesUpDown.Visible);
                    if (IsMnemonic(charCode, secondRowLabel.Text))
                    {
                        decimalPlacesUpDown.Focus();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case DateTimeIndex:
                    Debug.Assert(dateTimeFormatsListBox.Visible);
                    if (IsMnemonic(charCode, secondRowLabel.Text))
                    {
                        dateTimeFormatsListBox.Focus();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case CustomIndex:
                    Debug.Assert(customStringTextBox.Visible);
                    if (IsMnemonic(charCode, secondRowLabel.Text))
                    {
                        customStringTextBox.Focus();
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                default:
                    return false;
            }
        }

        public void ResetFormattingInfo()
        {
            decimalPlacesUpDown.ValueChanged -= new EventHandler(decimalPlacesUpDown_ValueChanged);
            customStringTextBox.TextChanged -= new EventHandler(customStringTextBox_TextChanged);
            dateTimeFormatsListBox.SelectedIndexChanged -= new EventHandler(dateTimeFormatsListBox_SelectedIndexChanged);
            formatTypeListBox.SelectedIndexChanged -= new EventHandler(formatTypeListBox_SelectedIndexChanged);

            decimalPlacesUpDown.Value = 2;
            nullValueTextBox.Text = string.Empty;
            dateTimeFormatsListBox.SelectedIndex = -1;
            formatTypeListBox.SelectedIndex = -1;
            customStringTextBox.Text = string.Empty;

            decimalPlacesUpDown.ValueChanged += new EventHandler(decimalPlacesUpDown_ValueChanged);
            customStringTextBox.TextChanged += new EventHandler(customStringTextBox_TextChanged);
            dateTimeFormatsListBox.SelectedIndexChanged += new EventHandler(dateTimeFormatsListBox_SelectedIndexChanged);
            formatTypeListBox.SelectedIndexChanged += new EventHandler(formatTypeListBox_SelectedIndexChanged);
        }

        private void UpdateControlVisibility(FormatTypeClass formatType)
        {
            if (formatType is null)
            {
                explanationLabel.Visible = false;
                sampleLabel.Visible = false;
                nullValueLabel.Visible = false;
                secondRowLabel.Visible = false;
                nullValueTextBox.Visible = false;
                thirdRowLabel.Visible = false;
                dateTimeFormatsListBox.Visible = false;
                customStringTextBox.Visible = false;
                decimalPlacesUpDown.Visible = false;
                return;
            }

            tableLayoutPanel1.SuspendLayout();
            secondRowLabel.Text = "";

            // process the decimalPlacesLabelVisible
            if (formatType.DropDownVisible)
            {
                secondRowLabel.Text = SR.BindingFormattingDialogDecimalPlaces;
                decimalPlacesUpDown.Visible = true;
            }
            else
            {
                decimalPlacesUpDown.Visible = false;
            }

            // process customFormatLabelVisible
            if (formatType.FormatStringTextBoxVisible)
            {
                secondRowLabel.Text = SR.BindingFormattingDialogCustomFormat;
                thirdRowLabel.Visible = true;
                tableLayoutPanel1.SetColumn(thirdRowLabel, 0);
                tableLayoutPanel1.SetColumnSpan(thirdRowLabel, 2);
                customStringTextBox.Visible = true;

                if (tableLayoutPanel1.Controls.Contains(dateTimeFormatsListBox))
                {
                    tableLayoutPanel1.Controls.Remove(dateTimeFormatsListBox);
                }

                tableLayoutPanel1.Controls.Add(customStringTextBox, 1, 1);
            }
            else
            {
                thirdRowLabel.Visible = false;
                customStringTextBox.Visible = false;
            }

            if (formatType.ListBoxVisible)
            {
                secondRowLabel.Text = SR.BindingFormattingDialogType;

                if (tableLayoutPanel1.Controls.Contains(customStringTextBox))
                {
                    tableLayoutPanel1.Controls.Remove(customStringTextBox);
                }

                dateTimeFormatsListBox.Visible = true;
                tableLayoutPanel1.Controls.Add(dateTimeFormatsListBox, 0, 2);
                tableLayoutPanel1.SetColumn(dateTimeFormatsListBox, 0);
                tableLayoutPanel1.SetColumnSpan(dateTimeFormatsListBox, 2);
            }
            else
            {
                dateTimeFormatsListBox.Visible = false;
            }

            if (secondRowLabel.Text == "")
            {
                secondRowLabel.Visible = false;
            }
            else
            {
                secondRowLabel.Visible = true;
            }

            tableLayoutPanel1.ResumeLayout(true /*performLayout*/);
        }

        private void UpdateCustomStringTextBox()
        {
            customStringTextBox = new TextBox();
            customStringTextBox.AccessibleDescription = SR.BindingFormattingDialogCustomFormatAccessibleDescription;
            customStringTextBox.Margin = new Padding(0, 3, 0, 3);
            customStringTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            customStringTextBox.TabIndex = 3;
            customStringTextBox.TextChanged += new EventHandler(customStringTextBox_TextChanged);
        }

        private void UpdateFormatTypeListBoxHeight()
        {
            // there seems to be a bug in layout because setting
            // the anchor on the list box does not work.
            formatTypeListBox.Height = tableLayoutPanel1.Bottom - formatTypeListBox.Top;
        }

        private void UpdateFormatTypeListBoxItems()
        {
            dateTimeFormatsListBox.SelectedIndexChanged -= new System.EventHandler(dateTimeFormatsListBox_SelectedIndexChanged);
            dateTimeFormatsListBox.Items.Clear();
            dateTimeFormatsListBox.Items.Add(new DateTimeFormatsListBoxItem(s_dateTimeFormatValue, "d"));
            dateTimeFormatsListBox.Items.Add(new DateTimeFormatsListBoxItem(s_dateTimeFormatValue, "D"));
            dateTimeFormatsListBox.Items.Add(new DateTimeFormatsListBoxItem(s_dateTimeFormatValue, "f"));
            dateTimeFormatsListBox.Items.Add(new DateTimeFormatsListBoxItem(s_dateTimeFormatValue, "F"));
            dateTimeFormatsListBox.Items.Add(new DateTimeFormatsListBoxItem(s_dateTimeFormatValue, "g"));
            dateTimeFormatsListBox.Items.Add(new DateTimeFormatsListBoxItem(s_dateTimeFormatValue, "G"));
            dateTimeFormatsListBox.Items.Add(new DateTimeFormatsListBoxItem(s_dateTimeFormatValue, "t"));
            dateTimeFormatsListBox.Items.Add(new DateTimeFormatsListBoxItem(s_dateTimeFormatValue, "T"));
            dateTimeFormatsListBox.Items.Add(new DateTimeFormatsListBoxItem(s_dateTimeFormatValue, "M"));
            dateTimeFormatsListBox.SelectedIndex = 0;
            dateTimeFormatsListBox.SelectedIndexChanged += new System.EventHandler(dateTimeFormatsListBox_SelectedIndexChanged);
        }

        private void UpdateTBLHeight()
        {
            tableLayoutPanel1.SuspendLayout();

            // set up the customStringTextBox
            tableLayoutPanel1.Controls.Add(customStringTextBox, 1, 1);
            customStringTextBox.Visible = false;

            // set the thirdRowLabel
            thirdRowLabel.MaximumSize = new Drawing.Size(tableLayoutPanel1.Width, 0);
            dateTimeFormatsListBox.Visible = false;
            tableLayoutPanel1.SetColumn(thirdRowLabel, 0);
            tableLayoutPanel1.SetColumnSpan(thirdRowLabel, 2);
            thirdRowLabel.AutoSize = true;
            tableLayoutPanel1.ResumeLayout(true /*performLayout*/);

            // Now that PerformLayout set the bounds for the tableLayoutPanel we can use these bounds to specify the tableLayoutPanel minimumSize.
            tableLayoutPanel1.MinimumSize = new Drawing.Size(tableLayoutPanel1.Width, tableLayoutPanel1.Height);
        }

        private void FormatControl_Load(object sender, EventArgs e)
        {
            if (_loaded)
            {
                // we already did the setup work
                return;
            }

            int minWidth, minHeight;
            nullValueLabel.Text = SR.BindingFormattingDialogNullValue;
            minWidth = nullValueLabel.Width;
            minHeight = nullValueLabel.Height;

            secondRowLabel.Text = SR.BindingFormattingDialogDecimalPlaces;
            minWidth = Math.Max(minWidth, secondRowLabel.Width);
            minHeight = Math.Max(minHeight, secondRowLabel.Height);

            secondRowLabel.Text = SR.BindingFormattingDialogCustomFormat;
            minWidth = Math.Max(minWidth, secondRowLabel.Width);
            minHeight = Math.Max(minHeight, secondRowLabel.Height);

            nullValueLabel.MinimumSize = new Drawing.Size(minWidth, minHeight);
            secondRowLabel.MinimumSize = new Drawing.Size(minWidth, minHeight);

            // add items to the list box
            formatTypeListBox.SelectedIndexChanged -= new EventHandler(formatTypeListBox_SelectedIndexChanged);
            formatTypeListBox.Items.Clear();
            formatTypeListBox.Items.Add(new NoFormattingFormatType());
            formatTypeListBox.Items.Add(new NumericFormatType(this));
            formatTypeListBox.Items.Add(new CurrencyFormatType(this));
            formatTypeListBox.Items.Add(new DateTimeFormatType(this));
            formatTypeListBox.Items.Add(new ScientificFormatType(this));
            formatTypeListBox.Items.Add(new CustomFormatType(this));
            formatTypeListBox.SelectedIndex = 0;
            formatTypeListBox.SelectedIndexChanged += new EventHandler(formatTypeListBox_SelectedIndexChanged);

            UpdateCustomStringTextBox();
            UpdateTBLHeight();
            UpdateFormatTypeListBoxHeight();
            UpdateFormatTypeListBoxItems();

            UpdateControlVisibility(formatTypeListBox.SelectedItem as FormatTypeClass);
            sampleLabel.Text = ((formatTypeListBox.SelectedItem) as FormatTypeClass).SampleString;
            explanationLabel.Size = new System.Drawing.Size(formatGroupBox.Width - 10, 30);
            explanationLabel.Text = ((formatTypeListBox.SelectedItem) as FormatTypeClass).TopLabelString;

            Dirty = false;

            FormatControlFinishedLoading();

            _loaded = true;
        }

        // This method tells the FormatStringDialog that the FormatControl is loaded and resized.
        private void FormatControlFinishedLoading()
        {
            FormatStringDialog fsd = null;
            Control ctl = Parent;

            while (ctl != null)
            {
                fsd = ctl as FormatStringDialog;

                if (fsd != null)
                {
                    break;
                }

                ctl = ctl.Parent;
            }

            if (fsd != null)
            {
                fsd.FormatControlFinishedLoading();
            }
        }

        private class DateTimeFormatsListBoxItem
        {
            DateTime value;
            string formatString;
            public DateTimeFormatsListBoxItem(DateTime value, string formatString)
            {
                this.value = value;
                this.formatString = formatString;
            }

            public string FormatString
            {
                get
                {
                    return formatString;
                }
            }

            public override string ToString()
            {
                return value.ToString(formatString, CultureInfo.CurrentCulture);
            }
        }

        internal abstract class FormatTypeClass
        {
            public abstract string TopLabelString { get; }
            public abstract string SampleString { get; }
            public abstract bool DropDownVisible { get; }
            public abstract bool ListBoxVisible { get; }
            public abstract bool FormatStringTextBoxVisible { get; }
            public abstract bool FormatLabelVisible { get; }
            public abstract string FormatString { get; }
            public abstract bool Parse(string formatString);
            public abstract void PushFormatStringIntoFormatType(string formatString);
        }

        private class NoFormattingFormatType : FormatTypeClass
        {
            public override string TopLabelString
            {
                get
                {
                    return SR.BindingFormattingDialogFormatTypeNoFormattingExplanation;
                }
            }
            public override string SampleString
            {
                get
                {
                    return "-1234.5";
                }
            }
            public override bool DropDownVisible
            {
                get
                {
                    return false;
                }
            }
            public override bool ListBoxVisible
            {
                get
                {
                    return false;
                }
            }

            public override bool FormatLabelVisible
            {
                get
                {
                    return false;
                }
            }

            public override string FormatString
            {
                get
                {
                    return "";
                }
            }

            public override bool FormatStringTextBoxVisible
            {
                get
                {
                    return false;
                }
            }

            public override bool Parse(string formatString)
            {
                return false;
            }

            public override void PushFormatStringIntoFormatType(string formatString)
            {
                // nothing to do;
            }

            public override string ToString()
            {
                return SR.BindingFormattingDialogFormatTypeNoFormatting;
            }
        }

        private class NumericFormatType : FormatTypeClass
        {
            FormatControl _owner;

            public NumericFormatType(FormatControl owner)
            {
                _owner = owner;
            }

            public override string TopLabelString
            {
                get
                {
                    return SR.BindingFormattingDialogFormatTypeNumericExplanation;
                }
            }
            public override string SampleString
            {
                get
                {
                    return (-1234.5678).ToString(FormatString, CultureInfo.CurrentCulture);
                }
            }
            public override bool DropDownVisible
            {
                get
                {
                    return true;
                }
            }
            public override bool ListBoxVisible
            {
                get
                {
                    return false;
                }
            }

            public override bool FormatLabelVisible
            {
                get
                {
                    return false;
                }
            }

            public override string FormatString
            {
                get
                {
                    switch ((int)_owner.decimalPlacesUpDown.Value)
                    {
                        case 0:
                            return "N0";
                        case 1:
                            return "N1";
                        case 2:
                            return "N2";
                        case 3:
                            return "N3";
                        case 4:
                            return "N4";
                        case 5:
                            return "N5";
                        case 6:
                            return "N6";
                        default:
                            Debug.Fail("decimalPlacesUpDown should allow only up to 6 digits");
                            return "";
                    }
                }
            }

            public override bool FormatStringTextBoxVisible
            {
                get
                {
                    return false;
                }
            }

            public static bool ParseStatic(string formatString)
            {
                return formatString.Equals("N0") ||
                       formatString.Equals("N1") ||
                       formatString.Equals("N2") ||
                       formatString.Equals("N3") ||
                       formatString.Equals("N4") ||
                       formatString.Equals("N5") ||
                       formatString.Equals("N6");
            }

            public override bool Parse(string formatString)
            {
                return ParseStatic(formatString);
            }

            public override void PushFormatStringIntoFormatType(string formatString)
            {
#if DEBUG
                Debug.Assert(Parse(formatString), "we only push valid strings");
#endif // DEBUG
                if (formatString.Equals("N0"))
                {
                    _owner.decimalPlacesUpDown.Value = 0;
                }
                else if (formatString.Equals("N1"))
                {
                    _owner.decimalPlacesUpDown.Value = 1;
                }
                else if (formatString.Equals("N2"))
                {
                    _owner.decimalPlacesUpDown.Value = 2;
                }
                else if (formatString.Equals("N3"))
                {
                    _owner.decimalPlacesUpDown.Value = 3;
                }
                else if (formatString.Equals("N4"))
                {
                    _owner.decimalPlacesUpDown.Value = 4;
                }
                else if (formatString.Equals("N5"))
                {
                    _owner.decimalPlacesUpDown.Value = 5;
                }
                else if (formatString.Equals("N6"))
                {
                    _owner.decimalPlacesUpDown.Value = 6;
                }
            }

            public override string ToString()
            {
                return SR.BindingFormattingDialogFormatTypeNumeric;
            }
        }

        private class CurrencyFormatType : FormatTypeClass
        {
            FormatControl _owner;

            public CurrencyFormatType(FormatControl owner)
            {
                this._owner = owner;
            }

            public override string TopLabelString
            {
                get
                {
                    return SR.BindingFormattingDialogFormatTypeCurrencyExplanation;
                }
            }

            public override string SampleString
            {
                get
                {
                    return (-1234.5678).ToString(FormatString, CultureInfo.CurrentCulture);
                }
            }

            public override bool DropDownVisible
            {
                get
                {
                    return true;
                }
            }

            public override bool ListBoxVisible
            {
                get
                {
                    return false;
                }
            }

            public override bool FormatLabelVisible
            {
                get
                {
                    return false;
                }
            }

            public override string FormatString
            {
                get
                {
                    switch ((int)_owner.decimalPlacesUpDown.Value)
                    {
                        case 0:
                            return "C0";
                        case 1:
                            return "C1";
                        case 2:
                            return "C2";
                        case 3:
                            return "C3";
                        case 4:
                            return "C4";
                        case 5:
                            return "C5";
                        case 6:
                            return "C6";
                        default:
                            Debug.Fail("decimalPlacesUpDown should allow only up to 6 digits");
                            return "";
                    }
                }
            }

            public override bool FormatStringTextBoxVisible
            {
                get
                {
                    return false;
                }
            }

            public static bool ParseStatic(string formatString)
            {
                return formatString.Equals("C0") ||
                       formatString.Equals("C1") ||
                       formatString.Equals("C2") ||
                       formatString.Equals("C3") ||
                       formatString.Equals("C4") ||
                       formatString.Equals("C5") ||
                       formatString.Equals("C6");
            }

            public override bool Parse(string formatString)
            {
                return ParseStatic(formatString);
            }

            public override void PushFormatStringIntoFormatType(string formatString)
            {
#if DEBUG
                Debug.Assert(Parse(formatString), "we only push valid strings");
#endif // DEBUG
                if (formatString.Equals("C0"))
                {
                    _owner.decimalPlacesUpDown.Value = 0;
                }
                else if (formatString.Equals("C1"))
                {
                    _owner.decimalPlacesUpDown.Value = 1;
                }
                else if (formatString.Equals("C2"))
                {
                    _owner.decimalPlacesUpDown.Value = 2;
                }
                else if (formatString.Equals("C3"))
                {
                    _owner.decimalPlacesUpDown.Value = 3;
                }
                else if (formatString.Equals("C4"))
                {
                    _owner.decimalPlacesUpDown.Value = 4;
                }
                else if (formatString.Equals("C5"))
                {
                    _owner.decimalPlacesUpDown.Value = 5;
                }
                else if (formatString.Equals("C6"))
                {
                    _owner.decimalPlacesUpDown.Value = 6;
                }
            }

            public override string ToString()
            {
                return SR.BindingFormattingDialogFormatTypeCurrency;
            }
        }

        private class DateTimeFormatType : FormatTypeClass
        {
            FormatControl _owner;

            public DateTimeFormatType(FormatControl owner)
            {
                _owner = owner;
            }

            public override string TopLabelString
            {
                get
                {
                    return SR.BindingFormattingDialogFormatTypeDateTimeExplanation;
                }
            }

            public override string SampleString
            {
                get
                {
                    if (_owner.dateTimeFormatsListBox.SelectedItem is null)
                    {
                        return "";
                    }

                    return s_dateTimeFormatValue.ToString(FormatString, CultureInfo.CurrentCulture);
                }
            }

            public override bool DropDownVisible
            {
                get
                {
                    return false;
                }
            }

            public override bool ListBoxVisible
            {
                get
                {
                    return true;
                }
            }

            public override bool FormatLabelVisible
            {
                get
                {
                    return false;
                }
            }

            public override string FormatString
            {
                get
                {
                    DateTimeFormatsListBoxItem item = _owner.dateTimeFormatsListBox.SelectedItem as DateTimeFormatsListBoxItem;
                    return item.FormatString;
                }
            }

            public override bool FormatStringTextBoxVisible
            {
                get
                {
                    return false;
                }
            }

            public static bool ParseStatic(string formatString)
            {
                return formatString.Equals("d") ||
                       formatString.Equals("D") ||
                       formatString.Equals("f") ||
                       formatString.Equals("F") ||
                       formatString.Equals("g") ||
                       formatString.Equals("G") ||
                       formatString.Equals("t") ||
                       formatString.Equals("T") ||
                       formatString.Equals("M");
            }

            public override bool Parse(string formatString)
            {
                return ParseStatic(formatString);
            }

            public override void PushFormatStringIntoFormatType(string formatString)
            {
#if DEBUG
                Debug.Assert(Parse(formatString), "we only push valid strings");
#endif // DEBUG
                int selectedIndex = -1;

                if (formatString.Equals("d"))
                {
                    selectedIndex = 0;
                }
                else if (formatString.Equals("D"))
                {
                    selectedIndex = 1;
                }
                else if (formatString.Equals("f"))
                {
                    selectedIndex = 2;
                }
                else if (formatString.Equals("F"))
                {
                    selectedIndex = 3;
                }
                else if (formatString.Equals("g"))
                {
                    selectedIndex = 4;
                }
                else if (formatString.Equals("G"))
                {
                    selectedIndex = 5;
                }
                else if (formatString.Equals("t"))
                {
                    selectedIndex = 6;
                }
                else if (formatString.Equals("T"))
                {
                    selectedIndex = 7;
                }
                else if (formatString.Equals("M"))
                {
                    selectedIndex = 8;
                }

                _owner.dateTimeFormatsListBox.SelectedIndex = selectedIndex;
            }
            public override string ToString()
            {
                return SR.BindingFormattingDialogFormatTypeDateTime;
            }
        }

        private class ScientificFormatType : FormatTypeClass
        {
            FormatControl _owner;

            public ScientificFormatType(FormatControl owner)
            {
                _owner = owner;
            }

            public override string TopLabelString
            {
                get
                {
                    return SR.BindingFormattingDialogFormatTypeScientificExplanation;
                }
            }

            public override string SampleString
            {
                get
                {
                    return (-1234.5678).ToString(FormatString, CultureInfo.CurrentCulture);
                }
            }

            public override bool DropDownVisible
            {
                get
                {
                    return true;
                }
            }

            public override bool ListBoxVisible
            {
                get
                {
                    return false;
                }
            }

            public override bool FormatLabelVisible
            {
                get
                {
                    return false;
                }
            }

            public override string FormatString
            {
                get
                {
                    switch ((int)_owner.decimalPlacesUpDown.Value)
                    {
                        case 0:
                            return "E0";
                        case 1:
                            return "E1";
                        case 2:
                            return "E2";
                        case 3:
                            return "E3";
                        case 4:
                            return "E4";
                        case 5:
                            return "E5";
                        case 6:
                            return "E6";
                        default:
                            Debug.Fail("decimalPlacesUpDown should allow only up to 6 digits");
                            return "";
                    }
                }
            }

            public override bool FormatStringTextBoxVisible
            {
                get
                {
                    return false;
                }
            }

            public static bool ParseStatic(string formatString)
            {
                return formatString.Equals("E0") ||
                       formatString.Equals("E1") ||
                       formatString.Equals("E2") ||
                       formatString.Equals("E3") ||
                       formatString.Equals("E4") ||
                       formatString.Equals("E5") ||
                       formatString.Equals("E6");
            }

            public override bool Parse(string formatString)
            {
                return ParseStatic(formatString);
            }

            public override void PushFormatStringIntoFormatType(string formatString)
            {
#if DEBUG
                Debug.Assert(Parse(formatString), "we only push valid strings");
#endif // DEBUG
                if (formatString.Equals("E0"))
                {
                    _owner.decimalPlacesUpDown.Value = 0;
                }
                else if (formatString.Equals("E1"))
                {
                    _owner.decimalPlacesUpDown.Value = 1;
                }
                else if (formatString.Equals("E2"))
                {
                    _owner.decimalPlacesUpDown.Value = 2;
                }
                else if (formatString.Equals("E3"))
                {
                    _owner.decimalPlacesUpDown.Value = 3;
                }
                else if (formatString.Equals("E4"))
                {
                    _owner.decimalPlacesUpDown.Value = 4;
                }
                else if (formatString.Equals("E5"))
                {
                    _owner.decimalPlacesUpDown.Value = 5;
                }
                else if (formatString.Equals("E6"))
                {
                    _owner.decimalPlacesUpDown.Value = 6;
                }
            }

            public override string ToString()
            {
                return SR.BindingFormattingDialogFormatTypeScientific;
            }
        }

        private class CustomFormatType : FormatTypeClass
        {
            FormatControl _owner;

            public CustomFormatType(FormatControl owner)
            {
                _owner = owner;
            }

            public override string TopLabelString
            {
                get
                {
                    return SR.BindingFormattingDialogFormatTypeCustomExplanation;
                }
            }

            public override string SampleString
            {
                get
                {
                    string formatString = FormatString;

                    if (string.IsNullOrEmpty(formatString))
                    {
                        return "";
                    }

                    string label = "";

                    // first see if the formatString is one of DateTime's format strings
                    if (DateTimeFormatType.ParseStatic(formatString))
                    {
                        label = s_dateTimeFormatValue.ToString(formatString, CultureInfo.CurrentCulture);
                    }

                    // the formatString was not one of DateTime's strings
                    // Try a double
                    if (label.Equals(""))
                    {
                        try
                        {
                            label = (-1234.5678).ToString(formatString, CultureInfo.CurrentCulture);
                        }
                        catch (FormatException)
                        {
                            label = "";
                        }
                    }

                    // double failed.
                    // Try an Int
                    if (label.Equals(""))
                    {
                        try
                        {
                            label = (-1234).ToString(formatString, CultureInfo.CurrentCulture);
                        }
                        catch (FormatException)
                        {
                            label = "";
                        }
                    }

                    // int failed.
                    // apply the formatString to the dateTime value
                    if (label.Equals(""))
                    {
                        try
                        {
                            label = s_dateTimeFormatValue.ToString(formatString, CultureInfo.CurrentCulture);
                        }
                        catch (FormatException)
                        {
                            label = "";
                        }
                    }

                    if (label.Equals(""))
                    {
                        label = SR.BindingFormattingDialogFormatTypeCustomInvalidFormat;
                    }

                    return label;
                }
            }

            public override bool DropDownVisible
            {
                get
                {
                    return false;
                }
            }

            public override bool ListBoxVisible
            {
                get
                {
                    return false;
                }
            }

            public override bool FormatStringTextBoxVisible
            {
                get
                {
                    return true;
                }
            }
            public override bool FormatLabelVisible
            {
                get
                {
                    return false;
                }
            }
            public override string FormatString
            {
                get
                {
                    return _owner.customStringTextBox.Text;
                }
            }

            public static bool ParseStatic(string formatString)
            {
                // anything goes...
                return true;
            }

            public override bool Parse(string formatString)
            {
                return ParseStatic(formatString);
            }

            public override void PushFormatStringIntoFormatType(string formatString)
            {
                _owner.customStringTextBox.Text = formatString;
            }

            public override string ToString()
            {
                return SR.BindingFormattingDialogFormatTypeCustom;
            }
        }

        private void nullValueTextBox_TextChanged(object sender, EventArgs e)
        {
            Dirty = true;
        }
    }
}
