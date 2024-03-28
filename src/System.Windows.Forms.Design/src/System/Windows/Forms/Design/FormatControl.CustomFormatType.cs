// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Windows.Forms.Design;

internal partial class FormatControl
{
    private class CustomFormatType : FormatTypeClass
    {
        private readonly FormatControl _owner;

        public CustomFormatType(FormatControl owner)
        {
            _owner = owner;
        }

        public override string TopLabelString =>
            SR.BindingFormattingDialogFormatTypeCustomExplanation;

        public override string SampleString
        {
            get
            {
                string formatString = FormatString;

                if (string.IsNullOrEmpty(formatString))
                {
                    return string.Empty;
                }

                string label = string.Empty;

                // first see if the formatString is one of DateTime's format strings
                if (DateTimeFormatType.ParseStatic(formatString))
                {
                    label = s_dateTimeFormatValue.ToString(formatString, CultureInfo.CurrentCulture);
                }

                // the formatString was not one of DateTime's strings
                // Try a double
                if (label.Equals(string.Empty))
                {
                    try
                    {
                        label = (-1234.5678).ToString(formatString, CultureInfo.CurrentCulture);
                    }
                    catch (FormatException)
                    {
                        label = string.Empty;
                    }
                }

                // double failed.
                // Try an Int
                if (label.Equals(string.Empty))
                {
                    try
                    {
                        label = (-1234).ToString(formatString, CultureInfo.CurrentCulture);
                    }
                    catch (FormatException)
                    {
                        label = string.Empty;
                    }
                }

                // int failed.
                // apply the formatString to the dateTime value
                if (label.Equals(string.Empty))
                {
                    try
                    {
                        label = s_dateTimeFormatValue.ToString(formatString, CultureInfo.CurrentCulture);
                    }
                    catch (FormatException)
                    {
                        label = string.Empty;
                    }
                }

                if (label.Equals(string.Empty))
                {
                    label = SR.BindingFormattingDialogFormatTypeCustomInvalidFormat;
                }

                return label;
            }
        }

        public override bool DropDownVisible => false;

        public override bool ListBoxVisible => false;

        public override bool FormatStringTextBoxVisible => true;

        public override bool FormatLabelVisible => false;

        public override string FormatString => _owner._customStringTextBox.Text;

        public override bool Parse(string formatString) => true;

        public override void PushFormatStringIntoFormatType(string formatString) =>
            _owner._customStringTextBox.Text = formatString;

        public override string ToString() =>
            SR.BindingFormattingDialogFormatTypeCustom;
    }
}
