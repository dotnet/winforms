// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Collections;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

/// <summary>
///  <para>
///  Provides a designer that can design components
///  that extend UpDownBase.</para>
/// </summary>
internal class UpDownBaseDesigner : ControlDesigner
{
    public UpDownBaseDesigner()
    {
        AutoResizeHandles = true;
    }

    /// <summary>
    ///  Retrieves a set of rules concerning the movement capabilities of a component.
    ///  This should be one or more flags from the SelectionRules class. If no designer
    ///  provides rules for a component, the component will not get any UI services.
    /// </summary>
    public override SelectionRules SelectionRules
    {
        get
        {
            SelectionRules rules = base.SelectionRules;
            rules &= ~(SelectionRules.TopSizeable | SelectionRules.BottomSizeable);
            return rules;
        }
    }

    /// <summary>
    ///  Adds a baseline SnapLine to the list of SnapLines related to this control.
    /// </summary>
    public override IList SnapLines
    {
        get
        {
            IList<SnapLine> snapLines = SnapLinesInternal;
            int baseline = DesignerUtils.GetTextBaseline(Control, Drawing.ContentAlignment.TopLeft);
            BorderStyle borderStyle = BorderStyle.Fixed3D;
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(Component);
            props.TryGetPropertyDescriptorValue(
                "BorderStyle",
                Component,
                ref borderStyle);

            baseline += borderStyle == BorderStyle.None ? -1 : 2;
            snapLines.Add(new SnapLine(SnapLineType.Baseline, baseline, SnapLinePriority.Medium));
            return snapLines.Unwrap();
        }
    }
}
