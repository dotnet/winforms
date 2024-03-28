// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

internal class TabPageDesigner : PanelDesigner
{
    public override bool CanBeParentedTo(IDesigner parentDesigner) => (parentDesigner is not null && parentDesigner.Component is TabControl);

    public override SelectionRules SelectionRules
    {
        get
        {
            SelectionRules rules = base.SelectionRules;
            Control ctl = Control;

            if (ctl is not null && ctl.Parent is TabControl)
            {
                rules &= ~SelectionRules.AllSizeable;
            }

            return rules;
        }
    }

    internal void OnDragDropInternal(DragEventArgs de) => OnDragDrop(de);

    internal void OnDragEnterInternal(DragEventArgs de) => OnDragEnter(de);

    internal void OnDragLeaveInternal(EventArgs e) => OnDragLeave(e);

    internal void OnDragOverInternal(DragEventArgs e) => OnDragOver(e);

    internal void OnGiveFeedbackInternal(GiveFeedbackEventArgs e) => OnGiveFeedback(e);

    protected override ControlBodyGlyph GetControlGlyph(GlyphSelectionType selectionType)
    {
        // create a new body glyph with empty bounds. This will keep incorrect tab pages from stealing drag/drop messages
        // which are now handled by the TabControlDesigner get the right cursor for this component.
        OnSetCursor();

        Rectangle translatedBounds = Rectangle.Empty;

        // create our glyph, and set its cursor appropriately
        ControlBodyGlyph g = new(translatedBounds, Cursor.Current, Control, this);

        return g;
    }
}
