// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms.Design;

internal partial class FormatControl
{
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
}
