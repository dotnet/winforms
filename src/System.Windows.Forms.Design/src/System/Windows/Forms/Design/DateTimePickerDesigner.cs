// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Drawing;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

internal class DateTimePickerDesigner : ControlDesigner
{
    public DateTimePickerDesigner()
    {
        AutoResizeHandles = true;
    }

    /// <summary>
    ///  Adds a baseline SnapLine to the list of SnapLines related to this control.
    /// </summary>
    public override IList SnapLines
    {
        get
        {
            IList<SnapLine> snapLines = SnapLinesInternal;

            // A single text-baseline for the label (and linklabel) control.
            int baseline = DesignerUtils.GetTextBaseline(Control, ContentAlignment.MiddleLeft);

            // DateTimePicker doesn't have an alignment, so we use MiddleLeft and add a fudge-factor.
            baseline += 2;
            snapLines.Add(new SnapLine(SnapLineType.Baseline, baseline, SnapLinePriority.Medium));

            return snapLines.Unwrap();
        }
    }

    /// <summary>
    ///  Retrieves a set of rules concerning the movement capabilities of a component.
    ///  This should be one or more flags from the SelectionRules class. If no designer
    ///  provides rules for a component, the component will not get any UI services.
    /// </summary>
    public override SelectionRules SelectionRules
        => base.SelectionRules & ~(SelectionRules.TopSizeable | SelectionRules.BottomSizeable);
}
