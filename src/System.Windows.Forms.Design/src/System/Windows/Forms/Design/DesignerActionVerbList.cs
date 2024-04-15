// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

internal class DesignerActionVerbList : DesignerActionList
{
    private readonly DesignerVerb[] _verbs;

    public DesignerActionVerbList(DesignerVerb[] verbs)
        : base(null)
    {
        _verbs = verbs;
    }

    public override bool AutoShow => false;

    public override DesignerActionItemCollection GetSortedActionItems()
    {
        DesignerActionItemCollection items = [];
        for (int i = 0; i < _verbs.Length; i++)
        {
            if (_verbs[i].Visible && _verbs[i].Enabled && _verbs[i].Supported)
            {
                items.Add(new DesignerActionVerbItem(_verbs[i]));
            }
        }

        return items;
    }
}
