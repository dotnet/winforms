// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
            IList<SnapLine> snapLines = SnapLinesInternal;

            // identify and remove all paddings
            for (int i = snapLines.Count - 1; i >= 0; i--)
            {
                if (snapLines[i] is SnapLine line
                    && line.Filter?.Contains(SnapLine.Padding) == true)
                {
                    snapLines.RemoveAt(i);
                }
            }

            return snapLines.Unwrap();
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

        SelectionManager? sm = GetService(typeof(SelectionManager)) as SelectionManager;
        sm?.Refresh();
    }
}
