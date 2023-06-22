﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Globalization;

namespace System.Windows.Forms.Design;

internal partial class FormatControl
{
    private class NumericFormatType : FormatTypeClass
    {
        private readonly FormatControl _owner;

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
}
