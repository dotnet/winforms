// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

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
}
