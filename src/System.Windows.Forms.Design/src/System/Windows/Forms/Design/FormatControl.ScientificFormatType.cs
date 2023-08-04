// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Windows.Forms.Design;

internal partial class FormatControl
{
    private class ScientificFormatType : FormatTypeClass
    {
        private readonly FormatControl _owner;

        public ScientificFormatType(FormatControl owner)
        {
            _owner = owner;
        }

        public override string TopLabelString =>
            SR.BindingFormattingDialogFormatTypeScientificExplanation;

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
                        return string.Empty;
                }
            }
        }

        public override bool FormatStringTextBoxVisible => false;

        public static bool ParseStatic(string formatString) =>
            formatString.Equals("E0") ||
            formatString.Equals("E1") ||
            formatString.Equals("E2") ||
            formatString.Equals("E3") ||
            formatString.Equals("E4") ||
            formatString.Equals("E5") ||
            formatString.Equals("E6");

        public override bool Parse(string formatString) =>
            ParseStatic(formatString);

        public override void PushFormatStringIntoFormatType(string formatString)
        {
            Debug.Assert(Parse(formatString), "we only push valid strings");

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

        public override string ToString() =>
            SR.BindingFormattingDialogFormatTypeScientific;
    }
}
