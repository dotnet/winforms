// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

internal class MonthCalendarDesigner : ControlDesigner
{
    public MonthCalendarDesigner()
    {
        AutoResizeHandles = true;
    }

    /// <summary>
    /// Retrieves a set of rules concerning the movement capabilities of a component.
    /// This should be one or more flags from the SelectionRules class. If no designer
    /// provides rules for a component, the component will not get any UI services.
    /// </summary>
    public override SelectionRules SelectionRules
    {
        get
        {
            SelectionRules rules = base.SelectionRules;

            if (Control.Parent is null || !Control.Parent.IsMirrored)
            {
                rules &= ~(SelectionRules.TopSizeable | SelectionRules.LeftSizeable);
            }
            else
            {
                rules &= ~(SelectionRules.TopSizeable | SelectionRules.RightSizeable);
            }

            return rules;
        }
    }
}
