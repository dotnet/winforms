// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Windows.Forms.Design;

internal partial class FormatControl : UserControl
{
    private const int NoFormattingIndex = 0;
    private const int NumericIndex = 1;
    private const int CurrencyIndex = 2;
    private const int DateTimeIndex = 3;
    private const int ScientificIndex = 4;
    private const int CustomIndex = 5;

    private TextBox _customStringTextBox = new();

    // static because we want this value to be the same across a
    // VS session
    private static readonly DateTime s_dateTimeFormatValue = DateTime.Now;
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
            if (formatTypeListBox.SelectedItem is FormatTypeClass formatType)
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
                var formatType = (FormatTypeClass)formatTypeListBox.Items[i];

                if (formatType.ToString().Equals(value))
                {
                    formatTypeListBox.SelectedIndex = i;
                }
            }
        }
    }

    public FormatTypeClass? FormatTypeItem => formatTypeListBox.SelectedItem as FormatTypeClass;

    public string? NullValue
    {
        get
        {
            // If text box is empty or contains just whitespace, return 'null' for the NullValue.
            // Otherwise we end up pushing empty string as the NullValue for every binding, which breaks non-string
            // bindings. This does mean that setting the NullValue to empty string now becomes a code-only scenario
            // for users, but that is an acceptable trade-off.
            string nullValue = nullValueTextBox.Text;

            return string.IsNullOrWhiteSpace(nullValue) ? null : nullValue.Trim();
        }
        set
        {
            nullValueTextBox.TextChanged -= nullValueTextBox_TextChanged;
            nullValueTextBox.Text = value;
            nullValueTextBox.TextChanged += nullValueTextBox_TextChanged;
        }
    }

    public bool NullValueTextBoxEnabled
    {
        set
        {
            nullValueTextBox.Enabled = value;
        }
    }

    private void customStringTextBox_TextChanged(object? sender, EventArgs e)
    {
        var customFormatType = (CustomFormatType?)formatTypeListBox.SelectedItem;
        Debug.Assert(customFormatType is not null);
        sampleLabel.Text = customFormatType.SampleString;
        Dirty = true;
    }

    private void dateTimeFormatsListBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        // recompute the SampleLabel
        var item = (FormatTypeClass?)formatTypeListBox.SelectedItem;
        Debug.Assert(item is not null);
        sampleLabel.Text = item.SampleString;
        Dirty = true;
    }

    private void decimalPlacesUpDown_ValueChanged(object? sender, EventArgs e)
    {
        // update the sample label
        var item = (FormatTypeClass?)formatTypeListBox.SelectedItem;
        Debug.Assert(item is not null);
        sampleLabel.Text = item.SampleString;
        Dirty = true;
    }

    private void formatGroupBox_Enter(object? sender, EventArgs e)
    {
    }

    private void formatTypeListBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        var item = (FormatTypeClass?)formatTypeListBox.SelectedItem;
        Debug.Assert(item is not null);
        UpdateControlVisibility(item);
        sampleLabel.Text = item.SampleString;
        explanationLabel.Text = item.TopLabelString;
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
                Debug.Assert(_customStringTextBox.Visible);
                if (IsMnemonic(charCode, secondRowLabel.Text))
                {
                    _customStringTextBox.Focus();
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
        decimalPlacesUpDown.ValueChanged -= decimalPlacesUpDown_ValueChanged;
        _customStringTextBox.TextChanged -= customStringTextBox_TextChanged;
        dateTimeFormatsListBox.SelectedIndexChanged -= dateTimeFormatsListBox_SelectedIndexChanged;
        formatTypeListBox.SelectedIndexChanged -= formatTypeListBox_SelectedIndexChanged;

        decimalPlacesUpDown.Value = 2;
        nullValueTextBox.Text = string.Empty;
        dateTimeFormatsListBox.SelectedIndex = -1;
        formatTypeListBox.SelectedIndex = -1;
        _customStringTextBox.Text = string.Empty;

        decimalPlacesUpDown.ValueChanged += decimalPlacesUpDown_ValueChanged;
        _customStringTextBox.TextChanged += customStringTextBox_TextChanged;
        dateTimeFormatsListBox.SelectedIndexChanged += dateTimeFormatsListBox_SelectedIndexChanged;
        formatTypeListBox.SelectedIndexChanged += formatTypeListBox_SelectedIndexChanged;
    }

    private void UpdateControlVisibility(FormatTypeClass formatType)
    {
        tableLayoutPanel1.SuspendLayout();
        secondRowLabel.Text = string.Empty;

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
            _customStringTextBox.Visible = true;

            if (tableLayoutPanel1.Controls.Contains(dateTimeFormatsListBox))
            {
                tableLayoutPanel1.Controls.Remove(dateTimeFormatsListBox);
            }

            tableLayoutPanel1.Controls.Add(_customStringTextBox, 1, 1);
        }
        else
        {
            thirdRowLabel.Visible = false;
            _customStringTextBox.Visible = false;
        }

        if (formatType.ListBoxVisible)
        {
            secondRowLabel.Text = SR.BindingFormattingDialogType;

            if (tableLayoutPanel1.Controls.Contains(_customStringTextBox))
            {
                tableLayoutPanel1.Controls.Remove(_customStringTextBox);
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

        secondRowLabel.Visible = secondRowLabel.Text.Length > 0;

        tableLayoutPanel1.ResumeLayout(performLayout: true);
    }

    private void UpdateCustomStringTextBox()
    {
        _customStringTextBox = new TextBox
        {
            AccessibleDescription = SR.BindingFormattingDialogCustomFormatAccessibleDescription,
            Margin = new Padding(0, 3, 0, 3),
            Anchor = AnchorStyles.Left | AnchorStyles.Right,
            TabIndex = 3
        };
        _customStringTextBox.TextChanged += customStringTextBox_TextChanged;
    }

    private void UpdateFormatTypeListBoxHeight()
    {
        // there seems to be a bug in layout because setting
        // the anchor on the list box does not work.
        formatTypeListBox.Height = tableLayoutPanel1.Bottom - formatTypeListBox.Top;
    }

    private void UpdateFormatTypeListBoxItems()
    {
        dateTimeFormatsListBox.SelectedIndexChanged -= dateTimeFormatsListBox_SelectedIndexChanged;
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
        dateTimeFormatsListBox.SelectedIndexChanged += dateTimeFormatsListBox_SelectedIndexChanged;
    }

    private void UpdateTBLHeight()
    {
        tableLayoutPanel1.SuspendLayout();

        // set up the customStringTextBox
        tableLayoutPanel1.Controls.Add(_customStringTextBox, 1, 1);
        _customStringTextBox.Visible = false;

        // set the thirdRowLabel
        thirdRowLabel.MaximumSize = new Drawing.Size(tableLayoutPanel1.Width, 0);
        dateTimeFormatsListBox.Visible = false;
        tableLayoutPanel1.SetColumn(thirdRowLabel, 0);
        tableLayoutPanel1.SetColumnSpan(thirdRowLabel, 2);
        thirdRowLabel.AutoSize = true;
        tableLayoutPanel1.ResumeLayout(performLayout: true);

        // Now that PerformLayout set the bounds for the tableLayoutPanel we can use these bounds
        // to specify the tableLayoutPanel minimumSize.
        tableLayoutPanel1.MinimumSize = new Drawing.Size(tableLayoutPanel1.Width, tableLayoutPanel1.Height);
    }

    private void FormatControl_Load(object? sender, EventArgs e)
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
        formatTypeListBox.SelectedIndexChanged -= formatTypeListBox_SelectedIndexChanged;
        formatTypeListBox.Items.Clear();
        formatTypeListBox.Items.Add(new NoFormattingFormatType());
        formatTypeListBox.Items.Add(new NumericFormatType(this));
        formatTypeListBox.Items.Add(new CurrencyFormatType(this));
        formatTypeListBox.Items.Add(new DateTimeFormatType(this));
        formatTypeListBox.Items.Add(new ScientificFormatType(this));
        formatTypeListBox.Items.Add(new CustomFormatType(this));
        formatTypeListBox.SelectedIndex = 0;
        formatTypeListBox.SelectedIndexChanged += formatTypeListBox_SelectedIndexChanged;

        UpdateCustomStringTextBox();
        UpdateTBLHeight();
        UpdateFormatTypeListBoxHeight();
        UpdateFormatTypeListBoxItems();

        var item = (FormatTypeClass?)formatTypeListBox.SelectedItem;
        Debug.Assert(item is not null);

        UpdateControlVisibility(item);
        sampleLabel.Text = item.SampleString;
        explanationLabel.Size = new Drawing.Size(formatGroupBox.Width - 10, 30);
        explanationLabel.Text = item.TopLabelString;

        Dirty = false;

        FormatControlFinishedLoading();

        _loaded = true;
    }

    // This method tells the FormatStringDialog that the FormatControl is loaded and resized.
    private void FormatControlFinishedLoading()
    {
        for (Control? ctl = Parent; ctl is not null; ctl = ctl.Parent)
        {
            if (ctl is FormatStringDialog fsd)
            {
                fsd.FormatControlFinishedLoading();
                break;
            }
        }
    }

    private class DateTimeFormatsListBoxItem
    {
        private readonly DateTime _value;

        public DateTimeFormatsListBoxItem(DateTime value, string formatString)
        {
            _value = value;
            FormatString = formatString;
        }

        public string FormatString { get; }

        public override string ToString()
        {
            return _value.ToString(FormatString, CultureInfo.CurrentCulture);
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

        public abstract override string ToString();
    }

    private void nullValueTextBox_TextChanged(object? sender, EventArgs e)
    {
        Dirty = true;
    }
}
