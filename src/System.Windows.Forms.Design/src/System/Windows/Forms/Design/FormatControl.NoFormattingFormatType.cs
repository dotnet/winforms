// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

internal partial class FormatControl
{
    private class NoFormattingFormatType : FormatTypeClass
    {
        public override string TopLabelString =>
            SR.BindingFormattingDialogFormatTypeNoFormattingExplanation;

        public override string SampleString => "-1234.5";

        public override bool DropDownVisible => false;

        public override bool ListBoxVisible => false;

        public override bool FormatLabelVisible => false;

        public override string FormatString => string.Empty;

        public override bool FormatStringTextBoxVisible => false;

        public override bool Parse(string formatString) => false;

        public override void PushFormatStringIntoFormatType(string formatString)
        {
            // nothing to do;
        }

        public override string ToString() => SR.BindingFormattingDialogFormatTypeNoFormatting;
    }
}
