// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.Windows.Forms.Design;

internal partial class FormatControl
{
    private class DateTimeFormatType : FormatTypeClass
    {
        private readonly FormatControl _owner;

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
                DateTimeFormatsListBoxItem item = (_owner.dateTimeFormatsListBox.SelectedItem as DateTimeFormatsListBoxItem)!;
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
}
