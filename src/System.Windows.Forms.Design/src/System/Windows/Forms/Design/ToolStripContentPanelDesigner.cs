// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Collections;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal class ToolStripContentPanelDesigner : PanelDesigner
{
    private BaseContextMenuStrip? contextMenu;

    private ContextMenuStrip DesignerContextMenu
    {
        get
        {
            if (contextMenu is null)
            {
                contextMenu = new BaseContextMenuStrip(Component.Site, Component as Component);

                // If multiple Items Selected don't show the custom properties...
                contextMenu.GroupOrdering.Clear();
                contextMenu.GroupOrdering.AddRange(
                [
                    StandardGroups.Code,
                    StandardGroups.Verbs,
                    StandardGroups.Custom,
                    StandardGroups.Selection,
                    StandardGroups.Edit,
                    StandardGroups.Properties
                ]);
                contextMenu.Text = "CustomContextMenu";
            }

            return contextMenu;
        }
    }

    public override IList SnapLines
    {
        get
        {
            // We don't want margin snap lines, so call directly to the internal method.
            ArrayList? snapLines = null;
            AddPaddingSnapLines(ref snapLines);
            return snapLines;
        }
    }

    public override bool CanBeParentedTo(IDesigner parentDesigner)
    {
        return false;
    }

    protected override void OnContextMenu(int x, int y)
    {
        ToolStripContentPanel? panel = Component as ToolStripContentPanel;
        if (panel is not null && panel.Parent is ToolStripContainer)
        {
            DesignerContextMenu.Show(x, y);
        }
        else
        {
            base.OnContextMenu(x, y);
        }
    }

    protected override void PreFilterEvents(IDictionary events)
    {
        base.PreFilterEvents(events);
        EventDescriptor? eventDescriptor;
        string[] noBrowseEvents =
        [
            "BindingContextChanged",
            "ChangeUICues",
            "ClientSizeChanged",
            "EnabledChanged",
            "FontChanged",
            "ForeColorChanged",
            "GiveFeedback",
            "ImeModeChanged",
            "Move",
            "QueryAccessibilityHelp",
            "Validated",
            "Validating",
            "VisibleChanged",
        ];

        for (int i = 0; i < noBrowseEvents.Length; i++)
        {
            eventDescriptor = (EventDescriptor?)events[noBrowseEvents[i]];
            if (eventDescriptor is not null)
            {
                events[noBrowseEvents[i]] = TypeDescriptor.CreateEvent(eventDescriptor.ComponentType, eventDescriptor, BrowsableAttribute.No);
            }
        }
    }
}
