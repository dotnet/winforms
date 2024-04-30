// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;

namespace System.Windows.Forms.Design;

internal class GroupedContextMenuStrip : ContextMenuStrip
{
    private StringCollection? _groupOrdering;
    private ContextMenuStripGroupCollection? _groups;
    private bool _populated;

    public bool Populated
    {
        set => _populated = value;
    }

    public GroupedContextMenuStrip()
    {
    }

    public ContextMenuStripGroupCollection Groups
    {
        get
        {
            _groups ??= new ContextMenuStripGroupCollection();

            return _groups;
        }
    }

    public StringCollection GroupOrdering
    {
        get
        {
            _groupOrdering ??= [];

            return _groupOrdering;
        }
    }

    // merges all the items which are currently in the groups into the items collection.
    public void Populate()
    {
        Items.Clear();
        foreach (string? groupName in GroupOrdering)
        {
            if (groupName is not null && _groups is not null && _groups.ContainsKey(groupName))
            {
                List<ToolStripItem> items = _groups[groupName].Items;

                if (Items.Count > 0 && items.Count > 0)
                {
                    Items.Add(new ToolStripSeparator());
                }

                foreach (ToolStripItem item in items)
                {
                    Items.Add(item);
                }
            }
        }

        _populated = true;
    }

    protected override void OnOpening(CancelEventArgs e)
    {
        SuspendLayout();
        if (!_populated)
        {
            Populate();
        }

        RefreshItems();
        ResumeLayout(true);
        PerformLayout();
        e.Cancel = (Items.Count == 0);
        base.OnOpening(e);
    }

    public virtual void RefreshItems()
    {
    }
}
