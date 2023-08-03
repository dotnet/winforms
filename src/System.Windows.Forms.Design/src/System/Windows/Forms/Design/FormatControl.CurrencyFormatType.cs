// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Windows.Forms.Design;

internal partial class FormatControl
{
    private class CurrencyFormatType : FormatTypeClass
    {
        private readonly FormatControl _owner;

        public CurrencyFormatType(FormatControl owner)
        {
            _owner = owner;
        }

        public override string TopLabelString =>
            SR.BindingFormattingDialogFormatTypeCurrencyExplanation;

        public override string SampleString =>
            (-1234.5678).ToString(FormatString, CultureInfo.CurrentCulture);

        public override bool DropDownVisible => true;

        public override bool ListBoxVisible => false;

        public override bool FormatLabelVisible => false;

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
                        return string.Empty;
                }
            }
        }

        public override bool FormatStringTextBoxVisible => false;

        public static bool ParseStatic(string formatString) =>
            formatString.Equals("C0") ||
            formatString.Equals("C1") ||
            formatString.Equals("C2") ||
            formatString.Equals("C3") ||
            formatString.Equals("C4") ||
            formatString.Equals("C5") ||
            formatString.Equals("C6");

        public override bool Parse(string formatString) =>
            ParseStatic(formatString);

        public override void PushFormatStringIntoFormatType(string formatString)
        {
            Debug.Assert(Parse(formatString), "we only push valid strings");

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

        public override string ToString() =>
            SR.BindingFormattingDialogFormatTypeCurrency;
    }
}
