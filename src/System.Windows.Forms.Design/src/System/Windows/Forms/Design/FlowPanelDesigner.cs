// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

internal class FlowPanelDesigner : PanelDesigner
{
    public override bool ParticipatesWithSnapLines => false;

    public override IList SnapLines
    {
        get
        {
            ArrayList snapLines = (ArrayList)base.SnapLines;

            //identify all the paddings to remove
            ArrayList paddingsToRemove = new ArrayList(4);
            foreach (SnapLine line in snapLines)
            {
                if (line.Filter is not null && line.Filter.Contains(SnapLine.Padding))
                {
                    paddingsToRemove.Add(line);
                }
            }

            //remove all padding
            foreach (SnapLine line in paddingsToRemove)
            {
                snapLines.Remove(line);
            }

            return snapLines;
        }
    }

    // Skip location adjustment because FlowPanel is going to position this control.
    // Also, Skip z-order adjustment because SendToFront will put the new control at the
    // beginning of the flow instead of the end, plus FlowLayout is already preventing
    // overlap.
    internal override void AddChildControl(Control newChild) => Control.Controls.Add(newChild);

    protected override void OnDragDrop(DragEventArgs de)
    {
        base.OnDragDrop(de);

        SelectionManager sm = GetService(typeof(SelectionManager)) as SelectionManager;
        sm?.Refresh();
    }
}
