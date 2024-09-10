// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Design;

internal class NewItemsContextMenuStrip : GroupedContextMenuStrip
{
    private readonly IComponent _component;
    private readonly EventHandler _onClick;
    private readonly bool _convertTo;
    private readonly IServiceProvider _serviceProvider;
    private readonly ToolStripItem _currentItem;

    public NewItemsContextMenuStrip(
        IComponent component,
        ToolStripItem currentItem,
        EventHandler onClick,
        bool convertTo,
        IServiceProvider serviceProvider)
    {
        _component = component;
        _onClick = onClick;
        _convertTo = convertTo;
        _serviceProvider = serviceProvider;
        _currentItem = currentItem;
        if (serviceProvider.TryGetService(out IUIService? uis))
        {
            Renderer = (ToolStripProfessionalRenderer)uis.Styles["VsRenderer"]!;
        }
    }

    protected override void OnOpening(CancelEventArgs e)
    {
        Groups["StandardList"].Items.Clear();
        Groups["CustomList"].Items.Clear();
        Populated = false;

        // plumb through the standard and custom items.
        foreach (ToolStripItem item in ToolStripDesignerUtils.GetStandardItemMenuItems(_component, _onClick, _convertTo))
        {
            Groups["StandardList"].Items.Add(item);
            if (_convertTo)
            {
                if (item is ItemTypeToolStripMenuItem toolItem && _currentItem is not null && toolItem.ItemType == _currentItem.GetType())
                {
                    toolItem.Enabled = false;
                }
            }
        }

        foreach (ToolStripItem item in ToolStripDesignerUtils.GetCustomItemMenuItems(_component, _onClick, _convertTo, _serviceProvider))
        {
            Groups["CustomList"].Items.Add(item);
            if (_convertTo)
            {
                if (item is ItemTypeToolStripMenuItem toolItem && _currentItem is not null && toolItem.ItemType == _currentItem.GetType())
                {
                    toolItem.Enabled = false;
                }
            }
        }

        base.OnOpening(e);
    }

    // We don't want the runtime behavior for this Design Time only DropDown and hence we override
    // the ProcessDialogKey and just close the DropDown instead of running through the runtime implementation
    // for RIGHT/LEFT Keys which ends up setting ModalMenuFilter.
    protected override bool ProcessDialogKey(Keys keyData)
    {
        Keys keyCode = keyData & Keys.KeyCode;
        switch (keyCode)
        {
            case Keys.Left:
            case Keys.Right:
                Close();
                return true;
        }

        return base.ProcessDialogKey(keyData);
    }
}
